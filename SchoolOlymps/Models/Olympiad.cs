using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolOlymps.Models
{
    public class Olympiad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; } = null!;

        [Required]
        public double MaxPoints { get; set; }

        public int OrganizerId { get; set; }

        [ForeignKey("OrganizerId")]
        public User Organizer { get; set; } = null!;

        public ICollection<OlympiadParticipant> Participants { get; set; } = new List<OlympiadParticipant>();
    }
}
