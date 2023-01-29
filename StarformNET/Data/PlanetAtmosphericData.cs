namespace DLS.StarformNET.Data;

public class PlanetAtmosphericData
{
    public PlanetAtmosphericData()
    {
    }

    /// <summary>
    /// The root-mean-square velocity of N2 at the planet's exosphere given
    /// in cm/sec. Used to determine where or not a planet is capable of
    /// retaining an atmosphere.
    /// </summary>
    public double RMSVelocityCMSec { get; set; }

    /// <summary>
    /// The smallest molecular weight the planet is capable of retaining.
    /// I believe this is in g/mol.
    /// </summary>
    public double MolecularWeightRetained { get; set; }

    /// <summary>
    /// Unitless value for the inventory of volatile gases that result from
    /// outgassing. Used in the calculation of surface pressure. See Fogg
    /// eq. 16. 
    /// </summary>
    public double VolatileGasInventory { get; set; }

    /// <summary>
    /// Boiling point of water on the planet given in Kelvin.
    /// </summary>
    public double BoilingPointWaterKelvin { get; set; }

    /// <summary>
    /// Planetary albedo. Unitless value between 0 (no reflection) and 1 
    /// (completely reflective).
    /// </summary>
    public double Albedo { get; set; }
}