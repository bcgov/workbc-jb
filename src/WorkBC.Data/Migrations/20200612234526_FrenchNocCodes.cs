using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FrenchNocCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "FrenchTitle",
                "NocCodes",
                maxLength: 180,
                nullable: true);

            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Membres des corps législatifs' WHERE ID = 11");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Cadres supérieurs/cadres supérieures - administration publique' WHERE ID = 12");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Cadres supérieurs/cadres supérieures - services financiers, communications et autres services aux entreprises' WHERE ID = 13");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Cadres supérieurs/cadres supérieures - santé, enseignement, services sociaux et communautaires et associations mutuelles' WHERE ID = 14");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Cadres supérieurs/cadres supérieures - commerce, radiotélédiffusion et autres services, n.c.a.' WHERE ID = 15");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Cadres supérieurs/cadres supérieures - construction, transport, production et services d''utilité publique' WHERE ID = 16");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs financiers/directrices financières' WHERE ID = 111");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des ressources humaines' WHERE ID = 112");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des achats' WHERE ID = 113");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices d''autres services administratifs' WHERE ID = 114");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des assurances, de l''immobilier et du courtage financier' WHERE ID = 121");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de banque, du crédit et d''autres services de placements' WHERE ID = 122");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de la publicité, du marketing et des relations publiques' WHERE ID = 124");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices d''autres services aux entreprises' WHERE ID = 125");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices d''entreprises de télécommunications' WHERE ID = 131");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des services postaux et de messageries' WHERE ID = 132");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des services de génie' WHERE ID = 211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des services d''architecture et de sciences' WHERE ID = 212");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Gestionnaires des systèmes informatiques' WHERE ID = 213");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des soins de santé' WHERE ID = 311");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Gestionnaires de la fonction publique - élaboration de politiques et administration de programmes sociaux et de santé' WHERE ID = 411");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Gestionnaires de la fonction publique - analyse économique, élaboration de politiques et administration de programmes' WHERE ID = 412");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Gestionnaires de la fonction publique - élaboration de politiques en matière d''éducation et administration de programmes' WHERE ID = 413");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres gestionnaires de la fonction publique' WHERE ID = 414");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Administrateurs/administratrices - enseignement postsecondaire et formation professionnelle' WHERE ID = 421");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices d''école et administrateurs/administratrices de programmes d''enseignement aux niveaux primaire et secondaire' WHERE ID = 422");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des services sociaux, communautaires et correctionnels' WHERE ID = 423");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Officiers/officières de direction des services de police' WHERE ID = 431");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Chefs et officiers supérieurs/officières supérieures des services d''incendie' WHERE ID = 432");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Officiers/officières de direction des Forces armées canadiennes' WHERE ID = 433");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de bibliothèques, des archives, de musées et de galeries d''art' WHERE ID = 511");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices - édition, cinéma, radiotélédiffusion et arts de la scène' WHERE ID = 512");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de programmes et de services de sports, de loisirs et de conditionnement physique' WHERE ID = 513");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des ventes corporatives' WHERE ID = 601");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices - commerce de détail et de gros' WHERE ID = 621");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de la restauration et des services alimentaires' WHERE ID = 631");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des services d''hébergement' WHERE ID = 632");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices du service à la clientèle et des services personnels, n.c.a.' WHERE ID = 651");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de la construction' WHERE ID = 711");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Gestionnaires en construction et rénovation domiciliaire' WHERE ID = 712");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de l''exploitation et de l''entretien d''immeubles' WHERE ID = 714");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des transports' WHERE ID = 731");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de l''exploitation des ressources naturelles et de la pêche' WHERE ID = 811");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Gestionnaires en agriculture' WHERE ID = 821");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Gestionnaires en horticulture' WHERE ID = 822");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Gestionnaires en aquaculture' WHERE ID = 823");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de la fabrication' WHERE ID = 911");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices des services d''utilité publique' WHERE ID = 912");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Vérificateurs/vérificatrices et comptables' WHERE ID = 1111");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Analystes financiers/analystes financières et analystes en placements' WHERE ID = 1112");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes en valeurs, agents/agentes en placements et négociateurs/négociatrices en valeurs' WHERE ID = 1113");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres agents financiers/agentes financières' WHERE ID = 1114");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Professionnels/professionnelles en ressources humaines' WHERE ID = 1121");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Professionnels/professionnelles des services-conseils en gestion aux entreprises' WHERE ID = 1122");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Professionnels/professionnelles en publicité, en marketing et en relations publiques' WHERE ID = 1123");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures de commis de bureau et du personnel de soutien administratif' WHERE ID = 1211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures de commis de finance et d''assurance' WHERE ID = 1212");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures de commis de bibliothèque, de correspondanciers et d''autres commis à l''information' WHERE ID = 1213");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures de services postaux et de messageries' WHERE ID = 1214");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures du personnel de coordination de la chaîne d''approvisionnement, du suivi et des horaires' WHERE ID = 1215");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes d''administration' WHERE ID = 1221");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Adjoints/adjointes de direction' WHERE ID = 1222");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes des ressources humaines et de recrutement' WHERE ID = 1223");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de gestion immobilière' WHERE ID = 1224");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Agents/agentes aux achats' WHERE ID = 1225");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Planificateurs/planificatrices de congrès et d''événements' WHERE ID = 1226");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Juges de paix et officiers/officières de justice' WHERE ID = 1227");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes d''assurance-emploi, d''immigration, de services frontaliers et du revenu' WHERE ID = 1228");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Adjoints administratifs/adjointes administratives' WHERE ID = 1241");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Adjoints administratifs juridiques/adjointes administratives juridiques' WHERE ID = 1242");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Adjoints administratifs médicaux/adjointes administratives médicales' WHERE ID = 1243");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Sténographes judiciaires, transcripteurs médicaux/transcriptrices médicales et personnel assimilé' WHERE ID = 1251");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Professionnels/professionnelles de la gestion de l''information sur la santé' WHERE ID = 1252");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes à la gestion des documents' WHERE ID = 1253");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de statistiques et professions connexes du soutien de la recherche' WHERE ID = 1254");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes en comptabilité et teneurs/teneuses de livres' WHERE ID = 1311");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Experts/expertes en sinistres et rédacteurs/rédactrices sinistres' WHERE ID = 1312");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Assureurs/assureures' WHERE ID = 1313");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Estimateurs/estimatrices et évaluateurs/évaluatrices' WHERE ID = 1314");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Courtiers/courtières en douanes, courtiers maritimes/courtières maritimes et autres courtiers/courtières' WHERE ID = 1315");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Employés de soutien de bureau généraux/employées de soutien de bureau générales' WHERE ID = 1411");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Réceptionnistes' WHERE ID = 1414");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Commis des services du personnel' WHERE ID = 1415");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Commis des services judiciaires' WHERE ID = 1416");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Commis à la saisie de données' WHERE ID = 1422");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices d''équipement d''éditique et personnel assimilé' WHERE ID = 1423");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Commis à la comptabilité et personnel assimilé' WHERE ID = 1431");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Administrateurs/administratrices de la paye' WHERE ID = 1432");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Commis de banque, d''assurance et d''autres services financiers' WHERE ID = 1434");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Agents/agentes de recouvrement' WHERE ID = 1435");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Commis et assistants/assistantes dans les bibliothèques' WHERE ID = 1451");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Correspondanciers/correspondancières et commis aux publications et aux règlements' WHERE ID = 1452");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Intervieweurs/intervieweuses pour enquêtes et commis aux statistiques' WHERE ID = 1454");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Commis au courrier et aux services postaux et personnel assimilé' WHERE ID = 1511");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Facteurs/factrices' WHERE ID = 1512");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Messagers/messagères et distributeurs/distributrices porte-à-porte' WHERE ID = 1513");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Expéditeurs/expéditrices et réceptionnaires' WHERE ID = 1521");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Magasiniers/magasinières et commis aux pièces' WHERE ID = 1522");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Coordonnateurs/coordonnatrices de la logistique de la production' WHERE ID = 1523");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Commis aux achats et au contrôle de l''inventaire' WHERE ID = 1524");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Répartiteurs/répartitrices' WHERE ID = 1525");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Horairistes de trajets et d''équipages' WHERE ID = 1526");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Physiciens/physiciennes et astronomes' WHERE ID = 2111");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Chimistes' WHERE ID = 2112");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Géoscientifiques et océanographes' WHERE ID = 2113");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Météorologues et climatologues' WHERE ID = 2114");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres professionnels/professionnelles des sciences physiques' WHERE ID = 2115");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Biologistes et personnel scientifique assimilé' WHERE ID = 2121");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Professionnels/professionnelles des sciences forestières' WHERE ID = 2122");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agronomes, conseillers/conseillères et spécialistes en agriculture' WHERE ID = 2123");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs civils/ingénieures civiles' WHERE ID = 2131");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs mécaniciens/ingénieures mécaniciennes' WHERE ID = 2132");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs électriciens et électroniciens/ingénieures électriciennes et électroniciennes' WHERE ID = 2133");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs chimistes/ingénieures chimistes' WHERE ID = 2134");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs/ingénieures d''industrie et de fabrication' WHERE ID = 2141");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs/ingénieures métallurgistes et des matériaux' WHERE ID = 2142");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs miniers/ingénieures minières' WHERE ID = 2143");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs géologues/ingénieures géologues' WHERE ID = 2144");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs/ingénieures de l''extraction et du raffinage du pétrole' WHERE ID = 2145");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs/ingénieures en aérospatiale' WHERE ID = 2146");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs informaticiens/ingénieures informaticiennes (sauf ingénieurs/ingénieures et concepteurs/conceptrices en logiciel)' WHERE ID = 2147");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres ingénieurs/ingénieures, n.c.a.' WHERE ID = 2148");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Architectes' WHERE ID = 2151");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Architectes paysagistes' WHERE ID = 2152");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Urbanistes et planificateurs/planificatrices de l''utilisation des sols' WHERE ID = 2153");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Arpenteurs-géomètres/arpenteuses-géomètres' WHERE ID = 2154");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mathématiciens/mathématiciennes, statisticiens/statisticiennes et actuaires' WHERE ID = 2161");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Analystes et consultants/consultantes en informatique' WHERE ID = 2171");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Analystes de bases de données et administrateurs/administratrices de données' WHERE ID = 2172");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ingénieurs/ingénieures et concepteurs/conceptrices en logiciel' WHERE ID = 2173");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Programmeurs/programmeuses et développeurs/développeuses en médias interactifs' WHERE ID = 2174");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Concepteurs/conceptrices et développeurs/développeuses Web' WHERE ID = 2175");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en chimie' WHERE ID = 2211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en géologie et en minéralogie' WHERE ID = 2212");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en biologie' WHERE ID = 2221");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Inspecteurs/inspectrices des produits agricoles et de la pêche' WHERE ID = 2222");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en sciences forestières' WHERE ID = 2223");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes du milieu naturel et de la pêche' WHERE ID = 2224");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes et spécialistes de l''aménagement paysager et de l''horticulture' WHERE ID = 2225");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en génie civil' WHERE ID = 2231");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en génie mécanique' WHERE ID = 2232");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en génie industriel et en génie de fabrication' WHERE ID = 2233");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Estimateurs/estimatrices en construction' WHERE ID = 2234");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en génie électronique et électrique' WHERE ID = 2241");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Électroniciens/électroniciennes d''entretien (biens domestiques et commerciaux)' WHERE ID = 2242");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes et mécaniciens/mécaniciennes d''instruments industriels' WHERE ID = 2243");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes, techniciens/techniciennes et contrôleurs/contrôleuses d''avionique et d''instruments et d''appareillages électriques d''aéronefs' WHERE ID = 2244");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en architecture' WHERE ID = 2251");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Designers industriels/designers industrielles' WHERE ID = 2252");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en dessin' WHERE ID = 2253");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes en arpentage' WHERE ID = 2254");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel technique en géomatique et en météorologie' WHERE ID = 2255");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Vérificateurs/vérificatrices et essayeurs/essayeuses des essais non destructifs' WHERE ID = 2261");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Inspecteurs/inspectrices d''ingénierie et officiers/officières de réglementation' WHERE ID = 2262");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Inspecteurs/inspectrices de la santé publique, de l''environnement et de l''hygiène et de la sécurité au travail' WHERE ID = 2263");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Inspecteurs/inspectrices en construction' WHERE ID = 2264");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Pilotes, navigateurs/navigatrices et instructeurs/instructrices de pilotage du transport aérien' WHERE ID = 2271");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Contrôleurs aériens/contrôleuses aériennes et personnel assimilé' WHERE ID = 2272");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Officiers/officières de pont du transport par voies navigables' WHERE ID = 2273");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Officiers mécaniciens/officières mécaniciennes du transport par voies navigables' WHERE ID = 2274");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Contrôleurs/contrôleuses de la circulation ferroviaire et régulateurs/régulatrices de la circulation maritime' WHERE ID = 2275");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes de réseau informatique' WHERE ID = 2281");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de soutien aux utilisateurs' WHERE ID = 2282");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Évaluateurs/évaluatrices de systèmes informatiques' WHERE ID = 2283");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Coordonnateurs/coordonnatrices et superviseurs/superviseures des soins infirmiers' WHERE ID = 3011");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Infirmiers autorisés/infirmières autorisées et infirmiers psychiatriques autorisés/infirmières psychiatriques autorisées' WHERE ID = 3012");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Médecins spécialistes' WHERE ID = 3111");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Omnipraticiens/omnipraticiennes et médecins en médecine familiale' WHERE ID = 3112");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Dentistes' WHERE ID = 3113");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Vétérinaires' WHERE ID = 3114");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Optométristes' WHERE ID = 3121");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Chiropraticiens/chiropraticiennes' WHERE ID = 3122");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Praticiens/praticiennes reliés en soins de santé primaire' WHERE ID = 3124");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres professionnels/professionnelles en diagnostic et en traitement de la santé' WHERE ID = 3125");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Pharmaciens/pharmaciennes' WHERE ID = 3131");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Diététistes et nutritionnistes' WHERE ID = 3132");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Audiologistes et orthophonistes' WHERE ID = 3141");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Physiothérapeutes' WHERE ID = 3142");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Ergothérapeutes' WHERE ID = 3143");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres professionnels/professionnelles en thérapie et en diagnostic' WHERE ID = 3144");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues de laboratoires médicaux' WHERE ID = 3211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes de laboratoire médical et assistants/assistantes en pathologie' WHERE ID = 3212");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues en santé animale et techniciens/techniciennes vétérinaires' WHERE ID = 3213");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Inhalothérapeutes, perfusionnistes cardiovasculaires et technologues cardiopulmonaires' WHERE ID = 3214");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues en radiation médicale' WHERE ID = 3215");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Technologues en échographie' WHERE ID = 3216");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues en cardiologie et technologues en électrophysiologie diagnostique, n.c.a.' WHERE ID = 3217");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres technologues et techniciens/techniciennes des sciences de la santé (sauf soins dentaires)' WHERE ID = 3219");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Denturologistes' WHERE ID = 3221");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Hygiénistes et thérapeutes dentaires' WHERE ID = 3222");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Technologues et techniciens/techniciennes dentaires et auxiliaires dans les laboratoires dentaires' WHERE ID = 3223");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opticiens/opticiennes d''ordonnances' WHERE ID = 3231");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Praticiens/praticiennes des médecines douces' WHERE ID = 3232");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Infirmiers auxiliaires/infirmières auxiliaires' WHERE ID = 3233");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel ambulancier et paramédical' WHERE ID = 3234");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Massothérapeutes' WHERE ID = 3236");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autre personnel technique en thérapie et en diagnostic' WHERE ID = 3237");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Assistants/assistantes dentaires' WHERE ID = 3411");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Aides-infirmiers/aides-infirmières, aides-soignants/aides-soignantes et préposés/préposées aux bénéficiaires' WHERE ID = 3413");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autre personnel de soutien des services de santé' WHERE ID = 3414");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Professeurs/professeures et chargés/chargées de cours au niveau universitaire' WHERE ID = 4011");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Assistants/assistantes d''enseignement et de recherche au niveau postsecondaire' WHERE ID = 4012");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Enseignants/enseignantes au niveau collégial et autres instructeurs/instructrices en formation professionnelle' WHERE ID = 4021");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Enseignants/enseignantes au niveau secondaire' WHERE ID = 4031");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Enseignants/enseignantes aux niveaux primaire et préscolaire' WHERE ID = 4032");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conseillers/conseillères en information scolaire' WHERE ID = 4033");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Juges' WHERE ID = 4111");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Avocats/avocates (partout au Canada) et notaires (au Québec)' WHERE ID = 4112");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Psychologues' WHERE ID = 4151");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Travailleurs sociaux/travailleuses sociales' WHERE ID = 4152");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Thérapeutes conjugaux/thérapeutes conjugales, thérapeutes familiaux/thérapeutes familiales et autres conseillers assimilés/conseillères assimilées' WHERE ID = 4153");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel professionnel relié à la religion' WHERE ID = 4154");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de probation et de libération conditionnelle et personnel assimilé' WHERE ID = 4155");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conseillers/conseillères en emploi' WHERE ID = 4156");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Recherchistes, experts-conseils/expertes-conseils et agents/agentes de programmes, en sciences naturelles et appliquées' WHERE ID = 4161");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Économistes, recherchistes et analystes des politiques économiques' WHERE ID = 4162");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de développement économique, recherchistes et experts-conseils/expertes-conseils en marketing' WHERE ID = 4163");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Recherchistes, experts-conseils/expertes-conseils et agents/agentes de programmes en politiques sociales' WHERE ID = 4164");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Recherchistes, experts-conseils/expertes-conseils et agents/agentes de programmes en politiques de la santé' WHERE ID = 4165");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Recherchistes, experts-conseils/expertes-conseils et agents/agentes de programmes en politiques de l''enseignement' WHERE ID = 4166");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Recherchistes, experts-conseils/expertes-conseils et agents/agentes de programme en sports, en loisirs et en conditionnement physique' WHERE ID = 4167");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de programmes propres au gouvernement' WHERE ID = 4168");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres professionnels/professionnelles des sciences sociales, n.c.a.' WHERE ID = 4169");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes juridiques et personnel assimilé' WHERE ID = 4211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Travailleurs/travailleuses des services sociaux et communautaires' WHERE ID = 4212");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Éducateurs/éducatrices et aides-éducateurs/aides-éducatrices de la petite enfance' WHERE ID = 4214");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Instructeurs/instructrices pour personnes ayant une déficience' WHERE ID = 4215");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres instructeurs/instructrices' WHERE ID = 4216");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autre personnel relié à la religion' WHERE ID = 4217");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Policiers/policières (sauf cadres supérieurs)' WHERE ID = 4311");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Pompiers/pompières' WHERE ID = 4312");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Sous-officiers/sous-officières des Forces armées canadiennes' WHERE ID = 4313");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Gardiens/gardiennes d''enfants en milieu familial' WHERE ID = 4411");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Aides familiaux résidents/aides familiales résidentes, aides de maintien à domicile et personnel assimilé' WHERE ID = 4412");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Aides-enseignants/aides-enseignantes aux niveaux primaire et secondaire' WHERE ID = 4413");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Shérifs et huissiers/huissières de justice' WHERE ID = 4421");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de services correctionnels' WHERE ID = 4422");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes d''application de règlements municipaux et autres agents/agentes de réglementation, n.c.a.' WHERE ID = 4423");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Bibliothécaires' WHERE ID = 5111");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Restaurateurs/restauratrices et conservateurs/conservatrices' WHERE ID = 5112");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Archivistes' WHERE ID = 5113");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Auteurs/auteures, rédacteurs/rédactrices et écrivains/écrivaines' WHERE ID = 5121");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Réviseurs/réviseures, rédacteurs-réviseurs/rédactrices-réviseures et chefs du service des nouvelles' WHERE ID = 5122");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Journalistes' WHERE ID = 5123");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Traducteurs/traductrices, terminologues et interprètes' WHERE ID = 5125");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Producteurs/productrices, réalisateurs/réalisatrices, chorégraphes et personnel assimilé' WHERE ID = 5131");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Chefs d''orchestre, compositeurs/compositrices et arrangeurs/arrangeuses' WHERE ID = 5132");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Musiciens/musiciennes et chanteurs/chanteuses' WHERE ID = 5133");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Danseurs/danseuses' WHERE ID = 5134");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Acteurs/actrices et comédiens/comédiennes' WHERE ID = 5135");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Peintres, sculpteurs/sculpteures et autres artistes des arts visuels' WHERE ID = 5136");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes dans les bibliothèques et les services d''archives publiques' WHERE ID = 5211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel technique des musées et des galeries d''art' WHERE ID = 5212");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Photographes' WHERE ID = 5221");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Cadreurs/cadreuses de films et cadreurs/cadreuses vidéo' WHERE ID = 5222");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes en graphisme' WHERE ID = 5223");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes en radiotélédiffusion' WHERE ID = 5224");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes en enregistrement audio et vidéo' WHERE ID = 5225");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autre personnel technique et personnel de coordination du cinéma, de la radiotélédiffusion et des arts de la scène' WHERE ID = 5226");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel de soutien du cinéma, de la radiotélédiffusion, de la photographie et des arts de la scène' WHERE ID = 5227");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Annonceurs/annonceuses et autres communicateurs/communicatrices' WHERE ID = 5231");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres artistes de spectacle, n.c.a.' WHERE ID = 5232");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Designers graphiques et illustrateurs/illustratrices' WHERE ID = 5241");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Designers d''intérieur et décorateurs/décoratrices d''intérieur' WHERE ID = 5242");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ensembliers/ensemblières de théâtre, dessinateurs/dessinatrices de mode, concepteurs/conceptrices d''expositions et autres concepteurs/conceptrices artistiques' WHERE ID = 5243");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Artisans/artisanes' WHERE ID = 5244");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Patronniers/patronnières de produits textiles et d''articles en cuir et en fourrure' WHERE ID = 5245");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Athlètes' WHERE ID = 5251");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Entraîneurs/entraîneuses' WHERE ID = 5252");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Arbitres et officiels/officielles de sports' WHERE ID = 5253");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Animateurs/animatrices et responsables de programmes de sports, de loisirs et de conditionnement physique' WHERE ID = 5254");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures des ventes - commerce de détail' WHERE ID = 6211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Spécialistes des ventes techniques - commerce de gros' WHERE ID = 6221");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Acheteurs/acheteuses des commerces de détail et de gros' WHERE ID = 6222");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes et courtiers/courtières d''assurance' WHERE ID = 6231");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes et vendeurs/vendeuses en immobilier' WHERE ID = 6232");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Représentants/représentantes des ventes financières' WHERE ID = 6235");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures des services alimentaires' WHERE ID = 6311");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Gouvernants principaux/gouvernantes principales' WHERE ID = 6312");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures des services d''hébergement, de voyages, de tourisme et des services connexes' WHERE ID = 6313");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Superviseurs/superviseures des services d''information et des services à la clientèle' WHERE ID = 6314");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes des services de nettoyage' WHERE ID = 6315");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes des autres services' WHERE ID = 6316");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Chefs' WHERE ID = 6321");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Cuisiniers/cuisinières' WHERE ID = 6322");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Bouchers/bouchères, coupeurs/coupeuses de viande et poissonniers/poissonnières - commerce de gros et de détail' WHERE ID = 6331");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Boulangers-pâtissiers/boulangères-pâtissières' WHERE ID = 6332");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Coiffeurs/coiffeuses et barbiers' WHERE ID = 6341");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Tailleurs/tailleuses, couturiers/couturières, fourreurs/fourreuses et modistes' WHERE ID = 6342");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Cordonniers/cordonnières et fabricants/fabricantes de chaussures' WHERE ID = 6343");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Bijoutiers/bijoutières, réparateurs/réparatrices de bijoux, horlogers-rhabilleurs/horlogères-rhabilleuses et personnel assimilé' WHERE ID = 6344");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Tapissiers-garnisseurs/tapissières-garnisseuses' WHERE ID = 6345");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Directeurs/directrices de funérailles et embaumeurs/embaumeuses' WHERE ID = 6346");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Représentants/représentantes des ventes et des comptes - commerce de gros (non-technique)' WHERE ID = 6411");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Vendeurs/vendeuses - commerce de détail' WHERE ID = 6421");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Maîtres d''hôtel et hôtes/hôtesses' WHERE ID = 6511");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Barmans/barmaids' WHERE ID = 6512");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Serveurs/serveuses d''aliments et de boissons' WHERE ID = 6513");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conseillers/conseillères en voyages' WHERE ID = 6521");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Commissaires et agents/agentes de bord' WHERE ID = 6522");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes à la billetterie et aux services aériens' WHERE ID = 6523");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes à la billetterie, représentants/représentantes du service en matière de fret et personnel assimilé dans le transport routier et maritime' WHERE ID = 6524");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Réceptionnistes d''hôtel' WHERE ID = 6525");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Guides touristiques et guides itinérants/guides itinérantes' WHERE ID = 6531");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Guides d''activités récréatives et sportives de plein air' WHERE ID = 6532");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Travailleurs/travailleuses dans les casinos' WHERE ID = 6533");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de sécurité et personnel assimilé des services de sécurité' WHERE ID = 6541");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Représentants/représentantes au service à la clientèle - institutions financières' WHERE ID = 6551");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres préposés/autres préposées aux services d''information et aux services à la clientèle' WHERE ID = 6552");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conseillers/conseillères imagistes, conseillers mondains/conseillères mondaines et autres conseillers/conseillères en soins personnalisés' WHERE ID = 6561");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Esthéticiens/esthéticiennes, électrolystes et personnel assimilé' WHERE ID = 6562");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Soigneurs/soigneuses d''animaux et travailleurs/travailleuses en soins des animaux' WHERE ID = 6563");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autre personnel de services personnalisés' WHERE ID = 6564");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Caissiers/caissières' WHERE ID = 6611");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Préposés/préposées de stations-service' WHERE ID = 6621");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Garnisseurs/garnisseuses de tablettes, commis et préposés/préposées aux commandes dans les magasins' WHERE ID = 6622");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autre personnel assimilé des ventes' WHERE ID = 6623");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Serveurs/serveuses au comptoir, aides de cuisine et personnel de soutien assimilé' WHERE ID = 6711");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel de soutien en services d''hébergement, de voyage et en services de montage d''installation' WHERE ID = 6721");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices et préposés/préposées aux sports, aux loisirs et dans les parcs d''attractions' WHERE ID = 6722");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Préposés/préposées à l''entretien ménager et au nettoyage - travaux légers' WHERE ID = 6731");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Nettoyeurs spécialisés/nettoyeuses spécialisées' WHERE ID = 6732");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Concierges et surintendants/surintendantes d''immeubles' WHERE ID = 6733");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel de blanchisseries et d''établissements de nettoyage à sec et personnel assimilé' WHERE ID = 6741");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autre personnel de soutien en service, n.c.a.' WHERE ID = 6742");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et contremaîtres/contremaîtresses des machinistes et du personnel des métiers du formage, du profilage et du montage des métaux et personnel assimilé' WHERE ID = 7201");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et contremaîtres/contremaîtresses en électricité et en télécommunications' WHERE ID = 7202");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et contremaîtres/contremaîtresses en tuyauterie' WHERE ID = 7203");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et contremaîtres/contremaîtresses en charpenterie' WHERE ID = 7204");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et contremaîtres/contremaîtresses des autres métiers de la construction et des services de réparation et d''installation' WHERE ID = 7205");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Machinistes et vérificateurs/vérificatrices d''usinage et d''outillage' WHERE ID = 7231");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Outilleurs-ajusteurs/outilleuses-ajusteuses' WHERE ID = 7232");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Tôliers/tôlières' WHERE ID = 7233");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Chaudronniers/chaudronnières' WHERE ID = 7234");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Assembleurs/assembleuses et ajusteurs/ajusteuses de plaques et de charpentes métalliques' WHERE ID = 7235");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses de charpentes métalliques' WHERE ID = 7236");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Soudeurs/soudeuses et opérateurs/opératrices de machines à souder et à braser' WHERE ID = 7237");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Électriciens/électriciennes (sauf électriciens industriels/électriciennes industrielles et de réseaux électriques)' WHERE ID = 7241");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Électriciens industriels/électriciennes industrielles' WHERE ID = 7242");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Électriciens/électriciennes de réseaux électriques' WHERE ID = 7243");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses de lignes électriques et de câbles' WHERE ID = 7244");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses de lignes et de câbles de télécommunications' WHERE ID = 7245");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Installateurs/installatrices et réparateurs/réparatrices de matériel de télécommunications' WHERE ID = 7246");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Techniciens/techniciennes en montage et en entretien d''installations de câblodistribution' WHERE ID = 7247");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Plombiers/plombières' WHERE ID = 7251");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Tuyauteurs/tuyauteuses, monteurs/monteuses d''appareils de chauffage et poseurs/poseuses de gicleurs' WHERE ID = 7252");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses d''installations au gaz' WHERE ID = 7253");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Charpentiers-menuisiers/charpentières-menuisières' WHERE ID = 7271");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Ébénistes' WHERE ID = 7272");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Briqueteurs-maçons/briqueteuses-maçonnes' WHERE ID = 7281");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Finisseurs/finisseuses de béton' WHERE ID = 7282");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Carreleurs/carreleuses' WHERE ID = 7283");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Plâtriers/plâtrières, poseurs/poseuses et finisseurs/finisseuses de systèmes intérieurs et latteurs/latteuses' WHERE ID = 7284");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Couvreurs/couvreuses et poseurs/poseuses de bardeaux' WHERE ID = 7291");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Vitriers/vitrières' WHERE ID = 7292");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Calorifugeurs/calorifugeuses' WHERE ID = 7293");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Peintres et décorateurs/décoratrices (sauf décorateurs/décoratrices d''intérieur)' WHERE ID = 7294");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Poseurs/poseuses de revêtements d''intérieur' WHERE ID = 7295");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et contremaîtres/contremaîtresses en mécanique' WHERE ID = 7301");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et contremaîtres/contremaîtresses des équipes d''opérateurs d''équipement lourd' WHERE ID = 7302");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes de l''imprimerie et du personnel assimilé' WHERE ID = 7303");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes des opérations du transport ferroviaire' WHERE ID = 7304");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes du transport routier et du transport en commun' WHERE ID = 7305");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes de chantier et mécaniciens industriels/mécaniciennes industrielles' WHERE ID = 7311");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes d''équipement lourd' WHERE ID = 7312");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes en chauffage, réfrigération et climatisation' WHERE ID = 7313");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Réparateurs/réparatrices de wagons' WHERE ID = 7314");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes et contrôleurs/contrôleuses d''aéronefs' WHERE ID = 7315");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ajusteurs/ajusteuses de machines' WHERE ID = 7316");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Constructeurs/constructrices et mécaniciens/mécaniciennes d''ascenseurs' WHERE ID = 7318");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes et réparateurs/réparatrices de véhicules automobiles, de camions et d''autobus' WHERE ID = 7321");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Débosseleurs/débosseleuses et réparateurs/réparatrices de carrosserie' WHERE ID = 7322");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Installateurs/installatrices de brûleurs à l''huile et à combustibles solides' WHERE ID = 7331");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Réparateurs/réparatrices et préposés/préposées à l''entretien d''appareils' WHERE ID = 7332");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Électromécaniciens/électromécaniciennes' WHERE ID = 7333");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes de motocyclettes, de véhicules tout-terrain et personnel mécanicien assimilé' WHERE ID = 7334");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres réparateurs/réparatrices de petits moteurs et de petits équipements' WHERE ID = 7335");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes de locomotive et de cour de triage' WHERE ID = 7361");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Chefs de train et serre-freins' WHERE ID = 7362");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Grutiers/grutières' WHERE ID = 7371");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Foreurs/foreuses et dynamiteurs/dynamiteuses de mines à ciel ouvert, de carrières et de chantiers de construction' WHERE ID = 7372");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Foreurs/foreuses de puits d''eau' WHERE ID = 7373");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de presses à imprimer' WHERE ID = 7381");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autre personnel des métiers et personnel assimilé, n.c.a.' WHERE ID = 7384");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel d''installation, d''entretien et de réparation d''équipement résidentiel et commercial' WHERE ID = 7441");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel d''entretien des canalisations d''eau et de gaz' WHERE ID = 7442");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Fumigateurs/fumigatrices et préposés/préposées au contrôle de la vermine' WHERE ID = 7444");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres réparateurs/réparatrices et préposés/préposées à l''entretien' WHERE ID = 7445");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Débardeurs/débardeuses' WHERE ID = 7451");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Manutentionnaires' WHERE ID = 7452");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conducteurs/conductrices de camions de transport' WHERE ID = 7511");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conducteurs/conductrices d''autobus et opérateurs/opératrices de métro et autres transports en commun' WHERE ID = 7512");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Chauffeurs/chauffeuses de taxi, chauffeurs/chauffeuses de limousine et chauffeurs/chauffeuses' WHERE ID = 7513");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Chauffeurs-livreurs/chauffeuses-livreuses - services de livraison et de messagerie' WHERE ID = 7514");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conducteurs/conductrices d''équipement lourd (sauf les grues)' WHERE ID = 7521");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conducteurs/conductrices de machinerie d''entretien public et personnel assimilé' WHERE ID = 7522");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ouvriers/ouvrières de gares de triage et à l''entretien de la voie ferrée' WHERE ID = 7531");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Matelots de pont et matelots de salle des machines du transport par voies navigables' WHERE ID = 7532");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de bateau à moteur, de bac à câble et personnel assimilé' WHERE ID = 7533");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Agents/agentes de piste dans le transport aérien' WHERE ID = 7534");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres préposés/préposées à la pose et à l''entretien des pièces mécaniques d''automobiles' WHERE ID = 7535");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Aides de soutien des métiers et manoeuvres en construction' WHERE ID = 7611");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres manoeuvres et aides de soutien de métiers' WHERE ID = 7612");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres à l''entretien des travaux publics' WHERE ID = 7621");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres dans le transport ferroviaire et routier' WHERE ID = 7622");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes de l''exploitation forestière' WHERE ID = 8211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes de l''exploitation des mines et des carrières' WHERE ID = 8221");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et surveillants/surveillantes du forage et des services reliés à l''extraction de pétrole et de gaz' WHERE ID = 8222");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mineurs/mineuses d''extraction et de préparation, mines souterraines' WHERE ID = 8231");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Foreurs/foreuses et personnel de mise à l''essai et des autres services reliés à l''extraction de pétrole et de gaz' WHERE ID = 8232");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Conducteurs/conductrices de machines d''abattage d''arbres' WHERE ID = 8241");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses de services agricoles, surveillants/surveillantes d''exploitations agricoles et ouvriers spécialisés/ouvrières spécialisées dans l''élevage' WHERE ID = 8252");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Entrepreneurs/entrepreneuses et superviseurs/superviseures des services de l''aménagement paysager, de l''entretien des terrains et de l''horticulture' WHERE ID = 8255");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Capitaines et officiers/officières de bateaux de pêche' WHERE ID = 8261");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Pêcheurs indépendants/pêcheuses indépendantes' WHERE ID = 8262");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel d''entretien et de soutien des mines souterraines' WHERE ID = 8411");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Personnel du forage et de l''entretien des puits de pétrole et de gaz et personnel assimilé' WHERE ID = 8412");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de scies à chaîne et d''engins de débardage' WHERE ID = 8421");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ouvriers/ouvrières en sylviculture et en exploitation forestière' WHERE ID = 8422");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Ouvriers/ouvrières agricoles' WHERE ID = 8431");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ouvriers/ouvrières de pépinières et de serres' WHERE ID = 8432");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Matelots de pont sur les bateaux de pêche' WHERE ID = 8441");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Trappeurs/trappeuses et chasseurs/chasseuses' WHERE ID = 8442");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Manoeuvres à la récolte' WHERE ID = 8611");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres en aménagement paysager et en entretien des terrains' WHERE ID = 8612");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres de l''aquaculture et de la mariculture' WHERE ID = 8613");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Manoeuvres des mines' WHERE ID = 8614");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres de forage et d''entretien des puits de pétrole et de gaz, et personnel assimilé' WHERE ID = 8615");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres de l''exploitation forestière' WHERE ID = 8616");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la transformation des métaux et des minerais' WHERE ID = 9211");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans le raffinage du pétrole, dans le traitement du gaz et des produits chimiques et dans les services d''utilité publique' WHERE ID = 9212");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la transformation des aliments et des boissons' WHERE ID = 9213");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la fabrication de produits en caoutchouc et en plastique' WHERE ID = 9214");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la transformation des produits forestiers' WHERE ID = 9215");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la transformation et la fabrication de produits textiles, de tissus, de fourrure et de cuir' WHERE ID = 9217");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la fabrication de véhicules automobiles' WHERE ID = 9221");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la fabrication de matériel électronique' WHERE ID = 9222");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la fabrication d''appareils électriques' WHERE ID = 9223");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la fabrication de meubles et d''accessoires' WHERE ID = 9224");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la fabrication d''autres produits métalliques et de pièces mécaniques' WHERE ID = 9226");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Surveillants/surveillantes dans la fabrication et le montage de produits divers' WHERE ID = 9227");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de poste central de contrôle et de conduite de procédés industriels dans le traitement des métaux et des minerais' WHERE ID = 9231");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de salle de commande centrale et de conduite de procédés industriels dans le raffinage du pétrole et le traitement du gaz et des produits chimiques' WHERE ID = 9232");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices au contrôle de la réduction en pâte des pâtes et papiers, de la fabrication du papier et du couchage' WHERE ID = 9235");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Mécaniciens/mécaniciennes de centrales et opérateurs/opératrices de réseaux électriques' WHERE ID = 9241");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices d''installations du traitement de l''eau et des déchets' WHERE ID = 9243");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines dans le traitement des métaux et des minerais' WHERE ID = 9411");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ouvriers/ouvrières de fonderies' WHERE ID = 9412");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines à former et à finir le verre et coupeurs/coupeuses de verre' WHERE ID = 9413");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines dans le façonnage et la finition des produits en béton, en argile ou en pierre' WHERE ID = 9414");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Contrôleurs/contrôleuses et essayeurs/essayeuses dans la transformation des métaux et des minerais' WHERE ID = 9415");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines à forger et à travailler les métaux' WHERE ID = 9416");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines d''usinage' WHERE ID = 9417");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines d''autres produits métalliques' WHERE ID = 9418");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices d''installations de traitement des produits chimiques' WHERE ID = 9421");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines de traitement des matières plastiques' WHERE ID = 9422");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines de transformation du caoutchouc et personnel assimilé' WHERE ID = 9423");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines à scier dans les scieries' WHERE ID = 9431");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines dans les usines de pâte à papier' WHERE ID = 9432");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines dans la fabrication et la finition du papier' WHERE ID = 9433");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres opérateurs/opératrices de machines dans la transformation du bois' WHERE ID = 9434");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines à façonner le papier' WHERE ID = 9435");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Classeurs/classeuses de bois d''oeuvre et autres vérificateurs/vérificatrices et classeurs/classeuses dans la transformation du bois' WHERE ID = 9436");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines à travailler le bois' WHERE ID = 9437");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines et travailleurs/travailleuses de traitement des fibres et des fils textiles, du cuir et des peaux' WHERE ID = 9441");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Tisseurs/tisseuses, tricoteurs/tricoteuses et autres opérateurs/opératrices de machines textiles' WHERE ID = 9442");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Coupeurs/coupeuses de tissu, de fourrure et de cuir' WHERE ID = 9445");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines à coudre industrielles' WHERE ID = 9446");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Contrôleurs/contrôleuses et trieurs/trieuses dans la fabrication de produits textiles, de tissus, de fourrure et de cuir' WHERE ID = 9447");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines et de procédés industriels dans la transformation des aliments et des boissons' WHERE ID = 9461");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Bouchers industriels/bouchères industrielles, dépeceurs-découpeurs/dépeceuses-découpeuses de viande, préparateurs/préparatrices de volaille et personnel assimilé' WHERE ID = 9462");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Ouvriers/ouvrières dans les usines de transformation du poisson et de fruits de mer' WHERE ID = 9463");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Échantillonneurs/échantillonneuses et trieurs/trieuses dans la transformation des aliments et des boissons' WHERE ID = 9465");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices d''équipement d''impression sans plaque' WHERE ID = 9471");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Photograveurs-clicheurs/photograveuses-clicheuses, photograveurs-reporteurs/photograveuses-reporteuses et autre personnel de prépresse' WHERE ID = 9472");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines à relier et de finition' WHERE ID = 9473");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Développeurs/développeuses de films et de photographies' WHERE ID = 9474");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses d''aéronefs et contrôleurs/contrôleuses de montage d''aéronefs' WHERE ID = 9521");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Assembleurs/assembleuses, contrôleurs/contrôleuses et vérificateurs/vérificatrices de véhicules automobiles' WHERE ID = 9522");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Assembleurs/assembleuses, monteurs/monteuses, contrôleurs/contrôleuses et vérificateurs/vérificatrices de matériel électronique' WHERE ID = 9523");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses et contrôleurs/contrôleuses dans la fabrication de matériel, d''appareils et d''accessoires électriques' WHERE ID = 9524");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Assembleurs/assembleuses, monteurs/monteuses et contrôleurs/contrôleuses dans la fabrication de transformateurs et de moteurs électriques industriels' WHERE ID = 9525");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses et contrôleurs/contrôleuses de matériel mécanique' WHERE ID = 9526");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Opérateurs/opératrices de machines et contrôleurs/contrôleuses dans la fabrication d''appareils électriques' WHERE ID = 9527");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses de bateaux et contrôleurs/contrôleuses de montage de bateaux' WHERE ID = 9531");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses et contrôleurs/contrôleuses de meubles et d''accessoires' WHERE ID = 9532");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses et contrôleurs/contrôleuses d''autres produits en bois' WHERE ID = 9533");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Finisseurs/finisseuses et restaurateurs/restauratrices de meubles' WHERE ID = 9534");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Assembleurs/assembleuses, finisseurs/finisseuses et contrôleurs/contrôleuses de produits en plastique' WHERE ID = 9535");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Peintres, enduiseurs/enduiseuses et opérateurs/opératrices de procédés dans le finissage du métal - secteur industriel' WHERE ID = 9536");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Monteurs/monteuses, finisseurs/finisseuses et contrôleurs/contrôleuses de produits divers' WHERE ID = 9537");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres dans le traitement des métaux et des minerais' WHERE ID = 9611");
            migrationBuilder.Sql("Update NocCodes SET FrenchTitle = 'Manoeuvres en métallurgie' WHERE ID = 9612");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres dans le traitement des produits chimiques et les services d''utilité publique' WHERE ID = 9613");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres dans le traitement des pâtes et papiers et la transformation du bois' WHERE ID = 9614");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres dans la fabrication des produits en caoutchouc et en plastique' WHERE ID = 9615");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres des produits du textile' WHERE ID = 9616");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres dans la transformation des aliments et des boissons' WHERE ID = 9617");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Manoeuvres dans la transformation du poisson et des fruits de mer' WHERE ID = 9618");
            migrationBuilder.Sql(
                "Update NocCodes SET FrenchTitle = 'Autres manoeuvres des services de transformation, de fabrication et d''utilité publique' WHERE ID = 9619");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "FrenchTitle",
                "NocCodes");
        }
    }
}