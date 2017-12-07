using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Infrastructure.Data
{
    public interface IUnitOfWorkFactory<TUnitOfWork> where TUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Creates an unit of work scope.
        /// </summary>
        TUnitOfWork Create();

        /// <summary>
        /// Get one Existing Unit of work or throw exception if isNeeded
        /// </summary>
        /// <param name="isNeeded">If is TRUE and Unit of Work does not exists, then exception is thrown</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        TUnitOfWork GetCurrent(bool isNeeded = true);
    }
}
