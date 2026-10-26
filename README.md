# Garage 2.0

En konsolapplikation i C# som simulerar ett parkeringsgarage. Övning 6 i .NET-utbildningen hos Lexicon.

Nu är det `version 2` och jag har bytt ut Arraylogiken mot Lists, väldigt smidigt när det var uppbyggt för refaktorisering innan, också smidigt generellt med List. 
Generics ser vi mer av `version 3`.

## Funktioner

- Skapa ett garage med valfri storlek vid start, eller ett nytt via menyn
- Parkera fordon via en undermeny för varje fordonstyp
- Hämta ut fordon via registreringsnummer
- Lista alla parkerade fordon
- Lista vilka fordonstyper som står i garaget och hur många av varje
- Hitta ett fordon via registreringsnummer (ABC123, abc123 och AbC123 fungerar)
- Sök med fritext i alla egenskaper, även fordonsspecifika (t.ex. "500" hittar en motorcykel med 500 cc)
- Sök på en eller flera egenskaper i kombination, t.ex. alla svarta fordon med fyra hjul
- Fyll garaget med testfordon
- Färgad feedback: grönt när något gick bra, rött vid fel, gult vid varningar
- Robust inmatning: programmet frågar igen vid felaktig input istället för att krascha

## Fordonstyper

| Typ | Egen egenskap |
|---|---|
| `Car` | Bränsle, biltyp (Sedan, Kombi, Halvkombi, Suv, Cab) |
| `Motorcycle` | Cylindervolym, motorcykeltyp (Sport, Naked, Touring, Cruiser, Enduro) |
| `Bus` | Antal säten |
| `Boat` | Längd |
| `Airplane` | Antal motorer |

Alla fordon har registreringsnummer, färg, märke och antal hjul.

## Teknik och upplägg

- **.NET 10 / C#**, **xUnit** för enhetstester
- Garaget lagrar fordonen i en privat List<IVehicle>. Ingen LINQ, och inga egna generiska klasser (kommer i `version 3`)
- Abstrakt basklass `Vehicle` med gemensamma egenskaper, en subklass per fordonstyp
- Fritextsökningen använder polymorfism: varje fordon har en egen `Matches`-metod som anropar basklassens och lägger till sina egna fält
- Registreringsnummer valideras med regex (tre bokstäver, två siffror, en siffra eller bokstav) och normaliseras till versaler
- `GarageHandler` är mellanlagret mellan menyn och garaget, så menyn aldrig pratar direkt med garaget
- All färgad utskrift och all inmatning är samlad i `ConsoleOutput` och `ConsoleInput`

## Struktur

```
GaragetV2/
├── Ovning_5_Garaget/
│   ├── Garages/        Garage, själva samlingen av fordon
│   ├── Handlers/       GarageHandler och TypeCount
│   ├── UI/             MainMenu, ConsoleInput, ConsoleOutput
│   ├── Validation/     RegNumberValidation
│   └── Vehicles/       IVehicle, Vehicle, subklasserna och enums
└── Ovning_5_Garaget.Tests/
                        Tester för garage, handler, fordon och validering
```

## Klasser och ansvar

| Klass | Ansvar |
|-------|--------|
| `Program` | Startpunkt, skapar garaget och startar huvudmenyn |
| `MainMenu` | Visar menyer och styr flödet utifrån användarens val |
| `ConsoleInput` | All inläsning från användaren, frågar om vid felaktig input |
| `ConsoleOutput` | All utskrift, inklusive färgad feedback |
| `GarageHandler` | Mellanlager mellan UI och garage, menyn pratar aldrig direkt med `Garage` |
| `TypeCount` | Håller räkning per fordonstyp vid listning |
| `Garage` | Lagrar fordonen i en privat List, hanterar parkering, uttag och sökning |
| `RegNumberValidation` | Validerar och normaliserar registreringsnummer med regex |
| `IVehicle` / `Vehicle` | Interface och abstrakt basklass med gemensamma egenskaper och `Matches` |
| `Car`, `Motorcycle`, `Bus`, `Boat`, `Airplane` | Subklasser med egna egenskaper och utökad `Matches` |



## Kända begränsningar
- Alla fordon tar en plats, har inte byggt något för storlek ännu
- Ingen LINQ eller egna generiska klasser ännu (kommer i version 3)
- Data sparas inte mellan körningarna, spara och ladda från fil kommer framöver

## Jag vill ha feedback på
**Arkitektur**

Är det lätt att förstå hur koden hänger ihop?

Jag har byggt det med skalering i åtanke, för att det ska vara lätt att bygga ut / vidare.
Även hela menyn är byggd för att kunna användas till andra applikationer med mindre refaktorisering.
Skulle gärna ta både positiv och negativ feedback på hur jag har byggt det och om ni ser något bättre sätt att lösa det på.

**Sökning**
- Hur skulle ni göra kombinerad sökning på flera egenskaper mer flexibel och lättläst?
- Är polymorfismen i `Matches` en bra lösning för fritextsökning, eller finns det smidigare sätt?

**Tester**
- Är det något mer som är värt att testa?

## Kom igång

```bash
git clone https://github.com/irlmonster/GaragetV2.git
cd GaragetV2
dotnet run --project Ovning_5_Garaget
```

Köra testerna:

```bash
dotnet test
```






