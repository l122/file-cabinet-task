﻿namespace FileCabinetApp
{
    /// <summary>
    /// Handles the List Command Request.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "list";
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        /// <param name="printer">An <see cref="IRecordPrinter"/> specialized instance.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
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
            this.printer.Print(this.service.GetRecords());
        }
    }
}
