namespace GuidesFusion360Server.Models
{
    public class User
    {
        public int Id { get; set; }
        
        public string Email { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string StudyGroup { get; set; }
        
        public string Access { get; set; }
        
        public byte[] PasswordHash { get; set; }
        
        public byte[] PasswordSalt { get; set; }
    }
}
