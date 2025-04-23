﻿using DataAccess.Entities;
using DataAccess.Enums;

namespace BusinessLogic.Contracts
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public UserRoles Role { get; set; }
        public IEnumerable<UserNotification> Notifications { get; set; }
        public IEnumerable<UserTransaction> Transactions { get; set; }
    }
}
