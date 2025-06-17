# FinSync

```
    ·▄▄▄▪   ▐ ▄ .▄▄ ·  ▄· ▄▌ ▐ ▄  ▄▄·      ▄▄▄·  ▄▄▄·▪
    ▐▄▄·██ •█▌▐█▐█ ▀. ▐█▪██▌•█▌▐█▐█ ▌▪    ▐█ ▀█ ▐█ ▄███
    ██▪ ▐█·▐█▐▐▌▄▀▀▀█▄▐█▌▐█▪▐█▐▐▌██ ▄▄    ▄█▀▀█  ██▀·▐█·
    ██▌.▐█▌██▐█▌▐█▄▪▐█ ▐█▀·.██▐█▌▐███▌    ▐█ ▪▐▌▐█▪·•▐█▌
    ▀▀▀ ▀▀▀▀▀ █▪ ▀▀▀▀   ▀ • ▀▀ █▪·▀▀▀      ▀  ▀ .▀   ▀▀▀
```

FinSync is a personal finance platform built as the final year project for the PSI1623R course. It consists of two main pieces:

* **FinSync API** – an ASP.NET Core Web API (\`src/api/api\`)
* **FinSync Client** – a Windows Forms desktop application (\`src/Client/login\`)

Both projects are included in this repository along with a small helper for clearing authentication tokens.

## Features

- JWT-based authentication with API key header
- Budget, income and expense management screens
- LiveCharts visualizations
- Simple console panel for monitoring the API

## Prerequisites

- **.NET 8 SDK** for building and running the API
- **.NET Framework 4.7.2** and Visual Studio (Windows) for the desktop client
- SQL Server (connection string can be set in `appsettings.json`)

## Running the API

```bash
cd src/api/api
# optional helper script that cleans and restarts on exit
./starter.bat     # Windows
# or run directly
dotnet run
```

The API listens by default on `http://localhost:5034` and requires a header `x-api-key` with value `12345-abcdef-67890`.

## Running the Client

Open `src/Client/login/login.sln` with Visual Studio and build the `login` project. Ensure the API is running before launching the client. On first login a token is saved to `auth.token`; deleting this file logs you out.

A small helper project `token-eraser` can be used to remove the token.

## Repository Links

- [FinSync Client on GitHub](https://github.com/tigokraft/FinSync)
- [FinSync API on GitHub](https://github.com/tigokraft/FinSync-api)

This project is licensed under the Apache 2.0 license. See [LICENSE](LICENSE) for details.
