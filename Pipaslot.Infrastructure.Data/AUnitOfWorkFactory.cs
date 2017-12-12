using System;

namespace Pipaslot.Infrastructure.Data
{
    /// <summary>
    /// A base implementation of unit of work provider.
    /// </summary>
    public abstract class AUnitOfWorkFactory<TUnitOfWork> : IUnitOfWorkFactory<TUnitOfWork> where TUnitOfWork : IUnitOfWork
    {
        private readonly IUnitOfWorkRegistry registry;
        
        protected AUnitOfWorkFactory(IUnitOfWorkRegistry registry)
        {
            this.registry = registry;
        }

        /// <inheritdoc />
        public virtual TUnitOfWork Create()
        {
            var uow = CreateUnitOfWork();
            registry.Register(uow);
            uow.Disposing += OnUnitOfWorkDisposing;
            return uow;
        }

        public TUnitOfWork GetCurrent(bool isNeeded = true)
        {
            var index = 0;
            var uow = registry.GetCurrent(index);
            while (uow != null)
            {
                if (uow is TUnitOfWork expectedUoW)
                {
                    return expectedUoW;
                }

                index++;
                uow = registry.GetCurrent(index);
            }
            if(isNeeded)throw new InvalidOperationException("Can not get current Unit of work. This method must be used in a unit of work scope surrounded by using(var uow = uowFactory.Create()){...}");
            return default(TUnitOfWork);
        }

        /// <summary>
        /// Creates the real unit of work instance.
        /// </summary>
        protected abstract TUnitOfWork CreateUnitOfWork();
        
        /// <summary>
        /// Called when the unit of work is being disposed.
        /// </summary>
        private void OnUnitOfWorkDisposing(object sender, EventArgs e)
        {
            registry.Unregister((IUnitOfWork)sender);
        }
    }
}
