using System.Collections.Concurrent;
using System.Text.Json;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Services;

public class NotificationStreamService
{
    private readonly ConcurrentDictionary<int, List<StreamWriter>> _userConnections = new();

    public void AddConnection(int userId, StreamWriter writer)
    {
        _userConnections.AddOrUpdate(
            userId,
            new List<StreamWriter> { writer },
            (key, existing) =>
            {
                existing.Add(writer);
                return existing;
            });
    }

    public void RemoveConnection(int userId, StreamWriter writer)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            connections.Remove(writer);
            if (connections.Count == 0)
            {
                _userConnections.TryRemove(userId, out _);
            }
        }
    }

    public async Task SendNotificationToUserAsync(int userId, NotificationDto notification)
    {
        if (!_userConnections.TryGetValue(userId, out var connections))
            return;

        var data = JsonSerializer.Serialize(notification, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var sseMessage = $"data: {data}\n\n";

        var deadConnections = new List<StreamWriter>();

        foreach (var connection in connections.ToList())
        {
            try
            {
                await connection.WriteAsync(sseMessage);
                await connection.FlushAsync();
            }
            catch
            {
                deadConnections.Add(connection);
            }
        }

        // Remove dead connections
        foreach (var deadConnection in deadConnections)
        {
            connections.Remove(deadConnection);
        }

        if (connections.Count == 0)
        {
            _userConnections.TryRemove(userId, out _);
        }
    }

    public async Task SendUnreadCountToUserAsync(int userId, int count)
    {
        if (!_userConnections.TryGetValue(userId, out var connections))
            return;

        var data = JsonSerializer.Serialize(new { type = "unreadCount", count }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var sseMessage = $"data: {data}\n\n";

        var deadConnections = new List<StreamWriter>();

        foreach (var connection in connections.ToList())
        {
            try
            {
                await connection.WriteAsync(sseMessage);
                await connection.FlushAsync();
            }
            catch
            {
                deadConnections.Add(connection);
            }
        }

        // Remove dead connections
        foreach (var deadConnection in deadConnections)
        {
            connections.Remove(deadConnection);
        }

        if (connections.Count == 0)
        {
            _userConnections.TryRemove(userId, out _);
        }
    }

    public bool HasConnections(int userId)
    {
        return _userConnections.ContainsKey(userId) && _userConnections[userId].Count > 0;
    }
}