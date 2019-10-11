using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
    public interface ICommonDataService
    {
        DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class;

        TEntity GetById<TEntity>(Guid id) where TEntity : class;

        void SaveChanges();
    }
}
