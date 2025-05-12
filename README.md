ProductApiMvc – Instructions for Running the Project

Project Summary: 
This is an ASP.NET MVC application that exposes a RESTful API to manage products using in-memory storage.  
You can perform Create, Read, Update, and Delete operations, along with filtering and pagination.

 How to Run This Project?

1. Open the Solution
   - Open ProductApiMvc.sln in **Visual Studio 2019**.

2. Build and Run
   - Press F` to run the project in a browser.
   - The API endpoints can be tested using tool like **Postman**.
      
3. API Endpoints:

I. Create
Method: POST
URL: /ProductApi/Create
Notes: Send product JSON in request body

II. Read (All Products)
Method: GET
URL: /ProductApi/List
Notes: Supports filtering and pagination

III. Read (Products by Pagination)
Method: GET
URL: /ProductApi/List?Page=1&PageSize=5
Notes: Product ID in the URL

IV. Update
Method: POST
URL: /ProductApi/Update?id=1
Notes: Send updated product JSON in request body

V. Delete
Method: POST
URL: /ProductApi/Delete?id=1
Notes: Product ID in the URL

 Sample JSON for Create/Update

json
{
  "Name": "Sample Product",
  "Category": "Electronics",
  "Quantity": 10,
  "Price": 49.99
}

 Project Structure

- Controllers/ProductApiController.cs – RESTful API logic
- Models/Product.cs – Product entity
- Models/ProductFilter.cs – Filtering/pagination logic




