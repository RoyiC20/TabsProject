namespace BlazorAppForTabs.Services
{
    public class UserService


    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }

        // פונקציה לאיפוס הנתונים במקרה של יציאה מהמערכת
        public void ClearUser()
        {
            UserId = 0;
            UserName = string.Empty;
            Role = string.Empty;
        }
    }


}
