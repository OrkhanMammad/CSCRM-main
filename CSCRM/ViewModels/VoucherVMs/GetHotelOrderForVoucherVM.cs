namespace CSCRM.ViewModels.VoucherVMs
{
    public class GetHotelOrderForVoucherVM
    {
        public string? HotelName { get; set; } = string.Empty;
        public short? Count { get; set; } = 0;
        public string? RoomType { get; set; } = string.Empty;
        public List<string>? ConfirmationNumbers { get; set; } = new List<string>();

        public string? FromDate { get; set; } = string.Empty;
        public string? ToDate { get; set;} = string.Empty;
    }
}
