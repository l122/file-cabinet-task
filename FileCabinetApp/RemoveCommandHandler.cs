using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Remove Command Request.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "remove";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                Remove(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Remove(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect id parameter: {0}", parameters);
                return;
            }

            Program.fileCabinetService.RemoveRecord(id);
        }
    }
}
