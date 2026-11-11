# CLAUDE.md — Alex Toolkit (v4.x C# / WPF)
> Instructions machine prioritaires. Mode : Performance & Qualité C# / WPF.

## 🎯 Architecture du Projet
1. **Application principale :** C# / WPF (.NET 8.0-windows x64).
2. **Exécutable :** `AlexToolkit.exe` à la racine (généré via `build.bat` en SingleFile autonome).
3. **Structure des sources :**
   - `ScriptsWindows/ScriptsWindows/Core/` : Moteurs d'exécution (PowerShell async, CMD, élévation UAC, thèmes, registre d'outils, favoris).
   - `ScriptsWindows/ScriptsWindows/Services/` : Services métiers (Diagnostic, Réparation, Nettoyage, Réseau, Dns, Matériel, Sécurité, etc.).
   - `ScriptsWindows/ScriptsWindows/Views/` : Fenêtres de contrôles/dialogues XAML et pages de catégories.

## 🏗️ Standards Techniques C# / WPF
- **Async/Await & UI Thread :** Tout traitement long ou externe s'exécute de façon asynchrone sans bloquer l'UI (`Dispatcher` pour les retours visuels).
- **Design & couleurs :** aucune couleur hexadécimale dans les vues. Toujours `{DynamicResource Jeton}` en XAML et `ThemeManager.GetBrush("Jeton")` en C# (jetons définis dans `Assets/Styles/Theme.xaml` et `Core/ThemeManager.cs` : `BgDeep`, `BgCard`, `BgInput`, `Border`, `TextPrimary/Secondary/Muted`, `Accent`, `AccentText`, `OnAccent`, `Success/Warning/Danger` + variantes `Text` et `Bg`). Boutons : styles globaux `ActionButton`, `PrimaryActionButton`, `DangerActionButton`… (ne pas les redéfinir localement). Icônes : police `FontIcon` (Segoe Fluent Icons) via `Core/IconGlyphs.cs`, pas d'emoji dans l'interface.
- **Encodage & Sorties :** Forcer UTF-8 pour tous les flux PowerShell / CMD afin d'éviter la corruption des accents.
- **Single-File Release :** Les bibliothèques natives (SQLite, etc.) sont incluses pour auto-extraction (`IncludeNativeLibrariesForSelfExtract`). Aucun fichier DLL résiduel ne doit être commité à la racine ou dans git.
- **Scripts de maintenance :**
  - `build.bat` : compile en Release x64 SingleFile et dépose `AlexToolkit.exe` à la racine.
  - `clean.bat` : supprime les dossiers temporaires de compilation `bin/` et `obj/` et les logs résiduels.
- **Mise à jour obligatoire de l'arborescence :** À CHAQUE ajout ou modification d'outils/scripts :
  1. Enregistrer l'outil dans `Core/ToolRegistry.cs` avec sa catégorie, son `SubHeading` (sous-catégorie) et sa commande associée dans `CommandMap`.
  2. S'assurer que l'arborescence dans `ParametresPage.xaml.cs` (prévisualisation & copie) le référence automatiquement.
  3. Mettre à jour la documentation du projet (`README.md`, liste des catégories et architecture des fichiers).