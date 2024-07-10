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
                                                               .Include(c => c.HotelOrders)
                                                               .Include(c => c.TourOrders)
                                                               .Include(c => c.InclusiveOrders)
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
                                                                   HotelOrders = c.HotelOrders.Where(o => !o.IsDeleted).Select(o => new GetHotelOrdersForInvoiceVM
                                                                   {
                                                                       HotelName = o.HotelName,
                                                                       Id = o.Id,
                                                                   }).ToList(),
                                                                   TourOrders = c.TourOrders.Where(o=>!o.IsDeleted).Select(o=> new GetTourOrdersForInvoiceVM
                                                                   {
                                                                       CarType = o.CarType,
                                                                       Date = o.Date,
                                                                       Guide = o.Guide,
                                                                       TourName = o.Tour.Name,
                                                                   }).ToList(),
                                                                   InclusiveOrders = c.InclusiveOrders.Where(o => !o.IsDeleted).Select(o=>new GetInclusiveOrdersForInvoiceVM
                                                                   {
                                                                       InclusiveName = o.InclusiveName,
                                                                   }).ToList()
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

            GetCarVM carType;
            short paxsSize = 0;

            if (clientOrders.TourOrders.Any())
            {
                carType = await _context.CarTypes.Where(c => c.Name == clientOrders.TourOrders.FirstOrDefault().CarType)
                                                  .Select(c => new GetCarVM
                                                  {
                                                      Capacity = c.Capacity,
                                                      Name = c.Name,
                                                      Id = c.Id,
                                                  }).FirstOrDefaultAsync();
                paxsSize += carType.Capacity;
            }


            var hotelOrders = new List<GetHotelOrdersForInvoiceVM>();

            foreach (var hotelOrder in clientOrders.HotelOrders)
            {
                if (!hotelOrders.Any(h => h.HotelName == hotelOrder.HotelName))
                {
                    hotelOrders.Add(hotelOrder);
                }
            }

            clientOrders.HotelOrders = hotelOrders;


            InvoicePageMainVm invoicePageMainVm = new InvoicePageMainVm
            {
                ClientOrders = clientOrders,
                Company = company,
                PaxsSize = paxsSize,
            };
            return invoicePageMainVm;


               
        }
    }
}
