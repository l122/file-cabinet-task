using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// The Command Handler Base class.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler? nextHandler;

        /// <inheritdoc/>
        public virtual void Handle(AppCommandRequest appCommandRequest)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(appCommandRequest);
            }
        }

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
        }

        /// <summary>
        /// Checks if the object can handle the command request.
        /// </summary>
        /// <param name="trigger">A <see cref="string"/> instance of the trigger.</param>
        /// <param name="request">A <see cref="string"/> instance of the request.</param>
        /// <returns>true if can handle, false otherwise.</returns>
        protected static bool CanHandle(string trigger, string request)
        {
            return request.Equals(trigger, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Prints record to console.
        /// </summary>
        /// <param name="records">The <see cref="ReadOnlyCollection{T}"/> of type <see cref="FileCabinetRecord"/> instance.</param>
        protected static void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records == null)
            {
                return;
            }

            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }
    }
}
