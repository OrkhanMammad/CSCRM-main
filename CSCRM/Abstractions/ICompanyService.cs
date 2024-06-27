using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CompanyVMs;

namespace CSCRM.Abstractions
{
    public interface ICompanyService
    {
        Task<BaseResponse> GetAllCompaniesAsync();
        Task<BaseResponse> AddCompanyAsync(AddCompanyVM companyVM, AppUser appUser);
        Task<BaseResponse> RemoveCompanyAsync(int hotelId, AppUser appUser);
        Task<BaseResponse> GetCompanyByIdAsync(int companyId);

        Task<BaseResponse> EditCompanyAsync(EditCompanyVM company, AppUser appUser);
    }
}
