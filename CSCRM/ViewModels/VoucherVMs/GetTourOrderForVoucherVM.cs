namespace CSCRM.ViewModels.VoucherVMs
{
    public class GetTourOrderForVoucherVM
    {
        public string? Date { get; set; } = string.Empty;
       public GetTourForVoucherVM? Tour {  get; set; } = new();
    }
}
