using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the List Command Request.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "list";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                List();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void List()
        {
            PrintRecords(Program.fileCabinetService.GetRecords());
        }
    }
}
