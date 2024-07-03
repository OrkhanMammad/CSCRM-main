using CSCRM.Models.ResponseTypes;
using CSCRM.Models;
using CSCRM.ViewModels.CarVMs;
using CSCRM.ViewModels.InclusivesVMs;

namespace CSCRM.Abstractions
{
    public interface IIncludedService
    {
        Task<BaseResponse> GetAllInclusivesAsync();
        Task<BaseResponse> AddInclusiveAsync(AddNewInclusiveVM inclusiveVM, AppUser appUser);
        Task<BaseResponse> RemoveInclusiveAsync(int nclusiveId, AppUser appUser);
        Task<BaseResponse> GetInclusiveByIdAsync(int inclusiveId);
        Task<BaseResponse> EditInclusiveAsync(EditInclusiveVM inclusive, AppUser appUser);

    }
}
