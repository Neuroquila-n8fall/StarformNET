namespace DLS.StarformNET.Data;

public class PlanetProperties
{
    public PlanetProperties()
    {
    }

    public PlanetType Type { get; set; }

    public bool IsGasGiant => System.Type == PlanetType.GasGiant ||
                              System.Type == PlanetType.SubGasGiant ||
                              System.Type == PlanetType.SubSubGasGiant;

    public bool IsTidallyLocked { get; set; }
    public bool IsEarthlike { get; set; }
    public bool IsHabitable { get; set; }
    public bool HasResonantPeriod { get; set; }
    public bool HasGreenhouseEffect { get; set; }
}