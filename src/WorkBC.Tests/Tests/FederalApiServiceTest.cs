using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using RichardSzalay.MockHttp;
using Serilog.Core;
using WorkBC.Importers.Federal.Models;
using WorkBC.Importers.Federal.Services;
using WorkBC.Shared.Services;
using Xunit;

namespace WorkBC.Tests.Tests;

public class FederalApiServiceTest
{
    private readonly HttpClient _httpClient;
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;
    private readonly Logger _logger;

    public FederalApiServiceTest()
    {
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHttpMessageHandler);

        // configure logger 
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables();

        IConfiguration configuration = builder.Build();
        _logger = new LoggingService(configuration, "WorkBC.Importers.Federal").Logger;
    }

    [Fact(DisplayName = "GetAllJobPostingItems() returns a list with the correct JobPostings")]
    public async Task TestGetAllJobPostingItems()
    {
        const string baseUrl = "https://www.jobbank.gc.ca/xmlfeed";
        const string urlExtension = "/en/bc?includevirtual=true";

        _mockHttpMessageHandler
            .When(baseUrl + urlExtension)
            .Respond("application/xml", GetSampleXmlOfAllJobPostings());

        var service = new FederalApiService(_httpClient, baseUrl, _logger);
        var results = await service.GetAllJobPostingItems();

        Assert.Equal(3, results.Count);
        Assert.IsType<JobPosting>(results[0]);
        Assert.Equal(38886453, results[0].Id);
        Assert.Equal(Convert.ToDateTime("2023-10-21T22:00:00Z"), results[0].FileUpdateDate);
    }

    [Fact(DisplayName = "GetAllJobPostingItems() returns an empty list when the API doesn't respond")]
    public async Task GetAllJobPostingsReturnEmptyList()
    {
        const string baseUrl = "https://www.jobbank.gc.ca/xmlfeed";
        const string urlExtension = "/en/bc?includevirtual=true";

        _mockHttpMessageHandler
            .When(baseUrl + urlExtension)
            .Respond(HttpStatusCode.GatewayTimeout);

        var service = new FederalApiService(_httpClient, baseUrl, _logger);
        Assert.Equal(new List<JobPosting>(), await service.GetAllJobPostingItems());
    }

    [Fact(DisplayName = "If API call isn't successful, we wait 5 seconds and try again")]
    public async Task GetAllJobPostingsTriesApiTwice()
    {
        const string baseUrl = "https://www.jobbank.gc.ca/xmlfeed";
        const string urlExtension = "/en/bc?includevirtual=true";

        // first request times out
        _mockHttpMessageHandler
            .Expect(baseUrl + urlExtension)
            .Respond(HttpStatusCode.GatewayTimeout);

        // second request is successful
        _mockHttpMessageHandler
            .Expect(baseUrl + urlExtension)
            .Respond("application/xml", GetSampleXmlOfAllJobPostings());

        var service = new FederalApiService(_httpClient, baseUrl, _logger);
        var results = await service.GetAllJobPostingItems();
        Assert.Equal(3, results.Count);
        Assert.IsType<JobPosting>(results[0]);
    }

    [Fact(DisplayName = "GetXmlDisplayUntil() returns the display until date from the XML")]
    public void TestGetXmlDisplayUntil()
    {
        var displayUntil = FederalApiService.GetXmlDisplayUntil(GetSampleXmlOfJobDetailXml());
        Assert.Equal(Convert.ToDateTime("2023-10-22T07:38:00Z"), displayUntil);
    }
    
    [Fact(DisplayName = "GetEnglishAndFrenchJobDetails() returns the french and english job details")]
    public async Task TestGetEnglishAndFrenchJobDetails()
    {
        const string baseUrl = "https://www.jobbank.gc.ca";

        _mockHttpMessageHandler
            .Expect(baseUrl + "/en/38611392.xml")
            .Respond("application/xml", GetSampleXmlOfJobDetailXml());
        
        _mockHttpMessageHandler
            .Expect(baseUrl + "/fr/38611392.xml")
            .Respond("application/xml", GetSampleXmlOfJobDetailXml());

        var service = new FederalApiService(_httpClient, baseUrl, _logger);
        var (english, french) = await service.GetEnglishAndFrenchJobDetails(38611392);
        Assert.Contains("<jobs_id>38611392</jobs_id>", english.OuterXml);
        Assert.Contains("<jobs_id>38611392</jobs_id>", french.OuterXml);
    }

    private static string GetSampleXmlOfAllJobPostings()
    {
        return @"<SolrResponse>
            <Header>
                <numFound>3</numFound>
                <QTime>0</QTime>
                <numFound>1</numFound>
            </Header>
            <Documents>
                <Document>
                    <jobs_id>38886453</jobs_id>   
                    <job_type>normal</job_type>
                    <file_update_date>2023-10-21T22:00:00Z</file_update_date>
                </Document>
                <Document>  
                    <jobs_id>38854066</jobs_id>   
                    <job_type>normal</job_type>
                    <file_update_date>2023-10-21T21:51:00Z</file_update_date>
                </Document>
                <Document>  
                    <jobs_id>38854387</jobs_id>   
                    <job_type>normal</job_type>
                    <file_update_date>2023-10-21T21:51:00Z</file_update_date>
                </Document>
            </Documents>
        </SolrResponse>";
    }

    private static string GetSampleXmlOfJobDetailXml()
    {
        return @"<SolrResponse>
                <Header>
                    <status>0</status>
                    <QTime>0</QTime>
                    <numFound>1</numFound>
                </Header>
                <Documents>
                    <Document>
                        <jobs_id>38611392</jobs_id>
                        <skill_level>B</skill_level>
                        <remote_cd>2497045</remote_cd>
                        <title>pastry maker</title>
                        <work_period_cd>F</work_period_cd>
                        <employer_type_id>0</employer_type_id>
                        <edulevel_cd>C</edulevel_cd>
                        <wage_class>D</wage_class>
                        <postal_code>V5M4V8</postal_code>
                        <region_spelling>Vancouver</region_spelling>
                        <employer_id>261258</employer_id>
                        <lang>en</lang>
                        <employer_postal_code>V5M4V8</employer_postal_code>
                        <skill_categories>
                            <skill_category id=""100003"">
                                <name>Education</name>
                                <options>
                                    <option_name id=""los"">Secondary (high) school graduation certificate</option_name>
                                </options>
                            </skill_category>
                            <skill_category id=""29"">
                                <name>Tasks</name>
                                <options>
                                    <option_name id=""30008"">Prepare dough for pies, bread, rolls and sweet goods, batters for
                                        muffins, cookies and cakes and icings and frostings according to recipes or special customer
                                        orders
                                    </option_name>
                                    <option_name id=""3361"">Requisition or order materials, equipment and supplies</option_name>
                                    <option_name id=""14141"">Bake mixed dough and batters</option_name>
                                    <option_name id=""10216"">Frost and decorate cakes and baked goods</option_name>
                                    <option_name id=""11478"">Draw up production schedules</option_name>
                                    <option_name id=""11045"">Ensure that the quality of products meets established standards
                                    </option_name>
                                    <option_name id=""19514"">Organize and maintain inventory</option_name>
                                    <option_name id=""24169"">Clean work area</option_name>
                                </options>
                            </skill_category>
                            <skill_category id=""100001"">
                                <name>Experience</name>
                                <options>
                                    <option_name id=""16716"">2 years to less than 3 years</option_name>
                                </options>
                            </skill_category>
                            <skill_category id=""102001"">
                                <name>Health benefits</name>
                                <options>
                                    <option_name id=""102003"">Dental plan</option_name>
                                    <option_name id=""102001"">Health care plan</option_name>
                                    <option_name id=""102008"">Vision care benefits</option_name>
                                </options>
                            </skill_category>
                        </skill_categories>
                        <job_source_id>16</job_source_id>
                        <work_term_cd>P</work_term_cd>
                        <work_lang_cd>E</work_lang_cd>
                        <app_methods>
                            <app_resumesharing>0</app_resumesharing>
                            <app_online></app_online>
                            <app_email>employment.pyc@gmail.com</app_email>
                            <app_phone>
                                <app_phone_number></app_phone_number>
                                <app_phone_ext></app_phone_ext>
                                <app_phone_start_bus_hours></app_phone_start_bus_hours>
                                <app_phone_end_bus_hours></app_phone_end_bus_hours>
                            </app_phone>
                            <app_fax></app_fax>
                            <app_person>
                                <app_person_street></app_person_street>
                                <app_person_room></app_person_room>
                                <app_person_city></app_person_city>
                                <app_person_province></app_person_province>
                                <app_person_country></app_person_country>
                                <app_person_pstlcd></app_person_pstlcd>
                                <app_person_start_bus_hours></app_person_start_bus_hours>
                                <app_person_end_bus_hours></app_person_end_bus_hours>
                            </app_person>
                            <app_mail>
                                <app_mail_street></app_mail_street>
                                <app_mail_room></app_mail_room>
                                <app_mail_city></app_mail_city>
                                <app_mail_province></app_mail_province>
                                <app_mail_country></app_mail_country>
                                <app_mail_pstlcd></app_mail_pstlcd>
                            </app_mail>
                        </app_methods>
                        <naics_id>36</naics_id>
                        <hours>40 hours per week</hours>
                        <employer_name_string>Pacific Yacht Charters</employer_name_string>
                        <employer_name_case>Pacific Yacht Charters</employer_name_case>
                        <num_positions>1</num_positions>
                        <start_date/>
                        <noc2016>6332</noc2016>
                        <noc2021>63202</noc2021>
                        <job_senior_flag>false</job_senior_flag>
                        <job_canadian_citizen_flag>false</job_canadian_citizen_flag>
                        <various_location_flag>false</various_location_flag>
                        <job_student_flag>false</job_student_flag>
                        <job_recruiter_flag>false</job_recruiter_flag>
                        <job_youth_flag>true</job_youth_flag>
                        <job_apprentice_flag>false</job_apprentice_flag>
                        <job_global_applicant_flag>true</job_global_applicant_flag>
                        <job_disability_flag>false</job_disability_flag>
                        <job_aboriginal_flag>true</job_aboriginal_flag>
                        <program_title>false</program_title>
                        <job_vismin_flag>true</job_vismin_flag>
                        <job_general_flag>true</job_general_flag>
                        <job_newcomer_flag>true</job_newcomer_flag>
                        <job_veteran_flag>true</job_veteran_flag>
                        <latlng_variance>1</latlng_variance>
                        <los_id>2</los_id>
                        <postal_code_ind>1</postal_code_ind>
                        <experience_min>24</experience_min>
                        <date_posted>2023-06-24T07:38:00Z</date_posted>
                        <import_date>2023-09-28T20:04:43Z</import_date>
                        <file_update_date>2023-09-28T19:25:00Z</file_update_date>
                        <display_until>2023-10-22T07:38:00Z</display_until>
                        <salary_yearly>41600.0</salary_yearly>
                        <salary_hourly>20.0</salary_hourly>
                        <salary_weekly>800.0</salary_weekly>
                        <work_hours>40.0</work_hours>
                        <city_name>
                            <string>Vancouver</string>
                        </city_name>
                        <arr_app_methods>
                            <string>email</string>
                        </arr_app_methods>
                        <benefits>
                            <string>H</string>
                        </benefits>
                        <lmi_geoarea_cd>
                            <string>25565</string>
                        </lmi_geoarea_cd>
                        <url>
                            <string>https://www.jobbank.gc.ca/jobsearch/jobposting/38611392</string>
                        </url>
                        <activity_type>
                            <string>3361</string>
                            <string>10216</string>
                            <string>11045</string>
                            <string>11478</string>
                            <string>14141</string>
                            <string>16716</string>
                            <string>19514</string>
                            <string>24169</string>
                            <string>30008</string>
                            <string>101010</string>
                            <string>102001</string>
                            <string>102003</string>
                            <string>102008</string>
                        </activity_type>
                        <latlng>
                            <string>49.263527,-123.024625</string>
                        </latlng>
                        <latlng_rpt>
                            <string>49.263527,-123.024625</string>
                        </latlng_rpt>
                        <city_id>
                            <string>39070</string>
                        </city_id>
                        <noc_jobtitle_concordance_id>
                            <string>22261</string>
                        </noc_jobtitle_concordance_id>
                        <special_type>
                            <string>101010</string>
                            <string>102001</string>
                            <string>102003</string>
                            <string>102008</string>
                        </special_type>
                        <major_city_id>
                            <string>39070</string>
                        </major_city_id>
                        <employment_terms>
                            <string>Weekend</string>
                            <string>Shift</string>
                        </employment_terms>
                        <work_hour_term>
                            <string>W</string>
                            <string>S</string>
                        </work_hour_term>
                        <province_cd>
                            <string>BC</string>
                        </province_cd>
                        <salary>
                            <string>$20.00 hourly</string>
                        </salary>
                        <employer_name>
                            <string>Pacific Yacht Charters</string>
                        </employer_name>
                        <category_type>
                            <string>101000</string>
                            <string>102001</string>
                            <string>102001</string>
                            <string>102001</string>
                        </category_type>
                    </Document>
                </Documents>
            </SolrResponse>";
    }
}