# Guide: Byt ut arrayer mot List<T>

## Vad är skillnaden?

En array har **fast storlek** – du måste bestämma hur stor den är när du skapar den.  
En `List<T>` har **dynamisk storlek** – den växer och krymper automatiskt när du lägger till och tar bort element.

```csharp
// Array – fast storlek
IVehicle?[] _vehicles = new IVehicle?[10];

// List – dynamisk storlek
List<IVehicle> _vehicles = new List<IVehicle>();
```

---

## Filer att ändra i ordning

### 1. `Garage.cs` — det viktigaste filen

Det är här all array-logik sitter. Gör allt här innan du rör något annat.

#### 1a. Byt ut fältet `_vehicles`

**Gammal kod:**
```csharp
private readonly IVehicle?[] _vehicles;
```

**Ny kod:**
```csharp
private readonly List<IVehicle> _vehicles;
```

Observera: med en `List<IVehicle>` behöver du **inte** `?` (nullable) längre. Listan innehåller bara riktiga fordon, aldrig `null`-platser.

---

#### 1b. Byt ut `Capacity` och `Count`

**Gammal kod:**
```csharp
public int Capacity => _vehicles.Length;
public int Count { get; private set; }
```

**Ny kod:**
```csharp
public int Capacity { get; }
public int Count => _vehicles.Count;
```

`Capacity` sätts en gång i konstruktorn (se nästa steg).  
`Count` räknar nu listan direkt istället för att hålla en egen räknare.

---

#### 1c. Byt ut konstruktorn

**Gammal kod:**
```csharp
public Garage(int capacity)
{
    if (capacity <= 0)
        throw new ArgumentOutOfRangeException(...);

    _vehicles = new IVehicle?[capacity];
}
```

**Ny kod:**
```csharp
public Garage(int capacity)
{
    if (capacity <= 0)
        throw new ArgumentOutOfRangeException(...);

    Capacity = capacity;
    _vehicles = new List<IVehicle>();
}
```

---

#### 1d. Byt ut metoden `Park`

Med en lista slipper du loopa för att hitta en ledig plats – du bara lägger till!  
Men du måste fortfarande kolla dubbletter.

**Gammal kod:**
```csharp
public bool Park(IVehicle vehicle)
{
    if (IsFull) return false;

    for (int i = 0; i < _vehicles.Length; i++)
    {
        if (_vehicles[i] is not null &&
            _vehicles[i]!.RegNumber.Equals(vehicle.RegNumber, StringComparison.OrdinalIgnoreCase))
            return false;
    }

    for (int i = 0; i < _vehicles.Length; i++)
    {
        if (_vehicles[i] is null)
        {
            _vehicles[i] = vehicle;
            Count++;
            return true;
        }
    }
    return false;
}
```

**Ny kod:**
```csharp
public bool Park(IVehicle vehicle)
{
    if (IsFull) return false;

    foreach (var v in _vehicles)
    {
        if (v.RegNumber.Equals(vehicle.RegNumber, StringComparison.OrdinalIgnoreCase))
            return false;
    }

    _vehicles.Add(vehicle);
    return true;
}
```

Notera: Du behöver **inte** `Count++` längre – `Count` räknar listan direkt (se 1b).

---

#### 1e. Byt ut metoden `Remove`

**Gammal kod:**
```csharp
public bool Remove(string regNumber)
{
    for (int i = 0; i < _vehicles.Length; i++)
    {
        if (_vehicles[i] is not null &&
            _vehicles[i]!.RegNumber.Equals(regNumber, StringComparison.OrdinalIgnoreCase))
        {
            _vehicles[i] = null;
            Count--;
            return true;
        }
    }
    return false;
}
```

**Ny kod:**
```csharp
public bool Remove(string regNumber)
{
    for (int i = 0; i < _vehicles.Count; i++)
    {
        if (_vehicles[i].RegNumber.Equals(regNumber, StringComparison.OrdinalIgnoreCase))
        {
            _vehicles.RemoveAt(i);
            return true;
        }
    }
    return false;
}
```

`RemoveAt(i)` tar bort elementet på plats `i` och täpper ihop hålet automatiskt.

---

#### 1f. Förenkla `GetVehicles`

Den gamla metoden behövde bygga en ny array av rätt storlek.  
Med en lista returnerar du bara en kopia av listan direkt.

**Gammal kod:**
```csharp
public IVehicle[] GetVehicles()
{
    var result = new IVehicle[Count];
    int index = 0;
    for (int i = 0; i < _vehicles.Length; i++)
    {
        if (_vehicles[i] is not null)
        {
            result[index] = _vehicles[i]!;
            index++;
        }
    }
    return result;
}
```

