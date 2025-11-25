# EasyVend

EasyVend is a marketplace administration and listing publisher solution built with .NET 9 and Blazor WebAssembly. It provides services for integrating with external marketplaces and a single-page front-end for managing listings, publishing, and monitoring.

## Key features
- Blazor WebAssembly front-end and .NET 9 backend
- Marketplace integration services (example: Etsy)
- Listing publishing and management endpoints
- Designed for cloud deployment (App Service / Key Vault friendly)

## Prerequisites
- .NET 9 SDK (https://dotnet.microsoft.com)
- Visual Studio 2022/2023 or VS Code (recommended)
- Node.js (only required if you use front-end toolchains)

## Getting started (local development)
1. Clone the repository:
   `git clone <repo-url>`
2. Restore and build:
   `dotnet restore`
   `dotnet build`
3. Run the solution (from solution folder):
   `dotnet run --project EasyVend`
4. Open the Blazor WebAssembly client in your browser (the run output will show the URL).

## Configuration
- Local settings and secrets should not be committed. Use `appsettings.Development.Local.json` or user secrets for credentials.
- Consider Azure Key Vault or environment variables for production secrets.

## Testing
- Run unit tests with:
  `dotnet test`

## Publishing
- Publish the server app:
  `dotnet publish -c Release -o ./publish`
- For Blazor WebAssembly client, publish from the client project or as part of the solution publish.

## Contributing
- Create a feature branch for each change: `git checkout -b feat/your-change`
- Open a pull request and describe the purpose and scope of your change
- Keep secrets out of commits; add sensitive files to `.gitignore`

## License
This repository does not include a license file. Add a `LICENSE` if you want to permit reuse.

## Contact
For questions or issues, open an issue on the repository or reach out to the maintainers.