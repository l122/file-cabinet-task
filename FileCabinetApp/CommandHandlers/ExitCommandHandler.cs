using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the Exit Command Request.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "exit";
        private readonly Action<bool> onExit;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="onExit">An <see cref="Action{T}"/> instance.</param>
        public ExitCommandHandler(Action<bool> onExit)
        {
            this.onExit = onExit;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Exit();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Exit()
        {
            Console.WriteLine("Exiting an application...");
            this.onExit(false);
        }
    }
}
