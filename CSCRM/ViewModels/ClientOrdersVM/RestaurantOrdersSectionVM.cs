namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class RestaurantOrdersSectionVM
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int ClientId { get; set; }
        public string StatusCode { get; set; }
        public List<string> RestaurantNames { get; set; } = new List<string>();
        public List<GetRestaurantOrdersVM> RestaurantOrders { get; set; } = new List<GetRestaurantOrdersVM>();
    }
}
