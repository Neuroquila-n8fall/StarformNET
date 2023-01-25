using Xunit;

namespace DLS.StarformNET.UnitTests
{
    using StarformNET;
    using Data;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Environment = DLS.StarformNET.Environment;

    class GeneratorTests
    {
        
        public class GenerateStellarSystemTests
        {
            [Fact][Trait("Category", "Generator Regression")]
            
            public void TestSeedAgainstSameSeed()
            {
                Utilities.InitRandomSeed(0);
                var refSystem = Generator.GenerateStellarSystem("test").Planets;

                Utilities.InitRandomSeed(0);
                var newSystem = Generator.GenerateStellarSystem("test").Planets;
                Assert.Equal(refSystem.Count, newSystem.Count);
                for (var i = 0; i < refSystem.Count; i++)
                {
                    Assert.True(refSystem[i].Equals(newSystem[i]), String.Format("Planet {0} not equal", i));
                }
            }

            [Fact][Trait("Category", "Generator Regression")]
            
            public void TestDifferentSeedAgainstSavedOutput()
            {
                Utilities.InitRandomSeed(0);
                var refSystem = Generator.GenerateStellarSystem("test").Planets;

                Utilities.InitRandomSeed(1);
                var newSystem = Generator.GenerateStellarSystem("test").Planets;
                var atleastOneDifferent = false;
                for (var i = 0; i < refSystem.Count; i++)
                {
                    if (!refSystem[i].Equals(newSystem[i]))
                    {
                        atleastOneDifferent = true;
                        break;
                    }
                }
                Assert.True(atleastOneDifferent);
            }
        }

        
        public class CalculateGasesTest
        {
            private double DELTA = 0.0001;

            private Star GetTestStar()
            {
                return new Star()
                {
                    Luminosity = 1.0,
                    Mass = 1.0,
                    AgeYears = 4600000000
                };
            }

            private Planet GetTestPlanetAtmosphere()
            {
                var planet = new Planet();
                planet.Star = GetTestStar();
                planet.Star.EcosphereRadiusAU = System.Math.Sqrt(planet.Star.Luminosity);
                planet.SemiMajorAxisAU = 0.723332;
                planet.Eccentricity = 0.0067;
                planet.AxialTiltDegrees = 2.8;
                planet.OrbitZone = Environment.OrbitalZone(planet.Star.Luminosity, planet.SemiMajorAxisAU);
                planet.DayLengthHours = 2802;
                planet.OrbitalPeriodDays = 225;

                planet.MassSM = 0.000002447;
                planet.GasMassSM = 2.41E-10;
                planet.DustMassSM = planet.MassSM - planet.GasMassSM;
                planet.RadiusKM = 6051.8;
                planet.DensityGCC = Environment.EmpiricalDensity(planet.MassSM, planet.SemiMajorAxisAU, planet.Star.EcosphereRadiusAU, true);
                planet.ExosphereTempKelvin = GlobalConstants.EARTH_EXOSPHERE_TEMP / Utilities.Pow2(planet.SemiMajorAxisAU / planet.Star.EcosphereRadiusAU);
                planet.SurfaceAccelerationCMSec2 = Environment.Acceleration(planet.MassSM, planet.RadiusKM);
                planet.EscapeVelocityCMSec = Environment.EscapeVelocity(planet.MassSM, planet.RadiusKM);
                
                planet.Atmosphere.SurfacePressure = 92000;
                planet.DaytimeTempKelvin = 737;
                planet.NighttimeTempKelvin = 737;
                planet.SurfaceTempKelvin = 737;
                planet.SurfaceGravityG = 0.9;
                planet.MolecularWeightRetained = Environment.MinMolecularWeight(planet);

                return planet;
            }

            private Planet GetTestPlanetNoAtmosphere()
            {
                var star = GetTestStar();
                var planet = new Planet();
                planet.Star = star;
                planet.Atmosphere.SurfacePressure = 0;
                return planet;
            }

            [Fact][Trait("Category", "Atmosphere")]
            
            public void TestEmptyPlanet()
            {
                var planet = new Planet();
                var sun = GetTestStar();
                planet.Star = sun;
                Generator.CalculateGases(planet, new ChemType[0]);
                
                Assert.Empty(planet.Atmosphere.Composition);
            }

            [Fact][Trait("Category", "Atmosphere")]
            
            public void TestEmptyChemTable()
            {
                var planet = GetTestPlanetAtmosphere();
                var star = planet.Star;
                Generator.CalculateGases(planet, new ChemType[0]);

                Assert.Empty(planet.Atmosphere.Composition);
            }

            [Fact][Trait("Category", "Atmosphere")]
            
            public void TestAtmosphereDefaultChemTable()
            {
                var expected = new Dictionary<string, double>()
                {
                    {"Ar", 87534.0399 },
                    {"CO2", 2702.8010 },
                    {"H2O", 1316.9480 },
                    {"Kr", 339.5423 },
                    {"Ne", 65.7954 },
                    {"Xe", 40.8734 },
                    {"NH3", 0.0000 },
                    {"CH4", 0.0000 },
                    {"O3", 0.0000 },
                    {"O", 0.0000 }
                };

                var planet = GetTestPlanetAtmosphere();
                var star = planet.Star;
                Generator.CalculateGases(planet, ChemType.GetDefaultTable());

                Assert.Equal(expected.Count, planet.Atmosphere.Composition.Count);

                foreach (var gas in planet.Atmosphere.Composition)
                {
                    Assert.Equal(expected[gas.GasType.symbol], gas.surf_pressure, DELTA);
                }
            }

            [Fact][Trait("Category", "Atmosphere")]
            
            public void TestNoAtmosphereDefaultChemTable()
            {
                var planet = GetTestPlanetNoAtmosphere();
                var star = planet.Star;
                Generator.CalculateGases(planet, ChemType.GetDefaultTable());

                Assert.Empty(planet.Atmosphere.Composition);
            }
        }
    }
}
