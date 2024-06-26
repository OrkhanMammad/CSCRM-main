using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
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
        public async Task<BaseResponse> GetAllTrCrTyps()
        {
            List<TourByCarType> tourByCarTypes = await _context.TourByCarTypes.Where(tc => tc.IsDeleted == false).Include(tc=>tc.CarType).ToListAsync();
           var result = tourByCarTypes.GroupBy(t=>t.TourId);
            var a = 100;
            return new BaseResponse();
        }
    }
}
