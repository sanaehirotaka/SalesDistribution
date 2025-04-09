namespace SalesDistribution;

public static class Common
{
    public static string GenerateId()
    {
        byte[] bytes = new byte[3];
        Random.Shared.NextBytes(bytes);
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString("x") + BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}
