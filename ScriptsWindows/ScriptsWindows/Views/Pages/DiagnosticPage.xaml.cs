using System.Windows.Controls;
using ScriptsWindows.Core;
using ScriptsWindows.Views.Controls;

namespace ScriptsWindows.Views.Pages;

public partial class DiagnosticPage : Page
{
    private bool _loaded;

    public DiagnosticPage()
    {
        InitializeComponent();
        Loaded += (s, e) => LoadTools();
    }

    private void LoadTools()
    {
        if (_loaded) return;
        _loaded = true;

        ToolsPanel.Children.Clear();
        foreach (var tool in ToolRegistry.GetByCategory("DIAGNOSTIC"))
        {
            var card = new ToolCard();
            card.BindTool(tool);
            ToolsPanel.Children.Add(card);
        }
    }
}
