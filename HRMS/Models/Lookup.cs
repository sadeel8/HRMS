using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class Lookup
    {
        public long Id { get; set; }
        public int MajorCode { get; set; }
        public int MinorCode { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

    }
}
