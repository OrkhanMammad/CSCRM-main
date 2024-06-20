namespace CSCRM.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? DeletedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? CreatedBy { get; set; }
    }
}
