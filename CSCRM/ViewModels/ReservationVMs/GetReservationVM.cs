namespace CSCRM.ViewModels.ReservationVMs
{
    public class GetReservationVM
    {
        public int HotelOrderId {  get; set; }
        public string ClientNameSurname { get; set; }
        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public short RoomCount { get; set; }
        public short Days { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public List<string>? ConfirmationNumbers { get; set; }   
                             
    }
}
