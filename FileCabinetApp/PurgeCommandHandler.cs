namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Purge Command Request.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "purge";
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
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
