using System;

namespace WorkBC.Data.Model.JobBoard
{
    public interface IImportedJob
    {
        public long JobId { get; set; }
        JobId Id { get; set; }
        string JobPostEnglish { get; set; }
        string JobPostFrench { get; set; }
        DateTime DateFirstImported { get; set; }
        DateTime DateLastImported { get; set; }

        // For Federal Jobs, this is the date returned by the initial index list (the xml feed that lists all jobs)
        // For Wanted Jobs, this is the Refresh date
        DateTime ApiDate { get; set; }
    }
}