using Microsoft.EntityFrameworkCore;
using API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace API.Controllers;

public static class NewsArticleEndpoints
{
    public static void MapNewsArticleEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/NewsArticle").WithTags(nameof(NewsArticle));
        group.RequireAuthorization();

        group.MapGet("/", async (int pageNumber, int pageSize, AppDBContext db) =>
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var skip = (pageNumber - 1) * pageSize;
            var totalItems = await db.NewsArticles.CountAsync();
            var articles = await db.NewsArticles
                .AsNoTracking()
                .OrderByDescending(n => n.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = articles
            };

            return TypedResults.Ok(result);
        })
   .WithName("GetAllNewsArticles")
   .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<NewsArticle>, NotFound>> (int id, AppDBContext db) =>
        {
            return await db.NewsArticles.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is NewsArticle model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetNewsArticleById")
        .AllowAnonymous()
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, NewsArticle newsArticle, AppDBContext db) =>
        {
            var affected = await db.NewsArticles
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, newsArticle.Id)
                    .SetProperty(m => m.Title, newsArticle.Title)
                    .SetProperty(m => m.Content, newsArticle.Content)
                    .SetProperty(m => m.CreatedAt, newsArticle.CreatedAt)
                    .SetProperty(m => m.UpdatedAt, newsArticle.UpdatedAt)
                    .SetProperty(m => m.AuthorId, newsArticle.AuthorId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateNewsArticle")
        .WithOpenApi();

        group.MapPost("/", async (NewsArticle newsArticle, AppDBContext db) =>
        {
            db.NewsArticles.Add(newsArticle);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/NewsArticle/{newsArticle.Id}", newsArticle);
        })
        .WithName("CreateNewsArticle")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDBContext db) =>
        {
            var affected = await db.NewsArticles
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteNewsArticle")
        .WithOpenApi();
    }
}
