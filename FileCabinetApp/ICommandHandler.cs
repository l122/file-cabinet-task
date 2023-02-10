namespace FileCabinetApp
{
    /// <summary>
    /// Command Handler interface.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets the next command hander object.
        /// </summary>
        /// <param name="commandHandler">An <see cref="ICommandHandler"/> instance.</param>
        public void SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handles the command passed by parameter.
        /// </summary>
        /// <param name="appCommandRequest">The <see cref="AppCommandRequest"/> instance parameter.</param>
        public void Handle(AppCommandRequest appCommandRequest);
    }
}