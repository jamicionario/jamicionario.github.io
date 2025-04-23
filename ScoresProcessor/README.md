
# Scores Processor

This C# console application updates the local Jamicionário data from your `*.mscz` MuseScore files.

It reads the MSCZ files from the configured folder, and exports the data to the public folder.

## Running

To run, open a terminal in the _ScoresProcessor_ folder and execute:

```shell
dotnet run ScoresProcessor
```

The console application logs to the console any issues it finds.

### In VS Code

This can also be run in [VS Code](https://code.visualstudio.com/), which allows debugging.

- Open this folder in VS Code and run with <kbd>F5</kbd>: menu _Run_ > _Start Debugging_.
- Or open the root folder in VS Code, then:
  1. Select the "Run and Debug" tab on the left;
  2. Choose the option "run Scores Processor" on the dropdown.
  3. Start debugging with the green ▶️ "Run" button.

![screenshot of running in VS Code from the root folder](../docs/processing%20data%20from%20VS%20Code.png)

## Installing

To run the project, you will need to have the [.Net SDK](https://dotnet.microsoft.com/download).
