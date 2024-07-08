namespace CSCRM.ViewModels.TourCarVMs
{
    //TourCarType sehifesindeki table-da carTypelarin adlarini table head olaraq eks etdirmek ucun. Id deyeri ise yeni tourCarType 
    //elave ederken input degerlerine verilmesi ucundur
    //Clients services(orders) sehifesinde yeni tour service(order) elave ederken istifade olunacaq

    public class GetCarIdNameVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
