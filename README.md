# Pollyglot Language School Desktop App

Desktop application for managing a language school: students, teachers, courses, groups, class sessions, attendance, grades, payments, and teacher payroll reports.

## Architecture Overview

The project uses a **classic WPF MVVM architecture** with Entity Framework Database-First access:

- **Presentation layer**: WPF Views (`Views/*`) + shared XAML templates (`Themes/Generic.xaml`).
- **Application layer**: ViewModels (`ViewModels/*`) coordinating UI state, commands, filtering/sorting, and user actions.
- **Domain/data access layer**: EF5 Database-First model (`Models/ModelPollyglot.edmx` and generated entities).
- **Business logic layer**: focused report/services classes (`Models/BusinessLogic/*`) for attendance, payments balance, and payroll calculations.
- **Data transfer/projection models**: `Models/ForAllView/*` used as read models for table/report screens.

This is not Clean Architecture/CQRS. It is a pragmatic MVVM + EF Database-First desktop architecture with business logic extracted into dedicated classes where needed.

## Tech Stack

- **Language**: C#
- **Framework**: .NET Framework 4.8
- **UI**: WPF (XAML)
- **Pattern**: MVVM
- **MVVM library**: MVVM Light (`GalaSoft.MvvmLight`, Messenger)
- **ORM**: Entity Framework 5 (Database-First, EDMX)
- **Database**: Microsoft SQL Server (connection configured for SQL Server Express)
- **Other**: CSV export (`SaveFileDialog`, UTF-8 BOM), basic validation with `IDataErrorInfo`

## Database Structure (Concise)

Main tables/entities:

- `Uczen` (student)
- `Lektor` (teacher)
- `Jezyk` (language)
- `RodzajKursu` (course type)
- `Kurs` (course definition)
- `Grupa` (group classes)
- `GrupaUczen` (student-group membership, with join/end dates)
- `UczenKurs` (individual student-course assignment)
- `Zajecia` (class session: group or individual)
- `Obecnosc` (attendance per class/student)
- `Ocena` (grade per class/student)
- `Platnosc` (payments)
- `Wynagrodzenie` (teacher payroll)
- `Sala` (classroom)
- `Podrecznik` (textbook)

Key relationships:

- `Kurs -> Jezyk`, `Kurs -> RodzajKursu`
- `Grupa -> Kurs`, `Grupa -> Lektor`, `Grupa -> Sala`, `Grupa -> Podrecznik`
- `GrupaUczen` is a many-to-many bridge between `Uczen` and `Grupa`
- `UczenKurs` links `Uczen` with `Kurs/Jezyk/RodzajKursu/Lektor/Podrecznik`
- `Zajecia` can reference either a `Grupa` or a single `Uczen`
- `Obecnosc` and `Ocena` link `Uczen` with `Zajecia`
- `Platnosc` links `Uczen` and `Kurs`
- `Wynagrodzenie` links `Lektor`

## Key Features

- Master-data views for students, teachers, languages, rooms, course types, courses, groups, textbooks.
- Operational views for sessions (`Zajecia`), attendance, grades, payments, and payroll records.
- Add workflows for student, teacher, room, language, course type, group membership (`GrupaUczen`), teacher language competency (`LektorJezyk`), individual student-course assignment (`UczenKurs`), and session scheduling (`Zajecia`).
- Context-aware selection flow between screens using MVVM Light Messenger.
- Sorting/filtering support on selected modules (not all table screens).
- Session helpers: filter today’s classes, filter by selected teacher, show selected group members.
- Report modules: monthly attendance report (counts + percentage), monthly payment balance report (expected vs paid vs balance), monthly payroll report + annual teacher earnings calculation.
- CSV export for all three report modules.
- Payroll generation trigger using SQL stored procedure `dbo.usp_GenerujWynagrodzeniaZaOkres`.

## Business Rules and Advanced Concepts in Code

- `IDataErrorInfo` validation in add forms (`NewZajeciaViewModel`, `NewUczenKursViewModel`, `NewGrupaUczenViewModel`, `NewLektorJezykViewModel`).
- Dedicated validators for date/text/session consistency (`Models/Validatory/*`).
- Read-model mapping (`Models/ForAllView/*`) to avoid binding raw entities directly in complex grids/reports.
- Report/business services (`Models/BusinessLogic/*`) separated from View code.
- Direct stored procedure execution via `DbConnection.CreateCommand()` for payroll generation.
- Decoupled inter-ViewModel communication using `Messenger.Default.Send/Register`.

## API Overview

This project is a **desktop application** and does **not** expose an HTTP/REST API.

The “application API” is command-based within MVVM:

- UI commands in ViewModels (`Load`, `Add`, `Sort`, `Find`, report calculations, exports)
- message-based selection events between modules (MVVM Light Messenger)
- EF DbContext operations (`DbSet`, LINQ queries, `SaveChanges`)
- stored procedure call for payroll generation

## How to Run Locally

### 1. Prerequisites

- Windows
- Visual Studio 2022 (with `.NET desktop development` workload)
- SQL Server Express (or compatible SQL Server instance)

### 2. Restore database

A backup file is included in repository root:

- `PollyglotDB.bak`

Restore it as database **`PollyglotDB`** in SQL Server.

### 3. Configure connection string

Current connection string is in:

- `PollyglotDesktopApp/PollyglotDesktopApp/App.config`

Default server name in repo:

- `LAPTOP-D1G0P0AU\SQLEXPRESS`

If your instance name is different, update `data source` accordingly.

### 4. Build and run

- Open solution: `PollyglotDesktopApp/PollyglotDesktopApp/PollyglotDesktopApp.sln`
- Restore NuGet packages (if not restored automatically)
- Set startup project to `PollyglotDesktopApp`
- Build and run (`F5`)

## Folder Structure

```text
PollyglotDesktopApp/
  PollyglotDesktopApp/
    Models/
      BusinessLogic/      # report and business calculation services
      ForAllView/         # read-model/DTO-like projection classes for grids/reports
      Validatory/         # validation helpers
      ModelPollyglot.edmx # EF Database-First model
      *.cs                # generated entities + partial context
    ViewModels/
      Abstract/           # generic base ViewModels for list/add screens
      Add/                # add forms and validation logic
      AllTables/          # list/detail screens
      Raporty/            # reporting modules
    Views/
      Add/                # add form views
      AllTables/          # list views
      Raporty/            # report views
      MainWindow.xaml     # shell/navigation
    Themes/               # shared styles and control templates
    Helper/               # commands + CSV helpers
```