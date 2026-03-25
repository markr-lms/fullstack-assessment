namespace LMS.Assessment.Api.Helpers;

public static class RequestExtensions
{
    public static Guid? GetUserId(this HttpRequest request)
    {
        if (request.Headers.TryGetValue("X-User-Id", out var value) && Guid.TryParse(value.FirstOrDefault(), out var userId))
        {
            return userId;
        }

        return null;
    }
}
