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

        public List<IVehicle> GetAllVehicles()
        {
            return _garage.GetAllVehicles(); // returnerar en kopia av garagets List
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

        // Räknar hur många fordon det finns av varje typ i garaget.
        // Returnerar t.ex. [Car: 3, Motorcycle: 2, Bus: 1]
        public List<TypeCount> GetTypeCounts()
        {
            var vehicles = GetAllVehicles(); // en kopia av alla parkerade fordon
            var typeNames = new List<string>(); // här samlas varje typnamn, bara en gång per typ

            // först ta reda på vilka typer som finns
            foreach (var vehicle in vehicles)
            {
                var typeName = vehicle.GetType().Name; // klassens namn, t.ex. "Car" eller "Bus"

                // Lägg bara till typen om vi inte redan sett den, så blir det inga dubbletter
                if (!typeNames.Contains(typeName))
                    typeNames.Add(typeName);
            }

            // Steg 2: räkna antal fordon för varje typ vi hittade
            var result = new List<TypeCount>();
            foreach (var typeName in typeNames)
            {
                // Skapar ett TypeCount med typnamnet och antalet, och lägger det i resultatet
                result.Add(new TypeCount(typeName, CountByType(typeName)));
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