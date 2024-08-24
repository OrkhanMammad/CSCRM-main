namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class EditTourOrderVM
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string CarType { get; set; }
        public string Date { get; set; }
        public bool Guide { get; set; }
    }
}
