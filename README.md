
# Dofus Group Finder

**DofusGroupFinder** est une application desktop permettant aux joueurs de Dofus de trouver et organiser des groupes de donjons de manière simple et rapide.

Projet développé en C# avec une architecture propre full .NET, utilisant WPF pour le client et ASP.NET Core pour le backend.

---

## 🧭 Objectif du projet

L'objectif est de simplifier la recherche de groupes pour les joueurs multi-comptes de Dofus en centralisant :

- La gestion de ses personnages
- La création d'annonces de groupes (listings)
- La recherche d'annonces actives par donjon et serveur
- La gestion simplifiée des membres de groupe
- Un client simple, fluide, ergonomique, full desktop.

---

## 🏗️ Architecture technique

### Client Desktop

- **Technologie :** WPF (.NET 8)
- **MVVM :** CommunityToolkit MVVM
- **Théming dynamique** via un système custom de `ThemeManager` en JSON
- **UI customisée** : styles et contrôles skinnés pour une intégration fluide avec l’univers Dofus

### Backend API

- **Technologie :** ASP.NET Core WebAPI (.NET 8)
- **Architecture :** Clean Architecture (Domain, Infrastructure, Application, Api)
- **Base de données :** PostgreSQL
- **ORM :** Entity Framework Core

### Synchronisation des données Dofus

- Un projet externe `DungeonBaker` est utilisé pour récupérer dynamiquement la liste des donjons officiels via l'API Ankama, injectée directement dans la base de données.

---

## 📦 Fonctionnalités principales

### Comptes et Personnages

- Création de compte via email/password avec sécurisation par JWT
- Ajout de ses personnages multi-comptes par serveur

### Listings (annonces de groupe)

- Création de listings par donjon et par serveurs
- Ajout de slots restants, succès recherchés et commentaires
- Possibilité d'ajouter des membres extérieurs au logiciel

### Recherche de groupes

- Filtrage rapide des annonces publiques
- Tri par donjon, serveur, slots restants

### Gestion des serveurs

- Sélection dynamique du serveur actif sur le client (affecte les listings créés et recherchés)

### UI moderne

- Navigation fluide par onglets : Create Listing, My Listings, Search Group
- Custom control : `FilteredComboBox` avec recherche intégrée
- État de connexion visuel : statut online, en groupe, hors-ligne

---

## 🚀 Roadmap à court terme

- ✅ Version MVP opérationnelle
- 🔄 Amélioration continue des contrôles custom WPF
- 🔄 Gestion des amis et des favoris
- 🔄 Intégration de notifications de nouveaux groupes disponibles
- 🔄 Possibilité de prévisualiser la composition d’un groupe avant de rejoindre

---

## 🔧 Stack technique complète

| Composant  | Technologie |
| ----------- | ------------ |
| Client UI   | WPF (.NET 8) |
| MVVM        | CommunityToolkit MVVM |
| Backend API | ASP.NET Core 8 |
| Database    | PostgreSQL |
| ORM         | Entity Framework Core |
| Authentification | JWT |
| Theme manager | Custom JSON theming |
| API donjons | API officielle Dofus Ankama |
| Contrôles UI avancés | Custom UserControls (FilteredComboBox, etc) |

---

## 📂 Organisation du projet

```
/DofusGroupFinder.Client        --> Application WPF
/DofusGroupFinder.Api           --> ASP.NET Core WebAPI
/DofusGroupFinder.Application   --> Business layer backend
/DofusGroupFinder.Domain        --> Entities & Domain Models
/DofusGroupFinder.Infrastructure --> EF Core, DB Context & Repositories
/DungeonBaker                   --> Projet externe pour l'import des donjons officiels
```

---

## 🔒 Sécurité

- Authentification JWT sécurisée
- Hashage des mots de passe avec BCrypt
- Communication sécurisée entre le client et l'API

---

## 👤 Auteur

**Kevin Bouetard - VRDTM Studio**

---

## ⚠️ Disclaimer

Projet non affilié à Ankama.  
Ce projet est un outil personnel pour faciliter la recherche de groupes entre joueurs et ne permet pas de se connecter au jeu.
