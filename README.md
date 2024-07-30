# AspDotnetApi

## Overview

`AspDotnetApi` is a project designed to explore and learn about ASP.NET. This API project provides user authentication functionalities using local username/password and Microsoft 365 accounts. The primary goal of this project is to gain hands-on experience with ASP.NET and its features.

## Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- Microsoft SQL Server (for future database integration)

### Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/mouad-halli/Asp-Dotnet-Api.git aspDotnetApi
   ```

2. **Navigate to the Project Directory**

   ```bash
   cd aspDotnetApi
   ```
   
4. **Restore Dependencies**

    ```bash
    dotnet restore
    ```
    
### Usage

1. **Build the Project**

   ```bash
   dotnet build
   ```

2. **Run the Project**

   ```bash
   dotnet run
   ```

### Configuration

- ***Connection Strings:*** Update the appsettings.json file with your Microsoft SQL Server connection string.
- ***JWT Token:*** Configure the JWT settings in appsettings.json.
- ***AzureAD:*** Future updates will include Azure Active Directory (AzureAD) configuration.

### API Endpoints

#### Login

- **URL**: `/api/Authentication/login`
- **Method**: `POST`
- **Description**: Authenticates a user.
- **Request Body**:
  ```json
  {
     "email": "string",
     "password": "string",
  }

#### Login

- **URL**: `/api/Authentication/register`
- **Method**: `POST`
- **Description**: Register a user.
- **Request Body**:
  ```json
  {
     "userName": "string",
     "firstName": "string",
     "lastName": "string",
     "email": "string", "user@example.com",
     "password": "string"
  }
  
#### Logout

- **URL**: `/api/Authentication/logout`
- **Method**: `POST`
- **Description**: Logout a user.

### Future Enhancements

- ***AzureAD Integration***: Future versions of this project will include integration with Azure Active Directory for authentication.




  
