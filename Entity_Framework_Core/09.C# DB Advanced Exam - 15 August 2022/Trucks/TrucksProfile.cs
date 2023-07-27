namespace Trucks
{
    using AutoMapper;
    using Trucks.Data.Models;
    using Trucks.DataProcessor.ExportDto;
    using Trucks.DataProcessor.ImportDto;

    public class TrucksProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
        public TrucksProfile()
        {
            
            this.CreateMap<Despatcher, ExportDespatcher>()
                .ForMember(dto => dto.TrucksCount,
                           m => m.MapFrom(d => d.Trucks.Count))
                .ForMember(dto => dto.Trucks,
                           m => m.MapFrom(d => d.Trucks
                                                .ToArray()
                                                .OrderBy(t => t.RegistrationNumber)
                                                .ToArray()));

            
            this.CreateMap<Truck, ExportDespatcherWithTruck>()
                .ForMember(dto => dto.Make,
                            m => m.MapFrom(t => t.MakeType.ToString()));
        }
    }
}
