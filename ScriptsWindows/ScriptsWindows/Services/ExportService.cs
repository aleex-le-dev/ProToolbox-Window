using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.Sqlite;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>
/// Service d'exportation et d'audit de sécurité :
/// Clé Windows, pilotes, logiciels, historique navigateurs SQLite et audit des fichiers sensibles.
/// </summary>
public class ExportService
{
    // ─── 1. Clé de produit Windows ───────────────────────────────────────────
    public static async Task GetWindowsKey(Action<string> onOutput, CancellationToken ct = default)
    {
        string? foundKey = null;
        string? source = null;

        await PowerShellRunner.RunAsync(@"
$key = (Get-WmiObject -query 'select * from SoftwareLicensingService').OA3xOriginalProductKey
if ($key) {
    Write-Output ""[OK] Clé produit Windows BIOS/UEFI : $key""
} else {
    $regPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SoftwareProtectionPlatform'
    $key2 = (Get-ItemProperty -Path $regPath -Name BackupProductKeyDefault -ErrorAction SilentlyContinue).BackupProductKeyDefault
    if ($key2) {
        Write-Output ""[OK] Clé Windows (Registre de secours) : $key2""
    } else {
        Write-Output '[!] Aucune clé OEM physique en mémoire UEFI (Licence numérique liée au compte Microsoft).'
    }
}
", line =>
        {
            if (line.Contains("Clé produit Windows BIOS/UEFI :"))
            {
                var idx = line.IndexOf(':');
                if (idx >= 0) { foundKey = line.Substring(idx + 1).Trim(); source = "BIOS/UEFI (OA3x)"; }
            }
            else if (line.Contains("Clé Windows (Registre de secours) :"))
            {
                var idx = line.IndexOf(':');
                if (idx >= 0) { foundKey = line.Substring(idx + 1).Trim(); source = "Registre de secours"; }
            }
            onOutput(line);
        }, ct);

        if (!string.IsNullOrWhiteSpace(foundKey))
        {
            var targetFilePath = FolderDialogHelper.AskSaveFilePath(
                title: "Enregistrer la clé produit Windows",
                defaultFileName: "Cle_Produit_Windows.txt",
                confirmQuestion: $"Une clé de licence Windows a été détectée ({source}) :\n\n{foundKey}\n\nSouhaitez-vous enregistrer cette clé dans un fichier texte à l'emplacement de votre choix ?",
                confirmTitle: "Sauvegarde de la clé Windows — Alex Toolkit"
            );

            if (!string.IsNullOrEmpty(targetFilePath))
            {
                var content = $"===========================================================\n" +
                              $" CLÉ DE PRODUIT WINDOWS — {Environment.MachineName}\n" +
                              $" Date d'export : {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                              $" Source        : {source}\n" +
                              $"===========================================================\n\n" +
                              $"Clé de licence : {foundKey}\n";
                await File.WriteAllTextAsync(targetFilePath, content, ct);
                onOutput($"\n✅ [SUCCÈS] Clé de licence enregistrée directement dans : {targetFilePath}");
                FolderDialogHelper.OpenInExplorer(targetFilePath);
            }
            else
            {
                onOutput("\n[i] Sauvegarde de la clé ignorée ou annulée par l'utilisateur.");
            }
        }
    }

