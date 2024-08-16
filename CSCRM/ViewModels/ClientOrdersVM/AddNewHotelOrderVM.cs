namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class AddNewHotelOrderVM
    {
        public string ClientNameSurname { get; set; }
        public int ClientId { get; set; }
        public string? HotelName { get; set; }
        public short RoomCount { get; set; }
        public short Days {  get; set; }
        public string? RoomType { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
}
