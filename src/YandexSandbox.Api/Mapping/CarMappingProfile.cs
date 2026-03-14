using AutoMapper;
using YandexSandbox.Api.Models;
using YandexSandbox.Bll.Models;

namespace YandexSandbox.Api.Mapping;

public class CarMappingProfile : Profile
{
    public CarMappingProfile()
    {
        CreateMap<CreateCarApiRequest, CreateCarRequest>();
        CreateMap<CarDto, CarApiResponse>();
    }
}
