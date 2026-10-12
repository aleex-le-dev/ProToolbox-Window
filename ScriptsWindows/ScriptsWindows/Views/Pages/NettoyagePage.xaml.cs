using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class NettoyagePage : Page, ISectionPage
{
    private SectionController? _controller;

    public NettoyagePage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("NETTOYAGE"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
