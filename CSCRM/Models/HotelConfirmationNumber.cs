namespace CSCRM.Models
{
    public class HotelConfirmationNumber 
    {
        public int Id { get; set; }
        public HotelOrder HotelOrder { get; set; }
        public int HotelOrderId { get; set; }
        public string? Number { get; set; } = string.Empty;
    }
}
