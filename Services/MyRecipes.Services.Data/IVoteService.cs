﻿namespace MyRecipes.Services.Data
{
    using System.Threading.Tasks;

    public interface IVoteService
    {
        Task SetVoteAsync(int recipeId, string userId, byte value);

        double GetAverageVote(int recipeId);
    }
}
