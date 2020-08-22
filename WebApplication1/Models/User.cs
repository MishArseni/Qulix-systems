using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите Фамилию")] // я использую Required  для валидации
        public string Surname { get; set; }
        [Required(ErrorMessage = "Введите Имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Введите Отчество")]
        public string Patronymic { get; set; }
        [Required(ErrorMessage = "Введите Дату")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Введите должность")]
        public string Position { get; set; }
        [Required(ErrorMessage = "Выберите компанию")]
        public string Company { get; set; }

    }
}
