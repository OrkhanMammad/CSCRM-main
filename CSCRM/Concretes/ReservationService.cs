using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.ClientOrdersVM;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class ReservationService : IReservationService
    {
        readonly AppDbContext _context;
        public ReservationService(AppDbContext context)
        {
            _context = context;
        }
        public Task AddConfirmationNumberToOrderAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse> GetHotelOrdersAsync()
        {
            try
            {
                List<GetClientOrdersVM> clientHotelOrders = await _context.Clients.Where(c => !c.IsDeleted)
                     .Include(c => c.HotelOrders)
                     .Select(c => new GetClientOrdersVM
                     {
                         Id = c.Id,
                         InvCode = c.InvCode,
                         MailCode = c.MailCode,
                         Name = c.Name,
                         Surname = c.Surname,
                         HotelOrders = c.HotelOrders.Where(o => !o.IsDeleted).Select(o => new GetHotelOrdersVM
                         {
                             Id = o.Id,
                             ClientId = o.ClientId,
                             HotelName = o.HotelName,
                             RoomType = o.RoomType,
                             RoomCount = o.RoomCount,
                             Days = o.Days,
                             DateFrom = o.DateFrom,
                             DateTo = o.DateTo,
                             ConfirmationNumbers = o.ConfirmationNumbers.Select(co => co.Number).ToList(),

                         }).ToList(),
                     }).ToListAsync();

                if (!clientHotelOrders.Any())
                {
                    return new BaseResponse
                    {
                        Data = new List<GetClientOrdersVM>(),
                        Message = "Hotel Orders Could Not Found",
                        StatusCode = "404",
                        Success = true
                    };
                }

                else
                {
                    return new BaseResponse
                    {
                        Data = clientHotelOrders,
                        StatusCode = "200",
                        Success = true
                    };

                }
            }
            catch (Exception ex) 
            {
                return new BaseResponse
                {
                    Data = new List<GetClientOrdersVM>(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = true
                };


            }
    }
    }
}
