using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.ClientOrdersVM;
using CSCRM.ViewModels.ReservationVMs;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace CSCRM.Concretes
{
    public class ReservationService : IReservationService
    {
        readonly AppDbContext _context;
        public ReservationService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<BaseResponse> EditReservationAsync(EditReservationDTO dto)
        {

            try
            {
                List<HotelConfirmationNumber> ConfirmationNums = await _context.HotelConfirmationNumbers.Where(hc => hc.HotelOrderId == dto.ReservationId).ToListAsync();
                if (ConfirmationNums.Any())
                {
                    _context.HotelConfirmationNumbers.RemoveRange(ConfirmationNums);                   
                }
                
                    List<HotelConfirmationNumber> NewConfirmationNumbers = new List<HotelConfirmationNumber>();
                    foreach (string ConfNum in dto.ConfirmationNumbers)
                    {
                        NewConfirmationNumbers.Add(new HotelConfirmationNumber { HotelOrderId = dto.ReservationId, Number = ConfNum });
                    }

                    await _context.HotelConfirmationNumbers.AddRangeAsync(NewConfirmationNumbers);
                    await _context.SaveChangesAsync();               

                GetReservationVM reservation = await _context.HotelOrders.Where(ho => ho.IsDeleted == false && ho.Id == dto.ReservationId).Include(ho => ho.ConfirmationNumbers).Select(ho => new GetReservationVM
                {
                    HotelOrderId = ho.Id,
                    ClientNameSurname = ho.ClientNameSurname,
                    HotelName = ho.HotelName,
                    Days = ho.Days,
                    DateFrom = ho.DateFrom,
                    DateTo = ho.DateTo,
                    RoomCount = ho.RoomCount,
                    RoomType = ho.RoomType,
                    ConfirmationNumbers = ho.ConfirmationNumbers.Select(cn => cn.Number).ToList()
                }).FirstOrDefaultAsync();

                if (reservation != null)
                {
                    return new BaseResponse
                    {
                        Data = reservation,
                        Message = "Confirmation Numbers Added Successfully",
                        StatusCode = "200",
                        Success = true
                    }; 
                }

                else
                {
                    return new BaseResponse
                    {
                        Data = new GetReservationVM(),
                        Success = false,
                        Message = "Confirmation Numbers Added, But Reservation Could not found",
                        StatusCode = "404",                      
                    };
                }





            }
            catch (Exception ex) 
            {
                return new BaseResponse
                {
                    Data = new GetReservationVM(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false
                };
            }                       
        }

        public async Task<BaseResponse> GetHotelOrdersAsync(int pageIndex)
        {
            try
            {
                List<GetReservationVM> Reservations = await _context.HotelOrders.Where(ho => ho.IsDeleted == false)
                                        .OrderByDescending(ho => ho.Id)
                                        .Skip((pageIndex - 1) * 10)
                                        .Take(10)
                                        .Include(ho=>ho.ConfirmationNumbers).Select(ho=>new GetReservationVM
                {
                    HotelOrderId = ho.Id,
                    ClientNameSurname = ho.ClientNameSurname,
                    HotelName = ho.HotelName,
                    Days = ho.Days,
                    DateFrom = ho.DateFrom,
                    DateTo = ho.DateTo,
                    RoomCount = ho.RoomCount,
                    RoomType = ho.RoomType,
                    ConfirmationNumbers = ho.ConfirmationNumbers.Select(cn=> cn.Number).ToList()
                }).ToListAsync();

                if (!Reservations.Any())
                {
                    return new BaseResponse
                    {
                        Data = new List<GetReservationVM>(),
                        Message = "Hotel Orders Could Not Found",
                        StatusCode = "404",
                        Success = false
                    };
                }

                else
                {
                    var reservationCount = await _context.HotelOrders.CountAsync(h => h.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)reservationCount / 10);
                    return new BaseResponse
                    {
                        Data = Reservations,
                        StatusCode = "200",
                        Success = true,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };

                }
            }
            catch (Exception ex) 
            {
                return new BaseResponse
                {
                    Data = new List<GetReservationVM>(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false
                };


            }
    }

        public async Task<BaseResponse> GetReservationForEditAsync(int reservationid)
        {
            try
            {
                GetReservationVM reservation = await _context.HotelOrders.Where(ho => ho.IsDeleted == false && ho.Id == reservationid).Include(ho => ho.ConfirmationNumbers).Select(ho => new GetReservationVM
                {
                    HotelOrderId = ho.Id,
                    ClientNameSurname = ho.ClientNameSurname,
                    HotelName = ho.HotelName,
                    Days = ho.Days,
                    DateFrom = ho.DateFrom,
                    DateTo = ho.DateTo,
                    RoomCount = ho.RoomCount,
                    RoomType = ho.RoomType,
                    ConfirmationNumbers = ho.ConfirmationNumbers.Select(cn => cn.Number).ToList()
                }).FirstOrDefaultAsync();

                if (reservation == null)
                {
                    return new BaseResponse
                    {
                        Data = new GetReservationVM(),
                        Message = "Hotel Order Could Not Found",
                        StatusCode = "404",
                        Success = false
                    };
                }

                else
                {

                    return new BaseResponse
                    {
                        Data = reservation,
                        StatusCode = "200",
                        Success = true,
                    };

                }




            }
            catch (Exception ex) 
            {
                return new BaseResponse
                {
                    Data = new GetReservationVM(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false
                };

            }

            
        }
    }
}
