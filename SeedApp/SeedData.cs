using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace SeedApp;

public class SeedData
{
    private static async Task<Dictionary<string, bool>> CheckTablesExist(ApplicationDbContext context)
    {
        var tables = new Dictionary<string, bool>();
        var tableNames = new[] { "Categories", "Subcategories", "Users", "Posts", "Comments", "Likes", "Bookmarks", "StarDustPointsHistory", "UserSubcategoryFollows" };
        
        foreach (var tableName in tableNames)
        {
            try
            {
                var result = await context.Database.ExecuteSqlRawAsync($"SELECT 1 FROM \"{tableName}\" LIMIT 1");
                tables[tableName] = true;
            }
            catch
            {
                tables[tableName] = false;
            }
        }
        
        return tables;
    }

    public static async Task SeedDatabase(ApplicationDbContext context)
    {
        // Check if data already exists
        // Always clear and re-seed for development
        Console.WriteLine("🌱 Starting to seed database...");

        // Clear existing data (check if tables exist first)
        Console.WriteLine("🗑️ Clearing existing data...");
        
        // Check if tables exist before clearing
        var tablesExist = await CheckTablesExist(context);
        
        if (tablesExist.ContainsKey("Posts") && tablesExist["Posts"])
            context.Posts.RemoveRange(context.Posts);
        if (tablesExist.ContainsKey("Comments") && tablesExist["Comments"])
            context.Comments.RemoveRange(context.Comments);
        if (tablesExist.ContainsKey("Likes") && tablesExist["Likes"])
            context.Likes.RemoveRange(context.Likes);
        if (tablesExist.ContainsKey("Bookmarks") && tablesExist["Bookmarks"])
            context.Bookmarks.RemoveRange(context.Bookmarks);
        if (tablesExist.ContainsKey("StarDustPointsHistory") && tablesExist["StarDustPointsHistory"])
            context.StarDustPointsHistory.RemoveRange(context.StarDustPointsHistory);
        if (tablesExist.ContainsKey("Users") && tablesExist["Users"])
            context.Users.RemoveRange(context.Users);
        if (tablesExist.ContainsKey("Subcategories") && tablesExist["Subcategories"])
            context.Subcategories.RemoveRange(context.Subcategories);
        if (tablesExist.ContainsKey("Categories") && tablesExist["Categories"])
            context.Categories.RemoveRange(context.Categories);
            
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Cleared existing data");

        // Create Categories
        var categories = new List<Category>
        {
            new Category
            {
                Name = "Frontend",
                Description = "Frontend web development technologies and frameworks",
                Color = "#10B981",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Backend",
                Description = "Backend development, APIs, and server-side technologies",
                Color = "#3B82F6",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Programming",
                Description = "Programming languages, algorithms, and coding practices",
                Color = "#8B5CF6",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Full-Stack",
                Description = "Complete web application development from frontend to backend",
                Color = "#F59E0B",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "DevOps",
                Description = "Deployment, CI/CD, and infrastructure management",
                Color = "#EF4444",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Mobile Development",
                Description = "iOS, Android, and cross-platform mobile development",
                Color = "#06B6D4",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "AI",
                Description = "Artificial intelligence, machine learning, and data science",
                Color = "#EC4899",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Created {categories.Count} categories");

        // Create Subcategories
        var subcategories = new List<Subcategory>
        {
            // Frontend subcategories
            new Subcategory
            {
                Name = "React",
                Description = "React.js library and ecosystem",
                Color = "#61DAFB",
                CategoryId = categories[0].Id, // Frontend
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "Vue.js",
                Description = "Vue.js framework and ecosystem",
                Color = "#4FC08D",
                CategoryId = categories[0].Id, // Frontend
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "Angular",
                Description = "Angular framework and ecosystem",
                Color = "#DD0031",
                CategoryId = categories[0].Id, // Frontend
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "CSS & Styling",
                Description = "CSS, SCSS, Tailwind, and styling frameworks",
                Color = "#1572B6",
                CategoryId = categories[0].Id, // Frontend
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Backend subcategories
            new Subcategory { 
                Name = ".NET", 
                Description = "Backend development with .NET Core, ASP.NET, and C#", 
                Color = "#512BD4", 
                CategoryId = categories[1].Id, 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            },

            new Subcategory { 
                Name = "Node.js", 
                Description = "JavaScript and TypeScript backend development with Node.js and Express", 
                Color = "#339933", 
                CategoryId = categories[1].Id, 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            },

            new Subcategory { 
                Name = "Django", 
                Description = "Python backend development with Django framework", 
                Color = "#092E20", 
                CategoryId = categories[1].Id, 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            },

            new Subcategory { 
                Name = "Java Spring Boot", 
                Description = "Backend and enterprise development with Java Spring Boot", 
                Color = "#6DB33F", 
                CategoryId = categories[1].Id, 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            },

            new Subcategory { 
                Name = "Flask", 
                Description = "Lightweight Python backend development with Flask", 
                Color = "#000000", // Flask doesn’t have an official color, black is often used
                CategoryId = categories[1].Id, 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            },

            new Subcategory { 
                Name = "Ruby on Rails", 
                Description = "Web application development with Ruby on Rails", 
                Color = "#CC0000", 
                CategoryId = categories[1].Id, 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            },

            new Subcategory { 
                Name = "Laravel", 
                Description = "PHP backend development with Laravel framework", 
                Color = "#FF2D20", 
                CategoryId = categories[1].Id, 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            },

            new Subcategory { 
                Name = "Symfony", 
                Description = "PHP backend development with Symfony framework", 
                Color = "#000000", 
                CategoryId = categories[1].Id, 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            },

            // Programming subcategories
            new Subcategory
            {
                Name = "C#",
                Description = "C# programming language and best practices",
                Color = "#239120",
                CategoryId = categories[2].Id, // Programming
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "JavaScript",
                Description = "JavaScript programming and ES6+ features",
                Color = "#F7DF1E",
                CategoryId = categories[2].Id, // Programming
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "TypeScript",
                Description = "TypeScript programming and type safety",
                Color = "#3178C6",
                CategoryId = categories[2].Id, // Programming
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "Algorithms",
                Description = "Data structures, algorithms, and problem solving",
                Color = "#FF6B6B",
                CategoryId = categories[2].Id, // Programming
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Full-Stack subcategories
            new Subcategory
            {
                Name = "MERN Stack",
                Description = "MongoDB, Express, React, Node.js full-stack development",
                Color = "#68A063",
                CategoryId = categories[3].Id, // Full-Stack
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "MEAN Stack",
                Description = "MongoDB, Express, Angular, Node.js full-stack development",
                Color = "#DD1B16",
                CategoryId = categories[3].Id, // Full-Stack
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "JAMstack",
                Description = "JavaScript, APIs, and Markup static site generation",
                Color = "#F0047F",
                CategoryId = categories[3].Id, // Full-Stack
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // DevOps subcategories
            new Subcategory
            {
                Name = "Docker",
                Description = "Containerization with Docker and container orchestration",
                Color = "#2496ED",
                CategoryId = categories[4].Id, // DevOps
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "Kubernetes",
                Description = "Kubernetes orchestration and cloud-native development",
                Color = "#326CE5",
                CategoryId = categories[4].Id, // DevOps
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "AWS",
                Description = "Amazon Web Services cloud platform",
                Color = "#FF9900",
                CategoryId = categories[4].Id, // DevOps
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "CI/CD",
                Description = "Continuous Integration and Continuous Deployment",
                Color = "#2088FF",
                CategoryId = categories[4].Id, // DevOps
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Mobile Development subcategories
            new Subcategory
            {
                Name = "React Native",
                Description = "Cross-platform mobile development with React Native",
                Color = "#61DAFB",
                CategoryId = categories[5].Id, // Mobile Development
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "Flutter",
                Description = "Cross-platform mobile development with Flutter",
                Color = "#02569B",
                CategoryId = categories[5].Id, // Mobile Development
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "iOS",
                Description = "Native iOS development with Swift",
                Color = "#000000",
                CategoryId = categories[5].Id, // Mobile Development
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "Android",
                Description = "Native Android development with Kotlin/Java",
                Color = "#3DDC84",
                CategoryId = categories[5].Id, // Mobile Development
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // AI subcategories
            new Subcategory
            {
                Name = "Machine Learning",
                Description = "Machine learning algorithms and applications",
                Color = "#FF6F00",
                CategoryId = categories[6].Id, // AI
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "Deep Learning",
                Description = "Neural networks and deep learning frameworks",
                Color = "#FF5722",
                CategoryId = categories[6].Id, // AI
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "Data Science",
                Description = "Data analysis, visualization, and insights",
                Color = "#9C27B0",
                CategoryId = categories[6].Id, // AI
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Subcategory
            {
                Name = "NLP",
                Description = "Natural Language Processing and text analysis",
                Color = "#607D8B",
                CategoryId = categories[6].Id, // AI
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Subcategories.AddRangeAsync(subcategories);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Created {subcategories.Count} subcategories");

        // Create multiple users with complete profiles
        Console.WriteLine("👤 Creating users...");
        var users = new List<User>
        {
            new User
            {
                Username = "alex_dev",
                Name = "Alex Rodriguez",
                Email = "alex@example.com",
                Role = "User",
                Avatar = "https://api.dicebear.com/7.x/personas/svg?seed=Alex&backgroundColor=4f46e5&clothingColor=262e33&skinColor=edb98a",
                Biography = "Full-stack developer passionate about React and .NET. Love building scalable applications and sharing knowledge with the community.",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                StarDustPoints = 250
            },
            new User
            {
                Username = "sarah_ui",
                Name = "Sarah Chen",
                Email = "sarah@example.com",
                Role = "User",
                Avatar = "https://api.dicebear.com/7.x/personas/svg?seed=Sarah&backgroundColor=ec4899&clothingColor=262e33&skinColor=fdbcb4",
                Biography = "UI/UX designer and frontend developer. Creating beautiful and intuitive user experiences is my passion.",
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                StarDustPoints = 180
            },
            new User
            {
                Username = "mike_backend",
                Name = "Mike Johnson",
                Email = "mike@example.com",
                Role = "User",
                Avatar = "https://api.dicebear.com/7.x/personas/svg?seed=Mike&backgroundColor=3b82f6&clothingColor=262e33&skinColor=edb98a",
                Biography = "Backend specialist with expertise in microservices and cloud architecture. Always learning new technologies.",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                StarDustPoints = 320
            },
            new User
            {
                Username = "emma_mobile",
                Name = "Emma Wilson",
                Email = "emma@example.com",
                Role = "User",
                Avatar = "https://api.dicebear.com/7.x/personas/svg?seed=Emma&backgroundColor=06b6d4&clothingColor=262e33&skinColor=fdbcb4",
                Biography = "Mobile app developer focused on Flutter and React Native. Building cross-platform apps that users love.",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                StarDustPoints = 150
            },
            new User
            {
                Username = "david_ai",
                Name = "David Kim",
                Email = "david@example.com",
                Role = "User",
                Avatar = "https://api.dicebear.com/7.x/personas/svg?seed=David&backgroundColor=8b5cf6&clothingColor=262e33&skinColor=edb98a",
                Biography = "AI/ML engineer and data scientist. Exploring the intersection of artificial intelligence and real-world applications.",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                StarDustPoints = 400
            }
        };

        try
        {
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Created {users.Count} users successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error creating users: {ex.Message}");
            throw new Exception("Could not create users for seeding");
        }

        // Create Posts - using actual user and category references
        var frontendCategory = categories.FirstOrDefault(c => c.Name == "Frontend");
        var backendCategory = categories.FirstOrDefault(c => c.Name == "Backend");
        var programmingCategory = categories.FirstOrDefault(c => c.Name == "Programming");
        var fullstackCategory = categories.FirstOrDefault(c => c.Name == "Full-Stack");
        var mobileCategory = categories.FirstOrDefault(c => c.Name == "Mobile Development");
        var devopsCategory = categories.FirstOrDefault(c => c.Name == "DevOps");
        var aiCategory = categories.FirstOrDefault(c => c.Name == "AI");

        var reactSubcategory = subcategories.FirstOrDefault(s => s.Name == "React");
        var dotnetSubcategory = subcategories.FirstOrDefault(s => s.Name == ".NET");
        var flutterSubcategory = subcategories.FirstOrDefault(s => s.Name == "Flutter");
        var dockerSubcategory = subcategories.FirstOrDefault(s => s.Name == "Docker");
        var machineLearningSubcategory = subcategories.FirstOrDefault(s => s.Name == "Machine Learning");
        var cssSubcategory = subcategories.FirstOrDefault(s => s.Name == "CSS & Styling");
        var typescriptSubcategory = subcategories.FirstOrDefault(s => s.Name == "TypeScript");
        var mernSubcategory = subcategories.FirstOrDefault(s => s.Name == "MERN Stack");

        var posts = new List<Post>
        {
            // Frontend Posts
            new Post
            {
                Title = "Getting Started with React 18: New Features and Best Practices",
                Content = "React 18 introduces several exciting new features that enhance both developer experience and user experience. In this comprehensive guide, we'll explore the key additions including concurrent rendering, automatic batching, and enhanced Suspense capabilities.",
                Summary = "Explore React 18's new features including concurrent rendering, automatic batching, and enhanced Suspense capabilities.",
                ImageUrl = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
                AuthorId = users[0].Id,
                CategoryId = frontendCategory?.Id ?? 1,
                SubcategoryId = reactSubcategory?.Id,
                LikesCount = 24,
                CommentsCount = 8,
                VisitsCount = 156,
                Tags = new string[] { "React", "JavaScript", "Frontend", "Web Development" }
            },
            new Post
            {
                Title = "Vue.js 3 Composition API: A Complete Guide",
                Content = "Vue.js 3 introduces the Composition API, providing a more flexible way to organize component logic. Learn how to use the Composition API effectively in your Vue applications.",
                Summary = "Master the Vue.js 3 Composition API for better component organization and reusability.",
                ImageUrl = "https://images.unsplash.com/photo-1517077304055-6e8ab7b83f58?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                UpdatedAt = DateTime.UtcNow.AddDays(-12),
                AuthorId = users[1].Id,
                CategoryId = frontendCategory?.Id ?? 1,
                SubcategoryId = subcategories.FirstOrDefault(s => s.Name == "Vue.js")?.Id,
                LikesCount = 18,
                CommentsCount = 6,
                VisitsCount = 124,
                Tags = new string[] { "Vue.js", "JavaScript", "Frontend", "Composition API" }
            },
            new Post
            {
                Title = "Angular 17: Standalone Components and New Features",
                Content = "Angular 17 brings exciting new features including standalone components, improved performance, and enhanced developer experience. Discover what's new and how to leverage these features.",
                Summary = "Explore Angular 17's new standalone components and performance improvements.",
                ImageUrl = "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-18),
                UpdatedAt = DateTime.UtcNow.AddDays(-18),
                AuthorId = users[0].Id,
                CategoryId = frontendCategory?.Id ?? 1,
                SubcategoryId = subcategories.FirstOrDefault(s => s.Name == "Angular")?.Id,
                LikesCount = 21,
                CommentsCount = 9,
                VisitsCount = 167,
                Tags = new string[] { "Angular", "TypeScript", "Frontend", "Standalone Components" }
            },
            // Backend Posts
            new Post
            {
                Title = "Building Scalable Microservices with .NET 8",
                Content = "Microservices architecture has become the go-to approach for building large-scale applications. With .NET 8, we have powerful tools to create robust, scalable microservices including minimal APIs, built-in dependency injection, and health checks.",
                Summary = "Learn how to build scalable microservices using .NET 8's latest features and best practices.",
                ImageUrl = "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3),
                AuthorId = users[2].Id,
                CategoryId = backendCategory?.Id ?? 2,
                SubcategoryId = dotnetSubcategory?.Id,
                LikesCount = 18,
                CommentsCount = 5,
                VisitsCount = 89,
                Tags = new string[] { ".NET", "Microservices", "Backend", "C#" }
            },
            new Post
            {
                Title = "Node.js Performance Optimization: Best Practices",
                Content = "Node.js applications can face performance bottlenecks as they scale. Learn essential optimization techniques including clustering, caching strategies, and memory management to build high-performance applications.",
                Summary = "Master Node.js performance optimization techniques for scalable applications.",
                ImageUrl = "https://images.unsplash.com/photo-1627398242454-45a1465c2479?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-9),
                UpdatedAt = DateTime.UtcNow.AddDays(-9),
                AuthorId = users[2].Id,
                CategoryId = backendCategory?.Id ?? 2,
                SubcategoryId = subcategories.FirstOrDefault(s => s.Name == "Node.js")?.Id,
                LikesCount = 26,
                CommentsCount = 11,
                VisitsCount = 198,
                Tags = new string[] { "Node.js", "Performance", "Backend", "JavaScript" }
            },
            new Post
            {
                Title = "Laravel 10: New Features and Migration Guide",
                Content = "Laravel 10 introduces several exciting features including improved performance, new validation rules, and enhanced testing capabilities. Learn how to migrate your applications and leverage new features.",
                Summary = "Discover Laravel 10's new features and learn how to migrate your applications.",
                ImageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-14),
                UpdatedAt = DateTime.UtcNow.AddDays(-14),
                AuthorId = users[0].Id,
                CategoryId = backendCategory?.Id ?? 2,
                SubcategoryId = subcategories.FirstOrDefault(s => s.Name == "Laravel")?.Id,
                LikesCount = 23,
                CommentsCount = 8,
                VisitsCount = 145,
                Tags = new string[] { "Laravel", "PHP", "Backend", "Migration" }
            },
            // Mobile Development Posts
            new Post
            {
                Title = "The Future of Mobile Development: Cross-Platform Solutions",
                Content = "Mobile development has evolved significantly over the years. Today, developers have multiple options for building cross-platform applications that can run on both iOS and Android using Flutter, React Native, or Xamarin.",
                Summary = "Explore cross-platform mobile development frameworks and learn how to choose the right solution for your project.",
                ImageUrl = "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow.AddDays(-7),
                AuthorId = users[3].Id,
                CategoryId = mobileCategory?.Id ?? 6,
                SubcategoryId = flutterSubcategory?.Id,
                LikesCount = 31,
                CommentsCount = 12,
                VisitsCount = 203,
                Tags = new string[] { "Flutter", "Mobile", "Cross-Platform", "Dart" }
            },
            new Post
            {
                Title = "React Native vs Flutter: A Comprehensive Comparison",
                Content = "Choosing between React Native and Flutter for mobile development can be challenging. This comprehensive comparison covers performance, development experience, ecosystem, and real-world usage scenarios.",
                Summary = "Compare React Native and Flutter to make an informed decision for your mobile project.",
                ImageUrl = "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-11),
                UpdatedAt = DateTime.UtcNow.AddDays(-11),
                AuthorId = users[3].Id,
                CategoryId = mobileCategory?.Id ?? 6,
                SubcategoryId = subcategories.FirstOrDefault(s => s.Name == "React Native")?.Id,
                LikesCount = 28,
                CommentsCount = 14,
                VisitsCount = 234,
                Tags = new string[] { "React Native", "Flutter", "Mobile", "Comparison" }
            },
            // DevOps Posts
            new Post
            {
                Title = "Docker and Kubernetes: Container Orchestration Made Simple",
                Content = "Containerization has revolutionized how we deploy and manage applications. Docker and Kubernetes work together to provide a powerful platform for container orchestration with benefits like consistency, scalability, and resource efficiency.",
                Summary = "Learn the fundamentals of Docker and Kubernetes for container orchestration and deployment.",
                ImageUrl = "https://images.unsplash.com/photo-1605379399642-870262d3d051?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                AuthorId = users[2].Id,
                CategoryId = devopsCategory?.Id ?? 5,
                SubcategoryId = dockerSubcategory?.Id,
                LikesCount = 27,
                CommentsCount = 9,
                VisitsCount = 142,
                Tags = new string[] { "Docker", "Kubernetes", "DevOps", "Containers" }
            },
            new Post
            {
                Title = "AWS Cloud Architecture: Best Practices for Scalability",
                Content = "Amazon Web Services provides a comprehensive suite of cloud services. Learn how to design scalable, cost-effective architectures using AWS services like EC2, RDS, S3, and Lambda.",
                Summary = "Master AWS cloud architecture patterns for building scalable applications.",
                ImageUrl = "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-16),
                UpdatedAt = DateTime.UtcNow.AddDays(-16),
                AuthorId = users[2].Id,
                CategoryId = devopsCategory?.Id ?? 5,
                SubcategoryId = subcategories.FirstOrDefault(s => s.Name == "AWS")?.Id,
                LikesCount = 32,
                CommentsCount = 13,
                VisitsCount = 267,
                Tags = new string[] { "AWS", "Cloud", "DevOps", "Architecture" }
            },

            // AI/ML Posts
            new Post
            {
                Title = "Introduction to Machine Learning with Python",
                Content = "Machine Learning is transforming industries and creating new opportunities for developers. Python has become the go-to language for ML due to its rich ecosystem of libraries including NumPy, Pandas, Scikit-learn, TensorFlow, and PyTorch.",
                Summary = "Discover the fundamentals of machine learning with Python and popular ML libraries.",
                ImageUrl = "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                AuthorId = users[4].Id,
                CategoryId = aiCategory?.Id ?? 7,
                SubcategoryId = machineLearningSubcategory?.Id,
                LikesCount = 35,
                CommentsCount = 15,
                VisitsCount = 278,
                Tags = new string[] { "Python", "Machine Learning", "AI", "Data Science" }
            },
            new Post
            {
                Title = "Deep Learning with TensorFlow: A Practical Guide",
                Content = "Deep learning has revolutionized artificial intelligence. Learn how to build neural networks using TensorFlow, from basic concepts to advanced architectures like CNNs and RNNs.",
                Summary = "Build deep learning models with TensorFlow from basics to advanced architectures.",
                ImageUrl = "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow.AddDays(-8),
                AuthorId = users[4].Id,
                CategoryId = aiCategory?.Id ?? 7,
                SubcategoryId = machineLearningSubcategory?.Id,
                LikesCount = 29,
                CommentsCount = 12,
                VisitsCount = 189,
                Tags = new string[] { "TensorFlow", "Deep Learning", "Neural Networks", "AI" }
            },

            // Programming Posts
            new Post
            {
                Title = "TypeScript Best Practices for Large Applications",
                Content = "TypeScript brings type safety to JavaScript, making large applications more maintainable and less error-prone. Learn the best practices for using TypeScript in enterprise applications.",
                Summary = "Master TypeScript best practices for building scalable and maintainable applications.",
                ImageUrl = "https://images.unsplash.com/photo-1516116216624-53e697fedbea?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-6),
                UpdatedAt = DateTime.UtcNow.AddDays(-6),
                AuthorId = users[0].Id,
                CategoryId = programmingCategory?.Id ?? 3,
                SubcategoryId = typescriptSubcategory?.Id,
                LikesCount = 22,
                CommentsCount = 7,
                VisitsCount = 134,
                Tags = new string[] { "TypeScript", "JavaScript", "Programming", "Best Practices" }
            },
            new Post
            {
                Title = "Python 3.12: New Features and Performance Improvements",
                Content = "Python 3.12 introduces several exciting features including improved error messages, new syntax features, and performance optimizations. Learn what's new and how to leverage these improvements.",
                Summary = "Explore Python 3.12's new features and performance improvements.",
                ImageUrl = "https://images.unsplash.com/photo-1526379095098-d400fd0bf935?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-13),
                UpdatedAt = DateTime.UtcNow.AddDays(-13),
                AuthorId = users[4].Id,
                CategoryId = programmingCategory?.Id ?? 3,
                SubcategoryId = subcategories.FirstOrDefault(s => s.Name == "Python")?.Id,
                LikesCount = 25,
                CommentsCount = 10,
                VisitsCount = 156,
                Tags = new string[] { "Python", "Programming", "Performance", "New Features" }
            },
            new Post
            {
                Title = "Java 21: Modern Java Development with Virtual Threads",
                Content = "Java 21 introduces virtual threads, a revolutionary feature that simplifies concurrent programming. Learn how to use virtual threads and other modern Java features for better performance.",
                Summary = "Discover Java 21's virtual threads and modern development features.",
                ImageUrl = "https://images.unsplash.com/photo-1517077304055-6e8ab7b83f58?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-15),
                AuthorId = users[2].Id,
                CategoryId = programmingCategory?.Id ?? 3,
                SubcategoryId = subcategories.FirstOrDefault(s => s.Name == "Java")?.Id,
                LikesCount = 20,
                CommentsCount = 8,
                VisitsCount = 112,
                Tags = new string[] { "Java", "Virtual Threads", "Concurrency", "Performance" }
            },

            // Full-Stack Posts
            new Post
            {
                Title = "Building Full-Stack Applications with MERN Stack",
                Content = "The MERN stack (MongoDB, Express, React, Node.js) provides a complete solution for building modern web applications. Learn how to create a full-stack application from scratch.",
                Summary = "Complete guide to building full-stack applications using the MERN stack.",
                ImageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow.AddDays(-8),
                AuthorId = users[0].Id,
                CategoryId = fullstackCategory?.Id ?? 4,
                SubcategoryId = mernSubcategory?.Id,
                LikesCount = 29,
                CommentsCount = 11,
                VisitsCount = 189,
                Tags = new string[] { "MERN", "Full-Stack", "JavaScript", "MongoDB" }
            },
            new Post
            {
                Title = "Next.js 14: Full-Stack React Applications Made Easy",
                Content = "Next.js 14 introduces powerful features for building full-stack React applications including App Router, Server Components, and enhanced performance. Learn how to build modern web apps with Next.js.",
                Summary = "Master Next.js 14 for building full-stack React applications with modern features.",
                ImageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10),
                AuthorId = users[1].Id,
                CategoryId = fullstackCategory?.Id ?? 4,
                SubcategoryId = reactSubcategory?.Id,
                LikesCount = 33,
                CommentsCount = 16,
                VisitsCount = 245,
                Tags = new string[] { "Next.js", "React", "Full-Stack", "Server Components" }
            },

            // Additional Frontend Posts
            new Post
            {
                Title = "Modern CSS Techniques: Grid, Flexbox, and Custom Properties",
                Content = "CSS has evolved significantly with modern features that make layout and styling more powerful and flexible. Learn about CSS Grid, Flexbox, Custom Properties, and other modern CSS features for creating responsive designs.",
                Summary = "Learn modern CSS techniques including Grid, Flexbox, and Custom Properties for better layouts and styling.",
                ImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=800&h=400&fit=crop",
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-4),
                AuthorId = users[1].Id,
                CategoryId = frontendCategory?.Id ?? 1,
                SubcategoryId = cssSubcategory?.Id,
                LikesCount = 19,
                CommentsCount = 6,
                VisitsCount = 98,
                Tags = new string[] { "CSS", "Grid", "Flexbox", "Frontend" }
            }
        };

        try
        {
            await context.Posts.AddRangeAsync(posts);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Created {posts.Count} posts");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error creating posts: {ex.Message}");
            // Try to create posts one by one to identify the problematic one
            Console.WriteLine("🔄 Trying to create posts one by one...");
            var successCount = 0;
            foreach (var post in posts)
            {
                try
                {
                    context.Posts.Add(post);
                    await context.SaveChangesAsync();
                    successCount++;
                    Console.WriteLine($"✅ Created post: {post.Title}");
                }
                catch (Exception postEx)
                {
                    Console.WriteLine($"❌ Failed to create post '{post.Title}': {postEx.Message}");
                }
            }
            Console.WriteLine($"✅ Successfully created {successCount} out of {posts.Count} posts");
        }

        // Create Comments
        var comments = new List<Comment>
        {
            new Comment
            {
                Content = "Great article! The concurrent rendering feature is a game-changer for performance.",
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-4),
                PostId = posts[0].Id,
                AuthorId = users[0].Id
            },
            new Comment
            {
                Content = "Thanks for sharing! I've been using React 18 in production and it's been amazing.",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3),
                PostId = posts[0].Id,
                AuthorId = users[0].Id
            },
            new Comment
            {
                Content = "Excellent breakdown of microservices patterns. The .NET 8 examples are very helpful.",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                PostId = posts[1].Id,
                AuthorId = users[0].Id
            },
            new Comment
            {
                Content = "I've been considering Flutter for our next mobile project. This comparison helps a lot!",
                CreatedAt = DateTime.UtcNow.AddDays(-6),
                UpdatedAt = DateTime.UtcNow.AddDays(-6),
                PostId = posts[2].Id,
                AuthorId = users[0].Id
            },
            new Comment
            {
                Content = "Docker and Kubernetes have simplified our deployment process significantly.",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                PostId = posts[3].Id,
                AuthorId = users[0].Id
            },
            new Comment
            {
                Content = "The ML examples are clear and practical. Looking forward to trying these techniques!",
                CreatedAt = DateTime.UtcNow.AddHours(-12),
                UpdatedAt = DateTime.UtcNow.AddHours(-12),
                PostId = posts[4].Id,
                AuthorId = users[0].Id
            }
        };

        await context.Comments.AddRangeAsync(comments);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Created {comments.Count} comments");

        // Create Likes - only if we have posts
        if (posts.Count > 0)
        {
            var likes = new List<Like>();
            
            // Create likes for the first few posts only
            var maxPosts = Math.Min(posts.Count, 5);
            for (int i = 0; i < maxPosts; i++)
            {
                likes.Add(new Like { CreatedAt = DateTime.UtcNow.AddDays(-(i + 1)), PostId = posts[i].Id, UserId = users[i % users.Count].Id });
                if (i < 3) // Add multiple likes for first 3 posts
                {
                    likes.Add(new Like { CreatedAt = DateTime.UtcNow.AddDays(-(i + 2)), PostId = posts[i].Id, UserId = users[(i + 1) % users.Count].Id });
                }
            }

            try
            {
                await context.Likes.AddRangeAsync(likes);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Created {likes.Count} likes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating likes: {ex.Message}");
                Console.WriteLine("⏭️ Skipping likes creation");
            }
        }
        else
        {
            Console.WriteLine("⏭️ No posts available for likes creation");
        }

        Console.WriteLine("🎉 Database seeding completed successfully!");
    }
}
