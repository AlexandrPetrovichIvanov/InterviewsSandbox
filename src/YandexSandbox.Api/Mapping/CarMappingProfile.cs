using AutoMapper;
using YandexSandbox.Api.Models;
using YandexSandbox.Bll.Models;

namespace YandexSandbox.Api.Mapping;

public class CarMappingProfile : Profile
{
    public CarMappingProfile()
    {
        CreateMap<CreateCarApiRequest, CreateCarRequest>()
            .ForMember(d => d.Make, o => o.MapFrom(s => s.Make!.Trim()))
            .ForMember(d => d.Model, o => o.MapFrom(s => s.Model!.Trim()))
            .ForMember(d => d.Color, o => o.MapFrom(s => s.Color!.Trim()))
            .ForMember(d => d.Vin, o => o.MapFrom(s => s.Vin != null ? s.Vin.Trim() : null));

        CreateMap<CarDto, CarApiResponse>();
    }
}
