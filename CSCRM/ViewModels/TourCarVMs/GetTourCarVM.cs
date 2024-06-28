namespace CSCRM.ViewModels.TourCarVMs
{
    //sehifede her tour-a uygun olaraq her car type-in qiymetini gostermek ucun istifade olunur.
    public class GetTourCarVM
    {
        public string TourName { get; set; }
        public Dictionary<string, decimal> CarPrices { get; set; }
    }
}
