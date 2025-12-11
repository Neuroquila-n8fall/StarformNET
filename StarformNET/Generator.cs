using System;
using System.Collections.Generic;
using DLS.StarformNET.Data;

namespace DLS.StarformNET;

public class Generator
{
    public static StellarGroup GenerateStellarGroup(int seed, int numSystems, SystemGenerationOptions genOptions = null)
    {
        Utilities.InitRandomSeed(seed);
        genOptions ??= SystemGenerationOptions.DefaultOptions;
        var group = new StellarGroup { Seed = seed, GenOptions = genOptions, Systems = new List<StellarSystem>() };
        for (var i = 0; i < numSystems; i++)
        {
            var name = $"System {i}";
            group.Systems.Add(GenerateStellarSystem(name, genOptions));
        }

        return group;
    }

    public static StellarSystem GenerateStellarSystem(string systemName, SystemGenerationOptions genOptions = null,
        Star sun = null, List<PlanetSeed> seedSystem = null)
    {
        genOptions ??= SystemGenerationOptions.DefaultOptions;
        sun ??= StarGenerator.GetDefaultStar();
        var useRandomTilt = seedSystem == null;

        var accrete = new Accrete(genOptions.CloudEccentricity, genOptions.GasDensityRatio);
        var outer_planet_limit = HelperFunctions.GetOuterLimit(sun);
        var outer_dust_limit = HelperFunctions.GetStellarDustLimit(sun.Mass);
        seedSystem = seedSystem ?? accrete.GetPlanetaryBodies(sun.Mass,
            sun.Luminosity, 0.0, outer_dust_limit, outer_planet_limit,
            genOptions.DustDensityCoeff, null, true);

        var planets = GeneratePlanets(sun, seedSystem, useRandomTilt, genOptions);
        return new StellarSystem
        {
            Options = genOptions,
            Planets = planets,
            Name = systemName,
            Star = sun
        };
    }

    private static List<Planet> GeneratePlanets(Star sun, List<PlanetSeed> seeds, bool useRandomTilt,
        SystemGenerationOptions genOptions)
    {
        var planets = new List<Planet>();
        for (var i = 0; i < seeds.Count; i++)
        {
            var planetNo = i + 1; // start counting planets at 1
            var seed = seeds[i];

            var planet_id = planetNo.ToString();

            var planet = GeneratePlanet(seed, planetNo, ref sun, useRandomTilt, planet_id, false, genOptions);
            planets.Add(planet);

            // Now we're ready to test for habitable planets,
            // so we can count and log them and such
            HelperFunctions.CheckPlanet(planet, planet_id, false);

            for (var m = 0; m < planet.PlanetMoonData.Moons.Count; m++)
            {
                var moon_id = $"{planet_id}.{m}";
                HelperFunctions.CheckPlanet(planet.PlanetMoonData.Moons[m], moon_id, true);
            }
        }

        return planets;
    }

