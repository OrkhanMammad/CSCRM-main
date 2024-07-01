using CSCRM.Abstractions;
using CSCRM.Areas.Manage.Controllers;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.TourCarVMs;
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

        public async Task<ResponseForTourByCarPage> AddTrCrTypAsync(AddTourCarVM tourCarVM)
        {            
            List<TourByCarType> tourByCarTypes = new List<TourByCarType>();
            foreach(var carPrice in tourCarVM.CarPrices)
            {                 
                tourByCarTypes.Add(new TourByCarType { CarTypeId = carPrice.Key, Price = carPrice.Value, TourId = tourCarVM.TourId });
            }

            await _context.TourByCarTypes.AddRangeAsync(tourByCarTypes);
            await _context.SaveChangesAsync();
            return new ResponseForTourByCarPage();
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


                List<GetTourIdNameVM> TourIdNames = await _context.Tours
                                                                  .Where(ct => ct.IsDeleted == false)
                                                                  .Select(ct => new GetTourIdNameVM
                                                                  {
                                                                      Id = ct.Id,
                                                                      Name = ct.Name
                                                                  }).ToListAsync();

                TourCarPageMainVM tourCarPageMainVM = new TourCarPageMainVM { CarIdNameVM = CarIdNames, TourIdNameVM = TourIdNames, TourCarVM = getTourCars };

                return new ResponseForTourByCarPage 
                { 
                    Data = tourCarPageMainVM, 
                    Success = true,
                    StatusCode="200",
                    
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


    }
}
