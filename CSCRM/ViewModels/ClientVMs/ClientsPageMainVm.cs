using CSCRM.ViewModels.CompanyVMs;

namespace CSCRM.ViewModels.ClientVMs
{
    public class ClientsPageMainVm
    {
       public List<GetClientVM> Clients { get; set; } = new List<GetClientVM>();
       public List<string> CompanyNames { get; set; } = new List<string>();
        public List<string> CarTypes { get; set; } = new List<string>();
    }
}
