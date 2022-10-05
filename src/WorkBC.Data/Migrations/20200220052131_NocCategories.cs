using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class NocCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NocCategories",
                columns: table => new
                {
                    CategoryCode = table.Column<string>(maxLength: 3, nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NocCategories", x => x.CategoryCode);
                });

            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('0',1,'Management occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('1',1,'Business, finance and administration occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('2',1,'Natural and applied sciences and related occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('3',1,'Health occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('4',1,'Occupations in education, law and social, community and government services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('5',1,'Occupations in art, culture, recreation and sport')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('6',1,'Sales and service occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('7',1,'Trades, transport and equipment operators and related occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('8',1,'Natural resources, agriculture and related production occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('9',1,'Occupations in manufacturing and utilities')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('00',2,'Senior management occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('01',2,'Specialized middle management occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('02',2,'Specialized middle management occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('03',2,'Specialized middle management occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('04',2,'Specialized middle management occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('05',2,'Specialized middle management occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('06',2,'Middle management occupations in retail and wholesale trade and customer services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('07',2,'Middle management occupations in trades, transportation, production and utilities')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('08',2,'Middle management occupations in trades, transportation, production and utilities')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('09',2,'Middle management occupations in trades, transportation, production and utilities')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('11',2,'Professional occupations in business and finance')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('12',2,'Administrative and financial supervisors and administrative occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('13',2,'Finance, insurance and related business administrative occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('14',2,'Office support occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('15',2,'Distribution, tracking and scheduling co-ordination occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('21',2,'Professional occupations in natural and applied sciences')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('22',2,'Technical occupations related to natural and applied sciences')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('30',2,'Professional occupations in nursing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('31',2,'Professional occupations in health (except nursing)')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('32',2,'Technical occupations in health')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('34',2,'Assisting occupations in support of health services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('40',2,'Professional occupations in education services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('41',2,'Professional occupations in law and social, community and government services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('42',2,'Paraprofessional occupations in legal, social, community and education services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('43',2,'Occupations in front-line public protection services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('44',2,'Care providers and educational, legal and public protection support occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('51',2,'Professional occupations in art and culture')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('52',2,'Technical occupations in art, culture, recreation and sport')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('62',2,'Retail sales supervisors and specialized sales occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('63',2,'Service supervisors and specialized service occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('64',2,'Sales representatives and salespersons - wholesale and retail trade')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('65',2,'Service representatives and other customer and personal services occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('66',2,'Sales support occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('67',2,'Service support and other service occupations, n.e.c.')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('72',2,'Industrial, electrical and construction trades')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('73',2,'Maintenance and equipment operation trades')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('74',2,'Other installers, repairers and servicers and material handlers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('75',2,'Transport and heavy equipment operation and related maintenance occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('76',2,'Trades helpers, construction labourers and related occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('82',2,'Supervisors and technical occupations in natural resources, agriculture and related production')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('84',2,'Workers in natural resources, agriculture and related production')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('86',2,'Harvesting, landscaping and natural resources labourers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('92',2,'Processing, manufacturing and utilities supervisors and central control operators')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('94',2,'Processing and manufacturing machine operators and related production workers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('95',2,'Assemblers in manufacturing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('96',2,'Labourers in processing, manufacturing and utilities')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('001',3,'Legislators and senior management')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('011',3,'Administrative services managers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('012',3,'Managers in financial and business services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('013',3,'Managers in communication (except broadcasting)')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('021',3,'Managers in engineering, architecture, science and information systems')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('031',3,'Managers in health care')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('041',3,'Managers in public administration')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('042',3,'Managers in education and social and community services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('043',3,'Managers in public protection services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('051',3,'Managers in art, culture, recreation and sport')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('060',3,'Corporate sales managers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('062',3,'Retail and wholesale trade managers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('063',3,'Managers in food service and accommodation')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('065',3,'Managers in customer and personal services, n.e.c.')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('071',3,'Managers in construction and facility operation and maintenance')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('073',3,'Managers in transportation')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('081',3,'Managers in natural resources production and fishing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('082',3,'Managers in agriculture, horticulture and aquaculture')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('091',3,'Managers in manufacturing and utilities')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('111',3,'Auditors, accountants and investment professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('112',3,'Human resources and business service professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('121',3,'Administrative services supervisors')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('122',3,'Administrative and regulatory occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('124',3,'Office administrative assistants - general, legal and medical')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('125',3,'Court reporters, transcriptionists, records management technicians and statistical officers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('131',3,'Finance, insurance and related business administrative occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('141',3,'General office workers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('142',3,'Office equipment operators')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('143',3,'Financial, insurance and related administrative support workers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('145',3,'Library, correspondence and other clerks')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('151',3,'Mail and message distribution occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('152',3,'Supply chain logistics, tracking and scheduling co-ordination occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('211',3,'Physical science professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('212',3,'Life science professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('213',3,'Civil, mechanical, electrical and chemical engineers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('214',3,'Other engineers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('215',3,'Architects, urban planners and land surveyors')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('216',3,'Mathematicians, statisticians and actuaries')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('217',3,'Computer and information systems professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('221',3,'Technical occupations in physical sciences')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('222',3,'Technical occupations in life sciences')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('223',3,'Technical occupations in civil, mechanical and industrial engineering')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('224',3,'Technical occupations in electronics and electrical engineering')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('225',3,'Technical occupations in architecture, drafting, surveying, geomatics and meteorology')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('226',3,'Other technical inspectors and regulatory officers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('227',3,'Transportation officers and controllers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('228',3,'Technical occupations in computer and information systems')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('301',3,'Professional occupations in nursing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('311',3,'Physicians, dentists and veterinarians')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('312',3,'Optometrists, chiropractors and other health diagnosing and treating professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('313',3,'Pharmacists, dietitians and nutritionists')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('314',3,'Therapy and assessment professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('321',3,'Medical technologists and technicians (except dental health)')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('322',3,'Technical occupations in dental health care')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('323',3,'Other technical occupations in health care')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('341',3,'Assisting occupations in support of health services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('401',3,'University professors and post-secondary assistants')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('402',3,'College and other vocational instructors')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('403',3,'Secondary and elementary school teachers and educational counsellors')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('411',3,'Judges, lawyers and Quebec notaries')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('415',3,'Social and community service professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('416',3,'Policy and program researchers, consultants and officers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('421',3,'Paraprofessional occupations in legal, social, community and education services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('431',3,'Occupations in front-line public protection services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('441',3,'Home care providers and educational support occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('442',3,'Legal and public protection support occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('511',3,'Librarians, archivists, conservators and curators')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('512',3,'Writing, translating and related communications professionals')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('513',3,'Creative and performing artists')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('521',3,'Technical occupations in libraries, public archives, museums and art galleries')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('522',3,'Photographers, graphic arts technicians and technical and co-ordinating occupations in motion pictures, broadcasting and the performing arts')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('523',3,'Announcers and other performers, n.e.c.')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('524',3,'Creative designers and craftspersons')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('525',3,'Athletes, coaches, referees and related occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('621',3,'Retail sales supervisors')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('622',3,'Technical sales specialists in wholesale trade and retail and wholesale buyers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('623',3,'Insurance, real estate and financial sales occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('631',3,'Service supervisors')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('632',3,'Chefs and cooks')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('633',3,'Butchers and bakers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('634',3,'Specialized occupations in personal and customer services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('641',3,'Sales and account representatives - wholesale trade (non-technical)')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('642',3,'Retail salespersons')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('651',3,'Occupations in food and beverage service')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('652',3,'Occupations in travel and accommodation')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('653',3,'Tourism and amusement services occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('654',3,'Security guards and related security service occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('655',3,'Customer and information services representatives')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('656',3,'Other occupations in personal service')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('661',3,'Cashiers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('662',3,'Other sales support and related occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('671',3,'Food counter attendants, kitchen helpers and related support occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('672',3,'Support occupations in accommodation, travel and amusement services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('673',3,'Cleaners')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('674',3,'Other service support and related occupations, n.e.c.')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('720',3,'Contractors and supervisors, industrial, electrical and construction trades and related workers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('723',3,'Machining, metal forming, shaping and erecting trades')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('724',3,'Electrical trades and electrical power line and telecommunications workers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('725',3,'Plumbers, pipefitters and gas fitters')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('727',3,'Carpenters and cabinetmakers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('728',3,'Masonry and plastering trades')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('729',3,'Other construction trades')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('730',3,'Contractors and supervisors, maintenance trades and heavy equipment and transport operators')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('731',3,'Machinery and transportation equipment mechanics (except motor vehicles)')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('732',3,'Automotive service technicians')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('733',3,'Other mechanics and related repairers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('736',3,'Train crew operating occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('737',3,'Crane operators, drillers and blasters')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('738',3,'Printing press operators and other trades and related occupations, n.e.c.')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('744',3,'Other installers, repairers and servicers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('745',3,'Longshore workers and material handlers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('751',3,'Motor vehicle and transit drivers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('752',3,'Heavy equipment operators')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('753',3,'Other transport equipment operators and related maintenance workers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('761',3,'Trades helpers and labourers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('762',3,'Public works and other labourers, n.e.c.')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('821',3,'Supervisors, logging and forestry')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('822',3,'Contractors and supervisors, mining, oil and gas')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('823',3,'Underground miners, oil and gas drillers and related occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('824',3,'Logging machinery operators')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('825',3,'Contractors and supervisors, agriculture, horticulture and related operations and services')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('826',3,'Fishing vessel masters and fishermen/women')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('841',3,'Mine service workers and operators in oil and gas drilling')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('842',3,'Logging and forestry workers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('843',3,'Agriculture and horticulture workers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('844',3,'Other workers in fishing and trapping and hunting occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('861',3,'Harvesting, landscaping and natural resources labourers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('921',3,'Supervisors, processing and manufacturing occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('922',3,'Supervisors, assembly and fabrication')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('923',3,'Central control and process operators in processing and manufacturing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('924',3,'Utilities equipment operators and controllers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('941',3,'Machine operators and related workers in mineral and metal products processing and manufacturing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('942',3,'Machine operators and related workers in chemical, plastic and rubber processing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('943',3,'Machine operators and related workers in pulp and paper production and wood processing and manufacturing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('944',3,'Machine operators and related workers in textile, fabric, fur and leather products processing and manufacturing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('946',3,'Machine operators and related workers in food, beverage and associated products processing')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('947',3,'Printing equipment operators and related occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('952',3,'Mechanical, electrical and electronics assemblers')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('953',3,'Other assembly and related occupations')");
            migrationBuilder.Sql("Insert into NocCategories(CategoryCode,Level,Title) values ('961',3,'Labourers in processing, manufacturing and utilities')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NocCategories");
        }
    }
}
