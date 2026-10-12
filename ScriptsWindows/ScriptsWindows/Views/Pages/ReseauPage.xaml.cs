using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class ReseauPage : Page
{
    private bool _loaded;

    public ReseauPage()
    {
        InitializeComponent();
        Loaded += (s, e) => LoadTools();
    }

    private void LoadTools()
    {
        if (_loaded) return;
        _loaded = true;

        SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("RESEAU"));
    }
}
