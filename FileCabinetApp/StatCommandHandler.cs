using System;

namespace FileCabinetApp
{
    /// <summary>
    /// The class for handling the statistics command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "stat";
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Stat();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Stat()
        {
            var recordsCount = this.service.GetStat();
            Console.WriteLine("{0} record(s), {1} deleted record(s).", recordsCount.Item1, recordsCount.Item2);
        }
    }
}
