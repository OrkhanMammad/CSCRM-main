namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class EditInclusiveOrderVM
    {
        public int Id { get; set; } = 0;
        public string InclusiveName { get; set; } = string.Empty;
        public short Count { get; set; } = 0;
        public string Date { get; set; } = string.Empty;
        public List<string>? InclusivesList { get; set; } = new List<string>();
    }
}
