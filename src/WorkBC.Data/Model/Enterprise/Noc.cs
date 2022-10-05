using System.Collections.Generic;

namespace WorkBC.Data.Model.Enterprise
{
    public partial class Noc
    {
        public Noc()
        {
            InverseNocId2006Navigation = new HashSet<Noc>();
            InverseParentNoc = new HashSet<Noc>();
        }

        public int NocId { get; set; }
        public string Noccode { get; set; }
        public string NameEnglish { get; set; }
        public string NameFrench { get; set; }
        public string EnglishShortAlias { get; set; }
        public string Nocs2006 { get; set; }
        public byte? NocgroupTypeId { get; set; }
        public int? ParentNocId { get; set; }
        public int Nocyear { get; set; }
        public int? NocId2006 { get; set; }
        public bool? TradesOutlookNoc { get; set; }
        public string SkillLevel { get; set; }

        public virtual Noc NocId2006Navigation { get; set; }
        public virtual Noc ParentNoc { get; set; }
        public virtual CareerProfile CareerProfile { get; set; }
        public virtual ICollection<Noc> InverseNocId2006Navigation { get; set; }
        public virtual ICollection<Noc> InverseParentNoc { get; set; }
    }
}