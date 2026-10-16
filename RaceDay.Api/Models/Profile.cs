using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    // Extra (optional) information about a user.
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }

        // Foreign key to Users (one user = one profile)
        public int UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? ProfilePictureUrl { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}