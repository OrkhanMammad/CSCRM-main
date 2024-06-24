using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CarVMs;

namespace CSCRM.Abstractions
{
    public interface ICarService
    {
        Task<BaseResponse> GetAllCarsAsync();
        Task<BaseResponse> AddCarAsync(AddCarVM carVM);
    }
}
