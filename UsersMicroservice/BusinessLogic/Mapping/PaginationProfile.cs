using AutoMapper;
using Shared.Common;

namespace BusinessLogic.Mapping
{
    public class PaginationProfile : Profile
    {
        public PaginationProfile()
        {
            CreateMap(typeof(PagedCollection<>), typeof(PagedCollection<>));
        }
    }
}
