using Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Services
{
    public class CommonDataService : ICommonDataService
    {
        private readonly AppDbContext _context;

        public CommonDataService(AppDbContext context)
        {
            _context = context;
        }

        public TEntity GetById<TEntity>(Guid id) where TEntity : class
        {
            return this.GetDbSet<TEntity>().Find(id);
        }

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return this._context.Set<TEntity>();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
