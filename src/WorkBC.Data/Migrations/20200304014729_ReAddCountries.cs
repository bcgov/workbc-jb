using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ReAddCountries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update aspnetusers set countryid = 37 where countryid = 1");
            migrationBuilder.Sql("update aspnetusers set countryid = null where countryid <> 37");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    CountryTwoLetterCode = table.Column<string>(maxLength: 2, nullable: true),
                    SortOrder = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerVersions_CountryId",
                table: "JobSeekerVersions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CountryId",
                table: "AspNetUsers",
                column: "CountryId");

            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (37,'Canada','CA',1)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (1,'Afghanistan','AF',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (2,'Albania','AL',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (3,'Algeria','DZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (4,'American Samoa','AS',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (5,'Andorra','AD',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (6,'Angola','AO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (7,'Anguilla','AI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (8,'Antarctica','AQ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (9,'Antigua And Barbuda','AG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (10,'Argentina','AR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (11,'Armenia','AM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (12,'Aruba','AW',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (13,'Australia','AU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (14,'Austria','AT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (15,'Azerbaijan','AZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (16,'Bahamas','BS',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (17,'Bahrain','BH',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (18,'Bangladesh','BD',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (19,'Barbados','BB',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (20,'Belarus','BY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (21,'Belgium','BE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (22,'Belize','BZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (23,'Benin','BJ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (24,'Bermuda','BM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (25,'Bhutan','BT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (26,'Bolivia','BO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (27,'Bosnia And Herzegovina','BA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (28,'Botswana','BW',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (29,'Brazil','BR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (30,'British Indian Ocean Territory','IO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (31,'Brunei Darussalam','BN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (32,'Bulgaria','BG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (33,'Burkina Faso','BF',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (34,'Burundi','BI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (35,'Cambodia','KH',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (36,'Cameroon','CM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (38,'Cape Verde','CV',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (39,'Cayman Islands','KY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (40,'Central African Republic','CF',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (41,'Chad','TD',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (42,'Chile','CL',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (43,'China','CN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (44,'Colombia','CO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (45,'Comoros','KM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (46,'Congo','CG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (47,'Congo, The Democratic Republic Of The','CD',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (48,'Cook Islands','CK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (49,'Costa Rica','CR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (50,'Cote d''Ivoire','CI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (51,'Croatia','HR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (52,'Cuba','CU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (53,'Cyprus','CY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (54,'Czech Republic','CZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (55,'Denmark','DK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (56,'Djibouti','DJ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (57,'Dominica','DM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (58,'Dominican Republic','DO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (59,'Ecuador','EC',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (60,'Egypt','EG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (61,'El Salvador','SV',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (62,'Equatorial Guinea','GQ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (63,'Eritrea','ER',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (64,'Estonia','EE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (65,'Ethiopia','ET',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (66,'Falkland Islands (Malvinas)','FK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (67,'Faroe Islands','FO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (68,'Fiji','FJ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (69,'Finland','FI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (70,'France','FR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (71,'French Guiana','GF',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (72,'French Polynesia','PF',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (73,'French Southern Territories','TF',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (74,'Gabon','GA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (75,'Gambia','GM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (76,'Georgia','GE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (77,'Germany','DE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (78,'Ghana','GH',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (79,'Gibraltar','GI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (80,'Greece','GR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (81,'Greenland','GL',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (82,'Grenada','GD',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (83,'Guadeloupe','GP',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (84,'Guam','GU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (85,'Guatemala','GT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (86,'Guinea','GN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (87,'Guinea-Bissau','GW',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (88,'Guyana','GY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (89,'Haiti','HT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (90,'Holy See, Vatican City State','VA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (91,'Honduras','HN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (92,'Hong Kong','HK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (93,'Hungary','HU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (94,'Iceland','IS',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (95,'India','IN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (96,'Indonesia','ID',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (97,'Iran','IR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (99,'Iraq','IQ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (100,'Ireland','IE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (101,'Israel','IL',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (102,'Italy','IT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (103,'Jamaica','JM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (104,'Japan','JP',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (105,'Jordan','JO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (106,'Kazakhstan','KZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (107,'Kenya','KE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (108,'Kiribati','KI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (109,'Korea, Democratic People''s Republic Of','KP',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (110,'Korea, Republic Of','KR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (111,'Kuwait','KW',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (112,'Kyrgyzstan','KG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (113,'Lao People''s Democratic Republic','LA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (114,'Latvia','LV',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (115,'Lebanon','LB',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (116,'Lesotho','LS',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (117,'Liberia','LR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (119,'Liechtenstein','LI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (120,'Lithuania','LT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (121,'Luxembourg','LU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (122,'Macao','MO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (124,'Madagascar','MG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (125,'Malawi','MW',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (126,'Malaysia','MY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (127,'Maldives','MV',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (128,'Mali','ML',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (129,'Malta','MT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (130,'Martinique','MQ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (131,'Mauritania','MR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (132,'Mauritius','MU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (133,'Mayotte','YT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (134,'Mexico','MX',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (135,'Micronesia, Federated States Of','FM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (136,'Moldova, Republic Of','MD',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (137,'Monaco','MC',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (138,'Mongolia','MN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (139,'Montserrat','MS',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (140,'Morocco','MA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (141,'Mozambique','MZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (142,'Myanmar','MM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (143,'Namibia','NA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (144,'Nauru','NR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (145,'Nepal','NP',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (146,'Netherlands','NL',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (147,'Netherlands Antilles','AN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (148,'New Caledonia','NC',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (149,'New Zealand','NZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (150,'Nicaragua','NI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (151,'Niger','NE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (152,'Nigeria','NG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (153,'Niue','NU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (154,'Norfolk Island','NF',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (155,'Northern Mariana Islands','MP',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (156,'Norway','NO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (157,'Oman','OM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (158,'Pakistan','PK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (159,'Palau','PW',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (161,'Panama','PA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (163,'Papua New Guinea','PG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (162,'Paraguay','PY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (164,'Peru','PE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (165,'Philippines','PH',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (166,'Pitcairn','PN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (167,'Poland','PL',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (168,'Portugal','PT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (169,'Puerto Rico','PR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (170,'Qatar','QA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (171,'Romania','RO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (172,'Russian Federation','RU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (173,'Rwanda','RW',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (174,'Saint Helena','SH',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (175,'Saint Kitts And Nevis','KN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (176,'Saint Pierre And Miquelon','PM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (177,'Saint Vincent And The Grenadines','VC',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (178,'Samoa','WS',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (179,'San Marino','SM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (180,'Sao Tome And Principe','ST',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (181,'Saudi Arabia','SA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (182,'Serbia','RS',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (183,'Seychelles','SC',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (184,'Sierra Leone','SL',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (185,'Singapore','SG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (186,'Slovakia','SK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (187,'Slovenia','SI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (188,'Somalia','SO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (189,'South Africa','ZA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (190,'Spain','ES',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (191,'Sri Lanka','LK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (192,'Sudan','SD',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (193,'Suriname','SR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (194,'Swaziland','SZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (195,'Sweden','SE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (196,'Switzerland','CH',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (197,'Syrian Arab Republic','SY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (198,'Taiwan, Province Of China','TW',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (199,'Tajikistan','TJ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (200,'Tanzania, United Republic Of','TZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (201,'Thailand','TH',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (202,'Timor-Leste','TL',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (203,'Togo','TG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (204,'Tokelau','TK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (205,'Tonga','TO',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (206,'Trinidad And Tobago','TT',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (207,'Tunisia','TN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (208,'Turkey','TR',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (209,'Turkmenistan','TM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (210,'Tuvalu','TV',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (211,'Uganda','UG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (212,'Ukraine','UA',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (213,'United Arab Emirates','AE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (214,'United Kingdom','GB',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (215,'United States of America','US',2)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (216,'Uruguay','UY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (217,'Uzbekistan','UZ',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (218,'Vanuatu','VU',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (219,'Venezuela','VE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (220,'Viet Nam','VN',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (222,'Virgin Islands, British','VG',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (221,'Virgin Islands, U.S.','VI',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (223,'Wallis And Futuna','WF',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (224,'Western Sahara','EH',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (225,'Yemen','YE',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (226,'Zambia','ZM',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries] ([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (227,'Zimbabwe','ZW',9999)");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Countries_CountryId",
                table: "AspNetUsers",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerVersions_Countries_CountryId",
                table: "JobSeekerVersions",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Countries_CountryId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerVersions_Countries_CountryId",
                table: "JobSeekerVersions");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerVersions_CountryId",
                table: "JobSeekerVersions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CountryId",
                table: "AspNetUsers");
        }
    }
}
