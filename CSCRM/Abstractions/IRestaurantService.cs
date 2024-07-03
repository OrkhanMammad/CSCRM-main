using CSCRM.Models.ResponseTypes;
using CSCRM.Models;
using CSCRM.ViewModels.RestaurantVMs;

namespace CSCRM.Abstractions
{
    public interface IRestaurantService
    {
        Task<BaseResponse> GetAllRestaurantsAsync(short pageIndex);
        Task<BaseResponse> AddRestaurantAsync(AddRestaurantVM addRestaurantVM, AppUser appUser);
        Task<BaseResponse> RemoveRestaurantAsync(int restaurantId, AppUser appUser);
        Task<BaseResponse> GetRestaurantByIdAsync(int id);
        Task<BaseResponse> EditRestaurantAsync(EditRestaurantVM restaurant, AppUser appUser);

    }
}
