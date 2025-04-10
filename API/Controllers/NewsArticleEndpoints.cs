using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.ModelsDTO;

namespace API.Endpoints;

public static class NewsArticleEndpoints
{
    public static void MapNewsArticleEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/articles")
            .WithTags("Articles")
            .RequireAuthorization();

        // Get paginated articles
        group.MapGet("/", GetPaginatedArticles)
            .WithName("GetPaginatedArticles")
            .WithOpenApi();

        // Get article by ID
        group.MapGet("/{id}", GetArticleById)
            .WithName("GetArticleById")
            .AllowAnonymous()
            .WithOpenApi();

        // Update article
        group.MapPut("/{id}", UpdateArticle)
            .WithName("UpdateArticle")
            .WithOpenApi();

        // Create article
        group.MapPost("/", CreateArticle)
            .WithName("CreateArticle")
            .WithOpenApi();

        // Delete article
        group.MapDelete("/{id}", DeleteArticle)
            .WithName("DeleteArticle")
            .WithOpenApi();
    }

    private static async Task<IResult> GetPaginatedArticles(
        int pageNumber = 1,
        int pageSize = 10,
        AppDBContext db = null!)
    {
        // Validate pagination parameters
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        // Get total count and paginated data in parallel
        var totalItemsTask = db.NewsArticles.CountAsync();

        var articlesTask = db.NewsArticles
            .AsNoTracking()
            .OrderByDescending(n => n.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        await Task.WhenAll(totalItemsTask, articlesTask);

        var totalItems = await totalItemsTask;
        var articles = await articlesTask;

        var paginatedResult = new PaginatedResult<NewsArticle>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = articles
        };

        return TypedResults.Ok(paginatedResult);
    }

    private static async Task<Results<Ok<NewsArticle>, NotFound>> GetArticleById(
        int id,
        AppDBContext db)
    {
        return await db.NewsArticles
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id)
            is NewsArticle model
                ? TypedResults.Ok(model)
                : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> UpdateArticle(
        int id,
        NewsArticle newsArticle,
        AppDBContext db)
    {
        // Don't update the ID field
        var affected = await db.NewsArticles
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Title, newsArticle.Title)
                .SetProperty(m => m.Content, newsArticle.Content)
                .SetProperty(m => m.UpdatedAt, DateTime.UtcNow) // Set current time
                .SetProperty(m => m.AuthorId, newsArticle.AuthorId)
            );

        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<IResult> CreateArticle(
        NewsArticle newsArticle,
        AppDBContext db)
    {
        // Set creation timestamp
        newsArticle.CreatedAt = DateTime.UtcNow;
        newsArticle.UpdatedAt = DateTime.UtcNow;

        db.NewsArticles.Add(newsArticle);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/articles/{newsArticle.Id}", newsArticle);
    }

    private static async Task<Results<Ok, NotFound>> DeleteArticle(
        int id,
        AppDBContext db)
    {
        var affected = await db.NewsArticles
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();

        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
