using System.ComponentModel.DataAnnotations;

namespace CSCRM.Models
{
    public class Company : BaseEntity
    {
        public string Name { get; set; }
        public string? ContactPerson { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        [EmailAddress]
        public string? Email { get; set; }


    }
}
