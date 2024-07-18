namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class GetHotelOrdersVM
    {
      public int Id {  get; set; } 
      public int ClientId {  get; set; }
      public string HotelName {  get; set; }
      public string RoomType {  get; set; }
      public short RoomCount {  get; set; }
      public short Days {  get; set; }
      public string DateFrom {  get; set; }
      public string DateTo {  get; set; }
      public List<string>? ConfirmationNumbers { get; set; }
    }
}
