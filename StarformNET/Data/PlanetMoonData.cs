using System.Collections.Generic;

namespace DLS.StarformNET.Data;

public class PlanetMoonData
{
    public PlanetMoonData()
    {
    }

    public List<Planet> Moons { get; set; }
    public double MoonSemiMajorAxisAU { get; set; }
    public double MoonEccentricity { get; set; }
}