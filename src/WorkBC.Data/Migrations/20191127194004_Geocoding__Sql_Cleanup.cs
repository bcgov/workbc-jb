using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class Geocoding__Sql_Cleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // IsDuplicate should only be true when there are 2 or more that are not hidden
            migrationBuilder.Sql("update LocationLookups set IsDuplicate = 0 where IsDuplicate = 1 and Latitude = ''");

            // store latitude and longitude for other ciies (this isn't a complete list.  Anything omitted got results outside BC from Google Maps)
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6439705', Longitude = '-121.2950097' WHERE City = '100 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.7', Longitude = '-121.316667' WHERE City = '105 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.750319', Longitude = '-121.3454781' WHERE City = '108 Mile Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.7747839', Longitude = '-121.391316' WHERE City = '111 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.804034', Longitude = '-121.433165' WHERE City = '114 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.816667', Longitude = '-124.966667' WHERE City = '12 Mile' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.850005', Longitude = '-121.586053' WHERE City = '122 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.884755', Longitude = '-121.673016' WHERE City = '127 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.006773', Longitude = '-121.863141' WHERE City = '141 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.104057', Longitude = '-121.928257' WHERE City = '150 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.9375', Longitude = '-130.033333' WHERE City = '40 Mile Flats' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.303136', Longitude = '-121.3957691' WHERE City = '70 Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.566667', Longitude = '-121.3333329' WHERE City = '93 Mile' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0504377', Longitude = '-122.3044697' WHERE City = 'Abbotsford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.9747343', Longitude = '-122.526817' WHERE City = 'Abbott Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1746684', Longitude = '-119.5707686' WHERE City = 'Adams Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2389409', Longitude = '-121.7658827' WHERE City = 'Agassiz' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.270852', Longitude = '-121.1277779' WHERE City = 'Agate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.2983792', Longitude = '-122.0893296' WHERE City = 'Ahbau' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2761659', Longitude = '-126.0563673' WHERE City = 'Ahousat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.733978', Longitude = '-116.91023' WHERE City = 'Ainsworth' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.733978', Longitude = '-116.91023' WHERE City = 'Ainsworth Hot Springs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.20578', Longitude = '-129.07833' WHERE City = 'Aiyansh' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.2477101', Longitude = '-129.1141783' WHERE City = 'Aiyansh 1' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4733635', Longitude = '-115.9889748' WHERE City = 'Akiskinook' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2877251', Longitude = '-123.1139605' WHERE City = 'Alamo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.2', Longitude = '-119.016667' WHERE City = 'Albas' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2655249', Longitude = '-124.813532' WHERE City = 'Alberni' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1362', Longitude = '-117.8573' WHERE City = 'Albert Canyon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.388162', Longitude = '-123.505321' WHERE City = 'Albert Head' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.183295', Longitude = '-122.556863' WHERE City = 'Albion' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.6572513', Longitude = '-119.1943113' WHERE City = 'Albreda' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0580516', Longitude = '-122.4706669' WHERE City = 'Aldergrove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5844855', Longitude = '-126.9254094' WHERE City = 'Alert Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.6332629', Longitude = '-122.4523446' WHERE City = 'Alexandria' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.082986', Longitude = '-123.277992' WHERE City = 'Alexis Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.116667', Longitude = '-122.033333' WHERE City = 'Aleza Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.482822', Longitude = '-129.4899691' WHERE City = 'Alice Arm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.120654', Longitude = '-116.523693' WHERE City = 'Alice Siding' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.788563', Longitude = '-122.2283371' WHERE City = 'Alkali Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4166669', Longitude = '-120.516667' WHERE City = 'Allenby' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.2', Longitude = '-131.9833329' WHERE City = 'Alliford Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.68914', Longitude = '-120.602292' WHERE City = 'Allison Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.016024', Longitude = '-118.498443' WHERE City = 'Almond Gardens' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.145516', Longitude = '-122.965375' WHERE City = 'Alpine Meadows' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1167848', Longitude = '-122.9815293' WHERE City = 'Alta Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.877725', Longitude = '-120.954506' WHERE City = 'Altona' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.60741', Longitude = '-122.633752' WHERE City = 'Alvin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3282979', Longitude = '-123.156729' WHERE City = 'Ambleside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1679201', Longitude = '-1.7629783' WHERE City = 'Amsbury' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0786159', Longitude = '-118.684499' WHERE City = 'Anaconda' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.466667', Longitude = '-125.316667' WHERE City = 'Anahim Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.635632', Longitude = '-122.4127528' WHERE City = 'Anderson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.635632', Longitude = '-122.4127528' WHERE City = 'Anderson Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.0875897', Longitude = '-127.8317772' WHERE City = 'Andimaul' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.964704', Longitude = '-119.163761' WHERE City = 'Anglemont' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Angusmac' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.314625', Longitude = '-122.8556687' WHERE City = 'Anmore' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.172719', Longitude = '-122.7238031' WHERE City = 'Anniedale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5142739', Longitude = '-123.3054159' WHERE City = 'Anvil Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.415909', Longitude = '-129.8296048' WHERE City = 'Anyox' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.766667', Longitude = '-122.5' WHERE City = 'Anzac' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.642901', Longitude = '-117.5314539' WHERE City = 'Appledale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.790308', Longitude = '-118.078544' WHERE City = 'Applegrove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6925481', Longitude = '-123.5399202' WHERE City = 'Arbutus' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.253611', Longitude = '-123.1604311' WHERE City = 'Arbutus Ridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Arden Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.638881', Longitude = '-123.463841' WHERE City = 'Ardmore' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.160737', Longitude = '-116.917903' WHERE City = 'Argenta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4476983', Longitude = '-119.1969938' WHERE City = 'Armstrong' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.007829', Longitude = '-122.1505341' WHERE City = 'Arnold' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.754089', Longitude = '-120.524384' WHERE City = 'Arras' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.133657', Longitude = '-116.446142' WHERE City = 'Arrow Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.116667', Longitude = '-117.9' WHERE City = 'Arrow Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.711111', Longitude = '-117.920833' WHERE City = 'Arrowhead' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2184021', Longitude = '-124.542808' WHERE City = 'Arrowsmith' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.242122', Longitude = '-124.768975' WHERE City = 'Arrowview Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7259246', Longitude = '-121.2804736' WHERE City = 'Ashcroft' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.555859', Longitude = '-119.014228' WHERE City = 'Ashton Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.930292', Longitude = '-120.629654' WHERE City = 'Aspen Grove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.118362', Longitude = '-122.006722' WHERE City = 'Atchelitz' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.517378', Longitude = '-116.036891' WHERE City = 'Athalmer' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.578028', Longitude = '-133.689524' WHERE City = 'Atlin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.190389', Longitude = '-126.998979' WHERE City = 'Atluck' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.4', Longitude = '-125.8333331' WHERE City = 'Atnarko' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.218805', Longitude = '-121.4332079' WHERE City = 'Attachie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.250627', Longitude = '-122.869158' WHERE City = 'Austin Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.733333', Longitude = '-122.45' WHERE City = 'Australian' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.785209', Longitude = '-119.3231009' WHERE City = 'Avola' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.387537', Longitude = '-122.6125555' WHERE City = 'Azouzetta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5010369', Longitude = '-115.584682' WHERE City = 'Baker' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.928741', Longitude = '-122.996474' WHERE City = 'Baker Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.217228', Longitude = '-120.6892861' WHERE City = 'Baldonnel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.6164315', Longitude = '-122.9372276' WHERE City = 'Baldy Hughes' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.62475', Longitude = '-116.9628789' WHERE City = 'Balfour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.852507', Longitude = '-119.354498' WHERE City = 'Balmoral' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.669778', Longitude = '-124.9023869' WHERE City = 'Balmoral Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5947109', Longitude = '-123.527613' WHERE City = 'Bamberton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8332891', Longitude = '-125.142835' WHERE City = 'Bamfield' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.715573', Longitude = '-120.2333971' WHERE City = 'Bankeir' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.4535898', Longitude = '-130.1498672' WHERE City = 'Banks Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6090737', Longitude = '-124.0253624' WHERE City = 'Bargain Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6057058', Longitude = '-124.0215577' WHERE City = 'Bargain Harbour Sechelt Band 24' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.0703513', Longitude = '-121.5137657' WHERE City = 'Barkerville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.705', Longitude = '-116.8116665' WHERE City = 'Barlow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.023183', Longitude = '-122.446553' WHERE City = 'Barlow Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3', Longitude = '-122.916667' WHERE City = 'Barnet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6341279', Longitude = '-120.12526' WHERE City = 'Barnhartvale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.448508', Longitude = '-126.748464' WHERE City = 'Barrett Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1824784', Longitude = '-120.1237108' WHERE City = 'Barriere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.104066', Longitude = '-122.0951591' WHERE City = 'Barrowtown' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2689631', Longitude = '-123.1321174' WHERE City = 'Basford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2301229', Longitude = '-122.9860767' WHERE City = 'Basque' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.85', Longitude = '-119.083333' WHERE City = 'Bastion Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.715511', Longitude = '-120.368306' WHERE City = 'Batchelor Hills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7751388', Longitude = '-124.9759085' WHERE City = 'Bates Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.233482', Longitude = '-115.21559' WHERE City = 'Baynes Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0319239', Longitude = '-123.055374' WHERE City = 'Beach Grove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3193375', Longitude = '-124.3136411' WHERE City = 'Beachcomber' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2459879', Longitude = '-119.3770421' WHERE City = 'Beachcomber Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5176731', Longitude = '-117.2606983' WHERE City = 'Bealby Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.92414', Longitude = '-136.796737' WHERE City = 'Bear Camp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.272643', Longitude = '-121.226963' WHERE City = 'Bear Flat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.7596274', Longitude = '-120.2376623' WHERE City = 'Beard''s Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.484768', Longitude = '-117.458968' WHERE City = 'Beasley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.735739', Longitude = '-117.731234' WHERE City = 'Beaton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.7663396', Longitude = '-120.8594597' WHERE City = 'Beatton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.730591', Longitude = '-122.5754421' WHERE City = 'Beatton Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.535164', Longitude = '-126.857835' WHERE City = 'Beaver Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.316623', Longitude = '-124.900347' WHERE City = 'Beaver Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.079524', Longitude = '-117.593283' WHERE City = 'Beaver Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.50885', Longitude = '-123.410607' WHERE City = 'Beaver Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.079287', Longitude = '-121.851896' WHERE City = 'Beaver Pass House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.768003', Longitude = '-123.387322' WHERE City = 'Beaver Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.434206', Longitude = '-119.088297' WHERE City = 'Beaverdell' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.80926', Longitude = '-122.881547' WHERE City = 'Beaverley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.7263612', Longitude = '-122.1233689' WHERE City = 'Beavermouth' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.3230265', Longitude = '-123.5927638' WHERE City = 'Becher Bay 2' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.95', Longitude = '-122.5166671' WHERE City = 'Becher House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.745399', Longitude = '-123.2232071' WHERE City = 'Bedwell Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3148023', Longitude = '-122.9255129' WHERE City = 'Belcarra' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2887544', Longitude = '-123.1215934' WHERE City = 'Belford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.744444', Longitude = '-129.794444' WHERE City = 'Bell II' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.1605419', Longitude = '-128.1455793' WHERE City = 'Bella Bella' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.3721277', Longitude = '-126.7539346' WHERE City = 'Bella Coola' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.264127', Longitude = '-123.213345' WHERE City = 'Belleview' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2564477', Longitude = '-123.1015629' WHERE City = 'Bellos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.440677', Longitude = '-123.466617' WHERE City = 'Belmont Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2402182', Longitude = '-122.8782593' WHERE City = 'Belvedere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.76432', Longitude = '-121.058998' WHERE City = 'Bend' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Benledi' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.415951', Longitude = '-134.4640049' WHERE City = 'Ben-My-Chree' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.845821', Longitude = '-134.9928641' WHERE City = 'Bennett' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.366667', Longitude = '-127.233333' WHERE City = 'Benson Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8635146', Longitude = '-119.4535877' WHERE City = 'Benvoulin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.590607', Longitude = '-120.253378' WHERE City = 'Beresford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.144003', Longitude = '-119.1590527' WHERE City = 'Bergs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.084366', Longitude = '-122.043512' WHERE City = 'Beryl Prairie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.816667', Longitude = '-120.5' WHERE City = 'Bessborough' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.565798', Longitude = '-120.0822091' WHERE City = 'Bestwick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.658564', Longitude = '-125.089017' WHERE City = 'Bevan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.3093492', Longitude = '-121.7937254' WHERE City = 'Big Bar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.177983', Longitude = '-122.077913' WHERE City = 'Big Bar Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4', Longitude = '-125.1333329' WHERE City = 'Big Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.716667', Longitude = '-123.0333331' WHERE City = 'Big Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.997749', Longitude = '-118.224907' WHERE City = 'Big Eddy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.3821433', Longitude = '-121.8381634' WHERE City = 'Big Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.4166669', Longitude = '-121.85' WHERE City = 'Big Lake Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.721617', Longitude = '-118.926575' WHERE City = 'Big White Village' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.01767', Longitude = '-118.2281879' WHERE City = 'Billings' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.701459', Longitude = '-124.1900441' WHERE City = 'Billings Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.592765', Longitude = '-119.894231' WHERE City = 'Birch Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.162202', Longitude = '-117.73158' WHERE City = 'Birchbank' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.035756', Longitude = '-116.875443' WHERE City = 'Birchdale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.271246', Longitude = '-122.749135' WHERE City = 'Birchland Manor' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.479891', Longitude = '-122.622092' WHERE City = 'Birken' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9334909', Longitude = '-120.257842' WHERE City = 'Black Pines' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.649736', Longitude = '-120.2761459' WHERE City = 'Blackloam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6092665', Longitude = '-120.1101387' WHERE City = 'Blackpool' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6334887', Longitude = '-121.283977' WHERE City = 'Blackstock Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.435711', Longitude = '-117.0680761' WHERE City = 'Blaeberry' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2862797', Longitude = '-123.1198975' WHERE City = 'Blake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.507882', Longitude = '-120.693448' WHERE City = 'Blakeburn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.476405', Longitude = '-117.4053991' WHERE City = 'Blewett' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6585977', Longitude = '-126.522439' WHERE City = 'Bligh Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.886273', Longitude = '-119.397706' WHERE City = 'Blind Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.412877', Longitude = '-125.503265' WHERE City = 'Blind Channel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.03571', Longitude = '-124.815285' WHERE City = 'Bliss Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.119655', Longitude = '-125.386921' WHERE City = 'Bloedel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.829293', Longitude = '-126.6781352' WHERE City = 'Blowhole' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.794706', Longitude = '-124.6232971' WHERE City = 'Blubber Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1', Longitude = '-120.016667' WHERE City = 'Blucher Hall' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.102589', Longitude = '-117.48477' WHERE City = 'Blue Hills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.1064649', Longitude = '-119.3048807' WHERE City = 'Blue River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.25', Longitude = '-118.8500001' WHERE City = 'Blue Springs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.24665', Longitude = '-117.6580889' WHERE City = 'Blueberry Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2307491', Longitude = '-122.7447856' WHERE City = 'Blueberry Farm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.328116', Longitude = '-122.996359' WHERE City = 'Blueridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4791669', Longitude = '-126.425' WHERE City = 'Boat Basin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0910212', Longitude = '-123.7983535' WHERE City = 'Boat Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.972222', Longitude = '-130.247222' WHERE City = 'Bob Quinn Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.172061', Longitude = '-125.160964' WHERE City = 'Bold Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.83173', Longitude = '-121.428802' WHERE City = 'Bond' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.898515', Longitude = '-122.6165809' WHERE City = 'Bonnet Hill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.463164', Longitude = '-117.497793' WHERE City = 'Bonnington Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.965278', Longitude = '-121.4833329' WHERE City = 'Boothroyd (Part) 8A' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8634258', Longitude = '-121.4425737' WHERE City = 'Boston Bar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.772916', Longitude = '-121.310398' WHERE City = 'Boston Flats' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.021917', Longitude = '-122.604882' WHERE City = 'Bouchie Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.39796', Longitude = '-129.00414' WHERE City = 'Boulder' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.39796', Longitude = '-129.00414' WHERE City = 'Boulder City' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.47386', Longitude = '-123.749355' WHERE City = 'Boulder Island Sechelt Band 25' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.004716', Longitude = '-123.036412' WHERE City = 'Boundary Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0493', Longitude = '-118.692911' WHERE City = 'Boundary Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3767653', Longitude = '-123.370154' WHERE City = 'Bowen Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4389905', Longitude = '-124.6835177' WHERE City = 'Bowser' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2111213', Longitude = '-117.7192458' WHERE City = 'Box Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7660145', Longitude = '-123.1491278' WHERE City = 'Brackendale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1010099', Longitude = '-122.4265253' WHERE City = 'Bradner' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.801471', Longitude = '-119.5176379' WHERE City = 'Braeloch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.426094', Longitude = '-123.513024' WHERE City = 'Braemar Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.083333', Longitude = '-124.25' WHERE City = 'Braeside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.77049', Longitude = '-122.802284' WHERE City = 'Bralorne' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.767161', Longitude = '-117.4646421' WHERE City = 'Brandon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1147222', Longitude = '-123.2211111' WHERE City = 'Brandywine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.5063588', Longitude = '-128.626818' WHERE City = 'Brauns Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4384299', Longitude = '-124.663664' WHERE City = 'Brem River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1690901', Longitude = '-119.6051673' WHERE City = 'Brennan Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5724366', Longitude = '-123.4517571' WHERE City = 'Brentwood Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.776577', Longitude = '-124.374728' WHERE City = 'Brew Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8356525', Longitude = '-122.8243267' WHERE City = 'Brexton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.781299', Longitude = '-120.043327' WHERE City = 'Briar Ridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.191622', Longitude = '-121.738538' WHERE City = 'Bridal Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.037014', Longitude = '-119.1521521' WHERE City = 'Bridesville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3428609', Longitude = '-123.1149244' WHERE City = 'Bridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.481605', Longitude = '-120.729613' WHERE City = 'Bridge Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.501119', Longitude = '-120.315625' WHERE City = 'Brigade Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.170211', Longitude = '-123.136457' WHERE City = 'Brighouse' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3606849', Longitude = '-122.898062' WHERE City = 'Brighton Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.321423', Longitude = '-117.642875' WHERE City = 'Brilliant' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.826998', Longitude = '-116.269968' WHERE City = 'Brisco' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.623818', Longitude = '-123.205511' WHERE City = 'Britannia Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.703353', Longitude = '-119.245168' WHERE City = 'Broadview' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.710803', Longitude = '-120.409878' WHERE City = 'Brocklehurst' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.416667', Longitude = '-126.133333' WHERE City = 'Broman Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.817787', Longitude = '-120.872143' WHERE City = 'Brookmere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.232467', Longitude = '-117.749476' WHERE City = 'Brouse' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1620756', Longitude = '-125.3734097' WHERE City = 'Browns Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1913466', Longitude = '-122.8490125' WHERE City = 'Brownsville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Brunswick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5611124', Longitude = '-123.4249641' WHERE City = 'Bryn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4907709', Longitude = '-123.9837599' WHERE City = 'Buccaneer Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.792952', Longitude = '-122.656224' WHERE City = 'Buckhorn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.389395', Longitude = '-122.85146' WHERE City = 'Buckinghorse River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.523786', Longitude = '-124.849994' WHERE City = 'Buckley Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.733333', Longitude = '-121.15' WHERE City = 'Buffalo Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.759992', Longitude = '-121.277624' WHERE City = 'Buick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.249939', Longitude = '-127.462502' WHERE City = 'Bulkley Canyon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.709053', Longitude = '-126.23826' WHERE City = 'Bulkley House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9166669', Longitude = '-127.933333' WHERE City = 'Bull Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.468392', Longitude = '-115.448522' WHERE City = 'Bull River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.385393', Longitude = '-122.862421' WHERE City = 'Buntzen Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3075', Longitude = '-122.7111111' WHERE City = 'Burke Road' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.186692', Longitude = '-123.150368' WHERE City = 'Burkeville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2488091', Longitude = '-122.9805104' WHERE City = 'Burnaby' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.2334148', Longitude = '-125.763613' WHERE City = 'Burns Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Burnt Flats' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.989551', Longitude = '-117.877234' WHERE City = 'Burton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.15', Longitude = '-128.7' WHERE City = 'Butedale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8108099', Longitude = '-121.3233237' WHERE City = 'Cache Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.068286', Longitude = '-119.49662' WHERE City = 'Caesars' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.953568', Longitude = '-120.023597' WHERE City = 'Cahilty' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.320597', Longitude = '-115.173513' WHERE City = 'Caithness' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4402358', Longitude = '-123.3656536' WHERE City = 'Cale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.640898', Longitude = '-131.689146' WHERE City = 'Callison Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2402223', Longitude = '-123.1178489' WHERE City = 'Cambie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.788025', Longitude = '-117.647424' WHERE City = 'Camborne' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2198396', Longitude = '-124.8059964' WHERE City = 'Cameron Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2932861', Longitude = '-124.6330302' WHERE City = 'Cameron Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.117471', Longitude = '-119.183633' WHERE City = 'Camp McKinney' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.656753', Longitude = '-120.081568' WHERE City = 'Campbell Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.170605', Longitude = '-128.141156' WHERE City = 'Campbell Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0331226', Longitude = '-125.2733353' WHERE City = 'Campbell River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.032872', Longitude = '-125.2628521' WHERE City = 'Campbellton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1547849', Longitude = '-115.808063' WHERE City = 'Canal Flats' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.141578', Longitude = '-120.998161' WHERE City = 'Canford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.880847', Longitude = '-120.5716928' WHERE City = 'Canim' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.774437', Longitude = '-120.930601' WHERE City = 'Canim Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.750796', Longitude = '-119.2274331' WHERE City = 'Canoe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.722858', Longitude = '-119.273598' WHERE City = 'Canoe River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0836154', Longitude = '-116.4477734' WHERE City = 'Canyon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.916002', Longitude = '-121.448357' WHERE City = 'Canyon Alpine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3551898', Longitude = '-123.0957866' WHERE City = 'Canyon Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.138439', Longitude = '-117.858848' WHERE City = 'Canyon Hot Springs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.248688', Longitude = '-130.6218659' WHERE City = 'Cariboo Meadows' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.825416', Longitude = '-119.320403' WHERE City = 'Carlin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.970833', Longitude = '-122.570833' WHERE City = 'Carlson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.495488', Longitude = '-119.12278' WHERE City = 'Carmi' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.495488', Longitude = '-119.12278' WHERE City = 'Carmi Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.175403', Longitude = '-127.756678' WHERE City = 'Carnaby' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2623104', Longitude = '-123.095829' WHERE City = 'Carp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.027944', Longitude = '-117.897703' WHERE City = 'Carrolls Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.11654', Longitude = '-119.46196' WHERE City = 'Carrs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.11654', Longitude = '-119.46196' WHERE City = 'Carrs Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0012339', Longitude = '-118.492215' WHERE City = 'Carson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.016667', Longitude = '-118.2' WHERE City = 'Cascade' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.056627', Longitude = '-117.655495' WHERE City = 'Casino' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.28215', Longitude = '-129.8168949' WHERE City = 'Cassiar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0538587', Longitude = '-123.8838125' WHERE City = 'Cassidy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Cassin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.533333', Longitude = '-122.4833331' WHERE City = 'Castle Rock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.02305', Longitude = '-116.522547' WHERE City = 'Castledale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3237408', Longitude = '-117.6593341' WHERE City = 'Castlegar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Caswell' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4222786', Longitude = '-123.3594756' WHERE City = 'Cathedral' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.35', Longitude = '-123.25' WHERE City = 'Caulfeild' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1794814', Longitude = '-119.7575417' WHERE City = 'Cawston' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.884741', Longitude = '-124.366099' WHERE City = 'Caycuse' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.304706', Longitude = '-120.575253' WHERE City = 'Cecil Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.115469', Longitude = '-123.86428' WHERE City = 'Cedar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.135812', Longitude = '-119.453293' WHERE City = 'Cedar Grove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Cedar Heights Estates' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.783333', Longitude = '-119.25' WHERE City = 'Cedarside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.015954', Longitude = '-128.32244' WHERE City = 'Cedarvale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.869926', Longitude = '-126.697565' WHERE City = 'Ceepeecee' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.943942', Longitude = '-119.349578' WHERE City = 'Celista' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5703549', Longitude = '-123.3998135' WHERE City = 'Central Saanich' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.271649', Longitude = '-129.403727' WHERE City = 'Centreville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.106836', Longitude = '-132.5229671' WHERE City = 'Chaatl' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6988986', Longitude = '-120.2740008' WHERE City = 'Chain Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.065081', Longitude = '-127.284252' WHERE City = 'Chamiss Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8865445', Longitude = '-123.5667962' WHERE City = 'Channel Ridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6652462', Longitude = '-115.9679293' WHERE City = 'Chapman Camp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.71824', Longitude = '-121.421041' WHERE City = 'Chapmans' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.880157', Longitude = '-122.788736' WHERE City = 'Charella Garden' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2496622', Longitude = '-123.0947997' WHERE City = 'Charles' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.279902', Longitude = '-120.9634091' WHERE City = 'Charlie Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.225', Longitude = '-125.3000001' WHERE City = 'Charlotte Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8188483', Longitude = '-119.6845191' WHERE City = 'Chase' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.13077', Longitude = '-123.921338' WHERE City = 'Chase River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.216667', Longitude = '-121.483333' WHERE City = 'Chasm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.948109', Longitude = '-121.483359' WHERE City = 'Chaumox' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.830228', Longitude = '-123.13461' WHERE City = 'Cheakamus' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.251849', Longitude = '-121.67915' WHERE City = 'Cheam View' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.799064', Longitude = '-123.154936' WHERE City = 'Cheekye' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4741736', Longitude = '-123.7545601' WHERE City = 'Chelohsin Sechelt Band 13' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.9301648', Longitude = '-123.7344815' WHERE City = 'Chemainus' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7218193', Longitude = '-123.5811913' WHERE City = 'Cherry Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2467479', Longitude = '-118.629853' WHERE City = 'Cherryville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5630961', Longitude = '-126.9841895' WHERE City = 'Ches-la-kee 3' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.816667', Longitude = '-125.8' WHERE City = 'Cheslatta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.25', Longitude = '-126' WHERE City = 'Chetarpe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.6976802', Longitude = '-121.6296734' WHERE City = 'Chetwynd' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.405291', Longitude = '-124.0208599' WHERE City = 'Chezacut' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8222876', Longitude = '-123.7095368' WHERE City = 'Chickwat Sechelt Band 9' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.112068', Longitude = '-124.064853' WHERE City = 'Chilanko Forks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.114909', Longitude = '-122.565999' WHERE City = 'Chilcotin Forest' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1579401', Longitude = '-121.9514666' WHERE City = 'Chilliwack' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.7532683', Longitude = '-120.2723551' WHERE City = 'Chilton Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.0647822', Longitude = '-122.1681213' WHERE City = 'Chimney Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'China Bar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.23762', Longitude = '-120.1622999' WHERE City = 'Chinook Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.470958', Longitude = '-121.426066' WHERE City = 'Choate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.000588', Longitude = '-119.726345' WHERE City = 'Chopaka' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.545423', Longitude = '-118.808853' WHERE City = 'Christian Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.041715', Longitude = '-118.2120721' WHERE City = 'Christina Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.355012', Longitude = '-120.159544' WHERE City = 'Chu Chua' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.333333', Longitude = '-125.0666671' WHERE City = 'Church House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.692308', Longitude = '-119.535275' WHERE City = 'Chute Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.234594', Longitude = '-122.446586' WHERE City = 'Cinema' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.107942', Longitude = '-123.920943' WHERE City = 'Cinnabar Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2862797', Longitude = '-123.1198975' WHERE City = 'Cisco' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.255228', Longitude = '-120.9001419' WHERE City = 'Clairmont' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.333333', Longitude = '-121.216667' WHERE City = 'Clapperton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.081921', Longitude = '-122.260951' WHERE City = 'Clayburn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.188434', Longitude = '-120.0318009' WHERE City = 'Clayhurst' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1573644', Longitude = '-125.9322765' WHERE City = 'Clayoquot' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4767769', Longitude = '-127.7528781' WHERE City = 'Cleagh Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0515175', Longitude = '-122.3267648' WHERE City = 'Clearbrook' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6510712', Longitude = '-120.0381726' WHERE City = 'Clearwater' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.5678489', Longitude = '-119.1007275' WHERE City = 'Clemina East' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Clemina West' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.006166', Longitude = '-126.2850581' WHERE City = 'Clemretta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.358972', Longitude = '-123.106382' WHERE City = 'Cleveland Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.617256', Longitude = '-123.628522' WHERE City = 'Cliffside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.0926994', Longitude = '-121.5865308' WHERE City = 'Clinton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Clode' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6533779', Longitude = '-124.804508' WHERE City = 'Clo-oose' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.599568', Longitude = '-127.583335' WHERE City = 'Coal Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6861071', Longitude = '-123.3773219' WHERE City = 'Coal Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.65', Longitude = '-126.933333' WHERE City = 'Coal River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.507882', Longitude = '-120.693448' WHERE City = 'Coalmont' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6883499', Longitude = '-123.603089' WHERE City = 'Cobble Hill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.975126', Longitude = '-117.196865' WHERE City = 'Cody' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.468357', Longitude = '-115.066006' WHERE City = 'Cokato' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.47386', Longitude = '-123.749355' WHERE City = 'Cokoqueneets Sechelt Band 23' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.017331', Longitude = '-122.0818801' WHERE City = 'Coldspring House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2240647', Longitude = '-119.168328' WHERE City = 'Coldstream' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.40075', Longitude = '-122.889186' WHERE City = 'Coldwell Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.4623607', Longitude = '-122.5930532' WHERE City = 'Colebank' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1014429', Longitude = '-122.8738335' WHERE City = 'Colebrook' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.8636566', Longitude = '-122.7669328' WHERE City = 'College Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.106439', Longitude = '-120.807665' WHERE City = 'Collettville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.033333', Longitude = '-126.15' WHERE City = 'Colleymount' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.480109', Longitude = '-123.40978' WHERE City = 'Colquitz' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.068416', Longitude = '-117.589496' WHERE City = 'Columbia Gardens' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2926456', Longitude = '-115.8736042' WHERE City = 'Columere Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.371705', Longitude = '-115.3804958' WHERE City = 'Colvalli' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4287565', Longitude = '-123.4888933' WHERE City = 'Colwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Comer' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.166257', Longitude = '-122.137645' WHERE City = 'Commodore Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6735133', Longitude = '-124.9282659' WHERE City = 'Comox' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.201563', Longitude = '-122.946741' WHERE City = 'Connaught Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7376677', Longitude = '-116.766327' WHERE City = 'Conrad' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3055', Longitude = '-124.428831' WHERE City = 'Coombe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3055', Longitude = '-124.428831' WHERE City = 'Coombs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2', Longitude = '-116.966667' WHERE City = 'Cooper Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.503126', Longitude = '-116.027699' WHERE City = 'Copper City' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Copper Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.330292', Longitude = '-120.536201' WHERE City = 'Copper Mountain' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2837626', Longitude = '-122.7932065' WHERE City = 'Coquitlam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1339428', Longitude = '-119.4534506' WHERE City = 'Coral Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.515442', Longitude = '-114.656063' WHERE City = 'Corbin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5179793', Longitude = '-123.3670582' WHERE City = 'Cordova Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5851966', Longitude = '-126.9202353' WHERE City = 'Cormorant Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.471304', Longitude = '-117.4699249' WHERE City = 'Corra Linn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.067358', Longitude = '-124.940641' WHERE City = 'Cortes Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1167278', Longitude = '-124.9489278' WHERE City = 'Cortes Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.055189', Longitude = '-122.1741779' WHERE City = 'Cottonwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.055189', Longitude = '-122.1741779' WHERE City = 'Cottonwood Flats' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Couldwell Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6841391', Longitude = '-124.9904494' WHERE City = 'Courtenay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.126202', Longitude = '-120.825879' WHERE City = 'Coutlee' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.323074', Longitude = '-122.953543' WHERE City = 'Cove Cliff' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.341644', Longitude = '-123.369833' WHERE City = 'Cowans Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7373094', Longitude = '-123.6215502' WHERE City = 'Cowichan Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Coykendahl' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.152531', Longitude = '-120.875397' WHERE City = 'Coyle' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.533333', Longitude = '-126.383333' WHERE City = 'Cracroft' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.969555', Longitude = '-118.731025' WHERE City = 'Craigellachie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.87325', Longitude = '-124.5259021' WHERE City = 'Cranberry' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.599165', Longitude = '-128.538878' WHERE City = 'Cranberry Junction' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5129678', Longitude = '-115.7694002' WHERE City = 'Cranbrook' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.681288', Longitude = '-116.82423' WHERE City = 'Crawford Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.399733', Longitude = '-122.702899' WHERE City = 'Creekside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.212448', Longitude = '-118.776192' WHERE City = 'Creighton Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6081969', Longitude = '-117.144004' WHERE City = 'Crescent Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0520009', Longitude = '-122.885091' WHERE City = 'Crescent Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.583333', Longitude = '-120.683333' WHERE City = 'Crescent Spur' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4463401', Longitude = '-117.5577899' WHERE City = 'Crescent Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0955401', Longitude = '-116.5135079' WHERE City = 'Creston' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5237648', Longitude = '-123.5250035' WHERE City = 'Crestwood Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.043401', Longitude = '-120.729653' WHERE City = 'Criss Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.863844', Longitude = '-123.6457976' WHERE City = 'Crofton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.297829', Longitude = '-117.7745064' WHERE City = 'Crowsnest' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.066667', Longitude = '-119.716667' WHERE City = 'Croydon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.9383333', Longitude = '-123.4211111' WHERE City = 'Crysdale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0678959', Longitude = '-121.9762054' WHERE City = 'Cultus Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.618806', Longitude = '-125.0312689' WHERE City = 'Cumberland' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.04383', Longitude = '-131.6848259' WHERE City = 'Cumshewa' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.075062', Longitude = '-116.14068' WHERE City = 'Curzon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3462559', Longitude = '-123.249229' WHERE City = 'Cypress Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.65291', Longitude = '-120.0680347' WHERE City = 'Dallas' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.988889', Longitude = '-125.7916669' WHERE City = 'Danskin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.553902', Longitude = '-122.4788831' WHERE City = 'D''Arcy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5672251', Longitude = '-123.279095' WHERE City = 'D''Arcy Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.295727', Longitude = '-120.181728' WHERE City = 'Darfield' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.669064', Longitude = '-123.1662999' WHERE City = 'Darrell Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.366195', Longitude = '-124.5189479' WHERE City = 'Dashwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.283484', Longitude = '-123.117176' WHERE City = 'Davidson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.443348', Longitude = '-123.724804' WHERE City = 'Davis Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.7596274', Longitude = '-120.2376623' WHERE City = 'Dawson Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.5760835', Longitude = '-127.5910853' WHERE City = 'Dawsons Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.032937', Longitude = '-130.889883' WHERE City = 'Days Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Day''s Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1007546', Longitude = '-123.7436353' WHERE City = 'De Courcy Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Deadtree Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.099491', Longitude = '-118.701372' WHERE City = 'Deadwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.43741', Longitude = '-129.9993781' WHERE City = 'Dease Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.293395', Longitude = '-125.833358' WHERE City = 'Decker Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.453162', Longitude = '-124.710948' WHERE City = 'Deep Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.326817', Longitude = '-122.9519941' WHERE City = 'Deep Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.617886', Longitude = '-119.2157041' WHERE City = 'Deep Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.416217', Longitude = '-118.039438' WHERE City = 'Deer Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.750862', Longitude = '-123.76158' WHERE City = 'Deerholme' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.9', Longitude = '-130.4499999' WHERE City = 'Defot' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.60903', Longitude = '-120.8533521' WHERE City = 'Deka Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.09637', Longitude = '-119.562622' WHERE City = 'Del Oro Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.016667', Longitude = '-132.133333' WHERE City = 'Delkatla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0952155', Longitude = '-123.0264758' WHERE City = 'Delta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Demean' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5629711', Longitude = '-124.7981005' WHERE City = 'Denman Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.718223', Longitude = '-123.147909' WHERE City = 'Dentville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1993259', Longitude = '-123.9593708' WHERE City = 'Departure Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.188179', Longitude = '-122.072985' WHERE City = 'Deroche' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.523349', Longitude = '-122.490449' WHERE City = 'Devine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.164808', Longitude = '-122.196549' WHERE City = 'Dewdney' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.03252', Longitude = '-121.711601' WHERE City = 'Dewey' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.2945969', Longitude = '-130.4233874' WHERE City = 'Digby Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.999766', Longitude = '-120.088384' WHERE City = 'Doe River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.275301', Longitude = '-122.7303971' WHERE City = 'Dogpatch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.484665', Longitude = '-121.4219259' WHERE City = 'Dogwood Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.577522', Longitude = '-120.496455' WHERE City = 'Doig River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.701412', Longitude = '-122.2876167' WHERE City = 'Dokie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.649766', Longitude = '-121.734602' WHERE City = 'Dokie Siding' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9329023', Longitude = '-119.2516777' WHERE City = 'Dolan Road Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3079139', Longitude = '-122.953057' WHERE City = 'Dollarton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.285805', Longitude = '-124.14299' WHERE City = 'Dolphin Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7740643', Longitude = '-130.4408425' WHERE City = 'Dolphin Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.750223', Longitude = '-121.0345609' WHERE City = 'Dome Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.492841', Longitude = '-117.176223' WHERE City = 'Donald' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.483333', Longitude = '-125.6500001' WHERE City = 'Donald Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.625127', Longitude = '-124.04137' WHERE City = 'Donnely Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7107816', Longitude = '-123.8946952' WHERE City = 'Doriston' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.838889', Longitude = '-128.3458329' WHERE City = 'Dorreen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.23392', Longitude = '-121.100922' WHERE City = 'Dot' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.766667', Longitude = '-122.166667' WHERE City = 'Douglas' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0056264', Longitude = '-122.7451575' WHERE City = 'Douglas Hill Estates' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.166632', Longitude = '-120.217676' WHERE City = 'Douglas Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7246489', Longitude = '-125.0582013' WHERE City = 'Dove Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9899926', Longitude = '-118.1914687' WHERE City = 'Downie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.881908', Longitude = '-122.451202' WHERE City = 'Dragon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0944856', Longitude = '-125.1914406' WHERE City = 'Drew Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4152266', Longitude = '-116.811336' WHERE City = 'Drewry' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.816178', Longitude = '-127.077801' WHERE City = 'Driftwood Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5821619', Longitude = '-116.0384379' WHERE City = 'Dry Gulch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.5975', Longitude = '-127.8880555' WHERE City = 'DuBose' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.619531', Longitude = '-119.828335' WHERE City = 'Duck Range' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6601961', Longitude = '-120.3857857' WHERE City = 'Dufferin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.165487', Longitude = '-121.924588' WHERE City = 'Dugan Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7786908', Longitude = '-123.7079417' WHERE City = 'Duncan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.069333', Longitude = '-125.284734' WHERE City = 'Duncan Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.4056757', Longitude = '-127.645006' WHERE City = 'Duncanby Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.283333', Longitude = '-122.466667' WHERE City = 'Dunkley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.38584', Longitude = '-124.611987' WHERE City = 'Dunsmuir' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.1198152', Longitude = '-119.827689' WHERE City = 'Dunster' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.216828', Longitude = '-122.236298' WHERE City = 'Durieu' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6505502', Longitude = '-120.0578496' WHERE City = 'Dutch Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.932532', Longitude = '-119.2136759' WHERE City = 'Eagle Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.263889', Longitude = '-119.502778' WHERE City = 'Eagle Bluff' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.864516', Longitude = '-120.866204' WHERE City = 'Eagle Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.353651', Longitude = '-123.261872' WHERE City = 'Eagle Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.75877', Longitude = '-123.697709' WHERE City = 'Eagle Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.750973', Longitude = '-124.005678' WHERE City = 'Earls Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.089908', Longitude = '-117.918026' WHERE City = 'East Arrow Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8599109', Longitude = '-119.423812' WHERE City = 'East Kelowna' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.028252', Longitude = '-119.440872' WHERE City = 'East Osoyoos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.716183', Longitude = '-121.216169' WHERE City = 'East Pine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.366615', Longitude = '-123.706823' WHERE City = 'East Sooke' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3824934', Longitude = '-125.43728' WHERE City = 'East Thurlow Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0980914', Longitude = '-117.6859382' WHERE City = 'East Trail' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.173997', Longitude = '-124.01695' WHERE City = 'East Wellington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.395289', Longitude = '-123.4349671' WHERE City = 'Eastbourne' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1374923', Longitude = '-120.6156561' WHERE City = 'Eastgate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7508', Longitude = '-126.49755' WHERE City = 'Echo Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.968845', Longitude = '-125.056692' WHERE City = 'Ecoole' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.817687', Longitude = '-129.963909' WHERE City = 'Eddontenajon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.23627', Longitude = '-120.069538' WHERE City = 'Eddy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.31109', Longitude = '-116.970287' WHERE City = 'Edelweiss' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6995093', Longitude = '-116.1319634' WHERE City = 'Edgewater' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.782931', Longitude = '-118.141641' WHERE City = 'Edgewood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.75', Longitude = '-123.933333' WHERE City = 'Egmont' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7612662', Longitude = '-123.9235145' WHERE City = 'Egmont Sechelt Band 26' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9851035', Longitude = '-126.859324' WHERE City = 'Ehatisaht' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.154051', Longitude = '-118.546135' WHERE City = 'Eholt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5324014', Longitude = '-123.3808252' WHERE City = 'Ekins Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.5052234', Longitude = '-120.6796248' WHERE City = 'Ekwan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6651436', Longitude = '-128.0610507' WHERE City = 'Elephant Crossing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.277342', Longitude = '-125.4388059' WHERE City = 'Elk Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.762644', Longitude = '-114.888279' WHERE City = 'Elk Prairie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.024565', Longitude = '-114.92353' WHERE City = 'Elkford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.299036', Longitude = '-115.110965' WHERE City = 'Elko' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.024565', Longitude = '-114.92353' WHERE City = 'Elkview' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.4764524', Longitude = '-121.2969772' WHERE City = 'Elleh' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.939901', Longitude = '-119.3641549' WHERE City = 'Ellison' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.392862', Longitude = '-123.5294399' WHERE City = 'Elphinstone' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.0625691', Longitude = '-124.7259761' WHERE City = 'Encombe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.0856688', Longitude = '-125.0211534' WHERE City = 'Endako' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5508499', Longitude = '-119.1396705' WHERE City = 'Enderby' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.026871', Longitude = '-124.316087' WHERE City = 'Engen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.489846', Longitude = '-134.239954' WHERE City = 'Engineer' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.966667', Longitude = '-121.816667' WHERE City = 'Enterprise' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0912298', Longitude = '-116.4661136' WHERE City = 'Erickson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.191999', Longitude = '-117.334761' WHERE City = 'Erie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2894701', Longitude = '-124.3591725' WHERE City = 'Errington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.09934', Longitude = '-122.18123' WHERE City = 'Esler' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8737712', Longitude = '-126.7414828' WHERE City = 'Esperanza' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Espinosa Inlet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.435842', Longitude = '-123.4112341' WHERE City = 'Esquimalt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.383484', Longitude = '-126.544725' WHERE City = 'Estevan Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.811354', Longitude = '-123.1735153' WHERE City = 'Evans' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.878891', Longitude = '-127.2584' WHERE City = 'Evelyn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.64613', Longitude = '-120.020482' WHERE City = 'Evergreen Acres' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.168042', Longitude = '-119.5071939' WHERE City = 'Ewing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6527249', Longitude = '-121.327046' WHERE City = 'Exeter' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.116667', Longitude = '-120.1333329' WHERE City = 'Exlou' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.5239004', Longitude = '-129.2050261' WHERE City = 'Exstew' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.100173', Longitude = '-123.95709' WHERE City = 'Extension' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0604393', Longitude = '-127.1159337' WHERE City = 'Fair Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7414', Longitude = '-123.698883' WHERE City = 'Fairbridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.335927', Longitude = '-115.8511861' WHERE City = 'Fairmont' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.335927', Longitude = '-115.8511861' WHERE City = 'Fairmont Hot Springs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.174553', Longitude = '-119.601993' WHERE City = 'Fairview' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2634966', Longitude = '-123.1313336' WHERE City = 'Fairview Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.500051', Longitude = '-119.5527621' WHERE City = 'Falkland' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.7838615', Longitude = '-121.1911153' WHERE City = 'Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.066667', Longitude = '-121.55' WHERE City = 'Falls Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.5866718', Longitude = '-121.1629196' WHERE City = 'Fallsway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.490951', Longitude = '-124.3492699' WHERE City = 'False Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.494669', Longitude = '-124.819853' WHERE City = 'Fanny Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.905428', Longitude = '-120.505851' WHERE City = 'Farmington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.176766', Longitude = '-121.5763311' WHERE City = 'Farrell Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Farron' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Faulder' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.871992', Longitude = '-118.074253' WHERE City = 'Fauquier' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.4', Longitude = '-122.3833331' WHERE City = 'Federal Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.599663', Longitude = '-120.566858' WHERE City = 'Fellers Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4464265', Longitude = '-123.3772877' WHERE City = 'Fenwick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.676557', Longitude = '-117.477665' WHERE City = 'Ferguson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.983333', Longitude = '-122.483333' WHERE City = 'Ferndale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5040452', Longitude = '-115.0630649' WHERE City = 'Fernie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.43048', Longitude = '-123.345181' WHERE City = 'Fernwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.394953', Longitude = '-116.4900226' WHERE City = 'Field' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.060142', Longitude = '-118.2116981' WHERE City = 'Fife' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.433333', Longitude = '-127.9500001' WHERE City = 'Fifth Cabin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Fill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.97493', Longitude = '-123.6102981' WHERE City = 'Finmoore' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.137639', Longitude = '-119.495877' WHERE City = 'Fintry' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.666676', Longitude = '-127.1424199' WHERE City = 'Fireside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.433333', Longitude = '-126.283333' WHERE City = 'Firvale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.8305555', Longitude = '-118.4569445' WHERE City = 'Fitzwilliam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6666669', Longitude = '-117.45' WHERE City = 'Five Mile' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.360196', Longitude = '-114.621225' WHERE City = 'Flathead' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.266667', Longitude = '-120.2833329' WHERE City = 'Flatrock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.164085', Longitude = '-122.795712' WHERE City = 'Fleetwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.22576', Longitude = '-123.0766804' WHERE City = 'Fleming' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.36632', Longitude = '-121.510744' WHERE City = 'Floods' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.335442', Longitude = '-126.1380093' WHERE City = 'Flores Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.423116', Longitude = '-121.2155961' WHERE City = 'Flying U' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.283333', Longitude = '-121.733333' WHERE City = 'Fontas' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.2853154', Longitude = '-121.7264737' WHERE City = 'Fontas 1' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.457835', Longitude = '-117.110771' WHERE City = 'Forde' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1892057', Longitude = '-114.8791528' WHERE City = 'Fording' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.9445', Longitude = '-122.6618809' WHERE City = 'Foreman' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.7671149', Longitude = '-121.098162' WHERE City = 'Forest Grove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3420992', Longitude = '-123.096864' WHERE City = 'Forest Hills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.14819', Longitude = '-122.581088' WHERE City = 'Forest Knolls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.416667', Longitude = '-126.2' WHERE City = 'Forestdale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.3222287', Longitude = '-126.6266212' WHERE City = 'Fort Babine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.061971', Longitude = '-124.552796' WHERE City = 'Fort Fraser' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.16837', Longitude = '-122.579975' WHERE City = 'Fort Langley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.8050174', Longitude = '-122.697236' WHERE City = 'Fort Nelson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.7764028', Longitude = '-122.5417583' WHERE City = 'Fort Nelson 2' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.697544', Longitude = '-127.425581' WHERE City = 'Fort Rupert' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.4425721', Longitude = '-124.2510474' WHERE City = 'Fort St. James' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.252423', Longitude = '-120.846409' WHERE City = 'Fort St. John' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.615543', Longitude = '-115.630071' WHERE City = 'Fort Steele' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Foss' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.105462', Longitude = '-122.6507322' WHERE City = 'Foster' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.35', Longitude = '-117.933333' WHERE City = 'Fosthall' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.75216', Longitude = '-121.8933241' WHERE City = 'Fountain' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.734201', Longitude = '-121.862486' WHERE City = 'Fountain Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.249444', Longitude = '-127.933333' WHERE City = 'Fourth Cabin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.842247', Longitude = '-131.7713521' WHERE City = 'Fowler' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.1487143', Longitude = '-122.0943353' WHERE City = 'Fox Mountain' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3444384', Longitude = '-121.8500261' WHERE City = 'Francis Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6178666', Longitude = '-124.0577165' WHERE City = 'Francis Peninsula' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Franklin Camp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.717115', Longitude = '-135.049219' WHERE City = 'Fraser' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.204254', Longitude = '-122.7849241' WHERE City = 'Fraser Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.0562303', Longitude = '-124.849555' WHERE City = 'Fraser Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2202898', Longitude = '-123.065868' WHERE City = 'Fraserview' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4833472', Longitude = '-125.2668415' WHERE City = 'Frederick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.341372', Longitude = '-124.355087' WHERE City = 'French Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1137024', Longitude = '-117.5487834' WHERE City = 'Fruitvale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.058889', Longitude = '-116.875858' WHERE City = 'Fry Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.771358', Longitude = '-123.449449' WHERE City = 'Fulford Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.583333', Longitude = '-123.211111' WHERE City = 'Furry Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1702377', Longitude = '-123.8468027' WHERE City = 'Gabriola' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1577754', Longitude = '-123.7893349' WHERE City = 'Gabriola Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.616667', Longitude = '-117.8666669' WHERE City = 'Galena' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8822639', Longitude = '-123.3510011' WHERE City = 'Galiano' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.9236469', Longitude = '-123.441472' WHERE City = 'Galiano Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.240156', Longitude = '-119.5210208' WHERE City = 'Gallagher Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.374638', Longitude = '-115.225735' WHERE City = 'Galloway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4390855', Longitude = '-123.4382788' WHERE City = 'Gambier Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.55', Longitude = '-122.333333' WHERE City = 'Gang Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8544259', Longitude = '-123.5005072' WHERE City = 'Ganges' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Garbitt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.63406', Longitude = '-124.028113' WHERE City = 'Garden Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.238696', Longitude = '-123.009773' WHERE City = 'Garden Village' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.971041', Longitude = '-123.142923' WHERE City = 'Garibaldi' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7005169', Longitude = '-123.15922' WHERE City = 'Garibaldi Estates' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7426133', Longitude = '-123.1218908' WHERE City = 'Garibaldi Highlands' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7878026', Longitude = '-124.4596371' WHERE City = 'Garnet Rock Trailer Court' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.638485', Longitude = '-119.7102839' WHERE City = 'Garnet Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.508527', Longitude = '-122.533086' WHERE City = 'Gates' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6817459', Longitude = '-121.213043' WHERE City = 'Gateway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.812837', Longitude = '-119.626843' WHERE City = 'Gellatly' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2162667', Longitude = '-117.6902542' WHERE City = 'Genelle' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.75784', Longitude = '-123.600738' WHERE City = 'Genoa Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.557463', Longitude = '-125.519448' WHERE City = 'George River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.471383', Longitude = '-130.3996209' WHERE City = 'Georgetown Mills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.783333', Longitude = '-124.7' WHERE City = 'Germansen Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.2238157', Longitude = '-125.7694376' WHERE City = 'Gerow Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.509091', Longitude = '-117.276036' WHERE City = 'Gerrard' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Gibbs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.5298908', Longitude = '-122.2863939' WHERE City = 'Gibraltar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.361296', Longitude = '-117.670373' WHERE City = 'Gibson Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3974171', Longitude = '-123.5152222' WHERE City = 'Gibsons' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.104724', Longitude = '-122.341009' WHERE City = 'Gifford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.1839458', Longitude = '-129.2766359' WHERE City = 'Gill Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.681509', Longitude = '-124.484791' WHERE City = 'Gillies Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.009901', Longitude = '-118.317514' WHERE City = 'Gilpin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.066667', Longitude = '-122.366667' WHERE City = 'Giscome' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.26577', Longitude = '-128.0655448' WHERE City = 'Gitanyow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.191753', Longitude = '-129.2213341' WHERE City = 'Gitwinksihlkw' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.191753', Longitude = '-129.2213341' WHERE City = 'Gitwinksihlkw 7' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.2657809', Longitude = '-117.516296' WHERE City = 'Glacier' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.8263969', Longitude = '-127.2305873' WHERE City = 'Glacier Gulch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.400172', Longitude = '-117.5343931' WHERE City = 'Glade' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.057429', Longitude = '-122.3156017' WHERE City = 'Gladwin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2046', Longitude = '-123.124717' WHERE City = 'Gleam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.717115', Longitude = '-135.049219' WHERE City = 'Glen Fraser' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.433483', Longitude = '-123.52636' WHERE City = 'Glen Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.161867', Longitude = '-122.473495' WHERE City = 'Glen Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.3119575', Longitude = '-127.6788854' WHERE City = 'Glen Vowell' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.010787', Longitude = '-125.025138' WHERE City = 'Glenannan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2536', Longitude = '-117.790585' WHERE City = 'Glenbank' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.219585', Longitude = '-122.9178391' WHERE City = 'Glenbrooke North' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.1484949', Longitude = '-122.165685' WHERE City = 'Glendale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.367233', Longitude = '-123.28142' WHERE City = 'Gleneagles' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.728962', Longitude = '-119.3428119' WHERE City = 'Gleneden' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.470425', Longitude = '-119.354711' WHERE City = 'Glenemma' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.050135', Longitude = '-116.16402' WHERE City = 'Glenlily' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.099447', Longitude = '-117.663155' WHERE City = 'Glenmerry' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.898479', Longitude = '-119.4552539' WHERE City = 'Glenmore' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.3460457', Longitude = '-116.9479482' WHERE City = 'Glenogle' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8336759', Longitude = '-119.660518' WHERE City = 'Glenrosa' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.866667', Longitude = '-127.133333' WHERE City = 'Glentanna' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2477861', Longitude = '-120.2732427' WHERE City = 'Glimpse Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.53038', Longitude = '-120.5950011' WHERE City = 'Goat River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Goatfell' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.851194', Longitude = '-122.839148' WHERE City = 'Gold Bridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7781637', Longitude = '-126.0496148' WHERE City = 'Gold River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.2961188', Longitude = '-116.9631367' WHERE City = 'Golden' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.281944', Longitude = '-129.302778' WHERE City = 'Good Hope' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.281944', Longitude = '-129.302778' WHERE City = 'Good Hope Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.333333', Longitude = '-120.133333' WHERE City = 'Goodlow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.380688', Longitude = '-127.6570751' WHERE City = 'Goose Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.766667', Longitude = '-124.333333' WHERE City = 'Gordon River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1027088', Longitude = '-125.0246492' WHERE City = 'Gorge Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Gosnell' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.5737959', Longitude = '-128.4232442' WHERE City = 'Gossen Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8901891', Longitude = '-123.3207164' WHERE City = 'Gossip Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3862371', Longitude = '-123.5332046' WHERE City = 'Gower Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.4317595', Longitude = '-132.2621738' WHERE City = 'Graham' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.4317595', Longitude = '-132.2621738' WHERE City = 'Graham Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4491129', Longitude = '-122.68765' WHERE City = 'Gramsons' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0300946', Longitude = '-118.4451392' WHERE City = 'Grand Forks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.239042', Longitude = '-120.9001569' WHERE City = 'Grand Haven' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.783333', Longitude = '-124.8833329' WHERE City = 'Grand Rapids' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.8950961', Longitude = '-122.7628796' WHERE City = 'Grand Trunk' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.233333', Longitude = '-130.1' WHERE City = 'Granduc' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.65', Longitude = '-119.15' WHERE City = 'Grandview Bench' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.8845648', Longitude = '-126.207158' WHERE City = 'Granisle' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.492608', Longitude = '-117.3695059' WHERE City = 'Granite' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.236061', Longitude = '-125.302275' WHERE City = 'Granite Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3747201', Longitude = '-122.880095' WHERE City = 'Granite Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Grant Brook' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.76726', Longitude = '-125.027025' WHERE City = 'Grantham' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.413141', Longitude = '-123.494103' WHERE City = 'Granthams Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.099084', Longitude = '-115.0838111' WHERE City = 'Grasmere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.971874', Longitude = '-125.8840671' WHERE City = 'Grassy Plains' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2881159', Longitude = '-123.1159381' WHERE City = 'Gravelle Ferry' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.630122', Longitude = '-116.784215' WHERE City = 'Gray Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.323498', Longitude = '-124.991758' WHERE City = 'Great Central' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.370134', Longitude = '-125.2049559' WHERE City = 'Great Central Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.702654', Longitude = '-119.742816' WHERE City = 'Greata' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.991071', Longitude = '-124.978727' WHERE City = 'Green Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.255068', Longitude = '-122.8591203' WHERE City = 'Green River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1201135', Longitude = '-122.04558' WHERE City = 'Greendale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0846392', Longitude = '-114.871226' WHERE City = 'Greenhills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Greening' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.032217', Longitude = '-129.58121' WHERE City = 'Greenville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0878955', Longitude = '-118.6780998' WHERE City = 'Greenwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4341237', Longitude = '-123.3914713' WHERE City = 'Griffith' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.628095', Longitude = '-119.123451' WHERE City = 'Grindrod' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.781294', Longitude = '-120.923192' WHERE City = 'Groundbirch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8731716', Longitude = '-122.8782929' WHERE City = 'Gun Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.606954', Longitude = '-120.007125' WHERE City = 'Gundy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.3908829', Longitude = '-126.5422539' WHERE City = 'Hagensborg' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.258108', Longitude = '-127.6031539' WHERE City = 'Hagwilget' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.399414', Longitude = '-121.4531641' WHERE City = 'Haig' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.2128758', Longitude = '-132.0388007' WHERE City = 'Haina' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.980374', Longitude = '-128.648857' WHERE City = 'Haisla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.513965', Longitude = '-123.905929' WHERE City = 'Halfmoon Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.494369', Longitude = '-122.038773' WHERE City = 'Halfway Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3763379', Longitude = '-117.2403369' WHERE City = 'Hall' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4448526', Longitude = '-123.6528625' WHERE City = 'Hanbury' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.919444', Longitude = '-123.041667' WHERE City = 'Hanceville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.084209', Longitude = '-121.863403' WHERE City = 'Hansard' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.416151', Longitude = '-123.533169' WHERE City = 'Happy Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.267341', Longitude = '-122.850204' WHERE City = 'Harbour Chines' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.270865', Longitude = '-122.809809' WHERE City = 'Harbour Village' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4396712', Longitude = '-125.8462694' WHERE City = 'Hardwicke Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.135467', Longitude = '-123.858157' WHERE City = 'Harmac' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.178958', Longitude = '-122.9372689' WHERE City = 'Harmer' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3024727', Longitude = '-121.7853114' WHERE City = 'Harrison Hot Springs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.241664', Longitude = '-121.9458751' WHERE City = 'Harrison Mills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.980883', Longitude = '-116.4487609' WHERE City = 'Harrogate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.603512', Longitude = '-117.061311' WHERE City = 'Harrop' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.975', Longitude = '-122.789766' WHERE City = 'Hart Highlands' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.423919', Longitude = '-129.2535101' WHERE City = 'Hartley Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2538576', Longitude = '-122.738904' WHERE City = 'Harvey' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8645426', Longitude = '-124.6458321' WHERE City = 'Harwood Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.608421', Longitude = '-121.972089' WHERE City = 'Hasler Flat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.281126', Longitude = '-123.044077' WHERE City = 'Hastings-Sunrise' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.14963', Longitude = '-122.255254' WHERE City = 'Hatzic' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.845661', Longitude = '-120.9304694' WHERE City = 'Hawkins Lake Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2393205', Longitude = '-123.1270623' WHERE City = 'Hawks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.182709', Longitude = '-130.013504' WHERE City = 'Haysport' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2019239', Longitude = '-122.3875927' WHERE City = 'Hayward' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.031196', Longitude = '-122.713393' WHERE City = 'Hazelmere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.2568166', Longitude = '-127.6720019' WHERE City = 'Hazelton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.764533', Longitude = '-125.1116921' WHERE City = 'Headquarters' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Headwaters Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7', Longitude = '-126.6' WHERE City = 'Health Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.850769', Longitude = '-126.7461009' WHERE City = 'Hecate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8645338', Longitude = '-119.4909504' WHERE City = 'Hector' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.357177', Longitude = '-120.076544' WHERE City = 'Hedley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8603971', Longitude = '-120.2649133' WHERE City = 'Heffley Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.780665', Longitude = '-121.451615' WHERE City = 'Hells Gate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.376709', Longitude = '-121.934807' WHERE City = 'Hemlock Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.091965', Longitude = '-120.7935249' WHERE City = 'Hendrix Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1006379', Longitude = '-125.216888' WHERE City = 'Heriot Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9817276', Longitude = '-124.9164341' WHERE City = 'Hernando Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.39422', Longitude = '-126.4655751' WHERE City = 'Hesquiat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.583333', Longitude = '-125.583333' WHERE City = 'Heydon Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.512206', Longitude = '-122.218456' WHERE City = 'Hickethier Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3401532', Longitude = '-121.6994357' WHERE City = 'Hicks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5184625', Longitude = '-123.4895031' WHERE City = 'Highlands' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7163115', Longitude = '-123.6439571' WHERE City = 'Hillbank' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8', Longitude = '-123.8' WHERE City = 'Hillcrest' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.311453', Longitude = '-124.48811' WHERE City = 'Hilliers' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.102589', Longitude = '-117.48477' WHERE City = 'Hills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.53183', Longitude = '-132.9586' WHERE City = 'Hippa' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.041176', Longitude = '-130.9465129' WHERE City = 'Hiusta Meadow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.416942', Longitude = '-122.582528' WHERE City = 'Hixon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.383333', Longitude = '-125.916667' WHERE City = 'Hkusam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.8943443', Longitude = '-122.7919639' WHERE City = 'Hodda' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2049293', Longitude = '-122.0294141' WHERE City = 'Holachten 8' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.652272', Longitude = '-128.0252939' WHERE City = 'Holberg' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.334523', Longitude = '-123.156982' WHERE City = 'Hollyburn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.616667', Longitude = '-119.95' WHERE City = 'Holmwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3', Longitude = '-124.633333' WHERE City = 'Homfray Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.815585', Longitude = '-124.170934' WHERE City = 'Honeymoon Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.7596274', Longitude = '-120.2376623' WHERE City = 'Honeymoon Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.379491', Longitude = '-121.4416917' WHERE City = 'Hope' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8026802', Longitude = '-123.276987' WHERE City = 'Hope Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9187195', Longitude = '-127.8769597' WHERE City = 'Hope Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.924875', Longitude = '-126.817695' WHERE City = 'Hopetown' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.429255', Longitude = '-123.480212' WHERE City = 'Hopkins Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5353712', Longitude = '-124.6758784' WHERE City = 'Hornby Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.6857744', Longitude = '-119.0319836' WHERE City = 'Horse Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.33273', Longitude = '-121.417202' WHERE City = 'Horsefly' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5885048', Longitude = '-114.9615989' WHERE City = 'Hosmer' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3683298', Longitude = '-126.2723705' WHERE City = 'Hot Springs Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.3979972', Longitude = '-126.6482087' WHERE City = 'Houston' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.301146', Longitude = '-116.9482161' WHERE City = 'Howser' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.210125', Longitude = '-122.538609' WHERE City = 'Huble' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.0357172', Longitude = '-121.9038286' WHERE City = 'Hudson''s Hope' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.957588', Longitude = '-123.762299' WHERE City = 'Hulatt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Hulcross' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5', Longitude = '-119.266667' WHERE City = 'Hullcar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.9934291', Longitude = '-130.4875214' WHERE City = 'Humpback Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2147321', Longitude = '-123.9713352' WHERE City = 'Hunaechin Sechelt Band 11' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.004121', Longitude = '-122.2652403' WHERE City = 'Huntingdon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.004121', Longitude = '-122.2652403' WHERE City = 'Huntington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.070717', Longitude = '-130.448296' WHERE City = 'Hunts Inlet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.616667', Longitude = '-118.766667' WHERE City = 'Hupel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.014557', Longitude = '-116.46739' WHERE City = 'Huscroft' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.283155', Longitude = '-123.1123026' WHERE City = 'Hutchinson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.283155', Longitude = '-123.1123026' WHERE City = 'Hutchison' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.981499', Longitude = '-121.618103' WHERE City = 'Hutton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.105421', Longitude = '-125.2207766' WHERE City = 'Hyacinthe Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.583333', Longitude = '-126.9999999' WHERE City = 'Hyde Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.616667', Longitude = '-121.7' WHERE City = 'Hydraulic' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.650718', Longitude = '-128.163209' WHERE City = 'Hyland Post' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.221789', Longitude = '-117.4215346' WHERE City = 'Illecillewaet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Imperial Ranchettes' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6546335', Longitude = '-119.6287165' WHERE City = 'Indian Rock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.696914', Longitude = '-125.169901' WHERE City = 'Ingenika Mine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.122614', Longitude = '-119.509009' WHERE City = 'Inkaneep' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Inkitsaph' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5064562', Longitude = '-116.0291433' WHERE City = 'Invermere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2831515', Longitude = '-122.7603217' WHERE City = 'Irvine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6329236', Longitude = '-124.0575314' WHERE City = 'Irvines Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.833333', Longitude = '-129.9833328' WHERE City = 'Iskut' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.925305', Longitude = '-122.739274' WHERE City = 'Island Cache' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.960566', Longitude = '-123.234075' WHERE City = 'Isle Pierre' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.8844543', Longitude = '-121.4090871' WHERE City = 'Jackfish Lake Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.9333606', Longitude = '-119.3898153' WHERE City = 'Jackman' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.514716', Longitude = '-125.755104' WHERE City = 'Jackson Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.616667', Longitude = '-131.6833329' WHERE City = 'Jacksons' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.258333', Longitude = '-129.625' WHERE City = 'Jade City' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3708364', Longitude = '-115.3019374' WHERE City = 'Jaffray' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2611461', Longitude = '-123.1260327' WHERE City = 'Jaleslie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6047009', Longitude = '-123.3459708' WHERE City = 'James Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Jayem' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.3', Longitude = '-131.2166671' WHERE City = 'Jedway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.675937', Longitude = '-120.282421' WHERE City = 'Jellicoe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1', Longitude = '-117.2333329' WHERE City = 'Jersey' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0560375', Longitude = '-123.8145253' WHERE City = 'Jervis Inlet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.2485139', Longitude = '-121.9566722' WHERE City = 'Jesmond' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.442893', Longitude = '-127.4949369' WHERE City = 'Jeune Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4822616', Longitude = '-115.8465284' WHERE City = 'Jim Smith Lake and Area' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.409161', Longitude = '-122.857668' WHERE City = 'Johnson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0803325', Longitude = '-116.8791676' WHERE City = 'Johnsons Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.420459', Longitude = '-124.044797' WHERE City = 'Jordan River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8128342', Longitude = '-119.3835179' WHERE City = 'June Springs Estates' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6630211', Longitude = '-120.2507928' WHERE City = 'Juniper Ridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.533974', Longitude = '-120.449418' WHERE City = 'Jura' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.6142502', Longitude = '-132.312129' WHERE City = 'Juskatla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.35614', Longitude = '-120.9081901' WHERE City = 'Kahntah' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.3557043', Longitude = '-120.9070516' WHERE City = 'Kahntah 3' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.0328014', Longitude = '-132.4499879' WHERE City = 'Kaisun' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.188256', Longitude = '-125.914074' WHERE City = 'Kakawis' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.173685', Longitude = '-119.330827' WHERE City = 'Kalamalka' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.397805', Longitude = '-119.6051351' WHERE City = 'Kaleden' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6247184', Longitude = '-126.9728561' WHERE City = 'Kaleva' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.7386617', Longitude = '-121.3630725' WHERE City = 'Kallum' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.674522', Longitude = '-120.3272675' WHERE City = 'Kamloops' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Kanaka' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.115318', Longitude = '-121.563842' WHERE City = 'Kanaka Bar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9142499', Longitude = '-116.9154503' WHERE City = 'Kaslo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3686135', Longitude = '-121.5512291' WHERE City = 'Katz' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.56512', Longitude = '-123.403472' WHERE City = 'Keating' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3984929', Longitude = '-123.4507319' WHERE City = 'Keats Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.31614', Longitude = '-119.198396' WHERE City = 'Kedleston' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.02459', Longitude = '-121.530998' WHERE City = 'Keefers' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2487935', Longitude = '-122.9113808' WHERE City = 'Keekwillie Trailer Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.766667', Longitude = '-121.416667' WHERE City = 'Keithley Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8879519', Longitude = '-119.4960106' WHERE City = 'Kelowna' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.560594', Longitude = '-127.9397731' WHERE City = 'Kemano' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.483333', Longitude = '-128.116667' WHERE City = 'Kemano Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.741667', Longitude = '-126.65' WHERE City = 'Kendrick Camp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.102188', Longitude = '-122.797203' WHERE City = 'Kennedy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2515745', Longitude = '-121.8553025' WHERE City = 'Kent' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2024624', Longitude = '-119.8294828' WHERE City = 'Keremeos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.042179', Longitude = '-118.746408' WHERE City = 'Kerr Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.818813', Longitude = '-122.418658' WHERE City = 'Kersley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.056813', Longitude = '-118.943446' WHERE City = 'Kettle Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.135531', Longitude = '-123.103821' WHERE City = 'Kidd' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.7', Longitude = '-127.333333' WHERE City = 'Kilbella Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.833333', Longitude = '-128.4833329' WHERE City = 'Kildala Arm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.003956', Longitude = '-124.991681' WHERE City = 'Kildonan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.067743', Longitude = '-122.19357' WHERE City = 'Kilgard' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.839841', Longitude = '-120.27089' WHERE City = 'Kilkerran' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.185915', Longitude = '-119.505553' WHERE City = 'Killiney Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Killy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6651567', Longitude = '-115.9967207' WHERE City = 'Kimberley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.833333', Longitude = '-126.95' WHERE City = 'Kimsquit' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.994444', Longitude = '-129.9541671' WHERE City = 'Kincolith' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.966667', Longitude = '-126.183333' WHERE City = 'Kingcome' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.95', Longitude = '-126.2' WHERE City = 'Kingcome Inlet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.616667', Longitude = '-118.733333' WHERE City = 'Kingfisher' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.006039', Longitude = '-116.180387' WHERE City = 'Kingsgate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.91135', Longitude = '-120.906998' WHERE City = 'Kingsvale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.27753', Longitude = '-117.6486559' WHERE City = 'Kinnaird' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.6193298', Longitude = '-120.479629' WHERE City = 'Kiskatinaw' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.349017', Longitude = '-127.6877799' WHERE City = 'Kispiox' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.3030708', Longitude = '-127.6919287' WHERE City = 'Kispiox Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Kissick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.974585', Longitude = '-128.647123' WHERE City = 'Kitamaat Village' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.157992', Longitude = '-116.335749' WHERE City = 'Kitchener' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.049366', Longitude = '-128.6283529' WHERE City = 'Kitimat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.79448', Longitude = '-130.4332191' WHERE City = 'Kitkatla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.454649', Longitude = '-129.472385' WHERE City = 'Kitsault' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.083333', Longitude = '-127.8333329' WHERE City = 'Kitseguecla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.565681', Longitude = '-128.462799' WHERE City = 'Kitselas' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.518018', Longitude = '-128.6371399' WHERE City = 'Kitsumkalum' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.788592', Longitude = '-124.995709' WHERE City = 'Kitty Coleman' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.111672', Longitude = '-128.0267756' WHERE City = 'Kitwanga' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.1769399', Longitude = '-133.0233669' WHERE City = 'Kiusta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5036114', Longitude = '-123.7470259' WHERE City = 'Klaalth Sechelt Band 5' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4741736', Longitude = '-123.7545601' WHERE City = 'Klayekwim Sechelt Band 6' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7661436', Longitude = '-123.7142631' WHERE City = 'Klayekwim Sechelt Band 6A' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7766969', Longitude = '-123.7259765' WHERE City = 'Klayekwim Sechelt Band 7' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7913516', Longitude = '-123.7252393' WHERE City = 'Klayekwim Sechelt Band 8' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.297288', Longitude = '-124.948252' WHERE City = 'Kleecoot' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.936494', Longitude = '-124.801922' WHERE City = 'Kleena Kleene' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.633333', Longitude = '-123.966667' WHERE City = 'Kleindale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.5936331', Longitude = '-128.5241004' WHERE City = 'Klemtu' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.11834', Longitude = '-122.3521014' WHERE City = 'Klua' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.311086', Longitude = '0.1062' WHERE City = 'Knockholt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6334657', Longitude = '-120.3141559' WHERE City = 'Knutsford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.638889', Longitude = '-121.65' WHERE City = 'Kobes' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.533333', Longitude = '-126.85' WHERE City = 'Kokish' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.758618', Longitude = '-123.682508' WHERE City = 'Koksilah' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.678417', Longitude = '-116.870564' WHERE City = 'Kootenay Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.883351', Longitude = '-116.049325' WHERE City = 'Kootenay Crossing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0993712', Longitude = '-115.7574143' WHERE City = 'Kootenay Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Koster' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.174438', Longitude = '-115.217632' WHERE City = 'Kragmont' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.444242', Longitude = '-117.58367' WHERE City = 'Krestova' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.865834', Longitude = '-127.902837' WHERE City = 'Kuldo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.4227131', Longitude = '-129.263339' WHERE City = 'Kulkayu 4' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.0501014', Longitude = '-132.570432' WHERE City = 'Kung' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.9703723', Longitude = '-123.6446011' WHERE City = 'Kuper Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.300157', Longitude = '-116.662017' WHERE City = 'Kuskonook' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.3137181', Longitude = '-130.3309406' WHERE City = 'Kwinitsa' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7025031', Longitude = '-124.8674291' WHERE City = 'Kye Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0306331', Longitude = '-127.3788125' WHERE City = 'Kyuquot' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.812737', Longitude = '-121.4720029' WHERE City = 'Lac la Hache' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.487363', Longitude = '-120.495422' WHERE City = 'Lac Le Jeune' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Lachkaltsap 9' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.99534', Longitude = '-123.8161' WHERE City = 'Ladysmith' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Lafferty' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.324288', Longitude = '-121.615215' WHERE City = 'Laidlaw' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1842638', Longitude = '-122.0756713' WHERE City = 'Lakahahmen 11' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.375901', Longitude = '-122.867806' WHERE City = 'Lake Buntzen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0548556', Longitude = '-119.4147883' WHERE City = 'Lake Country, District of' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8258118', Longitude = '-124.054167' WHERE City = 'Lake Cowichan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.220123', Longitude = '-122.0112629' WHERE City = 'Lake Errock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.822355', Longitude = '-127.219989' WHERE City = 'Lake Kathlyn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.383333', Longitude = '-128.516667' WHERE City = 'Lakelse' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.370323', Longitude = '-128.530987' WHERE City = 'Lakelse Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.698054', Longitude = '-130.1089589' WHERE City = 'Laketon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.865994', Longitude = '-119.53516' WHERE City = 'Lakeview Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.35', Longitude = '-120.266667' WHERE City = 'Lamming Mills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.780189', Longitude = '-124.350535' WHERE City = 'Lang Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.433873', Longitude = '-123.479057' WHERE City = 'Langdale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4474626', Longitude = '-123.4956337' WHERE City = 'Langford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1041779', Longitude = '-122.6603519' WHERE City = 'Langley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2506156', Longitude = '-124.075031' WHERE City = 'Lantzville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.708848', Longitude = '-119.135613' WHERE City = 'Larch Hill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.146025', Longitude = '-116.954625' WHERE City = 'Lardeau' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2583176', Longitude = '-123.2481377' WHERE City = 'Larkin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.983333', Longitude = '-124.683333' WHERE City = 'Larsons Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Lasha' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.483333', Longitude = '-124.266667' WHERE City = 'Lasqueti' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.483333', Longitude = '-124.266667' WHERE City = 'Lasqueti Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Lauretta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.23639', Longitude = '-119.107243' WHERE City = 'Lavington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.402108', Longitude = '-131.9263869' WHERE City = 'Lawnhill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.554381', Longitude = '-130.43393' WHERE City = 'Lax Kw''alaams' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7137664', Longitude = '-124.910051' WHERE City = 'Lazo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.2961188', Longitude = '-116.9631367' WHERE City = 'Leanchoil' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.59214', Longitude = '-117.5881859' WHERE City = 'Lebahdo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.903539', Longitude = '-119.5534689' WHERE City = 'Lee Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.493955', Longitude = '-123.712449' WHERE City = 'Leechtown' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.941465', Longitude = '-123.100902' WHERE City = 'Lees Corner' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.405892', Longitude = '-120.389518' WHERE City = 'Legrand' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.05657', Longitude = '-124.737967' WHERE City = 'Lejac' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.701882', Longitude = '-117.489883' WHERE City = 'Lemon Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.537789', Longitude = '-122.483789' WHERE City = 'Lemoray' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.450172', Longitude = '-119.132446' WHERE City = 'Lempriere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.08546', Longitude = '-125.546578' WHERE City = 'Leo Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.510006', Longitude = '-122.1142301' WHERE City = 'Lexau Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.416667', Longitude = '-126.083333' WHERE City = 'Liard River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.9315819', Longitude = '-128.5061617' WHERE City = 'Liard River 3' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Liersch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.870756', Longitude = '-123.288866' WHERE City = 'Lighthouse Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.616667', Longitude = '-121.5500001' WHERE City = 'Likely' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6863017', Longitude = '-121.9367502' WHERE City = 'Lillooet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.916667', Longitude = '-124.5499999' WHERE City = 'Lily Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2749176', Longitude = '-123.1224948' WHERE City = 'Lime' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.278505', Longitude = '-122.740491' WHERE City = 'Lincoln Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.034456', Longitude = '-122.015837' WHERE City = 'Lindell Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.7850441', Longitude = '-135.087443' WHERE City = 'Lindeman' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.952283', Longitude = '-114.7556627' WHERE City = 'Line Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4592529', Longitude = '-123.234139' WHERE City = 'Lions Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.050396', Longitude = '-116.469498' WHERE City = 'Lister' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.4239653', Longitude = '-120.2053428' WHERE City = 'Little Fort' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.734223', Longitude = '-124.909888' WHERE City = 'Little River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.988982', Longitude = '-126.8532628' WHERE City = 'Little Zeballos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.71788', Longitude = '-131.8348921' WHERE City = 'Lockeport' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1322567', Longitude = '-121.9400029' WHERE City = 'Log Cabin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4986205', Longitude = '-121.0328413' WHERE City = 'Logan Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.554651', Longitude = '-121.20105' WHERE City = 'Lone Butte' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.567681', Longitude = '-121.384058' WHERE City = 'Lone Prairie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.85', Longitude = '-123.45' WHERE City = 'Long Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0688558', Longitude = '-125.7538321' WHERE City = 'Longbeach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.916667', Longitude = '-121.466667' WHERE City = 'Longworth' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1238076', Longitude = '-121.2053841' WHERE City = 'Loon Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1238076', Longitude = '-121.2053841' WHERE City = 'Loon Lake Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.5997', Longitude = '-120.702366' WHERE City = 'Loos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.138241', Longitude = '-120.122278' WHERE City = 'Louis Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.9512936', Longitude = '-131.7317657' WHERE City = 'Louise Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.215685', Longitude = '-117.6833971' WHERE City = 'Lower China Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.156549', Longitude = '-120.8797919' WHERE City = 'Lower Nicola' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.923837', Longitude = '-128.4863905' WHERE City = 'Lower Post' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6986969', Longitude = '-123.1530568' WHERE City = 'Lucas' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.85', Longitude = '-118.55' WHERE City = 'Lucerne' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4026983', Longitude = '-120.2282011' WHERE City = 'Lulu 5' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4242009', Longitude = '-115.880233' WHERE City = 'Lumberton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.250699', Longitude = '-118.967831' WHERE City = 'Lumby' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9817127', Longitude = '-124.7590955' WHERE City = 'Lund' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.95378', Longitude = '-122.4429219' WHERE City = 'Lust Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2678118', Longitude = '-123.0086702' WHERE City = 'Luxor' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.6594626', Longitude = '-131.531895' WHERE City = 'Lyell Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.069238', Longitude = '-121.8347701' WHERE City = 'Lynx Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.233258', Longitude = '-121.581404' WHERE City = 'Lytton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2418155', Longitude = '-121.6001709' WHERE City = 'Lytton 27B' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5612599', Longitude = '-118.7359302' WHERE City = 'Mabel Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.45', Longitude = '-122.4' WHERE City = 'Macalister' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.17759', Longitude = '-123.1335939' WHERE City = 'Macdonald' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.338446', Longitude = '-123.0948068' WHERE City = 'Mackenzie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2377686', Longitude = '-122.8633377' WHERE City = 'Mackin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.590142', Longitude = '-127.0847551' WHERE City = 'MacNeill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.617717', Longitude = '-124.021013' WHERE City = 'Madeira Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7613499', Longitude = '-123.283821' WHERE City = 'Magic Lake Estates' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9651179', Longitude = '-119.282764' WHERE City = 'Magna Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.5', Longitude = '-125.166667' WHERE City = 'Magnum Mine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.457536', Longitude = '-127.8016959' WHERE City = 'Mahatta River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6214999', Longitude = '-126.5715223' WHERE City = 'Mahmalillikullah' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.838738', Longitude = '-120.64945' WHERE City = 'Mahood Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.067839', Longitude = '-117.910786' WHERE City = 'Makinson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.544839', Longitude = '-123.5645412' WHERE City = 'Malahat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.938046', Longitude = '-118.7944749' WHERE City = 'Malakwa' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6447542', Longitude = '-126.9948139' WHERE City = 'Malcolm Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.165042', Longitude = '-123.858264' WHERE City = 'Malibu' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.615768', Longitude = '-126.5754009' WHERE City = 'Mamalilaculla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Mammel Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0689289', Longitude = '-120.9149345' WHERE City = 'Manning' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.064647', Longitude = '-120.781581' WHERE City = 'Manning Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.671712', Longitude = '-124.487338' WHERE City = 'Manson Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.063706', Longitude = '-124.981603' WHERE City = 'Mansons Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.883333', Longitude = '-123.8666671' WHERE City = 'Mapes' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.817237', Longitude = '-123.61511' WHERE City = 'Maple Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2193226', Longitude = '-122.5983981' WHERE City = 'Maple Ridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7840144', Longitude = '-119.0105146' WHERE City = 'Mara' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.25', Longitude = '-116.966667' WHERE City = 'Marblehead' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.33292', Longitude = '-127.489197' WHERE City = 'Margaret Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.501475', Longitude = '-122.4302779' WHERE City = 'Marguerite' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.469782', Longitude = '-123.388767' WHERE City = 'Marigold' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.701279', Longitude = '-125.847423' WHERE City = 'Marilla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2822034', Longitude = '-126.0729446' WHERE City = 'Marktosis' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4084366', Longitude = '-123.3242669' WHERE City = 'Marne' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.367931', Longitude = '-119.673452' WHERE City = 'Marron Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1448284', Longitude = '-117.5199906' WHERE City = 'Marsh Creek Area' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Martel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0504912', Longitude = '-116.5538606' WHERE City = 'Marten Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6640749', Longitude = '-119.812005' WHERE City = 'Martin Prairie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7899414', Longitude = '-119.5422662' WHERE City = 'Martin Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Martinson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6346194', Longitude = '-115.9532473' WHERE City = 'Marysville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.333333', Longitude = '-122.783333' WHERE City = 'Mason Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.0114581', Longitude = '-132.1471978' WHERE City = 'Masset' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.55', Longitude = '-126.183333' WHERE City = 'Matilpi' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1082279', Longitude = '-122.3489709' WHERE City = 'Matsqui Main 2' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2839008', Longitude = '-125.1446928' WHERE City = 'Maurelle Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8476105', Longitude = '-123.2838913' WHERE City = 'Mayne Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.483407', Longitude = '-115.570803' WHERE City = 'Mayook' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'McAbee' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.301347', Longitude = '-120.168461' WHERE City = 'McBride' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'McCabe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5018135', Longitude = '-123.3843295' WHERE City = 'McCall' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.994952', Longitude = '-124.015899' WHERE City = 'McCalls Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.157532', Longitude = '-116.3367' WHERE City = 'McConnel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6727193', Longitude = '-120.2946439' WHERE City = 'McCracken' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.796409', Longitude = '-119.19611' WHERE City = 'McCulloch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.186423', Longitude = '-129.2266129' WHERE City = 'McDame' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54', Longitude = '-126.033333' WHERE City = 'McDonalds Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6186972', Longitude = '-122.4315096' WHERE City = 'McGillivray' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.083333', Longitude = '-121.8333329' WHERE City = 'McGregor' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7038441', Longitude = '-119.2766986' WHERE City = 'McGuire' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.630953', Longitude = '-122.450263' WHERE City = 'McKearney Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.960807', Longitude = '-119.460056' WHERE City = 'McKinley Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.513767', Longitude = '-122.1444499' WHERE City = 'McLean Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.415371', Longitude = '-122.289717' WHERE City = 'McLeese Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.993075', Longitude = '-123.043873' WHERE City = 'McLeod' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.993075', Longitude = '-123.043873' WHERE City = 'McLeod Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'McLeod Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.05', Longitude = '-120.233333' WHERE City = 'McLure' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1759118', Longitude = '-122.56444' WHERE City = 'McMillan Island 6' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.150756', Longitude = '-116.767246' WHERE City = 'McMurdo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6672889', Longitude = '-119.515984' WHERE City = 'McMurphy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.557069', Longitude = '-123.3957229' WHERE City = 'McNab Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.623159', Longitude = '-116.267654' WHERE City = 'Meachen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.233333', Longitude = '-116.983333' WHERE City = 'Meadow Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1752746', Longitude = '-125.8390856' WHERE City = 'Meares Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.619444', Longitude = '-126.575' WHERE City = 'Meem Quam Leese' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.2029625', Longitude = '-122.5360913' WHERE City = 'Meldrum' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.110047', Longitude = '-122.341299' WHERE City = 'Meldrum Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1113079', Longitude = '-120.7862222' WHERE City = 'Merritt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.5114474', Longitude = '-122.9876873' WHERE City = 'Merton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.79243', Longitude = '-125.04846' WHERE City = 'Merville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.808933', Longitude = '-124.124293' WHERE City = 'Mesachie Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.9302917', Longitude = '-119.3583487' WHERE City = 'Messiter' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.382005', Longitude = '-123.53785' WHERE City = 'Metchosin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.337324', Longitude = '-130.444669' WHERE City = 'Metlakatla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.101389', Longitude = '-129.3' WHERE City = 'Meziadin Junction' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.101389', Longitude = '-129.3' WHERE City = 'Meziadin Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.0058999', Longitude = '-118.564275' WHERE City = 'Mica Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.219301', Longitude = '-122.964741' WHERE City = 'Middlegate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0102185', Longitude = '-118.7743535' WHERE City = 'Midway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Mile 19 Overhead' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.371537', Longitude = '-121.070046' WHERE City = 'Mile 62 1/2' AND IsHidden = 0");
//            migrationBuilder.Sql(
//                "UPDATE LocationLookups Set Latitude = '48.6505297', Longitude = '-123.5571899' WHERE City = 'Mill Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3897369', Longitude = '-123.329585' WHERE City = 'Millers Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.494844', Longitude = '-123.531205' WHERE City = 'Millstream' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.392937', Longitude = '-123.701406' WHERE City = 'Milnes Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.612029', Longitude = '-123.2134771' WHERE City = 'Minaty Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6202885', Longitude = '-126.3231047' WHERE City = 'Minstrel Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.198206', Longitude = '-121.948762' WHERE City = 'Minto Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.240633', Longitude = '-121.7915299' WHERE City = 'Miocene' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.255835', Longitude = '-122.2476251' WHERE City = 'Miracle Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.883176', Longitude = '-116.899124' WHERE City = 'Mirror Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7814984', Longitude = '-120.5109468' WHERE City = 'Missezula Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1329272', Longitude = '-122.3261603' WHERE City = 'Mission' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1329272', Longitude = '-122.3261603' WHERE City = 'Mission Island 2' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.633333', Longitude = '-126.85' WHERE City = 'Mitchell Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.9574751', Longitude = '-122.928676' WHERE City = 'Miworth' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.8233319', Longitude = '-121.7888866' WHERE City = 'Moberly' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.8233319', Longitude = '-121.7888866' WHERE City = 'Moberly Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.2808179', Longitude = '-121.4621364' WHERE City = 'Moffat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.872222', Longitude = '-122.169444' WHERE City = 'Moha' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.0551787', Longitude = '-121.239595' WHERE City = 'Monias' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2534465', Longitude = '-123.0301804' WHERE City = 'Mons' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8924435', Longitude = '-123.3894132' WHERE City = 'Montague Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.64894', Longitude = '-119.9564311' WHERE City = 'Monte Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.526671', Longitude = '-119.827305' WHERE City = 'Monte Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.450045', Longitude = '-120.926468' WHERE City = 'Montney' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0789717', Longitude = '-117.5923651' WHERE City = 'Montrose' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.083551', Longitude = '-122.4982269' WHERE City = 'Moose Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6322099', Longitude = '-126.4580875' WHERE City = 'Mooyah Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3697902', Longitude = '-125.0259321' WHERE City = 'Moran' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.048796', Longitude = '-132.0318999' WHERE City = 'Moresby Camp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Morey' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.033333', Longitude = '-127.333333' WHERE City = 'Moricetown' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.392589', Longitude = '-115.0141459' WHERE City = 'Morrissey' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.151417', Longitude = '-119.2403279' WHERE City = 'Mount Baldy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3163843', Longitude = '-122.7173501' WHERE City = 'Mount Currie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.116741', Longitude = '-122.4042878' WHERE City = 'Mount Lehman' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.033859', Longitude = '-119.231587' WHERE City = 'Mount Robson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.484858', Longitude = '-117.285835' WHERE City = 'Mountain Station' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.288053', Longitude = '-115.833511' WHERE City = 'Moyie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.766667', Longitude = '-123.016667' WHERE City = 'Mud River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.926463', Longitude = '-125.771076' WHERE City = 'Muncho Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4251848', Longitude = '-123.3680609' WHERE City = 'Munro' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.347072', Longitude = '-122.6847183' WHERE City = 'Munroe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.533333', Longitude = '-121' WHERE City = 'Murdale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.751919', Longitude = '-123.545025' WHERE City = 'Musgrave Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.762522', Longitude = '-122.6743869' WHERE City = 'Muskwa' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.803899', Longitude = '-119.31185' WHERE City = 'Myra' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.798032', Longitude = '-124.481112' WHERE City = 'Myrtle Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.849772', Longitude = '-132.115688' WHERE City = 'Nadu' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.05', Longitude = '-124.866667' WHERE City = 'Nahmint' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.083326', Longitude = '-119.498311' WHERE City = 'Nahun' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2398537', Longitude = '-117.8011055' WHERE City = 'Nakusp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Nalos Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.8613857', Longitude = '-127.8671337' WHERE City = 'Namu' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1658836', Longitude = '-123.9400648' WHERE City = 'Nanaimo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2723165', Longitude = '-124.1930531' WHERE City = 'Nanoose Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5962947', Longitude = '-119.5956537' WHERE City = 'Naramata' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.73888', Longitude = '-122.5247381' WHERE City = 'Narcosli Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5407508', Longitude = '-117.2624005' WHERE City = 'Nasookin Road Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.282358', Longitude = '-128.9931519' WHERE City = 'Nass Camp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7897222', Longitude = '-114.8238888' WHERE City = 'Natal' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53', Longitude = '-123.616667' WHERE City = 'Nazko' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.9651789', Longitude = '-123.7222156' WHERE City = 'Nechako Centre' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.047307', Longitude = '-126.3244548' WHERE City = 'Nedoats 11' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.874409', Longitude = '-118.097549' WHERE City = 'Needles' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Needley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4928119', Longitude = '-117.2948343' WHERE City = 'Nelson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.497361', Longitude = '-124.0085451' WHERE City = 'Nelson Forks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7264515', Longitude = '-124.1102331' WHERE City = 'Nelson Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.000447', Longitude = '-117.299432' WHERE City = 'Nelway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.483333', Longitude = '-123.8833329' WHERE City = 'Nemiah Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.20578', Longitude = '-129.07833' WHERE City = 'New Aiyansh' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.0703513', Longitude = '-121.5137657' WHERE City = 'New Barkerville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.20301', Longitude = '-128.3383137' WHERE City = 'New Bella Bella' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.450159', Longitude = '-123.4372871' WHERE City = 'New Brighton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.025879', Longitude = '-131.781177' WHERE City = 'New Clew' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.99129', Longitude = '-117.3720711' WHERE City = 'New Denver' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.247553', Longitude = '-127.591578' WHERE City = 'New Hazelton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4166669', Longitude = '-117.6166669' WHERE City = 'New Settlement' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2057179', Longitude = '-122.910956' WHERE City = 'New Westminster' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.181264', Longitude = '-123.921879' WHERE City = 'Newcastle' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.025716', Longitude = '-115.197493' WHERE City = 'Newgate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.1', Longitude = '-122.2' WHERE City = 'Newlands' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.106313', Longitude = '-118.466454' WHERE City = 'Niagara' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5961973', Longitude = '-119.5766683' WHERE City = 'Nichol' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.2447783', Longitude = '-116.9108391' WHERE City = 'Nicholson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1207226', Longitude = '-116.5969274' WHERE City = 'Nicks Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.163604', Longitude = '-120.670557' WHERE City = 'Nicola' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2052777', Longitude = '-122.12' WHERE City = 'Nicomen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.133865', Longitude = '-121.299249' WHERE City = 'Nig' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8797341', Longitude = '-127.7234318' WHERE City = 'Nigei Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.332817', Longitude = '-126.916935' WHERE City = 'Nimpkish' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.566667', Longitude = '-127' WHERE City = 'Nimpkish Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.333333', Longitude = '-125.1500001' WHERE City = 'Nimpo Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6439705', Longitude = '-121.2950097' WHERE City = 'Ninety Three Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.1', Longitude = '-131.2166669' WHERE City = 'Ninstints' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Niteal' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6842999', Longitude = '-126.7894679' WHERE City = 'Nootka' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6842999', Longitude = '-126.7894679' WHERE City = 'Nootka Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.984195', Longitude = '-126.433459' WHERE City = 'Noralee' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.322175', Longitude = '-123.117561' WHERE City = 'Norgate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Norlake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.882363', Longitude = '-121.4558121' WHERE City = 'North Bend' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.4', Longitude = '-120.9166669' WHERE City = 'North Bonaparte' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.483333', Longitude = '-126.4833329' WHERE City = 'North Bulkley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.052098', Longitude = '-125.267304' WHERE City = 'North Campbell River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8428574', Longitude = '-123.7044012' WHERE City = 'North Cowichan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.9947099', Longitude = '-123.5836656' WHERE City = 'North Galiano' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.699279', Longitude = '-120.3628221' WHERE City = 'North Kamloops' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.9623637', Longitude = '-122.8301001' WHERE City = 'North Nechako' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7866869', Longitude = '-123.2893996' WHERE City = 'North Pender Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.420944', Longitude = '-120.768367' WHERE City = 'North Pine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0304688', Longitude = '-122.3645051' WHERE City = 'North Poplar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6197483', Longitude = '-123.4169098' WHERE City = 'North Saanich' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3431921', Longitude = '-115.2629821' WHERE City = 'North Star' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3199816', Longitude = '-123.0724139' WHERE City = 'North Vancouver' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1919871', Longitude = '-123.9867694' WHERE City = 'Northfield' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.702818', Longitude = '-123.1384319' WHERE City = 'Northridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2812537', Longitude = '-123.1162442' WHERE City = 'Norton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.85', Longitude = '-119.433333' WHERE City = 'Notch Hill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8069489', Longitude = '-126.9633782' WHERE City = 'Nuchatlitz' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.083308', Longitude = '-122.987465' WHERE City = 'Nukko Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.917135', Longitude = '-124.2011289' WHERE City = 'Nulki' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.016667', Longitude = '-118.4000001' WHERE City = 'Nursery' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4264814', Longitude = '-123.3141267' WHERE City = 'Oak Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.74518', Longitude = '-120.35418' WHERE City = 'Oak Hills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5097946', Longitude = '-123.7972687' WHERE City = 'Oalthkyim Sechelt Band 4' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.135216', Longitude = '-117.7466549' WHERE City = 'Oasis' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.354021', Longitude = '-127.693746' WHERE City = 'Ocean Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.950451', Longitude = '-125.200831' WHERE City = 'Ocean Grove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'O''Dell' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.791327', Longitude = '-122.831068' WHERE City = 'Ogden' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.045814', Longitude = '-119.449845' WHERE City = 'Okanagan Centre' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.344885', Longitude = '-119.5714929' WHERE City = 'Okanagan Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.239576', Longitude = '-119.3388541' WHERE City = 'Okanagan Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.822947', Longitude = '-119.488761' WHERE City = 'Okanagan Mission' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3647941', Longitude = '-119.2811785' WHERE City = 'O''Keefe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.260835', Longitude = '-119.828762' WHERE City = 'Olalla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.153924', Longitude = '-128.1209571' WHERE City = 'Old Bella Bella' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.47123', Longitude = '-128.7102159' WHERE City = 'Old Remo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.549669', Longitude = '-115.966677' WHERE City = 'Old Town' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1823264', Longitude = '-119.550428' WHERE City = 'Oliver' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.583333', Longitude = '-123.2208331' WHERE City = 'Oliver''s Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0486129', Longitude = '-122.786857' WHERE City = 'Olson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Onward' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.949473', Longitude = '-130.259568' WHERE City = 'Oona River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.283698', Longitude = '-117.631439' WHERE City = 'Ootischenia' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.283698', Longitude = '-117.631439' WHERE City = 'Ootischenia Flats' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.806821', Longitude = '-126.042532' WHERE City = 'Ootsa Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.173504', Longitude = '-125.90717' WHERE City = 'Opitsat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.603488', Longitude = '-120.3804599' WHERE City = 'Osborn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8647603', Longitude = '-123.6378415' WHERE City = 'Osborn Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.137204', Longitude = '-130.160619' WHERE City = 'Osland' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.032304', Longitude = '-119.468163' WHERE City = 'Osoyoos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.712762', Longitude = '-120.2062635' WHERE City = 'Osprey Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.380349', Longitude = '-121.358322' WHERE City = 'Othello' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.801034', Longitude = '-123.315844' WHERE City = 'Otter Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.971055', Longitude = '-122.873161' WHERE City = 'Otway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.680556', Longitude = '-127.225' WHERE City = 'Oweekeno' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.316667', Longitude = '-125.2166671' WHERE City = 'Owen Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.34751', Longitude = '-122.733194' WHERE City = 'Owl Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1106178', Longitude = '-119.3872402' WHERE City = 'Oyama' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.873264', Longitude = '-125.132104' WHERE City = 'Oyster River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.76844', Longitude = '-128.275454' WHERE City = 'Pacific' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9114125', Longitude = '-127.2900832' WHERE City = 'Pahas 3' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.790476', Longitude = '-123.852285' WHERE City = 'Paldi' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.35', Longitude = '-125.8833331' WHERE City = 'Palling' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.35', Longitude = '-125.8833331' WHERE City = 'Palling 1A' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7001072', Longitude = '-115.3853176' WHERE City = 'Palliser' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4583706', Longitude = '-116.2384659' WHERE City = 'Panorama' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4583706', Longitude = '-116.2384659' WHERE City = 'Panorama Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.799636', Longitude = '-119.1696229' WHERE City = 'Paradise Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.819809', Longitude = '-123.1555168' WHERE City = 'Paradise Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.173674', Longitude = '-117.495296' WHERE City = 'Park Siding' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.912567', Longitude = '-120.57115' WHERE City = 'Parkland' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3193375', Longitude = '-124.3136411' WHERE City = 'Parksville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.070135', Longitude = '-116.634861' WHERE City = 'Parson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.366334', Longitude = '-123.4613322' WHERE City = 'Pasley Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.388589', Longitude = '-117.679178' WHERE City = 'Pass Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.537594', Longitude = '-117.644185' WHERE City = 'Passmore' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.006832', Longitude = '-117.83438' WHERE City = 'Paterson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7395537', Longitude = '-120.1174855' WHERE City = 'Paul Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.203633', Longitude = '-118.117098' WHERE City = 'Paulson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Pavey' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.883894', Longitude = '-121.822792' WHERE City = 'Pavilion' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5586486', Longitude = '-119.7191967' WHERE City = 'Paxton Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4741736', Longitude = '-123.7545601' WHERE City = 'Paykulkum Sechelt Band 14' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7702994', Longitude = '-119.74079' WHERE City = 'Peachland' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.884459', Longitude = '-120.616075' WHERE City = 'Peejay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.322028', Longitude = '-122.8050498' WHERE City = 'Pemberton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.32767', Longitude = '-123.107655' WHERE City = 'Pemberton Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.442341', Longitude = '-122.91377' WHERE City = 'Pemberton Meadows' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7866869', Longitude = '-123.2893996' WHERE City = 'Pender Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.516667', Longitude = '-125.7166669' WHERE City = 'Pendleton Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4991381', Longitude = '-119.5937077' WHERE City = 'Pennington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.8433618', Longitude = '-121.2924098' WHERE City = 'Penny' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4991381', Longitude = '-119.5937077' WHERE City = 'Penticton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.516667', Longitude = '-126.4333329' WHERE City = 'Perow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.668741', Longitude = '-117.5036989' WHERE City = 'Perrys' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2861863', Longitude = '-123.1254321' WHERE City = 'Peterson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.552082', Longitude = '-125.3532439' WHERE City = 'Phillips Arm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.098458', Longitude = '-118.590177' WHERE City = 'Phoenix' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7039068', Longitude = '-123.4134259' WHERE City = 'Piers Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6572059', Longitude = '-116.8764681' WHERE City = 'Pilot Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.724929', Longitude = '-120.033542' WHERE City = 'Pinantan Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.566667', Longitude = '-124.5' WHERE City = 'Pinchi' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.627956', Longitude = '-124.404853' WHERE City = 'Pinchi Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.066667', Longitude = '-121.95' WHERE City = 'Pinegrove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Pinesul' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.8920922', Longitude = '-122.7880811' WHERE City = 'Pinewood Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.0713888', Longitude = '-122.8747222' WHERE City = 'Pink Mountain' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.757384', Longitude = '-122.780449' WHERE City = 'Pioneer Mine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Pitquah' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2190648', Longitude = '-122.6895165' WHERE City = 'Pitt Meadows' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.071336', Longitude = '-119.44508' WHERE City = 'Pixie Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.45', Longitude = '-117.533333' WHERE City = 'Playmor Junction' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.504408', Longitude = '-136.463234' WHERE City = 'Pleasant Camp' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.683333', Longitude = '-126.633333' WHERE City = 'Plumper Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.692328', Longitude = '-124.8682237' WHERE City = 'Point Holmes' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.5566666', Longitude = '-121.6358333' WHERE City = 'Polley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.619812', Longitude = '-124.0600719' WHERE City = 'Pope Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.203594', Longitude = '-121.738439' WHERE City = 'Popkum' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4166669', Longitude = '-117.133333' WHERE City = 'Poplar Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5123363', Longitude = '-119.5738472' WHERE City = 'Poplar Grove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.9934291', Longitude = '-130.4875214' WHERE City = 'Porcher Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5065188', Longitude = '-123.7469444' WHERE City = 'Porpoise Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2338882', Longitude = '-124.8055494' WHERE City = 'Port Alberni' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.951935', Longitude = '-125.5450221' WHERE City = 'Port Albion' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4254289', Longitude = '-127.4877075' WHERE City = 'Port Alice' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.6887137', Longitude = '-132.1847553' WHERE City = 'Port Clements' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2628382', Longitude = '-122.7810708' WHERE City = 'Port Coquitlam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.766667', Longitude = '-122.166667' WHERE City = 'Port Douglas' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.1956849', Longitude = '-130.1153024' WHERE City = 'Port Edward' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.15', Longitude = '-129.966667' WHERE City = 'Port Essington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2170148', Longitude = '-122.6227601' WHERE City = 'Port Hammond - Haney' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7123867', Longitude = '-127.460393' WHERE City = 'Port Hardy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.170087', Longitude = '-122.705324' WHERE City = 'Port Kells' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.590142', Longitude = '-127.0847551' WHERE City = 'Port McNeill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5217479', Longitude = '-123.4880589' WHERE City = 'Port Mellon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2849107', Longitude = '-122.8677562' WHERE City = 'Port Moody' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4929341', Longitude = '-126.0858154' WHERE City = 'Port Neville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.552966', Longitude = '-124.422299' WHERE City = 'Port Renfrew' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.554381', Longitude = '-130.43393' WHERE City = 'Port Simpson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.815559', Longitude = '-123.318612' WHERE City = 'Port Washington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.557304', Longitude = '-123.232734' WHERE City = 'Porteau' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Porteous' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.800907', Longitude = '-130.101152' WHERE City = 'Porter Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.721447', Longitude = '-123.3770195' WHERE City = 'Portland Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.331291', Longitude = '-117.244673' WHERE City = 'Porto Rico' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Poser' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9928879', Longitude = '-119.2090493' WHERE City = 'Postill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0888788', Longitude = '-122.6923915' WHERE City = 'Potter' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.7161664', Longitude = '-120.1338865' WHERE City = 'Pouce Coupe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8352352', Longitude = '-124.5247061' WHERE City = 'Powell River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Powers Addition' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.594305', Longitude = '-119.70566' WHERE City = 'Prairie Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.050921', Longitude = '-130.028249' WHERE City = 'Premier' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.95', Longitude = '-115.65' WHERE City = 'Premier Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.92158', Longitude = '-121.0607481' WHERE City = 'Prespatou' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.3754309', Longitude = '-121.0374781' WHERE City = 'Pressy Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.9170641', Longitude = '-122.7496693' WHERE City = 'Prince George' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.3150367', Longitude = '-130.3208187' WHERE City = 'Prince Rupert' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4589588', Longitude = '-120.5061567' WHERE City = 'Princeton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6864291', Longitude = '-119.8190069' WHERE City = 'Pritchard' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.618452', Longitude = '-116.959232' WHERE City = 'Procter' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.782474', Longitude = '-120.7161309' WHERE City = 'Progress' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1038053', Longitude = '-121.9351391' WHERE City = 'Promontory' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.090975', Longitude = '-122.7095109' WHERE City = 'Prophet River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.523842', Longitude = '-123.438695' WHERE City = 'Prospect Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1796297', Longitude = '-123.9198495' WHERE City = 'Protection Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.433333', Longitude = '-123.1833329' WHERE City = 'Punchaw' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.660904', Longitude = '-125.0568021' WHERE City = 'Puntledge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.9214966', Longitude = '-121.9041388' WHERE City = 'Purden Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1093202', Longitude = '-116.5227438' WHERE City = 'Pyramid' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2057635', Longitude = '-125.2682421' WHERE City = 'Quadra Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.396248', Longitude = '-124.610053' WHERE City = 'Qualicum Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3482346', Longitude = '-124.4428262' WHERE City = 'Qualicum Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8044491', Longitude = '-123.6577671' WHERE City = 'Quamichan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.051056', Longitude = '-125.2185621' WHERE City = 'Quathiaski Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.535387', Longitude = '-127.654504' WHERE City = 'Quatsino' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.9378862', Longitude = '-122.9787857' WHERE City = 'Quaw' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.2549952', Longitude = '-132.0868676' WHERE City = 'Queen Charlotte City' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.65673', Longitude = '-116.932872' WHERE City = 'Queens Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.884993', Longitude = '-126.985584' WHERE City = 'Queens Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.9817372', Longitude = '-122.4949058' WHERE City = 'Quesnel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.9970227', Longitude = '-122.3921974' WHERE City = 'Quesnel Canyon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.6666669', Longitude = '-121.666667' WHERE City = 'Quesnel Forks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.954444', Longitude = '-122.517734' WHERE City = 'Quesnel View' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.695016', Longitude = '-127.04761' WHERE City = 'Quick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.171252', Longitude = '-120.491475' WHERE City = 'Quilchena' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.983168', Longitude = '-125.4325859' WHERE City = 'Quinsam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6200385', Longitude = '-116.0733586' WHERE City = 'Radium' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6200385', Longitude = '-116.0733586' WHERE City = 'Radium Hot Springs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.6868484', Longitude = '-119.7645482' WHERE City = 'Raft River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.9435151', Longitude = '-121.4692406' WHERE City = 'Rail Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.55', Longitude = '-136.5333331' WHERE City = 'Rainy Hollow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6521789', Longitude = '-119.1934641' WHERE City = 'Ranchero' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.334123', Longitude = '-117.6576161' WHERE City = 'Raspberry' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.153525', Longitude = '-119.9173894' WHERE City = 'Raush Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8152093', Longitude = '-120.3074348' WHERE City = 'Rayleigh' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1868849', Longitude = '-125.091656' WHERE City = 'Read Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2720389', Longitude = '-123.0761415' WHERE City = 'Reclaim' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.9643609', Longitude = '-122.4675381' WHERE City = 'Red Bluff' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0919444', Longitude = '-117.8211111' WHERE City = 'Red Mountain' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.9867872', Longitude = '-119.0084552' WHERE City = 'Red Pass' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.680113', Longitude = '-122.672359' WHERE City = 'Red Rock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.132173', Longitude = '-127.625854' WHERE City = 'Red Rose' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.361491', Longitude = '1.001518' WHERE City = 'Redgrave' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.498988', Longitude = '-123.909926' WHERE City = 'Redroofs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.124665', Longitude = '-123.681047' WHERE City = 'Redstone' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1237487', Longitude = '-124.8390156' WHERE City = 'Refuge Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.966667', Longitude = '-123.1000001' WHERE City = 'Reid Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.029226', Longitude = '-117.386555' WHERE City = 'Remac' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.483333', Longitude = '-128.7166669' WHERE City = 'Remo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.433203', Longitude = '-118.104138' WHERE City = 'Renata' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.042169', Longitude = '-117.1476449' WHERE City = 'Retallack' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8121465', Longitude = '-124.5241712' WHERE City = 'Retaskit' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.998115', Longitude = '-118.195672' WHERE City = 'Revelstoke' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.227431', Longitude = '-119.019141' WHERE City = 'Rhone' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.915703', Longitude = '-122.4563269' WHERE City = 'Rich Bar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1665898', Longitude = '-123.133569' WHERE City = 'Richmond' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.144797', Longitude = '-122.849772' WHERE City = 'Rider' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.125513', Longitude = '-122.238633' WHERE City = 'Ridgedale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.2245579', Longitude = '-130.3250453' WHERE City = 'Ridley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.240486', Longitude = '-123.101472' WHERE City = 'Riley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.763677', Longitude = '-116.856438' WHERE City = 'Riondel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.968415', Longitude = '-122.529743' WHERE City = 'Riske Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1396888', Longitude = '-122.0581203' WHERE City = 'Ritchie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.420459', Longitude = '-124.044797' WHERE City = 'River Jordan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2896309', Longitude = '-122.7671465' WHERE City = 'River Springs' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.683333', Longitude = '-127.2500001' WHERE City = 'Rivers Inlet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.121827', Longitude = '-122.300132' WHERE City = 'Riverside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.125114', Longitude = '-117.738265' WHERE City = 'Rivervale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4301881', Longitude = '-123.6495116' WHERE City = 'Roberts Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.6630023', Longitude = '-126.869183' WHERE City = 'Robin Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3327426', Longitude = '-117.689958' WHERE City = 'Robson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3416156', Longitude = '-117.7230586' WHERE City = 'Robson West' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.324603', Longitude = '-125.4673309' WHERE City = 'Rock Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0559264', Longitude = '-118.9976834' WHERE City = 'Rock Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.500793', Longitude = '-115.795038' WHERE City = 'Rockyview' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.5147509', Longitude = '-120.83353' WHERE City = 'Roe Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.483333', Longitude = '-117.4833331' WHERE City = 'Rogers' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.299982', Longitude = '-117.518662' WHERE City = 'Rogers Pass' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.897988', Longitude = '-120.1404949' WHERE City = 'Rolla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.00072', Longitude = '-115.0554019' WHERE City = 'Roosville' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.14638', Longitude = '-131.0765649' WHERE City = 'Rose Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.4', Longitude = '-126.033333' WHERE City = 'Rose Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.5066064', Longitude = '-120.8175452' WHERE City = 'Rose Prairie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.033314', Longitude = '-117.413531' WHERE City = 'Rosebery' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1783398', Longitude = '-121.8710926' WHERE City = 'Rosedale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.245', Longitude = '-117.56' WHERE City = 'Ross Peak' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.18213', Longitude = '-117.462639' WHERE City = 'Ross Spur' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0781415', Longitude = '-117.8000037' WHERE City = 'Rossland' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.804917', Longitude = '-128.763918' WHERE City = 'Rosswood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.666667', Longitude = '-126.9166671' WHERE City = 'Round Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.06771', Longitude = '-114.922772' WHERE City = 'Round Prairie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.516667', Longitude = '-125.533333' WHERE City = 'Roy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6458193', Longitude = '-124.9464183' WHERE City = 'Royston' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.35', Longitude = '-121.6' WHERE City = 'Ruby Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.429326', Longitude = '-127.485624' WHERE City = 'Rumble Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.3150367', Longitude = '-130.3208187' WHERE City = 'Rupert' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5064562', Longitude = '-116.0291433' WHERE City = 'Rushmere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8900242', Longitude = '-119.3954476' WHERE City = 'Rutland' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.157589', Longitude = '-116.0094989' WHERE City = 'Ryan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.107442', Longitude = '-121.8939634' WHERE City = 'Ryder Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.002415', Longitude = '-116.500006' WHERE City = 'Rykerts' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4592626', Longitude = '-123.3767223' WHERE City = 'Saanich' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.5962689', Longitude = '-123.4169023' WHERE City = 'Saanichton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6305481', Longitude = '-121.3954433' WHERE City = 'Saddle Rock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.648178', Longitude = '-120.340229' WHERE City = 'Sahali' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.252987', Longitude = '-124.788007' WHERE City = 'Sahara Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.776335', Longitude = '-123.807141' WHERE City = 'Sahtlam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6169282', Longitude = '-124.0335296' WHERE City = 'Sallahlus Sechelt Band 20' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6198332', Longitude = '-124.0283244' WHERE City = 'Sallahlus Sechelt Band 20A' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.194892', Longitude = '-117.2792769' WHERE City = 'Salmo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7001034', Longitude = '-119.2838443' WHERE City = 'Salmon Arm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.099955', Longitude = '-122.658538' WHERE City = 'Salmon Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.133465', Longitude = '-131.354463' WHERE City = 'Saloon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8166622', Longitude = '-123.5088755' WHERE City = 'Salt Spring Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.9452039', Longitude = '-123.7585239' WHERE City = 'Saltair' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.783982', Longitude = '-124.179438' WHERE City = 'Saltery Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Salvus' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8232677', Longitude = '-123.2159073' WHERE City = 'Samuel Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.682523', Longitude = '-128.26685' WHERE City = 'San Josef' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.390147', Longitude = '-116.7354399' WHERE City = 'Sanca' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2134839', Longitude = '-122.8709714' WHERE City = 'Sanderson Site' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.975503', Longitude = '-117.226963' WHERE City = 'Sandon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.243025', Longitude = '-131.820879' WHERE City = 'Sandspit' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.701352', Longitude = '-124.982798' WHERE City = 'Sandwick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0298805', Longitude = '-119.45037' WHERE City = 'Sandy Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8653139', Longitude = '-125.1154266' WHERE City = 'Saratoga Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.131222', Longitude = '-121.960432' WHERE City = 'Sardis' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.883333', Longitude = '-125.033333' WHERE City = 'Sarita' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.391134', Longitude = '-123.667165' WHERE City = 'Saseenos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7820442', Longitude = '-123.1653007' WHERE City = 'Saturna Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6532647', Longitude = '-124.067979' WHERE City = 'Saughanaught Sechelt Band 22' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9396805', Longitude = '-124.8094136' WHERE City = 'Savary Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7527649', Longitude = '-120.843769' WHERE City = 'Savona' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.1', Longitude = '-125.166667' WHERE City = 'Savory' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6305604', Longitude = '-124.0208532' WHERE City = 'Sawquamain Sechelt Band 19A' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.383718', Longitude = '-125.960212' WHERE City = 'Sayward' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.903483', Longitude = '-119.45846' WHERE City = 'Scotch Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.600074', Longitude = '-133.825184' WHERE City = 'Scotia Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.766667', Longitude = '-126.458333' WHERE City = 'Scott Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9285008', Longitude = '-119.3263514' WHERE City = 'Scotty Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.083333', Longitude = '-124.9' WHERE City = 'Seaford' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4420444', Longitude = '-122.5555318' WHERE City = 'Seal Bay Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.525524', Longitude = '-123.481016' WHERE City = 'Seaside Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4741736', Longitude = '-123.7545601' WHERE City = 'Sechelt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4740943', Longitude = '-123.7545805' WHERE City = 'Sechelt Indian Government District' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4830488', Longitude = '-123.7425229' WHERE City = 'Sechelt Sechelt Band 2' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.533333', Longitude = '-123.95' WHERE City = 'Secret Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.9754598', Longitude = '-125.0550447' WHERE City = 'Seddall' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6502411', Longitude = '-123.399005' WHERE City = 'Seeney' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4741736', Longitude = '-123.7545601' WHERE City = 'Sekaleton Sechelt Band 21' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6299351', Longitude = '-124.050691' WHERE City = 'Sekaleton Sechelt Band 21A' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.460763', Longitude = '-123.737597' WHERE City = 'Selma Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2714129', Longitude = '-123.0637922' WHERE City = 'Semlin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4805555', Longitude = '-125.5136111' WHERE City = 'Septimus' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.707377', Longitude = '-122.2895451' WHERE City = 'Seton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.707377', Longitude = '-122.2895451' WHERE City = 'Seton Portage' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.897991', Longitude = '-120.322945' WHERE City = 'Seven Mile Corner' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.303136', Longitude = '-121.3957691' WHERE City = 'Seventy Mile House' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.766667', Longitude = '-132.3' WHERE City = 'Sewall' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.874187', Longitude = '-132.002019' WHERE City = 'Sewell Inlet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.237814', Longitude = '-118.9456681' WHERE City = 'Seymour Arm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.0690979', Longitude = '-127.0054354' WHERE City = 'Seymour Inlet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.750825', Longitude = '-127.167965' WHERE City = 'Seymour Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.986319', Longitude = '-122.724405' WHERE City = 'Shady Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.732205', Longitude = '-122.232405' WHERE City = 'Shalalth' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.484131', Longitude = '-128.9528149' WHERE City = 'Shames' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6699686', Longitude = '-123.15646' WHERE City = 'Shannon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.644557', Longitude = '-132.514086' WHERE City = 'Shannon Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6726389', Longitude = '-123.16102' WHERE City = 'Shannon Creek Sechelt Band 28' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8487826', Longitude = '-126.5589249' WHERE City = 'Shawl Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6511793', Longitude = '-123.6449962' WHERE City = 'Shawnigan Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.066667', Longitude = '-120.0833331' WHERE City = 'Shearer Dale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.148478', Longitude = '-128.0916' WHERE City = 'Shearwater' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.000784', Longitude = '-122.620081' WHERE City = 'Shelley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.635541', Longitude = '-117.9289101' WHERE City = 'Shelter Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9306849', Longitude = '-125.184946' WHERE City = 'Shelter Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.166667', Longitude = '-125.4666671' WHERE City = 'Sheraton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.03372', Longitude = '-119.584316' WHERE City = 'Shere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.532082', Longitude = '-120.882871' WHERE City = 'Sheridan Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.260261', Longitude = '-131.7927701' WHERE City = 'Sheslay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.516667', Longitude = '-119.8' WHERE City = 'Shingle Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.389275', Longitude = '-123.905092' WHERE City = 'Shirley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.425718', Longitude = '-117.530042' WHERE City = 'Shoreacres' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.302625', Longitude = '-117.851722' WHERE City = 'Shoreholme' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.156549', Longitude = '-120.8797919' WHERE City = 'Shulus' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.854135', Longitude = '-127.855266' WHERE City = 'Shushartie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.293683', Longitude = '-118.8153141' WHERE City = 'Shuswap Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9579499', Longitude = '-116.905241' WHERE City = 'Shutty Bench' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8378243', Longitude = '-118.9768156' WHERE City = 'Sicamous' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.029', Longitude = '-119.2544946' WHERE City = 'Sidley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6502411', Longitude = '-123.399005' WHERE City = 'Sidney' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.233333', Longitude = '-122.7000001' WHERE City = 'Sikanni' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.233333', Longitude = '-122.7000001' WHERE City = 'Sikanni Chief' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.033554', Longitude = '-117.847372' WHERE City = 'Silica' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3141173', Longitude = '-121.4121168' WHERE City = 'Silver Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.575187', Longitude = '-121.8199031' WHERE City = 'Silver River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1459297', Longitude = '-122.4049093' WHERE City = 'Silverdale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.176271', Longitude = '-122.3792279' WHERE City = 'Silverhill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9528617', Longitude = '-117.3572961' WHERE City = 'Silverton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.592609', Longitude = '-122.4324509' WHERE City = 'Simpson Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.016026', Longitude = '-121.670427' WHERE City = 'Sinclair Mills' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.95', Longitude = '-123.866667' WHERE City = 'Sinkut River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.236053', Longitude = '-116.611971' WHERE City = 'Sirdar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.766714', Longitude = '-119.007147' WHERE City = 'Six Mile Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.966667', Longitude = '-131.616667' WHERE City = 'Skedans' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.24978', Longitude = '-129.8326649' WHERE City = 'Skeena' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.098611', Longitude = '-127.811111' WHERE City = 'Skeena Crossing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.2665', Longitude = '-131.9913154' WHERE City = 'Skidegate' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.247222', Longitude = '-132.0083331' WHERE City = 'Skidegate Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.616667', Longitude = '-127.1166669' WHERE City = 'Skooks Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7288631', Longitude = '-123.8991704' WHERE City = 'Skookumchuck Sechelt Band 27' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Skoonka' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4741736', Longitude = '-123.7545601' WHERE City = 'Skwawkweehm Sechelt Band 17' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1717205', Longitude = '-122.0843285' WHERE City = 'Skweahm 10' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.9961818', Longitude = '-124.0139642' WHERE City = 'Slayathlum Sechelt Band 16' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.076291', Longitude = '-121.830289' WHERE City = 'Slesse Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.898488', Longitude = '-124.603327' WHERE City = 'Sliammon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.762993', Longitude = '-117.46992' WHERE City = 'Slocan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.51397', Longitude = '-117.618387' WHERE City = 'Slocan Park' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6425704', Longitude = '-123.9843683' WHERE City = 'Smeshalin Sechelt Band 18' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.882287', Longitude = '-126.432997' WHERE City = 'Smith River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.782355', Longitude = '-127.1685541' WHERE City = 'Smithers' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.050009', Longitude = '-126.514467' WHERE City = 'Smithers Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.0398766', Longitude = '-122.4570401' WHERE City = 'Snake 5' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.033333', Longitude = '-122.45' WHERE City = 'Snake River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Snyder' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4170036', Longitude = '-123.4831741' WHERE City = 'Soames Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1239663', Longitude = '-123.1847379' WHERE City = 'Sockeye' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.35', Longitude = '-122.2833331' WHERE City = 'Soda Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.6308641', Longitude = '-127.017264' WHERE City = 'Sointula' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2646392', Longitude = '-123.1126133' WHERE City = 'Solly' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.858677', Longitude = '-118.953572' WHERE City = 'Solsqua' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.81418', Longitude = '-123.736086' WHERE City = 'Somenos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3596385', Longitude = '-125.2319104' WHERE City = 'Sonora Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.3740346', Longitude = '-123.7355539' WHERE City = 'Sooke' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.87686', Longitude = '-119.469938' WHERE City = 'Sorrento' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.045342', Longitude = '-126.668081' WHERE City = 'South Bentinck' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.699858', Longitude = '-119.215827' WHERE City = 'South Canoe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.735453', Longitude = '-120.349382' WHERE City = 'South Dawson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.895532', Longitude = '-122.7413469' WHERE City = 'South Fort George' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.233333', Longitude = '-127.6666669' WHERE City = 'South Hazelton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.11067', Longitude = '-122.088061' WHERE City = 'South Lakeside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.11067', Longitude = '-122.088061' WHERE City = 'South Lakeside (Williams Lake)' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.7499457', Longitude = '-123.2110296' WHERE City = 'South Pender Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.016894', Longitude = '-122.3151851' WHERE City = 'South Poplar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.728281', Longitude = '-122.244441' WHERE City = 'South Shalalth' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4576298', Longitude = '-117.5237637' WHERE City = 'South Slocan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2144754', Longitude = '-123.0023565' WHERE City = 'South Slope' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.118352', Longitude = '-121.992072' WHERE City = 'South Sumas' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.147131', Longitude = '-120.6761561' WHERE City = 'South Taylor' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4984943', Longitude = '-123.9794558' WHERE City = 'South Thormanby Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.105313', Longitude = '-123.894489' WHERE City = 'South Wellington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.016667', Longitude = '-125.766667' WHERE City = 'Southbank' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Soyandostar 2' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4279241', Longitude = '-119.2469274' WHERE City = 'Spallumcheen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7344685', Longitude = '-114.8796552' WHERE City = 'Sparwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.55', Longitude = '-121.3' WHERE City = 'Spatsum' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.414053', Longitude = '-121.359529' WHERE City = 'Spences Bridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.852485', Longitude = '-119.4658249' WHERE City = 'Sperling' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Spicer' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.905829', Longitude = '-116.365237' WHERE City = 'Spillamacheen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.2735658', Longitude = '-122.249958' WHERE City = 'Springfield Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.962193', Longitude = '-122.142324' WHERE City = 'Springhouse' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2667081', Longitude = '-125.0368939' WHERE City = 'Sproat Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.08657', Longitude = '-123.037183' WHERE City = 'Sproatt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5055987', Longitude = '-117.4001141' WHERE City = 'Sproule Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.760868', Longitude = '-116.20249' WHERE City = 'Spur Valley Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.675172', Longitude = '-121.418656' WHERE City = 'Spuzzum' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7016339', Longitude = '-123.1558121' WHERE City = 'Squamish' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.244679', Longitude = '-122.0011084' WHERE City = 'Squawkum Creek 3' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.501916', Longitude = '-121.41981' WHERE City = 'Squeah' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.871401', Longitude = '-119.6055429' WHERE City = 'Squilax' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.116667', Longitude = '-124.916667' WHERE City = 'Squirrel Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2812948', Longitude = '-123.1267862' WHERE City = 'St. Andrews' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5875978', Longitude = '-115.7559557' WHERE City = 'St. Eugene Mission' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9789249', Longitude = '-119.103617' WHERE City = 'St. Ives' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.139802', Longitude = '-122.3229408' WHERE City = 'St. Joseph Mission' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8889648', Longitude = '-123.5429622' WHERE City = 'St. Mary Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.151369', Longitude = '-111.278133' WHERE City = 'St. Vincent Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.033333', Longitude = '-121.716667' WHERE City = 'Stanley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3597927', Longitude = '-119.0588591' WHERE City = 'Star Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2572656', Longitude = '-123.139092' WHERE City = 'Starks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.229304', Longitude = '-122.357305' WHERE City = 'Stave Falls' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.683333', Longitude = '-123.7166671' WHERE City = 'Steamboat' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.219355', Longitude = '-122.315097' WHERE City = 'Steelhead' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.046676', Longitude = '-124.897698' WHERE City = 'Stellako' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.45', Longitude = '-116.2999999' WHERE City = 'Stephen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2138652', Longitude = '-119.5415094' WHERE City = 'Stepping Stones Estates' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.421371', Longitude = '-126.315341' WHERE City = 'Stewardson Inlet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.9413124', Longitude = '-129.9878921' WHERE City = 'Stewart' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.695618', Longitude = '-131.8074291' WHERE City = 'Stikine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2446331', Longitude = '-122.9347508' WHERE City = 'Still Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Stockett' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.630507', Longitude = '-122.663428' WHERE City = 'Stoner' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.4395202', Longitude = '-124.2505059' WHERE City = 'Stones Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.943958', Longitude = '-124.116742' WHERE City = 'Stoney Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.919444', Longitude = '-125.186111' WHERE City = 'Stories Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.274059', Longitude = '-123.069704' WHERE City = 'Stout' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.079808', Longitude = '-122.20559' WHERE City = 'Straiton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8922192', Longitude = '-125.6528069' WHERE City = 'Strathcona Lodge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.283512', Longitude = '-122.494913' WHERE City = 'Strathnaver' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.833333', Longitude = '-126.25' WHERE City = 'Streatham' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3929727', Longitude = '-125.1186076' WHERE City = 'Stuart Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.2024151', Longitude = '-123.8758206' WHERE City = 'Stuart River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.369857', Longitude = '-126.065858' WHERE City = 'Stuie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3931479', Longitude = '-120.327697' WHERE City = 'Stump Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8788025', Longitude = '-123.3177664' WHERE City = 'Sturdies Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6322203', Longitude = '-124.0246544' WHERE City = 'Suahbin Sechelt Band 19' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.1', Longitude = '-121.983333' WHERE City = 'Sugarcane' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.883333', Longitude = '-126.816667' WHERE City = 'Sullivan Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6073297', Longitude = '-119.6768815' WHERE City = 'Summerland' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.877459', Longitude = '-119.910142' WHERE City = 'Sun Peaks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.089348', Longitude = '-123.041458' WHERE City = 'Sundance' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.090996', Longitude = '-122.9951899' WHERE City = 'Sundance Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.116128', Longitude = '-117.718234' WHERE City = 'Sunningdale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.773318', Longitude = '-119.2716811' WHERE City = 'Sunnybrae' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.654156', Longitude = '-124.7070759' WHERE City = 'Sunnyside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.866667', Longitude = '-120.666667' WHERE City = 'Sunrise Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.398404', Longitude = '-123.248231' WHERE City = 'Sunset Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.853457', Longitude = '-120.7668681' WHERE City = 'Sunset Prairie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6087', Longitude = '-117.005232' WHERE City = 'Sunshine Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.274439', Longitude = '-121.2318899' WHERE City = 'Sunshine Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.633333', Longitude = '-127.25' WHERE City = 'Suquash' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.235329', Longitude = '-125.0982801' WHERE City = 'Surge Narrows' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.625932', Longitude = '-133.4168469' WHERE City = 'Surprise' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1913466', Longitude = '-122.8490125' WHERE City = 'Surrey' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5083011', Longitude = '-115.9528264' WHERE City = 'Swansea' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6879967', Longitude = '-123.4146546' WHERE City = 'Swartz Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4911403', Longitude = '-123.7668847' WHERE City = 'Swaycalse Sechelt Band 3' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4741736', Longitude = '-123.7545601' WHERE City = 'Swaywelat Sechelt Band 12' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3428609', Longitude = '-123.1149244' WHERE City = 'Sweetsbridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.912564', Longitude = '-120.4405461' WHERE City = 'Sweetwater' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4296485', Longitude = '-123.3710216' WHERE City = 'Swift' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1540577', Longitude = '-121.9507004' WHERE City = 'Swiftwater' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.6003054', Longitude = '-123.290831' WHERE City = 'Sydney Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.783029', Longitude = '-115.785787' WHERE City = 'Ta Ta Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0468028', Longitude = '-122.3389941' WHERE City = 'Tabor' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.7002772', Longitude = '-122.5435981' WHERE City = 'Tacheeda' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.656916', Longitude = '-124.7417319' WHERE City = 'Tachie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.109519', Longitude = '-117.719289' WHERE City = 'Tadanac' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.996052', Longitude = '-118.640602' WHERE City = 'Taft' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.491006', Longitude = '-117.3969301' WHERE City = 'Taghum' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.003239', Longitude = '-130.997305' WHERE City = 'Tahltan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.927861', Longitude = '-126.656541' WHERE City = 'Tahsis' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.4828875', Longitude = '-125.9677772' WHERE City = 'Takla Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.633333', Longitude = '-133.8500001' WHERE City = 'Taku' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.898399', Longitude = '-125.8713559' WHERE City = 'Takysie Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.388688', Longitude = '-126.836364' WHERE City = 'Tallheo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2597298', Longitude = '-123.2463444' WHERE City = 'Tamarack' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.091405', Longitude = '-123.007409' WHERE City = 'Tamarisk' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8102827', Longitude = '-123.1981339' WHERE City = 'Tantalus Acres' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.025879', Longitude = '-131.781177' WHERE City = 'Tanu' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.782998', Longitude = '-119.334404' WHERE City = 'Tappen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.377724', Longitude = '-117.557987' WHERE City = 'Tarrys' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.75922', Longitude = '-132.0383' WHERE City = 'Tasu' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.983333', Longitude = '-125.983333' WHERE City = 'Tatalrose' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.905156', Longitude = '-124.597755' WHERE City = 'Tatla Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.7', Longitude = '-127.1166671' WHERE City = 'Tatlow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.734175', Longitude = '-129.982887' WHERE City = 'Tatogga' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.716667', Longitude = '-121.366667' WHERE City = 'Tatton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0903434', Longitude = '-123.0835052' WHERE City = 'Taverna' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.1525689', Longitude = '-120.6814069' WHERE City = 'Taylor' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7245055', Longitude = '-123.8655399' WHERE City = 'Tchahchelailthtenum Sechelt Band 10' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.083333', Longitude = '-125.733333' WHERE City = 'Tchesinkut Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.185961', Longitude = '-124.816185' WHERE City = 'Teakerne Arm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.862313', Longitude = '-123.1795009' WHERE City = 'Telachick' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5492459', Longitude = '-126.832251' WHERE City = 'Telegraph Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.90293', Longitude = '-131.159156' WHERE City = 'Telegraph Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.695016', Longitude = '-127.04761' WHERE City = 'Telkwa' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.083168', Longitude = '-122.432854' WHERE City = 'Ten Mile Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.5181925', Longitude = '-128.6031539' WHERE City = 'Terrace' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.965885', Longitude = '-119.43007' WHERE City = 'Tete Jaune' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.965885', Longitude = '-119.43007' WHERE City = 'Tete Jaune Cache' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6596634', Longitude = '-124.4121947' WHERE City = 'Texada Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0601569', Longitude = '-124.6927371' WHERE City = 'Theodosia Arm' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.9986363', Longitude = '-123.6812002' WHERE City = 'Thetis Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5136032', Longitude = '-121.2775127' WHERE City = 'Thompson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5136032', Longitude = '-121.2775127' WHERE City = 'Thompson River Estates' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.802874', Longitude = '-126.012082' WHERE City = 'Thompson Sound' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.513787', Longitude = '-128.5401111' WHERE City = 'Thornhill' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.013059', Longitude = '-117.28516' WHERE City = 'Three Forks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9333408', Longitude = '-118.445714' WHERE City = 'Three Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.354177', Longitude = '-117.57777' WHERE City = 'Thrums' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.232482', Longitude = '-119.2168049' WHERE City = 'Thunder River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1318603', Longitude = '-122.5441266' WHERE City = 'Thunderbird' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.45', Longitude = '-125.3666669' WHERE City = 'Thurlow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.833333', Longitude = '-131.75' WHERE City = 'Thurston Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Tibbets' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9087249', Longitude = '-119.084746' WHERE City = 'Tiilis Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.447719', Longitude = '-123.400378' WHERE City = 'Tillicum' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.2', Longitude = '-125.083333' WHERE City = 'Tintagel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.736036', Longitude = '-122.147218' WHERE City = 'Tipella' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.2406986', Longitude = '-121.0183303' WHERE City = 'Tisdall' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.5629769', Longitude = '-131.9289541' WHERE City = 'Tlell' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.847789', Longitude = '-125.2317241' WHERE City = 'Toad River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.339865', Longitude = '-116.424919' WHERE City = 'Toby Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Tochty' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1529842', Longitude = '-125.9066184' WHERE City = 'Tofino' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Toketic' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.555446', Longitude = '-120.076197' WHERE City = 'Tomslake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.505051', Longitude = '-126.296656' WHERE City = 'Topley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2325279', Longitude = '-122.82292' WHERE City = 'Torrent' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.2611913', Longitude = '-125.0897914' WHERE City = 'Towdystan' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.016667', Longitude = '-120.5666671' WHERE City = 'Tower Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.284756', Longitude = '-123.1121501' WHERE City = 'Townsend' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Tracard' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.941208', Longitude = '-119.504483' WHERE City = 'Traders Cove' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4083191', Longitude = '-123.319692' WHERE City = 'Trafalgar' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0965676', Longitude = '-117.71173' WHERE City = 'Trail' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.725166', Longitude = '-120.507131' WHERE City = 'Tranquille' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2690026', Longitude = '-123.1389606' WHERE City = 'Tremblay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.8567773', Longitude = '-125.0982735' WHERE City = 'Trembleur' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.788837', Longitude = '-119.708012' WHERE City = 'Trepanier' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Trevor Channel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4', Longitude = '-118.916667' WHERE City = 'Trinity Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.571837', Longitude = '-119.630862' WHERE City = 'Trout Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.651292', Longitude = '-117.540371' WHERE City = 'Trout Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.733333', Longitude = '-122.95' WHERE City = 'Trutch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4384141', Longitude = '-123.7160038' WHERE City = 'Tsawcome Sechelt Band 1' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.004489', Longitude = '-123.089368' WHERE City = 'Tsawwassen Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.906111', Longitude = '-124.9649999' WHERE City = 'Tsay Keh Dene' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.3666667', Longitude = '-130.25' WHERE City = 'Tsimpsean 2 North Part' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6958142', Longitude = '-124.9942722' WHERE City = 'Tsolum River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0959594', Longitude = '-123.7416786' WHERE City = 'Tsooahdie Sechelt Band 15' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.54586', Longitude = '-120.76024' WHERE City = 'Tulameen' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '58.645396', Longitude = '-133.5401029' WHERE City = 'Tulsequah' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.125795', Longitude = '-120.993154' WHERE City = 'Tumbler Ridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.512426', Longitude = '-120.037882' WHERE City = 'Tupper' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Turner Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.806971', Longitude = '-119.59754' WHERE City = 'Turtle Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.55', Longitude = '-123.75' WHERE City = 'Tuwanek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.616667', Longitude = '-121.566667' WHERE City = 'Twidwell Bend' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3402409', Longitude = '-116.716888' WHERE City = 'Twin Bays' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.0077777', Longitude = '-117.9783332' WHERE City = 'Twin Butte' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4794259', Longitude = '-123.4853411' WHERE City = 'Twin Creeks' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0261235', Longitude = '-124.9317342' WHERE City = 'Twin Islands' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.266667', Longitude = '-127.6166669' WHERE City = 'Two Mile' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.2568166', Longitude = '-127.6720019' WHERE City = 'Two Mile Hazelton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.183248', Longitude = '-120.531269' WHERE City = 'Two Rivers' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.326481', Longitude = '-116.792372' WHERE City = 'Tye' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2534128', Longitude = '-123.0740134' WHERE City = 'Tyee' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.779446', Longitude = '-123.649468' WHERE City = 'Tzouhalem' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.779446', Longitude = '-123.649468' WHERE City = 'Tzuhalem' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.9415997', Longitude = '-125.5463446' WHERE City = 'Ucluelet' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53', Longitude = '-125.7' WHERE City = 'Ulkatcho' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5809036', Longitude = '-124.8844979' WHERE City = 'Union Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5003501', Longitude = '-119.5672309' WHERE City = 'Upper Bench' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.216667', Longitude = '-117.7' WHERE City = 'Upper China Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.520189', Longitude = '-120.4395519' WHERE City = 'Upper Cutbank' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.111782', Longitude = '-121.9321979' WHERE City = 'Upper Fraser' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.512217', Longitude = '-121.9654869' WHERE City = 'Upper Halfway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.067743', Longitude = '-122.19357' WHERE City = 'Upper Sumas 6' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.674469', Longitude = '-120.8703' WHERE City = 'Urling' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.6763888', Longitude = '-125.8969444' WHERE City = 'Urquhart' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.636167', Longitude = '-128.4117189' WHERE City = 'Usk' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0730059', Longitude = '-123.6504918' WHERE City = 'Valdes Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.83122', Longitude = '-119.264311' WHERE City = 'Valemount' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8', Longitude = '-117.6666667' WHERE City = 'Valhalla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.985259', Longitude = '-120.2448711' WHERE City = 'Valley View' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.70178', Longitude = '-123.12567' WHERE City = 'Valleycliffe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.985259', Longitude = '-120.2448711' WHERE City = 'Valleyview' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.559334', Longitude = '-117.6507209' WHERE City = 'Vallican' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.756122', Longitude = '-124.554059' WHERE City = 'Van Anda' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2827291', Longitude = '-123.1207375' WHERE City = 'Vancouver' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.0139798', Longitude = '-124.0129801' WHERE City = 'Vanderhoof' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.857956', Longitude = '-122.816032' WHERE City = 'Vanway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1911218', Longitude = '-125.9892639' WHERE City = 'Vargas Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2898482', Longitude = '-119.5304443' WHERE City = 'Vaseaux Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5070643', Longitude = '-123.9993615' WHERE City = 'Vaucroft Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.578841', Longitude = '-119.7179271' WHERE City = 'Vavenby' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1038704', Longitude = '-121.9625821' WHERE City = 'Vedder Crossing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.024858', Longitude = '-115.979586' WHERE City = 'Vermilion Crossing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.2670137', Longitude = '-119.2720107' WHERE City = 'Vernon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.883629', Longitude = '-123.56922' WHERE City = 'Vesuvius' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4284207', Longitude = '-123.3656444' WHERE City = 'Victoria' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.3668131', Longitude = '-127.3783179' WHERE City = 'Victoria Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.1666669', Longitude = '-120.9' WHERE City = 'Vidette' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.4527663', Longitude = '-123.4348032' WHERE City = 'View Royal' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.919378', Longitude = '-120.2353211' WHERE City = 'Vinsulla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Von Zuben' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0160833', Longitude = '-122.2745844' WHERE City = 'Vye' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3047448', Longitude = '-124.4235919' WHERE City = 'Wabi' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Wabron' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.516667', Longitude = '-127.5' WHERE City = 'Wadhams' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.1605419', Longitude = '-128.1455793' WHERE City = 'Waglisla' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.514952', Longitude = '-122.239737' WHERE City = 'Wagner Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Wakely' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.51746', Longitude = '-126.850332' WHERE City = 'Walcott' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.754262', Longitude = '-120.989949' WHERE City = 'Walhachin' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3276553', Longitude = '-123.1557987' WHERE City = 'Walker' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.708371', Longitude = '-116.864327' WHERE City = 'Walkers' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3009249', Longitude = '-124.222053' WHERE City = 'Wall Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.003918', Longitude = '-117.6119155' WHERE City = 'Waneta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Wanklyn' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.417837', Longitude = '-115.421878' WHERE City = 'Wardner' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.4230262', Longitude = '-125.6270035' WHERE City = 'Ware' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.095465', Longitude = '-117.7534689' WHERE City = 'Warfield' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.047179', Longitude = '-127.100043' WHERE City = 'Warner Bay' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.7684629', Longitude = '-115.737462' WHERE City = 'Wasa' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.780209', Longitude = '-115.7357135' WHERE City = 'Wasa Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.263641', Longitude = '-123.109646' WHERE City = 'Watson' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.9324723', Longitude = '-126.8181841' WHERE City = 'Watson Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.220456', Longitude = '-122.5125589' WHERE City = 'Websters Corners' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.0476357', Longitude = '-128.6538837' WHERE City = 'Wedeene' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1330555', Longitude = '-122.7933333' WHERE City = 'Wedge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2823496', Longitude = '-123.1227821' WHERE City = 'Wedgwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.683333', Longitude = '-128.783333' WHERE City = 'Weewanie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4884513', Longitude = '-123.9045196' WHERE City = 'Welcome Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2141515', Longitude = '-124.043906' WHERE City = 'Wellington' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.0859739', Longitude = '-121.6244178' WHERE City = 'Wells' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.966667', Longitude = '-123.983333' WHERE City = 'Weneez' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4961709', Longitude = '-119.629931' WHERE City = 'West Bench' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.5013242', Longitude = '-115.0749768' WHERE City = 'West Fernie' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.140187', Longitude = '-122.338372' WHERE City = 'West Heights' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.465312', Longitude = '-126.00265' WHERE City = 'West Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7840144', Longitude = '-119.0105146' WHERE City = 'West Mara Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0118991', Longitude = '-118.8214656' WHERE City = 'West Midway' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4741736', Longitude = '-123.7545601' WHERE City = 'West Sechelt' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4121882', Longitude = '-125.5886169' WHERE City = 'West Thurlow Island' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.092297', Longitude = '-117.708329' WHERE City = 'West Trail' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3286251', Longitude = '-123.1601982' WHERE City = 'West Vancouver' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.8313652', Longitude = '-119.6281931' WHERE City = 'Westbank' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.170418', Longitude = '-118.975194' WHERE City = 'Westbridge' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.866749', Longitude = '-123.7005989' WHERE City = 'Westholme' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2032286', Longitude = '-122.9066971' WHERE City = 'Westley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Westply' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.863612', Longitude = '-119.5644584' WHERE City = 'Westside' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.7575138', Longitude = '-120.3490077' WHERE City = 'Westsyde' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.836543', Longitude = '-124.5271939' WHERE City = 'Westview' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4723', Longitude = '-119.773993' WHERE City = 'Westwold' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.105079', Longitude = '-125.043559' WHERE City = 'Whaletown' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Whiplash Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.308333', Longitude = '-124.505556' WHERE City = 'Whisky Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.1163196', Longitude = '-122.9573563' WHERE City = 'Whistler' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.093591', Longitude = '-122.994939' WHERE City = 'Whistler Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.882026', Longitude = '-119.299486' WHERE City = 'White Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '59.6247222', Longitude = '-135.1380556' WHERE City = 'White Pass' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0253085', Longitude = '-122.802962' WHERE City = 'White Rock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.8622921', Longitude = '-119.9720284' WHERE City = 'Whitecroft' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.176467', Longitude = '-122.468939' WHERE City = 'Whonnock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.671362', Longitude = '-124.847778' WHERE City = 'Whyac' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.889594', Longitude = '-124.5522991' WHERE City = 'Wildwood' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3391671', Longitude = '-123.1411039' WHERE City = 'Wildwood Subdivision' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.515044', Longitude = '-126.336944' WHERE City = 'Wiley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.821091', Longitude = '-125.0509071' WHERE City = 'Williams Beach' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.1416736', Longitude = '-122.1416885' WHERE City = 'Williams Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.4395605', Longitude = '-122.5137142' WHERE City = 'Williams Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.455679', Longitude = '-123.474495' WHERE City = 'Williamsons Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.266492', Longitude = '-119.592269' WHERE City = 'Willow Brook' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Willow Flats' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.570313', Longitude = '-117.2362259' WHERE City = 'Willow Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.8173313', Longitude = '-119.2943464' WHERE City = 'Willow Ranch' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.075643', Longitude = '-122.474036' WHERE City = 'Willow River' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.85', Longitude = '-120.8666671' WHERE City = 'Willow Valley' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.033333', Longitude = '-124.45' WHERE City = 'Willowvale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.5382419', Longitude = '-116.0654809' WHERE City = 'Wilmer' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.44743', Longitude = '-123.711635' WHERE City = 'Wilson Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.983524', Longitude = '-119.4956879' WHERE City = 'Wilson Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.462525', Longitude = '-115.9886462' WHERE City = 'Windermere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.8676786', Longitude = '-118.1509206' WHERE City = 'Windy' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.0220455', Longitude = '-119.4052664' WHERE City = 'Winfield' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.05', Longitude = '-121.9666669' WHERE City = 'Wingdam' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.610414', Longitude = '-117.563463' WHERE City = 'Winlaw' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.52424', Longitude = '-128.028259' WHERE City = 'Winter Harbour' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.85', Longitude = '-126.266667' WHERE City = 'Wistaria' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.4133975', Longitude = '-117.0094569' WHERE City = 'Wolf' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4272222', Longitude = '-127.5663887' WHERE City = 'Wolfenden' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.8333333', Longitude = '-124.2666667' WHERE City = 'Wolverine' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '56.730259', Longitude = '-121.808989' WHERE City = 'Wonowon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '55.065845', Longitude = '-128.234018' WHERE City = 'Woodcock' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.667145', Longitude = '-123.251134' WHERE City = 'Woodfibre' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.679876', Longitude = '-126.9633431' WHERE City = 'Woodmere' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.523428', Longitude = '-122.642526' WHERE City = 'Woodpecker' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.070832', Longitude = '-119.056108' WHERE City = 'Woods Landing' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.049268', Longitude = '-119.389937' WHERE City = 'Woodsdale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.532677', Longitude = '-119.5675724' WHERE City = 'Worth' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.215414', Longitude = '-126.5893911' WHERE City = 'Woss' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.866667', Longitude = '-121.666667' WHERE City = 'Wright' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.598173', Longitude = '-115.867148' WHERE City = 'Wycliffe' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.3281718', Longitude = '-123.1557815' WHERE City = 'Wyman' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.1807329', Longitude = '-116.555034' WHERE City = 'Wynndel' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.022019', Longitude = '-125.196191' WHERE City = 'Yaculta' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0867938', Longitude = '-116.0883997' WHERE City = 'Yahk' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.1808', Longitude = '-133.0272764' WHERE City = 'Yaku' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.561062', Longitude = '-121.4291721' WHERE City = 'Yale' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.516667', Longitude = '-119.3666671' WHERE City = 'Yankee Flats' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.166766', Longitude = '-125.971069' WHERE City = 'Yarksis' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0821013', Longitude = '-122.0647472' WHERE City = 'Yarrow' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.6', Longitude = '-125.075' WHERE City = 'Yekooche' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.0724665', Longitude = '-123.8009079' WHERE City = 'Yellow Point' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.8621179', Longitude = '-123.2343249' WHERE City = 'Yellowhead' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.235076', Longitude = '-122.579395' WHERE City = 'Yennadon' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.284569', Longitude = '-117.218357' WHERE City = 'Ymir' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '51.4666667', Longitude = '-116.5833333' WHERE City = 'Yoho' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '48.8667023', Longitude = '-124.2000107' WHERE City = 'Youbou' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.4644772', Longitude = '-127.5613786' WHERE City = 'Yreka' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.591767', Longitude = '-126.618654' WHERE City = 'Yuquot' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.15378', Longitude = '-118.98176' WHERE City = 'Zamora' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.981803', Longitude = '-126.845404' WHERE City = 'Zeballos' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.7266683', Longitude = '-127.6476205' WHERE City = 'Zeke' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.038231', Longitude = '-117.2054319' WHERE City = 'Zincton' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.528477', Longitude = '-121.759864' WHERE City = 'Bear Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '54.492974', Longitude = '-122.68306' WHERE City = 'Bear Lake' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '53.283333', Longitude = '-123.1333331' WHERE City = 'Blackwater' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.453918', Longitude = '-116.762954' WHERE City = 'Boswell' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '52.4031805', Longitude = '-123.4553619' WHERE City = 'Cariboo' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '50.790253', Longitude = '-120.767329' WHERE City = 'Copper Creek' AND IsHidden = 0");
//            migrationBuilder.Sql(
//                "UPDATE LocationLookups Set Latitude = '51.581613', Longitude = '-122.237828' WHERE City = 'Dog Creek' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '57.837863', Longitude = '-131.386695' WHERE City = 'Glenora' AND IsHidden = 0");
            migrationBuilder.Sql(
                "UPDATE LocationLookups Set Latitude = '49.2190648', Longitude = '-122.6895165' WHERE City = 'Meadows' AND IsHidden = 0");

            // cache these values
            migrationBuilder.Sql(
                @"insert into GeocodedLocationCache([Name], Latitude, Longitude, DateGeocoded, IsPermanent)
                        select City + ', BC, CANADA', Latitude, Longitude, '2019-01-01 00:00:00.0000000', 1
                        from LocationLookups
                        where IsDuplicate = 0 and IsHidden = 0 and Latitude <> ''");

            // this isn't even in BC!!    (Mile 422 Alaska Highway)
            migrationBuilder.Sql("update LocationLookups set IsHidden = 1 where LocationId = 1290");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}