namespace Footballers
{
    using AutoMapper;
    using Footballers.Data.Models;
    using Footballers.DataProcessor.ExportDto;

    // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
    public class FootballersProfile : Profile
    {
        public FootballersProfile()
        {
            this.CreateMap<Coach, ExportCoachDto>()
                .ForMember(d => d.FootballersCount,
                m => m.MapFrom(c => c.Footballers.Count))
                .ForMember(d => d.Footballers,
                m => m.MapFrom(c => c.Footballers
                                    .ToArray()
                                    .OrderBy(f => f.Name)
                                    .ToArray()
                                ));


            this.CreateMap<Footballer, ExportFootballerDto>()
                .ForMember(d => d.Position,
                m => m.MapFrom(f => f.PositionType.ToString()));
        }
    }
}
