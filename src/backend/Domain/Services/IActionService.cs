using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
    public interface IActionService<TEntity>
    {
        IEnumerable<IAction<TEntity>> GetActions();

        IEnumerable<IAction<IEnumerable<TEntity>>> GetGroupActions();
    }
}
