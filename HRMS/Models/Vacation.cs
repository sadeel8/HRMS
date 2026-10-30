using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    public class Vacation
    {
        public long Id { get; set; }

        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public Employee? Employee { get; set; } // Navigation Property

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [ForeignKey("Lookup")]
        public long TypeId { get; set; }
        public Lookup? Type { get; set; } // Navigation Property

        public string? Notes { get; set; }
    }
}
