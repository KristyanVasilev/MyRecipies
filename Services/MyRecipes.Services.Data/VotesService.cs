﻿namespace MyRecipes.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using MyRecipes.Data.Common.Repositories;
    using MyRecipes.Data.Models;

    public class VotesService : IVoteService
    {
        private readonly IRepository<Vote> votesRepository;

        public VotesService(IRepository<Vote> votesRepository)
        {
            this.votesRepository = votesRepository;
        }

        public double GetAverageVote(int recipeId)
            => this.votesRepository.All()
               .Where(x => x.RecipeId == recipeId)
               .Average(x => x.Value);

        public async Task SetVoteAsync(int recipeId, string userId, byte value)
        {
            var vote = this.votesRepository.All()
                .FirstOrDefault(x => x.RecipeId == recipeId && x.UserId == userId);

            if (vote == null)
            {
                vote = new Vote
                {
                    RecipeId = recipeId,
                    UserId = userId,
                };

                await this.votesRepository.AddAsync(vote);
            }

            vote.Value = value;

            await this.votesRepository.SaveChangesAsync();
        }
    }
}
