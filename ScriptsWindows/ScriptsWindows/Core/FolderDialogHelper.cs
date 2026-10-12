using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ScriptsWindows.Core;

/// <summary>
/// Gestionnaire centralisé des boîtes de dialogue de sélection d'emplacement et d'export.
/// Assure une expérience homogène : confirmation, sélection à la souris, création d'un dossier horodaté et ouverture dans l'Explorateur.
/// </summary>
public static class FolderDialogHelper
{
    /// <summary>
    /// Affiche une boîte de dialogue pour demander un emplacement de sauvegarde et y crée un sous-dossier horodaté.
    /// Si confirmQuestion est renseigné, pose d'abord la question à l'utilisateur via une MessageBox Yes/No.
    /// Retourne le chemin complet du dossier créé, ou null si l'utilisateur a annulé ou refusé.
    /// </summary>
    public static string? AskAndCreateDestinationFolder(
        string title,
        string folderPrefix,
        string? confirmQuestion = null,
        string? confirmTitle = "Sauvegarde — Alex Toolkit")
    {
        string? createdFolder = null;

        try
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(confirmQuestion))
                {
                    var result = MessageBox.Show(
                        Application.Current.MainWindow,
                        confirmQuestion,
                        confirmTitle ?? "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes)
                        return;
                }

                var dialog = new OpenFolderDialog
                {
                    Title = title,
                    Multiselect = false
                };

                if (dialog.ShowDialog(Application.Current.MainWindow) == true && !string.IsNullOrWhiteSpace(dialog.FolderName))
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string targetDir = Path.Combine(dialog.FolderName, $"{folderPrefix}_{timestamp}");
                    Directory.CreateDirectory(targetDir);
                    createdFolder = targetDir;
                }
            });
        }
        catch (Exception)
        {
            // Échec éventuel d'invocation UI
        }

        return createdFolder;
    }

    /// <summary>
    /// Ouvre le dossier spécifié dans l'Explorateur Windows.
    /// </summary>
    public static void OpenInExplorer(string folderPath)
    {
        try
        {
            if (Directory.Exists(folderPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = folderPath,
                    UseShellExecute = true
                });
            }
        }
        catch { }
    }
}
