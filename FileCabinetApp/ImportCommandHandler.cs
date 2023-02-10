using System;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Import Command Request.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "import";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                Import(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Import(string parameters)
        {
            const string csvParameter = "csv";
            const string xmlParameter = "xml";
            int oldQuantity = Program.fileCabinetService.GetStat().Item1;

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

            try
            {
                using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                var snapshot = new FileCabinetServiceSnapshot();

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

                Program.fileCabinetService.Restore(snapshot);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
                return;
            }

            Console.WriteLine("{0} records were imported from {1}", Program.fileCabinetService.GetStat().Item1 - oldQuantity, file);
        }

    }
}
