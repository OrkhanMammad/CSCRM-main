namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class GetRestaurantOrdersVM
    {
        public int Id { get; set; }
        public string RestaurantName { get; set; }
        public int ClientId { get; set; }
        public string MealType { get; set; }
        public short Count { get; set; }
        public string Date { get; set; }
    }
}
