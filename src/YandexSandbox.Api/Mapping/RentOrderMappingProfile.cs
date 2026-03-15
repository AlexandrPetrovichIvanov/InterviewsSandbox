using AutoMapper;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Models;

namespace YandexSandbox.Api.Mapping;

public class RentOrderMappingProfile : Profile
{
    public RentOrderMappingProfile()
    {
        CreateMap<PlaceRentOrderApiRequest, PlaceRentOrderCommand>();
        CreateMap<RentOrderModel, RentOrderApiResponse>();
    }
}
