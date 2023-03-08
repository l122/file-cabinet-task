# Description
This repository containes the implementation of the [File Cabinet task from the EPAM .Net Lab](https://github.com/epam-dotnet-lab/file-cabinet-task).
It's a console application of managing user data.

- Framework version: .Net 7.0
- Language version: C# 11.0
- IDE: Microsoft Visual Studio 2022

# How to use it on Windows
1) Download the [release](https://github.com/l122/file-cabinet-task/releases/tag/v.1.0.0)
2) Go to the downloaded folder.
3) Download [validation-rules.json](https://github.com/l122/file-cabinet-task/blob/main/ValidationRules/validation-rules.json) in this folder.
4) Open a command prompt in this folder.
5) Type 'FileCabinetGenerator.exe args' (see the generator arguments list below) to generate some sample data.
6) Type 'FileCabinetApp.exe args' (see the application arguments list below) to start the program.
7) Type 'help' in the appeared console window to see the available commands.

# Application arguments
The app can include any of the arguments below in any order:
- Validation rules options: -v | --validation-rules= default | custom
- Storage options: -s | --storage= memory | file
- Use stopwatch flag: --use-stopwatch
- Use log flag: --use-logger

Example: FileCabinetApp.exe -v custom -s file

The above command would start the application with custom data validation rules and save all the data to a file in the build folder.

# Generator arguments
The generator must include all of the argument below in any order:
- Output file type options: -t | --output-type= csv | xml
- Name of output file: -o | --output=
- Records amount: -a | --records-amount=
- Starting record id: -i | --start-id=
- Validation rules options: -v | --validation-rules= default | custom

Example: FileCabinetGenerator.exe -t csv -o data.csv -a 1000 -i 1 -v default

This would generate data.csv file with 1000 records starting with id = 1 with default validation rules. This generated file could be used for importing data in File Cabinet App.
