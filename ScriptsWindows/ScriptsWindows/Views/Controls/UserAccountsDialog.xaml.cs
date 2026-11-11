using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

/// <summary>Gestion des comptes locaux (UserService) : droits, mots de passe, activation, création, suppression.</summary>
public partial class UserAccountsDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private bool _busy;

    public UserAccountsDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;
        Loaded += async (_, _) => await RefreshAsync();
    }

    private LocalAccount? Selected => ListAccounts.SelectedItem as LocalAccount;

    private async Task RefreshAsync(string? reselect = null)
    {
        SetBusy(true, "Chargement des comptes…");
        try
        {
            var accounts = await UserService.GetAccountsAsync(_ct);
            // Comptes système (Invité, DefaultAccount, WDAGUtilityAccount) en fin de liste
            var ordered = accounts.OrderBy(a => a.IsSystemAccount).ThenByDescending(a => a.IsCurrent).ThenBy(a => a.Name).ToList();
            ListAccounts.ItemsSource = ordered;
            ListAccounts.SelectedItem = ordered.FirstOrDefault(a => a.Name == reselect);
            SetBusy(false, $"{ordered.Count} compte(s) local(aux).");
        }
        catch (Exception ex)
        {
            SetBusy(false, $"Impossible de lire les comptes : {ex.Message}");
        }
    }

    private void SetBusy(bool busy, string status)
    {
        _busy = busy;
        ProgBar.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
        TxtStatus.Text = status;
        BtnRefresh.IsEnabled = !busy;
        UpdateActions();
    }

    private void ListAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateActions();

    private void UpdateActions()
    {
        var a = Selected;
        PanelActions.IsEnabled = a != null && !_busy;
        if (a == null)
        {
            TxtSelected.Text = "Sélectionnez un compte";
            TxtSelectedInfo.Text = "";
            return;
        }

        TxtSelected.Text = a.Name;
        TxtSelectedInfo.Text = a.Subtitle;
        BtnToggleAdmin.Content = a.IsAdmin ? "Retirer les droits administrateur" : "Accorder les droits administrateur";
        BtnToggleEnabled.Content = a.Enabled ? "Désactiver le compte" : "Activer le compte";

        // Garde-fous : ne pas se couper l'accès à soi-même ni supprimer un compte intégré
        BtnToggleAdmin.IsEnabled = !a.IsCurrent && !a.IsBuiltinAdmin;
        BtnToggleEnabled.IsEnabled = !a.IsCurrent;
        BtnDelete.IsEnabled = !a.IsCurrent && !a.IsBuiltinAdmin && !a.IsSystemAccount;
    }

    private async Task RunAsync(string status, Func<Task> action, string? reselect)
    {
        if (_busy) return;
        SetBusy(true, status);
        _onOutput($"\n[i] Comptes utilisateurs : {status}");
        try
        {
            await action();
            await RefreshAsync(reselect);
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private bool Confirm(string message, string title)
        => MessageBox.Show(this, message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    private async void BtnSetPassword_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is not { } a) return;
        if (PwdNew.Password.Length == 0) { TxtStatus.Text = "Saisissez le nouveau mot de passe."; return; }
        if (a.IsCurrent && !Confirm("Vous allez changer le mot de passe de la session en cours. Continuer ?", "Mot de passe")) return;
        string pwd = PwdNew.Password;
        PwdNew.Clear();
        await RunAsync($"Nouveau mot de passe pour « {a.Name} »", () => UserService.ResetPassword(a.Name, pwd, _onOutput, _ct), a.Name);
    }

    private async void BtnRemovePassword_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is not { } a) return;
        if (!Confirm($"Le compte « {a.Name} » pourra se connecter sans mot de passe. Continuer ?", "Supprimer le mot de passe")) return;
        await RunAsync($"Suppression du mot de passe de « {a.Name} »", () => UserService.RemovePassword(a.Name, _onOutput, _ct), a.Name);
    }

    private async void BtnToggleAdmin_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is not { } a) return;
        bool make = !a.IsAdmin;
        await RunAsync(make ? $"Droits administrateur pour « {a.Name} »" : $"Retrait des droits administrateur de « {a.Name} »",
            () => UserService.SetAdmin(a.Name, make, _onOutput, _ct), a.Name);
    }

    private async void BtnToggleEnabled_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is not { } a) return;
        bool enable = !a.Enabled;
        if (!enable && !Confirm($"Le compte « {a.Name} » ne pourra plus ouvrir de session. Continuer ?", "Désactiver le compte")) return;
        await RunAsync(enable ? $"Activation de « {a.Name} »" : $"Désactivation de « {a.Name} »",
            () => UserService.SetEnabled(a.Name, enable, _onOutput, _ct), a.Name);
    }

    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is not { } a) return;
        if (!Confirm($"Supprimer définitivement le compte « {a.Name} » ?\n\nSon dossier de profil (C:\\Users\\{a.Name}) est conservé : pensez à sauvegarder ses données si besoin.",
                     "Supprimer le compte")) return;
        await RunAsync($"Suppression de « {a.Name} »", () => UserService.DeleteUser(a.Name, _onOutput, _ct), null);
    }

    private async void BtnCreate_Click(object sender, RoutedEventArgs e)
    {
        string name = TxtNewName.Text.Trim();
        if (name.Length == 0) { TxtStatus.Text = "Saisissez un nom d'utilisateur."; return; }
        if (name.IndexOfAny("\"/\\[]:;|=,+*?<>@".ToCharArray()) >= 0 || name.Length > 20)
        {
            TxtStatus.Text = "Nom invalide : 20 caractères maximum, sans \" / \\ [ ] : ; | = , + * ? < > @";
            return;
        }
        string pwd = PwdCreate.Password;
        bool admin = ChkCreateAdmin.IsChecked == true;
        await RunAsync($"Création du compte « {name} »", () => UserService.CreateUser(name, pwd, admin, _onOutput, _ct), name);
        TxtNewName.Clear(); PwdCreate.Clear(); ChkCreateAdmin.IsChecked = false;
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e) => await RefreshAsync(Selected?.Name);

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
