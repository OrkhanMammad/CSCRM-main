using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.HotelVMs;

namespace CSCRM.Abstractions
{
    public interface IHotelService
    {
        Task<BaseResponse> GetAllHotelsAsync();
        Task<BaseResponse> AddHotelAsync(AddHotelVM addHotelVM);
        Task<BaseResponse> RemoveHotelAsync();
        Task<BaseResponse> GetHotelByIdAsync(int id);
    }
}
