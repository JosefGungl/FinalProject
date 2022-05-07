using System;
using NLog.Web;
using System.IO;
using System.Linq;
using FinalProject.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FinalProject
{
    class Program
    {
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                
                string choice ="";
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Add New Product");
                    Console.WriteLine("4) Display Category and related products");
                    Console.WriteLine("5) Display all Categories and their related products");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");

                    if (choice == "1"){
                        var db = new NWConsole_48_JAGContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "2")
                    {
                        Category category = new Category();
                        Console.Write("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.Write("Enter the Category Description:");
                        category.Description = Console.ReadLine();
                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NWConsole_48_JAGContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                // TODO: save category to db
                                db.AddCategory(category);
                                logger.Info($"Category added - {category.CategoryName}");
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        Product product = new Product();
                        Console.Write("Enter Product Name: ");
                        product.ProductName = Console.ReadLine();
                        Console.Write("Enter Supplier ID: ");
                        product.SupplierId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Catergory ID: ");
                        product.CategoryId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Quantity Per Unit: ");
                        product.QuantityPerUnit = Console.ReadLine();
                        Console.Write("Enter Unit Price: ");
                        product.UnitPrice = int.Parse(Console.ReadLine());
                        Console.Write("Enter Number of Units in Stock: ");
                        product.UnitsInStock = short.Parse(Console.ReadLine());
                        Console.Write("Enter Number of Units on Order: ");
                        product.UnitsOnOrder = short.Parse(Console.ReadLine());
                        Console.Write("Enter Reorder Level: ");
                        product.ReorderLevel = short.Parse(Console.ReadLine());
                        Console.Write("Is it discontinued? (0 = no   1 = yes): ");
                        int tempDesc = int.Parse(Console.ReadLine());
                        if (tempDesc == 0){
                            product.Discontinued = false;
                        }else {product.Discontinued = true;}
                        
                    }
                    else if (choice == "4")
                    {
                        var db = new NWConsole_48_JAGContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "5")
                    {
                        var db = new NWConsole_48_JAGContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    Console.WriteLine();













                }
                while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            logger.Info("Program ended");
        }
    }
}
