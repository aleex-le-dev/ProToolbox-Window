using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ScriptsWindows.Core;

/// <summary>
/// Gestionnaire centralisé des boîtes de dialogue de sélection d'emplacement et d'export.
/// Assure une expérience homogène : confirmation, sélection à la souris, enregistrement direct de fichier ou dossier, et ouverture dans l'Explorateur.
/// </summary>
public static class FolderDialogHelper
{
    /// <summary>
    /// Affiche une boîte de dialogue standard "Enregistrer sous" (SaveFileDialog)
    /// pour permettre à l'utilisateur de choisir directement l'emplacement et le nom du fichier sans créer de sous-dossier intermédiaire.
    /// Retourne le chemin complet du fichier sélectionné, ou null si annulé/refusé.
    /// </summary>
    public static string? AskSaveFilePath(
        string title,
        string defaultFileName,
        string filter = "Fichier texte (*.txt)|*.txt|Tous les fichiers (*.*)|*.*",
        string defaultExt = ".txt",
        string? confirmQuestion = null,
        string? confirmTitle = "Sauvegarde — Alex Toolkit")
    {
        string? selectedFilePath = null;

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

                var dialog = new SaveFileDialog
                {
                    Title = title,
                    FileName = defaultFileName,
                    Filter = filter,
                    DefaultExt = defaultExt,
                    AddExtension = true,
                    OverwritePrompt = true
                };

                if (dialog.ShowDialog(Application.Current.MainWindow) == true && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    selectedFilePath = dialog.FileName;
                }
            });
        }
        catch (Exception)
        {
            // Échec éventuel d'invocation UI
        }

        return selectedFilePath;
    }

    /// <summary>
    /// Affiche une boîte de dialogue pour demander un emplacement et y crée un sous-dossier horodaté (utilisé pour les lots de documents sensibles, pilotes, etc.).
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
    /// Ouvre le dossier spécifié ou sélectionne directement le fichier dans l'Explorateur Windows.
    /// </summary>
    public static void OpenInExplorer(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{path}\"",
                    UseShellExecute = true
                });
            }
            else if (Directory.Exists(path))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
        }
        catch { }
    }
}
