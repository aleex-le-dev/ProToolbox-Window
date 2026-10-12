using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class ReseauPage : Page, ISectionPage
{
    private SectionController? _controller;

    public ReseauPage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("RESEAU"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
