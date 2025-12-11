using System.Text.Json.Serialization;

namespace DLS.StarformNET.Data
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Planet : IEquatable<Planet>
    {
        public int Position { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public Star Star { get; set; }
        public Atmosphere Atmosphere { get; set; } = new Atmosphere();
        public SolidInventory SolidInventory { get; set; } = new();

        public PlanetOrbitData PlanetOrbitData { get; }

        public PlanetSizeAndMassData PlanetSizeAndMassData { get; }

        public PlanetProperties PlanetProperties { get; }

        public PlanetAtmosphericData PlanetAtmosphericData { get; }

        public PlanetTemperatureData PlanetTemperatureData { get; }

        public PlanetCoverageData PlanetCoverageData { get; }

        public PlanetMoonData PlanetMoonData { get; }

        public Planet()
        {
            PlanetMoonData = new PlanetMoonData();
            PlanetCoverageData = new PlanetCoverageData();
            PlanetTemperatureData = new PlanetTemperatureData();
            PlanetAtmosphericData = new PlanetAtmosphericData();
            PlanetProperties = new PlanetProperties();
            PlanetOrbitData = new PlanetOrbitData();
            PlanetSizeAndMassData = new PlanetSizeAndMassData();
        }

        public Planet(PlanetSeed seed, Star star, int num)
        {
            PlanetMoonData = new PlanetMoonData();
            PlanetCoverageData = new PlanetCoverageData();
            PlanetTemperatureData = new PlanetTemperatureData();
            PlanetAtmosphericData = new PlanetAtmosphericData();
            PlanetProperties = new PlanetProperties();
            Star = star;
            Position = num;
            PlanetOrbitData = new PlanetOrbitData(seed.SemiMajorAxisAU, seed.Eccentricity);
            PlanetSizeAndMassData = new PlanetSizeAndMassData(seed.Mass, seed.DustMass, seed.GasMass);
        }

        public bool Equals(Planet other)
        {
            return Position == other.Position &&
                Utilities.AlmostEqual(PlanetOrbitData.SemiMajorAxisAU, other.PlanetOrbitData.SemiMajorAxisAU) &&
                Utilities.AlmostEqual(PlanetOrbitData.Eccentricity, other.PlanetOrbitData.Eccentricity) &&
                Utilities.AlmostEqual(PlanetOrbitData.AxialTiltDegrees, other.PlanetOrbitData.AxialTiltDegrees) && PlanetOrbitData.OrbitZone == other.PlanetOrbitData.OrbitZone &&
                Utilities.AlmostEqual(PlanetOrbitData.OrbitalPeriodDays, other.PlanetOrbitData.OrbitalPeriodDays) &&
                Utilities.AlmostEqual(PlanetOrbitData.DayLengthHours, other.PlanetOrbitData.DayLengthHours) &&
                Utilities.AlmostEqual(PlanetOrbitData.HillSphereKM, other.PlanetOrbitData.HillSphereKM) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.MassSM, other.PlanetSizeAndMassData.MassSM) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.DustMassSM, other.PlanetSizeAndMassData.DustMassSM) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.GasMassSM, other.PlanetSizeAndMassData.GasMassSM) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.EscapeVelocityCMSec, other.PlanetSizeAndMassData.EscapeVelocityCMSec) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.SurfaceAccelerationCMSec2, other.PlanetSizeAndMassData.SurfaceAccelerationCMSec2) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.SurfaceGravityG, other.PlanetSizeAndMassData.SurfaceGravityG) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.CoreRadiusKM, other.PlanetSizeAndMassData.CoreRadiusKM) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.RadiusKM, other.PlanetSizeAndMassData.RadiusKM) &&
                Utilities.AlmostEqual(PlanetSizeAndMassData.DensityGCC, other.PlanetSizeAndMassData.DensityGCC) && PlanetMoonData.Moons.Count == other.PlanetMoonData.Moons.Count &&
                Utilities.AlmostEqual(PlanetAtmosphericData.RMSVelocityCMSec, other.PlanetAtmosphericData.RMSVelocityCMSec) &&
                Utilities.AlmostEqual(PlanetAtmosphericData.MolecularWeightRetained, other.PlanetAtmosphericData.MolecularWeightRetained) &&
                Utilities.AlmostEqual(PlanetAtmosphericData.VolatileGasInventory, other.PlanetAtmosphericData.VolatileGasInventory) &&
                Utilities.AlmostEqual(PlanetAtmosphericData.BoilingPointWaterKelvin, other.PlanetAtmosphericData.BoilingPointWaterKelvin) &&
                Utilities.AlmostEqual(PlanetAtmosphericData.Albedo, other.PlanetAtmosphericData.Albedo) &&
                Utilities.AlmostEqual(PlanetTemperatureData.Illumination, other.PlanetTemperatureData.Illumination) &&
                Utilities.AlmostEqual(PlanetTemperatureData.ExosphereTempKelvin, other.PlanetTemperatureData.ExosphereTempKelvin) &&
                Utilities.AlmostEqual(PlanetTemperatureData.SurfaceTempKelvin, other.PlanetTemperatureData.SurfaceTempKelvin) &&
                Utilities.AlmostEqual(PlanetTemperatureData.GreenhouseRiseKelvin, other.PlanetTemperatureData.GreenhouseRiseKelvin) &&
                Utilities.AlmostEqual(PlanetTemperatureData.DaytimeTempKelvin, other.PlanetTemperatureData.DaytimeTempKelvin) &&
                Utilities.AlmostEqual(PlanetTemperatureData.NighttimeTempKelvin, other.PlanetTemperatureData.NighttimeTempKelvin) &&
                Utilities.AlmostEqual(PlanetTemperatureData.MaxTempKelvin, other.PlanetTemperatureData.MaxTempKelvin) &&
                Utilities.AlmostEqual(PlanetTemperatureData.MinTempKelvin, other.PlanetTemperatureData.MinTempKelvin) &&
                Utilities.AlmostEqual(PlanetCoverageData.WaterCoverFraction, other.PlanetCoverageData.WaterCoverFraction) &&
                Utilities.AlmostEqual(PlanetCoverageData.CloudCoverFraction, other.PlanetCoverageData.CloudCoverFraction) &&
                Utilities.AlmostEqual(PlanetCoverageData.IceCoverFraction, other.PlanetCoverageData.IceCoverFraction);
        }

        public override string ToString()
        {
            return $"{PlanetProperties.PlanetType}, Moons: {PlanetMoonData.Moons.Count}, Temp: {PlanetTemperatureData.DaytimeTempKelvin}/{PlanetTemperatureData.NighttimeTempKelvin}";
        }

    }
}
