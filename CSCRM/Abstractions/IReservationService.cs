using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.ReservationVMs;

namespace CSCRM.Abstractions
{
    public interface IReservationService
    {
        Task<BaseResponse> GetHotelOrdersAsync(int pageindex);
        Task<BaseResponse> GetReservationForEditAsync(int reservationid);
        Task<BaseResponse> EditReservationAsync(EditReservationDTO dto);
        
    }
}
