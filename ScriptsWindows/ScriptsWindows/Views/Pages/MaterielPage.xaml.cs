using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class MaterielPage : Page, ISectionPage
{
    private SectionController? _controller;

    public MaterielPage()
    {
        InitializeComponent();
        _controller = SectionViewBuilder.PopulateDirect(ToolsPanel, ToolRegistry.GetByCategory("MATERIEL"));
    }

    public void ResetSections() => _controller?.CollapseAll();
}
