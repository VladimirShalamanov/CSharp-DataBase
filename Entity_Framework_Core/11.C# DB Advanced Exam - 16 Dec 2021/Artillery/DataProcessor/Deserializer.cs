namespace Artillery.DataProcessor
{
    using Utilities;
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;

    using System.Text;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlHelper = new XmlHelper();

            var countriesDtos = xmlHelper.Deserialize<ImportCountryDto[]>(xmlString, "Countries");
            var countries = new HashSet<Country>();

            foreach (var cD in countriesDtos)
            {
                if (!IsValid(cD))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var c = new Country()
                {
                    CountryName = cD.CountryName,
                    ArmySize = cD.ArmySize
                };

                countries.Add(c);
                sb.AppendLine(string.Format(SuccessfulImportCountry, c.CountryName, c.ArmySize));
            }

            context.AddRange(countries);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlHelper = new XmlHelper();

            var manufacturersDtos = xmlHelper.Deserialize<ImportManufacturerDto[]>(xmlString, "Manufacturers");
            var manufacturers = new HashSet<Manufacturer>();

            foreach (var mD in manufacturersDtos)
            {
                if (!IsValid(mD) || manufacturers.Any(m => m.ManufacturerName == mD.ManufacturerName))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var m = new Manufacturer()
                {
                    ManufacturerName = mD.ManufacturerName,
                    Founded = mD.Founded
                };

                manufacturers.Add(m);

                var arr = m.Founded.Split(", ").ToArray();
                var cityCountry = arr.Skip(arr.Length - 2).ToArray();

                sb.AppendLine(string.Format(SuccessfulImportManufacturer, m.ManufacturerName, string.Join(", ", cityCountry)));
            }

            context.AddRange(manufacturers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlHelper = new XmlHelper();

            var shellsDtos = xmlHelper.Deserialize<ImportShellDto[]>(xmlString, "Shells");

            var shells = new HashSet<Shell>();

            foreach (var sD in shellsDtos)
            {
                if (!IsValid(sD))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var s = new Shell()
                {
                    ShellWeight = sD.ShellWeight,
                    Caliber = sD.Caliber,
                };

                shells.Add(s);
                sb.AppendLine(string.Format(SuccessfulImportShell, s.Caliber, s.ShellWeight));
            }

            context.AddRange(shells);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var validGT = new string[]
            { 
                "Howitzer", "Mortar",
                "FieldGun", "AntiAircraftGun",
                "MountainGun", "AntiTankGun"
            };

            var gunsDtos = JsonConvert.DeserializeObject<ImportGunDto[]>(jsonString);

            var guns = new HashSet<Gun>();

            foreach (var gD in gunsDtos!)
            {
                if (!IsValid(gD) || !validGT.Contains(gD.GunType))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var g = new Gun()
                {
                    ManufacturerId = gD.ManufacturerId,
                    GunWeight = gD.GunWeight,
                    BarrelLength = gD.BarrelLength,
                    NumberBuild = gD.NumberBuild,
                    Range = gD.Range,
                    GunType = (GunType)Enum.Parse(typeof(GunType), gD.GunType),
                    ShellId = gD.ShellId
                };

                foreach (var c in gD.Countries)
                {
                    g.CountriesGuns.Add(new CountryGun()
                    {
                        CountryId = c.Id,
                        Gun = g
                    });
                }

                guns.Add(g);
                sb.AppendLine(string.Format(SuccessfulImportGun, g.GunType, g.GunWeight, g.BarrelLength));
            }

            context.AddRange(guns);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}