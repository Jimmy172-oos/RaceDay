using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    // A race option inside an event (for example "Half Marathon (21km)").
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        // Foreign key to Events
        public int EventId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Event? Event { get; set; }
    }
}