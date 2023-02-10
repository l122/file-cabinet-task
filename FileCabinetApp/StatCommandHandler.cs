using System;

namespace FileCabinetApp
{
    /// <summary>
    /// The class for handling the statistics command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "stat";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                Stat();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Stat()
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine("{0} record(s), {1} deleted record(s).", recordsCount.Item1, recordsCount.Item2);
        }
    }
}
