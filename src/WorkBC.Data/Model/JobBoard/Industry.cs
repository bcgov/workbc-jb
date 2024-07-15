using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class Industry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Id { get; set; }

        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(150)]
        public string TitleBC { get; set; }
    }
}
