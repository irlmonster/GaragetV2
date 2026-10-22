using Ovning_5_Garaget.Vehicles;

namespace Ovning_5_Garaget.Garages
{
    public class Garage
    {
        private readonly List<IVehicle> _vehicles;

        public int Capacity { get; 
        }
        public int Count => _vehicles.Count;
        public bool IsFull => Count >= Capacity;

        public Garage(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Kapaciteten måste vara större än 0.");
            }

            Capacity = capacity; // sätter det här objektet till den listan
            _vehicles = new List<IVehicle>();
        }

        public bool Park(IVehicle vehicle)
        {
            if (IsFull) return false;

            foreach (var v in _vehicles)
            {
                // jämför regnumren som strängar, oavsett stor/liten bokstav
                if (v.RegNumber.Equals(vehicle.RegNumber, StringComparison.OrdinalIgnoreCase))
                    return false; // regnumret finns redan
            }

            _vehicles.Add(vehicle); // lägger fordonet sist i listan
            return true;
        }


        public bool Remove(string regNumber)
        {
            for (int i = 0; i < _vehicles.Count; i++)
            {
                if (_vehicles[i].RegNumber.Equals(regNumber, StringComparison.OrdinalIgnoreCase))
                {
                    _vehicles.RemoveAt(i); // tar bort på index i, resten flyttas ett steg
                    return true;
                }
            }
            return false;
        }

        public List<IVehicle> GetAllVehicles()
        {
            return new List<IVehicle>(_vehicles); // en kopia, så ingen utanför kan ändra garagets lista
        }

        public IVehicle? FindByRegNumber(string regNumber)
        {
            foreach (var v in _vehicles)
            {
                if (v.RegNumber.Equals(regNumber, StringComparison.OrdinalIgnoreCase))
                    return v; // hittade fordonet
            }
            return null; // inget fordon med det regnumret
        }



    }
}