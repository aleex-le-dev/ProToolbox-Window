using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class DemarragePage : Page, ISectionPage
{
    private SectionController? _controller;

    public DemarragePage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("DEMARRAGE"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
