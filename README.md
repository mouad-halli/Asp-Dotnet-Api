# AspDotnetCoreApi

## Overview

`AspDotnetCoreApi` is a project designed to explore and learn about ASP.NET Core. This API project provides user authentication functionalities using cookie and JWT, local username/password and Microsoft accounts with AzureAd. The primary goal of this project is to gain hands-on experience with ASP.NET Core and its features.

## Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- Microsoft SQL Server (for database integration)

### Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/mouad-halli/Asp-Dotnet-Api.git AspDotnetCoreApi
   ```

2. **Navigate to the Project Directory**

   ```bash
   cd AspDotnetCoreApi
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
***NOTE:*** you can use reference.appSettings.json as a reference to configure your appSettings.json.
- ***Connection Strings:*** Update the appsettings.json file with your Microsoft SQL Server connection string.
- ***JWT Token:*** Configure the JWT settings in appsettings.json.
- ***AzureAD:*** Configure AzureAD (openID) settings in appsettings.json.

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

#### Register

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

#### Microsoft Login

- **URL**: `/api/Authentication/microsoft-login`
- **Method**: `GET`
- **Description**: Authenticate user with his microsoft account then he will be redirected to /api/Authentication/microsoft-login-callback.

- **URL**: `/api/Authentication/microsoft-login-callback`
- **Method**: `POST`
- **Description**: After a successfull login with microsoft account user will be redirected here to compleate authentication.

![test](https://github.com/mouad-halli/AspDotnetCore-Api/raw/main/readMeImgs/AzureAd%20OAuth2%20diagram.png)
  
#### Logout

- **URL**: `/api/Authentication/logout`
- **Method**: `GET`
- **Description**: Logout a user.

### Future Enhancements

- ***Code Quality***:
   - Configurations, implementations and Controllers Logic will contain descriptif comments to ensure everything is explained and comprehensible.
   - variables, classes and methods will have readable names.

- ***Configuration***:
   - Creating a Reference Config file ( reference.appSettings.json ) to help in configuration.
          
- ***AzureAD Integration***:
   - **Implemented**:
      - Future versions of this project will include integration with Azure Active Directory for authentication.
        
- ***Authentication and Security***:
   - Add Two-Factor Authentication.
   - Add E-mail Confirmation.
   - Enhance Security by generating JWT Access Token and Refresh Token.
   - **Implemented**:
      - Generate JWT Access Token.
      - Secure Tokens by storing them inside an HTTP-ONLY cookie.




  
