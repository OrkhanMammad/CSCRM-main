using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.TourCarVMs;

namespace CSCRM.Abstractions
{
    public interface ITourByCarTypeService
    {
        Task<BaseResponse> GetAllTrCrTypsAsync();
        Task<BaseResponse> AddTrCrTypAsync(AddTourCarVM tourCarVM);

    }
}
