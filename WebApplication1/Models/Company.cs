using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Company
    {
        [Required(ErrorMessage = "Введите Идентификатор")] // я использую Required и Remote для валидации
        [Remote(action: "CheckId", controller: "Company", ErrorMessage = "Такой Id уже существует")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите Название")]
        [Remote(action: "CheckName", controller: "Company", ErrorMessage = "Компания с таким названием занята")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите Организационно-правовую форму ")]
        public string Form { get; set; }
        public object Size { get; set; }
    }
}
