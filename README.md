# WeatherForecast – Full-Stack Wetter App 

Eine kompakte Full-Stack-Anwendung zur Registrierung, Anmeldung, Wetterabfrage und Verwaltung persönlicher Favoriten — mit Fokus auf Clean Architecture.

1. [Über das Projekt](#über-das-projekt)
2. [Features](#features)
3. [Architektur](#architektur)
4. [Technologiestack](#technologiestack)
5. [Quickstart](#quickstart)
6. [Screenshots](#screenshots)
7. [Motivation & Lernziele](-motivation--lernziele)
8. [Autor](#autor)

## Über das Projekt
WeatherForecast ist eine vollständige Fullstack-Anwendung, die Nutzern ermöglicht:
- sich zu registrieren und anzumelden
- Wetterdaten aus der OpenWeatherMap-API abzurufen
- Lieblingsorte als Favoriten zu speichern
- ihre gespeicherten Orte wieder abzurufen oder zu entfernen

Der Fokus des Projekts liegt auf Clean Architecture, sicherer Authentifizierung, Docker-Deployment und 
einer modernen Benutzeroberfläche


## Features
### Authentifizierung

- Registrierung & Login via ASP.NET Core Identity
- JWT-basierte Authentifizierung
- Passwort-Hashing, User-Management
- Geschützte API-Routen

### Wetterdaten
- Suche nach Städten
- Anzeige von Temperatur, Wind, Feuchtigkeit, Icons
- Integration der OpenWeatherMap API

### Favoritensystem

- Städte als Favoriten speichern
- Favoriten auflisten
- Favoriten löschen
- Zugriff nur für eingeloggte Benutzer

### Frontend

- Moderne UI mit React & Vite
- Zustandshandling via State Hooks
- Fetch API / axios für API-Kommunikation
- JWT-Handling & LocalStorage
- Responsive Design mit TailwindCSS

   ---

## Architektur

<img src="docs/screenshots/Diagramm.png" width="500px">

```
WeatherForecast.Web            → Controller, Endpoints, Program/DI, DTOs, Mapping
WeatherForecast.Application    → Services, Interfaces
WeatherForecast.Infrastructure → EF Core, Identity, DB-Kontext. OpenWeatherMap API
WeatherForecast.Domain         → Domain Modelle

```
Diese Layer-Trennung sorgt für testbaren, erweiterbaren und wartbaren Code.
**Abhängigkeiten zeichen nach innen**
 
  ---

## Technologiestack

**Backend**
- C#/.NET 9
- ASP.NET Core Web API
- Entity Framework Core + MySQL
- ASP.NET Identity + JWT Auth
- Docker & Docker Compose

**Frontend**
- React
- Vite
- Tailwind CSS
- Axios

---

## Quickstart

**Projekt klonen**
```
git clone
cd WeatherForecast
```

**.env Datei im Projektroot anlegen**
```
OWM_API_KEY=Hier bitte eigenen OpenWeatherMap API-Key einfügen
```

**Backend starten**
```
docker compose up --build
```

**Frontend starten**
```
cd WeatherForecast-React
npm install
npm run dev
```


## Screenshots

<img src="docs/screenshots/Register.png" width="350px">

<img src="docs/screenshots/Login.png" width="350px">

<img src="docs/screenshots/Suchergebnis.png" width="350px">

<img src="docs/screenshots/Dashboard.png" width="350px">




## Motivation & Lernziele

Mir ist wichtig, die Technologien und Architekturprinzipien zu lernen,
die auch in professionellen Softwareprojekten eingesetzt werden.

Dieses Projekt entstand daher bewusst als kompakter, aber realitätsnaher Übungsanwendungsfall,
um zentrale Konzepte wie:

- mehrschichtige Architektur (Controller → Services → Infrastructure → Domain)
- saubere Trennung von Verantwortlichkeiten
- Identity & JWT-Authentifizierung
- persistente Datenhaltung mit EF Core & MySQL
- Docker-basierte Bereitstellung
- Frontend-Integration über klare API-Schnittstellen

praktisch zu beherrschen.

Durch das Arbeiten mit Layer Architecture, Docker, JWT und einem getrennten React-Frontend
möchte ich mir die Fähigkeiten aneignen, die mir den Einstieg in professionelle Entwicklungsteams erleichtern.

  
## Autor

**Name:** Michael Bannach
**Rolle:** angehender Softwareentwickler


























  
  
