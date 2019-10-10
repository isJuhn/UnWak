# UnWak
Wak file unpacker

Decrypt and unpack wak files used by the game Noita. Confirmed to work on Windows and Linux (Ubuntu 18.04 tested), probably works on macOS too.

# Requirements
* [.NET Core 3.0 Runtime](https://www.microsoft.com/net/download/windows) or newer

# Running
`UnWak [FILE] [<path>]`

Example: `UnWak data.wak` to unpack `data.wak` to the current directory or `UnWak data_old.wak data_old/` to unpack `data_old.wak` to `data_old/`

# Build Requirements
* [.NET Core 3.0 SDK](https://www.microsoft.com/net/download/windows) or newer

# Building
`dotnet build -c Release`

Or open `UnWak.sln` using Visual Studio 2019 with .NET Core 3.0 SDK installed and build the Release configuration
