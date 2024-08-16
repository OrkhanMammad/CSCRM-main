using CSCRM.Models.ResponseTypes;

namespace CSCRM.Abstractions
{
    public interface IReservationService
    {
        Task<BaseResponse> GetHotelOrdersAsync();
        Task AddConfirmationNumberToOrderAsync();
    }
}
