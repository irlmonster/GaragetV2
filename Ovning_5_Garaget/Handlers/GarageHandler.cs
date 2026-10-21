using Ovning_5_Garaget.Garages;
using Ovning_5_Garaget.Vehicles;
using Ovning_5_Garaget.Validation;

namespace Ovning_5_Garaget.Handlers
{
    public class GarageHandler
    {
        private readonly Garage _garage;

        public GarageHandler(int capacity)
        {
            _garage = new Garage(capacity);
        }

        public bool Park(IVehicle vehicle)
        {
            return _garage.Park(vehicle); // returnerar true eller false från garagets Park-metod
        }

        public bool Remove(string regNumber)
        {
            return _garage.Remove(RegNumberValidation.Normalize(regNumber)); // städar regnumret och skickar vidare till garaget
        }

        public IVehicle[] GetAllVehicles()
        {
            return _garage.GetVehicles(); // returnerar en kopia av garagets array
        }

        public IVehicle? FindByRegNumber(string regNumber)
        {
            return _garage.FindByRegNumber(RegNumberValidation.Normalize(regNumber)); // städar regnumret och skickar vidare till garaget
        }

        public int Count => _garage.Count; // antal parkerade fordon, direkt från garaget

        public int Capacity => _garage.Capacity; // totala antalet platser, direkt från garaget



        // Räknar hur många fordon det finns av en viss typ
        public int CountByType(string typeName)
        {
            int count = 0;

            foreach (var vehicle in GetAllVehicles())
            {
                if (vehicle.GetType().Name == typeName)
                {
                    count++;
                }
            }

            return count;
        }

        // En koll för att se vilka olika typer av vehicle som finns i garaget.
        public TypeCount[] GetTypeCounts()
        {
            var vehicles = GetAllVehicles();
            var typeNames = new string[vehicles.Length];
            int uniqueCount = 0;


            // Samla unika typnamn: skriv upp varje typ första gången vi ser den
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
                result[i] = new TypeCount(typeNames[i], CountByType(typeNames[i])); // “på plats i, lägg ett paket med typnamnet och hur många av den typen som finns” baserad på recorden TypeCount
            }

            return result;
        }



        // Fritextsökning: returnerar alla fordon där något fält matchar söktermen.
        // Varje fordon avgör själv vad som matchar via sin egen Matches-metod (polymorfism).
        public IVehicle[] Search(string searchTerm)
        {
            var term = searchTerm.Trim();
            var vehicles = GetAllVehicles();

            // Loop 1: räkna träffar för att veta hur stor arrayen ska vara
            int count = 0;
            foreach (var vehicle in vehicles)
            {
                if (vehicle.Matches(term))
                {
                    count++;
                }
            }

            // Loop 2: fyll arrayen med träffarna
            var result = new IVehicle[count];
            int index = 0;
            foreach (var vehicle in vehicles)
            {
                if (vehicle.Matches(term))
                {
                    result[index] = vehicle;
                    index++;
                }
            }

            return result;
        }

        // Sök på en eller flera egenskaper. Null betyder att egenskapen inte används i sökningen.
        public IVehicle[] SearchByProperties(string? color = null, int? wheels = null, string? brand = null, string? type = null)
        {
            var vehicles = GetAllVehicles();

            // Loop 1: räkna träffar för att veta hur stor arrayen ska vara
            int count = 0;
            foreach (var vehicle in vehicles)
            {
                if (MatchesProperties(vehicle, color, wheels, brand, type))
                {
                    count++;
                }
            }

            // Loop 2: fyll arrayen med träffarna
            var result = new IVehicle[count];
            int index = 0;
            foreach (var vehicle in vehicles)
            {
                if (MatchesProperties(vehicle, color, wheels, brand, type))
                {
                    result[index] = vehicle;
                    index++;
                }
            }

            return result;
        }

        // Villkoret för SearchByProperties: varje kriterium är antingen null (bryr oss inte) eller måste stämma
        private static bool MatchesProperties(IVehicle vehicle, string? color, int? wheels, string? brand, string? type)
        {
            return (color is null || vehicle.Color.Equals(color, StringComparison.OrdinalIgnoreCase))
                && (wheels is null || vehicle.Wheels == wheels)
                && (brand is null || vehicle.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase))
                && (type is null || vehicle.GetType().Name.Equals(type, StringComparison.OrdinalIgnoreCase));
        }
    }
}