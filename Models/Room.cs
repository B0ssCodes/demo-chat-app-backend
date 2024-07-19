using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
    