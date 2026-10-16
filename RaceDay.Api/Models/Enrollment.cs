using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    // Links a Participant to the Category they entered.
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        // Foreign key to Users (the Participant)
        public int ParticipantId { get; set; }

        // Foreign key to Categories
        public int CategoryId { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        // Confirmed, Pending or Cancelled
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        // Navigation properties
        public User? Participant { get; set; }
        public Category? Category { get; set; }
        public Result? Result { get; set; }
    }
}