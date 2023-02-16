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

            var oldRecord = this.service.FindById(id);
            if (oldRecord == null)
            {
                Console.WriteLine("#{0} record is not found.", id);
                return;
            }

            var newRecord = Program.GetInputData();
            newRecord.Id = oldRecord.Id;

            if (this.service.EditRecord(newRecord))
            {
                Console.WriteLine("Record #{0} is updated.", id);
            }
            else
            {
                Console.WriteLine("Record is not updated.");
            }
        }
    }
}
