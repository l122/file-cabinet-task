using System;
using System.Collections.Generic;

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
            Console.WriteLine($"There is no '{appCommandRequest.Command}' command. See 'help'.");
            Console.WriteLine();
            var possibleCommands = GetSimilarCommands(appCommandRequest);
            if (possibleCommands.Count == 0)
            {
                return;
            }

            Console.WriteLine("The most similar commands are:");
            foreach (var command in possibleCommands)
            {
                Console.WriteLine("\t\t{0}", command);
            }

            Console.WriteLine();
        }

        private static List<string> GetSimilarCommands(AppCommandRequest request)
        {
            List<string> result = new ();

            if (string.IsNullOrEmpty(request.Command))
            {
                return result;
            }

            foreach (var line in HelpMessages)
            {
                var command = line[0];
                if (command.StartsWith(request.Command[0]) && request.Command.Length <= command.Length)
                {
                    result.Add(command);
                }
            }

            return result;
        }
    }
}
