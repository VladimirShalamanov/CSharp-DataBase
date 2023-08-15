namespace Artillery
{
    using Artillery.Data.Models;
    using Artillery.DataProcessor.ExportDto;
    using Artillery.DataProcessor.ImportDto;
    using AutoMapper;

    public class ArtilleryProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public ArtilleryProfile()
        {
            this.CreateMap<Gun, ExportGunDto>()
            .ForMember(d => d.ManufacturerName,
                        m => m.MapFrom(g => g.Manufacturer.ManufacturerName))
            .ForMember(d => d.GunType,
                        m => m.MapFrom(g => g.GunType.ToString()))
            .ForMember(d => d.Countries,
                        m => m.MapFrom(g => g.CountriesGuns
                                            .Where(c => c.Country.ArmySize > 4500000)
                                            .OrderBy(c => c.Country.ArmySize)
                                            .ToArray()
                                        ));


            this.CreateMap<CountryGun, ExportGunWithCountryDto>()
                .ForMember(d => d.CountryName,
                            m => m.MapFrom(g => g.Country.CountryName))
                .ForMember(d => d.ArmySize,
                            m => m.MapFrom(g => g.Country.ArmySize));
        }
    }
}