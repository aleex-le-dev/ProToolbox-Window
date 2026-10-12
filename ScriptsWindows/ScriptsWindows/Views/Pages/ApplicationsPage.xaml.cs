using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class ApplicationsPage : Page
{
    private bool _loaded;

    public ApplicationsPage()
    {
        InitializeComponent();
        Loaded += (s, e) => LoadTools();
    }

    private void LoadTools()
    {
        if (_loaded) return;
        _loaded = true;

        SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("APPLICATIONS"));
    }
}
