using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CompanyController:Controller
    {
        //наверное хорошим тоном будет вынести connectionString в appsetting, но мои попытки не увенчались успехом
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Qulix;Integrated Security=True;MultipleActiveResultSets=true";

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckName(string name) // метод для валидации, проверяет уникально ли название компании
        {
            string sqlExpression = String.Format("SELECT Name FROM Company Where Name = '{0}'", name);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection); // отправляет SELECT запрос, если ничего не приходит, то название уникально
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    return Json(false);
                    
                }
                return Json(true);
            }
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckId(int id) // метод для валидации, проверяет уникален ли id компании
        {
            string sqlExpression = String.Format("SELECT Id FROM Company Where Id = '{0}'", id);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    return Json(false);

                }
                return Json(true);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CompanyList() // список компаний
        {
            string sqlExpression = "SELECT * FROM Company";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                List<Company> companies = new List<Company>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Company company = new Company(); //Отправляет SELECT запрос, затем мы считываем значения полей
                        company.Id = reader.GetInt32(0);
                        company.Name = reader.GetString(1);
                        string currentCompanySelect = string.Format("SELECT COUNT(Company) FROM Users WHERE Company = '{0}'", company.Name); 
                        SqlCommand selectCount = new SqlCommand(currentCompanySelect, connection); //так как с ADO я раньше не работал, я не могу оценить правильность данного подхода
                        company.Form = reader.GetString(2);                                                                           //мне кажется, данное решение неудачно
                        company.Size = selectCount.ExecuteScalar();
                        companies.Add(company);   //формирует компанию и добавляем ее в list,для того, чтобы передать во View                     
                    }
                }
                return View(companies);
            }
        }

        [HttpGet]
        public IActionResult AddCompany()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany(Company company) //Метод создания компании
        {
            string sqlExpression = String.Format("INSERT INTO Company (Name, Form, Id) VALUES ('{0}', '{1}',{2})", company.Name,company.Form,company.Id);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery(); //отправляем SELECT запрос, данные на форме проходят валидацию
            }
            return RedirectToAction("CompanyList", "Company");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id) //метод удаления компании, принимает id удаляемой компании,по нему находит записть и удаляет ее
        {
            string sqlExpression = String.Format("DELETE  FROM Company WHERE Id= {0}", id);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            return RedirectToAction("CompanyList", "Company");
        }

        [HttpGet]
        public IActionResult UpdateCompany()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompany(Company company) //метод для обновления записи
        {
            string sqlExpression = String.Format("UPDATE Company SET Name = '{0}', Form = '{1}' WHERE Id = {2}", company.Name,company.Form,company.Id);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            return RedirectToAction("CompanyList", "Company");
        }

    }
}
