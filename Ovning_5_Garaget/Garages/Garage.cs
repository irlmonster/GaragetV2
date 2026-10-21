using Ovning_5_Garaget.Vehicles;

namespace Ovning_5_Garaget.Garages
{
    public class Garage
    {
        private readonly IVehicle?[] _vehicles;

        public int Capacity => _vehicles.Length;
        public int Count { get; private set; }
        public bool IsFull => Count >= Capacity;

        public Garage(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Kapaciteten måste vara större än 0.");
            }

            _vehicles = new IVehicle?[capacity];
        }

        public bool Park(IVehicle vehicle)
        {
            if (IsFull) return false;

            for (int i = 0; i < _vehicles.Length; i++)
            {
                if (_vehicles[i] is not null &&
                    _vehicles[i]!.RegNumber.Equals(vehicle.RegNumber, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
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

        public IVehicle[] GetVehicles()
        {
            // Ny array med storleken av Count
            var result = new IVehicle[Count];

            //Egen räknade för var i nya arrayen nästa fordon ska hamna
            int index = 0;

            //Gå igenom alla platser i garaget
            for (int i = 0; i < _vehicles.Length; i++)
            {
                if (_vehicles[i] is not null)
                {
                    // lägg till fordonet (! säger att vi vet att det inte är null eftersom vi kolla det i if-satsen)
                    result[index] = _vehicles[i]!;
                    index++;
                }
            }
            return result;
        }

        public IVehicle? FindByRegNumber(string regNumber)
        {
            for (int i = 0; i < _vehicles.Length; i++)
            {
                if (_vehicles[i] is not null &&
                    _vehicles[i]!.RegNumber.Equals(regNumber, StringComparison.OrdinalIgnoreCase))
                {
                    return _vehicles[i];
                }
            }
            return null;
        }



    }
}
