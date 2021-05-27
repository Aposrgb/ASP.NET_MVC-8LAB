using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class OrderDetails
    {
        public int id_client { get; set; }
        public int id_product { get; set; }
        public int count { get; set; }
    }
}