using System;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Handles the Export Command Request.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        private const string Trigger = "export";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                Export(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Export(string parameters)
        {
            const string csvParameter = "csv";
            const string xmlParameter = "xml";
            const string yes = "Y";

            var input = parameters.Split(" ");
            if (input.Length != 2)
            {
                Console.WriteLine("Invalid parameters.");
                Console.WriteLine("Use syntax 'export <csv, xml> <file_name>'");
                return;
            }

            // Create / open file
            string file = input[1];
            if (File.Exists(file))
            {
                Console.Write("File alredy exists. Rewrite '{0}'? [Y/n] ", file);
                var response = Console.ReadLine();
                if (!string.Equals(response, yes, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            StreamWriter? sw = null;
            try
            {
                //// Create stream writer
                sw = File.CreateText(file);

                //// Make Snapshot
                var snapshot = Program.fileCabinetService.MakeSnapshot();

                string parameter = input[0];
                switch (parameter)
                {
                    case csvParameter:
                        snapshot.SaveToCsv(sw);
                        break;
                    case xmlParameter:
                        snapshot.SaveToXml(sw);
                        break;
                    default:
                        Console.WriteLine("Invalid parameters.");
                        break;
                }

                //// Close stream writer
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Export failed: can't open file {0}.", file);
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
    }
}
