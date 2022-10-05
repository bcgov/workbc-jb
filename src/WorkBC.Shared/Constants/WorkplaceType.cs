namespace WorkBC.Shared.Constants
{
    public enum WorkplaceTypeId
    {
        OnSite = 0,
        Hybrid = 100000,
        Travelling = 100001,
        Virtual = 15141
    }

    public static class WorkPlaceTypeEnglish
    {
        public const string OnSite = "On-site only";
        public const string Hybrid = "On-site or remote work";
        public const string Travelling = "Work location varies";
        public const string Virtual = "Virtual job";
    }

    public static class WorkPlaceTypeFrench
    {
        public const string OnSite = "Présentiel seulement";
        public const string Hybrid = "Présentiel ou télétravail";
        public const string Travelling = "Lieux de travail variés";
        public const string Virtual = "Emploi virtuel";
    }
}