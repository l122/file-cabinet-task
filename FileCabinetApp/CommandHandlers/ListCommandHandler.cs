using System;
using System.Collections.Generic;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the Select Command Request.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "list";
        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        /// <param name="printer">An <see cref="Action{T}"/> specialized instance.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
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
            this.printer(this.service.GetRecords());
        }
    }
}
