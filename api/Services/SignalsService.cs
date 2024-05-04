using api.Backend;

namespace api.Services
{
    /// <summary>
    /// Signals endpoint "controller"
    /// </summary>
    public class SignalsService
    {
        private readonly SignalsProvider _signalsProvider;

        public SignalsService(SignalsProvider signalsProvider)
        {
            _signalsProvider = signalsProvider;
        }

        /// <summary>
        /// Simulates HTTP request to another service responsible for signals managing
        /// </summary>
        /// <returns>Returns information about individual signal lights as a JSON object.</returns>
        //Here, a call to another container/service should be made to keep the API separated.
        //For the sake of saving time, I will replace it with a class.
        public async Task<string> GetSignalsAsync() => await _signalsProvider.GetSignalsAsync();
    }
}

