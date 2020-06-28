using System.Collections.Generic;
using System.Linq;
using Versus.Data.Dto;
using Versus.Data.Entities;

namespace Versus.Data.Converters
{
    public class UserSearchConverter
    {
        public static UserSearchDto Convert(User item)
        {
            return new UserSearchDto
            {
                UserName = item.UserName,
                Photo = item.Photo
            };
        }

        public static List<UserSearchDto> Convert(List<User> items)
        {
            return items.Select(a => Convert(a)).ToList();
        }
    }
}