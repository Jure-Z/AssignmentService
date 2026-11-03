# AssignmentService

**AssignmentService** je zaledna mikrostoritev, razvita v ogrodju **.NET 10**, namenjena upravljanju nalog, študentskih oddaj in njihovemu ocenjevanju. Zgrajena je po načelih **Clean Architecture** in vključuje globalno obravnavo napak, preverjanje zdravja ter kontejnerizacijo z Dockerjem.

---

## Ključne funkcionalnosti

- **Upravljanje nalog (Assignments):**
  - Ustvarjanje, pregled, posodabljanje in brisanje nalog.
  - Nastavitev roka oddaje in maksimalnega števila točk.
- **Oddaja rešitev (Submissions):**
  - Študent začne z osnutkom (`Draft`), ki ga lahko poljubno ureja.
  - Oddaja naloge (`Submit`) zaklene urejanje in avtomatsko določi status:
    - `Submitted` (oddano pred iztekom roka).
    - `Late` (oddano po roku).
- **Ocenjevanje (Evaluations):**
  - Vpis ali posodobitev ocene (`Upsert`) s povratno informacijo.
- **Operativna pripravljenost (Production readiness):**
  - **OpenAPI / Swagger UI** za interaktivno testiranje vseh končnih točk.
  - **Health check (`/health`)** za preverjanje stanja storitve in povezave do baze.
  - **Global Exception Handling** z vračanjem standardiziranih napak.
  - **FluentValidation** za samodejno preverjanje veljavnosti vhodnih podatkov (DTO).

---

## Tehnološki sklad

- **Ogrodje:** .NET 10 (C# / ASP.NET Core Web API)
- **Podatkovna baza:** PostgreSQL 16
- **ORM:** Entity Framework Core (Code-First pristop z avtomatskimi migracijami)
- **Validacija:** FluentValidation
- **Dokumentacija:** OpenAPI 3 / Swagger UI
- **Kontejnerizacija:** Docker & Docker Compose
- **Testiranje:** xUnit, FluentAssertions, NSubstitute, Testcontainers for .NET

---

## Zahteve pred zagonom

Za lokalni zagon in testiranje potrebujete:
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (ali Docker Engine)

---

## Zagon z Docker Compose (Priporočeno)

Najhitrejši način za zagon celotnega okolja (storitve in PostgreSQL baze podatkov) je z orodjem Docker Compose:

1. V korenski mapi projekta zaženite:
   ```bash
   docker compose up --build
   ```

2. Po zagonu sta na voljo:
   - **Swagger UI (dokumentacija in testiranje):** [http://localhost:7001](http://localhost:7001)
   - **Health Check endpoint:** [http://localhost:7001/health](http://localhost:7001/health)

3. Za zaustavitev okolja uporabite:
    ```bash
    docker compose down
    ```

---

## Lokalni zagon

Če želite API poganjati lokalno:

1. **Zaženite le PostgreSQL bazo**:
   ```bash
   docker run --name assignment_postgres_db -e POSTGRES_DB=assignments_db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=secretpassword -p 5432:5432 -d postgres:16-alpine
   ```

2. V datoteki `src/AssignmentService.Api/appsettings.json` preverite, da povezovalni niz ustreza:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=assignments_db;Username=postgres;Password=secretpassword"
   }
   ```

3. Zaženite aplikacijo:
   ```bash
   dotnet run --project src/AssignmentService.Api
   ```

---

## Poganjanje testov

Projekt vsebuje tako enotne kot integracijske teste.

### Zagon vseh testov:
```bash
dotnet test
```

### Zagon samo enotnih testov:
```bash
dotnet test tests/AssignmentService.UnitTests
```

### Zagon integracijskih testov:
> *Za integracijske teste mora biti zagnan Docker demon, saj knjižnica Testcontainers avtomatsko ustvari izoliran PostgreSQL vsebnik.*
```bash
dotnet test tests/AssignmentService.IntegrationTests
```

---

## Pregled ključnih API končnih točk

### Naloge (`/api/assignments`)
| Metoda | Pot | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/assignments` | Seznam vseh nalog |
| `GET` | `/api/assignments/{id}` | Podrobnosti posamezne naloge |
| `POST` | `/api/assignments` | Ustvari novo nalogo |
| `PUT` | `/api/assignments/{id}` | Posodobi obstoječo nalogo |
| `DELETE` | `/api/assignments/{id}` | Izbriši nalogo |

### Oddaje (`/api/submissions`)
| Metoda | Pot | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/submissions/{id}` | Podrobnosti oddaje (vključno s statusom in oceno) |
| `GET` | `/api/submissions/assignment/{assignmentId}` | Seznam vseh oddaj za določeno nalogo |
| `POST` | `/api/submissions` | Ustvari nov osnutek oddaje (`Draft`) |
| `PUT` | `/api/submissions/{id}` | Uredi vsebino osnutka oddaje |
| `POST` | `/api/submissions/{id}/submit` | Oddaj nalogo v pregled (`Submitted` ali `Late`) |
| `DELETE` | `/api/submissions/{id}` | Izbriši oddajo |

### Ocenjevanje (`/api/submissions/{submissionId}/evaluation`)
| Metoda | Pot | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/submissions/{submissionId}/evaluation` | Pridobi oceno oddaje |
| `POST` | `/api/submissions/{submissionId}/evaluation` | Vpiši ali posodobi oceno oddaje (Upsert) |
| `DELETE` | `/api/submissions/{submissionId}/evaluation` | Izbriši oceno |

### Nadzor delovanja
| Metoda | Pot | Opis |
| :--- | :--- | :--- |
| `GET` | `/health` | Preverjanje zdravja storitve in povezljivosti do baze podatkov |