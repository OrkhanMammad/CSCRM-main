using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.TourVMs;

namespace CSCRM.Abstractions
{
    public interface ITourService
    {
        Task<BaseResponse> GetAllToursAsync();
        Task<BaseResponse> RemoveTourAsync(int tourId, AppUser appUser);
        Task<BaseResponse> AddTourAsync(AddTourVM tourVM, AppUser appUser);

        Task<BaseResponse> GetTourByIdAsync(int tourId);

        Task<BaseResponse> EditTourAsync(EditTourVM tour, AppUser appUser);
    }
}
