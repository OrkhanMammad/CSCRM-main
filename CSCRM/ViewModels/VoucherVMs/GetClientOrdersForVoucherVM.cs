namespace CSCRM.ViewModels.VoucherVMs
{
    public class GetClientOrdersForVoucherVM
    {
        //clientNAME, clientCarType, arrival date, departure date, COMPANYnAME, CompanyContactName, CompanyContactPhone, HotelOrderHotelname,
        //roomType, count, confirmationNo, checkin, checkout, TourOrderTourName, itineraries, inclusiveorderNames
        public string? ClientName { get; set; }
        public string? ClientCar { get; set; }
        
        public string? ArrivalDate { get; set; }
        public string? DepartureDate { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyContactPerson { get; set; }
        public string? CompanyContactPhone { get; set; }

           
    }
}
