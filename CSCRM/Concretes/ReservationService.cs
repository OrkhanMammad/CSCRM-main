using CSCRM.Abstractions;
using CSCRM.dataAccessLayers;
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
        private readonly ILogger<ReservationService> _logger;
        public ReservationService(AppDbContext context, ILogger<ReservationService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<BaseResponse> EditReservationAsync(EditReservationDTO dto)
        {
            _logger.LogInformation("EditReservationAsync started for ReservationId: {ReservationId}", dto.ReservationId);
          

            try
            {

                List<HotelConfirmationNumber> ConfirmationNums = await _context.HotelConfirmationNumbers
                    .Where(hc => hc.HotelOrderId == dto.ReservationId)
                    .ToListAsync();

                _logger.LogInformation("Fetched {Count} existing confirmation numbers for ReservationId: {ReservationId}", ConfirmationNums.Count, dto.ReservationId);

                if (ConfirmationNums.Any())
                {
                    _context.HotelConfirmationNumbers.RemoveRange(ConfirmationNums);
                    _logger.LogInformation("Removed {Count} confirmation numbers for ReservationId: {ReservationId}", ConfirmationNums.Count, dto.ReservationId);
                }


                List<HotelConfirmationNumber> NewConfirmationNumbers = new List<HotelConfirmationNumber>();
                foreach (string ConfNum in dto.ConfirmationNumbers)
                {
                    NewConfirmationNumbers.Add(new HotelConfirmationNumber { HotelOrderId = dto.ReservationId, Number = ConfNum });
                }

                await _context.HotelConfirmationNumbers.AddRangeAsync(NewConfirmationNumbers);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added {Count} new confirmation numbers for ReservationId: {ReservationId}", NewConfirmationNumbers.Count, dto.ReservationId);

                GetReservationVM reservation = await _context.HotelOrders
                    .Where(ho => !ho.IsDeleted && ho.Id == dto.ReservationId)
                    .Include(ho => ho.ConfirmationNumbers)
                    .Select(ho => new GetReservationVM
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
                    })
                    .FirstOrDefaultAsync();

                if (reservation != null)
                {
                    _logger.LogInformation("Successfully fetched reservation data for ReservationId: {ReservationId}", dto.ReservationId);

                    return new BaseResponse
                    {
                        data = reservation,
                        Message = "Confirmation Numbers Added Successfully",
                        StatusCode = "200",
                        Success = true
                    };
                }
                else
                {
                    _logger.LogWarning("Reservation not found after adding confirmation numbers for ReservationId: {ReservationId}", dto.ReservationId);

                    return new BaseResponse
                    {
                        data = new GetReservationVM(),
                        Success = false,
                        Message = "Confirmation Numbers Added, But Reservation Could not be found",
                        StatusCode = "404",
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while editing reservation with ReservationId: {ReservationId}", dto.ReservationId);

                return new BaseResponse
                {
                    data = new GetReservationVM(),
                    Message = "Unhandled Error Occurred",
                    StatusCode = "500",
                    Success = false
                };
            }
        }
        public async Task<BaseResponse> GetHotelOrdersAsync(int pageIndex)
        {
            _logger.LogInformation("GetHotelOrdersAsync started for PageIndex: {PageIndex}", pageIndex);

            try
            {
               
                List<GetReservationVM> Reservations = await _context.HotelOrders
                    .Where(ho => !ho.IsDeleted)
                    .OrderByDescending(ho => ho.Id)
                    .Skip((pageIndex - 1) * 10)
                    .Take(10)
                    .Include(ho => ho.ConfirmationNumbers)
                    .Select(ho => new GetReservationVM
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
                    })
                    .ToListAsync();

                _logger.LogInformation("Fetched {Count} reservations for PageIndex: {PageIndex}", Reservations.Count, pageIndex);

                if (!Reservations.Any())
                {
                    _logger.LogWarning("No hotel orders found for PageIndex: {PageIndex}", pageIndex);

                    return new BaseResponse
                    {
                        data = new List<GetReservationVM>(),
                        Message = "Hotel Orders Could Not Be Found",
                        StatusCode = "404",
                        Success = false
                    };
                }
                else
                {
                    var reservationCount = await _context.HotelOrders.CountAsync(h => !h.IsDeleted);
                    int pageSize = (int)Math.Ceiling((decimal)reservationCount / 10);

                    _logger.LogInformation("Successfully fetched reservations. Total count: {TotalCount}, PageSize: {PageSize}", reservationCount, pageSize);

                    return new BaseResponse
                    {
                        data = Reservations,
                        StatusCode = "200",
                        Success = true,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while fetching hotel orders for PageIndex: {PageIndex}", pageIndex);

                return new BaseResponse
                {
                    data = new List<GetReservationVM>(),
                    Message = "Unhandled Error Occurred",
                    StatusCode = "500",
                    Success = false
                };
            }
        }
        public async Task<BaseResponse> GetReservationForEditAsync(int reservationid)
        {
            _logger.LogInformation("GetReservationForEditAsync started for ReservationId: {ReservationId}", reservationid);

            try
            {
               
                GetReservationVM reservation = await _context.HotelOrders
                    .Where(ho => !ho.IsDeleted && ho.Id == reservationid)
                    .Include(ho => ho.ConfirmationNumbers)
                    .Select(ho => new GetReservationVM
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
                    })
                    .FirstOrDefaultAsync();

                if (reservation == null)
                {
                    _logger.LogWarning("Hotel order not found for ReservationId: {ReservationId}", reservationid);

                    return new BaseResponse
                    {
                        data = new GetReservationVM(),
                        Message = "Hotel Order Could Not Be Found",
                        StatusCode = "404",
                        Success = false
                    };
                }
                else
                {
                    _logger.LogInformation("Successfully fetched reservation for ReservationId: {ReservationId}", reservationid);

                    return new BaseResponse
                    {
                        data = reservation,
                        StatusCode = "200",
                        Success = true
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while fetching reservation for ReservationId: {ReservationId}", reservationid);

                return new BaseResponse
                {
                    data = new GetReservationVM(),
                    Message = "Unhandled Error Occurred",
                    StatusCode = "500",
                    Success = false
                };
            }
        }

    }
}
