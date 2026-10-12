using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class MisesAJourPage : Page, ISectionPage
{
    private SectionController? _controller;

    public MisesAJourPage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateDirect(ToolsPanel, ToolRegistry.GetByCategory("MISES_A_JOUR"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