    // ─── 2. Export des pilotes tiers système ─────────────────────────────────
    public static async Task ExportDrivers(Action<string> onOutput, CancellationToken ct = default)
    {
        var targetDir = FolderDialogHelper.AskAndCreateDestinationFolder(
            title: "Sélectionnez l'emplacement où sauvegarder les pilotes système",
            folderPrefix: "Sauvegarde_Pilotes_Systeme",
            confirmQuestion: "L'exportation DISM va extraire et rassembler tous les pilotes tiers installés sur votre ordinateur.\n\nSouhaitez-vous sélectionner le dossier où créer la sauvegarde ?",
            confirmTitle: "Sauvegarde des pilotes — Alex Toolkit"
        );

        if (string.IsNullOrEmpty(targetDir))
        {
            onOutput("[i] Sauvegarde des pilotes annulée par l'utilisateur.");
            return;
        }

        onOutput($"[i] Dossier cible créé : {targetDir}");
        onOutput("[i] Sauvegarde et extraction de tous les pilotes système tiers via DISM (cette opération peut prendre quelques minutes)...");
        await CommandRunner.RunAsync($"DISM /Online /Export-Driver /Destination:\"{targetDir}\"", onOutput, ct);
        onOutput($"\n✅ [SUCCÈS] Pilotes exportés avec succès dans : {targetDir}");
        FolderDialogHelper.OpenInExplorer(targetDir);
    }

    // ─── 3. Export de la liste des logiciels installés ───────────────────────
    public static async Task ExportSoftwareList(Action<string> onOutput, CancellationToken ct = default)
    {
        var dest = FolderDialogHelper.AskSaveFilePath(
            title: "Enregistrer la liste des logiciels",
            defaultFileName: "liste_logiciels.txt",
            confirmQuestion: "Souhaitez-vous exporter la liste complète des logiciels installés dans un fichier texte à l'emplacement de votre choix ?",
            confirmTitle: "Export inventaire des logiciels — Alex Toolkit"
        );

        if (string.IsNullOrEmpty(dest))
        {
            onOutput("[i] Export de la liste des logiciels annulé par l'utilisateur.");
            return;
        }

        onOutput($"[i] Export de la liste des logiciels via winget vers {dest}...");
        await CommandRunner.RunAsync($"winget list > \"{dest}\"", onOutput, ct);
        onOutput($"\n✅ [SUCCÈS] Liste des logiciels exportée directement dans : {dest}");
        FolderDialogHelper.OpenInExplorer(dest);
    }

