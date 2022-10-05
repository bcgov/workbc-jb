using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateTermsOfUse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 
'<h4><strong>Introduction</strong></h4>
<p>The WorkBC website is provided by the Province of British Columbia (the ""Province"").</p>
<p>The WorkBC Job Board is British Columbia''s most comprehensive job board, where job seekers can search for and find jobs. You can apply to jobs directly with employers, receive job alerts when new positions are posted, manage your favourite jobs and more. The WorkBC Job Board service is available 24 hours per day, seven days a week, free of charge.</p>
<p>These Terms of Use set out the terms and conditions under which the WorkBC Job Board service is made available to you, the job seeker (""you"" or ""your""), to view job postings.</p>
<div style=""padding: 16px; border: 1px solid black; margin-bottom: 25px"">
    <p><strong>Disclaimer:</strong></p>
    <p style=""margin-bottom:4px""><strong>It is important that you carefully read and understand the Terms of Use. </strong>By clicking ""I Agree"" during the registration process and by accessing, using and/or viewing job postings on the WorkBC Job Board, you are confirming that you have read and understood the Terms of Use. You are also confirming your initial and ongoing acceptance and agreement to be bound by these Terms of Use.</p>
</div>
<h4><strong>How the Work BC Job Board service works and collection, use and disclosure of personal information</strong></h4>
<p>The job board is designed to display both job postings submitted to the Government of Canada''s National Job Bank by employers and other current online job postings that may be available. A job seeker may search any of these job postings and use the contact information provided by employers in those job postings to respond directly to employers for jobs that are of interest.</p>
<p>You may use the job board either as an anonymous job seeker (""anonymous user"") or, if you would like to take advantage of additional services, you may set up a user account and become a registered job seeker (""registered user""). In either case, you will be bound by thee Terms of Use, to the extent applicable.</p>
<p>Any personal information collected from you will be collected, used and disclosed in accordance with the <em>British Columbia Freedom of Information and Protection of Privacy Act </em>(""FOIPPA"") and for the purpose of providing you with access to job search resources. The authority to collect personal information comes from Section 26(c) of the FOIPPA. If you have any questions about the collection of your personal information, please contact: Manager, WorkBC Operations, <span class=""text-nowrap"">1-877-952-6914,</span> P.O. Box 9189 Stn Prov Govt, Victoria, B.C. V8W 9E6.</p>
<h5><strong>Anonymous user</strong></h5>
<p>You may use the job board to search current job postings without providing any personal information. However, as an Anonymous User, you will need to re-enter any job search parameters and preferences each time you access the WorkBC Job Board.</p>
<h5><strong>Registered user</strong></h5>
<p>To become a registered user, you must provide the following to create and access your account:</p>
<ul>
    <li>First and last name</li>
    <li>Location (country, province and city)</li>
    <li>Email address</li>
    <li>Password</li>
    <li>Security question and answer</li>
</ul>
<p>You must also supply a password to allow you to access your account. Upon request, your email address may be used to provide you with notices regarding available job postings that meet your search parameters and preferences. Your preferences will be stored within your user account and can only be accessed by you. Your email address may also be used if you forget your login information. As well, you may be notified by email if there are any significant changes made to the WorkBC Job Board that impact you.</p>
<p>If you are accessing your account or the WorkBC Job Board from outside Canada, you hereby consent to your personal information being accessed from outside of Canada. You also understand and agree that any non-personal information provided by you through your use of the job board may be used, in aggregate form only, for program review, analysis and reporting, and for statistical research purposes.</p>
<h5><strong>Voluntary participation</strong></h5>
<p>Participation is voluntary. If you do not provide your personal information, you will not be able to create an account on the WorkBC Job Board. You may still use the WorkBC Job Board as an anonymous user to search for jobs, but you will not be able to access features associated with having an account (i.e. create job alerts).</p>
<h4><strong>Job seeker obligations</strong></h4>
<p>You agree to not use the WorkBC Job Board service for any purpose other than to search available employment opportunities and, if desired, to respond to those employment opportunities, both solely for your personal purposes. You must not copy, scrape, link to, frame or use any data, materials or other information provided on the WorkBC Job Board for any other purpose whatsoever.</p>
<h4><strong>Amendments</strong></h4>
<p>The Province reserves the right to cease providing the WorkBC Job Board service at any time. The Province also reserves the right to modify these Terms of Use at any time without notice being provided directly to you.</p>
<p>You understand and agree that it is your responsibility to consult the Terms of Use on a regular basis to become aware of any modifications that have been made. If you do not agree to be bound by such modified terms, your sole remedy is to cease using the service.</p>
<h4><strong>Job postings</strong></h4>
<p>The job postings on the WorkBC Job Board come from two sources:</p>
<ol>
    <li>Jobs postings submitted to the Government of Canada''s National Job Bank by employers</li>
    <li>Other current online job postings that may be available</li>
