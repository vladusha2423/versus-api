using System.Collections.Generic;
using System.Linq;
using Versus.Data.Dto;
using Versus.Data.Entities;

namespace Versus.Data.Converters
{
    public static class UserConverter
    {
        public static UserDto Convert(User item)
        {
            Settings settings;
            if (item.Settings != null)
            {
                settings = item.Settings;
                settings.User = null;
            }
            Exercises exercises;
            if (item.Settings != null)
            {
                exercises = item.Exercises;
                exercises.User = null;
            }
            VIP vip;
            if (item.Settings != null)
            {
                vip = item.Vip;
                vip.User = null;
            }

            return new UserDto
            {
                UserName = item.UserName,
                Email = item.Email,
                Token = item.Token,
                LastTime = item.LastTime,
                Country = item.Country,
                Id = item.Id,
                Online = item.Online,
                IsVip = item.IsVip,
                Settings = item.Settings,
                Vip = item.Vip,
                Exercises = item.Exercises,
            };
        }

        public static User Convert(UserDto item)
        {
            return new User
            {
                UserName = item.UserName,
                Email = item.Email,
                LastTime = item.LastTime,
                Token = item.Token,
                Online = item.Online,
                IsVip = item.IsVip,
                Country = item.Country,
                Id = item.Id,
            };
        }

        public static List<UserDto> Convert(List<User> items)
        {
            return items.Select(a => Convert(a)).ToList();
        }

        public static List<User> Convert(List<UserDto> items)
        {
            return items.Select(a => Convert(a)).ToList();
        }
    }
}
