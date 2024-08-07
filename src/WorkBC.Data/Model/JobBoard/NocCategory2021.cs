﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkBC.Data.Enums;

namespace WorkBC.Data.Model.JobBoard
{
    public class NocCategory2021
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(4)]
        public string CategoryCode { get; set; }

        [Column(TypeName = "tinyint")]
        public NocCategoryLevel Level { get; set; }

        [StringLength(150)]
        public string Title { get; set; }
    }
}