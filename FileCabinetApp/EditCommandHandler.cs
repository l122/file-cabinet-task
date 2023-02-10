using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Edit Command Request.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "edit";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                Edit(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect id parameter: {0}", parameters);
                return;
            }

            Program.fileCabinetService.EditRecord(id);
        }

    }
}
