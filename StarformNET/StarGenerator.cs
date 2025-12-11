namespace DLS.StarformNET
{
    using Data;

    public static class StarGenerator
    {
        public static double MinSunAge = 1.0E9;
        public static double MaxSunAge = 6.0E9;

        public static Star GetDefaultStar()
        {
            var mass = Utilities.RandomNumber(0.7, 1.4);
            // SOL as default
            var sun = new Star(1, mass, 5772);

            if (sun.Luminosity == 0)
            {
                sun.Luminosity = HelperFunctions.Luminosity(sun.Mass);
            }

            sun.EcosphereRadiusAU = HelperFunctions.StarEcosphereRadiusAU(sun.Luminosity);
            sun.Life = 1.0E10 * (sun.Mass / sun.Luminosity);

            sun.AgeYears = Utilities.RandomNumber(
                MinSunAge,
                sun.Life < MaxSunAge ? sun.Life : MaxSunAge);

            return sun;
        }
    }
}
