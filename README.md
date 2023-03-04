# Description
This repository containes the implementation of the [File Cabinet task from the EPAM .Net Lab](https://github.com/epam-dotnet-lab/file-cabinet-task).
It's a console application of managing user data.

- Framework version: .Net 7.0
- Language version: C# 11.0
- IDE: Microsoft Visual Studio 2022

# How to use it on Windows
1) Clone the repository to your computer.
2) Go to the downloaded folder and open the FileCabinet.sln with Microsoft Visual Studio.
3) Press F5 to build and run the application.
4) Type 'help' in the appeared console window to see the available commands.

# Application arguments
An alternative way of running the application is to use the application arguments.
To do so, open the command prompt in the build folder (*file-cabinet-task\FileCabinetApp\bin\Debug\net7.0*) where the executable files are located, and type 'FileCabinetApp.exe args', where args could be any of the following flags:
- Validation rules options: -v | --validation-rules= default | custom
- Storage options: -s | --storage= memory | file
- Use stopwatch flag: --use-stopwatch
- Use log flag: --use-logger

Example: FileCabinetApp.exe -v custom -s file

The above command would start the application with custom data validation rules and save all the data to a file in the build folder.

# How to generate sample records
1) Go to the *file-cabinet-task\FileCabinetGenerator\bin\Debug\net7.0* folder.
2) Open a command prompt in this folder.
3) Type 'FileCabinetGenerator.exe args', where args should be the following:
- Output file type options: -t | --output-type= csv | xml
- Name of output file: -o | --output=
- Records amount: -a | --records-amount=
- Starting record id: -i | --start-id=

Example: FileCabinetGenerator.exe -t csv -o data.csv -a 1000 -i 1

This would generate data.csv file with 1000 records starting with id = 1. This generated file could be used for importing data in File Cabinet App.