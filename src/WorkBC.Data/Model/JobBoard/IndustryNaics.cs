using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class IndustryNaics
    {
        public short IndustryId { get; set; }

        [ForeignKey("IndustryId")]
        public virtual Industry Industry { get; set; }

        public short NaicsId { get; set; }
    }
}