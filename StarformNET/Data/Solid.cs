using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.StarformNET.Data
{
    public class Solid
    {
        public ChemType ElementType { get; set; }
        /// <summary>
        /// The amount of this element
        /// </summary>
        public double ScalarAmount { get; set; }
        public double PlanetMassSM { get; set; }
        public double AmountSM { get; set; }
        public double AmountKg => GlobalConstants.SOLAR_MASS_IN_KILOGRAMS * AmountSM;
        public double PlanetMassKG => GlobalConstants.SOLAR_MASS_IN_KILOGRAMS * PlanetMassSM;

        public override string ToString()
        {
            return $"{ElementType.Symbol}: {AmountSM} SM in inventory";
        }
    }
}
