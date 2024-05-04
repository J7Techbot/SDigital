using System.Text.Json;

namespace api.Backend
{
    /// <summary>
    /// Replaces the service responsible for providing and processing signals.
    /// </summary>
    public class SignalsProvider
    {
        public Signal[] Signals { get; private set; }

        public SignalsProvider() 
        {
            //data seed
            Signals = new Signal[]
            {
                new Signal { Id = 1, Name = "Signal1", Coordinates = new Coordinates(61.451510,23.871328)},
                new Signal { Id = 2, Name = "Signal2", Coordinates = new Coordinates(61.451973, 23.873911)},
                new Signal { Id = 3, Name = "Signal3", Coordinates = new Coordinates(61.452087, 23.876412)},
                new Signal { Id = 4, Name = "Signal4", Coordinates = new Coordinates(61.451408, 23.876535)},
                new Signal { Id = 5, Name = "Signal5", Coordinates = new Coordinates(61.450734, 23.873496)},
                new Signal { Id = 6, Name = "Signal6", Coordinates = new Coordinates(61.450359, 23.871259)},
                new Signal { Id = 7, Name = "Signal7", Coordinates = new Coordinates(61.450831, 23.870561)},
                new Signal { Id = 8, Name = "Signal8", Coordinates = new Coordinates(61.451334, 23.870459)},
            };
        }

        /// <summary>
        /// Returns all signals on track
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSignalsAsync()
        {           
            int indexOfRed = FindFirstRed(LightRandom(Signals));

            return await Task.FromResult(
                JsonSerializer.Serialize(
                    new SignalsData() 
                    { 
                        Signals = this.Signals, 
                        FirstRed = indexOfRed 
                    })); 
        }

        /// <summary>
        /// Finds the first signal with red state (SignalState.Red) in a sorted array of signals.
        /// Binary search is used to find the first signal with a red state.        
        /// </summary>
        /// <param name="signals"></param>
        /// <returns>
        /// Returns the index of the first signal with red state (SignalState.Red), if it exists.
        /// If no signal with red state is found, returns -1.
        /// </returns>
        private int FindFirstRed(Signal[] signals)
        {
            int left = 0;
            int right = signals.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (signals[mid].State == SignalState.RED)
                {
                    if (mid == 0 || signals[mid - 1].State != SignalState.RED)                   
                        return mid;
                    
                    else
                        right = mid - 1;                    
                }
                else                
                    left = mid + 1;                
            }

            return -1;
        }
        /// <summary>
        /// Randomly selects a signal light and sets its state and the state of all preceding lights to green.
        /// Used only for generate examples.
        /// </summary>
        private Signal[] LightRandom(Signal[] signals)
        {
            foreach (var signal in signals) 
            {
                signal.State = SignalState.RED;
            }

            Random random = new Random();
            int randomIndex = random.Next(signals.Length); 

            for (int i = 0; i < randomIndex; i++)
            {
                signals[i].State = SignalState.GREEN;
            }

            return signals;
        }
    }

    //Wrappers for JSON string used for comunication between services

    public class SignalsData
    {
        public int FirstRed { get; set; }
        public Signal[] Signals { get; set; }

    }

    public class Signal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        public SignalState State { get; set; }

    }
    public struct Coordinates
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
    public enum SignalState
    {
        RED,ORANGE,GREEN
    }
}
