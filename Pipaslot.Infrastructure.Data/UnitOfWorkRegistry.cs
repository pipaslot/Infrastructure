using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pipaslot.Infrastructure.Data
{
    /// <inheritdoc />
    /// <summary>
    /// A unit of work storage which persists the unit of work instances in a AsyncLocal object.
    /// </summary>
    public class UnitOfWorkRegistry : IUnitOfWorkRegistry
    {
        private readonly AsyncLocal<Stack<IUnitOfWork>> stack = new AsyncLocal<Stack<IUnitOfWork>>();
        
        protected Stack<IUnitOfWork> GetStack()
        {
            if (stack.Value == null)
            {
                stack.Value = new Stack<IUnitOfWork>();
            }
            return stack.Value;
        }

        /// <inheritdoc />
        public void Register(IUnitOfWork unitOfWork)
        {
            var unitOfWorkStack = GetStack();
            unitOfWorkStack.Push(unitOfWork);
        }

        /// <inheritdoc />
        public void Unregister(IUnitOfWork unitOfWork)
        {
            var unitOfWorkStack = GetStack();
            if (unitOfWorkStack.Any())
            {
                if (unitOfWorkStack.Pop() == unitOfWork)
                {
                    return;
                }
            }
            throw new InvalidOperationException("Some of the unit of works was not disposed correctly!");
        }

        /// <inheritdoc />
        public IUnitOfWork GetCurrent(int level = 0)
        {
            var unitOfWorkStack = GetStack();

            if (level >= unitOfWorkStack.Count)
            {
                return null;
            }
            else if (level == 0)
            {
                return unitOfWorkStack.Peek();
            }
            else
            {
                return unitOfWorkStack.ToArray()[level];
            }
        }
    }
}
