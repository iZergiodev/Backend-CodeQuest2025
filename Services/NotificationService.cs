using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services;

public class NotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserFollowRepository _userFollowRepository;
    private readonly NotificationStreamService _streamService;

    public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository, IUserFollowRepository userFollowRepository, NotificationStreamService streamService)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _userFollowRepository = userFollowRepository;
        _streamService = streamService;
    }

    public async Task<ICollection<NotificationDto>> GetNotificationsByUserIdAsync(int userId)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        return notifications.Select(MapToNotificationDto).ToList();
    }

    public async Task<ICollection<NotificationDto>> GetUnreadNotificationsByUserIdAsync(int userId)
    {
        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
        return notifications.Select(MapToNotificationDto).ToList();
    }

    public async Task<int> GetUnreadCountByUserIdAsync(int userId)
    {
        return await _notificationRepository.GetUnreadCountByUserIdAsync(userId);
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        return notification != null ? MapToNotificationDto(notification) : null;
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        return await _notificationRepository.MarkAsReadAsync(id);
    }

    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        return await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    public async Task<bool> DeleteNotificationAsync(int id)
    {
        return await _notificationRepository.DeleteAsync(id);
    }

    // Methods for creating notifications for different events
    public async Task CreatePostLikeNotificationAsync(int postAuthorId, int likerUserId, int postId)
    {
        if (postAuthorId == likerUserId) return; // Don't notify yourself

        var liker = await _userRepository.GetByIdAsync(likerUserId);
        if (liker == null) return;

        var createNotificationDto = new CreateNotificationDto
        {
            UserId = postAuthorId,
            Type = "like",
            Title = "¡Tu post recibió un like!",
            Message = $"{liker.Name ?? liker.Username} le dio like a tu post",
            RelatedPostId = postId,
            RelatedUserId = likerUserId
        };

        var notification = await _notificationRepository.CreateAsync(createNotificationDto);
        var notificationDto = MapToNotificationDto(notification);

        // Send real-time notification via SSE
        await _streamService.SendNotificationToUserAsync(postAuthorId, notificationDto);

        // Update unread count
        var unreadCount = await GetUnreadCountByUserIdAsync(postAuthorId);
        await _streamService.SendUnreadCountToUserAsync(postAuthorId, unreadCount);
    }

    public async Task CreateCommentNotificationAsync(int postAuthorId, int commenterUserId, int postId, int commentId)
    {
        if (postAuthorId == commenterUserId) return; // Don't notify yourself

        var commenter = await _userRepository.GetByIdAsync(commenterUserId);
        if (commenter == null) return;

        var createNotificationDto = new CreateNotificationDto
        {
            UserId = postAuthorId,
            Type = "comment",
            Title = "¡Nuevo comentario en tu post!",
            Message = $"{commenter.Name ?? commenter.Username} comentó en tu post",
            RelatedPostId = postId,
            RelatedCommentId = commentId,
            RelatedUserId = commenterUserId
        };

        var notification = await _notificationRepository.CreateAsync(createNotificationDto);
        var notificationDto = MapToNotificationDto(notification);

        // Send real-time notification via SSE
        await _streamService.SendNotificationToUserAsync(postAuthorId, notificationDto);

        // Update unread count
        var unreadCount = await GetUnreadCountByUserIdAsync(postAuthorId);
        await _streamService.SendUnreadCountToUserAsync(postAuthorId, unreadCount);
    }

    public async Task CreateFollowNotificationAsync(int followedUserId, int followerUserId)
    {
        if (followedUserId == followerUserId) return; // Don't notify yourself

        var follower = await _userRepository.GetByIdAsync(followerUserId);
        if (follower == null) return;

        var createNotificationDto = new CreateNotificationDto
        {
            UserId = followedUserId,
            Type = "follow",
            Title = "¡Tienes un nuevo seguidor!",
            Message = $"{follower.Name ?? follower.Username} ahora te sigue",
            RelatedUserId = followerUserId
        };

        var notification = await _notificationRepository.CreateAsync(createNotificationDto);
        var notificationDto = MapToNotificationDto(notification);

        // Send real-time notification via SSE
        await _streamService.SendNotificationToUserAsync(followedUserId, notificationDto);

        // Update unread count
        var unreadCount = await GetUnreadCountByUserIdAsync(followedUserId);
        await _streamService.SendUnreadCountToUserAsync(followedUserId, unreadCount);
    }

    public async Task CreatePostInFollowedSubcategoryNotificationAsync(int authorId, int postId, int subcategoryId)
    {
        var followers = await _userFollowRepository.GetFollowersBySubcategoryIdAsync(subcategoryId);
        var author = await _userRepository.GetByIdAsync(authorId);
        if (author == null) return;

        foreach (var follower in followers)
        {
            if (follower.UserId == authorId) continue; // Don't notify the author

            var createNotificationDto = new CreateNotificationDto
            {
                UserId = follower.UserId,
                Type = "post_in_followed_subcategory",
                Title = "¡Nuevo post en una subcategoría que sigues!",
                Message = $"{author.Name ?? author.Username} publicó un nuevo post",
                RelatedPostId = postId,
                RelatedUserId = authorId
            };

            var notification = await _notificationRepository.CreateAsync(createNotificationDto);
            var notificationDto = MapToNotificationDto(notification);

            // Send real-time notification via SSE
            await _streamService.SendNotificationToUserAsync(follower.UserId, notificationDto);

            // Update unread count
            var unreadCount = await GetUnreadCountByUserIdAsync(follower.UserId);
            await _streamService.SendUnreadCountToUserAsync(follower.UserId, unreadCount);
        }
    }

    public async Task CreateStarDustMilestoneNotificationAsync(int userId, int points, string reason)
    {
        var createNotificationDto = new CreateNotificationDto
        {
            UserId = userId,
            Type = "stardust_milestone",
            Title = "¡Milestone de StarDust Points!",
            Message = $"¡Felicidades! Has alcanzado {points} StarDust Points por {reason}",
        };

        var notification = await _notificationRepository.CreateAsync(createNotificationDto);
        var notificationDto = MapToNotificationDto(notification);

        // Send real-time notification via SSE
        await _streamService.SendNotificationToUserAsync(userId, notificationDto);

        // Update unread count
        var unreadCount = await GetUnreadCountByUserIdAsync(userId);
        await _streamService.SendUnreadCountToUserAsync(userId, unreadCount);
    }

    private static NotificationDto MapToNotificationDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            RelatedPostId = notification.RelatedPostId,
            RelatedPostTitle = notification.RelatedPost?.Title,
            RelatedCommentId = notification.RelatedCommentId,
            RelatedUserId = notification.RelatedUserId,
            RelatedUserName = notification.RelatedUser?.Name ?? notification.RelatedUser?.Username,
            RelatedUserAvatar = notification.RelatedUser?.Avatar,
            CreatedAt = notification.CreatedAt
        };
    }
}