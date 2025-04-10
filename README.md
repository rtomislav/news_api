# 📚 ASP.NET Core Web API – Authors & News Articles

An **ASP.NET Core Web API** project that manages **Authors** and **News Articles** with integrated **authentication and authorization** using **Identity**.

---

## 🚀 Features

- 🔐 JWT-based Authentication using ASP.NET Core Identity
- 🧑‍💼 Endpoints for managing **Authors** (`/api/Author`)
- 📰 Endpoints for managing **News Articles** (`/api/NewsArticle`)
- 📄 Pagination support for News Articles
- 🧪 Swagger UI with OAuth2 security support
- 🛠️ Built with **Entity Framework Core** and **SQL Server**

---

## 📦 Endpoints Overview

### 🔒 `/api/Author` (Requires Auth)

- `GET /GetCurrentUserId` – Returns current user's ID
- `GET /` – Get all authors
- `GET /{id}` – Get author by ID
- `POST /` – Create a new author
- `PUT /{id}` – Update an author
- `DELETE /{id}` – Delete an author

### 📰 `/api/NewsArticle`

- `GET /?pageNumber=1&pageSize=10` – Get paginated articles
- `GET /{id}` – Get article by ID _(Public)_
- `POST /` – Create a new article
- `PUT /{id}` – Update an article

---

## 📌 TODO

- [ ] Implement image upload for articles
- [ ] Add docker support
- [ ] Add env configuration, keyvalut, insights etc...
- [ ] Add logic for saving content from WYSIWYG editor
- [ ] Apply consistent **date handling** with `DateTime.UtcNow` for created/updated timestamps
- [ ] Add **global error handling middleware**
- [ ] Introduce a base response DTO or wrapper for consistent API responses
- [ ] Support **role-based authorization** for certain article actions (e.g. only authors can edit/delete their own articles)
- [ ] Improve author create when registering user
