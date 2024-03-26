using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WorkBC.Data.Model.JobBoard
{
    public class NocCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Id { get; set; }

        [StringLength(5)]
        public string Code { get; set; }

        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(180)]
        public string FrenchTitle { get; set; }
        
        [StringLength(100)]
        public string Noc2016 { get; set; }

        [NotMapped]
        public List<string> Noc2016List
        {
            get => Noc2016.Split(',').ToList();
            set => throw new NotImplementedException();
        }
    }
}
