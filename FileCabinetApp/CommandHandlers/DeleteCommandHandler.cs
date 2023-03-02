using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the Delete Command Request.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "delete";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Delete(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Delete(string parameters)
        {
            Console.WriteLine(this.service.Delete(parameters));
        }
    }
}
