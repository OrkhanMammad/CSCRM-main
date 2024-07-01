using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.HotelVMs;

namespace CSCRM.Abstractions
{
    public interface IHotelService
    {
        Task<BaseResponse> GetAllHotelsAsync(short pageIndex);
        Task<BaseResponse> AddHotelAsync(AddHotelVM addHotelVM, AppUser appUser);
        Task<BaseResponse> RemoveHotelAsync(int hotelId, AppUser appUser);
        Task<BaseResponse> GetHotelByIdAsync(int id);
        Task<BaseResponse> EditHotelAsync(EditHotelVM hotel, AppUser appUser);
    }
}
