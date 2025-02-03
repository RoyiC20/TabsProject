using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TabsClassLibrary
{
    public class LoginResponse
    {
        public int UserID { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty; // User's name
        public string Role { get; set; } = string.Empty; // Role (Teacher or Student)
        public string Email { get; set; } = string.Empty; // User's email address
    }
}
