using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string name_prod { get; set; }
        public int price { get; set; }
        public string description { get; set; }
        public int count { get; set; }
    }
}