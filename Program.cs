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
                    Console.WriteLine("4) Display a specific Category and related products");
                    Console.WriteLine("5) Display all Categories and their related products");
                    Console.WriteLine("6) Edit a Product");
                    Console.WriteLine("7) Display a Product");
                    Console.WriteLine("8) Display all Products");
                    Console.WriteLine("9) Edit a Category");
                    Console.WriteLine("10) Display all Categories and their related active products");

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
                                //save category to db
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

                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            var db = new NWConsole_48_JAGContext();
                            // check for unique name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                //save products to db
                                db.AddProduct(product);
                                logger.Info($"Product added - {product.ProductName}");
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
                    else if (choice == "6")
                    {
                        var db = new NWConsole_48_JAGContext();
                        Console.Write("Enter the Product to edit: ");
                        var product = GetProduct(db);
                        if (product != null)
                        {
                            Product UpdatedProduct = InputProduct(db);
                            if (UpdatedProduct != null)
                            {
                                UpdatedProduct.ProductId = product.ProductId;
                                Console.WriteLine("What would you like to update?\n1) Product Name\n2) SupplierId\n3) CategoryId\n4) Quantity Per Unit\n5) Unit Price\n6) Units in stock\n7) Units on order\n8) Reorder level\n9) Discontinued");
                                int choice2 = int.Parse(Console.ReadLine());
                                Console.Clear();
                                logger.Info($"Option {choice2} selected");
                                if (choice2 == 1) 
                                {
                                    Console.Write("Enter new product name: ");
                                    UpdatedProduct.ProductName = Console.ReadLine();
                                }
                                if (choice2 == 2) 
                                {
                                    Console.Write("Enter new supplier ID: ");
                                    UpdatedProduct.SupplierId = int.Parse(Console.ReadLine());
                                }
                                if (choice2 == 3) 
                                {
                                    Console.Write("Enter new catergory ID: ");
                                    UpdatedProduct.CategoryId = int.Parse(Console.ReadLine());
                                }
                                if (choice2 == 4) 
                                {
                                    Console.Write("Enter new quantity per unit: ");
                                    UpdatedProduct.QuantityPerUnit = Console.ReadLine();
                                }
                                if (choice2 == 5) 
                                {
                                    Console.Write("Enter new unit price: ");
                                    UpdatedProduct.UnitPrice = int.Parse(Console.ReadLine());
                                }
                                if (choice2 == 6) 
                                {
                                    Console.Write("Enter new number of units in stock: ");
                                    UpdatedProduct.UnitsInStock = short.Parse(Console.ReadLine());
                                }
                                if (choice2 == 7) 
                                {
                                    Console.Write("Enter new number of units on order: ");
                                    UpdatedProduct.UnitsOnOrder = short.Parse(Console.ReadLine());
                                }
                                if (choice2 == 8) 
                                {
                                    Console.Write("Enter new reorder level: ");
                                    UpdatedProduct.ReorderLevel = short.Parse(Console.ReadLine());
                                }
                                if (choice2 == 9) 
                                {
                                    Console.Write("Is it discontinued? (0 = no   1 = yes): ");
                                    int tempDesc = int.Parse(Console.ReadLine());
                                    if (tempDesc == 0){
                                        product.Discontinued = false;
                                }
                                else 
                                {
                                    product.Discontinued = true;
                                }
                                }
                                
                                    db.EditProduct(UpdatedProduct);
                                    logger.Info($"Product (id: {product.ProductId}) updated");
                                }
                        }
                    }
                    else if (choice == "7")
                    {
                        var db = new NWConsole_48_JAGContext();
                        string view;
                        Console.WriteLine("What product would you like to view?");
                        view = Console.ReadLine();
                        Console.Clear();
                        logger.Info($"{view} selected");
                        var query = db.Products;
                        foreach (var item in query)
                        {
                            if (item.ProductName == view)
                            {
                                int id = item.ProductId;
                                if(item.ProductId == id)
                                {
                                    Console.WriteLine($"Product Id: {item.ProductId} \nProduct Name: {item.ProductName} \nSupplierId: {item.SupplierId} \nCategoryId: {item.CategoryId} \nQuantity Per Unit: {item.QuantityPerUnit} \nUnit Price: {item.UnitPrice} \nUnits in Stock: {item.UnitsInStock} \nUnits on Order: {item.UnitsOnOrder} \nReorder Level: {item.ReorderLevel} \nDiscontinued: {item.Discontinued}");
                                    Console.WriteLine(" ");
                                }
                                
                            }
                        }
                    }
                    else if (choice == "8")
                    {
                        var db = new NWConsole_48_JAGContext();
                        int choice2;
                        Console.WriteLine("Select one option:\n1) View all products\n2) View discontinued products\n3) View active products");
                        choice2 = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"Option {choice2} selected");
                        

                            var query = db.Products;
                            Console.WriteLine("Discontinued Products are red");
                            foreach (var item in query)
                            {
                                if (choice2 == 1)
                                {
                                    if (item.Discontinued == true)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine(item.ProductName);
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    if (item.Discontinued == false)
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine(item.ProductName);
                                    }
                                }
                                if (choice2 == 2)
                                {
                                    if (item.Discontinued == true)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine(item.ProductName);
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                }
                                if (choice2 == 3)
                                {
                                    if (item.Discontinued == false)
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine(item.ProductName);
                                    }
                                }
                            }
                            if(choice2 != 1 || choice2 != 2 || choice2 != 3){logger.Info("Invalid Choice");}
                        
                        }
                        else if (choice == "9")
                        {
                            var db = new NWConsole_48_JAGContext();
                            Console.Write("Enter the category to edit: ");
                            var query = db.Categories.OrderBy(p => p.CategoryName);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            foreach (var item in query)
                            {
                                Console.WriteLine(item.CategoryName);
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            var category = GetCategory(db);
                            if (category != null)
                            {
                                Category UpdatedCategory = InputCategory(db);
                                if (UpdatedCategory != null)
                                {
                                    UpdatedCategory.CategoryId = category.CategoryId;
                                    Console.WriteLine("Enter new Category Name");
                                    UpdatedCategory.CategoryName = Console.ReadLine();
                                    Console.WriteLine("Enter new Category Description");
                                    UpdatedCategory.Description = Console.ReadLine();
                                    logger.Info($"Category {UpdatedCategory.CategoryName} updated");
                                }
                            }
                        }
                        else if (choice == "10")
                            {
                                var db = new NWConsole_48_JAGContext();
                                var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.CategoryName}");
                                    foreach (Product p in item.Products)
                                    {
                                        if (p.Discontinued == false)
                                        {
                                            Console.WriteLine($"\t{p.ProductName}");
                                        }
                                        
                                    }
                                }
                            }
                        Console.WriteLine();













                
                }while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            logger.Info("Program ended");
        }
        public static Category InputCategory (NWConsole_48_JAGContext db)
        {
            Category category = new Category();
            return category;
        }
        public static Product InputProduct(NWConsole_48_JAGContext db)
        {
            
            Product product = new Product();
            /*
            Console.WriteLine("Enter the Product name");
            product.ProductName = Console.ReadLine();

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(p => p.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Product name exists", new string[] { "Name" }));
                }
                else
                {
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }*/
            return product;
        }
        public static Product GetProduct(NWConsole_48_JAGContext db)
        {
            var products = db.Products.OrderBy(p => p.ProductId);
            foreach (Product p in products)
            {
                Console.WriteLine($"{p.ProductId}: {p.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }
        public static Category GetCategory(NWConsole_48_JAGContext db)
        {
            var category = db.Categories.OrderBy(p => p.CategoryId);
            foreach (Category c in category)
            {
                Console.WriteLine($"{c.CategoryId}: {c.CategoryName}");
            }
            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Category category1 = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryId);
                if (category != null)
                {
                    return category1;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }
    }
}