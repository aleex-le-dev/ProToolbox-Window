using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class SecuritePage : Page, ISectionPage
{
    private SectionController? _controller;

    public SecuritePage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("SECURITE"));
    }

    public void ResetSections() => _controller?.ExpandAll();
}
