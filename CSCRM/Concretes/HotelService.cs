using CSCRM.Abstractions;
using CSCRM.dataAccessLayers;
using CSCRM.Models.ResponseTypes;
using CSCRM.Models;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CSCRM.Concretes
{
    public class HotelService : IHotelService
    {
        readonly AppDbContext _context;
        private readonly ILogger<HotelService> _logger;
        public HotelService(AppDbContext context, ILogger<HotelService> logger)
        {
            _context = context;
            _logger = logger;
        }
        private void HotelEditor(Hotel hotel, EditHotelVM updatedHotel, string userNmSrnm)
        {
            hotel.Name= updatedHotel.Name.Trim();
            hotel.TriplePrice= updatedHotel.TriplePrice;
            hotel.DoublePrice= updatedHotel.DoublePrice;
            hotel.SinglePrice= updatedHotel.SinglePrice;
            hotel.ContactPerson= updatedHotel.ContactPerson.Trim();
            hotel.ContactNumber= updatedHotel.ContactNumber.Trim();
            hotel.UpdatedBy = userNmSrnm;
        }
        private async Task<List<GetHotelVM>> GetHotelsAsync(short pageIndex)
        {
            return await _context.Hotels
                                        .Where(h => h.IsDeleted == false)
                                        .OrderByDescending(h => h.Id)
                                        .Skip((pageIndex-1)*6)
                                        .Take(6)
                                        .Select(h => new GetHotelVM
                                                        {
                                                            Id = h.Id,
                                                            Name = h.Name,
                                                            SinglePrice = h.SinglePrice,
                                                            DoublePrice = h.DoublePrice,
                                                            TriplePrice = h.TriplePrice,
                                                            ContactPerson = h.ContactPerson,
                                                            ContactNumber = h.ContactNumber,
                                                        })
                                        .ToListAsync();
        }
        public async Task<BaseResponse> AddHotelAsync(AddHotelVM addHotelVM, AppUser appUser)
        {
            try
            {
                if (addHotelVM == null || string.IsNullOrEmpty(addHotelVM.Name))
                {
                    _logger.LogWarning("Attempted to add hotel with null or empty name. AddHotelVM: {@AddHotelVM}", addHotelVM);

                    List<GetHotelVM> hotelsInDb = await GetHotelsAsync(1);
                    int hotelsCount = await _context.Hotels.CountAsync(h => h.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)hotelsCount / 6);

                    return new BaseResponse
                    {
                        Message = $"Hotel Name can not be empty",
                        StatusCode = "201",
                        Success = true,
                        data = hotelsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                var hotelNamesInDB = await _context.Hotels.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();
                if (hotelNamesInDB.Any(hn => hn.ToLower() == addHotelVM.Name.Trim().ToLower()))
                {
                    _logger.LogWarning("Attempted to add a hotel with an existing name: {HotelName}.", addHotelVM.Name);

                    List<GetHotelVM> hotelsInDb = await GetHotelsAsync(1);
                    int hotelsCount = await _context.Hotels.CountAsync(h => h.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)hotelsCount / 6);

                    return new BaseResponse
                    {
                        Message = $"Hotel {addHotelVM.Name} already exists",
                        StatusCode = "201",
                        Success = true,
                        data = hotelsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                _logger.LogInformation("Adding new hotel with name: {HotelName}.", addHotelVM.Name);

                Hotel newHotel = new Hotel
                {
                    Name = addHotelVM.Name.Trim(),
                    SinglePrice = addHotelVM.SinglePrice,
                    DoublePrice = addHotelVM.DoublePrice,
                    TriplePrice = addHotelVM.TriplePrice,
                    ContactNumber = addHotelVM.ContactNumber.Trim(),
                    ContactPerson = addHotelVM.ContactPerson.Trim(),
                    CreatedBy = appUser.Name + " " + appUser.SurName
                };

                await _context.Hotels.AddAsync(newHotel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Hotel with name {HotelName} created successfully.", addHotelVM.Name);

                List<GetHotelVM> hotels = await GetHotelsAsync(1);
                int hotelsCountInDb = await _context.Hotels.CountAsync(h => h.IsDeleted == false);
                int pageSizeForHotels = (int)Math.Ceiling((decimal)hotelsCountInDb / 6);

                return new BaseResponse
                {
                    data = hotels,
                    Message = "Hotel Created Successfully",
                    StatusCode = "201",
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeForHotels
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a hotel with name: {HotelName}.", addHotelVM?.Name);

                return new BaseResponse
                {
                    Message = "Hotel Could Not Be Created Successfully",
                    StatusCode = "500",
                    Success = false,
                    data = new List<GetHotelVM>()
                };
            }
        }

        public async Task<BaseResponse> GetAllHotelsAsync(short pageIndex)
        {
            try
            {
                _logger.LogInformation("Fetching hotels for page index {PageIndex}.", pageIndex);

                List<GetHotelVM> hotels = await GetHotelsAsync(pageIndex);
                var hotelsCount = await _context.Hotels.CountAsync(h => h.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)hotelsCount / 6);

                if (hotels.Any())
                {
                    _logger.LogInformation("Found {HotelCount} hotels for page index {PageIndex}.", hotels.Count, pageIndex);

                    return new BaseResponse
                    {
                        data = hotels,
                        Success = true,
                        StatusCode = "201",
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    _logger.LogInformation("No hotels found for page index {PageIndex}.", pageIndex);

                    return new BaseResponse
                    {
                        data = new List<GetHotelVM>(),
                        Message = "No hotel found",
                        Success = true,
                        StatusCode = "404"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while fetching hotels for page index {PageIndex}.", pageIndex);

                return new BaseResponse
                {
                    StatusCode = "500",
                    Message = "Unhandled Error Occurred",
                    Success = false,
                    data = new List<GetHotelVM>()
                };
            }
        }

        public async Task<BaseResponse> RemoveHotelAsync(int hotelId, AppUser appUser)
        {
            try
            {
                _logger.LogInformation("Attempting to remove hotel with ID {HotelId}.", hotelId);

                Hotel deletingHotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId && h.IsDeleted == false);
                if (deletingHotel == null)
                {
                    _logger.LogWarning("Hotel with ID {HotelId} not found.", hotelId);

                    List<GetHotelVM> hotelsInDb = await GetHotelsAsync(1);
                    int hotelsCount = await _context.Hotels.CountAsync(h => h.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)hotelsCount / 6);
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Hotel Could Not Found",
                        StatusCode = "404",
                        data = hotelsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                deletingHotel.IsDeleted = true;
                deletingHotel.DeletedBy = appUser.Name + " " + appUser.SurName;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Hotel with ID {HotelId} marked as deleted by user {User}.", hotelId, appUser.Name);

                List<GetHotelVM> hotels = await GetHotelsAsync(1);
                int hotelsCountInDb = await _context.Hotels.CountAsync(h => h.IsDeleted == false);
                int pageSizeForHotels = (int)Math.Ceiling((decimal)hotelsCountInDb / 6);
                return new BaseResponse
                {
                    Success = true,
                    Message = $"Hotel {deletingHotel.Name} is deleted successfully.",
                    data = hotels,
                    StatusCode = "203",
                    PageIndex = 1,
                    PageSize = pageSizeForHotels
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing hotel with ID {HotelId}.", hotelId);

                return new BaseResponse
                {
                    Success = false,
                    StatusCode = "500",
                    Message = "Hotel Could Not Be Deleted Successfully",
                    data = new List<GetHotelVM>()
                };
            }
        }

        public async Task<BaseResponse> GetHotelByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve hotel with ID {HotelId}.", id);

                Hotel hotelEntity = await _context.Hotels.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == id);
                if (hotelEntity == null)
                {
                    _logger.LogWarning("Hotel with ID {HotelId} not found.", id);
                    return new BaseResponse
                    {
                        Message = "Hotel Could Not Be Found",
                        StatusCode = "404",
                        Success = false,
                        data = new EditHotelVM()
                    };
                }

                EditHotelVM hotelForEdit = new EditHotelVM
                {
                    Id = hotelEntity.Id,
                    Name = hotelEntity.Name,
                    SinglePrice = hotelEntity.SinglePrice,
                    DoublePrice = hotelEntity.DoublePrice,
                    TriplePrice = hotelEntity.TriplePrice,
                    ContactPerson = hotelEntity.ContactPerson,
                    ContactNumber = hotelEntity.ContactNumber
                };

                _logger.LogInformation("Hotel with ID {HotelId} retrieved successfully.", id);
                return new BaseResponse
                {
                    Success = true,
                    data = hotelForEdit,
                    StatusCode = "201"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving hotel with ID {HotelId}.", id);
                return new BaseResponse
                {
                    Success = false,
                    data = new EditHotelVM(),
                    StatusCode = "500",
                    Message = "Unhandled error occurred"
                };
            }
        }

        public async Task<BaseResponse> EditHotelAsync(EditHotelVM hotel, AppUser appUser)
        {
            if (string.IsNullOrWhiteSpace(hotel.Name))
            {
                _logger.LogWarning("Attempted to edit hotel with empty name.");
                return new BaseResponse
                {
                    Success = false,
                    Message = "Hotel name cannot be empty.",
                    StatusCode = "400",
                    data = hotel
                };
            }
            if (hotel == null || hotel.Id <= 0)
            {
                _logger.LogWarning("Attempted to edit hotel with invalid ID: {HotelId}.", hotel?.Id);
                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid hotel ID.",
                    StatusCode = "400",
                    data = hotel
                };
            }

            try
            {
                _logger.LogInformation("Attempting to check if hotel name '{HotelName}' already exists.", hotel.Name);
                bool hotelExists = await _context.Hotels.AnyAsync(h => h.Name.ToLower() == hotel.Name.ToLower().Trim()
                                                                     && h.IsDeleted == false
                                                                     && h.Id != hotel.Id);

                if (hotelExists)
                {
                    _logger.LogWarning("Hotel with name '{HotelName}' already exists.", hotel.Name);
                    return new BaseResponse
                    {
                        Message = $"Hotel {hotel.Name} already exists.",
                        StatusCode = "409",
                        Success = false,
                        data = hotel
                    };
                }

                _logger.LogInformation("Attempting to retrieve hotel with ID {HotelId}.", hotel.Id);
                Hotel editHotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == hotel.Id);
                if (editHotel == null)
                {
                    _logger.LogWarning("Hotel with ID {HotelId} not found.", hotel.Id);
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Hotel not found.",
                        StatusCode = "404",
                        data = hotel
                    };
                }

                string userNmSrnm = appUser.Name + " " + appUser.SurName;
                _logger.LogInformation("Updating hotel with ID {HotelId} by user {UserName}.", hotel.Id, userNmSrnm);

                HotelEditor(editHotel, hotel, userNmSrnm);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Hotel with ID {HotelId} updated successfully.", hotel.Id);

                Hotel hotelEntity = await _context.Hotels
                                                    .FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == editHotel.Id);

                EditHotelVM hotelEdited = new EditHotelVM
                {
                    Id = hotelEntity.Id,
                    Name = hotelEntity.Name,
                    SinglePrice = hotelEntity.SinglePrice,
                    DoublePrice = hotelEntity.DoublePrice,
                    TriplePrice = hotelEntity.TriplePrice,
                    ContactPerson = hotelEntity.ContactPerson,
                    ContactNumber = hotelEntity.ContactNumber
                };

                return new BaseResponse
                {
                    data = hotelEdited,
                    Message = "Hotel updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating hotel with ID {HotelId}.", hotel?.Id);
                return new BaseResponse
                {
                    data = hotel,
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }


    }
}
