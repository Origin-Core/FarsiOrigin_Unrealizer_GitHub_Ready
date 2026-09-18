using System.Text;
using FarsiOrigin.Unrealizer.Core;
using FarsiOrigin.Unrealizer.RTL;

namespace FarsiOrigin.Unrealizer;

public sealed class MainForm : Form
{
    private readonly TextBox gamePath = new()
    {
        Dock = DockStyle.Fill,
        PlaceholderText = "مسیر پوشه اصلی بازی را انتخاب کنید..."
    };

    private readonly TextBox log = new()
    {
        Dock = DockStyle.Fill,
        Multiline = true,
        ScrollBars = ScrollBars.Both,
        ReadOnly = true,
        Font = new Font("Consolas", 10)
    };

    private readonly TextBox preview = new()
    {
        Dock = DockStyle.Fill,
        Multiline = true,
        ScrollBars = ScrollBars.Vertical,
        RightToLeft = RightToLeft.Yes,
        Font = new Font("Tahoma", 14)
    };

    private string? workspace;

    public MainForm()
    {
        Text = "FARSI ORIGIN — Unrealizer v0.1";
        Width = 1100;
        Height = 720;
        StartPosition = FormStartPosition.CenterScreen;

        var scan = new Button { Text = "اسکن امن بازی", AutoSize = true };
        var workspaceButton = new Button { Text = "ساخت Workspace", AutoSize = true };
        var rtl = new Button { Text = "تست RTL", AutoSize = true };
        var font = new Button { Text = "افزودن فونت", AutoSize = true };

        scan.Click += (_, _) => Scan();
        workspaceButton.Click += (_, _) => CreateWorkspace();
        rtl.Click += (_, _) => ShowRtlPreview();
        font.Click += (_, _) => PickFont();

        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 85,
            ColumnCount = 5,
            Padding = new Padding(10)
        };

        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        top.Controls.Add(gamePath, 0, 0);
        top.Controls.Add(scan, 1, 0);
        top.Controls.Add(workspaceButton, 2, 0);
        top.Controls.Add(rtl, 3, 0);
        top.Controls.Add(font, 4, 0);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        var logTab = new TabPage("گزارش");
        var previewTab = new TabPage("RTL Preview");

        logTab.Controls.Add(log);
        previewTab.Controls.Add(preview);

        tabs.TabPages.Add(logTab);
        tabs.TabPages.Add(previewTab);

        Controls.Add(tabs);
        Controls.Add(top);
    }

    private void Scan()
    {
        try
        {
            if (!Directory.Exists(gamePath.Text))
            {
                using var dlg = new FolderBrowserDialog
                {
                    Description = "پوشه اصلی بازی را انتخاب کنید"
                };

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                gamePath.Text = dlg.SelectedPath;
            }

            var report = GameScanner.Scan(gamePath.Text);
            workspace ??= GameScanner.CreateWorkspace(gamePath.Text);
            GameScanner.SaveReport(report, workspace);

            var sb = new StringBuilder();
            sb.AppendLine("FARSI ORIGIN — SAFE SCAN");
            sb.AppendLine($"Workspace: {workspace}");
            sb.AppendLine($"EXE: {report.ExePath}");
            sb.AppendLine($"Content: {report.ContentPath}");
            sb.AppendLine($"Paks: {report.PaksPath}");
            sb.AppendLine($"IoStore: {report.HasIoStore}");
            sb.AppendLine($"PAK: {report.HasPak}");
            sb.AppendLine();

            sb.AppendLine("[Archives]");
            foreach (var a in report.Archives)
                sb.AppendLine("  " + a);

            sb.AppendLine();
            sb.AppendLine("[Candidates]");
            foreach (var c in report.Candidates.Take(100))
                sb.AppendLine("  " + c);

            sb.AppendLine();
            foreach (var x in report.Warnings)
                sb.AppendLine("WARNING: " + x);

            foreach (var x in report.Notes)
                sb.AppendLine("NOTE: " + x);

            log.Text = sb.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CreateWorkspace()
    {
        if (!Directory.Exists(gamePath.Text))
        {
            using var dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            gamePath.Text = dlg.SelectedPath;
        }

        workspace = GameScanner.CreateWorkspace(gamePath.Text);
        log.AppendText(Environment.NewLine + $"Workspace ساخته شد:\r\n{workspace}\r\n");
    }

    private void ShowRtlPreview()
    {
        var sample =
            "این یک تست فارسی برای FARSI ORIGIN است. " +
            "کلمه‌ها نباید بی‌دلیل جدا شوند. " +
            "شماره 123 و English Text هم باید سالم بماند.";

        preview.Text =
            PersianTextProcessor.SafePreview(sample) +
            Environment.NewLine + Environment.NewLine +
            PersianTextProcessor.PrepareLogicalRtl(sample);
    }

    private void PickFont()
    {
        using var dlg = new OpenFileDialog
        {
            Filter = "Fonts|*.ttf;*.otf"
        };

        if (dlg.ShowDialog() != DialogResult.OK)
            return;

        workspace ??= GameScanner.CreateWorkspace(gamePath.Text);

        var destination = Path.Combine(
            workspace,
            "Fonts",
            Path.GetFileName(dlg.FileName));

        File.Copy(dlg.FileName, destination, true);
        log.AppendText(Environment.NewLine + $"فونت ذخیره شد: {destination}\r\n");
    }
}
