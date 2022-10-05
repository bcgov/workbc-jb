using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Admin.Controllers;
using WorkBC.Data;
using System.Linq;
using WorkBC.Admin.Areas.SystemSettings.Models;
using WorkBC.Data.Model.JobBoard;
using System;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using WorkBC.Admin.Services;
using WorkBC.Data.Enums;
using WorkBC.Shared.Constants;
using static System.String;

namespace WorkBC.Admin.Areas.SystemSettings.Controllers
{
    [Authorize(Roles = MinAccessLevel.SuperAdmin)]
    [Area("SystemSettings")]
    [Route("system-settings/[controller]/[action]")]
    public class SettingsController : AdminControllerBase
    {
        private readonly JobBoardContext _jobBoardContext;
        private readonly IDistributedCache _cache;

        public SettingsController(JobBoardContext jobBoardContext, IDistributedCache cache)
        {
            _jobBoardContext = jobBoardContext;
            _cache = cache;
        }

        public IActionResult Index()
        {
            var allSettings = _jobBoardContext.SystemSettings.ToList();
            var result = new SystemSettingManagementViewModel()
            {
                Results = allSettings,
                TotalUsers = allSettings.Count
            };

            return View("Index", result);
        }

        [HttpGet]
        [Route("/system-settings/Settings/Edit/{name}")]
        public IActionResult Edit(string name)
        {
            //find setting
            SystemSetting setting = _jobBoardContext.SystemSettings.FirstOrDefault(s => s.Name == name);

            if (setting == null)
            {
                return NotFound();
            }

            //set view model
            var model = new SystemSettingViewModel
            {
                FieldType = setting.FieldType,
                Description = setting.Description,
                Name = setting.Name,
                Value = setting.Value
            };

            switch (setting.FieldType)
            {
                case SystemSettingFieldType.Boolean:
                    model.ValueBoolean = setting.Value;
                    break;
                case SystemSettingFieldType.MultiLineText:
                    model.ValueMultiLine = setting.Value;
                    break;
                case SystemSettingFieldType.Number:
                    model.ValueNumber = setting.Value;
                    break;
                case SystemSettingFieldType.SingleLineText:
                    model.ValueSingleLine = setting.Value;
                    break;
                case SystemSettingFieldType.Html:
                    model.ValueHtml = setting.Value;
                    break;
            }

            return View("edit", model);
        }

        [HttpPost]
        public IActionResult Update(SystemSettingViewModel model)
        {
            var showError = false;

            //Find setting with ID
            SystemSetting setting = _jobBoardContext.SystemSettings.FirstOrDefault(s => s.Name == model.Name);

            if (setting == null)
            {
                return NotFound();
            }

            try
            {
                //Update setting with new values

                var wasUpdated = false;

                switch (setting.FieldType)
                {
                    case SystemSettingFieldType.Boolean:
                        if (model.ValueBoolean != null)
                        {
                            (setting.Value, wasUpdated) = Assign(setting.Value, model.ValueBoolean);
                        }
                        else
                        {
                            showError = true;
                        }

                        break;

                    case SystemSettingFieldType.MultiLineText:
                        if (!IsNullOrEmpty(model.ValueMultiLine))
                        {
                            (setting.Value, wasUpdated) = Assign(setting.Value, model.ValueMultiLine);
                        }
                        else if (model.ValueIsOptional)
                        {
                            (setting.Value, wasUpdated) = Assign(setting.Value, "");
                        }
                        else
                        {
                            showError = true;
                        }

                        break;

                    case SystemSettingFieldType.Number:
                        if (!IsNullOrEmpty(model.ValueNumber))
                        {
                            (setting.Value, wasUpdated) = Assign(setting.Value, model.ValueNumber);
                        }
                        else
                        {
                            showError = true;
                        }

                        break;

                    case SystemSettingFieldType.SingleLineText:
                        if (!IsNullOrEmpty(model.ValueSingleLine))
                        {
                            (setting.Value, wasUpdated) = Assign(setting.Value, model.ValueSingleLine);
                        }
                        else if (model.ValueIsOptional)
                        {
                            (setting.Value, wasUpdated) = Assign(setting.Value, "");
                        }
                        else
                        {
                            showError = true;
                        }

                        break;

                    case SystemSettingFieldType.Html:
                        if (!IsNullOrEmpty(model.ValueHtml))
                        {
                            (setting.Value, wasUpdated) = Assign(setting.Value, model.ValueHtml);
                        }
                        else if (model.ValueIsOptional)
                        {
                            (setting.Value, wasUpdated) = Assign(setting.Value, "");
                        }
                        else
                        {
                            showError = true;
                        }

                        break;
                }

                if (!showError)
                {
                    if (wasUpdated)
                    {
                        setting.DateUpdated = DateTime.Now;
                        setting.ModifiedByAdminUserId = CurrentAdminUserId;
                    }

                    //Update setting
                    _jobBoardContext.SystemSettings.Update(setting);
                    _jobBoardContext.SaveChanges();

                    // Invalidated cached settings in WorkBC.Web
                    InvalidateCachedSettingsInTheApiSite();

                    //Go back to the listing page
                    return RedirectToAction("Index", "Settings");
                }

                ModelState.AddModelError("error", "Please complete all fields");

                View("edit", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", $"Unexpected error: {ex.Message}");
            }

            //go back to edit screen with errors on screen (invalid values for the fields)
            return View("Edit", model);
        }

        /// <summary>
        ///     Assigns a model value to a setting value and returns true if the value changed.
        /// </summary>
        private (string newValue, bool isChanged) Assign(string settingValue, string modelValue)
        {
            return settingValue == modelValue
                ? (settingValue, false)
                : (modelValue, true);
        }

        /// <summary>
        ///     Changes a value in the Redis cache, which will cause a singleton service in the WorkBC.Web
        ///     project to refresh itself.
        /// </summary>
        private void InvalidateCachedSettingsInTheApiSite()
        {
            // set the cache control value to DateTime.Now
            byte[] now = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(DateTime.Now));

            _cache.Set(General.SystemSettingsTimestampCacheKey, now, new DistributedCacheEntryOptions
            {
                // 60 minute expiration
                SlidingExpiration = TimeSpan.FromSeconds(3600)
            });
        }
    }
}