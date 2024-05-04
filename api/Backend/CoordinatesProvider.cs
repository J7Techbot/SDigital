using System.Globalization;
using System.Text.Json;
using System.Text;

namespace api.Backend
{
    /// <summary>
    /// Replaces the service responsible for providing and processing coordinates.
    /// </summary>
    public class CoordinatesProvider
    {
        /// <summary>
        /// Returns coordinates and angle with a time delay simulating a stream.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<(double Latitude, double Longitude)> GetCoordinatesAsync(CancellationToken cancellationToken)
        {
            var culture = CultureInfo.InvariantCulture;
            var coordinatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "gps_input.txt");
            string[] coordinates = null;

            try
            {
                coordinates = await File.ReadAllLinesAsync(coordinatesPath);
            }
            catch
            {
                Console.WriteLine($"Failed to load coordinates from file.");
            }

            if (coordinates.Length != null)
            {
                int index = 0;
                int length = coordinates.Length;

                while (true) //this is not ok, its only for "stream mimic"
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (index >= length)
                    {
                        index = 0;
                    }

                    var currentLine = coordinates[index];
                    var parts = currentLine.Split(' ');

                    if (parts.Length == 2)
                    {
                        bool latParsed = float.TryParse(parts[0], NumberStyles.Float, culture, out float latitude);
                        bool lonParsed = float.TryParse(parts[1], NumberStyles.Float, culture, out float longitude);

                        if (latParsed && lonParsed)
                        {
                            yield return (latitude, longitude);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to parse coordinates at line {index + 1}");
                        }
                    }

                    index++;

                    await Task.Delay(500); //same as above
                }
            }
        }

        /// <summary>
        /// Returns coordinates buffer with a time delay simulating a stream.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<byte[]> GetCoordinatesBufferAsync(CancellationToken cancellationToken)
        {
            await foreach (var coords in new CoordinatesProvider().GetCoordinatesAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                byte[] coordBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                    coordinates = new
                    {
                        latitude = coords.Latitude,
                        longitude = coords.Longitude,
                        angle = coords.Angle,
                    }
                }));

                yield return coordBuffer;
            }
        }        
    }
}


