using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class ApplicationsPage : Page, ISectionPage
{
    private SectionController? _controller;

    public ApplicationsPage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("APPLICATIONS"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
