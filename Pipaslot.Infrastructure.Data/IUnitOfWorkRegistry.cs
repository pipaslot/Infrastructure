namespace Pipaslot.Infrastructure.Data
{
    /// <summary>
    /// An interface for unit of work registry which is responsible for maintaining active unit of work object in the current scope.
    /// </summary>
    public interface IUnitOfWorkRegistry
    {
        /// <summary>
        /// Registers a new unit of work.
        /// </summary>
        void Register(IUnitOfWork unitOfWork);

        /// <summary>
        /// Unregisters a specified unit of work.
        /// </summary>
        void Unregister(IUnitOfWork unitOfWork);

        /// <summary>
        /// Current/last registered unit of work in the current scope.
        /// </summary>
        /// <returns></returns>
        IUnitOfWork GetCurrent(int level = 0);
    }
}
