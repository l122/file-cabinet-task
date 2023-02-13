using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Find Command Request.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "find";
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Find(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Find(string parameters)
        {
            const string firstNameField = "firstname";
            const string lastNameField = "lastname";
            const string dateOfBirthField = "dateofbirth";

            var input = parameters.Split(" ");
            if (input.Length != 2)
            {
                Console.WriteLine("Invalid parameters.");
                Console.WriteLine("Use syntax 'find <firstname, lastname, dateofbirth> <criterion>'");
                return;
            }

            string field = input[0].ToLower(CultureInfo.InvariantCulture);
            string criterion = input[1].Trim('"');
            try
            {
                switch (field)
                {
                    case firstNameField:
                        PrintRecords(this.service.FindByFirstName(criterion));
                        break;

                    case lastNameField:
                        PrintRecords(this.service.FindByLastName(criterion));
                        break;

                    case dateOfBirthField:
                        PrintRecords(this.service.FindByDateOfBirth(criterion));
                        break;

                    default:
                        Console.WriteLine("Invalid parameters.");
                        Console.WriteLine("Use syntax 'find <firstname, lastname, dateofbirth> <criterion>'");
                        break;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
