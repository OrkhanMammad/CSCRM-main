namespace CSCRM.Models
{
    public class RestaurantOrder : BaseEntity
    {       
        public Client Client { get; set; }
        public int ClientID { get; set; }
        public string RestaurantName { get; set; }
        public string MealType { get; set; }
        public short Count { get; set; }
        public string Date { get; set; }

    }
}
