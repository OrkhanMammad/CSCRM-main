namespace CSCRM.ViewModels.TourCarVMs
{
    //TourCarType sehifesinde yeni tourCarType elave ederken Tour secimi ucun id ve name deyerlerini bildirmek.
    //Clients services(orders) sehifesinde yeni tour service(order) elave ederken istifade olunacaq
    public class GetTourIdNameVM
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
