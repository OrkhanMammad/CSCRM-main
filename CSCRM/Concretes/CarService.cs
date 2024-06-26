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


        private async Task<List<GetCarVM>> GetCarsAsync()
        {
            return await _context.CarTypes.Where(c => c.IsDeleted == false)
                                                        .Select(c => new GetCarVM
                                                        {
                                                            Id = c.Id,
                                                            Name = c.Name,
                                                            Capacity = c.Capacity,

                                                        })
                                                        .ToListAsync();
        }

        private void CarEditor(CarType car, EditCarVM updatedCar)
        {
            car.Name = updatedCar.Name;
            car.Capacity = updatedCar.Capacity;
           
        }

        public async Task<BaseResponse> GetAllCarsAsync()
        {
            try
            {
                List<GetCarVM> cars = await GetCarsAsync();

                return cars.Any()
                ? new BaseResponse { Data = cars, Success = true, StatusCode = "201" }
                : new BaseResponse { Data = new List<GetCarVM>(), Message = "No car found", Success = true, StatusCode = "200" };

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
                    List<GetCarVM> carsInDb = await GetCarsAsync();
                    return new BaseResponse { Message = $"Car Type Name can not be empty", StatusCode = "400", Success = false, Data = carsInDb };
                }

                List<string> carNamesInDB = await _context.CarTypes.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();
                if (carNamesInDB.Any(hn => hn.ToLower() == carVM.Name.Trim().ToLower()))
                {
                    List<GetCarVM> carsInDb = await GetCarsAsync();

                    return new BaseResponse { Message = $"Car {carVM.Name} is already exists", StatusCode = "409", Success = false, Data = carsInDb };
                }

                CarType newCar = new CarType
                {
                    Name = carVM.Name,
                    Capacity= carVM.Capacity,
                    
                };

                await _context.CarTypes.AddAsync(newCar);
                await _context.SaveChangesAsync();

                List<GetCarVM> cars = await GetCarsAsync();
                return new BaseResponse { Data = cars, Message = "Car Type Created Successfully", StatusCode = "201", Success = true };


            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = "Car Type Could Not Created Successfully, Unhadled error occured", StatusCode = "500", Success = false, Data = new List<GetCarVM>() };
            }
        }

        public async Task<BaseResponse> RemoveCarAsync(int carId)
        {
            try
            {
                CarType deletingCar = await _context.CarTypes.FirstOrDefaultAsync(h => h.Id == carId && h.IsDeleted == false);
                if (deletingCar == null) 
                {
                    List<GetCarVM> carsinDb = await GetCarsAsync();
                    return new BaseResponse { Success = false, Message = "Car Could Not Found By Its Property", StatusCode = "404", Data=carsinDb }; 
                }

                deletingCar.IsDeleted = true;
                await _context.SaveChangesAsync();
                List<GetCarVM> cars = await GetCarsAsync();

                return new BaseResponse { Success = true, Message = $"Car {deletingCar.Name} is deleted successfully.", Data = cars };
            }

            catch (Exception ex)
            {
                return new BaseResponse { Success = false, StatusCode = "500", Message = "Car Could Not Deleted Successfully, Unhandled error occured", Data = new List<GetCarVM>() };
            }
        }

        public async Task<BaseResponse> GetCarByIdAsync(int carId)
        {
            try
            {
                CarType carEntity = await _context.CarTypes.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == carId);
                if (carEntity == null)
                {
                    return new BaseResponse { Message = "Car Could Not Found by Its Property", StatusCode = "404", Success = false, Data = new EditCarVM() };
                }

                EditCarVM carForEdit = new EditCarVM
                {
                    Id = carEntity.Id,
                    Name = carEntity.Name,
                    Capacity= carEntity.Capacity,
                };
                return new BaseResponse { Success = true, Data = carForEdit, StatusCode = "200" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Success = false, Data = new EditCarVM(), StatusCode = "500", Message = "Unhandled error occured" };
            }
        }

        public async Task<BaseResponse> EditCarAsync(EditCarVM car)
        {
            if (car == null || car.Id <= 0)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid car ID.",
                    StatusCode = "400",
                    Data = car
                };
            }

            try
            {
                CarType editCar = await _context.CarTypes.FirstOrDefaultAsync(c => c.Id == car.Id && c.IsDeleted==false);
                
                if (editCar == null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Car not found.",
                        StatusCode = "404",
                        Data = car
                    };
                }


                if (string.IsNullOrWhiteSpace(car.Name))
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Car name cannot be empty.",
                        StatusCode = "400",
                        Data = car
                    };
                }


                CarEditor(editCar, car);
                await _context.SaveChangesAsync();


                CarType carEntity = await _context.CarTypes
                                                       .FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == editCar.Id);

                EditCarVM carEdited = new EditCarVM
                {
                    Id = carEntity.Id,
                    Name = carEntity.Name,
                    Capacity = carEntity.Capacity,
                };

                return new BaseResponse
                {
                    Data = carEdited,
                    Message = "Car updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Data = new EditCarVM(),
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }

    }
}
