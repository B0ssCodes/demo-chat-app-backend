using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }    
        
        public string Description { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }


    }
}
