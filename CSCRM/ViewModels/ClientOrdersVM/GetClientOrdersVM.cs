

namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class GetClientOrdersVM
    {
        public int Id { get; set; } //Clientin oz id-si
        public string Name { get; set; }
        public string Surname { get; set; }
        public string InvCode { get; set; }
        public string MailCode { get; set; }
        public List<GetHotelOrdersVM>? HotelOrders { get; set; }
        public List<GetTourOrdersVM>? TourOrders { get; set; }
        public List<GetRestaurantOrdersVM>? RestaurantOrders { get; set; }
        public List<GetInclusiveOrdersVM>? InclusiveOrders { get; set; }

    }
}
