namespace CSCRM.Models
{
    public class InclusiveOrder : BaseEntity
    {
        public int Id { get; set; }
        
        public string InclusiveName { get; set; }
        public short Count { get; set; }
        public Client Client { get; set; }
        public int ClientId { get; set; }
        public string Date { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? DeletedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? CreatedBy { get; set; }
    }
}
