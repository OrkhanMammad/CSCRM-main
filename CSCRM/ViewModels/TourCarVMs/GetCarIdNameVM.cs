namespace CSCRM.ViewModels.TourCarVMs
{
    //TourCarType sehifesindeki table-da carTypelarin adlarini table head olaraq eks etdirmek ucun. Id deyeri ise yeni tourCarType 
    //elave ederken input degerlerine verilmesi ucundur
    public class GetCarIdNameVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
