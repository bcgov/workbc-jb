import { Injectable } from '@angular/core';
import { CheckboxCategory } from '../../models/checkbox-category';

@Injectable({
    providedIn: 'root'
})
export class CheckboxInfo {

    public static industry: CheckboxCategory[] = [
        {
            obj: 'industryFilters',
            prefix: 'Industry',
            key: 'industry',
            listName: 'activeFilters',
            listValueField: 'id',
            filters: [
                { id: '37', key: 'accommodationAndFoodServices', label: 'Accommodation and food services' },
                { id: '40', key: 'administrativeAndSupport', label: 'Administrative and support services' },
                { id: '1', key: 'agricultureAndFishing', label: 'Agriculture, forestry, fishing and hunting' },
                { id: '36', key: 'artsEntertainmentRecreation', label: 'Arts, entertainment and recreation' },
                { id: '23', key: 'construction', label: 'Construction' },
                { id: '34', key: 'educationalServices', label: 'Educational services' },
                { id: '42', key: 'employmentServices', label: 'Employment services' },
                { id: '29', key: 'financeInsurance', label: 'Finance and insurance' },
                { id: '35', key: 'healthCareAndSocialAssistance', label: 'Health care and social assistance' },
                { id: '28', key: 'informationAndCulturalIndustries', label: 'Information and cultural industries' },
                { id: '32', key: 'managementOfCompaniesAndEnterprises', label: 'Management of companies and enterprises' },
                { id: '24', key: 'manufacturing', label: 'Manufacturing' },
                { id: '21', key: 'miningAndOilAndGasExtraction', label: 'Mining and oil and gas extraction' },
                { id: '44', key: 'personalAndLaundry', label: 'Personal and laundry services' },
                { id: '46', key: 'privateHouseholds', label: 'Private households' },
                { id: '31', key: 'professionalScientificAndTechnicalServices', label: 'Professional, scientific and technical services' },
                { id: '39', key: 'publicAdministration', label: 'Public administration' },
                { id: '30', key: 'realEstateAndRental', label: 'Real estate and rental and leasing' },
                { id: '45', key: 'religiousGrantMaking', label: 'Religious, grant-making, civic, and professional and similar organizations' },
                { id: '43', key: 'repairAndMaintenance', label: 'Repair and maintenance' },
                { id: '26', key: 'retailTrade', label: 'Retail trade' },
                { id: '27', key: 'transportationAndWarehousing', label: 'Transportation and warehousing' },
                { id: '22', key: 'utilities', label: 'Utilities' },
                { id: '41', key: 'wasteManagement', label: 'Waste management and remediation services' },
                { id: '25', key: 'wholesaleTrade', label: 'Wholesale trade' }
            ]
        }
    ];

    public static jobType: CheckboxCategory[] = [
        {
            obj: 'jobTypeFilters',
            prefix: 'Job Type',
            key: 'hoursofwork',
            filters: [
                { id: '1', key: 'fullTime', label: 'Full-time' },
                { id: '2', key: 'partTime', label: 'Part-time' },
                { id: '3', key: 'partTimeLeadingToFullTime', label: 'Part-time leading to full-time' }
            ]
        },
        {
            obj: 'jobTypeFilters',
            prefix: 'Job Type',
            key: 'periodofemployment',
            filters: [
                { id: '1', key: 'permanent', label: 'Permanent' },
                { id: '2', key: 'temporary', label: 'Temporary' },
                { id: '3', key: 'casual', label: 'Casual' },
                { id: '4', key: 'seasonal', label: 'Seasonal' }
            ]
        },
        {
            obj: 'jobTypeFilters',
            prefix: 'Job Type',
            key: 'employmentterms',
            filters: [
                { id: '1', key: 'day', label: 'Day' },
                { id: '2', key: 'early', label: 'Early morning' },
                { id: '3', key: 'evening', label: 'Evening' },
                { id: '4', key: 'flexible', label: 'Flexible hours' },
                { id: '5', key: 'morning', label: 'Morning' },
                { id: '6', key: 'night', label: 'Night' },
                { id: '7', key: 'onCall', label: 'On call' },
                { id: '8', key: 'overtime', label: 'Overtime' },
                { id: '9', key: 'shift', label: 'Shift' },
                { id: '10', key: 'toBeDetermined', label: 'To be determined' },
                { id: '12', key: 'weekend', label: 'Weekend' }
            ]
        },
        {
            obj: 'jobTypeFilters',
            prefix: 'Job Type',
            key: 'workplacetype',
            filters: [
                { id: '0', key: 'onSite', label: 'On-site only' },
                { id: '100000', key: 'hybrid', label: 'On-site or remote work' },
                { id: '100001', key: 'travelling', label: 'Work location varies' },
                { id: '15141', key: 'virtual', label: 'Virtual job' }
            ]
        }
    ];

