# Description
This repository containes the implementation of the [File Cabinet task from the EPAM .Net Lab](https://github.com/epam-dotnet-lab/file-cabinet-task).
It's a console application of managing user data.

# How to use it
1) Clone the repository to your computer.
2) Go to the downloaded folder and open the FileCabinet.sln with Microsoft Visual Studio.
3) Press F5 to build and run the application.
4) Type 'help' in the appeared console window to see the available commands.

# Application arguments
- Validation rules options: -v | --validation-rules= default | custom
- Storage options: -s | --storage= memory | file
- Use stopwatch flag: --use-stopwatch
- Use log flag: --use-logger

# Data generator arguments
- Output file type options: -t | --output-type= csv | xml
- Records amount: -a | --records-amount=
- Starting record id: -i | --start-id=