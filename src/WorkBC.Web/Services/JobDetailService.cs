using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Web.Controllers;
using WorkBC.Web.Models.Search;

namespace WorkBC.Web.Services
{
    public class JobDetailService
    {
        private readonly JobDetailQuery _jobDetailQuery;
        private readonly ILogger<SearchController> _logger;

        public JobDetailService(IConfiguration configuration, ILogger<SearchController> logger = null)
        {
            //Set default values
            _jobDetailQuery = new JobDetailQuery(configuration);
            _logger = logger;
        }

        public async Task<ElasticSearchResponse> GetJobDetail(long jobId, JobBoardContext dbContext,
            ViewCountService viewCountService, string language, bool isToggle)
        {
            //Create new object to return

            //Get data from elastic search
            ElasticSearchResponse jobDetail = await _jobDetailQuery.GetSearchResults(jobId, language);

            //format necessary fields and set language where necessary
            jobDetail = FormatJobFields(jobDetail, language);

            // figure out if this is a Federal Job or a Wanted/Gartner job
            bool isFederalJob = (jobDetail?.Hits?.HitsHits?.Count ?? 0) > 0 
                                && (jobDetail?.Hits?.HitsHits[0]?.Source?.IsFederalJob ?? false);

            //Do not increase views if the user toggle between english and french or if it's not a federal job
            if (!isToggle && isFederalJob)
            {
                //Update view(s) for this job
                JobView jv = dbContext.JobViews.FirstOrDefault(j => j.JobId == jobId);

                int views;

                if (jv == null)
                {
                    //add new record - first view of job
                    var jvNew = new JobView
                    {
                        JobId = jobId,
                        DateLastViewed = DateTime.Now,
                        Views = 1
                    };

                    views = 1;
                    await dbContext.JobViews.AddAsync(jvNew);
                }
                else
                {
                    //Update views
                    jv.DateLastViewed = DateTime.Now;
                    jv.Views++;

                    views = jv.Views ?? 0;
                    dbContext.JobViews.Update(jv);
                }

                //update changes
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    _logger?.LogError($"Failed to update job view count. jobId = {jobId}");
                }

                //update views
                foreach (Source job in jobDetail.Hits.HitsHits.Select(hit => hit.Source).ToArray())
                {
                    job.Views = views;
                }

                //clear job from cache
                await viewCountService.RemoveJob(jobId);
            }

            //return
            return jobDetail;
        }

        public JobDetailsPageLabels SetTextHeader(string language)
        {
            bool english = language.ToLower().Equals("en");

            JobDetailsPageLabels labels = new JobDetailsPageLabels
            {
                JobPosting = english ? "Job Posting" : "Perspective d'emploi",
                Salary = english ? "Salary" : "Salaire",
                JobType = english ? "Job type" : "Type d'emploi",
                Language = english ? "Language" : "Langue",
                English = english ? "English" : "Anglais",
                PositionsAvailable = english ? "Positions available" : "Postes disponibles",
                NocGroup = english ? "2016 NOC group" : "Groupe CNP 2016",
                Education = english ? "Education" : "Études",
                ExpiresIn = english ? "Expires in" : "Expire dans",
                Days = english ? "days" : "jours",
                Expires = english ? "Expires" : "Expire le",
                Posted = english ? "Posted" : "Affichée",
                LastUpdated = english ? "Last updated" : "Dernière mise à jour",
                NumberOfViews = english ? "Number of views" : "Nombre de vues",
                Views = english ? "views" : "vues",
                JobNumber = english ? "Job number" : "Numéro de l'offre d'emploi",
                Save = english ? "Save" : "Sauvegarder",
                Print = english ? "Print" : "Imprimer",
                Share = english ? "Share" : "Partager",
                JobRequirements = english ? "Job Requirements" : "Exigences de l'emploi",
                JobLocations = english ? "Location" : "Ville",
                Experience = english ? "Experience" : "Expérience",
                Credentials = english
                    ? "Credentials (certificates, licences, memberships, courses, etc.)"
                    : "Titres de compétence (certificats, permis, affiliations, cours, etc,)",
                AdditionalSkills = english ? "Additional Skills" : "Autres compétences",
                WorkSetting = english ? "Work Setting" : "Milieu de travail",
                SpecificSkills = english ? "Specific Skills" : "Compétences particulières",
                SecuritySafety = english ? "Security and Safety" : "Sécurité et sûreté",
                WorksiteEnvironment = english ? "Work Site Environment" : "Environnement de milieu de travail",
                workLocationInformation = english ? "Work Location Information" : "Information sur le lieu de travail",
                PersonalSuitability = english ? "Personal Suitability" : "Qualités personnelles",
                ApplyNow = english ? "How to apply" : "Comment postuler",
                Online = english ? "Online" : "En ligne",
                ByEmail = english ? "By email" : "Par courriel",
                ByFax = english ? "By fax" : "Par fax",
                ByPhone = english ? "By phone" : "Par téléphone",
                Benefits = english ? "Benefits" : "Avantages",
                ByMail = english ? "By mail" : "Par courrier",
                InPerson = english ? "In person" : "En personne",
                StartDate = english ? "Start date" : "Date de début",
                AsSoonAsPossible = english ? "As soon as possible" : "Dès que possible",
                WorkSchedule = english ? "Work schedule" : "Horaire de travail",
                WorkplaceType = english ? "Workplace type" : "Lieu de travail"
            };

            return labels;
        }

