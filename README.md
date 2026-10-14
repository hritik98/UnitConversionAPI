# Unit Conversion API

A lightweight ASP.NET Core Web API for converting values across common unit categories such as length, temperature, and weight/mass. The solution is designed to be easy to extend with additional categories and units while keeping the conversion logic predictable and testable.

## Features

- Conversion endpoint for length, temperature, and weight/mass values
- Case-insensitive unit matching
- Validation for unsupported categories or units
- OpenAPI support for local API exploration
- Unit tests covering representative conversions and validation failures

## Project structure

- src/UnitConversion.Api: API entry point and HTTP layer
- src/UnitConversion.Application: application services, contracts, and DTOs
- src/UnitConversion.Domain: domain models and core conversion definitions
- tests/UnitConversion.Api.Tests/Integration: end-to-end API integration tests

This follows a clean, layered architecture that keeps the API thin, the business logic testable, and the domain model reusable as the solution grows.

## Run locally

1. Ensure the .NET 10 SDK is installed.
2. From the repository root, run:

   dotnet restore
   dotnet build
   dotnet run --project src/UnitConversion.Api/UnitConversion.Api.csproj --urls http://localhost:5062

3. The API starts in development mode and is available at http://localhost:5062.
4. OpenAPI metadata is available at http://localhost:5062/openapi/v1.json while running locally.
5. Example request:

   curl -X POST "http://localhost:5062/api/conversion/convert" \
     -H "Content-Type: application/json" \
     -d '{
       "category": "length",
       "fromUnit": "meter",
       "toUnit": "foot",
       "value": 10
     }'

## Example responses

{
  "category": "length",
  "fromUnit": "meter",
  "toUnit": "foot",
  "value": 10,
  "convertedValue": 32.8084
}

## Design decisions

- The conversion system uses a catalog-based approach: each unit is defined alongside its transformation to and from a canonical base value. This keeps the logic centralized and makes adding new categories straightforward.
- Temperature conversions are implemented with the correct offset-aware formulas, which avoids the common mistake of treating temperature as a simple linear scale like distance.
- The API uses a single conversion endpoint with explicit category and unit names so the contract stays clear and easy to extend without introducing fragile branching logic.

## Notes

- This version intentionally hardcodes the supported units to keep the implementation simple and focused on the challenge.
- The project is structured as a team-friendly ASP.NET Core solution using a clean layered design: domain, application, and API responsibilities are separated for maintainability.
