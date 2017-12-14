using System;

namespace Pipaslot.Infrastructure.Data
{
    /// <summary>
    /// A base implementation of unit of work provider.
    /// </summary>
    public abstract class AUnitOfWorkFactory<TUnitOfWork> : IUnitOfWorkFactory
        where TUnitOfWork : IUnitOfWork
    {
        private readonly IUnitOfWorkRegistry _registry;

        protected AUnitOfWorkFactory(IUnitOfWorkRegistry registry)
        {
           _registry = registry;
        }

        IUnitOfWork IUnitOfWorkFactory.Create()
        {
            return Create();
        }

        /// <inheritdoc />
        public virtual TUnitOfWork Create()
        {
            var uow = CreateUnitOfWork();
            _registry.Register(uow);
            uow.Disposing += OnUnitOfWorkDisposing;
            return uow;
        }

        IUnitOfWork IUnitOfWorkFactory.GetCurrent(int index)
        {
            return GetCurrent(index);
        }

        public virtual TUnitOfWork GetCurrent(int index = 0)
        {
            var uow = _registry.GetCurrent(index);
            if (uow is TUnitOfWork expectedUoW)
            {
                return expectedUoW;
            }
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
            _registry.Unregister((TUnitOfWork)sender);
        }
    }
}
