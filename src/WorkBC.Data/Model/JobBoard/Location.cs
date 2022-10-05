using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    /// <summary>
    ///     This table was added to help implement BRD requirements on how to handle edge
    ///     case where two or more places in British Columbia share the same name.
    /// </summary>
    /// <remarks>
    ///     Most of this data was extracted from the EDM_Location table in the WorkBC_Enterprise
    ///     database. The SQL query used to extract the data is in a comment at the bottom of
    ///     this file.
    ///     For duplicate locations, the FederalCityId, Latitude and Longitude came from the
    ///     autocomplete on the federal job bank.  IsHidden and IsDuplicate were set manually.
    /// </remarks>
    public class Location
    {
        // Reference to WorkBC_Enterprise_DB.EDM_Location.LocationId
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int LocationId { get; set; }

        // Reference to WorkBC_Enterprise_DB.EDM_Location.LocationId
        [Column("EDM_Location_DistrictLocationId")]
        public int DistrictId { get; set; }

        // Reference to WorkBC_Enterprise_DB.EDM_Location.LocationId
        [ForeignKey("Region")]
        public int? RegionId { get; set; }

        public virtual Region Region { get; set; }

        // This references a field in the Federal called city_id
        // NOTES: 
        // - Only duplicate locations (e.g. Mill Bay, Summit Lake) have values in this field.
        // - For unique place names, we just assign regions to places based on the name and do not
        //   use the city_id. 
        // - The data for this field was obtained by manually scraping values from the autocomplete
        //   on the federal site.
        public int? FederalCityId { get; set; }

        public int? BcStatsPlaceId { get; set; }

        // The name of the city
        [StringLength(50)]
        public string City { get; set; }

        // Usually this is just the name of the city, but for duplicate locations it also contains the region name
        // (appended with a hyphen)
        [StringLength(50)]
        public string Label { get; set; }

        // Flag to indicate that this is one of the special cases where two places in BC have the
        // same name.
        public bool IsDuplicate { get; set; }

        // Flag to indicate this this field should be hidden (I suppose we could have just deleted these records)
        public bool IsHidden { get; set; }

        // This isn't really used for anything, but if somebody accidentally deletes the "Permanent" records
        // from the GeocodedLocationCache table then this data can be used to reconstruct the deleted data.
        // NOTE:  The data is also in the migrations 20191126214547_Geocoding__Add_Permanent_Records.cs and
        // 20191127194004_Geocoding__Sql_Cleanup.Designer.cs
        // Values for duplicates were copied & pasted from the federal API.
        // Values for unique locations were geocoded from the B.C. Address Geocoder (not Google maps)
        [StringLength(25)]
        public string Latitude { get; set; }

        // Same as Latitude, we are just keeping this here because the Permanent data in the GeocodedLocationCache
        // table might get purged one day, and it was a lot of work to manually collect all this data
        [StringLength(25)]
        public string Longitude { get; set; }
    }
}


// SQL query for rebuilding the table from the WorkBC_Enterprise_Dev database
/*
 
with r AS (select min (LocationId) as LocationId, LocationName, ParentLocationId 
from EDM_Location where LocationTypeID =3 group by LocationName, ParentLocationId),
Cities as (
select r.LocationID, r.LocationName as Place, l.LocationID as DistrictId, l.LocationName as District, l2.LocationID as RegionId, l2.LocationName as Region 
from r inner join EDM_location l on l.LocationID = r.ParentLocationID
inner join EDM_Location l2 on l2.LocationID = l.ParentLocationID)

select min(LocationId) As LocationId, DistrictId, RegionId, Place, District, Region 
from Cities where Place in (
select LocationName from 
r group by LocationName)
group by Place, District, Region, RegionId, DistrictId 
order by Place, LocationId

*/