namespace BlazorAppForTabs.Services
{
    public class UserService


    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }

        // פונקציה לאיפוס הנתונים במקרה של יציאה מהמערכת

        public event Action? OnUserChanged;

        public void SetUser(int userId, string userName, string role)
        {
            UserId = userId;
            UserName = userName;
            Role = role;
            OnUserChanged?.Invoke(); 
        }

        public void ClearUser()
        {
            UserId = 0;
            UserName = string.Empty;
            Role = string.Empty;
        }


        public bool IsTeacher()
        {
            if (Role == "Teacher")
            {
                return true;
            }
            return false;
        }

        public bool IsAdmin()
        {
            if(Role == "Admin")
            {
                return true;

            }
            return false;
        }

    }


}
