# ManufacturingApp API
This project is a Chemical Recipes manufacturing-related application using .NET 8, which follows a RESTful approach. The application manages entities such as RawMaterial, Supplier, Recipe, and Product, with their respective relationships, based on the assessment from [Manufacturing App Assessment](https://github.com/zepeda-luis/ManufacturingApp)

## Prerequisites
To run this project, ensure you have the following installed:

-   .NET 8 SDK
-   SQL Server
-   Visual Studio 2022 || VSCode

## Getting Started

### 1.  Clone the Repository
```sh
git clone https://github.com/diego-devs/ManufacturingApp.git
cd ManufacturingApp
```
### 2. Setup the Database

-   Ensure your SQL Server is running and accessible.
-   Update the connection string in ``appsettings.json`` to point to your SQL Server instance:
```json
{
  "ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ManufacturingApp;Encrypt=False;Trusted_Connection=True;"
    }
}
```
### 3. Apply Migrations:
   
Open the ``Package Manager Console`` in Visual Studio and run the following commands:

```sh
Update-Database
```

Or you can also accomplish this by executing the following command in the .NET command line interface:
```sh
dotnet ef database update
```
This will apply the migrations and create the database schema.

### 4. Build and Run the Application

-   If you are in Visual Studio, Set ``ManufacturingApp.API`` as the startup project.
-   Press ``F5`` to build and run the application.
### 5. Access the Swagger UI
Once the application is running, navigate to the following URL in your browser:

```
https://localhost:7221/swagger/index.html
```
The Swagger UI will provide an interactive interface for testing the API endpoints.

## API Endpoints

### RawMaterial Endpoints

| HTTP Method | Endpoint                        | Description                                    |
|-------------|---------------------------------|------------------------------------------------|
| **GET**         | /api/rawmaterials               | Retrieves all raw materials                    |
| **GET**         | /api/rawmaterials/{id}          | Retrieves a specific raw material by ID        |
| **POST**        | /api/rawmaterials               | Creates a new raw material                     |
| **PUT**         | /api/rawmaterials/{id}          | Updates an existing raw material               |
| **DELETE**      | /api/rawmaterials/{id}          | Deletes a raw material by ID                   |

### Supplier Endpoints

| HTTP Method | Endpoint                        | Description                                    |
|-------------|---------------------------------|------------------------------------------------|
| **GET**         | /api/suppliers                  | Retrieves all suppliers                        |
| **GET**         | /api/suppliers/{id}             | Retrieves a specific supplier by ID            |
| **POST**        | /api/suppliers                  | Creates a new supplier                         |
| **PUT**         | /api/suppliers/{id}             | Updates an existing supplier                   |
| **DELETE**      | /api/suppliers/{id}             | Deletes a supplier by ID                       |

### Recipe Endpoints

| HTTP Method | Endpoint                        | Description                                    |
|-------------|---------------------------------|------------------------------------------------|
| **GET**         | /api/recipes                    | Retrieves all recipes                          |
| **GET**         | /api/recipes/{id}               | Retrieves a specific recipe by ID              |
| **POST**        | /api/recipes                    | Creates a new recipe                           |
| **PUT**         | /api/recipes/{id}               | Updates an existing recipe                     |
| **DELETE**      | /api/recipes/{id}               | Deletes a recipe by ID                         |

### Product Endpoints

| HTTP Method | Endpoint                        | Description                                    |
|-------------|---------------------------------|------------------------------------------------|
| **GET**         | /api/products                   | Retrieves all products                         |
| **GET**         | /api/products/{id}              | Retrieves a specific product by ID             |
| **POST**        | /api/products                   | Creates a new product                          |
| **PUT**         | /api/products/{id}              | Updates an existing product                    |
| **DELETE**      | /api/products/{id}              | Deletes a product by ID                        |

### Optimization Endpoint

| HTTP Method | Endpoint                        | Description                                    |
|-------------|---------------------------------|------------------------------------------------|
| POST        | /api/recipes/optimizeSuppliers  | Calculates the optimal combination of suppliers for a given recipe to minimize the total cost of ingredients. Requires a recipe ID in the request body |

## Sample Data
Optionally, you can execute the InitializeSampleData.sql file located in 
ManufacturingApp.API/Data/ folder. 

This will create the needed objects for the example recipe from [the Manufacturing Assessment](https://github.com/zepeda-luis/ManufacturingApp).

## Testing

You can use the Swagger UI to interact with and test the API endpoints, or any other tool like Postman to test the API endpoints. 

The **Unit Tests project** is still in development on a parallel branch called ``develop``.

