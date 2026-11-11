# 📜 Journal des Modifications (Changelog)

Toutes les modifications notables apportées au projet **Alex Toolkit** sont consignées dans ce document.

Le format est basé sur [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/) et le projet respecte le versionnage sémantique.

---

## [Non publié] - Audit performances

### 🧰 Nouveaux outils pour techniciens (49 outils au total)
- **Intervention technicien et rapport client** : checkup enchaîné au choix (point de restauration, nettoyage, cache DNS, antivirus mis à jour + analyse, DISM/SFC, mises à jour Winget) avec durée et résultat de chaque étape, puis rapport HTML imprimable (machine, n° de série, licence, état avant / après, conclusion, journal, signatures).
- **Écrans bleus et périphériques en erreur** : codes d'arrêt des 90 derniers jours traduits avec la cause probable, arrêts brutaux, périphériques en erreur expliqués (codes 10, 28, 43…) et réinstallables en un clic.
- **Comptes utilisateurs** : mot de passe, droits administrateur, activation, création, suppression ; groupes désignés par SID (l'ancien code utilisait « Administrators », inexistant sur un Windows français).
- **Désinfection** : détection et correction avec sauvegarde (fichier hosts, proxy WinINET / WinHTTP lu en binaire, tâches planifiées et démarrages masqués / encodés / lancés depuis Temp), analyses Defender complète et hors ligne, AdwCleaner et Malwarebytes via Winget.
- **Sauvegarde et transfert de profil** : fichiers personnels (chemins réels, OneDrive compris), favoris Chrome / Edge (tous profils) / Firefox, réseaux Wi-Fi, liste des logiciels réinstallable via Winget ; restauration sans suppression ni écrasement de fichiers plus récents, contrôle de l'espace disponible.
- **Applications préinstallées** : suppression du bloatware connu (pré-coché), option tous comptes + blocage de la réinstallation ; composants essentiels, codecs et packs de langue jamais proposés.

### 🔧 Outils existants améliorés
- **Tester la puissance du PC** : refonte en interface graphique (au lieu du texte brut de WinSAT dans la console) : jauge de la note globale avec verdict, composant limitant et conseil, barres par composant avec repère de comparaison, mesures réelles (processeur multi / mono-cœur, bande passante mémoire, lecture disque séquentielle et aléatoire, fluidité du bureau), comparatif avec 4 profils types ou une évaluation précédente du même PC, historique avec évolution, progression étape par étape et alerte si le PC est sur batterie.
- **Console** : sélectionner du texte provoquait une erreur « Paragraph n'est pas un objet Visual » (recherche du focus dans l'arbre visuel).
- **Centre de diagnostic** : valeurs S.M.A.R.T. réelles par disque (usure, température, heures, erreurs non corrigées) ; le bouton « Audit complet » n'affichait que la carte mère, il enchaîne désormais 7 volets en lecture seule.
- **Intégrité système** : état de Windows Update (services, dernières mises à jour, échecs des 30 derniers jours, redémarrage en attente) et réinitialisation robuste des composants (fonctionnait une seule fois : `ren` échouait si un ancien `.old` existait).
- **Point de restauration** : `-Drive '$env:SystemDrive'` entre apostrophes empêchait l'activation de la protection système ; message clair quand Windows refuse un second point dans les 24 h.
- **Export Wi-Fi** : contournement d'un défaut de `netsh` qui tronque le chemin des fichiers (~128 caractères) et pouvait écraser un réseau par un autre.