**Ny kod:**
```csharp
public IVehicle[] GetVehicles()
{
    return _vehicles.ToArray();
}
```

`ToArray()` är en inbyggd metod på `List<T>` som gör precis vad din gamla loop gjorde.

---

#### 1g. Förenkla `FindByRegNumber`

**Gammal kod:**
```csharp
public IVehicle? FindByRegNumber(string regNumber)
{
    for (int i = 0; i < _vehicles.Length; i++)
    {
        if (_vehicles[i] is not null &&
            _vehicles[i]!.RegNumber.Equals(regNumber, StringComparison.OrdinalIgnoreCase))
            return _vehicles[i];
    }
    return null;
}
```

**Ny kod:**
```csharp
public IVehicle? FindByRegNumber(string regNumber)
{
    foreach (var vehicle in _vehicles)
    {
        if (vehicle.RegNumber.Equals(regNumber, StringComparison.OrdinalIgnoreCase))
            return vehicle;
    }
    return null;
}
```

Ingen `is not null`-koll behövs – listan har inga null-platser.

---

### 2. `GarageHandler.cs` — två metoder att förenkla

Nu när `Garage` är klar kan du förenkla de metoder i `GarageHandler` som byggde arrayer manuellt.

#### 2a. `GetTypeCounts`

Det är den mest komplicerade metoden. Den bygger arrayer manuellt för att samla unika typnamn.  
Med en `List<string>` slipper du räknaren `uniqueCount` och allt det där.

**Gammal kod (förenklat):**
```csharp
var typeNames = new string[vehicles.Length];
int uniqueCount = 0;

foreach (var vehicle in vehicles)
{
    var typeName = vehicle.GetType().Name;
    bool alreadyFound = false;

    for (int i = 0; i < uniqueCount; i++)
    {
        if (typeNames[i] == typeName)
        {
            alreadyFound = true;
            break;
        }
    }

    if (!alreadyFound)
    {
        typeNames[uniqueCount] = typeName;
        uniqueCount++;
    }
}

var result = new TypeCount[uniqueCount];
for (int i = 0; i < uniqueCount; i++)
{
    result[i] = new TypeCount(typeNames[i], CountByType(typeNames[i]));
}
return result;
```

**Ny kod:**
```csharp
public TypeCount[] GetTypeCounts()
{
    var vehicles = GetAllVehicles();
    var typeNames = new List<string>();

    foreach (var vehicle in vehicles)
    {
        var typeName = vehicle.GetType().Name;
        if (!typeNames.Contains(typeName))
            typeNames.Add(typeName);
    }

    var result = new List<TypeCount>();
    foreach (var typeName in typeNames)
    {
        result.Add(new TypeCount(typeName, CountByType(typeName)));
    }
    return result.ToArray();
}
```

`typeNames.Contains(typeName)` ersätter din inre loop som letade efter dubbletter.

---

#### 2b. `Search` och `SearchByProperties`

Dessa metoder hade samma problem: de loopade två gånger – en gång för att räkna träffar, en gång för att fylla arrayen. Med en lista behövs bara en loop.

**Gammal kod (Search):**
```csharp
int count = 0;
foreach (var vehicle in vehicles)
    if (vehicle.Matches(term)) count++;

var result = new IVehicle[count];
int index = 0;
foreach (var vehicle in vehicles)
    if (vehicle.Matches(term)) { result[index] = vehicle; index++; }

return result;
```

**Ny kod (Search):**
```csharp
public IVehicle[] Search(string searchTerm)
{
    var term = searchTerm.Trim();
    var vehicles = GetAllVehicles();
    var result = new List<IVehicle>();

    foreach (var vehicle in vehicles)
    {
        if (vehicle.Matches(term))
            result.Add(vehicle);
    }

    return result.ToArray();
}
```

Gör samma sak för `SearchByProperties`.

---

## Snabbreferens: Array vs List

| Array | List |
|-------|------|
| `new IVehicle[10]` | `new List<IVehicle>()` |
| `array.Length` | `list.Count` |
| `array[i] = vehicle` | `list.Add(vehicle)` |
| `array[i] = null` | `list.RemoveAt(i)` |
| Manuell null-hantering | Inga null-platser |
| Måste veta storleken i förväg | Växer automatiskt |

---

## Ordning att koda om

1. `Garage.cs` — fältet, konstruktorn, alla metoder
2. Kör testerna (`dotnet test`) — de ska fortfarande gå igenom
3. `GarageHandler.cs` — `GetTypeCounts`, `Search`, `SearchByProperties`
4. Kör testerna igen

Testerna i `GarageTests.cs` behöver **inte** ändras eftersom de bara testar `Garage`s publika API (Park, Remove, Count) – och det förändras inte, bara hur det är implementerat inuti.
