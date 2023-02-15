﻿using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The Command Handler Base class.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler? nextHandler;

        /// <inheritdoc/>
        public virtual void Handle(AppCommandRequest appCommandRequest)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(appCommandRequest);
            }
        }

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
        }

        /// <summary>
        /// Checks if the object can handle the command request.
        /// </summary>
        /// <param name="trigger">A <see cref="string"/> instance of the trigger.</param>
        /// <param name="request">A <see cref="string"/> instance of the request.</param>
        /// <returns>true if can handle, false otherwise.</returns>
        protected static bool CanHandle(string trigger, string request)
        {
            return request.Equals(trigger, StringComparison.OrdinalIgnoreCase);
        }
    }
}