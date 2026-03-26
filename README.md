# LMS.Assessment.Api

An API for managing law firm details.

## Requirements

- .NET 10
- Node 24+

## Setup

```sh
git clone git@github.com:markr-lms/fullstack-assessment.git 
cd fullstack-assessment 
code .
```

### API

Run API
```sh
dotnet restore
dotnet tool restore
cd LMS.Assessment.Api
dotnet run
```

Run tests

`dotnet test`

Run mutation tests

`dotnet stryker`


### UI

```sh
cd lms-assessment-ui
npm i
npm run dev
```
