## 🏛️ Architettura API (Controller Structure)
Il progetto `eTeller.Api` utilizza una struttura di controller organizzata per aree funzionali. Ogni nuovo Controller deve essere collocato nella sottocartella corretta all'interno di `Controllers/`.

### 📂 Mapping delle Cartelle Controller
Segui rigorosamente la gerarchia esistente per mantenere la simmetria con il frontend:
- **Area Core:** `Account`, `Authentication`, `Branch`, `Customer`, `Language`.
- **Area Manager:** Tutti i controller di gestione devono essere sotto `Controllers/Manager/`.
  - **Esempio Casse:** `Controllers/Manager/CassePeriferiche/Casse/Client/ClientController.cs`
- **Area System:** `Function`, `Personalisation`, `Role`, `User`, `Tabelle`.

### 🛠️ Standard di Sviluppo (ASP.NET 10)
- **Routing:** Definisci le rotte in modo che riflettano la struttura delle cartelle (es. `[Route("api/manager/casse-periferiche/casse/client")]`).
- **Namespace:** Il namespace deve corrispondere alla cartella (es. `eTeller.Api.Controllers.Manager.CassePeriferiche.Casse.Client`).
- **Dependency Injection:** Usa i Primary Constructors di C# 14/15 per iniettare i servizi nei controller.
- **Action Results:** Usa `TypedResults` per garantire la compatibilità con l'AOT di ASP.NET 10 e facilitare la generazione automatica di OpenAPI/Swagger per l'agente Angular.

## 🏗️ Struttura Progetto eTeller.Application (CQRS Pattern)
Il progetto segue rigorosamente il pattern CQRS con MediatR. Ogni nuova funzionalità deve essere implementata all'interno della cartella `Features/`.

### 1. Organizzazione Feature
Ogni cartella in `Features/` rappresenta un dominio o un sotto-modulo (es. `Manager`, `Account`, `CassePeriferiche`).
- **Commands/**: Contiene le operazioni di scrittura (Insert, Update, Delete). Ogni comando deve avere la propria sottocartella (es. `DeleteClient/`) contenente:
    - `NomeCommand.cs`
    - `NomeCommandHandler.cs`
    - `NomeCommandValidator.cs` (se necessario).
- **Queries/**: Contiene le operazioni di lettura. Ogni query deve avere la propria sottocartella (es. `GetFuncAccType/`) contenente:
    - `NomeQuery.cs`
    - `NomeQueryHandler.cs`

### 2. Supporto e Infrastruttura Application
- **Dtos/**: Oggetti di trasferimento dati piatti per le risposte API.
- **Contracts/**: Interfacce per servizi che verranno implementati nel layer Infrastructure.
- **Mappings/**: Profili AutoMapper o configurazioni di mapping tra Entity e DTO.
- **Behaviours/**: Pipeline di MediatR (es. Validation, Logging, Transaction management).
- **Exceptions/**: Eccezioni custom specifiche del dominio applicativo.

### 📍 Regole di Naming e Posizionamento
- Se un task riguarda la gestione di una cassa, il percorso sarà: `Features/Manager/Commands/CassePeriferiche/Casse/[NomeAzione]`.
- Le query di lettura dati devono essere collocate in `Features/[NomeDominio]/Queries/[NomeQuery]`.
- I parametri di input devono essere definiti come `record` all'interno dei file di Command/Query per sfruttare l'immutabilità di C# 14/15.

## 💎 Struttura Progetto eTeller.Domain (Core Domain)
Il progetto Domain contiene le fondamenta del database e le regole di business invarianti. È il layer con zero dipendenze verso altri progetti della solution.

### 1. Models (Entità del Database)
Le classi in `Models/` rappresentano la struttura dei dati. Devono essere implementate seguendo queste regole:
- **Entità Standard**: Classi C# (o `record` per modelli immutabili) che mappano le tabelle principali (es. `Branch.cs`, `Client.cs`).
- **StoredProcedure/**: Modelli specifici per i risultati restituiti dalle procedure memorizzate.
- **View/**: Modelli ottimizzati per la sola lettura che mappano le viste del database.

### 2. Logica Globale e Infrastruttura di Dominio
- **Common/**: Contiene classi base riutilizzabili (es. `EntityBase`, `IAuditable`) e Value Objects.
- **Exceptions/**: Definizione di eccezioni specifiche di dominio che non riguardano la logica applicativa ma violazioni di regole di business (es. `InvalidaDataException`).

### 📍 Linee Guida per ASP.NET 10 & EF Core
- **Data Annotations**: Usa gli attributi (`[Key]`, `[Required]`, `[Table]`) per definire i vincoli direttamente nel modello dove appropriato.
- **Nullable Context**: Tutti i modelli devono gestire correttamente il reference type nullability (`string?` per campi opzionali nel DB).
- **Naming**: I nomi dei file devono corrispondere esattamente al nome della classe (PascalCase).

## 🏗️ Struttura Progetto eTeller.Infrastructure (Data & External Services)
Questo layer implementa la logica di accesso ai dati e i servizi esterni. Deve referenziare `eTeller.Application` e `eTeller.Domain`.

### 1. Context (Database Access)
- **eTellerDbContext.cs**: L'unico punto di accesso al database tramite EF Core. Deve contenere i `DbSet` per le entità del dominio e le configurazioni (Fluent API) per View e Stored Procedure.

### 2. Repositories (Data Persistence)
Il progetto utilizza il Repository Pattern per isolare la logica di accesso ai dati.
- **BaseSimpleRepository.cs**: Classe base generica che contiene i metodi CRUD standard.
- **Cartelle per Feature**: Ogni dominio (es. `Account`, `Vigilanza`, `Manager`) ha la sua sottocartella con le implementazioni specifiche che ereditano dalla base.
- **UnitOfWork.cs**: Coordina le transazioni tra diversi repository assicurando l'atomicità delle operazioni.

### 3. Services & Registration
- **Services/**: Contiene implementazioni di interfacce definite in Application che riguardano infrastrutture esterne (Email, File System, API esterne).
- **InfrastructureServiceRegistration.cs**: Metodo di estensione per `IServiceCollection` dove vengono registrati tutti i Repository, la Unit of Work e il DbContext tramite Dependency Injection.

### 📍 Regole Tecniche per ASP.NET 10
- **Performance**: Usa `AsNoTracking()` per le query di sola lettura chiamate dalle Query CQRS.
- **Primary Constructors**: Implementa i repository usando i costruttori primari per iniettare il `DbContext`.
- **Dapper**: Se necessario per performance estreme su Stored Procedure complesse, l'infrastruttura è il posto dove integrarlo.