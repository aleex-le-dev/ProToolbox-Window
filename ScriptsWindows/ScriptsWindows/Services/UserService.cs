using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service utilisateurs — um_* du .bat.</summary>
public class UserService
{
    public static async Task ListUsers(Action<string> onOutput, CancellationToken ct = default)
        => await PowerShellRunner.RunAsync("Get-LocalUser | Format-Table Name, Enabled, LastLogon, PasswordRequired -AutoSize", onOutput, ct);

    public static async Task AddUser(string username, string password, Action<string> onOutput, CancellationToken ct = default)
        => await PowerShellRunner.RunAsync($"New-LocalUser -Name '{username}' -Password (ConvertTo-SecureString '{password}' -AsPlainText -Force) -FullName '{username}'; Write-Output '[OK] Utilisateur créé'", onOutput, ct);

    public static async Task DeleteUser(string username, Action<string> onOutput, CancellationToken ct = default)
        => await PowerShellRunner.RunAsync($"Remove-LocalUser -Name '{username}'; Write-Output '[OK] Utilisateur supprimé'", onOutput, ct);

    public static async Task SetAdmin(string username, bool makeAdmin, Action<string> onOutput, CancellationToken ct = default)
    {
        if (makeAdmin)
            await PowerShellRunner.RunAsync($"Add-LocalGroupMember -Group 'Administrators' -Member '{username}'; Write-Output '[OK] Droits admin accordés'", onOutput, ct);
        else
            await PowerShellRunner.RunAsync($"Remove-LocalGroupMember -Group 'Administrators' -Member '{username}'; Write-Output '[OK] Droits admin retirés'", onOutput, ct);
    }

    public static async Task ResetPassword(string username, string newPassword, Action<string> onOutput, CancellationToken ct = default)
        => await PowerShellRunner.RunAsync($"Set-LocalUser -Name '{username}' -Password (ConvertTo-SecureString '{newPassword}' -AsPlainText -Force); Write-Output '[OK] Mot de passe modifié'", onOutput, ct);

    public static async Task RemovePassword(string username, Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync($"net user {username} \"\"", onOutput, ct);

    public static async Task ToggleSuperAdmin(bool enable, Action<string> onOutput, CancellationToken ct = default)
    {
        var action = enable ? "active" : "inactive";
        await CommandRunner.RunAsync($"net user administrator /{action}", onOutput, ct);
        onOutput($"[OK] Compte Administrateur (RID-500) {(enable ? "activé" : "désactivé")}");
    }
}
