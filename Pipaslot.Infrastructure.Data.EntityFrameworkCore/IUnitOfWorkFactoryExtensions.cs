using System;
using Microsoft.EntityFrameworkCore;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    public static class IUnitOfWorkFactoryExtensions
    {
        /// <summary>
        /// Get DbContext from Unit of work Using Entity Framework
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="factory"></param>
        /// <param name="isNeeded"></param>
        /// <returns></returns>
        public static TDbContext GetDbContext<TDbContext>(this IUnitOfWorkFactory factory, bool isNeeded = true) where TDbContext : DbContext
        {
            var index = 0;
            var uow = factory.GetCurrent(index);
            while (uow != null)
            {
                if (uow is IEntityFrameworkUnitOfWork<TDbContext> expectedUoW)
                {
                    return expectedUoW.Context;
                }

                index++;
                uow = factory.GetCurrent(index);
            }
            if (isNeeded) throw new InvalidOperationException("Can not get current Unit of Work for Entity Framework. This method must be used in a unit of work scope surrounded by using(var uow = uowFactory.Create()){...} and Unit of Work must implements interface "+typeof(IEntityFrameworkUnitOfWork<TDbContext>).FullName);
            return default(TDbContext);
        }
    }
}
