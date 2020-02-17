using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.FieldSetter
{
    public class FieldSetterFactory : IFieldSetterFactory
    {
        public IFieldSetter<TEntity> Create<TEntity>() where TEntity : class, IPersistable
        {
            return new FieldUpdater<TEntity>();
        }
    }
}
