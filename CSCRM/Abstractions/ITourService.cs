using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.TourVMs;

namespace CSCRM.Abstractions
{
    public interface ITourService
    {
        Task<BaseResponse> GetAllToursAsync();
        Task<BaseResponse> RemoveTourAsync(int tourId);
        Task<BaseResponse> AddTourAsync(AddTourVM tourVM);
    }
}
