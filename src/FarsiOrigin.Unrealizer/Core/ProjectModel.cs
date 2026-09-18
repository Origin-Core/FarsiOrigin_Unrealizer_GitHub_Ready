namespace FarsiOrigin.Unrealizer.Core;

public sealed class GameScanReport
{
    public string GameRoot { get; init; } = "";
    public string? ExePath { get; set; }
    public string? ContentPath { get; set; }
    public string? PaksPath { get; set; }
    public bool HasIoStore { get; set; }
    public bool HasPak { get; set; }
    public List<string> Archives { get; } = [];
    public List<string> Candidates { get; } = [];
    public List<string> Warnings { get; } = [];
    public List<string> Notes { get; } = [];
}
