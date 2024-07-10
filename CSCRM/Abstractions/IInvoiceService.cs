using CSCRM.ViewModels.InvoiceVMs;

namespace CSCRM.Abstractions
{
    public interface IInvoiceService
    {
        Task<InvoicePageMainVm> GetInvoiceAsync(int clientId);
       
    }
}
