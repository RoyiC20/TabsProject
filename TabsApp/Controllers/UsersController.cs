using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using TabsClassLibrary;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Bcpg;

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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            Console.WriteLine("Login endpoint called."); // Debug: Endpoint entry
            Console.WriteLine($"Email: {loginRequest.Email}, Password: {loginRequest.Password}"); // Debug: Input data

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened."); // Debug: DB connection success

                    string query = "SELECT * FROM users WHERE Email = @Email AND Password = @Password";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", loginRequest.Email);
                    command.Parameters.AddWithValue("@Password", loginRequest.Password); // Ideally hashed passwords

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("User found in database."); // Debug: User found
                            return Ok(new { Message = "Login successful", UserId = reader["UserID"], Name = reader["Name"], Role = reader["Role"] });
                        }
                        else
                        {
                            Console.WriteLine("Invalid credentials."); // Debug: Invalid login attempt
                            return Unauthorized(new { Message = "Invalid credentials" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during login: {ex.Message}"); // Debug: Exception details
                    return StatusCode(500, new { Message = "Internal server error" });
                }
            }
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        // GET: api/Users
        [HttpGet]
        public IActionResult GetUsers()
        {
            Console.WriteLine("GetUsers endpoint called."); // Debug: Endpoint entry

            var users = new List<User>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened."); // Debug: DB connection success

                    string query = "SELECT UserID, Name, Role, Email, Password FROM users"; // Updated query to include Email and Password
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine($"Fetching user: {reader["Name"]}"); // Debug: User data
                        var user = new User
                        {
                            UserID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Role = Enum.TryParse(reader.GetString(2), out UserRole role) ? role : UserRole.Student, // Default to User if invalid role
                            Email = reader.GetString(3), // Fetch Email
                            Password = reader.GetString(4) // Fetch Password
                        };
                        users.Add(user);
                    }

                    reader.Close();
                    Console.WriteLine("Users fetched successfully."); // Debug: Success
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching users: {ex.Message}"); // Debug: Exception details
                    return StatusCode(500, new { Message = "Internal server error" });
                }
            }

            return Ok(users);
        }


        [HttpPost("add")]
        public IActionResult AddUser(User newUser)
        {
            Console.WriteLine("AddUser endpoint called."); // Debug: Endpoint entry

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened."); // Debug: DB connection success

                    // Insert new user into the database
                    string query = "INSERT INTO users (Name, Role, Email, Password) VALUES (@Name, @Role, @Email, @Password)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", newUser.Name);
                    command.Parameters.AddWithValue("@Role", "student");
                    command.Parameters.AddWithValue("@Email", newUser.Email);
                    command.Parameters.AddWithValue("@Password", newUser.Password); // Hash this in production

                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}"); // Debug: Rows inserted

                    if (rowsAffected > 0)
                    {
                        return Ok(new { Message = "User added successfully", User = newUser});
                    }
                    else
                    {
                        return BadRequest(new { Message = "Failed to add user" });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding user: {ex.Message}"); // Debug: Exception
                    return StatusCode(500, new { Message = "Internal server error" });
                }
            }
        }

    }


}

