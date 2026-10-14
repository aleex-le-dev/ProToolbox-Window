using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class DiagnosticPage : Page, ISectionPage
{
    private SectionController? _controller;

    public DiagnosticPage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("DIAGNOSTIC"));
    }

    public void ResetSections() => _controller?.ExpandAll();
}
