namespace CSCRM.ViewModels.ClientVMs
{
    public class EditClientInfoPageMainVM
    {
        public EditClientInfoVM ClientForUpdate { get; set; } = new EditClientInfoVM();
        public List<string> CompanyNames { get; set; } = new List<string>();

        public List<string> CarTypes { get; set; } = new List<string>();
    }
}
