using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolOlymps.Models
{
    public class OlympiadParticipant
    {
        [Key]
        public int Id { get; set; }

        public int OlympiadId { get; set; }

        [ForeignKey("OlympiadId")]
        public Olympiad Olympiad { get; set; } = null!;

        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public User Student { get; set; } = null!;

        [Required]
        public double PointsEarned { get; set; }
    }
}
