namespace TabsClassLibrary
{
    public class User
    {
        public int UserID { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty; // User's name
        public string Role { get; set; } = string.Empty; // Role (Teacher or Student)
    }
}
