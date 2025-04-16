using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ASM.Models;
using ASM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // SEED CATEGORIES
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Electronics" },
                    new Category { CategoryName = "Fashion" }
                );
                await context.SaveChangesAsync();
            }

            // Lấy RoleManager và UserManager
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // SEED ROLES
            string[] roles = new string[] { "Customer", "Employee", "Manager" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // SEED USERS
            async Task SeedUser(string email, string role, string name)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = name,
                        Address = $"{role} Address",
                        TinhTrangHoatDong = "on"
                    };

                    var result = await userManager.CreateAsync(user, "Pass@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            await SeedUser("customer1@gmail.com", "Customer", "Customer User");
            await SeedUser("employee@gmail.com", "Employee", "Employee User");
            await SeedUser("manager1@gmail.com", "Manager", "Manager User");

            await SeedProductsCombos(context);
            await SeedCartsOrders(context, userManager);
        }
    }

    private static async Task SeedProductsCombos(ApplicationDbContext context)
    {
        var electronics = context.Categories.FirstOrDefault(c => c.CategoryName == "Electronics");
        var fashion = context.Categories.FirstOrDefault(c => c.CategoryName == "Fashion");

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { ProductName = "Laptop", Price = 1000, Quantity = 10, CategoryID = electronics?.CategoryID, TinhTrang = "on" },
                new Product { ProductName = "Smartphone", Price = 500, Quantity = 20, CategoryID = electronics?.CategoryID, TinhTrang = "on" },
                new Product { ProductName = "Headphones", Price = 150, Quantity = 50, CategoryID = electronics?.CategoryID, TinhTrang = "off" },
                new Product { ProductName = "Smartwatch", Price = 200, Quantity = 30, CategoryID = electronics?.CategoryID, TinhTrang = "on" },
                new Product { ProductName = "T-Shirt", Price = 20, Quantity = 100, CategoryID = fashion?.CategoryID, TinhTrang = "on" },
                new Product { ProductName = "Jeans", Price = 40, Quantity = 70, CategoryID = fashion?.CategoryID, TinhTrang = "off" },
                new Product { ProductName = "Sneakers", Price = 80, Quantity = 60, CategoryID = fashion?.CategoryID, TinhTrang = "on" }
            );

            await context.SaveChangesAsync();
        }

        if (!context.Combos.Any())
        {
            var laptop = context.Products.FirstOrDefault(p => p.ProductName == "Laptop");
            var smartphone = context.Products.FirstOrDefault(p => p.ProductName == "Smartphone");
            var headphones = context.Products.FirstOrDefault(p => p.ProductName == "Headphones");
            var tshirt = context.Products.FirstOrDefault(p => p.ProductName == "T-Shirt");
            var jeans = context.Products.FirstOrDefault(p => p.ProductName == "Jeans");
            var sneakers = context.Products.FirstOrDefault(p => p.ProductName == "Sneakers");

            var techCombo = new Combo
            {
                ComboName = "Tech Combo",
                Price = 1600,
                Description = "Laptop + Smartphone + Headphones"
            };
            context.Combos.Add(techCombo);
            await context.SaveChangesAsync();

            context.ComboDetails.AddRange(
                new ComboDetail { ComboId = techCombo.ComboId, ProductId = laptop.ProductID, Quantity = 1 },
                new ComboDetail { ComboId = techCombo.ComboId, ProductId = smartphone.ProductID, Quantity = 1 },
                new ComboDetail { ComboId = techCombo.ComboId, ProductId = headphones.ProductID, Quantity = 1 }
            );

            var fashionCombo = new Combo
            {
                ComboName = "Style Pack",
                Price = 120,
                Description = "T-Shirt + Jeans + Sneakers"
            };
            context.Combos.Add(fashionCombo);
            await context.SaveChangesAsync();

            context.ComboDetails.AddRange(
                new ComboDetail { ComboId = fashionCombo.ComboId, ProductId = tshirt.ProductID, Quantity = 1 },
                new ComboDetail { ComboId = fashionCombo.ComboId, ProductId = jeans.ProductID, Quantity = 1 },
                new ComboDetail { ComboId = fashionCombo.ComboId, ProductId = sneakers.ProductID, Quantity = 1 }
            );

            await context.SaveChangesAsync();
        }
    }


    private static async Task SeedCartsOrders(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var customer = await userManager.FindByEmailAsync("customer1@gmail.com");
        if (customer == null) return;

        if (!context.Carts.Any())
        {
            var cart = new Cart
            {
                CustomerId = customer.Id,
                CreatedDate = DateTime.Now
            };
            context.Carts.Add(cart);
            await context.SaveChangesAsync();

            var product = context.Products.FirstOrDefault();
            var combo = context.Combos.FirstOrDefault();

            context.CartDetails.AddRange(
                new CartDetail
                {
                    CartId = cart.CartId,
                    ProductId = product?.ProductID,
                    Quantity = 1,
                    UnitPrice = product?.Price ?? 0
                },
                new CartDetail
                {
                    CartId = cart.CartId,
                    ComboId = combo?.ComboId,
                    Quantity = 1,
                    UnitPrice = combo?.Price ?? 0
                }
            );

            await context.SaveChangesAsync();
        }

        if (!context.Orders.Any())
        {
            var order = new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.Now,
                TotalAmount = 1040,
                Status = "Pending"
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var product = context.Products.FirstOrDefault();
            context.OrderDetails.Add(
                new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = product?.ProductID,
                    Quantity = 2,
                    UnitPrice = product?.Price ?? 0
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
