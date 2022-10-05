using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Extensions
{
    public static class ValidationExtensions
    {
        public static void ValidateDateRangePicker(this ModelStateDictionary state, IDateRangePicker model)
        {
            if (model.StartDate == null || model.EndDate == null)
            {
                state.AddModelError("StartDate", "From and To are both required.");
                state.AddModelError("EndDate", "From and To are both required.");
            }

            if (model.StartDate > model.EndDate)
            {
                state.AddModelError("StartDate", "Must be before To.");
                state.AddModelError("EndDate", "Must be after From.");
            }
        }

        public static void ValidateRegion(this ModelStateDictionary state, short? regionId)
        {
            if (regionId == null || regionId == 0)
            {
                state.AddModelError("RegionId", "Region is required.");
            }
        }

        public static void ValidateMatrixDateRangeParams(this ModelStateDictionary state, IMatrixDateRangeParams model)
        {
            if (model.DateTypeToggle == "yearly")
            {
                if (model.FiscalYearTo < model.FiscalYearFrom)
                {
                    state.AddModelError("FiscalYearFrom", "From Date is after To Date.");
                }
            }

            if (model.DateTypeToggle == "monthly")
            {
                var startMonth = new DateTime(model.MonthlyStartYear, model.MonthlyStartMonth, 1);
                var endMonth = new DateTime(model.MonthlyEndYear, model.MonthlyEndMonth, 1);

                if (startMonth > endMonth)
                {
                    state.AddModelError("MonthlyStartMonth", "From Date is after To Date.");
                }
                else if (endMonth > DateTime.Now && model.MonthlyRangeType == "custom")
                {
                    state.AddModelError("MonthlyStartMonth", "Future dates cannot be reported on. Please select a new date range.");
                }
                else if (startMonth.AddMonths(11) < endMonth)
                {
                    state.AddModelError("MonthlyStartMonth", "Period cannot exceed 12 months.");
                }
            }

            if (model.DateTypeToggle == "weekly")
            {
                var month = new DateTime(model.WeeklyYear, model.WeeklyMonth, 1);

                if (month > DateTime.Now)
                {
                    state.AddModelError("WeeklyMonth", "Future dates cannot be reported on. Please select a new date range.");
                }
            }
        }
    }
}