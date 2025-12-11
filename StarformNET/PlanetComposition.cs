using System;
using System.Collections.Generic;
using System.Linq;
using DLS.StarformNET.Data;

namespace DLS.StarformNET;

public class PlanetComposition
{
    public static void CalculateGases(Planet planet, ChemType[] gasTable)
    {
        var sun = planet.Star;
        planet.Atmosphere.Composition = new List<Gas>();

        if (!(planet.Atmosphere.SurfacePressure > 0))
        {
            return;
        }

        var amount = new double[gasTable.Length];
        double totamount = 0;
        var pressure = planet.Atmosphere.SurfacePressure / GlobalConstants.MILLIBARS_PER_BAR;
        var n = 0;

        // Determine the relative abundance of each gas in the planet's atmosphere
        for (var i = 0; i < gasTable.Length; i++)
        {
            //The boiling point relative to the atmospheric pressure
            var relativeBoilingPoint = gasTable[i].BoilingPoint / (373.0 * ((Math.Log((pressure) + 0.001) / -5050.5) + (1.0 / 373.0)));

            // TODO move both of these conditions to separate methods
            if (relativeBoilingPoint >= 0 && relativeBoilingPoint < planet.PlanetTemperatureData.NighttimeTempKelvin && (gasTable[i].Weight >= planet.PlanetAtmosphericData.MolecularWeightRetained))
            {
                double abund, react;
                CheckForSpecialRules(out abund, out react, pressure, planet, gasTable[i]);

                var vrms = HelperFunctions.RMSVelocity(gasTable[i].Weight, planet.PlanetTemperatureData.ExosphereTempKelvin);
                var pvrms = Math.Pow(1 / (1 + vrms / planet.PlanetSizeAndMassData.EscapeVelocityCMSec), sun.AgeYears / 1e9);

                var fract = (1 - (planet.PlanetAtmosphericData.MolecularWeightRetained / gasTable[i].Weight));

                // Note that the amount calculated here is unitless and doesn't really mean
                // anything except as a relative value
                amount[i] = abund * pvrms * react * fract;
                totamount += amount[i];
                if (amount[i] > 0.0)
                {
                    n++;
                }
            }
            else
            {
                amount[i] = 0.0;
            }
        }

        // For each gas present, calculate its partial pressure
        if (n <= 0) return;


        planet.Atmosphere.Composition = new List<Gas>();
        for (var i = 0; i < gasTable.Length; i++)
        {
            if (amount[i] > 0.0)
            {
                planet.Atmosphere.Composition.Add(
                    new Gas(gasTable[i], planet.Atmosphere.SurfacePressure * amount[i] / totamount));
            }
        }


    }

    private static void CheckForSpecialRules(out double abund, out double react, double pressure, Planet planet, ChemType gas)
    {
        var sun = planet.Star;
        abund = gas.SolarAbundance;

        double pres2;
        switch (gas.Symbol)
        {
            case "Ar":
                react = .15 * sun.AgeYears / 4e9;
                break;
            case "He":
                abund *= (0.001 + (planet.PlanetSizeAndMassData.GasMassSM / planet.PlanetSizeAndMassData.MassSM));
                pres2 = (0.75 + pressure);
                react = Math.Pow(1 / (1 + gas.Reactivity), sun.AgeYears / 2e9 * pres2);
                break;
            case "O" or "O2" when sun.AgeYears > 2e9 && planet.PlanetTemperatureData.SurfaceTempKelvin > 270 && planet.PlanetTemperatureData.SurfaceTempKelvin < 400:
                // pres2 = (0.65 + pressure/2); // Breathable - M: .55-1.4
                pres2 = (0.89 + pressure / 4);  // Breathable - M: .6 -1.8
                react = Math.Pow(1 / (1 + gas.Reactivity), Math.Pow(sun.AgeYears / 2e9, 0.25) * pres2);
                break;
            case "CO2" when sun.AgeYears > 2e9 && planet.PlanetTemperatureData.SurfaceTempKelvin > 270 && planet.PlanetTemperatureData.SurfaceTempKelvin < 400:
                pres2 = (0.75 + pressure);
                react = Math.Pow(1 / (1 + gas.Reactivity), Math.Pow(sun.AgeYears / 2e9, 0.5) * pres2);
                react *= 1.5;
                break;
            default:
                pres2 = (0.75 + pressure);
                react = Math.Pow(1 / (1 + gas.Reactivity), sun.AgeYears / 2e9 * pres2);
                break;
        }
    }

    /// <summary>
    /// Calculates the non-gaseous elements on a planet. The <see cref="Planet.Atmosphere"/> has to be calculated first using <see cref="CalculateGases"/>!<br/>
    /// <strong>Attention:</strong> This method will put gases into the list regardless of the boiling point which have been ruled out by <see cref="CalculateGases"/>. 
    /// </summary>
    /// <param name="planet"></param>
    /// <param name="elements"></param>
    public static void CalculateSolidInventory(Planet planet, ChemType[] elements)
    {
        //Bail if this is a gas giant.
        if (planet.PlanetProperties.IsGasGiant)
        {
            return;
        }
        var gases = planet.Atmosphere.Composition;
        gases.AddRange(planet.Atmosphere.PoisonousGases);
        foreach (var element in elements)  
        {
            //Skip element if it's present as a gas or the element would vaporize. 
            if (gases.Any(g => g.GasType.Num == element.Num))
            {
                continue;
            }
            // Constants for solar density and solar radius
            const double solarDensity = 1.408;
            // Solar radius in kilometers
            const double solarRadius = 696000;
            // Solar mass in kilograms
            var solarMass = 1.9885 * Math.Pow(10, 30); 

            // Convert planet radius to solar masses
            var planetRadiusInSolarMasses = (planet.PlanetSizeAndMassData.RadiusKM / solarRadius) * solarMass;
            var result = (element.SolarAbundance * (planet.PlanetSizeAndMassData.DensityGCC / solarDensity)) * Math.Pow(planetRadiusInSolarMasses / solarMass, 3);

            planet.SolidInventory.Solids.Add(new Solid{ElementType = element, ScalarAmount = result, AmountSM = result * planet.PlanetSizeAndMassData.MassSM, PlanetMassSM = planet.PlanetSizeAndMassData.MassSM});
        }
    }
}