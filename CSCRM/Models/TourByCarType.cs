namespace CSCRM.Models
{
    public class TourByCarType : BaseEntity
    {
        public Tour Tour { get; set; }
        public int TourId { get; set; }

        public CarType CarType { get; set; }

        public int CarTypeId { get; set; }
    }
}
