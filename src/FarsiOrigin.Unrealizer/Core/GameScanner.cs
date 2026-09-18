using System.Text;

namespace FarsiOrigin.Unrealizer.Core;

public static class GameScanner
{
    public static GameScanReport Scan(string root)
    {
        root = Path.GetFullPath(root);
        var report = new GameScanReport { GameRoot = root };

        report.ExePath = Directory.EnumerateFiles(root, "*.exe", SearchOption.AllDirectories)
            .OrderByDescending(p => Path.GetFileName(p).Contains("Win64", StringComparison.OrdinalIgnoreCase))
            .ThenBy(p => p.Length)
            .FirstOrDefault();

        report.ContentPath = Directory.Exists(Path.Combine(root, "Content"))
            ? Path.Combine(root, "Content")
            : Directory.EnumerateDirectories(root, "Content", SearchOption.AllDirectories).FirstOrDefault();

        if (report.ContentPath != null)
        {
            report.PaksPath = Directory.Exists(Path.Combine(report.ContentPath, "Paks"))
                ? Path.Combine(report.ContentPath, "Paks")
                : Directory.EnumerateDirectories(report.ContentPath, "Paks", SearchOption.AllDirectories).FirstOrDefault();

            if (report.PaksPath != null)
            {
                foreach (var f in Directory.EnumerateFiles(report.PaksPath, "*.*", SearchOption.TopDirectoryOnly))
                {
                    var ext = Path.GetExtension(f).ToLowerInvariant();
                    if (ext is ".pak" or ".utoc" or ".ucas")
                        report.Archives.Add(f);
                }

                report.HasIoStore = report.Archives.Any(f =>
                    f.EndsWith(".utoc", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".ucas", StringComparison.OrdinalIgnoreCase));

                report.HasPak = report.Archives.Any(f =>
                    f.EndsWith(".pak", StringComparison.OrdinalIgnoreCase));
            }
        }

        foreach (var dir in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories))
        {
            var name = Path.GetFileName(dir);
            if (name.Contains("Localization", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Subtitle", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Subtitles", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Widgets", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("UI", StringComparison.OrdinalIgnoreCase))
            {
                report.Candidates.Add(dir);
            }
        }

        if (report.ExePath == null)
            report.Warnings.Add("No executable (.exe) was found.");
        if (report.PaksPath == null)
            report.Warnings.Add("Content/Paks was not found.");
        if (report.HasIoStore)
            report.Notes.Add("IoStore detected (.utoc/.ucas). Asset rewriting will remain disabled until the asset is proven safe.");
        if (report.HasPak)
            report.Notes.Add("PAK archive detected.");

        return report;
    }

    public static string CreateWorkspace(string gameRoot)
    {
        var safeName = Path.GetFileName(Path.TrimEndingDirectorySeparator(gameRoot));
        var workspace = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "FarsiOrigin", "Projects", safeName);

        foreach (var folder in new[] { "Original", "Extracted", "Translation", "Fonts", "RTL", "Build", "Logs" })
            Directory.CreateDirectory(Path.Combine(workspace, folder));

        return workspace;
    }

    public static void SaveReport(GameScanReport report, string workspace)
    {
        Directory.CreateDirectory(Path.Combine(workspace, "Logs"));
        var path = Path.Combine(workspace, "Logs", "scan-report.txt");

        using var w = new StreamWriter(path, false, new UTF8Encoding(false));
        w.WriteLine("FARSI ORIGIN — Unrealizer");
        w.WriteLine($"GameRoot: {report.GameRoot}");
        w.WriteLine($"EXE: {report.ExePath}");
        w.WriteLine($"Content: {report.ContentPath}");
        w.WriteLine($"Paks: {report.PaksPath}");
        w.WriteLine($"IoStore: {report.HasIoStore}");
        w.WriteLine($"PAK: {report.HasPak}");
        w.WriteLine();
        w.WriteLine("[Archives]");
        foreach (var a in report.Archives) w.WriteLine(a);
        w.WriteLine();
        w.WriteLine("[Candidates]");
        foreach (var c in report.Candidates.Take(500)) w.WriteLine(c);
        w.WriteLine();
        w.WriteLine("[Warnings]");
        foreach (var x in report.Warnings) w.WriteLine(x);
        w.WriteLine();
        w.WriteLine("[Notes]");
        foreach (var x in report.Notes) w.WriteLine(x);
    }
}
