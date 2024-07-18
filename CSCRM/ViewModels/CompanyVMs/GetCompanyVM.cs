using System.ComponentModel.DataAnnotations;

namespace CSCRM.ViewModels.CompanyVMs
{
    public class GetCompanyVM
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }=string.Empty;
        public string? ContactPerson { get; set; }
        public string? Address { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
    }
}
