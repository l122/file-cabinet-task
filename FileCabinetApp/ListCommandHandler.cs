namespace FileCabinetApp
{
    /// <summary>
    /// Handles the List Command Request.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "list";
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.List();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void List()
        {
            PrintRecords(this.service.GetRecords());
        }
    }
}
