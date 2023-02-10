using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Exit Command Request.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "exit";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                Exit();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }
    }
}
