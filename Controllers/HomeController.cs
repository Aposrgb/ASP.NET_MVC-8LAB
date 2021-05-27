using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        public static List<Orders> order = new List<Orders>();
        public static List<OrderDetails> orderD = new List<OrderDetails>();
        public static List<Client> orderC = new List<Client>();
        private static List<int> id_products= new List<int>();
        private static List<int> count_products = new List<int>();
        private static string str = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\abbos\source\repos\WebApplication4\Товары.mdf;Integrated Security=True;Connect Timeout=30";
        public ActionResult Index()
        {
            Session["korz"] = 0;
            Order();
            return View();
        }

        [HttpPost]
        public ActionResult Indexex() {
            Order();
            if (int.Parse(Request["name"]) == 0)
            {
                return View("Index");
            }
            if (!id_products.Contains(int.Parse(Request["Id"])))
            {
                id_products.Add(int.Parse(Request["Id"]));
                count_products.Add(int.Parse(Request["name"]));
            }
            else if(id_products.Contains(int.Parse(Request["Id"]))){
                int j = 0;
                foreach (int i in id_products) {
                    if (i == int.Parse(Request["Id"])) {
                        foreach (Orders s in order) {
                            if (i == s.Id) {
                                if (count_products[j]+int.Parse(Request["name"]) > s.Count) {
                                    count_products[j] = s.Count;
                                    Count();
                                    return View("Index");
                                }
                            }
                        }
                        
                    }
                    j++;
                }
            }
            Count();
            return View("Index");
        }
        public ActionResult OrderList()
        {
            Order();
            Orderes();
            OrderC();
            return View();
        }
        [HttpPost]
        public ActionResult OrderCount(Orders s)
        {
            using (SqlConnection con = new SqlConnection(str))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update Товары set [Кол-во]="+s.Count+" where Id="+s.Id+"", con);
                com.ExecuteNonQuery();
            }
            Order();
            Orderes();
            OrderC();
            return View("OrderList");
        }
        [HttpPost]
        public ActionResult OrderList(Orders s) {
            using (SqlConnection con = new SqlConnection(str))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Insert Into Товары Values('" + s.Name + "','" + s.Price + "','" + s.Description + "','"+s.Count+"')", con);
                com.ExecuteNonQuery();
            }
            Order();
            Orderes();
            OrderC();
            return View();
        }
        public ActionResult AdminPage()
        {
            ViewData["wrong"] = "";
            return View();
        }
        [HttpPost]
        public ActionResult AdminPage(Admin ad)
        {
            if (ModelState.IsValid)
            {
                if (ad.login == "admin" && ad.password == "admin")
                {
                    ViewData["wrong"] = "";
                    Orderes();
                    OrderC();
                    return View("OrderList");
                }
                else {
                    ViewData["wrong"] ="wrong";
                }
            }
                return View();
        }

        public ActionResult BuyProd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BuyProd(Client c)
        {
            if (ModelState.IsValid) {
                using (SqlConnection con = new SqlConnection(str))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("Insert Into Клиент Values('" + c.FIO + "','" + c.Phone + "','" + c.Email + "')", con);
                    com.ExecuteNonQuery();

                    com = new SqlCommand("Select top(1) @@IDENTITY as Id From Клиент", con);
                    SqlDataReader r = com.ExecuteReader();
                    int id = 0;
                    while (r.Read())
                    {
                        id = int.Parse(r["Id"].ToString());
                    }

                    string commands="";
                    string commands1 = "Select * From Товары where";
                    r.Close();
                    if (id_products.Count > 1)
                    {
                        for (int i = 0; i < id_products.Count; i++)
                        {
                            commands+= "Insert Into Заказы Values('" + id + "','" + id_products[i] + "','" + count_products[i] + "') ";
                            if (i == id_products.Count - 1)
                            {
                                commands1 += " id=" + id_products[i];
                            }
                            else {
                                commands1 += " id=" + id_products[i] + " or";
                            }
                            
                        }
                    }
                    else if(id_products.Count==1) {
                        commands = "Insert Into Заказы Values('" + id + "','"+id_products[0]+"','"+count_products[0]+"')";
                        commands1 += " id=" + id_products[0];
                    }
                    SqlCommand com1 = new SqlCommand(commands, con);
                    com1.ExecuteNonQuery();

                    SqlCommand com2 = new SqlCommand(commands1, con);
                    SqlDataReader r1 = com2.ExecuteReader();
                    List<Product> prod = new List<Product>();

                    while (r1.Read()) {
                        Product p = new Product();
                        p.Id = int.Parse(r1["Id"].ToString());
                        p.count =int.Parse(r1["Кол-во"].ToString());
                        prod.Add(p);
                    }
                    r1.Close();
                    for (int i = 0; i < prod.Count; i++) {
                        for (int j = 0; j < prod.Count; j++) {
                            if (prod[i].Id == id_products[j]) {
                                prod[i].count = prod[i].count - count_products[j];
                            }
                        }
                        
                    }
                    string commands2 = "";
                    if (prod.Count > 1)
                    {
                        for (int l = 0; l < prod.Count; l++)
                        {
                            commands2 += "Update Товары Set [Кол-во]=" + prod[l].count + " where Id = " + prod[l].Id + "";
                        }
                    }
                    else {
                        commands2 += "Update Товары Set [Кол-во]=" + prod[0].count + " where Id = " + prod[0].Id + "";
                    }
                    SqlCommand com3 = new SqlCommand(commands2,con);
                    com3.ExecuteNonQuery();

                }
                id_products=new List<int>();//освобождение от корзины после оформления
                count_products = new List<int>();//
                Index();
                return View("Index");
            }
            return View();
        }
        private void Count()
        {
            int res = 0;
            foreach (int i in count_products)
            {
                res += i;
            }
            Session["korz"] = res;
        }
            
        
        private void Order()
        {
            order = new List<Orders>();
            using (SqlConnection con = new SqlConnection(str))
            {
                SqlCommand com = new SqlCommand("Select * From Товары", con);
                con.Open();
                SqlDataReader r = com.ExecuteReader();
                while (r.Read())
                {
                    Orders s = new Orders();
                    s.Id = (int)r["id"];
                    s.Price = (int)r["Цена"];
                    s.Name = (string)r["Имя_Товара"];
                    s.Description = (string)r["Описание"];
                    s.Count = (int)r["Кол-во"];
                    order.Add(s);
                }
            }
        }
        private void Orderes()
        {
            orderD = new List<OrderDetails>();
            using (SqlConnection con = new SqlConnection(str))
            {
                SqlCommand com = new SqlCommand("Select * From Заказы", con);
                con.Open();
                SqlDataReader r = com.ExecuteReader();
                while (r.Read())
                {
                    OrderDetails s = new OrderDetails();
                    s.id_client = (int)r["id_клиента"];
                    s.id_product = (int)r["id_продукта"];
                    s.count = (int)r["Количество_продуктов"];
                    orderD.Add(s);
                }
            }
        }
        private void OrderC()
        {
            orderC = new List<Client>();
            using (SqlConnection con = new SqlConnection(str))
            {
                SqlCommand com = new SqlCommand("Select * From Клиент", con);
                con.Open();
                SqlDataReader r = com.ExecuteReader();
                while (r.Read())
                {
                    Client s = new Client();
                    s.FIO = (string)r["ФИО"];
                    s.Phone = (int)r["Телефон"];
                    s.Email = (string)r["Почта"];
                    orderC.Add(s);
                }
            }
        }
    }
}