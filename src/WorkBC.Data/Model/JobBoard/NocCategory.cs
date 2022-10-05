using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkBC.Data.Enums;

namespace WorkBC.Data.Model.JobBoard
{
    public class NocCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(3)]
        public string CategoryCode { get; set; }

        [Column(TypeName = "tinyint")]
        public NocCategoryLevel Level { get; set; }

        [StringLength(150)]
        public string Title { get; set; }
    }
}