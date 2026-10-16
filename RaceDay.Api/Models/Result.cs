using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models
{
    // The finish result for one enrollment (one enrollment = one result).
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        // Foreign key to Enrollments
        public int EnrollmentId { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        // Finished, DNF, DNS or Pending
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [MaxLength(200)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Enrollment? Enrollment { get; set; }
    }
}