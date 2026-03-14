using AutoMapper;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.CommonModels;

namespace YandexSandbox.Api.Mapping;

public class RentMappingProfile : Profile
{
    public RentMappingProfile()
    {
        CreateMap<PlaceOrderApiRequest, PlaceOrderCommand>();
        CreateMap<RentOrderModel, RentOrderApiResponse>();
    }
}
