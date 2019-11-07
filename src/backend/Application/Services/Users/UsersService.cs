using Application.Shared;
using DAL.Queries;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Services.Users;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Users
{
    public class UsersService : DictonaryServiceBase<User, UserDto>, IUsersService
    {
        public UsersService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public ValidateResult SetActive(Guid id, bool active)
        {
            var user = _userProvider.GetCurrentUser();
            var entity = _dataService.GetDbSet<User>().GetById(id);
            if (entity == null)
            {
                return new ValidateResult("userNotFoundEntity".Translate(user.Language));
            }
            else
            {
                entity.IsActive = active;
                _dataService.SaveChanges();

                return new ValidateResult(null, entity.Id.ToString());
            }
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<User>().Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
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
            var roles = _dataService.GetDbSet<Role>().ToList();
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

        public override ValidateResult MapFromDtoToEntity(User entity, UserDto dto)
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

            return new ValidateResult(entity.Id.ToString());
        }

        protected override IQueryable<User> ApplySort(IQueryable<User> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Email)
                .ThenBy(i => i.Id);
        }
    }
}