using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    // A race created by an Organizer (for example "Johannesburg City Marathon").
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Distance { get; set; } = string.Empty;

        // Running, Walking or Cycling
        [Required]
        [MaxLength(50)]
        public string EventType { get; set; } = string.Empty;

        // Foreign key to Users (the Organizer who created the event)
        public int OrganizerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public User? Organizer { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}