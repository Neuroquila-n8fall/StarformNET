namespace DLS.StarformNET.Data;

public class PlanetOrbitData
{
    public PlanetOrbitData()
    {
    }

    public PlanetOrbitData(double semiMajorAxisAu, double eccentricity)
    {
        SemiMajorAxisAU = semiMajorAxisAu;
        Eccentricity = eccentricity;
    }

    /// <summary>
    /// Semi-major axis of the body's orbit in astronomical units (au).
    /// </summary>
    public double SemiMajorAxisAU { get; set; }

    /// <summary>
    /// Eccentricity of the body's orbit.
    /// </summary>
    public double Eccentricity { get; set; }

    /// <summary>
    /// Axial tilt of the planet expressed in degrees.
    /// </summary>
    public double AxialTiltDegrees { get; set; }

    /// <summary>
    /// Orbital zone the planet is located in. Value is 1, 2, or 3. Used in
    /// radius and volatile inventory calculations.
    /// </summary>
    public int OrbitZone { get; set; }

    /// <summary>
    /// The length of the planet's year in days.
    /// </summary>
    public double OrbitalPeriodDays { get; set; }

    /// <summary>
    /// Angular velocity about the planet's axis in radians/sec.
    /// </summary>
    public double AngularVelocityRadSec { get; set; }

    /// <summary>
    /// The length of the planet's day in hours.
    /// </summary>
    public double DayLengthHours { get; set; }

    /// <summary>
    /// The Hill sphere of the planet expressed in km.
    /// </summary>
    public double HillSphereKM { get; set; }
}