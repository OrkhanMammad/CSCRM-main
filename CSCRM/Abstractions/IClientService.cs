using CSCRM.Models.ResponseTypes;

namespace CSCRM.Abstractions
{
    public interface IClientService
    {
        Task<BaseResponse> GetClientsAsync();

    }
}
