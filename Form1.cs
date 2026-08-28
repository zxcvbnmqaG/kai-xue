using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace KaiXue;

public partial class Form1 : Form
{
    private static readonly Color AccentColor = Color.FromArgb(222, 54, 82);
    private static readonly Color AccentHoverColor = Color.FromArgb(244, 78, 103);
    private static readonly Color PanelColor = Color.FromArgb(218, 5, 10, 16);
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    private readonly System.Windows.Forms.Timer pulseTimer = new() { Interval = 250 };
    private Label stepLabel = null!;
    private Label questionLabel = null!;
    private Label descriptionLabel = null!;
    private Label tipLabel = null!;
    private Label statusLabel = null!;
    private FlowLayoutPanel choicePanel = null!;
    private bool completed;
    private int pulsePhase;

    public Form1()
    {
        Text = "开学.exe";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1180, 760);
        MinimumSize = new Size(900, 620);
        BackColor = Color.FromArgb(3, 8, 12);
        DoubleBuffered = true;
        BackgroundImage = LoadBackground();
        BackgroundImageLayout = ImageLayout.Stretch;

        BuildInterface();
        ShowOpeningQuestion();

        pulseTimer.Tick += (_, _) =>
        {
            if (completed)
            {
                statusLabel.Text = $"已通过 · {FormatDuration(stopwatch.Elapsed)}";
                return;
            }

            statusLabel.Text = $"记录中 · {FormatDuration(stopwatch.Elapsed)}";
            pulsePhase++;
            if (pulsePhase % 8 == 0)
            {
                questionLabel.ForeColor = questionLabel.ForeColor == Color.White
                    ? Color.FromArgb(255, 225, 228)
                    : Color.White;
            }
        };
        pulseTimer.Start();
        FormClosed += (_, _) => pulseTimer.Dispose();
    }

    private void BuildInterface()
    {
        var veil = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(204, 2, 7, 12),
            Padding = new Padding(72, 40, 72, 40)
        };
        Controls.Add(veil);

        var body = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };
        veil.Controls.Add(body);

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 58,
            BackColor = Color.Transparent
        };
        veil.Controls.Add(header);

        var brandLabel = new Label
        {
            Dock = DockStyle.Left,
            Width = 350,
            Text = "开学.exe",
            ForeColor = Color.White,
            Font = new Font("Microsoft YaHei UI", 15, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        header.Controls.Add(brandLabel);

        statusLabel = new Label
        {
            Dock = DockStyle.Right,
            Width = 300,
            Text = "记录中 · 00:00.00",
            ForeColor = Color.FromArgb(200, 214, 220),
            Font = new Font("Consolas", 10, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleRight
        };
        header.Controls.Add(statusLabel);

        var card = BuildCard();
        body.Controls.Add(card);
        body.Resize += (_, _) => CenterCard(body, card);
        CenterCard(body, card);
    }

    private Panel BuildCard()
    {
        var card = new Panel
        {
            Size = new Size(720, 500),
            BackColor = PanelColor,
            BorderStyle = BorderStyle.FixedSingle
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(42, 30, 42, 28),
            ColumnCount = 1,
            RowCount = 6,
            BackColor = Color.Transparent
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 128));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 1));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        card.Controls.Add(layout);

        stepLabel = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = AccentColor,
            Font = new Font("Consolas", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(stepLabel, 0, 0);

        questionLabel = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Microsoft YaHei UI", 28, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = false
        };
        layout.Controls.Add(questionLabel, 0, 1);

        descriptionLabel = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(218, 226, 230),
            Font = new Font("Microsoft YaHei UI", 13, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = false
        };
        layout.Controls.Add(descriptionLabel, 0, 2);

        choicePanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 8, 0, 0)
        };
        choicePanel.Resize += (_, _) => CenterChoices();
        layout.Controls.Add(choicePanel, 0, 3);

        var divider = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(110, 222, 54, 82)
        };
        layout.Controls.Add(divider, 0, 4);

        tipLabel = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(160, 185, 192),
            Font = new Font("Microsoft YaHei UI", 10, FontStyle.Regular),
            TextAlign = ContentAlignment.BottomLeft,
            AutoEllipsis = false
        };
        layout.Controls.Add(tipLabel, 0, 5);

        return card;
    }

    private void ShowOpeningQuestion()
    {
        Present(
            "01 / 06",
            "你开学了吗😄",
            "欢迎回来。先回答一个很简单的问题。",
            "无论你选什么，故事都会继续。",
            ("开学了", ShowHomeworkQuestion),
            ("还没开学", ShowHomeworkQuestion));
    }

    private void ShowHomeworkQuestion()
    {
        Present(
            "02 / 06",
            "你作业写完了吗",
            "请诚实作答。系统会记住你的选择。",
            "下面只有两个答案。",
            ("写完", ExitImmediately),
            ("没写完", ShowHomeworkWarning));
    }

    private void ShowHomeworkWarning()
    {
        Present(
            "03 / 06",
            "那你还玩电脑？\n写去啊",
            "检测到作业未完成。",
            "别装作没看见。",
            ("我这就去", ShowCopyQuestion));
    }

    private void ShowCopyQuestion()
    {
        Present(
            "04 / 06",
            "选择题",
            "作业不会写，可以抄吗？",
            "想清楚再按。",
            ("抄", ExitImmediately),
            ("不抄", ShowAbilityQuestion));
    }

    private void ShowAbilityQuestion()
    {
        Present(
            "05 / 06",
            "你 会 吗",
            "不抄的话，就凭自己的本事。",
            "这次请选真的。",
            ("不会", ShowLastQuestion),
            ("会", ExitImmediately));
    }

    private void ShowLastQuestion()
    {
        Present(
            "06 / 06",
            "最后一步",
            "请选择一个打开方式。",
            "任意选择都可以。",
            ("打开昨夜榜", ShowCompletion),
            ("笑园口酸", ShowCompletion));
    }

    private void ShowCompletion()
    {
        completed = true;
        Present(
            "07 / END",
            "你写完了",
            "9月1号到了\n你上交了作业",
            $"恭喜通过\n耗时：{FormatDuration(stopwatch.Elapsed)}",
            ("关闭程序", ExitImmediately));
        questionLabel.ForeColor = Color.FromArgb(255, 239, 241);
        statusLabel.Text = $"已通过 · {FormatDuration(stopwatch.Elapsed)}";
    }

    private void Present(
        string step,
        string question,
        string description,
        string tip,
        params (string Text, Action Action)[] choices)
    {
        stepLabel.Text = step;
        questionLabel.Text = question;
        questionLabel.ForeColor = Color.White;
        descriptionLabel.Text = description;
        tipLabel.Text = tip;
        choicePanel.SuspendLayout();
        choicePanel.Controls.Clear();
        foreach (var choice in choices)
        {
            choicePanel.Controls.Add(CreateChoiceButton(choice.Text, choice.Action));
        }

        choicePanel.ResumeLayout(true);
        CenterChoices();
    }

    private Button CreateChoiceButton(string text, Action action)
    {
        var button = new Button()
        {
            Text = text,
            Width = text.Length > 6 ? 230 : 190,
            Height = 58,
            Margin = new Padding(8, 8, 8, 8),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(190, 20, 26, 33),
            ForeColor = Color.White,
            Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand,
            TabStop = false
        };
        button.FlatAppearance.BorderColor = AccentColor;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(218, 74, 28, 46);
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(235, 125, 28, 48);
        button.Click += (_, _) => action();
        button.MouseEnter += (_, _) => button.ForeColor = AccentHoverColor;
        button.MouseLeave += (_, _) => button.ForeColor = Color.White;
        return button;
    }

    private void CenterCard(Control body, Control card)
    {
        card.Left = Math.Max(0, (body.ClientSize.Width - card.Width) / 2);
        card.Top = Math.Max(0, (body.ClientSize.Height - card.Height) / 2);
    }

    private void CenterChoices()
    {
        if (choicePanel is null || choicePanel.Controls.Count == 0)
        {
            return;
        }

        var buttonsWidth = choicePanel.Controls.Cast<Control>().Sum(control => control.Width + control.Margin.Horizontal);
        var leftPadding = Math.Max(0, (choicePanel.ClientSize.Width - buttonsWidth) / 2);
        choicePanel.Padding = new Padding(leftPadding, 8, 0, 0);
    }

    private void ExitImmediately()
    {
        pulseTimer.Stop();
        Application.Exit();
    }

    private static Image? LoadBackground()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "kai-xue-horror-background.png");
        if (!File.Exists(path))
        {
            return null;
        }

        using var source = Image.FromFile(path);
        return new Bitmap(source);
    }

    private static string FormatDuration(TimeSpan elapsed)
    {
        return $"{(int)elapsed.TotalMinutes:D2}:{elapsed.Seconds:D2}.{elapsed.Milliseconds / 10:D2}";
    }
}
