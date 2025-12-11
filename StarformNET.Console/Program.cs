namespace DLS.StarformNET.Console
{
    using System.Runtime.Serialization;
    using System.Text.Json;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

    class Program
    {
        private static string SYSTEM_NAME = "test";
        private static string SYSTEM_FILE = "testsystem.bin";

        static void Main(string[] args)
        {
            Utilities.InitRandomSeed(0);
            var system = Generator.GenerateStellarSystem(SYSTEM_NAME);
            // TODO: The following uses System.Text.Json for serialization. This is a proposal and may not work for deserialization if the format is not compatible.
            var json = JsonSerializer.Serialize(system);
            File.WriteAllText(SYSTEM_FILE, json);
            //Stream stream = new FileStream(SYSTEM_FILE, FileMode.Create, FileAccess.Write, FileShare.None);
            //formatter.Serialize(stream, system);
            //stream.Close();
        }
    }
}
