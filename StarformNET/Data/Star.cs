using System.ComponentModel;

namespace DLS.StarformNET.Data
{
    using System;

    // UGLY Not comfortable with binary systems just having a second mass value

    [Serializable]
    public class Star
    {
        private readonly double _radius;
        private double _mass;
        private readonly double _temperature;

        /// <summary>
        /// Stefan-Boltzman-Constant
        /// </summary>
        const double sigma = 5.67e-8;
        public string Name { get; set; }

        /// <summary>
        /// Age of the star in years.
        /// </summary>
        public double AgeYears { get; set; }

        /// <summary>
        /// The maximum lifetime of the star in years.
        /// </summary>
        public double Life { get; set; }

        /// <summary>
        /// Estimation of Radius (R<sub>☉</sub>)
        /// R = L * (2 * t) / M
        /// </summary>
        public double Radius => _radius == 0 ? Luminosity * (2 * (AgeYears / 4500000000)) / Mass : _radius;

        /// <summary>
        /// Temperature estimation of a star (T<sub>☉</sub>). The temperature of the Sun is 5780°K
        /// </summary>
        public double Temperature => _temperature == 0 ? Math.Pow(((Luminosity * (3.846 * 10e26)) / (4 * Math.PI * Math.Pow(Radius * (6.96 * 10e8), 2) * sigma)), 0.25) : _temperature;

        /// <summary>
        /// The distance that the star's "ecosphere" (as far as I can tell,
        /// ye olden science speak for circumstellar habitable zone) is
        /// centered on. Given in AU. 
        /// </summary>
        public double EcosphereRadiusAU { get; set; }

        /// <summary>
        /// Luminosity of the star in solar luminosity units (L<sub>☉</sub>).
        /// The luminosity of the sun is 1.0.
        /// </summary>
        public double Luminosity { get; set; }

        /// <summary>
        /// Mass of the star in solar mass units (M<sub>☉</sub>). The mass of
        /// the sun is 1.0.
        /// </summary>
        public double Mass
        {
            get => _mass;
            set => _mass = value;
        }

        /// <summary>
        /// The mass of this star's companion star (if any) in solar mass
        /// units (M<sub>☉</sub>). 
        /// </summary>
        public double BinaryMass { get; set; }

        /// <summary>
        /// The semi-major axis of the companion star in au.
        /// </summary>
        public double SemiMajorAxisAU { get; set; }

        /// <summary>
        /// The eccentricity of the companion star's orbit.
        /// </summary>
        public double Eccentricity { get; set; }

        public StarCategory Category =>
            Temperature switch
            {
                > 5700 and <= 7500 => StarCategory.MainSequence,
                > 7500 and <= 10000 => StarCategory.Giant,
                > 10000 => StarCategory.SuperGiant,
                _ => StarCategory.Uncategorized
            };

        public string Classification =>
            Temperature switch
                {
                    >= 33000 => "O",
                    >= 10000 => "B",
                    >= 7500 => "A",
                    >= 6000 => "F",
                    >= 5200 => "G",
                    >= 3700 => "K",
                    _ => "M"
                };

        public StarColor Color =>
            Temperature switch
            {
                >= 30000 => StarColor.Blue,
                > 10000 and <= 30000 => StarColor.BlueWhite,
                > 7500 and <= 10000 => StarColor.White,
                > 6000 and <= 7500 => StarColor.YellowWhite,
                > 5200 and <= 6000 => StarColor.Yellow,
                > 3700 and <= 5200 => StarColor.LightOrange,
                > 2400 and <= 3700 => StarColor.OrangeRed,
                <= 2400 => StarColor.Brown
            };

        public override string ToString()
        {
            return $"This is a {Color.GetDescription()} {Category.GetDescription()} classified as {Classification}";
        }


        /// <summary>
        /// Initialize the sun with predefined parameters
        /// </summary>
        /// <param name="radius">R☉</param>
        /// <param name="mass">M☉</param>
        /// <param name="temperature">Temperature in degrees Kelvin</param>
        public Star(double radius, double mass, double temperature)
        {
            _radius = radius;
            _mass = mass;
            _temperature = temperature;
            Luminosity = HelperFunctions.Luminosity(Mass);
            EcosphereRadiusAU = HelperFunctions.StarEcosphereRadiusAU(Luminosity);

            Life = 1.0E10 * (Mass / Luminosity);

            AgeYears = Utilities.RandomNumber(
                StarGenerator.MinSunAge,
                Life < StarGenerator.MaxSunAge ? Life : StarGenerator.MaxSunAge);

            //HINT: mass can be calculated using this formula: M = (L_star)^(1/a) * M☉
            //It is important to note that this is only an approximation and 'a' stands for the constant of the star category. 'a' would be 3.5 for main sequence stars.
        }

        //Default constructor
        public Star()
        {
            StarGenerator.GetDefaultStar();
        }

    }

    public enum StarCategory
    {
        [Description("Dwarf")]
        Dwarf,
        [Description("Main Sequence Star")]
        MainSequence,
        [Description("Giant")]
        Giant,
        [Description("Super Giant")]
        SuperGiant,
        [Description("Uncategorized")]
        Uncategorized
    }

    public enum StarColor
    {
        [Description("Orange-Red")]
        OrangeRed,
        [Description("Light Orange")]
        LightOrange,
        [Description("Yellow")]
        Yellow,
        [Description("Yellow-White")]
        YellowWhite,
        [Description("White")]
        White,
        [Description("Blue-White")]
        BlueWhite,
        [Description("Blue")]
        Blue,
        [Description("Brown")]
        Brown
    }
}
