namespace Anonwork.Application.Common;

public static class AnonAliasGenerator
{
    private static readonly string[] Adjectives =
    [
        "Silent", "Dark", "Cosmic", "Hollow", "Mystic",
        "Neon", "Frozen", "Ancient", "Wandering", "Hidden"
    ];

    private static readonly string[] Nouns =
    [
        "Whale", "Fox", "Raven", "Wolf", "Specter",
        "Comet", "Phantom", "Cipher", "Echo", "Shade"
    ];

    public static string Generate()
    {
        var adj = Adjectives[Random.Shared.Next(Adjectives.Length)];
        var noun = Nouns[Random.Shared.Next(Nouns.Length)];
        var num = Random.Shared.Next(10, 999);
        return $"{adj}{noun}{num}";
    }
}