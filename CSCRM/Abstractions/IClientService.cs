using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.ClientOrdersVM;
using CSCRM.ViewModels.ClientVMs;

namespace CSCRM.Abstractions
{
    public interface IClientService
    {
        Task<BaseResponse> GetAllClientsAsync(short pageIndex);
        Task<BaseResponse> AddClientAsync(AddClientVM clientVM, AppUser appUser);
        Task<BaseResponse> DeleteClientAsync(int clientId, AppUser appUser);
        Task<BaseResponse> GetClientForEditInfo(int clientId);
        Task<BaseResponse> EditClientInfoAsync(EditClientInfoVM clientVM, AppUser appUser);
        Task<BaseResponse> GetClientServicesAsync(int clientId);
        Task<HotelOrdersSectionVM> DeleteHotelOrderAsync(int clientId, int hotelOrderId, AppUser appUser);
        Task<HotelOrdersSectionVM> AddNewHotelOrderAsync(AddNewHotelOrderVM hotelOrder, AppUser appUser);
        Task<TourOrdersSectionVM> DeleteTourOrderAsync(int clientId, int tourOrderId, AppUser appUser);
        Task<TourOrdersSectionVM> AddNewTourOrderAsync(AddNewTourOrderVM newOrder, AppUser appUser);
        Task<RestaurantOrdersSectionVM> DeleteRestaurantOrderAsync(int clientId, int restaurantOrderId, AppUser appUser);
        Task<RestaurantOrdersSectionVM> AddNewRestaurantOrderAsync(AddNewRestaurantOrderVM newOrder, AppUser appUser);
        Task<InclusiveOrdersSectionVM> DeleteInclusiveOrderAsync(int clientId, int inclusiveOrderId, AppUser appUser);
        Task<InclusiveOrdersSectionVM> AddNewInclusiveOrderAsync(AddNewInclusiveOrderVM newOrder, AppUser appUser);
        Task GetInvoiceAsync(int clientId);
    }
}
