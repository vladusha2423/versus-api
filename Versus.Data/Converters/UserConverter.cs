using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Versus.Data.Dto;
using Versus.Data.Entities;

namespace Versus.Data.Converters
{
    public static class UserConverter
    {
        public static UserDto Convert(User item)
        {
            return new UserDto
            {
                UserName = item.UserName,
                Email = item.Email,
                Token = item.Token,
                Country = item.Country,
                Id = item.Id,
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
                Token = item.Token,
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
