using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Endpoints;

public static class AuthorEndpoints
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/authors")
            .WithTags("Authors")
            .RequireAuthorization();

        // Get current user ID
        group.MapGet("/current-user-id", GetCurrentUserId)
            .WithName("GetCurrentUserId")
            .WithOpenApi();

        // Get all authors
        group.MapGet("/", GetAllAuthors)
            .WithName("GetAllAuthors")
            .WithOpenApi();

        // Get author by ID
        group.MapGet("/{id}", GetAuthorById)
            .WithName("GetAuthorById")
            .WithOpenApi();

        // Update author
        group.MapPut("/{id}", UpdateAuthor)
            .WithName("UpdateAuthor")
            .WithOpenApi();

        // Create author
        group.MapPost("/", CreateAuthor)
            .WithName("CreateAuthor")
            .WithOpenApi();

        // Delete author
        group.MapDelete("/{id}", DeleteAuthor)
            .WithName("DeleteAuthor")
            .WithOpenApi();
    }

    private static async Task<IResult> GetCurrentUserId(
        HttpContext httpContext,
        [FromServices] UserManager<IdentityUser> userManager)
    {
        var user = await userManager.GetUserAsync(httpContext.User);
        return user == null ? Results.NotFound() : Results.Ok(user.Id);
    }

    private static async Task<IResult> GetAllAuthors(AppDBContext db)
    {
        var authors = await db.Authors.ToListAsync();
        return Results.Ok(authors);
    }

    private static async Task<Results<Ok<Author>, NotFound>> GetAuthorById(
        string id,
        AppDBContext db)
    {
        return await db.Authors.AsNoTracking()
            .FirstOrDefaultAsync(model => model.IdAuthors == id)
            is Author model
                ? TypedResults.Ok(model)
                : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> UpdateAuthor(
        string id,
        Author author,
        AppDBContext db)
    {
        var affected = await db.Authors
            .Where(model => model.IdAuthors == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.DisplayName, author.DisplayName)
                .SetProperty(m => m.Bio, author.Bio)
            );

        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<IResult> CreateAuthor(
        Author author,
        AppDBContext db)
    {
        db.Authors.Add(author);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/api/authors/{author.IdAuthors}", author);
    }

    private static async Task<Results<Ok, NotFound>> DeleteAuthor(
        string id,
        AppDBContext db)
    {
        var affected = await db.Authors
            .Where(model => model.IdAuthors == id)
            .ExecuteDeleteAsync();

        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
