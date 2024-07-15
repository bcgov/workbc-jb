using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkBC.Data;
using WorkBC.Data.Enums;

namespace WorkBC.Admin.Services
{
    public class SelectListService
    {
        private readonly JobBoardContext _jobBoardContext;

        public SelectListService(JobBoardContext jobBoardContext)
        {
            _jobBoardContext = jobBoardContext;
        }

        /// <summary>
        ///     Broad occupational categories for NOC Code Summary report
        /// </summary>
        public IEnumerable<SelectListItem> BroadOccupationalCategories
        {
            get
            {
                return _jobBoardContext.NocCategories2021
                    .Where(c => c.Level == NocCategoryLevel.BroadOccupationalCategory)
                    .OrderBy(c => c.CategoryCode)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryCode,
                        Text = $"{c.CategoryCode} – {c.Title}"
                    });
            }
        }

        /// <summary>
        ///     Major groups for NOC Code Summary report
        /// </summary>
        public IEnumerable<SelectListItem> MajorGroups
        {
            get
            {
                return _jobBoardContext.NocCategories2021
                    .Where(c => c.Level == NocCategoryLevel.MajorGroup)
                    .OrderBy(c => c.CategoryCode)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryCode,
                        Text = $"{c.CategoryCode} – {c.Title}"
                    });
            }
        }

        /// <summary>
        ///     Sub-major groups for NOC Code Summary report
        /// </summary>
        public IEnumerable<SelectListItem> SubMajorGroups
        {
            get
            {
                return _jobBoardContext.NocCategories2021
                    .Where(c => c.Level == NocCategoryLevel.SubMajorGroup)
                    .OrderBy(c => c.CategoryCode)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryCode,
                        Text = $"{c.CategoryCode} – {c.Title}"
                    });
            }
        }

        /// <summary>
        ///     Minor groups for NOC Code Summary report
        /// </summary>
        public IEnumerable<SelectListItem> MinorGroups
        {
            get
            {
                return _jobBoardContext.NocCategories2021
                    .Where(c => c.Level == NocCategoryLevel.MinorGroup)
                    .OrderBy(c => c.CategoryCode)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryCode,
                        Text = $"{c.CategoryCode} – {c.Title}"
                    });
            }
        }

        /// <summary>
        ///     Unit groups for NOC Code 2021 Summary report
        /// </summary>
        public IEnumerable<SelectListItem> UnitGroups
        {
            get
            {
                return _jobBoardContext.NocCodes2021
                    .OrderBy(c => c.Code)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Code,
                        Text = $"{c.Code} – {c.Title}"
                    });
            }
        }

        /// <summary>
        ///     Region dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> Regions
        {
            get
            {
                return _jobBoardContext.Regions.AsNoTracking()
                    .Where(l => !l.IsHidden)
                    .OrderBy(l => l.ListOrder)
                    .Select(l => new SelectListItem(l.Name, l.Id.ToString()));
            }
        }

        /// <summary>
        ///     Region dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> RegionsIncludingHidden
        {
            get
            {
                return _jobBoardContext.Regions.AsNoTracking()
                    .OrderBy(l => l.ListOrder)
                    .Select(l => new SelectListItem(l.Name, l.Id.ToString()));
            }
        }

        /// <summary>
        ///     Country dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> Countries
        {
            get
            {
                return _jobBoardContext.Countries.AsNoTracking()
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.Name)
                    .Select(country =>
                        new SelectListItem
                        {
                            Text = country.Name.ToString(),
                            Value = country.Id.ToString(),
                            Selected = false
                        });
            }
        }

        /// <summary>
        ///     Province dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> Provinces
        {
            get
            {
                return _jobBoardContext.Provinces.AsNoTracking()
                    .Select(province => new SelectListItem
                    {
                        Text = province.Name.ToString(),
                        Value = province.ProvinceId.ToString(),
                        Selected = false
                    });
            }
        }

        /// <summary>
        ///     Month dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> Months =>
            new List<SelectListItem>
            {
                new SelectListItem("Jan", "1"),
                new SelectListItem("Feb", "2"),
                new SelectListItem("Mar", "3"),
                new SelectListItem("Apr", "4"),
                new SelectListItem("May", "5"),
                new SelectListItem("Jun", "6"),
                new SelectListItem("Jul", "7"),
                new SelectListItem("Aug", "8"),
                new SelectListItem("Sep", "9"),
                new SelectListItem("Oct", "10"),
                new SelectListItem("Nov", "11"),
                new SelectListItem("Dec", "12")
            };

        /// <summary>
        ///     Job year dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> JobYears
        {
            get
            {
                List<int> years = _jobBoardContext.Jobs
                    .GroupBy(j => j.DateFirstImported.Year)
                    .Select(g => g.Key)
                    .OrderBy(y => y)
                    .ToList();

                if (DateTime.Now.Month >= 4)
                {
                    years = years.Append(years.Max() + 1).ToList();
                }

                var items = new List<SelectListItem>();

                foreach (int year in years)
                {
                    items.Add(new SelectListItem(year.ToString(), year.ToString()));
                }

                return items;
            }
        }

        /// <summary>
        ///     Job Fiscal year dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> JobFiscalYears
        {
            get
            {
                return JobYears.Select(i =>
                    new SelectListItem($"{int.Parse(i.Value) - 1}/{int.Parse(i.Value)}", i.Value));
            }
        }

        /// <summary>
        ///     Job seeker year dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> JobSeekerYears
        {
            get
            {
                int lastYear = DateTime.Now.Year;

                if (DateTime.Now.Month >= 4)
                {
                    lastYear++;
                }

                var items = new List<SelectListItem>();

                for (var year = 2013; year <= lastYear; year++)
                    items.Add(new SelectListItem(year.ToString(), year.ToString()));

                return items;
            }
        }

        /// <summary>
        ///     Job Fiscal year dropdown options
        /// </summary>
        public IEnumerable<SelectListItem> JobSeekerFiscalYears
        {
            get
            {
                return JobSeekerYears.Select(i =>
                    new SelectListItem($"{int.Parse(i.Value) - 1}/{int.Parse(i.Value)}", i.Value));
            }
        }

        public IEnumerable<SelectListItem> AdminLevels =>
            new List<SelectListItem>
            {
                new SelectListItem("Reporting", "1", true),
                new SelectListItem("Regular administrator", "2"),
                new SelectListItem("Super administrator", "3")
            };

        public IEnumerable<SelectListItem> SecurityQuestions
        {
            get
            {
                return _jobBoardContext.SecurityQuestions
                    .Select(s => new SelectListItem
                    {
                        Text = s.QuestionText,
                        Value = s.Id.ToString()
                    });
            }
        }

    }
}
