using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using TabsClassLibrary;
using System.Collections.Generic;

namespace TabsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public UsersController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Users
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new List<User>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = "SELECT UserID, Name, Role, Email, Password FROM users"; // Updated query to include Email and Password
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var user = new User
                    {
                        UserID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Role = reader.GetString(2),
                        Email = reader.GetString(3), // Fetch Email
                        Password = reader.GetString(4) // Fetch Password
                    };
                    users.Add(user);
                }

                reader.Close();
            }

            return Ok(users);
        }
    }
}
