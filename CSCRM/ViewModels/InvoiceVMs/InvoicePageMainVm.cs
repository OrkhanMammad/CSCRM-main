using CSCRM.ViewModels.CompanyVMs;

namespace CSCRM.ViewModels.InvoiceVMs
{
    public class InvoicePageMainVm
    {
        public GetClientOrdersForInvoiceVM ClientOrders {  get; set; } = new GetClientOrdersForInvoiceVM();
        public GetCompanyVM Company { get; set; } = new GetCompanyVM();

    }
}
