using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        //наверное хорошим тоном будет вынести connectionString в appsetting, но мои попытки не увенчались успехом
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Qulix;Integrated Security=True;MultipleActiveResultSets=true";

        [HttpGet]
        public async Task<IActionResult> UserList() //список работников
        {
            string sqlExpression = "SELECT * FROM Users";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                List<User> users = new List<User>();

                if (reader.HasRows)
                {
                    while (reader.Read())           // метод отправляет SELECT запрос, считывает данные,                                                      
                    {                                            //формирует User,затем добавляем его в массив для передачит во View
                        User user = new User();
                        user.Id = reader.GetInt32(0);
                        user.Surname = reader.GetString(1); 
                        user.Name = reader.GetString(2);
                        user.Patronymic = reader.GetString(3);
                        user.Date = reader.GetDateTime(4);
                        user.Position = reader.GetString(5);
                        user.Company = reader.GetString(6);
                        users.Add(user);
                    }
                }
                return View(users);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddUser() 
        {
            string sqlExpression = "SELECT * FROM Company";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection); //тут неудачное решение, я получаю список всех доступных компаний для DropDownListFor во View
                                                                                                                                      
                SqlDataReader reader = await command.ExecuteReaderAsync();
                List<Company> companyNames = new List<Company>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Company company = new Company();
                        company.Id = reader.GetInt32(0);
                        company.Name = reader.GetString(1);
                        company.Form = reader.GetString(2);
                        companyNames.Add(company);
                    }

                }
                ViewBag.companyNames = new SelectList(companyNames, "Id", "Name"); // тут я передаю эти компании,с помощью ViewBag
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user) //метод добавляет запись в бд,
        {
            
            string sqlExpressionName = String.Format("SELECT Name FROM Company Where Id= {0}", user.Company); // к сожалению я не смог разобраться, как сделать так, 
                                                                                                                                                                                           //чтобы DropDownListFor вернул мне название компании, вместо ее id, по этому мне приходится делать еще один запрос к бд, 
                                                                                                                                                                                           //это решение я считаю неудачным, но, оно работает


            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                await connection.OpenAsync();
                
                SqlCommand command2 = new SqlCommand(sqlExpressionName, connection);
                SqlDataReader reader = await command2.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user.Company = reader.GetString(0);
                    }
                }
                string sqlExpression = String.Format("INSERT INTO Users (Surname, Name, Patronymic, Date, Position, Company) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", user.Surname, user.Name, user.Patronymic, user.Date, user.Position, user.Company);
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            return RedirectToAction("UserList", "User");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id) //удаление записи пользователя из бд
        {
            string sqlExpression = String.Format("DELETE  FROM Users WHERE Id= {0}", id);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            return RedirectToAction("UserList", "User");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser() //обновление пользователя
        {
            string sqlExpression = "SELECT * FROM Company";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                List<Company> companyNames = new List<Company>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Company company = new Company();
                        company.Id = reader.GetInt32(0);
                        company.Name = reader.GetString(1);
                        company.Form = reader.GetString(2);
                        companyNames.Add(company);
                    }

                }
                ViewBag.companyNames = new SelectList(companyNames, "Id", "Name");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user)
        {
            string sqlExpressionName = String.Format("SELECT Name FROM Company Where Id= {0}", user.Company);


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                SqlCommand command2 = new SqlCommand(sqlExpressionName, connection);
                SqlDataReader reader = await command2.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user.Company = reader.GetString(0);
                    }
                }
                string sqlExpression = String.Format("UPDATE Users SET Surname = '{0}', Name = '{1}', Patronymic= '{2}', Date= '{3}', Position= '{4}', Company = '{5}' WHERE Id={6}", user.Surname, user.Name, user.Patronymic, user.Date, user.Position, user.Company, user.Id);
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            return RedirectToAction("UserList", "User");
        }

    }
}
