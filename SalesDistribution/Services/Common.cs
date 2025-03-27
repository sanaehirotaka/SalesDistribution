namespace SalesDistribution.Services;

public static class Common
{
    public static string GenerateId()
    {
        byte[] bytes = new byte[6];
        Random.Shared.NextBytes(bytes);
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}
