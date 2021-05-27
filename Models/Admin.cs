using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Admin
    {
        [Required(ErrorMessage = "Пожалуйста, введите Login")]
        public string login { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите свое Password")]
        public string password { get; set; }
    }
}