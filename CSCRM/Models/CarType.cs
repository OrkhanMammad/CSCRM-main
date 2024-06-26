namespace CSCRM.Models
{
    public class CarType : BaseEntity
    {
        public string Name { get; set; }
        public short Capacity { get; set; }

        public List<TourByCarType> TourByCarTypes { get; set; }
    }
}
