namespace CSCRM.ViewModels.HotelVMs
{
    public class AddHotelVM
    {
        public string Name { get; set; }
        public decimal? SinglePrice { get; set; }
        public decimal? DoublePrice { get; set; }
        public decimal? TriplePrice { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
    }
}
