using System.ComponentModel.DataAnnotations;

namespace CSCRM.ViewModels.AccountVMs
{
    public class SignInVM
    {
        [Required]        
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
