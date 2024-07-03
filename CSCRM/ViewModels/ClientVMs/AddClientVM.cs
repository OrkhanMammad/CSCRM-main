﻿namespace CSCRM.ViewModels.ClientVMs
{
    public class AddClientVM
    {
        public string InvCode { get; set; }
        public string MailCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PaySituation { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal Received { get; set; }
        public string VisaSituation { get; set; }
        public string? Country { get; set; }
        public string? Company { get; set; }
        public DateOnly? ArrivalDate { get; set; }
        public DateOnly? DepartureDate { get; set; }
    }
}
