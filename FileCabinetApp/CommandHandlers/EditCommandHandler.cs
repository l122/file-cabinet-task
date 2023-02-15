using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the Edit Command Request.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "edit";

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Edit(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect id parameter: {0}", parameters);
                return;
            }

            this.service.EditRecord(id);
        }
    }
}
