﻿using System;
using System.Collections.Generic;

namespace Prj1
{
    class Program
    {
        static Dictionary<Guid, StoreData> storeDictionary;

        static void Main(string[] args)
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("1. Log in");
                Console.WriteLine("2. Quit");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunApp();
                        break;
                    case "2":
                        isRunning = false;
                        Console.WriteLine("Quitting the program...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

            Console.WriteLine("End!");
        }

        static void RunApp()
        {
            // Create a dictionary to store the products with their quantities
            Dictionary<int, Product> productDictionary = new Dictionary<int, Product>
            {
                { 1, new Product(1, "Milk", 5.99m, "Organic Milk", new Dictionary<StoreData, int>()) },
                { 2, new Product(2, "Bread", 10.99m, "Wheat Bread", new Dictionary<StoreData, int>()) },
                { 3, new Product(3, "Chicken", 20.99m, "BBQ Chicken", new Dictionary<StoreData, int>()) },
            };

            // Create a dictionary to store stores with GUID ID
            storeDictionary = new Dictionary<Guid, StoreData>();

            // Create store objects
            StoreData store1 = new StoreData(Guid.NewGuid(), "Walmart");
            StoreData store2 = new StoreData(Guid.NewGuid(), "Kroger");
            StoreData store3 = new StoreData(Guid.NewGuid(), "HEB");

            // Add store objects to the dictionary
            storeDictionary.Add(store1.StoreId, store1);
            storeDictionary.Add(store2.StoreId, store2);
            storeDictionary.Add(store3.StoreId, store3);

            // Set initial quantities for each store
            foreach (var product in productDictionary.Values)
            {
                product.Quantities[store1] = 10; // Set initial quantity for store1
                product.Quantities[store2] = 15; // Set initial quantity for store2
                product.Quantities[store3] = 20; // Set initial quantity for store3
            }

            bool isLoggedOut = false;
            Guid orderId = Guid.Empty; // Order ID for the entire order
            DateTime orderTime = DateTime.MinValue; // Order time for the entire order

            while (!isLoggedOut)
            {
                Console.WriteLine("Hello there, please enter your first and last name. Enter 'quit' to return to the main menu.");
                string names = Console.ReadLine();

                if (names.ToLower() == "quit")
                {
                    isLoggedOut = true;
                    continue;
                }

                // Divide the string delimited by a space
                string[] namesArr = names.Split(' ');

                bool isValidStore = false;
                StoreData selectedStore = null;

                do
                {
                    Console.WriteLine("Thank you. Please choose a store from the list:");
                    Console.WriteLine("W. Walmart");
                    Console.WriteLine("K. Kroger");
                    Console.WriteLine("H. HEB");

                    string userChoice = Console.ReadLine()?.ToUpper();

                    if (userChoice == "W")
                    {
                        if (storeDictionary.ContainsKey(store1.StoreId))
                        {
                            selectedStore = storeDictionary[store1.StoreId];
                            Console.WriteLine($"{namesArr[0]} {namesArr[1]}, {selectedStore.Name} was selected");
                            isValidStore = true;
                        }
                    }
                    else if (userChoice == "K")
                    {
                        if (storeDictionary.ContainsKey(store2.StoreId))
                        {
                            selectedStore = storeDictionary[store2.StoreId];
                            Console.WriteLine($"{namesArr[0]} {namesArr[1]}, {selectedStore.Name} was selected");
                            isValidStore = true;
                        }
                    }
                    else if (userChoice == "H")
                    {
                        if (storeDictionary.ContainsKey(store3.StoreId))
                        {
                            selectedStore = storeDictionary[store3.StoreId];
                            Console.WriteLine($"{namesArr[0]} {namesArr[1]}, {selectedStore.Name} was selected");
                            isValidStore = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please try again.");
                    }
                } while (!isValidStore);

                List<Order> cart = new List<Order>();
                bool isAddingProducts = true;
                decimal totalAmount = 0m;

                while (isAddingProducts)
                {
                    Console.WriteLine("\nPlease choose a product from the list or enter '0' to checkout:");
                    Console.WriteLine("1. Milk");
                    Console.WriteLine("2. Bread");
                    Console.WriteLine("3. Chicken");

                    string userChoice = Console.ReadLine();

                    if (int.TryParse(userChoice, out int productId) && productDictionary.ContainsKey(productId))
                    {
                        Product selectedProduct = productDictionary[productId];

                        Console.WriteLine($"Enter the quantity of {selectedProduct.Name}:");

                        if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                        {
                            if (selectedProduct.Quantities.ContainsKey(selectedStore) && selectedProduct.Quantities[selectedStore] >= quantity)
                            {
                                selectedProduct.Quantities[selectedStore] -= quantity; // Deduct the selected quantity from the available quantity
                                cart.Add(new Order
                                {
                                    OrderId = orderId, // Use the same Order ID for each order in the cart
                                    Store = selectedStore,
                                    StoreLocation = StoreLocation.GetStoreLocation(selectedStore), // Get the store location from the StoreLocation class
                                    Product = selectedProduct,
                                    Customer = new Customer(namesArr[0], namesArr[1]), 
                                    Quantity = quantity,
                                    OrderTime = orderTime // Use the same Order Time for each order in the cart
                                });

                                Console.WriteLine($"{quantity} {selectedProduct.Name} added to the cart.");
                                totalAmount += selectedProduct.Price * quantity; // Update the total amount
                            }
                            else
                            {
                                Console.WriteLine("Insufficient quantity. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid quantity. Please try again.");
                        }
                    }
                    else if (userChoice == "0")
                    {
                        isAddingProducts = false;
                        orderId = Guid.NewGuid(); // Generate a new Order ID for the next order
                        orderTime = DateTime.Now; // Capture the current Order Time
                        Console.WriteLine("\nCheckout:");
                        Console.WriteLine($"Customer: {namesArr[0]} {namesArr[1]}");
                        Console.WriteLine($"Store: {selectedStore.Name}");
                        Console.WriteLine($"Store Location: {StoreLocation.GetStoreLocation(selectedStore)}");

                        foreach (var order in cart)
                        {
                            Console.WriteLine($"{order.Product.Name} x {order.Quantity} = ${order.Product.Price * order.Quantity}");
                        }

                        Console.WriteLine($"Order ID: {orderId}"); // Display the Order ID for the entire order
                        Console.WriteLine($"Order Time: {orderTime}"); // Display the Order Time for the entire order
                        Console.WriteLine($"Total: ${totalAmount}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please try again.");
                    }
                }
            }
        }
    }
}
