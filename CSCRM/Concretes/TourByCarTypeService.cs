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

        public async Task<BaseResponse> AddTrCrTypAsync(AddTourCarVM tourCarVM)
        {
            

            List<TourByCarType> tourByCarTypes = new List<TourByCarType>();
            foreach(var carPrice in tourCarVM.CarPrices)
            {                 
                tourByCarTypes.Add(new TourByCarType { CarTypeId = carPrice.Key, Price = carPrice.Value, TourId = tourCarVM.TourId });
            }
            await _context.TourByCarTypes.AddRangeAsync(tourByCarTypes);
            await _context.SaveChangesAsync();
            return new BaseResponse();






        }

        public Task<BaseResponse> GetAllTrCrTypsAsync()
        {
            throw new NotImplementedException();
        }


    }
}
