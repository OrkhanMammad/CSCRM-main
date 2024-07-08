namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class InclusiveOrdersSectionVM
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int ClientId { get; set; }
        public string StatusCode { get; set; }
        public List<string> InclusiveNames { get; set; } =  new List<string>();
        public List<GetInclusiveOrdersVM> InclusiveOrders { get; set; } = new List<GetInclusiveOrdersVM>();

    }
}
