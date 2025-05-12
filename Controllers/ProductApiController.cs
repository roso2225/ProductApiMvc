using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ProductApiMvc.Models;

namespace ProductApiMvc.Controllers
{
    public class ProductApiController : Controller
    {
        // In-memory product list and auto-incrementing ID
        private static List<Product> _products = new List<Product>();
        private static int _nextId = 1;

        #region CREATE
        /// Creates a new product.
        /// POST: /ProductApi/Create
        [HttpPost]
        public JsonResult Create(Product product)
        {
            try
            {
                // Check if product ID already exists
                if (_products.Any(p => p.ProductID == product.ProductID))
                {
                    Response.StatusCode = 400; // Bad Request
                    return Json(new { error = "Product ID already exists" });
                }
                // Validate Product Price
                if (product.Price < 0)
                {
                    Response.StatusCode = 400; // Bad Request
                    return Json(new { error = "Price must be non-negative" });
                }
                // Validate Product Quantity
                if (product.Quantity < 0)
                {
                    Response.StatusCode = 400; // Bad Request
                    return Json(new { error = "Quantity must be non-negative" });
                }
                // Check if ModelState is valid
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400; // Bad Request
                    return Json(new
                    {
                        error = "Invalid product data",
                        details = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }
                // Assign a new ID and add the product to the list
                product.ProductID = _nextId++;
                _products.Add(product);
                Response.StatusCode = 201; // Created
                return Json(product);
            }
            catch (Exception ex)
            {
                LogError("Error occurred while creating product: " + ex.Message);
                Response.StatusCode = 500; // Internal Server Error
                return Json(new { error = "An unexpected error occurred", details = ex.Message });
            }
        }
        #endregion

        #region READ - LIST
        /// Retrieves all the list of products.
        /// GET: /ProductApi/List
        /// Retrieves a filtered and paginated list of products.
        /// GET:/ProductApi/List?Page=1&PageSize=5  
        [HttpGet]
        public JsonResult List(ProductFilter filter)
        {
            try
            {
                var query = _products.AsQueryable();
                // Apply category filter if provided
                if (!string.IsNullOrWhiteSpace(filter.Category))
                    query = query.Where(p => p.Category.Equals(filter.Category, StringComparison.OrdinalIgnoreCase));
                // Apply price range filter if provided
                if (filter.MinPrice.HasValue)
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);
                if (filter.MaxPrice.HasValue)
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                var total = query.Count();
                var items = query.OrderBy(p => p.ProductID).Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
                return Json(new { total, data = items }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError("Error occurred while retrieving product list: " + ex.Message);
                Response.StatusCode = 500; // Internal Server Error
                return Json(new { error = "An unexpected error occurred", details = ex.Message });
            }
        }
        #endregion

        #region UPDATE
        /// Updates an existing product by ID.
        /// POST: /ProductApi/Update?id=1
        [HttpPost]
        public JsonResult Update(int id, Product updated)
        {
            try
            {
                var existing = _products.FirstOrDefault(p => p.ProductID == id);
                if (existing == null)
                {
                    Response.StatusCode = 404; // Not Found
                    return Json(new { error = "Product not found" });
                }
                // Validate Product Price
                if (updated.Price < 0)
                {
                    Response.StatusCode = 400; // Bad Request
                    return Json(new { error = "Price must be non-negative" });
                }
                // Validate Product Quantity
                if (updated.Quantity < 0)
                {
                    Response.StatusCode = 400; // Bad Request
                    return Json(new { error = "Quantity must be non-negative" });
                }
                // Validate ModelState
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400; // Bad Request
                    return Json(new
                    {
                        error = "Invalid product data",
                        details = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }
                existing.Name = updated.Name;
                existing.Category = updated.Category;
                existing.Quantity = updated.Quantity;
                existing.Price = updated.Price;
                return Json(existing);
            }
            catch (Exception ex)
            {
                LogError("Error occurred while updating product: " + ex.Message);
                Response.StatusCode = 500; // Internal Server Error
                return Json(new { error = "An unexpected error occurred", details = ex.Message });
            }
        }
        #endregion

        #region DELETE
        /// Deletes a product by ID.
        /// POST: /ProductApi/Delete?id=1
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var product = _products.FirstOrDefault(p => p.ProductID == id);

                if (product == null)
                {
                    Response.StatusCode = 404; // Not Found
                    return Json(new { error = "Product not found" });
                }
                _products.Remove(product);
                return Json(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                LogError("Error occurred while deleting product: " + ex.Message);
                Response.StatusCode = 500; // Internal Server Error
                return Json(new { error = "An unexpected error occurred", details = ex.Message });
            }
        }
        #endregion


        // Helper method to log errors to a text file
        private void LogError(string message)
        {
            try
            {
                string logFilePath = Server.MapPath("~/logs/errorlog.txt");
                string directory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                // Append the error message to the log file
                string logMessage = $"{DateTime.Now}: {message}";
                System.IO.File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error writing to log file: " + ex.Message);
            }
        }
    }
}
