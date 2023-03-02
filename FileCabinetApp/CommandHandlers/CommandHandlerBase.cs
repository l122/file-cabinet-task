using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The Command Handler Base class.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// Help messages.
        /// </summary>
        protected static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[]
            {
                "delete",
                "deletes records",
                "The 'delete where <expression>' deletes the records that satisfy the expression."
                + "\nThe expression should consist of record field names such as id, FirstName, LastName, DateOfBirth, Workplace, Salary, Department, "
                + "as well as the logical and comparison operators such as 'and', 'or', 'not', '=' and '!='."
                + "\nExampe: delete where id = 1 or firstname = johny and lastname != doe and not dateofbirth = 01.01.2000",
            },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "export", "exports records to csv or xml", "The 'export <csv, xml> <file_name>' command exports records to a csv or xml file." },
            new string[] { "find", "searches records", "The 'find <firstname, lastname, dateofbirth> <criterion>' command searches all records with <field> = <criterion>." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "import", "imports records from csv or xml", "The 'import <csv, xml> <file_name>' command imports records from a csv or xml file." },
            new string[] { "insert", "inserts a record", "The 'insert (id, firstname, lastname, dateofbirth, workplace, salary, department) values ('id#', 'firstname', 'lastname', 'dd.mm.yyyy', 'place#', 'salary', 'department')' command inserts a record." },
            new string[] { "list", "prints all records", "The 'list' command prints all records." },
            new string[] { "purge", "removes records marked as deleted from the database", "The 'purge' command removes records marked as deleted from the database." },
            new string[] { "stat", "prints the number of records", "The 'stat' command prints the number of records." },
            new string[]
            {
                "update",
                "updates records",
                "The 'update set field='value' where field='value' update the records that satisfy the where expression."
                + "\nThe expression should consist of record field names such as id, FirstName, LastName, DateOfBirth, Workplace, Salary, Department, "
                + "as well as the logical and comparison operators such as 'and', 'or', 'not', '=' and '!='."
                + "\nExampe: update set firstname = John, lastname = Doe , dateofbirth = 01.01.2000 where id = 1",
            },
        };

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