    // ─── 4. Analyse & Extraction de l'historique web (SQLite) ────────────────
    public static async Task AnalyzeBrowserHistory(Action<string> onOutput, CancellationToken ct = default)
    {
        try
        {
            SQLitePCL.Batteries_V2.Init();
        }
        catch { }

        onOutput("═══════════════════════════════════════════════════════════════");
        onOutput("  🌐 ANALYSE ET EXTRACTION DE L'HISTORIQUE DE NAVIGATION");
        onOutput("═══════════════════════════════════════════════════════════════");

        await Task.Run(() =>
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            int totalExtracted = 0;

            // Google Chrome
            var chromeProfiles = new[]
            {
                Path.Combine(localAppData, @"Google\Chrome\User Data\Default\History"),
                Path.Combine(localAppData, @"Google\Chrome\User Data\Profile 1\History"),
                Path.Combine(localAppData, @"Google\Chrome\User Data\Profile 2\History")
            };
            foreach (var hist in chromeProfiles)
            {
                if (File.Exists(hist))
                {
                    totalExtracted += ExtractChromiumHistory("Google Chrome", hist, onOutput, ct);
                    break;
                }
            }

            // Microsoft Edge
            var edgeProfiles = new[]
            {
                Path.Combine(localAppData, @"Microsoft\Edge\User Data\Default\History"),
                Path.Combine(localAppData, @"Microsoft\Edge\User Data\Profile 1\History"),
                Path.Combine(localAppData, @"Microsoft\Edge\User Data\Profile 2\History")
            };
            foreach (var hist in edgeProfiles)
            {
                if (File.Exists(hist))
                {
                    totalExtracted += ExtractChromiumHistory("Microsoft Edge", hist, onOutput, ct);
                    break;
                }
            }

            // Brave Browser
            string braveHist = Path.Combine(localAppData, @"BraveSoftware\Brave-Browser\User Data\Default\History");
            if (File.Exists(braveHist))
            {
                totalExtracted += ExtractChromiumHistory("Brave Browser", braveHist, onOutput, ct);
            }

            // Mozilla Firefox
            totalExtracted += ExtractFirefoxHistory(onOutput, ct);

            onOutput("\n═══════════════════════════════════════════════════════════════");
            onOutput($"✅ Total : {totalExtracted} entrées d'historique extraites.");
            onOutput("═══════════════════════════════════════════════════════════════");
        }, ct);
    }

    private static int ExtractChromiumHistory(string browserName, string historyPath, Action<string> onOutput, CancellationToken ct)
    {
        if (!File.Exists(historyPath)) return 0;

        var fi = new FileInfo(historyPath);
        onOutput($"\n--- 🌐 HISTORIQUE {browserName.ToUpperInvariant()} ---");
        onOutput($"Fichier SQLite : {historyPath} ({Math.Round(fi.Length / 1024.0, 1)} Ko)");

        string tempFile = Path.Combine(Path.GetTempPath(), $"{browserName.Replace(" ", "_")}_hist_{Guid.NewGuid():N}.tmp");
        int count = 0;

        try
        {
            // Copie partagée en lecture (contourne le verrouillage si le navigateur est ouvert)
            using (var src = new FileStream(historyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var dst = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                src.CopyTo(dst);
            }

            var cs = new SqliteConnectionStringBuilder
            {
                DataSource = tempFile,
                Mode = SqliteOpenMode.ReadOnly
            }.ToString();

            using var conn = new SqliteConnection(cs);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT title, url, visit_count, last_visit_time
                FROM urls
                WHERE url IS NOT NULL 
                  AND url NOT LIKE 'chrome://%' 
                  AND url NOT LIKE 'edge://%' 
                  AND url NOT LIKE 'chrome-extension://%'
                  AND url NOT LIKE 'about:%'
                  AND last_visit_time > 0
                ORDER BY last_visit_time DESC
                LIMIT 35;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (ct.IsCancellationRequested) break;
                count++;

                string title = reader.IsDBNull(0) ? "" : reader.GetString(0);
                string url = reader.IsDBNull(1) ? "" : reader.GetString(1);
                long visits = reader.IsDBNull(2) ? 1 : reader.GetInt64(2);
                long rawTime = reader.IsDBNull(3) ? 0 : reader.GetInt64(3);

                string dateStr = "Date inconnue";
                try
                {
                    if (rawTime > 0)
                    {
                        var dt = DateTime.FromFileTimeUtc(rawTime * 10).ToLocalTime();
                        dateStr = dt.ToString("dd/MM/yyyy HH:mm");
                    }
                }
                catch { }

                if (string.IsNullOrWhiteSpace(title)) title = url;
                if (title.Length > 85) title = title.Substring(0, 82) + "...";

                onOutput($"[{count:D2}] 📅 {dateStr} | {title}");
                onOutput($"     🔗 {url} (Visité {visits} fois)");
            }

            if (count == 0)
            {
                onOutput("[i] Aucun historique web récent trouvé dans cette base.");
            }
            else
            {
                onOutput($"[OK] {count} dernières visites listées pour {browserName}.");
            }
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
            onOutput($"[!] Erreur lors de la lecture SQLite pour {browserName} : {msg}");
        }
        finally
        {
            try
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
            catch { }
        }

        return count;
    }

    private static int ExtractFirefoxHistory(Action<string> onOutput, CancellationToken ct)
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string profilesDir = Path.Combine(appData, @"Mozilla\Firefox\Profiles");
        if (!Directory.Exists(profilesDir)) return 0;

        int count = 0;
        foreach (var dir in Directory.GetDirectories(profilesDir))
        {
            string places = Path.Combine(dir, "places.sqlite");
            if (!File.Exists(places)) continue;

            onOutput("\n--- 🦊 HISTORIQUE MOZILLA FIREFOX ---");
            string tempFile = Path.Combine(Path.GetTempPath(), $"firefox_hist_{Guid.NewGuid():N}.tmp");
            try
            {
                using (var src = new FileStream(places, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var dst = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    src.CopyTo(dst);
                }

                var cs = new SqliteConnectionStringBuilder
                {
                    DataSource = tempFile,
                    Mode = SqliteOpenMode.ReadOnly
                }.ToString();

                using var conn = new SqliteConnection(cs);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT title, url, visit_count, last_visit_date
                    FROM moz_places
                    WHERE url IS NOT NULL AND last_visit_date IS NOT NULL
                    ORDER BY last_visit_date DESC
                    LIMIT 35;";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (ct.IsCancellationRequested) break;
                    count++;

                    string title = reader.IsDBNull(0) ? "" : reader.GetString(0);
                    string url = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    long visits = reader.IsDBNull(2) ? 1 : reader.GetInt64(2);
                    long rawTime = reader.IsDBNull(3) ? 0 : reader.GetInt64(3);

                    string dateStr = "Date inconnue";
                    try
                    {
                        if (rawTime > 0)
                        {
                            var dt = DateTimeOffset.FromUnixTimeMilliseconds(rawTime / 1000).LocalDateTime;
                            dateStr = dt.ToString("dd/MM/yyyy HH:mm");
                        }
                    }
                    catch { }

                    if (string.IsNullOrWhiteSpace(title)) title = url;
                    if (title.Length > 85) title = title.Substring(0, 82) + "...";

                    onOutput($"[{count:D2}] 📅 {dateStr} | {title}");
                    onOutput($"     🔗 {url} (Visité {visits} fois)");
                }

                if (count > 0)
                    onOutput($"[OK] {count} dernières visites listées pour Firefox.");
            }
            catch (Exception ex)
            {
                onOutput($"[!] Erreur Firefox : {ex.Message}");
            }
            finally
            {
                try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { }
            }
            break;
        }

        return count;
    }

    // ─── 5. Audit approfondi des documents et secrets sensibles ──────────────
    public static async Task ScanSensitiveDocs(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("═══════════════════════════════════════════════════════════════");
        onOutput("  🔍 AUDIT DE SÉCURITÉ : CLÉS & DOCUMENTS SENSIBLES LOCAUX");
        onOutput("═══════════════════════════════════════════════════════════════");
        onOutput("[i] Analyse des clés privées, configurations Cloud, jetons d'API et documents sensibles...");

        await Task.Run(() =>
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var findings = new List<(string Category, string Path, string Details, string Icon)>();

            // 1. Clés SSH & Cryptographiques (.ssh)
            string sshDir = Path.Combine(userProfile, ".ssh");
            if (Directory.Exists(sshDir))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(sshDir))
                    {
                        var fi = new FileInfo(file);
                        string name = fi.Name.ToLowerInvariant();
                        bool isPrivate = name.Contains("id_") && !name.EndsWith(".pub");
                        string icon = isPrivate ? "🔴" : "🔑";
                        string cat = isPrivate ? "Clé privée SSH (Critique)" : "Configuration / Clé SSH";
                        findings.Add((cat, fi.FullName, $"{Math.Round(fi.Length / 1024.0, 1)} Ko, Modifié le {fi.LastWriteTime:dd/MM/yyyy HH:mm}", icon));
                    }
                }
                catch { }
            }

            // 2. Fichiers de configuration Cloud & Outils DevOps (.aws, .azure, .kube, .docker, etc.)
            var devOpsConfigs = new[]
            {
                Path.Combine(userProfile, @".aws\credentials"),
                Path.Combine(userProfile, @".aws\config"),
                Path.Combine(userProfile, @".kube\config"),
                Path.Combine(userProfile, @".docker\config.json"),
                Path.Combine(userProfile, ".git-credentials"),
                Path.Combine(userProfile, ".netrc"),
                Path.Combine(userProfile, ".npmrc")
            };
            foreach (var path in devOpsConfigs)
            {
                if (File.Exists(path))
                {
                    var fi = new FileInfo(path);
                    findings.Add(("Configuration Cloud / DevOps", fi.FullName, $"{Math.Round(fi.Length / 1024.0, 1)} Ko, Modifié le {fi.LastWriteTime:dd/MM/yyyy HH:mm}", "☁️"));
                }
            }

            // 3. Analyse des dossiers de travail : Bureau, Documents, Téléchargements
            var searchFolders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.Combine(userProfile, "Downloads")
            };

            var sensitiveKeywords = new[] { "password", "mot de passe", "motdepasse", "mdp", "secret", "token", "credential", "iban", "rib", "compte", "identifiant", "api_key", "apikey" };
            var certExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".kdbx", ".pem", ".key", ".pfx", ".p12", ".ppk", ".env" };
            var docExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".txt", ".docx", ".xlsx", ".pdf", ".json", ".csv", ".ini", ".cfg", ".xml" };

            foreach (var baseDir in searchFolders)
            {
                if (!Directory.Exists(baseDir)) continue;
                ScanDirectorySafe(baseDir, 0, maxDepth: 3, findings, sensitiveKeywords, certExtensions, docExtensions, ct);
            }

            // Affichage des résultats
            if (findings.Count == 0)
            {
                onOutput("\n✅ Aucun fichier sensible, clé privée ou mot de passe non protégé détecté dans les dossiers analysés.");
            }
            else
            {
                onOutput($"\n⚠️ {findings.Count} éléments sensibles répertoriés sur le système :\n");

                // Grouper par catégorie pour une lecture limpide
                var grouped = findings.GroupBy(f => f.Category);
                foreach (var group in grouped)
                {
                    onOutput($"📁 [{group.Key.ToUpperInvariant()}] ({group.Count()} trouvés)");
                    foreach (var item in group)
                    {
                        onOutput($"   {item.Icon} {item.Path}");
                        onOutput($"      └─ Info : {item.Details}");
                    }
                    onOutput("");
                }

                // ─── Boîte de dialogue Windows native pour choisir l'emplacement à la souris ───
                var targetDir = FolderDialogHelper.AskAndCreateDestinationFolder(
                    title: "Sélectionnez l'emplacement où copier tous les documents sensibles",
                    folderPrefix: "Sauvegarde_Secrets",
                    confirmQuestion: $"{findings.Count} document(s) et clé(s) sensible(s) ont été identifiés sur votre ordinateur.\n\nSouhaitez-vous copier et rassembler l'ensemble de ces fichiers dans un dossier de sauvegarde sécurisé de votre choix ?",
                    confirmTitle: "Sauvegarde des documents sensibles — Alex Toolkit"
                );

                if (!string.IsNullOrWhiteSpace(targetDir))
                {
                    onOutput("\n═══════════════════════════════════════════════════════════════");
                    onOutput("  📁 EXPORTATION SÉCURISÉE DES DOCUMENTS SENSIBLES");
                    onOutput("═══════════════════════════════════════════════════════════════");
                    onOutput($"[i] Dossier de destination créé : {targetDir}\n");

                    int copiedCount = 0;
                    int errorCount = 0;

                    foreach (var item in findings)
                    {
                        if (ct.IsCancellationRequested) break;
                        try
                        {
                            if (!File.Exists(item.Path)) continue;

                            // Sous-dossier ordonné par catégorie
                            string sanitizedCat = CleanFolderName(item.Category);
                            string catDir = Path.Combine(targetDir, sanitizedCat);
                            if (!Directory.Exists(catDir)) Directory.CreateDirectory(catDir);

                            string fileName = Path.GetFileName(item.Path);
                            string destPath = Path.Combine(catDir, fileName);

                            // Gestion des collisions de noms
                            int suffix = 1;
                            while (File.Exists(destPath))
                            {
                                string nameOnly = Path.GetFileNameWithoutExtension(fileName);
                                string ext = Path.GetExtension(fileName);
                                destPath = Path.Combine(catDir, $"{nameOnly}_{suffix++}{ext}");
                            }

                            // Copie partagée sécurisée (contourne les éventuels verrous applicatifs)
                            using (var src = new FileStream(item.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            using (var dst = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                src.CopyTo(dst);
                            }

                            copiedCount++;
                            onOutput($"[COPIE] {item.Icon} {fileName} ➜ {sanitizedCat}\\{Path.GetFileName(destPath)}");
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            onOutput($"[!] Erreur copie ({Path.GetFileName(item.Path)}) : {ex.Message}");
                        }
                    }

                    onOutput($"\n✅ {copiedCount} document(s) sensible(s) copié(s) avec succès !");
                    onOutput($"📂 Emplacement final : {targetDir}");

                    FolderDialogHelper.OpenInExplorer(targetDir);
                }
                else
                {
                    onOutput("\n[i] Exportation ignorée ou annulée par l'utilisateur.");
                }
            }

            onOutput("═══════════════════════════════════════════════════════════════");
            onOutput("✅ [TERMINÉ] Audit documents sensibles terminé.");
            onOutput("═══════════════════════════════════════════════════════════════");
        }, ct);
    }

    private static string CleanFolderName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var clean = new string(name.Where(c => !invalid.Contains(c)).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(clean) ? "Autres" : clean;
    }

    private static void ScanDirectorySafe(
        string currentDir, 
        int depth, 
        int maxDepth, 
        List<(string Category, string Path, string Details, string Icon)> findings,
        string[] sensitiveKeywords,
        HashSet<string> certExtensions,
        HashSet<string> docExtensions,
        CancellationToken ct)
    {
        if (depth > maxDepth || ct.IsCancellationRequested) return;

        // Ignorer les dossiers encombrants ou systèmes
        string dirName = Path.GetFileName(currentDir).ToLowerInvariant();
        if (dirName.StartsWith(".") && dirName != ".ssh" && dirName != ".aws" && dirName != ".kube") return;
        if (dirName == "node_modules" || dirName == "bin" || dirName == "obj" || dirName == "packages" || dirName == "$recycle.bin") return;

        try
        {
            var files = Directory.GetFiles(currentDir);
            foreach (var file in files)
            {
                if (ct.IsCancellationRequested) return;
                var fi = new FileInfo(file);
                string ext = fi.Extension;
                string nameWithoutExt = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
                string fullNameLower = file.ToLowerInvariant();

                // Fichiers d'environnement (.env, .env.local...)
                if (fi.Name.StartsWith(".env", StringComparison.OrdinalIgnoreCase))
                {
                    findings.Add(("Secrets applicatifs (.env)", fi.FullName, $"{Math.Round(fi.Length / 1024.0, 1)} Ko, Modifié le {fi.LastWriteTime:dd/MM/yyyy HH:mm}", "⚙️"));
                    continue;
                }

                // Certificats et clés
                if (certExtensions.Contains(ext))
                {
                    string icon = ext == ".kdbx" ? "🔒" : "🔑";
                    string cat = ext == ".kdbx" ? "Coffre-fort KeePass" : "Clé / Certificat cryptographique";
                    findings.Add((cat, fi.FullName, $"{Math.Round(fi.Length / 1024.0, 1)} Ko, Modifié le {fi.LastWriteTime:dd/MM/yyyy HH:mm}", icon));
                    continue;
                }

                // Documents contenant des mots-clés de secrets dans le nom
                if (docExtensions.Contains(ext))
                {
                    bool matchesKeyword = sensitiveKeywords.Any(k => nameWithoutExt.Contains(k));
                    if (matchesKeyword)
                    {
                        findings.Add(("Document contenant des mots-clés confidentiels", fi.FullName, $"{Math.Round(fi.Length / 1024.0, 1)} Ko, Modifié le {fi.LastWriteTime:dd/MM/yyyy HH:mm}", "📄"));
                    }
                }
            }

            var subDirs = Directory.GetDirectories(currentDir);
            foreach (var sub in subDirs)
            {
                ScanDirectorySafe(sub, depth + 1, maxDepth, findings, sensitiveKeywords, certExtensions, docExtensions, ct);
            }
        }
        catch { }
    }
}

