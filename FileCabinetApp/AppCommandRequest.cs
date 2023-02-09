namespace FileCabinetApp
{
    public class AppCommandRequest
    {
        public string Command { get; }

        public string Parameters { get; }

        public AppCommandRequest(string command, string parameters)
        {
            Command = command;
            Parameters = parameters;
        }

     }
}