namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class EditRestaurantOrderVM
    {
        public int Id { get; set; } = 0;
        public string RestaurantName { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public short Count { get; set; } = 0;
        public string Date { get; set; } = string.Empty;
        public List<string>? RestaurantsList { get; set; } = new List<string>();
    }
}
