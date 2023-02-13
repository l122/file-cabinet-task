using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Create Command Request.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "create";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Create();
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Create()
        {
            var id = this.service.CreateRecord();
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
