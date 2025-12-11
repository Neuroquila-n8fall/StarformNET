namespace DLS.StarformNET.Data;


/// <summary>
/// Contains general properties of a planet such as the type, if it's comparable to earth and so on.
/// </summary>
public class PlanetProperties
{
    public PlanetProperties()
    {
    }

    public PlanetType PlanetType { get; set; }

    public bool IsGasGiant => PlanetType is PlanetType.GasGiant or PlanetType.SubGasGiant or PlanetType.SubSubGasGiant;

    public bool IsTidallyLocked { get; set; }
    public bool IsEarthlike { get; set; }
    public bool IsHabitable { get; set; }
    public bool HasResonantPeriod { get; set; }
    public bool HasGreenhouseEffect { get; set; }
}