</ol>
<p>The Province does not generally monitor or filter the content of the WorkBC Job Board job postings. However, it is the Province''s policy that any job posting that falls within any of the following categories must not be posted or may be removed:</p>
<ul>
    <li>Volunteer opportunities</li>
    <li>Job postings where there are no available openings at the time of posting</li>
    <li>Job postings containing inappropriate, offensive or illegal content, such as defamatory statements, inflammatory or discriminatory content or hiring practices based on gender, sexual orientation, age, religious affiliations, disability or ethnicity</li>
    <li>Any commercial solicitation and/or advertising other than for specific employment opportunities</li>
    <li>Job postings that require job seekers to participate in unpaid training as a condition of hiring</li>
    <li>Job postings for the recruitment of replacement workers during a labour dispute</li>
    <li>Job postings for employment opportunities based outside of British Columbia</li>
</ul>
<p>If you should encounter a job posting that falls into any of the above categories, please immediately contact the Province at <span class=""text-nowrap"">1-877-952-6914</span> and provide details of the offending posting so that it can be reviewed.</p>
<p>Notwithstanding the foregoing, the Province will not have any liability for any information posted by third parties, even if the Province has been advised of any offending content in such information.</p>
<p>Note: Job postings on the WorkBC Job Board have an initial maximum advertising period of 30 calendar days. However, the Province will not have any liability for the earlier or later termination of a job posting for any reason.</p>
<h4><strong>Condition of use</strong></h4>
<p>By accessing and using WorkBC Job Board you agree that your use of this site and the job board service is entirely at your own risk and that you will be liable for any failure to abide by these Terms of Use.</p>
<p>The Province has no obligation to provide, or continue to provide, the job board service to you and all functionalities of WorkBC Job Board are provided on an as available basis. Without limiting the generality of the foregoing and in addition to the <a href=""https://www2.gov.bc.ca/gov/content/home/disclaimer""><u>Warranty Disclaimer</u></a>, the Province makes no representations or warranties, express or implied, with respect to:</p>
<ul>
    <li>WorkBC Job Board being free of malware, including viruses or other harmful components</li>
    <li>How accurate, complete or current any information is available through the job board</li>
</ul>
<p>The Province is not responsible and assumes no liability with respect to any of the information provided to create an account with WorkBC Job Board or in relation to any job advertisement. The Province is not responsible for lost, intercepted, incomplete, illegible, misdirected or stolen messages or mail, unavailable connections, failed, incomplete, garbled or delayed transmissions, online failures, hardware, software or other technical malfunctions or disturbances.<br /> <br /> Although an individual must provide a valid business payroll account number and social insurance number in order to post a job advertisement on the Government of Canada''s National Job Bank (which are in turn displayed on the WorkBC Job Board), they may not be who they claim to be (especially in the case of job postings found on other sites and merely linked to by WorkBC Job Board). As such, you should exercise caution and only disclose personal information that is essential for replying to any job posting.</p>
<p>If any provision of these Terms of Use is declared by an arbitrator or a court of competent jurisdiction to be invalid, illegal or unenforceable, such provision shall be severed from these Terms of Use and all other provisions shall remain in full force and effect.</p>
<p>The laws in force in the Province of British Columbia will govern these Terms of Use and your use of the WorkBC Job Board. You agree that any action at law or in equity in any way arising from these Terms of Use or your use of the service will be resolved by arbitration under the <em>British Columbia Commercial Arbitration Act</em> and the place of arbitration will be Victoria, British Columbia.</p>
<p>WorkBC Job Board users agree to <em>not</em>:</p>
<ul>
    <li>Transmit, post, distribute, store or destroy material, including without limitation WorkBC Job Board content, in violation of any applicable law or regulation, including but not limited to laws or regulations governing the collection, processing, or transfer of personal information, or in breach of WorkBC''s privacy policy</li>
    <li>Take any action that imposes an unreasonable or disproportionately large load on the WorkBC Job Board''s infrastructure</li>
    <li>Use any device to navigate or search the WorkBC Job Board, other than the tools available on the Job Board, generally available third-party web browsers, or other tools approved by the WorkBC Job Board</li>
    <li>Use any data mining, robots or similar data gathering or extraction methods</li>
    <li>Violate or attempt to violate the security of the WorkBC Job Board, including attempting to probe, scan or test the vulnerability of a system or network or to breach security or authentication measures without proper authorization</li>
    <li>Share with a third-party any login credentials to the WorkBC Job Board</li>
    <li>Access data not intended for you or logging into a server or account which you are not authorized to access</li>
    <li>Post or submit to the WorkBC Job Board any incomplete, false or inaccurate biographical information or information which is not your own</li>
    <li>Solicit passwords or personally identifiable information from other users</li>
    <li>Delete or alter any material posted by any other person or entity</li>
    <li>Harass, incite harassment or advocate harassment of any group, company, or individual</li>
    <li>Attempt to interfere with service to any user, host or network, including, without limitation, via means of submitting a virus to the WorkBC Job Board, overloading, ""flooding,"" ""spamming,"" ""mailbombing"" or ""crashing""</li>
