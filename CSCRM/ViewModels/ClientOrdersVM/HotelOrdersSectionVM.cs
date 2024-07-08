namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class HotelOrdersSectionVM
    {
        //ClientsOrders sehifesinde hotel-order sectionunda istifade olunacaq datalari gondermek ucun.
        public string Message { get; set; }
        public bool Success { get; set; }
        public string StatusCode { get; set; }
        public int ClientId { get; set; }
        public List<string> HotelNames { get; set; } = new List<string>();
        public List<GetHotelOrdersVM> HotelOrders { get; set; } = new List<GetHotelOrdersVM>();
    }
}
