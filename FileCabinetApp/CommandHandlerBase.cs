namespace FileCabinetApp
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        abstract public void Handle(AppCommandRequest appCommandRequest);

        public void SetNext(ICommandHandler commandHandler)
        {
            nextHandler = commandHandler;
        }
    }
}
