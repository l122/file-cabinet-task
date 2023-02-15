using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the Purge Command Request.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "purge";

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.service.Purge();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }
    }
}
