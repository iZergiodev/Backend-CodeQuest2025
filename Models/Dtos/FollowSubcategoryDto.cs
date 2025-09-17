namespace CodeQuestBackend.Models.Dtos
{
    public class FollowSubcategoryDto
    {
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
    }

    public class FollowAllSubcategoriesDto
    {
        public int CategoryId { get; set; }
    }

    public class UserFollowsDto
    {
        public List<SubcategoryDto> FollowedSubcategories { get; set; } = new List<SubcategoryDto>();
    }

    public class SubcategoryWithFollowerCountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int FollowerCount { get; set; }
        public bool IsFollowing { get; set; }
    }
}
