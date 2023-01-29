namespace DLS.StarformNET.Data;

public class PlanetSizeAndMassData
{
    public PlanetSizeAndMassData()
    {
    }

    public PlanetSizeAndMassData(double massSm, double dustMassSm, double gasMassSm)
    {
        MassSM = massSm;
        DustMassSM = dustMassSm;
        GasMassSM = gasMassSm;
    }

    /// <summary>
    /// The mass of the planet in units of Solar mass.
    /// </summary>
    public double MassSM { get; set; }

    /// <summary>
    /// The mass of dust retained by the planet (ie, the mass of the planet
    /// sans atmosphere). Given in units of Solar mass.
    /// </summary>
    public double DustMassSM { get; set; }

    /// <summary>
    /// The mass of gas retained by the planet (ie, the mass of its
    /// atmosphere). Given in units of Solar mass.
    /// </summary>
    public double GasMassSM { get; set; }

    /// <summary>
    /// The velocity required to escape from the body given in cm/sec.
    /// </summary>
    public double EscapeVelocityCMSec { get; set; }

    /// <summary>
    /// The gravitational acceleration felt at the surface of the planet. Given in cm/sec^2
    /// </summary>
    public double SurfaceAccelerationCMSec2 { get; set; }

    /// <summary>
    /// The gravitational acceleration felt at the surface of the planet. Given as a fraction of Earth gravity (Gs).
    /// </summary>
    public double SurfaceGravityG { get; set; }

    /// <summary>
    /// The radius of the planet's core in km.
    /// </summary>
    public double CoreRadiusKM { get; set; }

    /// <summary>
    /// The radius of the planet's surface in km.
    /// </summary>
    public double RadiusKM { get; set; }

    /// <summary>
    /// The density of the planet given in g/cc. 
    /// </summary>
    public double DensityGCC { get; set; }
}