using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the Update Command Request.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "update";

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Update(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Update(string parameters)
        {
            Console.WriteLine(this.service.Update(parameters));
        }
    }
}
