using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class PersonnalisationPage : Page, ISectionPage
{
    private SectionController? _controller;

    public PersonnalisationPage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateSections(ToolsPanel, ToolRegistry.GetByCategory("PERSONNALISATION"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
