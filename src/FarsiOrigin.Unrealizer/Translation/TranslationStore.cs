using System.Text;
using System.Text.Json;

namespace FarsiOrigin.Unrealizer.Translation;

public static class TranslationStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static void Save(string file, IEnumerable<TranslationEntry> entries)
    {
        var directory = Path.GetDirectoryName(file);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(file, JsonSerializer.Serialize(entries, Options), new UTF8Encoding(false));
    }

    public static List<TranslationEntry> Load(string file)
    {
        if (!File.Exists(file))
            return [];

        return JsonSerializer.Deserialize<List<TranslationEntry>>(
            File.ReadAllText(file, Encoding.UTF8), Options) ?? [];
    }
}
