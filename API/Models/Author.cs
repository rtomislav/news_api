﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
namespace API.Models;

public partial class Author
{

    public string IdAuthors { get; set; }

    public string DisplayName { get; set; }

    public string Bio { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}