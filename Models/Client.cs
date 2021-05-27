using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Client
    {
        [Required(ErrorMessage = "Пожалуйста, введите свое ФИО")]
        public string FIO { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите свой телефон")]
        public int Phone { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите Email")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Неверный email")]
        public string Email { get; set; }
    }
}