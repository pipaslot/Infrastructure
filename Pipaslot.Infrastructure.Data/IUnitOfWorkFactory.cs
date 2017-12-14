using System;

namespace Pipaslot.Infrastructure.Data
{
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// Creates an unit of work scope.
        /// </summary>
        IUnitOfWork Create();

        /// <summary>
        /// Get one Existing Unit of work on requested level of nesting
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IUnitOfWork GetCurrent(int index = 0);
        
    }
}
