using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CarVMs;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.HotelVMs;
using CSCRM.ViewModels.ItineraryVMS;
using CSCRM.ViewModels.TourVMs;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class CarService : ICarService
    {
        readonly AppDbContext _context;
        public CarService(AppDbContext context)
        {
            _context = context;   
        }

        public async Task<BaseResponse> GetAllCarsAsync()
        {
            try
            {
                List<GetCarVM> cars = await _context.CarTypes.Where(c => c.IsDeleted == false)
                                                        .Select(c => new GetCarVM
                                                        {
                                                            Id = c.Id,
                                                            Name = c.Name,
                                                            Capacity = c.Capacity,

                                                        })
                                                        .ToListAsync();



                if (cars.Count() == 0)
                {
                    return new BaseResponse { Data = new List<GetCarVM>(), Message = "No car found", Success = true, StatusCode = "200" };
                }
                else
                {
                    return new BaseResponse { Data = cars, Success = true, StatusCode = "201" };

                }

            }
            catch (Exception ex)
            {
                return new BaseResponse { StatusCode = "404", Message = "Unhandled error occured", Success = false, Data= new List<GetCarVM>() };
            }
        }

        public async Task<BaseResponse> AddCarAsync(AddCarVM carVM)
        {
            try
            {
                if (string.IsNullOrEmpty(carVM.Name))
                {
                    List<GetCarVM> carsInDb = await _context.CarTypes
                                                    .Where(c => c.IsDeleted == false)
                                                    .Select(c => new GetCarVM
                                                    {
                                                        Id = c.Id,
                                                        Name = c.Name,
                                                        Capacity = c.Capacity,
                                                        
                                                    })
                                                    .ToListAsync();
                    return new BaseResponse { Message = $"Car Type Name can not be empty", StatusCode = "201", Success = false, Data = carsInDb };

                }

                List<string> carNamesInDB = await _context.CarTypes.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();
                if (carNamesInDB.Any(hn => hn.ToLower() == carVM.Name.Trim().ToLower()))
                {
                    List<GetCarVM> carsInDb = await _context.CarTypes
                                                     .Where(c => c.IsDeleted == false)
                                                     .Select(c => new GetCarVM
                                                     {
                                                         Id = c.Id,
                                                         Name = c.Name,
                                                         Capacity = c.Capacity,

                                                     })
                                                     .ToListAsync();
                    return new BaseResponse { Message = $"Car {carVM.Name} is already exists", StatusCode = "201", Success = false, Data = carsInDb };
                }

                CarType newCar = new CarType
                {
                    Name = carVM.Name,
                    Capacity= carVM.Capacity,
                    
                };



                await _context.CarTypes.AddAsync(newCar);
                await _context.SaveChangesAsync();
                List<GetCarVM> cars = await _context.CarTypes
                                                     .Where(c => c.IsDeleted == false)
                                                     .Select(c => new GetCarVM
                                                     {
                                                         Id = c.Id,
                                                         Name = c.Name,
                                                         Capacity = c.Capacity,

                                                     })
                                                     .ToListAsync();
                return new BaseResponse { Data = cars, Message = "Car Type Created Successfully", StatusCode = "201", Success = true };


            }
            catch (Exception ex)
            {
                List<GetCarVM> cars = await _context.CarTypes
                                                    .Where(c => c.IsDeleted == false)
                                                    .Select(c => new GetCarVM
                                                    {
                                                        Id = c.Id,
                                                        Name = c.Name,
                                                        Capacity = c.Capacity,

                                                    })
                                                    .ToListAsync();
                return new BaseResponse { Message = "Car Type Could Not Created Successfully, Unhadled error occured", StatusCode = "500", Success = false, Data = cars };

            }
        }
    }
}
