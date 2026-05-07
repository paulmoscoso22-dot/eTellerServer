---
description: "Use when creating or modifying C# code in this project. Covers coding style, naming conventions, and best practices for C# development in this codebase. using clean code"
applyTo: '**' # when provided, instructions will automatically be added to the request context when the pattern matches an attached file
---

<!-- Tip: Use /create-instructions in chat to generate content with agent assistance -->

# Clean code best practices per questo progetto

## Convenzioni di codice
- **Nomi:** PascalCase per classi, metodi, proprietà; camelCase per variabili locali e parametri; prefisso `I` per interfacce.
- **Spaziatura:** 4 spazi per indentazione, 1 riga vuota tra metodi, 2 righe vuote tra classi.
- **Commenti:** solo dove aggiungono valore reale – non commentare l'ovvio. Usa XML comments per API pubbliche.
- **Usings:** posiziona `using` all'inizio del file, ordina alfabeticamente, rimuovi quelli non usati.
- **Stringhe:** usa string interpolation (`$"Hello {name}"`) invece di concatenazione.
- **Nullability:** abilita nullable reference types e gestisci i null in modo esplicito.


## Best practices
- **DRY:** evita duplicazione di codice – estrai metodi o classi ri
utilizzabili.
- **SRP:** ogni classe/metodo dovrebbe avere una sola responsabilità.
- **YAGNI:** non implementare funzionalità non necessarie al momento.

- **SOLID:** segui i principi SOLID per un design flessibile e manutenibile.
- **Error handling:** gestisci le eccezioni in modo appropriato, preferibilmente con middleware globale per API.
- **Unit testing:** scrivi test unitari per la logica di business, non per il codice triviale.
- **Performance:** evita operazioni costose in loop, usa async/await per I/O


## Quando produci codice
1. Mostra **struttura delle cartelle** se aggiungi nuovi file.
2. Mostra **interfaccia + implementazione** per ogni nuovo componente.
3. Includi la registrazione DI in `InfrastructureServiceRegistration.cs` o `Program.cs`.
4. Se individui un **anti-pattern** nel codice esistente, segnalalo prima di procedere.
5. Suggerisci miglioramenti architetturali quando rilevante.












