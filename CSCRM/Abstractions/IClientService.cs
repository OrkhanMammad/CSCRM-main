using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
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
    }
}
