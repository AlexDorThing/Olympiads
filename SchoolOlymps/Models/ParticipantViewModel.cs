using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolOlymps.Models
{
    public class ParticipantViewModel
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public double Points { get; set; }  // Теперь есть setter!
    }
}
