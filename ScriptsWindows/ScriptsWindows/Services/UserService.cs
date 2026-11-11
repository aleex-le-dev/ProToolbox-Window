using System.Text.Json;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Compte local Windows (affiché dans la fenêtre « Comptes utilisateurs »).</summary>
public class LocalAccount
{
    public string Name { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Sid { get; set; } = "";
    public bool Enabled { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBuiltinAdmin { get; set; }   // RID-500
    public bool IsSystemAccount { get; set; }  // Invité, DefaultAccount, WDAGUtilityAccount
    public bool IsCurrent { get; set; }
    public bool PasswordRequired { get; set; }
    public string LastLogon { get; set; } = "";

    public string Subtitle =>
        string.Join(" • ", new[]
        {
            IsBuiltinAdmin ? "Administrateur intégré" : IsAdmin ? "Administrateur" : "Utilisateur standard",
            Enabled ? null : "Désactivé",
            // (PasswordRequired n'est pas affiché : Windows le renvoie à « faux » pour les comptes liés à un compte Microsoft)
            string.IsNullOrEmpty(LastLogon) ? "Jamais connecté" : $"Dernière connexion : {LastLogon}"
        }.Where(s => s != null));
}

/// <summary>
/// Gestion des comptes locaux. Les groupes et comptes intégrés sont désignés par leur SID
/// (indépendant de la langue de Windows : « Administrateurs » / « Administrators »…).
/// Toutes les valeurs saisies sont échappées avant d'être insérées dans un script.
/// </summary>
public class UserService
{
    private const string AdminsGroupSid = "S-1-5-32-544";

    /// <summary>Échappe une valeur pour une chaîne PowerShell entre apostrophes.</summary>
    private static string Q(string value) => "'" + (value ?? "").Replace("'", "''") + "'";

    public static async Task<List<LocalAccount>> GetAccountsAsync(CancellationToken ct = default)
    {
        string json = await PowerShellRunner.RunAsync($@"
$admins = @(Get-LocalGroupMember -SID '{AdminsGroupSid}' -ErrorAction SilentlyContinue | ForEach-Object {{ $_.SID.Value }})
$me = [Security.Principal.WindowsIdentity]::GetCurrent().User.Value
@(Get-LocalUser | ForEach-Object {{
    $sid = $_.SID.Value
    [PSCustomObject]@{{
        Name = $_.Name
        FullName = ""$($_.FullName)""
        Sid = $sid
        Enabled = [bool]$_.Enabled
        IsAdmin = $admins -contains $sid
        IsBuiltinAdmin = $sid -like '*-500'
        IsSystemAccount = $sid -match '-(501|503|504)$'
        IsCurrent = $sid -eq $me
        PasswordRequired = [bool]$_.PasswordRequired
        LastLogon = if ($_.LastLogon) {{ $_.LastLogon.ToString('dd/MM/yyyy HH:mm') }} else {{ '' }}
    }}
}}) | ConvertTo-Json -Compress
", ct);

        json = json.Trim();
        if (string.IsNullOrEmpty(json)) return new();
        if (json.StartsWith("{")) json = "[" + json + "]";
        return JsonSerializer.Deserialize<List<LocalAccount>>(json) ?? new();
    }

    public static Task CreateUser(string username, string password, bool makeAdmin, Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync($@"
$name = {Q(username)}
if ({Q(password)} -eq '') {{ New-LocalUser -Name $name -NoPassword -FullName $name -ErrorAction Stop | Out-Null }}
else {{ New-LocalUser -Name $name -Password (ConvertTo-SecureString {Q(password)} -AsPlainText -Force) -FullName $name -PasswordNeverExpires -ErrorAction Stop | Out-Null }}
Add-LocalGroupMember -SID 'S-1-5-32-545' -Member $name -ErrorAction SilentlyContinue
{(makeAdmin ? $"Add-LocalGroupMember -SID '{AdminsGroupSid}' -Member $name -ErrorAction Stop" : "")}
Write-Output ""[OK] Compte « $name » créé{(makeAdmin ? " (administrateur)" : "")}.""
", onOutput, ct);

    public static Task DeleteUser(string username, Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync($@"
Remove-LocalUser -Name {Q(username)} -ErrorAction Stop
Write-Output '[OK] Compte supprimé (le dossier du profil dans C:\Users n''est pas effacé).'
", onOutput, ct);

    public static Task SetAdmin(string username, bool makeAdmin, Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync(makeAdmin
            ? $"Add-LocalGroupMember -SID '{AdminsGroupSid}' -Member {Q(username)} -ErrorAction Stop; Write-Output '[OK] Droits administrateur accordés.'"
            : $"Remove-LocalGroupMember -SID '{AdminsGroupSid}' -Member {Q(username)} -ErrorAction Stop; Write-Output '[OK] Droits administrateur retirés.'",
            onOutput, ct);

    public static Task SetEnabled(string username, bool enable, Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync(enable
            ? $"Enable-LocalUser -Name {Q(username)} -ErrorAction Stop; Write-Output '[OK] Compte activé.'"
            : $"Disable-LocalUser -Name {Q(username)} -ErrorAction Stop; Write-Output '[OK] Compte désactivé.'",
            onOutput, ct);

    public static Task ResetPassword(string username, string newPassword, Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync(
            $"Set-LocalUser -Name {Q(username)} -Password (ConvertTo-SecureString {Q(newPassword)} -AsPlainText -Force) -ErrorAction Stop; Write-Output '[OK] Mot de passe modifié.'",
            onOutput, ct);

    public static Task RemovePassword(string username, Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync(
            $"Set-LocalUser -Name {Q(username)} -Password (New-Object System.Security.SecureString) -ErrorAction Stop; Write-Output '[OK] Mot de passe supprimé : connexion sans mot de passe.'",
            onOutput, ct);

    // ── Méthodes historiques (compatibilité) ──
    public static Task ListUsers(Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync("Get-LocalUser | Format-Table Name, Enabled, LastLogon, PasswordRequired -AutoSize", onOutput, ct);

    public static Task AddUser(string username, string password, Action<string> onOutput, CancellationToken ct = default)
        => CreateUser(username, password, false, onOutput, ct);

    /// <summary>Active / désactive le compte Administrateur intégré (RID-500), quel que soit son nom localisé.</summary>
    public static Task ToggleSuperAdmin(bool enable, Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync($@"
$adm = Get-LocalUser | Where-Object {{ $_.SID.Value -like '*-500' }}
{(enable ? "Enable-LocalUser" : "Disable-LocalUser")} -SID $adm.SID -ErrorAction Stop
Write-Output ""[OK] Compte « $($adm.Name) » {(enable ? "activé" : "désactivé")}.""
", onOutput, ct);
}
