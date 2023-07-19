namespace CarDealer;

using AutoMapper;
using CarDealer.DTOs.Export;
using DTOs.Import;
using Models;
using System.Globalization;

public class CarDealerProfile : Profile
{

    public CarDealerProfile()
    {
        // sup
        this.CreateMap<ImportSupplierDto, Supplier>();
        this.CreateMap<Supplier, ExportLocalSupplierDto>();

        // part
        this.CreateMap<ImportPartDto, Part>()
            .ForMember(d => d.SupplierId,
            opt => opt.MapFrom(s => s.SupplierId.Value));
        this.CreateMap<Part, ExportPartOfCar>();

        // car
        this.CreateMap<ImportCarDto, Car>()
            .ForSourceMember(s => s.Parts,
                    opt => opt.DoNotValidate());
        this.CreateMap<Car, ExportCarWithParts>()
            .ForMember(d => d.Parts,
            opt => opt.MapFrom(s => s.PartsCars
                                    .Select(pc => pc.Part)
                                    .OrderByDescending(p => p.Price)
                                    .ToArray()));

        this.CreateMap<Car, ExportCarDto>();
        this.CreateMap<Car, ExportBmwCarDto>();
        this.CreateMap<Car, CarSaleDto>();

        // cus
        this.CreateMap<ImportCustomerDto, Customer>();
        //.ForMember(d => d.BirthDate,
        //        opt => opt.MapFrom(s => DateTime.Parse(s.BirthDate, CultureInfo.InvariantCulture)));
        this.CreateMap<Customer, ExportSalesByCustomer>();
            //.ForMember(d => d.BoughtCarsCount,
            //opt => opt.MapFrom(s => s.Sales.Count))
            //.ForMember(d => d.SpentMoney,
            //opt => 
            //opt.MapFrom(s =>
            //        s.Sales
            //        .SelectMany(s => s.Car.PartsCars)
            //        .Sum(pc => pc.Part.Price)));
        

        // sale
        this.CreateMap<ImportSaleDto, Sale>()
            .ForMember(d => d.CarId,
                        opt => opt.MapFrom(s => s.CarId.Value));
        this.CreateMap<Sale, ExportSalesWithDiscount>()
            .ForMember(d => d.Price,
            opt =>
            opt.MapFrom(s =>
                    s.Car.PartsCars
                    .Sum(s => s.Part.Price)
                    ))
            .ForMember(d => d.PriceWithDiscount,
            opt =>
            opt.MapFrom(s =>
                    Math.Round(s.Car.PartsCars
                    .Sum(s => s.Part.Price) * (1 - (s.Discount / 100m)),2)
                    ));
                    
    }
}
