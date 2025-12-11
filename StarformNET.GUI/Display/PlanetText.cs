
namespace DLS.StarformNET.Display
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using System.Collections.Generic;

    public static class PlanetText
    {
        public static string GetSystemText(List<Planet> planets)
        {
            var sb = new StringBuilder();
            var sun = planets[0].Star;
            sb.AppendLine(StarText.GetFullStarTextRelative(sun, true));
            sb.AppendLine();

            foreach (var p in planets)
            {
                sb.AppendLine(GetPlanetText(p));
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }
        public static string GetPlanetText(Planet planet)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{GetPlanetNumber(planet)} {GetPlanetTypeText(planet)}");
            sb.AppendLine();
            sb.AppendLine("-------------------------");
            sb.AppendLine();
            sb.Append($"Orbital Distance: {GetOrbitalDistanceAU(planet)}\n");
            sb.AppendLine();
            sb.Append($"Equatorial Radius: {GetRadiusER(planet)}\n");
            sb.AppendLine();
            sb.Append($"Surface Gravity: {GetSurfaceGravityG(planet)}\n");
            sb.AppendLine();
            sb.Append($"Escape Velocity: {GetEscapeVelocity(planet)}\n");
            sb.AppendLine();
            sb.Append($"Mass: {GetMassStringEM(planet)}\n");
            sb.AppendLine();
            sb.Append($"Density: {GetDensity(planet)}\n");
            sb.AppendLine();
            sb.Append($"Length of Year: {GetOrbitalPeriodDay(planet)}\n");
            sb.AppendLine();
            sb.Append($"Length of Day: {GetLengthofDayHours(planet)}\n");
            sb.AppendLine();
            sb.Append($"Average Day Temperature: {GetDayTemp(planet)}\n");
            sb.AppendLine();
            sb.Append($"Average Night Temperature: {GetNightTemp(planet)}\n");
            sb.AppendLine();
            sb.Append($"Boiling Point: {GetBoilingPoint(planet)}\n");
            sb.AppendLine();
            sb.Append($"Greenhouse Rise: {GetGreenhouseRise(planet)}\n");
            sb.AppendLine();
            sb.Append($"Water Cover: {GetHydrosphere(planet)}\n");
            sb.AppendLine();
            sb.Append($"Ice Cover: {GetIceCover(planet)}\n");
            sb.AppendLine();
            sb.Append($"Cloud Cover: {GetCloudCover(planet)}\n");
            sb.AppendLine();
            sb.Append($"Surface Pressure: {GetSurfacePressureStringAtm(planet)}\n");
            sb.AppendLine();
            sb.Append($"Atmospheric Composition (Percentage): {GetAtmoString(planet)}\n");
            sb.AppendLine();
            sb.Append($"Atmospheric Composition (Partial Pressure): {GetAtmoStringPP(planet)}\n");

            return sb.ToString();
        }

        public static string GetDensity(Planet planet)
        {
            return $"{planet.PlanetSizeAndMassData.DensityGCC:0.00} g/cm3";
        }

        public static string GetBoilingPoint(Planet planet)
        {
            if (planet.PlanetProperties.PlanetType == PlanetType.GasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubGasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubSubGasGiant)
            {
                return "-";
            }
            return $"{UnitConversions.KelvinToFahrenheit(planet.PlanetAtmosphericData.BoilingPointWaterKelvin):0.00} F";
        }

        public static string GetGreenhouseRise(Planet planet)
        {
            return $"{UnitConversions.KelvinToFahrenheit(planet.PlanetTemperatureData.GreenhouseRiseKelvin):0.00} F";
        }

        public static string GetEscapeVelocity(Planet planet)
        {
            return $"{UnitConversions.CMToKM(planet.PlanetSizeAndMassData.EscapeVelocityCMSec):0.00} km/sec";
        }

        public static string GetPlanetTypeText(Planet planet)
        {
            var sb = new StringBuilder();
            switch (planet.PlanetProperties.PlanetType)
            {
                case PlanetType.GasGiant:
                    sb.Append("Gas Giant");
                    break;
                case PlanetType.SubGasGiant:
                    sb.Append("Small Gas Giant");
                    break;
                case PlanetType.SubSubGasGiant:
                    sb.Append("Gas Dwarf");
                    break;
                default:
                    sb.Append(planet.PlanetProperties.PlanetType);
                    break;
            }
            if (planet.PlanetProperties.HasResonantPeriod)
            {
                sb.Append(", Resonant Orbital Period");
            }
            if (planet.PlanetProperties.IsTidallyLocked)
            {
                sb.Append(", Tidally Locked");
            }
            if (planet.Atmosphere.SurfacePressure > 0 && planet.PlanetProperties.HasGreenhouseEffect)
            {
                sb.Append(", Runaway Greenhouse Effect");
            }
            switch (planet.Atmosphere.Breathability)
            {
                case Breathability.Breathable:
                case Breathability.Unbreathable:
                case Breathability.Poisonous:
                    sb.AppendFormat(", {0} Atmosphere", planet.Atmosphere.Breathability);
                    break;
                default:
                    sb.Append(", No Atmosphere");
                    break;
            }
            if (planet.PlanetProperties.IsEarthlike)
            {
                sb.Append(", Earthlike");
            }
            return sb.ToString();
        }

        public static string GetSurfaceGravityG(Planet planet)
        {
            if (planet.PlanetProperties.PlanetType == PlanetType.GasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubGasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubSubGasGiant)
            {
                return "Oh yeah";
            }
            return $"{planet.PlanetSizeAndMassData.SurfaceGravityG:0.00} G";
        }

        public static string GetHydrosphere(Planet planet)
        {
            return $"{planet.PlanetCoverageData.WaterCoverFraction * 100:0.0}%";
        }

        public static string GetIceCover(Planet planet)
        {
            return $"{planet.PlanetCoverageData.IceCoverFraction * 100:0.0}%";
        }

        public static string GetCloudCover(Planet planet)
        {
            return $"{planet.PlanetCoverageData.CloudCoverFraction * 100:0.0}%";
        }

        public static string GetDayTemp(Planet planet)
        {
            return $"{UnitConversions.KelvinToFahrenheit(planet.PlanetTemperatureData.DaytimeTempKelvin):0.0} F";
        }

        public static string GetNightTemp(Planet planet)
        {
            return $"{UnitConversions.KelvinToFahrenheit(planet.PlanetTemperatureData.NighttimeTempKelvin):0.0} F";
        }

        public static string GetExoTemp(Planet planet)
        {
            return $"{planet.PlanetTemperatureData.ExosphereTempKelvin:0.0} K";
        }

        public static string GetEstimatedHillSphereKM(Planet planet)
        {
            return $"{planet.PlanetOrbitData.HillSphereKM:n0} km";
        }

        public static string GetLengthofDayHours(Planet planet)
        {
            if (planet.PlanetOrbitData.DayLengthHours > 24 * 7)
            {
                return
                    $"{planet.PlanetOrbitData.DayLengthHours / 24:0.0} days ({planet.PlanetOrbitData.DayLengthHours:0.0} hours)";
            }
            return $"{planet.PlanetOrbitData.DayLengthHours:0.0} hours";
        }

        public static string GetOrbitalPeriodDay(Planet planet)
        {
            if (planet.PlanetOrbitData.OrbitalPeriodDays > 365 * 1.5)
            {
                return $"{planet.PlanetOrbitData.OrbitalPeriodDays / 365:0.00} ({planet.PlanetOrbitData.OrbitalPeriodDays:0.0} days)";
            }
            return $"{planet.PlanetOrbitData.OrbitalPeriodDays:0.0} days";
        }

        public static string GetOrbitalEccentricity(Planet planet)
        {
            return $"{planet.PlanetOrbitData.Eccentricity:0.00}";
        }

        public static string GetOrbitalDistanceAU(Planet planet)
        {
            return $"{planet.PlanetOrbitData.SemiMajorAxisAU:0.00} AU";
        }

        public static string GetPlanetNumber(Planet planet)
        {
            return $"{planet.Position}.";
        }

        public static string GetRadiusKM(Planet planet)
        {
            return $"{planet.PlanetSizeAndMassData.RadiusKM:0} km";
        }

        public static string GetRadiusER(Planet planet)
        {
            return $"{planet.PlanetSizeAndMassData.RadiusKM / GlobalConstants.KM_EARTH_RADIUS:0.00} ER";
        }

        public static string GetMassStringEM(Planet planet)
        {
            return $"{UnitConversions.SolarMassesToEarthMasses(planet.PlanetSizeAndMassData.MassSM):0.00} EM";
        }

        public static string GetSurfacePressureStringAtm(Planet planet)
        {
            if (planet.PlanetProperties.PlanetType == PlanetType.GasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubGasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubSubGasGiant)
            {
                return "Uh, a lot";
            }
            return $"{UnitConversions.MillibarsToAtm(planet.Atmosphere.SurfacePressure):0.000} atm";
        }

        public static string GetAtmoStringPP(Planet planet)
        {
            if (planet.PlanetProperties.PlanetType == PlanetType.GasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubGasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubSubGasGiant)
            {
                return "Yes";
            }
            if (planet.Atmosphere.Composition.Count == 0)
            {
                return "None";
            }
            var str = "";
            var orderedGases = planet.Atmosphere.Composition.OrderByDescending(g => g.surf_pressure).ToArray();
            if (orderedGases.Length == 0)
            {
                return "Trace gases only";
            }
            for (var i = 0; i < orderedGases.Length; i++)
            {
                var gas = orderedGases[i];
                var curGas = gas.GasType;
                str += $"{curGas.Symbol} [{gas.surf_pressure:0.0000} mb]";
                if (i < orderedGases.Length - 1)
                {
                    str += ", ";
                }
            }
            return str;
        }

        public static string GetPoisonString(Planet planet)
        {
            var str = "";
            var orderedGases = planet.Atmosphere.PoisonousGases.OrderByDescending(g => g.surf_pressure).ToList();
            for (var i = 0; i < orderedGases.Count; i++)
            {
                if (orderedGases[i].surf_pressure > 1)
                {
                    str += $"{orderedGases[i].surf_pressure:0.0000}mb {orderedGases[i].GasType.Symbol}";
                }
                else
                {
                    var ppm = UnitConversions.MillibarsToPPM(orderedGases[i].surf_pressure);
                    str += $"{ppm:0.0000}ppm {orderedGases[i].GasType.Symbol}";
                }
                if (i < orderedGases.Count - 1)
                {
                    str += ", ";
                }
            }
            return str;
        }

        public static string GetAtmoString(Planet planet, double minFraction = 1.0)
        {
            if (planet.PlanetProperties.PlanetType == PlanetType.GasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubGasGiant || planet.PlanetProperties.PlanetType == PlanetType.SubSubGasGiant)
            {
                return "Yes";
            }
            if (planet.Atmosphere.Composition.Count == 0)
            {
                return "None";
            }
            if (planet.Atmosphere.SurfacePressure < 0.0005)
            {
                return "Almost None";
            }

            var str = "";
            var orderedGases = planet.Atmosphere.Composition.Where(g => ((g.surf_pressure / planet.Atmosphere.SurfacePressure) * 100) > minFraction).OrderByDescending(g => g.surf_pressure).ToArray();
            for (var i = 0; i < orderedGases.Length; i++)
            {
                var gas = orderedGases[i];
                var curGas = gas.GasType;
                var pct = (gas.surf_pressure / planet.Atmosphere.SurfacePressure) * 100;
                str += $"{pct:0.0}% {curGas.Symbol}";
                if (i < orderedGases.Length - 1)
                {
                    str += ", ";
                }
            }
            if (orderedGases.Length < planet.Atmosphere.Composition.Count)
            {
                var traceGasSum = 0.0;
                foreach (var gas in planet.Atmosphere.Composition)
                {
                    var frac = (gas.surf_pressure / planet.Atmosphere.SurfacePressure) * 100;
                    if (frac <= minFraction)
                    {
                        traceGasSum += frac;
                    }
                }
                if (traceGasSum > 0.05)
                {
                    str += $", {traceGasSum:0.0}% trace gases";
                }
            }
            return str;
        }
    }
}
