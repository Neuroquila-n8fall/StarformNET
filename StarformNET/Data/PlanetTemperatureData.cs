namespace DLS.StarformNET.Data;

public class PlanetTemperatureData
{
    public PlanetTemperatureData()
    {
    }

    /// <summary>
    /// Illumination received by the body at at the farthest point of its
    /// orbit. 1.0 is the amount of illumination received by an object 1 au
    /// from the Sun.
    /// </summary>
    public double Illumination { get; set; }

    /// <summary>
    /// Temperature at the body's exosphere given in Kelvin.
    /// </summary>
    public double ExosphereTempKelvin { get; set; }

    /// <summary>
    /// Temperature at the body's surface given in Kelvin.
    /// </summary>
    public double SurfaceTempKelvin { get; set; }

    /// <summary>
    /// Amount (in Kelvin) that the planet's surface temperature is being
    /// increased by a runaway greenhouse effect.
    /// </summary>
    public double GreenhouseRiseKelvin { get; set; }

    /// <summary>
    /// Average daytime temperature in Kelvin.
    /// </summary>
    public double DaytimeTempKelvin { get; set; }

    /// <summary>
    /// Average nighttime temperature in Kelvin.
    /// </summary>
    public double NighttimeTempKelvin { get; set; }

    /// <summary>
    /// Maximum (summer/day) temperature in Kelvin.
    /// </summary>
    public double MaxTempKelvin { get; set; }

    /// <summary>
    /// Minimum (winter/night) temperature in Kelvin.
    /// </summary>
    public double MinTempKelvin { get; set; }
}