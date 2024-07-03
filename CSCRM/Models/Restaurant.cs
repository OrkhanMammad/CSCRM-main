using System.Reflection;

namespace CSCRM.Models
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public decimal? Lunch { get; set; }
        public decimal? Dinner { get; set; }
        public decimal? Gala_Dinner_Simple { get; set; }
        public decimal? Gala_Dinner_Local_Alc { get; set; }
        public decimal? Gala_Dinner_Foreign_Alc { get; set; }
        public decimal? TakeAway { get; set; }

    }
}
