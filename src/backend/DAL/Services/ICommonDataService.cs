using Domain.Persistables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Services
{
    public interface ICommonDataService
    {
        DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IPersistable;

        TEntity GetById<TEntity>(Guid id) where TEntity : class, IPersistable;

        void SaveChanges();
    }
}
