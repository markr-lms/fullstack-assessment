namespace LMS.Assessment.Api.Helpers;

public static class RequestExtensions
{
    public static Guid? GetUserId(this HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-User-Id", out var userIdHeaders))
            return null;

        if (userIdHeaders.FirstOrDefault() is not string firstUserId)
            return null;

        if (!Guid.TryParse(firstUserId, out var userId))
            return null;

        return userId;
    }
}
