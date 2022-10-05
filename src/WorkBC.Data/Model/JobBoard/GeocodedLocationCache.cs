using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class GeocodedLocationCache
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(120)]
        public string Name { get; set; }

        [StringLength(25)]
        public string Latitude { get; set; }

        [StringLength(25)]
        public string Longitude { get; set; }

        [StringLength(80)]
        public string City { get; set; }

        [StringLength(80)]
        public string FrenchCity { get; set; }

        [StringLength(2)]
        public string Province { get; set; }

        public DateTime DateGeocoded { get; set; }

        public bool IsPermanent { get; set; }
    }
}