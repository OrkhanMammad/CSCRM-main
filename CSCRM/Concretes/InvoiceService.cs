using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.ViewModels.CarVMs;
using CSCRM.ViewModels.ClientOrdersVM;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.InvoiceVMs;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class InvoiceService : IInvoiceService
    {
        readonly AppDbContext _context;
        public InvoiceService(AppDbContext context)
        {
                _context = context;
        }

        public async Task<InvoicePageMainVm> GetInvoiceAsync(int clientId)
        {
            GetClientOrdersForInvoiceVM clientOrders = await _context.Clients                                                               
                                                               .Select(c => new GetClientOrdersForInvoiceVM
                                                               {
                                                                   ClientId = c.Id,
                                                                   InvCode = c.InvCode,
                                                                   Name = c.Name,
                                                                   Surname = c.Surname,
                                                                   ArrivalTime = c.ArrivalTime,
                                                                   ArrivalFlight = c.ArrivalFlight,
                                                                   DepartureTime = c.DepartureTime,
                                                                   DepartureFlight = c.DepartureFlight,
                                                                   CompanyName = c.Company,
                                                                   SalesAmount = c.SalesAmount,
                                                                   PaxSize = c.PaxSize,    
                                                               }).FirstOrDefaultAsync(co=> co.ClientId == clientId);

            GetCompanyVM company = await _context.Companies.Where(c => !c.IsDeleted && c.Name == clientOrders.CompanyName)
                                                           .Select(c => new GetCompanyVM
                                                           {
                                                               Name = c.Name,
                                                               Address = c.Address,
                                                               Email = c.Email,
                                                               Id = c.Id,
                                                               Phone = c.Phone,
                                                           }).FirstOrDefaultAsync();

           


            var hotelOrders = new List<GetHotelOrdersForInvoiceVM>();                       

            InvoicePageMainVm invoicePageMainVm = new InvoicePageMainVm
            {
                ClientOrders = clientOrders,
                Company = company,

            };
            return invoicePageMainVm;


               
        }
    }
}
