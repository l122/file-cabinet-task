namespace FileCabinetApp
{
    /// <summary>
    /// Class for passing command and parameters.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command">A <see cref="string"/> instance of the command.</param>
        /// <param name="parameters">A <see cref="string"/> instance of the parameters.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the <see cref="string"/> value of the command.
        /// </summary>
        /// <value>
        /// The <see cref="string"/> value of the command.
        /// </value>
        public string Command { get; }

        /// <summary>
        /// Gets the <see cref="string"/> value of the parameters.
        /// </summary>
        /// <value>
        /// The <see cref="string"/> value of the parameters.
        /// </value>
        public string Parameters { get; }
     }
}