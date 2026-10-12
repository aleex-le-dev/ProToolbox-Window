using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class SecretsPage : Page, ISectionPage
{
    private SectionController? _controller;

    public SecretsPage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("SECRETS"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
