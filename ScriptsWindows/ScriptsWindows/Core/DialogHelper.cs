using System.Windows;

namespace ScriptsWindows.Core;

/// <summary>Ouverture standard d'une fenêtre d'outil : modale, centrée sur la fenêtre principale, sur le thread UI.</summary>
public static class DialogHelper
{
    public static Task ShowDialogAsync(Func<Window> factory)
        => Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var window = factory();
            if (Application.Current.MainWindow is { IsLoaded: true } owner && !ReferenceEquals(owner, window))
                window.Owner = owner;
            window.ShowDialog();
        }).Task;
}