    private static Planet GeneratePlanet(PlanetSeed seed, int planetNo, ref Star sun, bool useRandomTilt,
        string planetID, bool isMoon, SystemGenerationOptions genOptions)
    {
        var planet = new Planet(seed, sun, planetNo);

        planet.PlanetOrbitData.OrbitZone =
            HelperFunctions.OrbitalZone(sun.Luminosity, planet.PlanetOrbitData.SemiMajorAxisAU);
        planet.PlanetOrbitData.OrbitalPeriodDays = HelperFunctions.Period(planet.PlanetOrbitData.SemiMajorAxisAU,
            planet.PlanetSizeAndMassData.MassSM, sun.Mass);
        if (useRandomTilt)
            planet.PlanetOrbitData.AxialTiltDegrees =
                HelperFunctions.Inclination(planet.PlanetOrbitData.SemiMajorAxisAU);
        planet.PlanetTemperatureData.ExosphereTempKelvin = GlobalConstants.EARTH_EXOSPHERE_TEMP /
                                                           Utilities.Pow2(planet.PlanetOrbitData.SemiMajorAxisAU /
                                                                          sun.EcosphereRadiusAU);
        planet.PlanetAtmosphericData.RMSVelocityCMSec = HelperFunctions.RMSVelocity(GlobalConstants.MOL_NITROGEN,
            planet.PlanetTemperatureData.ExosphereTempKelvin);
        planet.PlanetSizeAndMassData.CoreRadiusKM =
            HelperFunctions.KothariRadius(planet.PlanetSizeAndMassData.DustMassSM, false,
                planet.PlanetOrbitData.OrbitZone);

        // Calculate the radius as a gas giant, to verify it will retain gas.
        // Then if mass > Earth, it's at least 5% gas and retains He, it's
        // some flavor of gas giant.

        planet.PlanetSizeAndMassData.DensityGCC = HelperFunctions.EmpiricalDensity(planet.PlanetSizeAndMassData.MassSM,
            planet.PlanetOrbitData.SemiMajorAxisAU, sun.EcosphereRadiusAU, true);
        planet.PlanetSizeAndMassData.RadiusKM = HelperFunctions.VolumeRadius(planet.PlanetSizeAndMassData.MassSM,
            planet.PlanetSizeAndMassData.DensityGCC);

        planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2 =
            HelperFunctions.Acceleration(planet.PlanetSizeAndMassData.MassSM, planet.PlanetSizeAndMassData.RadiusKM);
        planet.PlanetSizeAndMassData.SurfaceGravityG =
            HelperFunctions.Gravity(planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2);

        planet.PlanetAtmosphericData.MolecularWeightRetained = HelperFunctions.MinMolecularWeight(planet);

        // Is the planet a gas giant?
        if (planet.PlanetSizeAndMassData.MassSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES > 1.0 &&
            planet.PlanetSizeAndMassData.GasMassSM / planet.PlanetSizeAndMassData.MassSM > 0.05 &&
            planet.PlanetAtmosphericData.MolecularWeightRetained <= 4.0)
        {
            if (planet.PlanetSizeAndMassData.GasMassSM / planet.PlanetSizeAndMassData.MassSM < 0.20)
                planet.PlanetProperties.PlanetType = PlanetType.SubSubGasGiant;
            else if (planet.PlanetSizeAndMassData.MassSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES < 20.0)
                planet.PlanetProperties.PlanetType = PlanetType.SubGasGiant;
            else
                planet.PlanetProperties.PlanetType = PlanetType.GasGiant;
        }
        else // If not, it's rocky.
        {
            planet.PlanetSizeAndMassData.RadiusKM = HelperFunctions.KothariRadius(planet.PlanetSizeAndMassData.MassSM,
                false, planet.PlanetOrbitData.OrbitZone);
            planet.PlanetSizeAndMassData.DensityGCC = HelperFunctions.VolumeDensity(planet.PlanetSizeAndMassData.MassSM,
                planet.PlanetSizeAndMassData.RadiusKM);

            planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2 =
                HelperFunctions.Acceleration(planet.PlanetSizeAndMassData.MassSM,
                    planet.PlanetSizeAndMassData.RadiusKM);
            planet.PlanetSizeAndMassData.SurfaceGravityG =
                HelperFunctions.Gravity(planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2);

            if (planet.PlanetSizeAndMassData.GasMassSM / planet.PlanetSizeAndMassData.MassSM > 0.000001)
            {
                var h2Mass = planet.PlanetSizeAndMassData.GasMassSM * 0.85;
                var heMass = (planet.PlanetSizeAndMassData.GasMassSM - h2Mass) * 0.999;

                var h2Life = HelperFunctions.GasLife(GlobalConstants.MOL_HYDROGEN,
                    planet.PlanetTemperatureData.ExosphereTempKelvin,
                    planet.PlanetSizeAndMassData.SurfaceGravityG, planet.PlanetSizeAndMassData.RadiusKM);
                var heLife = HelperFunctions.GasLife(GlobalConstants.HELIUM,
                    planet.PlanetTemperatureData.ExosphereTempKelvin,
                    planet.PlanetSizeAndMassData.SurfaceGravityG, planet.PlanetSizeAndMassData.RadiusKM);

                if (h2Life < sun.AgeYears)
                {
                    var h2Loss = (1.0 - 1.0 / Math.Exp(sun.AgeYears / h2Life)) * h2Mass;

                    planet.PlanetSizeAndMassData.GasMassSM -= h2Loss;
                    planet.PlanetSizeAndMassData.MassSM -= h2Loss;

                    planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2 =
                        HelperFunctions.Acceleration(planet.PlanetSizeAndMassData.MassSM,
                            planet.PlanetSizeAndMassData.RadiusKM);
                    planet.PlanetSizeAndMassData.SurfaceGravityG =
                        HelperFunctions.Gravity(planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2);
                }

                if (heLife < sun.AgeYears)
                {
                    var heLoss = (1.0 - 1.0 / Math.Exp(sun.AgeYears / heLife)) * heMass;

                    planet.PlanetSizeAndMassData.GasMassSM -= heLoss;
                    planet.PlanetSizeAndMassData.MassSM -= heLoss;

                    planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2 =
                        HelperFunctions.Acceleration(planet.PlanetSizeAndMassData.MassSM,
                            planet.PlanetSizeAndMassData.RadiusKM);
                    planet.PlanetSizeAndMassData.SurfaceGravityG =
                        HelperFunctions.Gravity(planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2);
                }
            }
        }

        planet.PlanetOrbitData.AngularVelocityRadSec = HelperFunctions.AngularVelocity(planet);
        planet.PlanetOrbitData.DayLengthHours = HelperFunctions.DayLength(planet.PlanetOrbitData.AngularVelocityRadSec,
            planet.PlanetOrbitData.OrbitalPeriodDays,
            planet.PlanetOrbitData.Eccentricity);
        planet.PlanetProperties.HasResonantPeriod = HelperFunctions.HasResonantPeriod(
            planet.PlanetOrbitData.AngularVelocityRadSec,
            planet.PlanetOrbitData.DayLengthHours, planet.PlanetOrbitData.OrbitalPeriodDays,
            planet.PlanetOrbitData.Eccentricity);
        planet.PlanetSizeAndMassData.EscapeVelocityCMSec =
            HelperFunctions.EscapeVelocity(planet.PlanetSizeAndMassData.MassSM, planet.PlanetSizeAndMassData.RadiusKM);
        planet.PlanetOrbitData.HillSphereKM = HelperFunctions.SimplifiedHillSphereKM(sun.Mass,
            planet.PlanetSizeAndMassData.MassSM, planet.PlanetOrbitData.SemiMajorAxisAU);

        if (planet.PlanetProperties.IsGasGiant)
        {
            planet.PlanetProperties.HasGreenhouseEffect = false;
            planet.PlanetAtmosphericData.VolatileGasInventory = GlobalConstants.INCREDIBLY_LARGE_NUMBER;
            planet.Atmosphere.SurfacePressure = GlobalConstants.INCREDIBLY_LARGE_NUMBER;

            planet.PlanetAtmosphericData.BoilingPointWaterKelvin = GlobalConstants.INCREDIBLY_LARGE_NUMBER;

            planet.PlanetTemperatureData.SurfaceTempKelvin = GlobalConstants.INCREDIBLY_LARGE_NUMBER;
            planet.PlanetTemperatureData.GreenhouseRiseKelvin = 0;
            planet.PlanetAtmosphericData.Albedo = Utilities.About(GlobalConstants.GAS_GIANT_ALBEDO, 0.1);
            planet.PlanetCoverageData.WaterCoverFraction = 1.0;
            planet.PlanetCoverageData.CloudCoverFraction = 1.0;
            planet.PlanetCoverageData.IceCoverFraction = 0.0;
            planet.PlanetSizeAndMassData.SurfaceGravityG =
                HelperFunctions.Gravity(planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2);
            planet.PlanetAtmosphericData.MolecularWeightRetained = HelperFunctions.MinMolecularWeight(planet);
            planet.PlanetSizeAndMassData.SurfaceGravityG = GlobalConstants.INCREDIBLY_LARGE_NUMBER;
        }
        else
        {
            planet.PlanetSizeAndMassData.SurfaceGravityG =
                HelperFunctions.Gravity(planet.PlanetSizeAndMassData.SurfaceAccelerationCMSec2);
            planet.PlanetAtmosphericData.MolecularWeightRetained = HelperFunctions.MinMolecularWeight(planet);

            planet.PlanetProperties.HasGreenhouseEffect =
                HelperFunctions.Greenhouse(sun.EcosphereRadiusAU, planet.PlanetOrbitData.SemiMajorAxisAU);
            planet.PlanetAtmosphericData.VolatileGasInventory = HelperFunctions.VolatileInventory(
                planet.PlanetSizeAndMassData.MassSM, planet.PlanetSizeAndMassData.EscapeVelocityCMSec,
                planet.PlanetAtmosphericData.RMSVelocityCMSec, sun.Mass,
                planet.PlanetOrbitData.OrbitZone, planet.PlanetProperties.HasGreenhouseEffect,
                planet.PlanetSizeAndMassData.GasMassSM / planet.PlanetSizeAndMassData.MassSM > 0.000001);
            planet.Atmosphere.SurfacePressure = HelperFunctions.Pressure(
                planet.PlanetAtmosphericData.VolatileGasInventory, planet.PlanetSizeAndMassData.RadiusKM,
                planet.PlanetSizeAndMassData.SurfaceGravityG);

            planet.PlanetAtmosphericData.BoilingPointWaterKelvin = Math.Abs(planet.Atmosphere.SurfacePressure) < 0.001
                ? 0.0
                : HelperFunctions.BoilingPoint(planet.Atmosphere.SurfacePressure);

            // Sets: planet.surf_temp, planet.greenhs_rise, planet.albedo, planet.hydrosphere,
            // planet.cloud_cover, planet.ice_cover
            HelperFunctions.IterateSurfaceTemp(ref planet);

            PlanetComposition.CalculateGases(planet, genOptions.GasTable);
            PlanetComposition.CalculateSolidInventory(planet, genOptions.GasTable);

            planet.PlanetProperties.IsTidallyLocked = HelperFunctions.IsTidallyLocked(planet);

            switch (planet.Atmosphere.SurfacePressure)
            {
                // Assign planet type
                case < 1.0 when !isMoon &&
                                planet.PlanetSizeAndMassData.MassSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES <
                                GlobalConstants.ASTEROID_MASS_LIMIT:
                    planet.PlanetProperties.PlanetType = PlanetType.Asteroids;
                    break;
                case < 1.0:
                    planet.PlanetProperties.PlanetType = PlanetType.Barren;
                    break;
                // Retains Hydrogen
                case > 6000.0 when planet.PlanetAtmosphericData.MolecularWeightRetained <= 2.0:
                    planet.PlanetProperties.PlanetType = PlanetType.SubSubGasGiant;
                    planet.Atmosphere.Composition = new List<Gas>();
                    break;
                default:
                {
                    // Atmospheres:
                    // TODO remove PlanetType enum entirely and replace it with a more flexible classification systme
                    if (planet.PlanetCoverageData.WaterCoverFraction >= 0.95) // >95% water
                    {
                        planet.PlanetProperties.PlanetType = PlanetType.Water;
                    }
                    else if (planet.PlanetCoverageData.IceCoverFraction >= 0.95) // >95% ice
                    {
                        planet.PlanetProperties.PlanetType = PlanetType.Ice;
                    }
                    else if (planet.PlanetCoverageData.WaterCoverFraction > 0.05) // Terrestrial
                    {
                        planet.PlanetProperties.PlanetType = PlanetType.Terrestrial;
                    }
                    else if (planet.PlanetTemperatureData.MaxTempKelvin >
                             planet.PlanetAtmosphericData.BoilingPointWaterKelvin) // Hot = Venusian
                    {
                        planet.PlanetProperties.PlanetType = PlanetType.Venusian;
                    }
                    else if (planet.PlanetSizeAndMassData.GasMassSM / planet.PlanetSizeAndMassData.MassSM >
                             0.0001) // Accreted gas, but no greenhouse or liquid water make it an ice world
                    {
                        planet.PlanetProperties.PlanetType = PlanetType.Ice;
                        planet.PlanetCoverageData.IceCoverFraction = 1.0;
                    }
                    else if (planet.Atmosphere.SurfacePressure <= 250.0) // Thin air = Martian
                    {
                        planet.PlanetProperties.PlanetType = PlanetType.Martian;
                    }
                    else if (planet.PlanetTemperatureData.SurfaceTempKelvin < GlobalConstants.FREEZING_POINT_OF_WATER)
                    {
                        planet.PlanetProperties.PlanetType = PlanetType.Ice;
                    }
                    else
                    {
                        planet.PlanetProperties.PlanetType = PlanetType.Unknown;
                    }

                    break;
                }
            }
        }

        // Generate moons
        planet.PlanetMoonData.Moons = new List<Planet>();

        if (isMoon) return planet;

        var curMoon = seed.FirstMoon;
        var n = 0;
        while (curMoon != null)
        {
            if (curMoon.Mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES > .000001)
            {
                curMoon.SemiMajorAxisAU = planet.PlanetOrbitData.SemiMajorAxisAU;
                curMoon.Eccentricity = planet.PlanetOrbitData.Eccentricity;

                n++;

                var moon_id = $"{planetID}.{n}";

                var generatedMoon = GeneratePlanet(curMoon, n, ref sun, useRandomTilt, moon_id, true, genOptions);

                var roche_limit = 2.44 * planet.PlanetSizeAndMassData.RadiusKM * Math.Pow(
                    planet.PlanetSizeAndMassData.DensityGCC / generatedMoon.PlanetSizeAndMassData.DensityGCC,
                    1.0 / 3.0);
                var hill_sphere = planet.PlanetOrbitData.SemiMajorAxisAU * GlobalConstants.KM_PER_AU *
                                  Math.Pow(planet.PlanetSizeAndMassData.MassSM / (3.0 * sun.Mass), 1.0 / 3.0);

                if (roche_limit * 3.0 < hill_sphere)
                {
                    generatedMoon.PlanetMoonData.MoonSemiMajorAxisAU =
                        Utilities.RandomNumber(roche_limit * 1.5, hill_sphere / 2.0) / GlobalConstants.KM_PER_AU;
                    generatedMoon.PlanetMoonData.MoonEccentricity = Utilities.RandomEccentricity();
                }
                else
                {
                    generatedMoon.PlanetMoonData.MoonSemiMajorAxisAU = 0;
                    generatedMoon.PlanetMoonData.MoonEccentricity = 0;
                }

                planet.PlanetMoonData.Moons.Add(generatedMoon);
            }

            curMoon = curMoon.NextPlanet;
        }

        return planet;
    }
}