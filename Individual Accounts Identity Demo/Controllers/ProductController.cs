﻿using Individual_Accounts_Identity_Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Individual_Accounts_Identity_Demo.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _repo;

        public ProductController(IProductRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["IDSortParam"] = sortOrder == "ID" ? "id_desc" : "ID";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CategorySortParam"] = sortOrder == "CategoryID" ? "category-id_desc" : "CategoryID";
            ViewData["OnSaleSortParam"] = sortOrder == "OnSale" ? "on-sale_desc" : "OnSale";
            ViewData["StockLevelSortParam"] = sortOrder == "StockLevel" ? "stock-level_desc" : "StockLevel";
            ViewData["CurrentFilter"] = searchString;

            var products = _repo.GetAllProducts();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }

            products = sortOrder switch
            {
                "ID" => products.OrderBy(s => s.ProductID),
                "id_desc" => products.OrderByDescending(s => s.ProductID),
                "Name" => products.OrderBy(s => s.Name),
                "name_desc" => products.OrderByDescending(s => s.Name),
                "Price" => products.OrderBy(s => s.Price),
                "price_desc" => products.OrderByDescending(s => s.Price),
                "CategoryID" => products.OrderBy(s => s.CategoryID),
                "category-id_desc" => products.OrderByDescending(s => s.CategoryID),
                "OnSale" => products.OrderBy(s => s.OnSale),
                "on-sale_desc" => products.OrderByDescending(s => s.OnSale),
                "StockLevel" => products.OrderBy(s => s.StockLevel),
                "stock-level_desc" => products.OrderByDescending(s => s.StockLevel),
                "" => products.OrderBy(s => s.Name),
                _ => products.OrderBy(s => s.Name)
            };
            return View(products);
        }

        [Authorize]
        public IActionResult ViewProduct(int id)
        {
            var product = _repo.GetProduct(id);

            return View(product);
        }

        [Authorize]
        public IActionResult UpdateProduct(int id)
        {
            Product prod = _repo.GetProduct(id);

            _repo.UpdateProduct(prod);

            if (prod == null)
            {
                return View("ProductNotFound");
            }

            return View(prod);
        }

        [Authorize]
        public IActionResult UpdateProductToDatabase(Product product)
        {
            _repo.UpdateProduct(product);

            return RedirectToAction("ViewProduct", new { id = product.ProductID });
        }

        [Authorize]
        public IActionResult InsertProduct()
        {
            var prod = _repo.AssignCategory();

            return View(prod);
        }

        [Authorize]
        public IActionResult InsertProductToDatabase(Product productToInsert)
        {
            _repo.InsertProduct(productToInsert);

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult DeleteProduct(Product product)
        {
            _repo.DeleteProduct(product);

            return RedirectToAction("Index");
        }

    }
}
