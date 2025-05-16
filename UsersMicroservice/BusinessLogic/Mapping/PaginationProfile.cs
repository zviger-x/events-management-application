using AutoMapper;
using DataAccess.Common;

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
