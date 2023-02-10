using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Create Command Request.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "create";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                Create();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Create()
        {
            var id = Program.fileCabinetService.CreateRecord();
            if (id > 0)
            {
                Console.WriteLine("Record #{0} is created.", id);
            }
            else
            {
                Console.WriteLine("Record is not created.");
            }
        }

    }
}
