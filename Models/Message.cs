using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User {get;set;}

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public virtual Room Room {get;set;}
    }
}
