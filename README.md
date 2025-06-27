// README.md

# ✅ To-Do-Liste mit ASP.NET Core

Dies ist eine Webanwendung mit Benutzer-Authentifizierung und Rollenverwaltung, die mit C#, ASP.NET Core und Razor Pages entwickelt wurde. 
Das Projekt dient als Lern- und Demonstrationsprojekt im Rahmen meiner Bewerbung als C#/.NET Junior Entwickler.
Es ist mein erstes umgesetztes Projekt in ASP.NET Core

## 🚀 Funktionen

-  Benutzerregistrierung und -anmeldung (ASP.NET Core Identity)
-  CRUD-Funktionalität für To-Do-Einträge
-  To-Dos nur für eingeloggte User sichtbar (mit `UserId`-Bindung)	 
-  Rollenbasiertes Berechtigungssystem (User / Admin)
-  Admin-Bereich mit erweiterten Rechten
-  Admin-spezifische Suchfunktion bestehender User
-  Suchen, Filtern, Sortieren, Erledigt markiert
-  Pagination für lange To-Do-Listen
-  Nutzung von DTOs und LINQ-Abfragen
-  Trennung von Business-Logik und UI mit Services und Dependency Injection
-  Sidebar-Navigation als View Component zur sauberen UI-Struktur
-  ASP.NET Identity mit Custom Claims und Rollenverwaltung
-  Datenbank mit SQLite, inklusive Migrationen
-  Beim ersten Start wird automatisch ein Admin-Benutzer in der Datenbank angelegt, falls noch keiner existiert.
	 
## Qualitätssicherung	
-  Unit Tests für Service-Methoden (xUnit) 
	 Derzeit 8 Methoden für Kernfunktionen 
-  Logging mit ILogger<T> 
	 für Fehler- und Informationsmeldungen 

---

## 🛠️ Verwendete Technologien

- C#
- ASP.NET Core 8.0, Razor Pages
- Entity Framework Core
- ASP.NET Identity
- SQLite
- xUnit (Testing)

---

## 📂 Projektstruktur

| Ordner / Datei                        | Beschreibung                                                |
|---------------------------------------|-------------------------------------------------------------|
| `/Pages/`                             | Razor Pages für To-Do-Funktionalität und Navigation         |
| `/Services/Todos/Interfaces/`         | Business-Logik, Dependency Injection                        |
| `/Models/DTOs`                        | Datenmodelle, Datenübertragungsobjekte                      |
| `/Data/`                              | AppDbContext												  |
| `/Areas/Identity/`                    | ASP.NET Identity (gescaffoldet, teils angepasst)            |
| `/Tests/`                             | Unit Tests mit xUnit                                        |
| `_ViewImports.cshtml`, `_ViewStart.cshtml` | Razor Page Konfiguration und Layouts                   |
| `/ViewComponents/`                    | Responsive Sidebar-Navigation                               |
| `Program.cs`, `appsettings.json`      | Anwendungskonfiguration                                     |

---

## 🔍 Hinweise 

Die Identity-Seiten wurden mithilfe des Scaffolding-Tools von Visual Studio generiert und anschließend von mir angepasst. 

---

## 💼 Eigene Implementierungen (Beispiele)

-  To-Do-CRUD inklusive Such-, Filter- und Sortierfunktionen
-  Rollen- und Rechteverwaltung über Custom Claims
-  Architektur mit Services, DTOs und Dependency Injection
-  Admin-Funktionalitäten wie Benutzerübersicht und Bearbeitung
-  Pagination für große Listen
-  Datenbank-Migration und Seed-Daten mit initialem Admin-Benutzer
-  Unit Tests für Service-Logik

---

## ▶️ Installation und Ausführung

## Voraussetzungen:

- .NET SDK 8.0 oder neuer
- IDE wie Visual Studio, Visual Studio Code oder JetBrains Rider

## Projekt klonen
Klonen Sie das Repository mit dem GitHub-Link in ein lokales Verzeichnis, z.B. todo-app.

```bash
git clone https://github.com/DimBoost/todo-app.git
cd todo-app
dotnet ef database update
dotnet run
```

## SQLite Datenbank
Die Anwendung nutzt SQLite als Datenbank. 
Führen Sie nach dem Klonen des Repositories den folgenden Befehl aus, 
um die Datenbank zu erstellen und die Migrationen anzuwenden.

dotnet ef database update

Die Verbindung zur Datenbank wird über appsettings.json konfiguriert (siehe ConnectionStrings:DefaultConnection).

## Optional:Tests ausführen

dotnet test

## Hinweis

Beim ersten Start wird automatisch ein Admin-Benutzer in der Datenbank angelegt, falls noch keiner existiert.
Die Standard-Login-Daten sind:

Benutzername: admin@todoapp.com

Passwort: Admin1234!

