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
            Console.WriteLine("Login endpoint called.");
            Console.WriteLine($"Email: {loginRequest.Email}, Password: {loginRequest.Password}");

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened.");

                    string query = "SELECT * FROM users WHERE Email = @Email";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", loginRequest.Email);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // שליפת הנתונים
                            int userId = reader.GetInt32("UserID");
                            string name = reader.GetString("Name");
                            string role = reader.GetString("Role");
                            string email = reader.GetString("Email");
                            string hashedPassword = reader.GetString("Password");

                            // יצירת אובייקט משתמש זמני לבדיקה
                            var user = new User { Email = email };

                            // בדיקת סיסמה
                            var hasher = new PasswordHasher<User>();
                            var result = hasher.VerifyHashedPassword(user, hashedPassword, loginRequest.Password);

                            if (result == PasswordVerificationResult.Success)
                            {
                                Console.WriteLine("Password matched. Login successful.");
                                return Ok(new LoginResponse
                                {
                                    UserID = userId,
                                    Name = name,
                                    Role = role,
                                    Email = email
                                });
                            }
                            else
                            {
                                Console.WriteLine("Password mismatch.");
                                return Unauthorized(new { Message = "Invalid credentials" });
                            }
                        }
                        else
                        {
                            Console.WriteLine("User not found.");
                            return Unauthorized(new { Message = "Invalid credentials" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during login: {ex.Message}");
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
        public IActionResult AddUser([FromBody] User newUser)
        {
            Console.WriteLine("AddUser endpoint called.");

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                try
                {
                    connection.Open();

                    var hasher = new PasswordHasher<User>();
                    string hashedPassword = hasher.HashPassword(newUser, newUser.Password);

                    // השאילתה הנכונה
                    string query = "INSERT INTO users (Name, Role, Email, Password) VALUES (@Name, @Role, @Email, @Password)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", newUser.Name);
                    command.Parameters.AddWithValue("@Role", newUser.Role.ToString());
                    command.Parameters.AddWithValue("@Email", newUser.Email);
                    command.Parameters.AddWithValue("@Password", hashedPassword); // ✅ משתמש ב־hash הנכון

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        int userId = (int)command.LastInsertedId;

                        return Ok(new LoginResponse
                        {
                            UserID = userId,
                            Name = newUser.Name,
                            Role = newUser.Role.ToString(),
                            Email = newUser.Email
                        });
                    }
                    else
                    {
                        return BadRequest(new { Message = "Failed to add user" });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding user: {ex.Message}");
                    return StatusCode(500, new { Message = "Internal server error" });
                }
            }
        }




        [HttpPut("{id}/password")]
        public IActionResult UpdatePassword(int id, [FromBody] PasswordUpdateModel model)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            var cmd = new MySqlCommand("UPDATE users SET Password = @Password WHERE UserID = @UserID", connection);
            cmd.Parameters.AddWithValue("@Password", model.Password);
            cmd.Parameters.AddWithValue("@UserID", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows > 0)
                return Ok();
            else
                return NotFound();
        }

        public class PasswordUpdateModel
        {
            public string Password { get; set; }
        }



        [HttpGet("exists")]
        public IActionResult CheckEmailExists([FromQuery] string email)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            var cmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE Email = @Email", connection);
            cmd.Parameters.AddWithValue("@Email", email);

            long count = (long)cmd.ExecuteScalar();
            return Ok(count > 0);
        }




        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            var cmd = new MySqlCommand("UPDATE users SET Name = @Name, Email = @Email, Role = @Role WHERE UserID = @UserID", connection);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Role", user.Role.ToString());
            cmd.Parameters.AddWithValue("@UserID", id);


            int rows = cmd.ExecuteNonQuery();
            if (rows > 0)
                return Ok(user);
            else
                return NotFound();
        }




        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                using var connection = _databaseService.GetConnection();
                connection.Open();

                string query = "DELETE FROM users WHERE UserID = @id";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return Ok(new { Message = "User deleted successfully" });
                }
                else
                {
                    return NotFound(new { Message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("שגיאה במחיקת משתמש: " + ex.Message);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }



    }


}

