using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CompanyVMs;

namespace CSCRM.Abstractions
{
    public interface ICompanyService
    {
        Task<BaseResponse> GetAllCompaniesAsync();
        Task<BaseResponse> AddCompanyAsync(AddCompanyVM companyVM);
        Task<BaseResponse> RemoveCompanyAsync(int hotelId);
        Task<BaseResponse> GetCompanyByIdAsync(int companyId);

        Task<BaseResponse> EditCompanyAsync(EditCompanyVM company);
    }
}
