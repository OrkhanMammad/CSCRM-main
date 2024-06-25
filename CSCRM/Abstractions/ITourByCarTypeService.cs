using CSCRM.Models.ResponseTypes;

namespace CSCRM.Abstractions
{
    public interface ITourByCarTypeService
    {
        Task<BaseResponse> GetAllTrCrTyps();
    }
}
