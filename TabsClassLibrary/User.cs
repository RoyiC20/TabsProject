namespace TabsClassLibrary
{
    public class User
    {
        public int UserID { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty; // User's name
        public UserRole Role { get; set; } = UserRole.Student; // Role (Teacher or Student or Admin)
        public string Email { get; set; } = string.Empty; // User's email address
        public string Password { get; set; } = string.Empty; // User's hashed password
    }
}