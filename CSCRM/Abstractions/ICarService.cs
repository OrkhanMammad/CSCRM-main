using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CarVMs;

namespace CSCRM.Abstractions
{
    public interface ICarService
    {
        Task<BaseResponse> GetAllCarsAsync();
        Task<BaseResponse> AddCarAsync(AddCarVM carVM, AppUser appUser);
        Task<BaseResponse> RemoveCarAsync(int carId, AppUser appUser);
        Task<BaseResponse> GetCarByIdAsync(int carId);
        Task<BaseResponse> EditCarAsync(EditCarVM car, AppUser appUser);
    }
}
