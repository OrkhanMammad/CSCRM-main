using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.TourCarVMs;

namespace CSCRM.Abstractions
{
    public interface ITourByCarTypeService
    {
        Task<ResponseForTourByCarPage> GetAllTrCrTypsAsync();
        Task CreateTourByCarTypeAsyncWhenNewCarCreating(int carTypeId);
        Task RemoveTourByCarTypeAsyncWhenCarRemoving(int carTypeId);

        Task RemoveTourByCarTypeAsyncWhenTourRemoving(int TourId);
        Task CreateTourByCarTypeAsyncWhenNewTourCreating(int TourId);
        Task<ResponseForTourByCarPage> GetTourCarForEditByTourName(string tourName);
    }
}
