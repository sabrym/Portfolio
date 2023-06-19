## Framework & languages
This project uses
* .Net6.0 / C#

## Available scripts
- `dotnet build` - To build the relevant dependencies
- `dotnet run` - Execute in the server folder (Portfolio.Api), to start the server 
- `dotnet test` - To execute the unit tests (Portfolio.Services.Tests)

## Implementation Summary
The system has been designed exposing a GET endpoint i.e. /Reporting which takes in a DateTime object as a parameter.
Based on this DateTime object, files containing monthly trades are imported which are within the 12 month time frame of the Date.
Then for each unique symbol imported, we will use the https://www.alphavantage.co/ API to get the relevant trading information. This information is then processed along with the imported data from files to generate a report.

The generated report is then served as the response to the GET endpoint.

## Future improvements
- Background Service to get the daily trade data and store, possiblity a documentDB
- Additional background service that imports file to storage. e.g. Daily
- When report endpoint is called, it will make use of the above two storage aspects and will generate the report.