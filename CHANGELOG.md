# 📜 Journal des Modifications (Changelog)

Toutes les modifications notables apportées au projet **Alex Toolkit** sont consignées dans ce document.

Le format est basé sur [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/) et le projet respecte le versionnage sémantique.

---

## [4.0.0] - 2026-10-14

### ✨ Nouveautés majeures
- **Refonte intégrale en C# / WPF (.NET 8.0)** : transition complète depuis les anciens scripts dispersés vers une architecture applicative moderne, typée et sécurisée.
- **Exécutable autonome unique (`AlexToolkit.exe`)** : compilation `PublishSingleFile` optimisée en ReadyToRun AOT pour un démarrage instantané sans dépendances.
- **5 thèmes visuels dynamiques** : bascule en temps réel entre *Aero Glass (Translucide)*, *Cyberpunk Neon*, *Matrix Phosphor*, *Nordic Frost* et *Obsidian Luxury*.
- **Console latérale DevTools intégrée** : terminal rétractable affichant les flux PowerShell et CMD en direct avec encodage strict UTF-8.
- **Centre de Diagnostic Unifié** : analyse complète du système, processeur, barrettes RAM, santé S.M.A.R.T. et audit d'éligibilité Windows 11 24H2 / NPU IA Copilot+.
- **Console de Démarrage & Récupération** : contrôle du démarrage sans échec, accès direct au BIOS/UEFI, WinRE et reconstruction du magasin BCD.
- **Bouclier de Confidentialité** : blocage réversible de la télémétrie et des services mouchards de Windows.
- **Optimiseur Gaming** : réduction de latence (HAGS, TCP Nagle) et déblocage du profil d'alimentation *Performances Ultimes*.
- **Benchmark DNS en temps réel** : mesure de latence en direct comparant les principaux résolveurs (Cloudflare, Google, OpenDNS).
- **Gestionnaire de Favoris** : mise en favoris en un clic avec stockage persistant local dans `%LocalAppData%`.
