namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Purge Command Request.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "purge";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                Program.fileCabinetService.Purge();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }
    }
}
