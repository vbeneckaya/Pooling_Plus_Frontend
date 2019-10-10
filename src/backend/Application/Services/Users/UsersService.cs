using System;
using System.Linq;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Users;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Domain.Shared;
using DAL.Queries;
using Domain.Services.UserProvider;
using Domain.Services.Translations;

namespace Application.Services.Users
{
    public class UsersService : DictonaryServiceBase<User, UserDto>, IUsersService
    {
        public UsersService(AppDbContext dbContext, IUserProvider userProvider)
            : base(dbContext)
        {
            _userProvider = userProvider;
        }

        public override DbSet<User> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Users;
        }

        public ValidateResult SetActive(Guid id, bool active)
        {
            var user = _userProvider.GetCurrentUser();
            var entity = db.Users.GetById(id);
            if (entity == null)
            {
                return new ValidateResult("userNotFoundEntity".translate(user.Language));
            }
            else
            {
                entity.IsActive = active;
                db.SaveChanges();

                return new ValidateResult(null, entity.Id.ToString());
            }
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = db.Users.OrderBy(x => x.Name).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString()
                };
            }
        }


        public override UserDto MapFromEntityToDto(User entity)
        {
            var roles = db.Roles.ToList();
            return new UserDto
            {
                Email = entity.Email,
                Id = entity.Id.ToString(),
                UserName = entity.Name,
                Role = roles.FirstOrDefault(role => role.Id == entity.RoleId).Name,
                RoleId = entity.RoleId.ToString(),
                FieldsConfig = entity.FieldsConfig,
                IsActive = entity.IsActive
            };
        }

        public override void MapFromDtoToEntity(User entity, UserDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id)) 
                entity.Id = Guid.Parse(dto.Id);
            
            entity.Email = dto.Email;
            entity.Name = dto.UserName;
            entity.RoleId = Guid.Parse(dto.RoleId);
            entity.FieldsConfig = dto.FieldsConfig;
            entity.IsActive = dto.IsActive;
            
            if (!string.IsNullOrEmpty(dto.Password)) 
                entity.PasswordHash = dto.Password.GetHash();
        }

        private readonly IUserProvider _userProvider;
    }
}