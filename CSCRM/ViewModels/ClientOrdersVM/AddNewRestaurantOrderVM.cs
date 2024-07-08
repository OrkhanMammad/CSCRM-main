namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class AddNewRestaurantOrderVM
    {
        public int ClientId { get; set; }
        public string RestaurantName { get; set; }
        public string MealType { get; set; }
        public short Count { get; set; }
        public string Date { get; set; }

    }
}
