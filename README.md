# CodeQuest Backend

A .NET 8 Web API backend for the CodeQuest platform, featuring user management, post ranking, and engagement tracking.

## Features

- **User Management**: Registration, authentication, and profile management
- **Post System**: Create, read, update, and delete posts with categories and subcategories
- **Ranking System**: Popular and trending post algorithms with background processing
- **Engagement Tracking**: Likes, comments, bookmarks, and visits
- **Real-time Notifications**: WebSocket-based notification system
- **StarDust Points**: Gamification system for user engagement
- **JWT Authentication**: Secure API access with Discord OAuth integration

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (or use Docker)
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

## Quick Start

### 1. Clone and Navigate

```bash
git clone <repository-url>
cd Backend-CodeQuest2025
```

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Database Setup

#### Option A: Using Docker (Recommended)

```bash
# Start PostgreSQL container
docker run --name codequest-postgres -e POSTGRES_PASSWORD=yourpassword -e POSTGRES_DB=codequest -p 5432:5432 -d postgres:15

# Update connection string in appsettings.Development.json
```

#### Option B: Local PostgreSQL

1. Install PostgreSQL locally
2. Create a database named `codequest`
3. Update connection string in `appsettings.Development.json`

### 4. Configure Database Connection

Update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=codequest;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-super-secret-jwt-key-here-must-be-at-least-32-characters",
    "Issuer": "CodeQuest",
    "Audience": "CodeQuestUsers"
  }
}
```

### 5. Run Database Migrations

```bash
dotnet ef database update
```

### 6. Run the Application

```bash
dotnet run
```

The API will be available at:

- HTTP: `http://localhost:5110`
- HTTPS: `https://localhost:7294`
- Swagger UI: `http://localhost:5110/swagger`

## Project Structure

```
Backend-CodeQuest2025/
├── Controllers/           # API Controllers
│   ├── AuthController.cs
│   ├── PostsController.cs
│   ├── RankingController.cs
│   └── ...
├── Data/                  # Database Context
│   └── ApplicationDbContext.cs
├── Models/                # Data Models
│   ├── User.cs
│   ├── Post.cs
│   ├── Dtos/             # Data Transfer Objects
│   └── ...
├── Services/              # Business Logic
│   ├── PostService.cs
│   ├── PopularityService.cs
│   ├── TrendingService.cs
│   └── ...
├── Repository/            # Data Access Layer
│   ├── IRepository/       # Repository Interfaces
│   └── ...
├── Migrations/            # EF Core Migrations
└── Program.cs             # Application Entry Point
```

## Key Services

### Ranking System

- **PopularityService**: Calculates post popularity based on cumulative engagement
- **TrendingService**: Tracks recent activity for trending posts
- **RankingBackgroundService**: Automatically recalculates scores every 15-30 minutes

### Authentication

- JWT-based authentication
- Discord OAuth integration
- User registration and login

### Engagement Tracking

- View tracking
- Like/Unlike system
- Comment system
- Bookmark functionality

## API Endpoints

### Authentication

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/discord` - Discord OAuth login

### Posts

- `GET /api/posts` - Get all posts
- `POST /api/posts` - Create new post
- `GET /api/posts/{id}` - Get specific post
- `PUT /api/posts/{id}` - Update post
- `DELETE /api/posts/{id}` - Delete post

### Ranking

- `GET /api/ranking/popular` - Get popular posts
- `GET /api/ranking/trending` - Get trending posts
- `POST /api/ranking/recalculate-popularity` - Manually recalculate popularity scores

### Engagement

- `POST /api/ranking/record-view` - Record post view
- `POST /api/ranking/record-like` - Record post like
- `POST /api/ranking/record-comment` - Record comment engagement
- `POST /api/ranking/record-bookmark` - Record bookmark

## Configuration

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set to "Development" or "Production"
- `ConnectionStrings__DefaultConnection`: Database connection string
- `Jwt__Key`: JWT secret key (minimum 32 characters)
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audience`: JWT audience

### CORS Configuration

The API is configured to allow requests from:

- `http://localhost:3000`
- `http://localhost:8080`
- `http://localhost:5173`

## Database Schema

### Key Tables

- **Users**: User accounts and profiles
- **Posts**: Blog posts with content and metadata
- **Categories/Subcategories**: Post organization
- **Likes**: User likes on posts
- **Comments**: Post comments
- **Bookmarks**: User bookmarks
- **EngagementEvents**: Activity tracking for trending

### Popularity Algorithm

Posts are ranked by popularity using:

- Views (weight: 1.0)
- Likes (weight: 2.0)
- Comments (weight: 3.0)
- Bookmarks (weight: 1.5)
- Age decay (1% per day)

## Development

### Adding New Features

1. Create model in `Models/` directory
2. Add to `ApplicationDbContext`
3. Create migration: `dotnet ef migrations add FeatureName`
4. Update database: `dotnet ef database update`
5. Create repository interface and implementation
6. Create service for business logic
7. Create controller for API endpoints

### Running Tests

```bash
dotnet test
```

### Code Quality

- Follow C# naming conventions
- Use async/await for database operations
- Implement proper error handling
- Add XML documentation for public APIs

## Troubleshooting

### Common Issues

1. **Database Connection Failed**

   - Check PostgreSQL is running
   - Verify connection string in appsettings
   - Ensure database exists

2. **Popular Posts Not Appearing**

   - Run: `POST /api/ranking/recalculate-popularity`
   - Check if RankingBackgroundService is running
   - Verify posts have engagement data

3. **JWT Authentication Issues**

   - Ensure JWT key is at least 32 characters
   - Check issuer and audience configuration
   - Verify token expiration settings

4. **CORS Issues**
   - Check frontend URL is in CORS policy
   - Verify CORS middleware is configured correctly

### Logs

Application logs are written to the console. For production, configure logging in `appsettings.Production.json`.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
