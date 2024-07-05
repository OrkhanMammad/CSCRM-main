using CSCRM.Models;

namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class GetTourOrdersVM
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string TourName { get; set; }
        public string CarType { get; set; }
        public bool Guide { get; set; }
        public string Date { get; set; }
    }
}
