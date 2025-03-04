﻿namespace MyRecipes.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MyRecipes.Data.Models;

    public class CategoriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            await dbContext.Categories.AddAsync(new Category { Name = "Тарт" });
            await dbContext.Categories.AddAsync(new Category { Name = "Кекс" });
            await dbContext.Categories.AddAsync(new Category { Name = "Печено Свинско" });
            await dbContext.Categories.AddAsync(new Category { Name = "Пица" });

            await dbContext.SaveChangesAsync();
        }
    }
}
