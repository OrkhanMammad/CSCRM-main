using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.TourCarVMs;

namespace CSCRM.Abstractions
{
    public interface ITourByCarTypeService
    {
        Task<ResponseForTourByCarPage> GetAllTrCrTypsAsync();
        Task<ResponseForTourByCarPage> AddTrCrTypAsync(AddTourCarVM tourCarVM);

    }
}
