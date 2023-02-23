using System;
using System.IO;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the Import Command Request.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "import";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Import(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Import(string parameters)
        {
            const string csvParameter = "csv";
            const string xmlParameter = "xml";
            int restoredQuantity = 0;

            var input = parameters.Split(" ");
            if (input.Length != 2)
            {
                Console.WriteLine("Invalid parameters.");
                Console.WriteLine("Use syntax 'import <csv, xml> <file_name>'");
                return;
            }

            // Create / open file
            string? file = input[1];
            if (!File.Exists(file))
            {
                Console.WriteLine("Import error: file {0} does not exist.", file);
                return;
            }

            var snapshot = new FileCabinetServiceSnapshot();
            try
            {
                using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

                string parameter = input[0];
                switch (parameter)
                {
                    case csvParameter:
                        snapshot.LoadFromCsv(fileStream);
                        break;
                    case xmlParameter:
                        snapshot.LoadFromXml(fileStream);
                        break;
                    default:
                        Console.WriteLine("Invalid parameters.");
                        break;
                }

                restoredQuantity = this.service.Restore(snapshot);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
                return;
            }

            Console.WriteLine("{0} records were imported from {1}", restoredQuantity, file);
        }
    }
}
