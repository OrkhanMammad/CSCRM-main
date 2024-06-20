using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models.ResponseTypes;
using CSCRM.Models;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.EntityFrameworkCore;

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
                return new BaseResponse { Data = newHotel, Message = "Hotel Created Successfully", StatusCode = "201", Success = true };


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

        public Task<BaseResponse> GetHotelByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> RemoveHotelAsync()
        {
            throw new NotImplementedException();
        }
    }
}
