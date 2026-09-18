namespace FarsiOrigin.Unrealizer.Translation;

public sealed class TranslationEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string SourceLanguage { get; set; } = "es";
    public string TargetLanguage { get; set; } = "fa";
    public string Source { get; set; } = "";
    public string Translation { get; set; } = "";
    public string Asset { get; set; } = "";
    public string Property { get; set; } = "";
    public string Status { get; set; } = "new";
}
