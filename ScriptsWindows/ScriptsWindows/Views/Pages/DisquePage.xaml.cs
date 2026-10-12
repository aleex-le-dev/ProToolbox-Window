using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class DisquePage : Page, ISectionPage
{
    private SectionController? _controller;

    public DisquePage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("DISQUE"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
