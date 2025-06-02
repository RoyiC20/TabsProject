
namespace TabsClassLibrary
{
    public class Comment
    {
        public int CommentID { get; set; }
        public int SongID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } = "";


        public Song? Song { get; set; } 
        public User? User { get; set; } 

    }

}
