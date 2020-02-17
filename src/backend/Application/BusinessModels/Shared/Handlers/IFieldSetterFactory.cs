using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessModels.Shared.Handlers
{
    public interface IFieldSetterFactory
    {
        IFieldSetter<TEntity> Create<TEntity>() where TEntity : class, IPersistable;
    }
}