### 🐛 Corrections
- **Scanner LAN** : réécrit en .NET natif (ping ICMP parallèle + DNS inverse borné). L'ancien script utilisait `ForEach-Object -Parallel` (PowerShell 7 uniquement) et ne trouvait aucun appareil sous Windows PowerShell 5.1.
- **Benchmark DNS** : le temps de résolution est désormais une vraie requête DNS UDP envoyée à chaque serveur (médiane de 3 domaines) au lieu d'une estimation `ping × 1,2` ; fournisseurs mesurés en parallèle.
- **Échappement des scripts PowerShell** : conforme aux règles de la ligne de commande Windows (un `\` avant un `"` ne casse plus le script).
- **Encodage du csproj** : accents corrompus corrigés (dont le Copyright de l'exe) ; `bump_version.ps1` lit désormais le fichier en UTF-8.
- **Menu contextuel « Ouvrir PowerShell (Admin) » sur le Bureau** : la commande inscrite dans le registre contenait une erreur de syntaxe PowerShell (apostrophe en trop) et échouait silencieusement. Réappliquer l'option depuis *Personnalisation → Menu contextuel* pour corriger une installation existante.
- **Mises à jour** : la fenêtre de mise à jour plantait en fin de vérification (modification d'un pinceau figé) ; seules les versions stables avec un exécutable joint sont proposées (fin de l'URL de secours `AlexToolkit.exe` qui n'existe plus) ; le fichier téléchargé est vérifié (taille exacte annoncée par GitHub + signature d'exécutable Windows) et supprimé s'il est incomplet ; si le remplacement échoue, l'ancienne version est relancée au lieu de laisser l'utilisateur sans application.
- **Arborescence (Paramètres)** : entièrement générée depuis `ToolRegistry` — un outil ajouté dans une nouvelle catégorie apparaît désormais aussi ; total cohérent (outils masqués exclus) ; commande manquante dans `CommandMap` ou identifiant en double signalés en tête au lieu d'afficher `powershell.exe` à tort. Code Markdown inutilisé supprimé.
- **Détection d'encodage CMD** : basée sur le nom de l'exécutable (`sfc`, `winget`…) et non plus sur une sous-chaîne.

### ⚡ Performances
- **Console latérale** : rendu par lots (1 mise à jour par frame au lieu d'une par ligne), limite de 3 000 lignes, et les barres de progression (SFC, DISM, winget) se mettent à jour sur une seule ligne.
- **Centre de diagnostic / Centre réseau** : collectes PowerShell lancées en parallèle (ouverture nettement plus rapide).
- **Favoris** : reconstruction uniquement quand la liste a changé ; suppression d'une page Favoris créée hors cache (fuite mémoire).
- **Préchauffage des pages** : une page par cycle d'inactivité au lieu des 14 d'un coup.
- **PowerShell** : barre de progression désactivée (accélère `Invoke-WebRequest` sous 5.1) ; annulation qui tue le processus dans toutes les méthodes.
- **Exécutable** : seules les ressources françaises des dépendances sont embarquées.

### 🎨 Refonte de l'interface
- **Nouveau système de design** : palette neutre (gris sans teinte bleutée) avec une seule couleur d'accent bleue, couleurs sémantiques (succès / avertissement / erreur) en versions texte et fond atténué, fin des dégradés et lueurs néon.
- **Thèmes Sombre et Clair** appliqués à **toutes** les fenêtres (les 30 dialogues restaient sombres en thème clair) et mémorisés entre deux lancements ; barres de titre des dialogues assorties au thème.
- **Icônes Windows** (Segoe Fluent Icons) dans la navigation, les cartes, les en-têtes et la console à la place des emojis ; une icône par catégorie.
- **Barre latérale** : monogramme, compteurs discrets, indicateur de sélection façon Windows 11. **Cartes d'outils** épurées, points clés cochés, boutons cohérents.
- **Boutons unifiés** : 72 styles locaux dupliqués remplacés par une famille unique (standard, principal, succès, danger, avertissement, icône) avec états survol / pressé / focus clavier.
- **Console** : barre d'outils à icônes, bouton « Effacer ». **Barres de défilement** fines et discrètes.
- Plus aucune couleur codée en dur : ~1 500 couleurs XAML et ~130 couleurs C# remplacées par des jetons du thème.

### ✨ Améliorations
- **Verrouillage global** : pendant l'exécution d'un script, les boutons « Exécuter » des autres outils sont grisés (avec info-bulle).
- **Regex** : délai maximal global de 2 s (protection de l'analyseur de sites web contre le HTML malveillant).

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
