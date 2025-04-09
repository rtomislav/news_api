using Microsoft.EntityFrameworkCore;
using API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

public static class AuthorEndpoints
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder routes)
    {

        var group = routes.MapGroup("/api/Author").WithTags(nameof(Author));
        group.RequireAuthorization();

        group.MapGet("/GetCurrentUserId", async (HttpContext httpContext, [FromServices] UserManager<IdentityUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(httpContext.User);

            return user == null ? Results.NotFound() : Results.Ok(user.Id);
        });

        group.MapGet("/", async (AppDBContext db) =>
        {
            return await db.Authors.ToListAsync();
        })
        .WithName("GetAllAuthors")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Author>, NotFound>> (string idauthors, AppDBContext db) =>
        {
            return await db.Authors.AsNoTracking()
                .FirstOrDefaultAsync(model => model.IdAuthors == idauthors)
                is Author model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetAuthorById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (string idauthors, Author author, AppDBContext db) =>
        {
            var affected = await db.Authors
                .Where(model => model.IdAuthors == idauthors)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.IdAuthors, author.IdAuthors)
                    .SetProperty(m => m.DisplayName, author.DisplayName)
                    .SetProperty(m => m.Bio, author.Bio)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateAuthor")
        .WithOpenApi();

        group.MapPost("/", async (Author author, AppDBContext db) =>
        {
            db.Authors.Add(author);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Author/{author.IdAuthors}", author);
        })
        .WithName("CreateAuthor")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (string idauthors, AppDBContext db) =>
        {
            var affected = await db.Authors
                .Where(model => model.IdAuthors == idauthors)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteAuthor")
        .WithOpenApi();
    }
}
