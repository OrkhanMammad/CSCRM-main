namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class GetInclusiveOrdersVM
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string InclusiveName { get; set; }
        public short Count { get; set; }
        public string Date { get; set; }

    }
}
