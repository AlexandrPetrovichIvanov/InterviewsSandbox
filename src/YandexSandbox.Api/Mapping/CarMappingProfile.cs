using AutoMapper;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Models;

namespace YandexSandbox.Api.Mapping;

public class CarMappingProfile : Profile
{
    public CarMappingProfile()
    {
        CreateMap<CreateCarApiRequest, CreateCarCommand>()
            .ForMember(d => d.Make, o => o.MapFrom(s => s.Make!.Trim()))
            .ForMember(d => d.Model, o => o.MapFrom(s => s.Model!.Trim()))
            .ForMember(d => d.Color, o => o.MapFrom(s => s.Color!.Trim()))
            .ForMember(d => d.Vin, o => o.MapFrom(s => s.Vin != null ? s.Vin.Trim() : null));

        CreateMap<CarModel, CarApiResponse>();
    }
}
