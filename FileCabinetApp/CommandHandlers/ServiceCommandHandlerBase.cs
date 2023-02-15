using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base class for the Service Command Handler Classes.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// A <see cref="IFileCabinetService"/> specialized instance.
        /// </summary>
        protected readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="service">A <see cref="IFileCabinetService"/> specialized instance.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.service = service;
        }
    }
}
