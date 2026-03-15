using AutoMapper;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Api.Mapping;

public class RentOrderMappingProfile : Profile
{
    public RentOrderMappingProfile()
    {
        CreateMap<PlaceRentOrderApiRequest, PlaceRentOrderCommand>();
        CreateMap<RentOrderModel, RentOrderApiResponse>();
    }
}
