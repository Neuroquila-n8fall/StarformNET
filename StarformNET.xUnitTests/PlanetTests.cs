
using Xunit;

namespace DLS.StarformNET.UnitTests
{
    using StarformNET;
    
    public class PlanetTests
    {
        
        public class EqualTests
        {
            [Fact][Trait("Category", "Planet.Equals")]
            
            public void TestGeneratedEquality()
            {
                Utilities.InitRandomSeed(0);
                var system1 = Generator.GenerateStellarSystem("system1").Planets;

                Utilities.InitRandomSeed(0);
                var system2 = Generator.GenerateStellarSystem("system2").Planets;

                Assert.True(system1[0].Equals(system2[0]));
            }

            [Fact][Trait("Category", "Planet.Equals")]
            
            public void TestGeneratedInequality()
            {
                Utilities.InitRandomSeed(0);
                var system1 = Generator.GenerateStellarSystem("system1").Planets;

                Utilities.InitRandomSeed(1);
                var system2 = Generator.GenerateStellarSystem("system2").Planets;

                Assert.False(system1[0].Equals(system2[0]));
            }
        }
    }
}
