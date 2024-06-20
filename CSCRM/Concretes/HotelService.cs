using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
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
        public HotelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponse> AddHotelAsync(AddHotelVM addHotelVM)
        {


            try
            {
                List<string> hotelNamesInDB = await _context.Hotels.Where(h=>h.IsDeleted==false).Select(h => h.Name).ToListAsync();
                if(hotelNamesInDB.Any(hn=>hn.ToLower()==addHotelVM.Name.Trim().ToLower()))
                    return new BaseResponse { Message = $"Hotel {addHotelVM.Name} is already exists", StatusCode = "201", Success = false };
                
                Hotel newHotel = new Hotel 
                { Name = addHotelVM.Name,
                    SinglePrice = addHotelVM.SinglePrice,
                    DoublePrice = addHotelVM.DoublePrice,
                    TriplePrice = addHotelVM.TriplePrice,
                    ContactNumber = addHotelVM.ContactNumber,
                    ContactPerson = addHotelVM.ContactPerson,
                };
                await _context.Hotels.AddAsync(newHotel);
                await _context.SaveChangesAsync();
                List<GetHotelVM> hotels = await _context.Hotels
                                                        .Where(h => h.IsDeleted == false)
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
                return new BaseResponse { Data = hotels, Message = "Hotel Created Successfully", StatusCode = "201", Success = true };


            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = "Hotel Could Not Created Successfully", StatusCode = "500", Success = false };

            }


        }

        public async Task<BaseResponse> GetAllHotelsAsync()
        {
            try
            {
                List<GetHotelVM> hotels = await _context.Hotels
                                                        .Where(h => h.IsDeleted == false)
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
                if (hotels.Count() == 0)
                {
                    return new BaseResponse { Data = new List<GetHotelVM>(), Message = "No hotel found", Success = true, StatusCode = "200" };
                }
                else
                {
                    return new BaseResponse { Data = hotels, Message = "Hotels are sent", Success = true, StatusCode = "201" };

                }

            }
            catch (Exception ex)
            {
                return new BaseResponse { StatusCode = "404", Message = ex.Message, Success = false };
            }
        }

        public async Task<BaseResponse> RemoveHotelAsync(int hotelId)
        {
            Hotel deletingHotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId && h.IsDeleted==false);
            if (deletingHotel == null) { return new BaseResponse { Success = false, Message="Hotel Could Not Found", StatusCode="404" }; }

            deletingHotel.IsDeleted = true;
            await _context.SaveChangesAsync();
            return new BaseResponse { Success = true, Message = $"Hotel {deletingHotel.Name} is deleted successfully. " };

        }
        public Task<BaseResponse> GetHotelByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task EditHotelByIdAsync(int hotelId)
        {
            Hotel hotel = await _context.Hotels.FirstOrDefaultAsync(ho => ho.Id == hotelId);
            hotel.Name = "NewName";
           await _context.SaveChangesAsync();
        }
    }
}
