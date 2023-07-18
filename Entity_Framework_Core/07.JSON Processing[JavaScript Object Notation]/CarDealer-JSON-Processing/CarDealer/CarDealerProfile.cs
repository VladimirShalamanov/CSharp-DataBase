namespace CarDealer;

using AutoMapper;
using DTOs.Import;
using Models;

public class CarDealerProfile : Profile
{
    public CarDealerProfile()
    {
        this.CreateMap<ImpoortSupplierDto, Supplier>();

        this.CreateMap<ImportPartDto, Part>();

        this.CreateMap<ImportCarDto, Car>();

        this.CreateMap<ImportCustomerDto, Customer>();

        this.CreateMap<ImportSaleDto, Sale>();
    }
}
