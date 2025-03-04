﻿namespace MyRecipes.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MyRecipes.Common;
    using MyRecipes.Data.Models;
    using MyRecipes.Services.Data;
    using MyRecipes.Web.ViewModels.Recipes;

    public class RecipesController : Controller
    {
        private readonly ICategoriesService categoriesService;
        private readonly IRecipesService recipesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;

        public RecipesController(
            ICategoriesService categoriesService,
            IRecipesService recipesService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            this.categoriesService = categoriesService;
            this.recipesService = recipesService;
            this.userManager = userManager;
            this.environment = environment;
        }

        [Authorize]
        public IActionResult Create()
        {
            var viewModel = new CreateRecipeInputModel();
            viewModel.CategoriesItems = this.categoriesService.GetCategories();
            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateRecipeInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.CategoriesItems = this.categoriesService.GetCategories();
                return this.View(input);
            }

            var user = await this.userManager.GetUserAsync(this.User);

            // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value; //information from cockies
            try
            {
                await this.recipesService.CreateAsync(input, user.Id, $"{this.environment.WebRootPath}/images");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                return this.View(input);
            }

            this.TempData["Message"] = "Recipe added successfuly!";
            return this.RedirectToAction("All");
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Edit(int id)
        {
            var inputModel = this.recipesService.GetSingleRecipe<EditRecipeViewModel>(id);
            inputModel.Id = id;
            inputModel.CategoriesItems = this.categoriesService.GetCategories();
            return this.View(inputModel);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(int id, EditRecipeViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.Id = id;
                input.CategoriesItems = this.categoriesService.GetCategories();
                return this.View(input);
            }

            await this.recipesService.UpdateAsync(id, input);

            return this.RedirectToAction(nameof(this.SingleRecipe), new { id });
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(int id)
        {
            await this.recipesService.DeleteAsync(id);

            return this.RedirectToAction("All");
        }

        public IActionResult All(int id = 1)
        {
            const int itemsPerPage = 6;
            var viewModel = new RecipeListViewModel
            {
                ItemsPerPage = itemsPerPage,
                PageNumber = id,
                RecipesCount = this.recipesService.GetRecipesCount(),
                Recipes = this.recipesService.GetAll<RecipeInListViewModel>(id, itemsPerPage),
            };

            return this.View(viewModel);
        }

        public IActionResult SingleRecipe(int id)
        {
            var recipe = this.recipesService.GetSingleRecipe<SingleRecipeViewModel>(id);

            return this.View(recipe);
        }
    }
}
