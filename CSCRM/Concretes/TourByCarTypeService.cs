using CSCRM.Abstractions;
using CSCRM.Areas.Manage.Controllers;
using CSCRM.DataAccessLayers;
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
        public TourByCarTypeService(AppDbContext context)
        {
            _context = context;
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
                    return new ResponseForTourByCarPage
                    {
                        Data = new TourCarPageMainVM(),
                        Success = false,
                        StatusCode = "404",
                        Message = "No Tour Found"


                    };
                }

                List<GetTourCarVM> getTourCars = tourByCarTypes
                    .GroupBy(t => t.TourName)
                    .Select(g => new GetTourCarVM
                    {
                        TourName = g.Key,
                        CarPrices = g.ToDictionary(t => t.CarTypeName, t => t.Price)
                    })
                    .ToList();

                List<GetCarIdNameVM> CarIdNames = await _context.CarTypes
                                                                .Where(ct => ct.IsDeleted == false)
                                                                .Select(ct => new GetCarIdNameVM
                                                                {
                                                                    Id = ct.Id,
                                                                    Name = ct.Name
                                                                }).ToListAsync();


               

                TourCarPageMainVM tourCarPageMainVM = new TourCarPageMainVM { CarIdNameVM = CarIdNames, TourCarVM = getTourCars };

                return new ResponseForTourByCarPage
                {
                    Data = tourCarPageMainVM,
                    Success = true,
                    StatusCode = "200",

                };

            }
            catch (Exception ex)
            {
                return new ResponseForTourByCarPage
                {
                    Data = new TourCarPageMainVM(),
                    Success = false,
                    StatusCode = "500",
                    Message = "Unhandled error occured"
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
                    return new ResponseForTourByCarPage
                    {
                        Data = new TourCarPageMainVM(),
                        Success = false,
                        StatusCode = "404",
                        Message = "No Tour Found"
                    };
                }

               List<GetTourCarVM> getTourCars = tourByCarTypes
                       .GroupBy(t => t.TourName)
                       .Select(g => new GetTourCarVM
                       {
                           TourName = g.Key,
                           CarPrices = g.ToDictionary(t => t.CarTypeName, t => t.Price)
                       })
                       .ToList();

                List<GetCarIdNameVM> CarIdNames = await _context.CarTypes
                                                                    .Where(ct => ct.IsDeleted == false)
                                                                    .Select(ct => new GetCarIdNameVM
                                                                    {
                                                                        Id = ct.Id,
                                                                        Name = ct.Name
                                                                    }).ToListAsync();


                TourCarPageMainVM tourCarPageMainVM = new TourCarPageMainVM { CarIdNameVM = CarIdNames, TourCarVM = getTourCars };

                return new ResponseForTourByCarPage
                {
                    Data = tourCarPageMainVM,
                    Success = true,
                    StatusCode = "200",

                };
            }
            catch (Exception ex) 
            {

                return new ResponseForTourByCarPage
                {
                    Data = new TourCarPageMainVM(),
                    Success = false,
                    StatusCode = "500",
                    Message = ex.Message

                };

            }
        }
        public async Task<ResponseForTourByCarPage> EditTourCarAsync(EditTourCarVM editTourCarVM)
        {
            try
            {
                Tour Tour = await _context.Tours.FirstOrDefaultAsync(t => t.Name == editTourCarVM.TourName);                       
                if (Tour == null) 
                {
                    return new ResponseForTourByCarPage
                    {
                        Data = new TourCarPageMainVM(),
                        Success = false,
                        StatusCode = "404",
                        Message = "Tour Could Not Found"
                    };
                }
                

                List<TourByCarType> tourByCarTypes = await _context.TourByCarTypes.Where(tc => tc.TourId == Tour.Id).ToListAsync();
               

                foreach (var carPrice in editTourCarVM.CarPrices)
                {
                    tourByCarTypes.FirstOrDefault(tc => tc.CarTypeId == carPrice.Key).Price= carPrice.Value;                    
                }                

                await _context.SaveChangesAsync();

                var tourByCarTypesInDb = await _context.TourByCarTypes
                                                .Include(t => t.Tour)
                                                .Include(t => t.CarType)
                                                .Select(t => new
                                                {
                                                    TourName = t.Tour.Name,
                                                    CarTypeName = t.CarType.Name,
                                                    t.Price
                                                })
                                                .Where(t => t.TourName == Tour.Name)
                                                .ToListAsync();


                if (!tourByCarTypesInDb.Any())
                {
                    return new ResponseForTourByCarPage
                    {
                        Data = new TourCarPageMainVM(),
                        Success = false,
                        StatusCode = "404",
                        Message = "No Tour Found"
                    };
                }

                List<GetTourCarVM> getTourCars = tourByCarTypesInDb
                        .GroupBy(t => t.TourName)
                        .Select(g => new GetTourCarVM
                        {
                            TourName = g.Key,
                            CarPrices = g.ToDictionary(t => t.CarTypeName, t => t.Price)
                        })
                        .ToList();

                List<GetCarIdNameVM> CarIdNames = await _context.CarTypes
                                                                    .Where(ct => ct.IsDeleted == false)
                                                                    .Select(ct => new GetCarIdNameVM
                                                                    {
                                                                        Id = ct.Id,
                                                                        Name = ct.Name
                                                                    }).ToListAsync();


                TourCarPageMainVM tourCarPageMainVM = new TourCarPageMainVM { CarIdNameVM = CarIdNames, TourCarVM = getTourCars };

                return new ResponseForTourByCarPage
                {
                    Data = tourCarPageMainVM,
                    Success = true,
                    StatusCode = "200",
                    Message = "Tour Prices Updated Successfully"
                };


            }
            catch (Exception ex) 
            {

                return new ResponseForTourByCarPage
                {
                    Data = new TourCarPageMainVM(),
                    Success = false,
                    StatusCode = "404",
                    Message = "Unhandled Error Occured"
                };
            }
           
        }

    }
}