</ul>
<p>You are responsible for maintaining the confidentiality of your account, profile and passwords, as applicable. You may not share your password or other account access information with any other party, temporarily or permanently, and you shall be responsible for all uses of your registration and passwords, whether or not authorized by you. You agree to immediately notify the WorkBC Job Board of any unauthorized use of your account, profile or passwords.</p>
<h4><strong>Limitation of Liability and Indemnity</strong></h4>
<p>In addition to the Province''s general <a href=""https://www2.gov.bc.ca/gov/content/home/disclaimer""><u>Limitation of Liability</u></a>, under no circumstances will the Province be liable to any person or business entity for any direct, indirect, special, incidental, consequential or other damages based on: (i) any use or inability to use the Service, or (ii) any lack of availability or delay in using the Service, or the posting or removal of any job advertisement.</p>
<p>Any dispute at any time arising between you and an individual/employer posting a job shall be solely between you and them.</p>
<p>You agree to indemnify and hold harmless the Province and its successors, assigns, ministers, officers, employees and agents from any claim, action, demand, loss or damages, including lawyers'' fees, made or incurred by any third party in any way arising out of your use of the Service, including as between you and the individual/employer posting the job.</p>
<h4><strong>Termination of use</strong></h4>
<p>The Province may, without notice to you, temporarily or permanently terminate your access to WorkBC Job Board if:</p>
<ul>
    <li>You fail to comply with any of these terms and conditions</li>
    <li>If your user ID or password are compromised or insecure or are suspected of being compromised or insecure</li>
</ul>
<p>If at any time you no longer wish to agree to be bound by these Terms of Use or for any reason wish to terminate your use of the job board service, you must immediately cease accessing and using WorkBC Job Board. In addition, you must notify us so that we may delete your account.</p>
<p>The Province may at any time revoke your access to WorkBC Job Board if you use the Service otherwise than for legitimately searching and applying for employment opportunities for your own personal purposes only.</p>
<h4><strong>Security</strong></h4>
<p>You are responsible for keeping your WorkBC Job Board account login information, including password, secure. You must not share this information with any third party for any purpose.</p>
<h4><strong>Agreement to WorkBC Job Board''s Terms of Use</strong></h4>
<p>By clicking on ""I Agree"", you confirm that you have read, understood and agree to be bound by these Terms of Use for the WorkBC Job Board. By accessing and using the WorkBC Job Board you agree the job board service is used entirely at your own risk and accept that WorkBC Job Board is not liable for events outside of its control.</p>
<p>If you do not agree to be bound by these Terms of Use, you must not access this website or use the job board.</p>' WHERE [Name] = 'jbAccount.registration.termsOfUseText'");


            migrationBuilder.Sql(@"Update SystemSettings SET [Value] = 'WorkBC Job Board''s Terms of Use' Where [Name] = 'jbAccount.registration.termsOfUseTitle'");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
