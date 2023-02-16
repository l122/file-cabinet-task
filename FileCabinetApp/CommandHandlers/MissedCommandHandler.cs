using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles an Incorrect Command Request.
    /// </summary>
    public class MissedCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            Console.WriteLine($"There is no '{appCommandRequest.Command}' command.");
            Console.WriteLine();
        }
    }
}
