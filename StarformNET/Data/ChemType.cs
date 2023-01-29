using System.Text.Json;

namespace DLS.StarformNET.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    // TODO abunde isn't used anywhere
    // TODO break out abundance into a separate class for star/planet profiles
    public class ChemType
    {
        /// <summary>
        /// Number of this element inside the periodic table
        /// </summary>
        public int    Num        { get; set; }
        /// <summary>
        /// Chemical Symbol
        /// </summary>
        public string Symbol     { get; set; }
        /// <summary>
        /// HTML Encoded Symbol
        /// </summary>
        public string HtmlSymbol { get; set; }
        /// <summary>
        /// Name of this element (eng.)
        /// </summary>
        public string Name       { get; set; }
        /// <summary>
        /// Molecular Weight (mol)
        /// </summary>
        public double Weight     { get; set; }
        /// <summary>
        /// Melting Point
        /// </summary>
        public double MeltingPoint       { get; set; }
        /// <summary>
        /// Boiling Point
        /// </summary>
        public double BoilingPoint       { get; set; }
        /// <summary>
        /// Density
        /// </summary>
        public double Density    { get; set; }
        /// <summary>
        /// Abundance, unknown
        /// </summary>
        public double Abunde     { get; set; }
        /// <summary>
        /// Abundance relative to Sun (solar)
        /// </summary>
        public double SolarAbundance     { get; set; }  // Solar system abundance
        /// <summary>
        /// Reactivity. The higher, the more reactive
        /// </summary>
        public double Reactivity { get; set; }
        /// <summary>
        /// Max inspired partial pressure im millibars
        /// </summary>
        public double MaxIpp     { get; set; }

        public ChemType(int an, string sym, string htmlsym, string name, double weight, double m, double b, double dens, double ae, double abs, double rea, double mipp)
        {
            Num = an;
            Symbol = sym;
            HtmlSymbol = htmlsym;
            Name = name;
            Weight = weight;
            MeltingPoint = m;
            BoilingPoint = b;
            Density = dens;
            Abunde = ae;
            SolarAbundance = abs;
            Reactivity = rea;
            MaxIpp = mipp;
        }

        public static ChemType[] LoadFromFile(string file)
        {
            var chemTable = new List<ChemType>();
            using (var r = new StreamReader(file))
            {
                var json = r.ReadToEnd();
                var options = new JsonSerializerOptions()
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                var items = JsonSerializer.Deserialize<List<List<JsonElement>>>(json,options);


                foreach (var item in items)
                {
                    var num = item[0].GetInt32();
                    var sym = item[1].GetString();
                    var name = item[2].GetString();
                    var weight = item[3].GetDouble();
                    var melt = item[4].GetDouble();
                    var boil = item[5].GetDouble();
                    var dens = item[6].GetDouble();
                    var abunde = item[7].GetDouble();
                    var abunds = item[8].GetDouble();
                    var rea = item[9].GetDouble();
                    var maxIpp = (item.Count == 11 ? item[10].GetDouble() : 0) * GlobalConstants.MMHG_TO_MILLIBARS;


                    chemTable.Add(new ChemType(num, sym, sym, name, weight, melt, boil, dens, abunde, abunds, rea, maxIpp));
                }

                return chemTable.ToArray();
            }
        }

        public static ChemType[] GetDefaultTable()
        {
            // ABUNDs source:
            // Thomas J. Ahrens (ed.), Global Earth Physics : A Handbook of Physical Constants, American Geophysical Union (1995). 
            // ISBN 0-87590-851-9 Composition of the Solar System, Planets, Meteorites, and Major Terrestrial Reservoirs, Horton E. Newsom. Tables 1, 14, 15.
            // https://en.wikipedia.org/wiki/Abundances_of_the_elements_(data_page)#Sun_and_solar_system (column Y2)

            //                An                     sym     HTML symbol                      name                    Aw    melt    boil   dens       ABUNDe       ABUNDs         Rea    Max inspired pp
            return new[]
            {
                new ChemType(GlobalConstants.AN_H,  "H",    "H<SUB><SMALL>2</SMALL></SUB>",  "Hydrogen",         1.0079,  14.06,  20.40,  8.99e-05,  0.00125893,  27925.4,       1,     0.0                        ),
                new ChemType(GlobalConstants.AN_HE, "He",   "He",                            "Helium",           4.0026,   3.46,   4.20,  0.0001787, 7.94328e-09, 2722.7,        0,     GlobalConstants.MAX_HE_IPP ),
                new ChemType(GlobalConstants.AN_N,  "N",    "N<SUB><SMALL>2</SMALL></SUB>",  "Nitrogen",        14.0067,  63.34,  77.40,  0.0012506, 1.99526e-05, 3.13329,       0,     GlobalConstants.MAX_N2_IPP ),
                new ChemType(GlobalConstants.AN_O,  "O",    "O<SUB><SMALL>2</SMALL></SUB>",  "Oxygen",          15.9994,  54.80,  90.20,  0.001429,  0.501187,    23.8232,       10,    GlobalConstants.MAX_O2_IPP ),
                new ChemType(GlobalConstants.AN_NE, "Ne",   "Ne",                            "Neon",            20.1700,  24.53,  27.10,  0.0009,    5.01187e-09, 3.4435e-5,     0,     GlobalConstants.MAX_NE_IPP ),
                new ChemType(GlobalConstants.AN_AR, "Ar",   "Ar",                            "Argon",           39.9480,  84.00,  87.30,  0.0017824, 3.16228e-06, 0.100925,      0,     GlobalConstants.MAX_AR_IPP ),
                new ChemType(GlobalConstants.AN_KR, "Kr",   "Kr",                            "Krypton",         83.8000, 116.60, 119.70,  0.003708,  1e-10,       4.4978e-05,    0,     GlobalConstants.MAX_KR_IPP ),
                new ChemType(GlobalConstants.AN_XE, "Xe",   "Xe",                            "Xenon",          131.3000, 161.30, 165.00,  0.00588,   3.16228e-11, 4.69894e-06,   0,     GlobalConstants.MAX_XE_IPP ),
                new ChemType(GlobalConstants.AN_NH3, "NH3", "NH<SUB><SMALL>3</SMALL></SUB>", "Ammonia",         17.0000, 195.46, 239.66,  0.001,     0.002,       0.0001,        1,     GlobalConstants.MAX_NH3_IPP),
                new ChemType(GlobalConstants.AN_H2O, "H2O", "H<SUB><SMALL>2</SMALL></SUB>O", "Water",           18.0000, 273.16, 373.16,  1.000,     0.03,        0.001,         0,     0.0                        ),
                new ChemType(GlobalConstants.AN_CO2, "CO2", "CO<SUB><SMALL>2</SMALL></SUB>", "CarbonDioxide",   44.0000, 194.66, 194.66,  0.001,     0.01,        0.0005,        0,     GlobalConstants.MAX_CO2_IPP),
                new ChemType(GlobalConstants.AN_O3,   "O3", "O<SUB><SMALL>3</SMALL></SUB>",  "Ozone",           48.0000,  80.16, 161.16,  0.001,     0.001,       0.000001,      2,     GlobalConstants.MAX_O3_IPP ),
                new ChemType(GlobalConstants.AN_CH4, "CH4", "CH<SUB><SMALL>4</SMALL></SUB>", "Methane",         16.0000,  90.16, 109.16,  0.010,     0.005,       0.0001,        1,     GlobalConstants.MAX_CH4_IPP),
            };
        }
    }
}