        private ElasticSearchResponse FormatJobFields(ElasticSearchResponse job, string language)
        {
            ElasticSearchResponse retval = job;

            //ensure we have a job
            if (retval.Hits.HitsHits.ToArray().Length == 1)
            {
                //test if we have a postal code for MAIL
                if (retval.Hits.HitsHits[0].Source.ApplyMailPostalCode != null)
                {
                    //test if its a full postal code
                    if (retval.Hits.HitsHits[0].Source.ApplyMailPostalCode.Length == 6)
                    {
                        //add space in the middle
                        retval.Hits.HitsHits[0].Source.ApplyMailPostalCode = retval.Hits.HitsHits[0].Source.ApplyMailPostalCode.Substring(0, 3) + " " + retval.Hits.HitsHits[0].Source.ApplyMailPostalCode.Substring(3, 3);
                    }
                }

                //test if we have a postal code for IN PERSON
                if (retval.Hits.HitsHits[0].Source.ApplyPersonPostalCode != null)
                {
                    //test if its a full postal code
                    if (retval.Hits.HitsHits[0].Source.ApplyPersonPostalCode.Length == 6)
                    {
                        //add space in the middle
                        retval.Hits.HitsHits[0].Source.ApplyPersonPostalCode = retval.Hits.HitsHits[0].Source.ApplyPersonPostalCode.Substring(0, 3) + " " + retval.Hits.HitsHits[0].Source.ApplyPersonPostalCode.Substring(3, 3);
                    }
                }

                //test if we have a postal code
                if (retval.Hits.HitsHits[0].Source.PostalCode != null)
                {
                    //test if its a full postal code
                    if (retval.Hits.HitsHits[0].Source.PostalCode.Length == 6)
                    {
                        //add space in the middle
                        retval.Hits.HitsHits[0].Source.PostalCode = retval.Hits.HitsHits[0].Source.PostalCode.Substring(0, 3) + " " + retval.Hits.HitsHits[0].Source.PostalCode.Substring(3, 3);
                    }
                }


                if (language == "fr")
                {
                    //set Hours of work language
                    for (int k = 0; k < retval.Hits.HitsHits[0].Source.HoursOfWork.Description.Count; k++)
                    {
                        switch (retval.Hits.HitsHits[0].Source.HoursOfWork.Description[k].ToLower())
                        {
                            case "full-time":
                                retval.Hits.HitsHits[0].Source.HoursOfWork.Description[k] = "Plein temps";
                                break;
                            case "part-time":
                                retval.Hits.HitsHits[0].Source.HoursOfWork.Description[k] = "Temps partiel";
                                break;
                            case "part-time leading to full-time":
                                retval.Hits.HitsHits[0].Source.HoursOfWork.Description[k] = "Temps partiel puis plein temps";
                                break;
                        }
                    }

                    //set period of employment language
                    for (int k = 0; k < retval.Hits.HitsHits[0].Source.PeriodOfEmployment.Description.Count; k++)
                    {
                        switch (retval.Hits.HitsHits[0].Source.PeriodOfEmployment.Description[k].ToLower())
                        {
                            case "permanent":
                                retval.Hits.HitsHits[0].Source.PeriodOfEmployment.Description[k] = "Permanent";
                                break;
                            case "temporary":
                                retval.Hits.HitsHits[0].Source.PeriodOfEmployment.Description[k] = "Temporaire";
                                break;
                            case "casual":
                                retval.Hits.HitsHits[0].Source.PeriodOfEmployment.Description[k] = "Occasionnel";
                                break;
                            case "seasonal":
                                retval.Hits.HitsHits[0].Source.PeriodOfEmployment.Description[k] = "Saisonnier";
                                break;
                        }
                    }
                }
            }

            return retval;
        }
    }
}