    public static education: CheckboxCategory[] = [
        {
            obj: 'educationFilters',
            prefix: 'Education',
            key: 'education',
            listName: 'activeFilters',
            listValueField: 'label',
            filters: [
                { id: '1', key: 'university', label: 'University' },
                { id: '3', key: 'collegeOrApprenticeship', label: 'College or apprenticeship' },
                { id: '2', key: 'secondarSchoolOrJobSpecificTraining', label: 'Secondary school or job-specific training' },
                { id: '4', key: 'noEducationRequired', label: 'No education' }
            ]
        }
    ];

    public static salary: CheckboxCategory[] = [
        {
            obj: 'salaryFilters',
            prefix: 'Benefits',
            key: 'benefits',
            listName: 'salaryConditions',
            listValueField: 'label',
            filters: [
                { id: '1', key: 'asPerCollectiveAgreement', label: 'As per collective agreement' },
                { id: '2', key: 'bonus', label: 'Bonus' },
                { id: '3', key: 'commission', label: 'Commission' },
                { id: '4', key: 'dentalBenefits', label: 'Dental benefits' },
                { id: '5', key: 'disabilityBenefits', label: 'Disability benefits' },
                { id: '6', key: 'gratuities', label: 'Gratuities' },
                { id: '7', key: 'groupInsuranceBenefits', label: 'Group insurance benefits' },
                { id: '8', key: 'lifeInsuranceBenefits', label: 'Life insurance benefits' },
                { id: '9', key: 'medicalBenefits', label: 'Medical benefits' },
                { id: '10', key: 'mileagePaid', label: 'Mileage paid' },
                { id: '11', key: 'pensionPlanBenefits', label: 'Pension plan benefits' },
                { id: '12', key: 'pieceWork', label: 'Piece work' },
                { id: '13', key: 'respBenefits', label: 'RESP benefits' },
                { id: '14', key: 'rrspBenefits', label: 'RRSP benefits' },
                { id: '15', key: 'visionCareBenefits', label: 'Vision care benefits' },
                { id: '16', key: 'otherBenefits', label: 'Other benefits' }
            ]
        }
    ];

    public static moreFilters: CheckboxCategory[] = [
        {
            obj: 'moreFilters',
            prefix: 'More Filters',
            key: 'employmentgroups',
            filters: [
                { id: '1', key: 'isApprentice', label: 'Apprentice' },
                { id: '2', key: 'isIndigenous', label: 'Indigenous person' },
                { id: '3', key: 'isMatureWorkers', label: 'Mature worker' },
                { id: '4', key: 'isNewcomers', label: 'Newcomer to B.C.' },
                { id: '5', key: 'isPeopleWithDisabilities', label: 'Person with a disability' },
                { id: '6', key: 'isStudents', label: 'Student' },
                { id: '7', key: 'isVeterans', label: 'Veteran of the Canadian Armed Forces' },
                { id: '8', key: 'isVisibleMinority', label: 'Visible minority' },
                { id: '9', key: 'isYouth', label: 'Youth' }
            ]
        }
    ];
}
