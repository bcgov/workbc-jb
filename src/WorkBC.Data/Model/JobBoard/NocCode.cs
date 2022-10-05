using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class NocCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Id { get; set; }

        [StringLength(4)]
        public string Code { get; set; }

        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(180)]
        public string FrenchTitle { get; set; }
    }
}
