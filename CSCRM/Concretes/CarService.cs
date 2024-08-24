using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CarVMs;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.HotelVMs;
using CSCRM.ViewModels.ItineraryVMS;
using CSCRM.ViewModels.TourVMs;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class CarService : ICarService
    {
        readonly AppDbContext _context;
        readonly ITourByCarTypeService _tourByCarTypeService;
        private readonly ILogger<CarService> _logger;
        public CarService(AppDbContext context, ITourByCarTypeService service, ILogger<CarService> logger)
        {
            _context = context;
            _tourByCarTypeService = service;
            _logger = logger;
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
            _logger.LogInformation("GetAllCarsAsync started for PageIndex: {PageIndex}", pageIndex);

            try
            {
                
                List<GetCarVM> cars = await GetCarsAsync(pageIndex);

                
                int carsCount = await _context.CarTypes.CountAsync(ct => !ct.IsDeleted);
                int pageSize = (int)Math.Ceiling((decimal)carsCount / 6);

                if (cars.Any())
                {
                    _logger.LogInformation("Successfully retrieved cars for PageIndex: {PageIndex}. Total Cars Count: {TotalCarsCount}", pageIndex, carsCount);

                    return new BaseResponse
                    {
                        Data = cars,
                        Success = true,
                        StatusCode = "201",
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    _logger.LogInformation("No cars found for PageIndex: {PageIndex}", pageIndex);

                    return new BaseResponse
                    {
                        Data = new List<GetCarVM>(),
                        Message = "No car found",
                        Success = true,
                        StatusCode = "200"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while fetching cars for PageIndex: {PageIndex}", pageIndex);

                return new BaseResponse
                {
                    StatusCode = "404",
                    Message = "Unhandled error occurred",
                    Success = false,
                    Data = new List<GetCarVM>()
                };
            }
        }

        public async Task<BaseResponse> AddCarAsync(AddCarVM carVM, AppUser appUser)
        {
            _logger.LogInformation("AddCarAsync method started. CarVM Name: {CarName}, AppUser: {AppUserName}", carVM.Name, appUser.Name);

            try
            {
                // Car name validation
                if (string.IsNullOrEmpty(carVM.Name))
                {
                    _logger.LogWarning("Car Type Name is empty. Returning error response.");

                    List<GetCarVM> carsInDb = await GetCarsAsync(1);
                    int carsCount = await _context.CarTypes.CountAsync(ct => ct.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)carsCount / 6);
                    return new BaseResponse
                    {
                        Message = "Car Type Name cannot be empty",
                        StatusCode = "400",
                        Success = false,
                        Data = carsInDb,
                        PageSize = pageSize,
                        PageIndex = 1
                    };
                }

                // Check if the car name already exists
                List<string> carNamesInDB = await _context.CarTypes
                    .Where(h => !h.IsDeleted)
                    .Select(h => h.Name)
                    .ToListAsync();

                if (carNamesInDB.Any(hn => hn.ToLower() == carVM.Name.Trim().ToLower()))
                {
                    _logger.LogWarning("Car Type already exists. Car Name: {CarName}", carVM.Name);

                    List<GetCarVM> carsInDb = await GetCarsAsync(1);
                    int carsCount = await _context.CarTypes.CountAsync(ct => !ct.IsDeleted);
                    int pageSize = (int)Math.Ceiling((decimal)carsCount / 6);
                    return new BaseResponse
                    {
                        Message = $"Car {carVM.Name} already exists",
                        StatusCode = "409",
                        Success = false,
                        Data = carsInDb,
                        PageSize = pageSize,
                        PageIndex = 1
                    };
                }

                // Start transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        CarType newCar = new CarType
                        {
                            Name = carVM.Name.Trim(),
                            Capacity = carVM.Capacity,
                            CreatedBy = $"{appUser.Name} {appUser.SurName}",
                        };

                        await _context.CarTypes.AddAsync(newCar);
                        await _context.SaveChangesAsync();

                        // Create TourByCarType objects
                        var newCarIndB = await _context.CarTypes
                            .FirstOrDefaultAsync(ct => ct.Name == carVM.Name.Trim() && !ct.IsDeleted);

                        await _tourByCarTypeService.CreateTourByCarTypeAsyncWhenNewCarCreating(newCarIndB.Id);

                        await transaction.CommitAsync();
                        _logger.LogInformation("Car Type created successfully. Car ID: {CarId}", newCar.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while creating Car Type. Rolling back transaction.");
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                // Return success response
                List<GetCarVM> cars = await GetCarsAsync(1);
                int carsCountInDb = await _context.CarTypes.CountAsync(ct => !ct.IsDeleted);
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
                _logger.LogError(ex, "Unhandled error occurred while creating Car Type.");

                return new BaseResponse
                {
                    Message = "Car Type Could Not Be Created Successfully, Unhandled error occurred",
                    StatusCode = "500",
                    Success = false,
                    Data = new List<GetCarVM>()
                };
            }
        }

        public async Task<BaseResponse> RemoveCarAsync(int carId, AppUser appUser)
        {
            _logger.LogInformation("RemoveCarAsync method started. CarId: {CarId}, AppUser: {AppUserName}", carId, appUser.Name);

            try
            {
                // Check if the car exists
                CarType deletingCar = await _context.CarTypes
                    .FirstOrDefaultAsync(h => h.Id == carId && !h.IsDeleted);

                if (deletingCar == null)
                {
                    _logger.LogWarning("Car with Id: {CarId} not found or already deleted.", carId);

                    List<GetCarVM> carsInDb = await GetCarsAsync(1);
                    int carsCount = await _context.CarTypes.CountAsync(ct => !ct.IsDeleted);
                    int pageSize = (int)Math.Ceiling((decimal)carsCount / 6);

                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Car Could Not Be Found By Its Property",
                        StatusCode = "404",
                        Data = carsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                // Start transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        deletingCar.IsDeleted = true;
                        deletingCar.DeletedBy = $"{appUser.Name} {appUser.SurName}";

                        // Remove related TourByCarType entries
                        _logger.LogInformation("Removing related TourByCarType entries for CarId: {CarId}", carId);
                        await _tourByCarTypeService.RemoveTourByCarTypeAsyncWhenCarRemoving(carId);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Car with Id: {CarId} deleted successfully.", carId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while deleting CarId: {CarId}. Rolling back transaction.", carId);
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                List<GetCarVM> cars = await GetCarsAsync(1);
                int carsCountInDb = await _context.CarTypes.CountAsync(ct => !ct.IsDeleted);
                int pageSizeForCars = (int)Math.Ceiling((decimal)carsCountInDb / 6);

                return new BaseResponse
                {
                    Success = true,
                    Message = $"Car {deletingCar.Name} is deleted successfully.",
                    Data = cars,
                    PageIndex = 1,
                    PageSize = pageSizeForCars
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while deleting CarId: {CarId}.", carId);

                return new BaseResponse
                {
                    Success = false,
                    StatusCode = "500",
                    Message = "Car Could Not Be Deleted Successfully, Unhandled error occurred",
                    Data = new List<GetCarVM>()
                };
            }
        }

        public async Task<BaseResponse> GetCarByIdAsync(int carId)
        {
            _logger.LogInformation("GetCarByIdAsync method started. CarId: {CarId}", carId);

            try
            {
                CarType carEntity = await _context.CarTypes
                    .FirstOrDefaultAsync(h => !h.IsDeleted && h.Id == carId);

                if (carEntity == null)
                {
                    _logger.LogWarning("Car with Id: {CarId} not found.", carId);

                    return new BaseResponse
                    {
                        Message = "Car Could Not Be Found by Its Property",
                        StatusCode = "404",
                        Success = false,
                        Data = new EditCarVM()
                    };
                }

                var carForEdit = new EditCarVM
                {
                    Id = carEntity.Id,
                    Name = carEntity.Name,
                    Capacity = carEntity.Capacity,
                };

                _logger.LogInformation("Car with Id: {CarId} found successfully.", carId);

                return new BaseResponse
                {
                    Success = true,
                    Data = carForEdit,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while getting CarId: {CarId}.", carId);

                return new BaseResponse
                {
                    Success = false,
                    Data = new EditCarVM(),
                    StatusCode = "500",
                    Message = "Unhandled error occurred"
                };
            }
        }

        public async Task<BaseResponse> EditCarAsync(EditCarVM car, AppUser appUser)
        {
            _logger.LogInformation("EditCarAsync method started. CarId: {CarId}", car?.Id);

            if (car == null || car.Id <= 0)
            {
                _logger.LogWarning("Invalid car ID provided. CarId: {CarId}", car?.Id);

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
                _logger.LogWarning("Car name cannot be empty. CarId: {CarId}", car?.Id);

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
                var carIdNamesInDB = await _context.CarTypes
                    .Where(h => !h.IsDeleted)
                    .Select(h => new { Id = h.Id, Name = h.Name })
                    .ToListAsync();

                if (carIdNamesInDB.Any(hn => hn.Name.ToLower() == car.Name.Trim().ToLower() && hn.Id != car.Id))
                {
                    _logger.LogWarning("Car name already exists. CarName: {CarName}", car.Name);

                    return new BaseResponse
                    {
                        Message = $"Car {car.Name} already exists, please change the name!",
                        StatusCode = "409",
                        Success = false,
                        Data = car
                    };
                }

                CarType editCar = await _context.CarTypes
                    .FirstOrDefaultAsync(c => c.Id == car.Id && !c.IsDeleted);

                if (editCar == null)
                {
                    _logger.LogWarning("Car not found. CarId: {CarId}", car.Id);

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

                var carEntity = await _context.CarTypes
                    .FirstOrDefaultAsync(h => !h.IsDeleted && h.Id == editCar.Id);

                var carEdited = new EditCarVM
                {
                    Id = carEntity.Id,
                    Name = carEntity.Name,
                    Capacity = carEntity.Capacity,
                };

                _logger.LogInformation("Car updated successfully. CarId: {CarId}", car.Id);

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
                _logger.LogError(ex, "Unhandled exception occurred while updating CarId: {CarId}", car?.Id);

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
