using AutoMapper;
using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateUserDTO, User>();
            CreateMap<User, UpdateUserDTO>();

            CreateMap<RegisterDTO, User>();
            CreateMap<User, RegisterDTO>();
        }

        // /// <summary>
        // /// Checks if the object has been initialized
        // /// </summary>
        // /// <param name="source">Object to check</param>
        // /// <returns>
        // /// Returns true if:
        // /// <list type="number">
        // ///     <item>The reference type has a null value</item>
        // ///     <item>The value type has its default value</item>
        // /// </list>
        // /// </returns>
        // private bool IsNotInitialized<T>(T source)
        // {
        //     return EqualityComparer<T>.Default.Equals(source, default);
        // }
    }
}
