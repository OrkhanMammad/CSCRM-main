namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class HotelTourRestaurantInclusiveOrdersTotal
    {
        public int? ClientId { get; set; }
        public string InvCode { get; set; }
        public string MailCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public TourOrdersSectionVM TourOrdersSection { get; set; } = new TourOrdersSectionVM();
        public HotelOrdersSectionVM HotelOrdersSection { get; set; } = new HotelOrdersSectionVM();
        public RestaurantOrdersSectionVM RestaurantOrdersSection { get;set; } = new RestaurantOrdersSectionVM();
        public InclusiveOrdersSectionVM InclusiveOrdersSection { get; set; } = new InclusiveOrdersSectionVM();
    }
}
