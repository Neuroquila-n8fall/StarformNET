namespace DLS.StarformNET.Data;

public class PlanetCoverageData
{
    public PlanetCoverageData()
    {
    }

    /// <summary>
    /// Amount of the body's surface that is covered in water. Given as a
    /// value between 0 (no water) and 1 (completely covered).
    /// </summary>
    public double WaterCoverFraction { get; set; }

    /// <summary>
    /// Amount of the body's surface that is obscured by cloud cover. Given
    /// as a value between 0 (no cloud coverage) and 1 (surface not visible
    /// at all).
    /// </summary>
    public double CloudCoverFraction { get; set; }

    /// <summary>
    /// Amount of the body's surface that is covered in ice. Given as a 
    /// value between 0 (no ice) and 1 (completely covered).
    /// </summary>
    public double IceCoverFraction { get; set; }
}