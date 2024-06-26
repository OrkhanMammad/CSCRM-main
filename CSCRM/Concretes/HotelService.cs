﻿using CSCRM.Abstractions;
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

        private void HotelEditor(Hotel hotel, EditHotelVM updatedHotel)
        {
            hotel.Name= updatedHotel.Name;
            hotel.TriplePrice= updatedHotel.TriplePrice;
            hotel.DoublePrice= updatedHotel.DoublePrice;
            hotel.SinglePrice= updatedHotel.SinglePrice;
            hotel.ContactPerson= updatedHotel.ContactPerson;
            hotel.ContactNumber= updatedHotel.ContactNumber;
        }

        private async Task<List<GetHotelVM>> GetHotelsAsync()
        {
            return await _context.Hotels
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
        }

        public async Task<BaseResponse> AddHotelAsync(AddHotelVM addHotelVM)
        {


            try
            {
                List<string> hotelNamesInDB = await _context.Hotels.Where(h=>h.IsDeleted==false).Select(h => h.Name).ToListAsync();
                if (hotelNamesInDB.Any(hn => hn.ToLower() == addHotelVM.Name.Trim().ToLower()))
                {
                    List<GetHotelVM> hotelsInDb = await GetHotelsAsync();
                    return new BaseResponse { Message = $"Hotel {addHotelVM.Name} is already exists", StatusCode = "201", Success = true, Data=hotelsInDb };

                }

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
                List<GetHotelVM> hotels = await GetHotelsAsync();

                return new BaseResponse { Data = hotels, Message = "Hotel Created Successfully", StatusCode = "201", Success = true };


            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = "Hotel Could Not Created Successfully", StatusCode = "500", Success = false, Data= new List<GetHotelVM>() };

            }


        }

        public async Task<BaseResponse> GetAllHotelsAsync()
        {
            try
            {
                List<GetHotelVM> hotels = await GetHotelsAsync();
                return hotels.Any()
                ?new BaseResponse { Data = hotels, Success = true, StatusCode = "201" }
                :new BaseResponse { Data = new List<GetHotelVM>(), Message = "No hotel found", Success = true, StatusCode = "200" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { StatusCode = "500", Message = "Unhandled Error Occured", Success = false, Data = new List<GetHotelVM>() };

            }
        }

        public async Task<BaseResponse> RemoveHotelAsync(int hotelId)
        {
            try
            {
                Hotel deletingHotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId && h.IsDeleted == false);
                if (deletingHotel == null) 
                {
                    List<GetHotelVM> hotelsInDb = await GetHotelsAsync();
                    return new BaseResponse { Success = false, Message = "Hotel Could Not Found", StatusCode = "404", Data=hotelsInDb }; 
                
                }

                deletingHotel.IsDeleted = true;
                await _context.SaveChangesAsync();
                List<GetHotelVM> hotels = await GetHotelsAsync();
                return new BaseResponse { Success = true, Message = $"Hotel {deletingHotel.Name} is deleted successfully.", Data = hotels, StatusCode="203" };
            }

            catch(Exception ex) 
            { 
                return new BaseResponse { Success = false, StatusCode = "500", Message = "Hotel Could Not Deleted Successfully", Data=new List<GetHotelVM>() }; 
            }
        }
        public async Task<BaseResponse> GetHotelByIdAsync(int id)
        {
            try
            {
                Hotel hotelEntity = await _context.Hotels.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == id);
                if (hotelEntity == null)
                {
                    return new BaseResponse { Message = "Hotel Could Not Found", StatusCode = "404", Success = false, Data = new EditHotelVM() };
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
                return new BaseResponse { Success = true, Data = hotelForEdit, StatusCode = "201" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Success = false, Data = new EditHotelVM(), StatusCode = "500", Message="Unhandled error occured" };
            }
        }

        public async Task<BaseResponse> EditHotelAsync(EditHotelVM hotel)
        {
            try
            {
                
                if (hotel == null || hotel.Id <= 0)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Invalid hotel ID.",
                        StatusCode = "400",
                        Data = hotel
                    };
                }

                
                Hotel editHotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == hotel.Id);
                if (editHotel == null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Hotel not found.",
                        StatusCode = "404",
                        Data = hotel
                    };
                }

                
                if (string.IsNullOrWhiteSpace(hotel.Name))
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Hotel name cannot be empty.",
                        StatusCode = "400",
                        Data = hotel
                    };
                }

               
                HotelEditor(editHotel, hotel);
                await _context.SaveChangesAsync();


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
                        Data = hotelEdited,
                        Message = "Hotel updated successfully.",
                        Success = true,
                        StatusCode = "200"
                    };        
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {  Data=hotel,
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }

    }
}
