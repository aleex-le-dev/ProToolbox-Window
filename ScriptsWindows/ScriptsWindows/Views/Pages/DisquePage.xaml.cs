using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class DisquePage : Page
{
    private bool _loaded;

    public DisquePage()
    {
        InitializeComponent();
        Loaded += (s, e) => LoadTools();
    }

    private void LoadTools()
    {
        if (_loaded) return;
        _loaded = true;

        SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("DISQUE"));
    }
}
