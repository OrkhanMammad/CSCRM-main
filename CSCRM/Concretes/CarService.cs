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
        private async Task<List<GetCarVM>> GetCarsAsync(int pageIndex)
        {
            return await _context.CarTypes.Where(c => c.IsDeleted == false)
                                                        .OrderByDescending(c => c.Id)
                                                        .Skip((pageIndex-1) * 6)
                                                        .Take(6)
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
            car.Name = updatedCar.Name.Trim();
            car.Capacity = updatedCar.Capacity;          
        }
        public async Task<BaseResponse> GetAllCarsAsync(int pageIndex)
        {
            try
            {
                List<GetCarVM> cars = await GetCarsAsync(pageIndex);
                int carsCount = await _context.CarTypes.CountAsync(ct=>ct.IsDeleted==false);
                int pageSize = (int)Math.Ceiling((decimal)carsCount / 6);
                return cars.Any()
                ? new BaseResponse { Data = cars, Success = true, StatusCode = "201", PageIndex=pageIndex, PageSize=pageSize }
                : new BaseResponse { Data = new List<GetCarVM>(), Message = "No car found", Success = true, StatusCode = "200" };

            }
            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    StatusCode = "404", 
                    Message = "Unhandled error occured", 
                    Success = false, 
                    Data= new List<GetCarVM>() 
                };
            }
        }
        public async Task<BaseResponse> AddCarAsync(AddCarVM carVM, AppUser appUser)
        {
            try
            {
         
                if (string.IsNullOrEmpty(carVM.Name))
                {
                    List<GetCarVM> carsInDb = await GetCarsAsync(1);
                    int carsCount = await _context.CarTypes.CountAsync(ct => ct.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)carsCount / 6);
                    return new BaseResponse 
                    { 
                        Message = $"Car Type Name can not be empty", 
                        StatusCode = "400", 
                        Success = false, 
                        Data = carsInDb,
                        PageSize = pageSize,
                        PageIndex=1
                    };
                }

                List<string> carNamesInDB = await _context.CarTypes.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();
                if (carNamesInDB.Any(hn => hn.ToLower() == carVM.Name.Trim().ToLower()))
                {
                    List<GetCarVM> carsInDb = await GetCarsAsync(1);
                    int carsCount = await _context.CarTypes.CountAsync(ct => ct.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)carsCount / 6);
                    return new BaseResponse 
                    { 
                        Message = $"Car {carVM.Name} is already exists", 
                        StatusCode = "409", 
                        Success = false,
                        Data = carsInDb,
                        PageSize = pageSize,
                        PageIndex=1
                    };
                }

                CarType newCar = new CarType
                {
                    Name = carVM.Name.Trim(),
                    Capacity= carVM.Capacity,
                    CreatedBy = appUser.Name + " " + appUser.SurName,                    
                };

                await _context.CarTypes.AddAsync(newCar);

               

                await _context.SaveChangesAsync();
                var newCarIndB = await _context.CarTypes.FirstOrDefaultAsync(ct => ct.Name == carVM.Name.Trim());

                List<GetCarVM> cars = await GetCarsAsync(1);
                int carsCountInDb = await _context.CarTypes.CountAsync(ct => ct.IsDeleted == false);
                int pageSizeForCars = (int)Math.Ceiling((decimal)carsCountInDb / 6);
                return new BaseResponse 
                { 
                    Data = cars, 
                    Message = "Car Type Created Successfully", 
                    StatusCode = "201", 
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeForCars
                };


            }
            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    Message = "Car Type Could Not Created Successfully, Unhadled error occured", 
                    StatusCode = "500", 
                    Success = false, 
                    Data = new List<GetCarVM>()
                };
            }
        }
        public async Task<BaseResponse> RemoveCarAsync(int carId, AppUser appUser)
        {
            try
            {
                CarType deletingCar = await _context.CarTypes.FirstOrDefaultAsync(h => h.Id == carId && h.IsDeleted == false);
                if (deletingCar == null) 
                {
                    List<GetCarVM> carsinDb = await GetCarsAsync(1);
                    int carsCount = await _context.CarTypes.CountAsync(ct => ct.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)carsCount / 6);
                    return new BaseResponse 
                    { 
                        Success = false, 
                        Message = "Car Could Not Found By Its Property", 
                        StatusCode = "404", 
                        Data=carsinDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    }; 
                }

                deletingCar.IsDeleted = true;
                deletingCar.DeletedBy = appUser.Name + " " + appUser.SurName;
                await _context.SaveChangesAsync();
                List<GetCarVM> cars = await GetCarsAsync(1);
                int carsCountInDb = await _context.CarTypes.CountAsync(ct => ct.IsDeleted == false);
                int pageSizeForCars = (int)Math.Ceiling((decimal)carsCountInDb / 6);
                return new BaseResponse 
                { 
                    Success = true, 
                    Message = $"Car {deletingCar.Name} is deleted successfully.", 
                    Data = cars,
                    PageIndex=1,
                    PageSize=pageSizeForCars
                };
            }

            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    Success = false, 
                    StatusCode = "500", 
                    Message = "Car Could Not Deleted Successfully, Unhandled error occured", 
                    Data = new List<GetCarVM>() 
                };
            }
        }
        public async Task<BaseResponse> GetCarByIdAsync(int carId)
        {
            try
            {
                CarType carEntity = await _context.CarTypes.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == carId);
                if (carEntity == null)
                {
                    return new BaseResponse 
                    { 
                        Message = "Car Could Not Found by Its Property", 
                        StatusCode = "404", 
                        Success = false, 
                        Data = new EditCarVM() 
                    };
                }

                EditCarVM carForEdit = new EditCarVM
                {
                    Id = carEntity.Id,
                    Name = carEntity.Name,
                    Capacity= carEntity.Capacity,
                };
                return new BaseResponse 
                { 
                    Success = true, 
                    Data = carForEdit, 
                    StatusCode = "200" 
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    Success = false, 
                    Data = new EditCarVM(), 
                    StatusCode = "500", 
                    Message = "Unhandled error occured" 
                };
            }
        }
        public async Task<BaseResponse> EditCarAsync(EditCarVM car, AppUser appUser)
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

            try
            {
                var carId_NamesInDB = await _context.CarTypes.Where(h => h.IsDeleted == false).Select(h => new {Id=h.Id, Name = h.Name }).ToListAsync();
                if (carId_NamesInDB.Any(hn => hn.Name.ToLower() == car.Name.Trim().ToLower() && hn.Id!=car.Id))
                {
                  
                    return new BaseResponse 
                    { 
                        Message = $"Car {car.Name} is already exists, please change the name!", 
                        StatusCode = "409", 
                        Success = false, 
                        Data = car 
                    };
                }


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
