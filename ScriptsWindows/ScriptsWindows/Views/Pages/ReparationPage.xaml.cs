using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class ReparationPage : Page, ISectionPage
{
    private SectionController? _controller;

    public ReparationPage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("REPARATION"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
