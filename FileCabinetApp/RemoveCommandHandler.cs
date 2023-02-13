using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Remove Command Request.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "remove";

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Remove(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Remove(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect id parameter: {0}", parameters);
                return;
            }

            this.service.RemoveRecord(id);
        }
    }
}
