using System;
using System.Linq;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Users;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Users
{
    public class UsersService : DictonaryServiceBase<User, UserDto>, IUsersService
    {
        public UsersService(AppDbContext dbContext) : base(dbContext)
        {
            
        }

        public override DbSet<User> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Users;
        }


        public override UserDto MapFromEntityToDto(User entity)
        {
            var roles = db.Roles.ToList();
            return new UserDto
            {
                Email = entity.Email,
                Id = entity.Id.ToString(),
                Name = entity.Name,
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
            entity.Name = dto.Name;
            entity.RoleId = Guid.Parse(dto.RoleId);
            entity.FieldsConfig = dto.FieldsConfig;
            entity.IsActive = dto.IsActive;
            
            if (!string.IsNullOrEmpty(dto.Password)) 
                entity.PasswordHash = dto.Password.GetHash();
        }
    }
}