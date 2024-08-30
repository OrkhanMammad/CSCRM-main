using CSCRM.Abstractions;
using CSCRM.Areas.Manage.Controllers;
using CSCRM.dataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.TourCarVMs;
using CSCRM.ViewModels.TourVMs;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class TourByCarTypeService : ITourByCarTypeService
    {
        readonly AppDbContext _context;
        private readonly ILogger<TourByCarTypeService> _logger;
        public TourByCarTypeService(AppDbContext context, ILogger<TourByCarTypeService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task CreateTourByCarTypeAsyncWhenNewCarCreating(int carTypeId)
        {
            List<int> ToursIds = await _context.Tours.Where(t => t.IsDeleted == false).Select(t => t.Id).ToListAsync();
            List<TourByCarType> NewTourByCarTypes = new List<TourByCarType>();
            if (ToursIds.Any())
            {
                foreach (var TourId in ToursIds)
                {
                    NewTourByCarTypes.Add(new TourByCarType { CarTypeId = carTypeId, TourId = TourId });
                }
                await _context.TourByCarTypes.AddRangeAsync(NewTourByCarTypes);
                await _context.SaveChangesAsync();
            }

        }
        public async Task RemoveTourByCarTypeAsyncWhenCarRemoving(int carTypeId)
        {
            List<TourByCarType> RemovingTourByCars = await _context.TourByCarTypes.Where(t => t.CarTypeId == carTypeId).ToListAsync();
            _context.TourByCarTypes.RemoveRange(RemovingTourByCars);
            await _context.SaveChangesAsync();
        }
        public async Task<ResponseForTourByCarPage> GetAllTrCrTypsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all TourByCarTypes from the database.");

                var tourByCarTypes = await _context.TourByCarTypes
                    .Include(t => t.Tour)
                    .Include(t => t.CarType)
                    .Select(t => new
                    {
                        TourName = t.Tour.Name,
                        CarTypeName = t.CarType.Name,
                        t.Price
                    })
                    .ToListAsync();

                if (!tourByCarTypes.Any())
                {
                    _logger.LogInformation("No TourByCarTypes found in the database.");

                    return new ResponseForTourByCarPage
                    {
                        data = new TourCarPageMainVM(),
                        Success = false,
                        StatusCode = "404",
                        Message = "No Tour Found"
                    };
                }

                _logger.LogInformation("Grouping and processing TourByCarTypes data.");

                List<GetTourCarVM> getTourCars = tourByCarTypes
                    .GroupBy(t => t.TourName)
                    .Select(g => new GetTourCarVM
                    {
                        TourName = g.Key,
                        CarPrices = g.ToDictionary(t => t.CarTypeName, t => t.Price)
                    })
                    .ToList();

                _logger.LogInformation("Fetching CarTypes for the page.");

                List<GetCarIdNameVM> CarIdNames = await _context.CarTypes
                    .Where(ct => ct.IsDeleted == false)
                    .Select(ct => new GetCarIdNameVM
                    {
                        Id = ct.Id,
                        Name = ct.Name
                    }).ToListAsync();

                TourCarPageMainVM tourCarPageMainVM = new TourCarPageMainVM
                {
                    CarIdNameVM = CarIdNames,
                    TourCarVM = getTourCars
                };

                _logger.LogInformation("Successfully fetched TourByCarTypes and CarTypes.");

                return new ResponseForTourByCarPage
                {
                    data = tourCarPageMainVM,
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while fetching TourByCarTypes and CarTypes.");

                return new ResponseForTourByCarPage
                {
                    data = new TourCarPageMainVM(),
                    Success = false,
                    StatusCode = "500",
                    Message = "Unhandled error occurred"
                };
            }
        }

        public async Task CreateTourByCarTypeAsyncWhenNewTourCreating(int TourId)
        {
            List<int> CarsIds = await _context.CarTypes.Where(t => t.IsDeleted == false).Select(t => t.Id).ToListAsync();
            List<TourByCarType> NewTourByCarTypes = new List<TourByCarType>();
            if (CarsIds.Any())
            {
                foreach (var CarId in CarsIds)
                {
                    NewTourByCarTypes.Add(new TourByCarType { CarTypeId = CarId, TourId = TourId });
                }
                await _context.TourByCarTypes.AddRangeAsync(NewTourByCarTypes);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveTourByCarTypeAsyncWhenTourRemoving(int TourId)
        {
            List<TourByCarType> RemovingTourByCars = await _context.TourByCarTypes.Where(t=>t.TourId==TourId).ToListAsync();
            _context.TourByCarTypes.RemoveRange(RemovingTourByCars);
            await _context.SaveChangesAsync();
        }
        public async Task<ResponseForTourByCarPage> GetTourCarForEditByTourName(string tourName)
        {
            try
            {
                _logger.LogInformation("Fetching TourByCarTypes for tour name: {TourName}", tourName);

                var tourByCarTypes = await _context.TourByCarTypes
                    .Include(t => t.Tour)
                    .Include(t => t.CarType)
                    .Select(t => new
                    {
                        TourName = t.Tour.Name,
                        CarTypeName = t.CarType.Name,
                        t.Price
                    })
                    .Where(t => t.TourName == tourName)
                    .ToListAsync();

                if (!tourByCarTypes.Any())
                {
                    _logger.LogInformation("No TourByCarTypes found for tour name: {TourName}", tourName);

                    return new ResponseForTourByCarPage
                    {
                        data = new TourCarPageMainVM(),
                        Success = false,
                        StatusCode = "404",
                        Message = "No Tour Found"
                    };
                }

                _logger.LogInformation("Grouping and processing TourByCarTypes data for tour name: {TourName}", tourName);

                List<GetTourCarVM> getTourCars = tourByCarTypes
                    .GroupBy(t => t.TourName)
                    .Select(g => new GetTourCarVM
                    {
                        TourName = g.Key,
                        CarPrices = g.ToDictionary(t => t.CarTypeName, t => t.Price)
                    })
                    .ToList();

                _logger.LogInformation("Fetching CarTypes for the page.");

                List<GetCarIdNameVM> CarIdNames = await _context.CarTypes
                    .Where(ct => ct.IsDeleted == false)
                    .Select(ct => new GetCarIdNameVM
                    {
                        Id = ct.Id,
                        Name = ct.Name
                    }).ToListAsync();

                TourCarPageMainVM tourCarPageMainVM = new TourCarPageMainVM
                {
                    CarIdNameVM = CarIdNames,
                    TourCarVM = getTourCars
                };

                _logger.LogInformation("Successfully fetched TourByCarTypes and CarTypes for tour name: {TourName}", tourName);

                return new ResponseForTourByCarPage
                {
                    data = tourCarPageMainVM,
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while fetching TourByCarTypes for tour name: {TourName}", tourName);

                return new ResponseForTourByCarPage
                {
                    data = new TourCarPageMainVM(),
                    Success = false,
                    StatusCode = "500",
                    Message = "Unhandled error occurred"
                };
            }
        }

        public async Task<ResponseForTourByCarPage> EditTourCarAsync(EditTourCarVM editTourCarVM)
        {
            try
            {
                _logger.LogInformation("Starting EditTourCarAsync with TourName: {TourName}", editTourCarVM.TourName);

                Tour tour = await _context.Tours.FirstOrDefaultAsync(t => t.Name == editTourCarVM.TourName);
                if (tour == null)
                {
                    _logger.LogInformation("Tour not found: {TourName}", editTourCarVM.TourName);

                    return new ResponseForTourByCarPage
                    {
                        data = new TourCarPageMainVM(),
                        Success = false,
                        StatusCode = "404",
                        Message = "Tour Could Not Be Found"
                    };
                }

                _logger.LogInformation("Tour found. TourId: {TourId}", tour.Id);

                List<TourByCarType> tourByCarTypes = await _context.TourByCarTypes
                    .Where(tc => tc.TourId == tour.Id)
                    .ToListAsync();

                _logger.LogInformation("Updating car prices for TourId: {TourId}", tour.Id);

                foreach (var carPrice in editTourCarVM.CarPrices)
                {
                    var tourByCarType = tourByCarTypes.FirstOrDefault(tc => tc.CarTypeId == carPrice.Key);
                    if (tourByCarType != null)
                    {
                        tourByCarType.Price = carPrice.Value;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Car prices updated for TourId: {TourId}", tour.Id);

                var tourByCarTypesInDb = await _context.TourByCarTypes
                    .Include(t => t.Tour)
                    .Include(t => t.CarType)
                    .Select(t => new
                    {
                        TourName = t.Tour.Name,
                        CarTypeName = t.CarType.Name,
                        t.Price
                    })
                    .Where(t => t.TourName == tour.Name)
                    .ToListAsync();

                if (!tourByCarTypesInDb.Any())
                {
                    _logger.LogInformation("No car types found for TourName: {TourName}", tour.Name);

                    return new ResponseForTourByCarPage
                    {
                        data = new TourCarPageMainVM(),
                        Success = false,
                        StatusCode = "404",
                        Message = "No Tour Found"
                    };
                }

                _logger.LogInformation("Generating response data for TourName: {TourName}", tour.Name);

                List<GetTourCarVM> getTourCars = tourByCarTypesInDb
                    .GroupBy(t => t.TourName)
                    .Select(g => new GetTourCarVM
                    {
                        TourName = g.Key,
                        CarPrices = g.ToDictionary(t => t.CarTypeName, t => t.Price)
                    })
                    .ToList();

                List<GetCarIdNameVM> carIdNames = await _context.CarTypes
                    .Where(ct => ct.IsDeleted == false)
                    .Select(ct => new GetCarIdNameVM
                    {
                        Id = ct.Id,
                        Name = ct.Name
                    }).ToListAsync();

                TourCarPageMainVM tourCarPageMainVM = new TourCarPageMainVM
                {
                    CarIdNameVM = carIdNames,
                    TourCarVM = getTourCars
                };

                _logger.LogInformation("Tour prices updated successfully for TourName: {TourName}", tour.Name);

                return new ResponseForTourByCarPage
                {
                    data = tourCarPageMainVM,
                    Success = true,
                    StatusCode = "200",
                    Message = "Tour Prices Updated Successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while editing tour car for TourName: {TourName}", editTourCarVM.TourName);

                return new ResponseForTourByCarPage
                {
                    data = new TourCarPageMainVM(),
                    Success = false,
                    StatusCode = "500",
                    Message = "Unhandled Error Occurred"
                };
            }
        }


    }
}
