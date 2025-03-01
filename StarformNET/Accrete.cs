namespace DLS.StarformNET
{
    using System;
    using Data;
    using System.Collections.Generic;

    public class Accrete
    {
        public double CloudEccentricity;
        public double GasDustRatio;

        private bool _dustLeft;
        private double _rInner;
        private double _rOuter;
        private double _reducedMass;
        private double _dustDensity;
        private double _cloudEccentricity;
        private DustRecord _dustHead;
        private PlanetSeed _planetHead;
        private Generation _histHead;

        public Accrete(double e, double gdr)
        {
            CloudEccentricity = e;
            GasDustRatio = gdr;
        }

        // TODO documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stellarMassRatio"></param>
        /// <param name="stellarLumRatio"></param>
        /// <param name="innerDust"></param>
        /// <param name="outerDust"></param>
        /// <param name="outerPlanetLimit"></param>
        /// <param name="dustDensityCoeff"></param>
        /// <param name="seedSystem"></param>
        /// <param name="doMoons"></param>
        /// <returns></returns>
        public List<PlanetSeed> GetPlanetaryBodies(double stellarMassRatio, 
            double stellarLumRatio, double innerDust, double outerDust,
            double outerPlanetLimit, double dustDensityCoeff, 
            PlanetSeed seedSystem, bool doMoons)
        {
            SetInitialConditions(innerDust, outerDust);

            var planet_inner_bound = NearestPlanet(stellarMassRatio);
            var planet_outer_bound = outerPlanetLimit == 0 
                ? FarthestPlanet(stellarMassRatio) 
                : outerPlanetLimit;

            var seeds = seedSystem;
            while (_dustLeft)
            {
                double a, e;
                if (seeds != null)
                {
                    a = seeds.SemiMajorAxisAU;
                    e = seeds.Eccentricity;
                    seeds = seeds.NextPlanet;
                }
                else
                {
                    a = Utilities.RandomNumber(planet_inner_bound, planet_outer_bound);
                    e = Utilities.RandomEccentricity();
                }

                var mass = GlobalConstants.PROTOPLANET_MASS;
                double dust_mass = 0;
                double gas_mass = 0;

                //Trace.TraceInformation("Checking {0:0.00} AU", a);

                if (DustAvailable(InnerEffectLimit(a, e, mass), OuterEffectLimit(a, e, mass)))
                {
                    //Trace.TraceInformation("Injecting protoplanet at {0:0.00} AU", a);

                    _dustDensity = dustDensityCoeff * Math.Sqrt(stellarMassRatio) * Math.Exp(-GlobalConstants.ALPHA * Math.Pow(a, (1.0 / GlobalConstants.N)));
                    var crit_mass = CriticalLimit(a, e, stellarLumRatio);
                    AccreteDust(ref mass, ref dust_mass, ref gas_mass, a, e, crit_mass, planet_inner_bound, planet_outer_bound);

                    dust_mass += GlobalConstants.PROTOPLANET_MASS;

                    if (mass > GlobalConstants.PROTOPLANET_MASS)
                    {
                        CoalescePlanetesimals(a, e, mass, crit_mass,
                                               dust_mass, gas_mass,
                                               stellarLumRatio,
                                               planet_inner_bound, planet_outer_bound,
                                               doMoons);
                    }

                    //Trace.TraceInformation(".. failed due to large neighbor.");
                }

                //Trace.TraceInformation(".. failed");
            }

            var seedList = new List<PlanetSeed>();
            var next = _planetHead;
            while (next != null)
            {
                seedList.Add(next);
                next = next.NextPlanet;
            }
            return seedList;
        }

        private void SetInitialConditions(double inner_limit_of_dust, double outer_limit_of_dust)
        {
            _histHead = new Generation();

            _planetHead = null;
            _dustHead = new DustRecord
            {
                NextBand = null,
                OuterEdge = outer_limit_of_dust,
                InnerEdge = inner_limit_of_dust,
                DustPresent = true,
                GasPresent = true
            };
            _dustLeft = true;
            _cloudEccentricity = CloudEccentricity;

            _histHead.Dusts = _dustHead;
            _histHead.Planets = _planetHead;
            _histHead.Next = _histHead;
        }

        private double NearestPlanet(double stell_mass_ratio)
        {
            return (0.3 * Math.Pow(stell_mass_ratio, (1.0 / 3.0)));
        }

        private double FarthestPlanet(double stell_mass_ratio)
        {
            return (50.0 * Math.Pow(stell_mass_ratio, (1.0 / 3.0)));
        }

        private double InnerEffectLimit(double a, double e, double mass)
        {
            return (a * (1.0 - e) * (1.0 - mass) / (1.0 + _cloudEccentricity));
        }

        private double OuterEffectLimit(double a, double e, double mass)
        {
            return (a * (1.0 + e) * (1.0 + mass) / (1.0 - _cloudEccentricity));
        }

        private bool DustAvailable(double inside_range, double outside_range)
        {
            DustRecord current_dust_band;
            bool dust_here;

            current_dust_band = _dustHead;
            while (current_dust_band != null && current_dust_band.OuterEdge < inside_range)
            {
                current_dust_band = current_dust_band.NextBand;
            }

            if (current_dust_band == null)
            {
                dust_here = false;
            }
            else
            {
                dust_here = current_dust_band.DustPresent;
                while (current_dust_band != null && current_dust_band.InnerEdge < outside_range)
                {
                    dust_here = dust_here || current_dust_band.DustPresent;
                    current_dust_band = current_dust_band.NextBand;
                }
            }

            return dust_here;
        }

        private void UpdateDustLanes(double min, double max, double mass, double crit_mass, double body_inner_bound, double body_outer_bound)
        {
            bool gas;
            DustRecord node1 = null;
            DustRecord node2 = null;
            DustRecord node3 = null;

            _dustLeft = false;
            if (mass > crit_mass)
            {
                gas = false;
            }
            else
            {
                gas = true;
            }

            node1 = _dustHead;
            while (node1 != null)
            {
                if (node1.InnerEdge < min && node1.OuterEdge > max)
                {
                    node2 = new DustRecord();
                    node2.InnerEdge = min;
                    node2.OuterEdge = max;
                    if ((node1.GasPresent == true))
                    {
                        node2.GasPresent = gas;
                    }
                    else
                    {
                        node2.GasPresent = false;
                    }
                    node2.DustPresent = false;
                    node3 = new DustRecord();
                    node3.InnerEdge = max;
                    node3.OuterEdge = node1.OuterEdge;
                    node3.GasPresent = node1.GasPresent;
                    node3.DustPresent = node1.DustPresent;
                    node3.NextBand = node1.NextBand;
                    node1.NextBand = node2;
                    node2.NextBand = node3;
                    node1.OuterEdge = min;
                    node1 = node3.NextBand;
                }
                else if (node1.InnerEdge < max && node1.OuterEdge > max)
                {
                    node2 = new DustRecord();
                    node2.NextBand = node1.NextBand;
                    node2.DustPresent = node1.DustPresent;
                    node2.GasPresent = node1.GasPresent;
                    node2.OuterEdge = node1.OuterEdge;
                    node2.InnerEdge = max;
                    node1.NextBand = node2;
                    node1.OuterEdge = max;
                    if ((node1.GasPresent == true))
                    {
                        node1.GasPresent = gas;
                    }
                    else
                    {
                        node1.GasPresent = false;
                    }
                    node1.DustPresent = false;
                    node1 = node2.NextBand;
                }
                else if (node1.InnerEdge < min && node1.OuterEdge > min)
                {
                    node2 = new DustRecord();
                    node2.NextBand = node1.NextBand;
                    node2.DustPresent = false;
                    if ((node1.GasPresent == true))
                    {
                        node2.GasPresent = gas;
                    }
                    else
                    {
                        node2.GasPresent = false;
                    }
                    node2.OuterEdge = node1.OuterEdge;
                    node2.InnerEdge = min;
                    node1.NextBand = node2;
                    node1.OuterEdge = min;
                    node1 = node2.NextBand;
                }
                else if (node1.InnerEdge >= min && node1.OuterEdge <= max)
                {
                    if (node1.GasPresent == true)
                    {
                        node1.GasPresent = gas;
                    }
                    node1.DustPresent = false;
                    node1 = node1.NextBand;
                }
                else if (node1.OuterEdge < min || node1.InnerEdge > max)
                {
                    node1 = node1.NextBand;
                }
            }

            // GL: This seems to be combining adjacent dust bands?
            node1 = _dustHead;
            while (node1 != null)
            {
                if (node1.DustPresent && (node1.OuterEdge >= body_inner_bound && node1.InnerEdge <= body_outer_bound))
                {
                    _dustLeft = true;
                }

                node2 = node1.NextBand;
                if (node2 != null)
                {
                    if (node1.DustPresent == node2.DustPresent && node1.GasPresent == node2.GasPresent)
                    {
                        node1.OuterEdge = node2.OuterEdge;
                        node1.NextBand = node2.NextBand;
                        node2 = null;
                    }
                }
                node1 = node1.NextBand;
            }
        }

        private double CollectDust(double last_mass, ref double new_dust, ref double new_gas, double a, double e, double crit_mass, ref DustRecord dust_band)
        {
            var temp = last_mass / (1.0 + last_mass);
            _reducedMass = Math.Pow(temp, (1.0 / 4.0));
            _rInner = InnerEffectLimit(a, e, _reducedMass);
            _rOuter = OuterEffectLimit(a, e, _reducedMass);

            if (_rInner < 0.0)
            {
                _rInner = 0.0;
            }

            if (dust_band == null)
            {
                return (0.0);
            }
            else
            {
                var gas_density = 0.0;
                double temp_density;
                double mass_density;
                if (dust_band.DustPresent == false)
                {
                    temp_density = 0.0;
                }
                else
                {
                    temp_density = _dustDensity;
                }

                if (last_mass < crit_mass || dust_band.GasPresent == false)
                {
                    mass_density = temp_density;
                }
                else
                {
                    mass_density = GasDustRatio * temp_density / (1.0 + Math.Sqrt(crit_mass / last_mass) * (GasDustRatio - 1.0));
                    gas_density = mass_density - temp_density;
                }

                if (dust_band.OuterEdge <= _rInner || dust_band.InnerEdge >= _rOuter)
                {
                    return (CollectDust(last_mass, ref new_dust, ref new_gas, a, e, crit_mass, ref dust_band.NextBand));
                }
                else
                {
                    var bandwidth = (_rOuter - _rInner);

                    var temp1 = _rOuter - dust_band.OuterEdge;
                    if (temp1 < 0.0)
                    {
                        temp1 = 0.0;
                    }
                    var width = bandwidth - temp1;

                    var temp2 = dust_band.InnerEdge - _rInner;
                    if (temp2 < 0.0)
                    {
                        temp2 = 0.0;
                    }
                    width = width - temp2;

                    temp = 4.0 * Math.PI * Math.Pow(a, 2.0) * _reducedMass * (1.0 - e * (temp1 - temp2) / bandwidth);
                    var volume = temp * width;

                    var new_mass = volume * mass_density;
                    new_gas = volume * gas_density;
                    new_dust = new_mass - new_gas;

                    double next_dust = 0;
                    double next_gas = 0;
                    var next_mass = CollectDust(last_mass, ref next_dust, ref next_gas, a, e, crit_mass, ref dust_band.NextBand);

                    new_gas = new_gas + next_gas;
                    new_dust = new_dust + next_dust;

                    return (new_mass + next_mass);
                }
            }
        }

        private double CriticalLimit(double orb_radius, double eccentricity, double stell_luminosity_ratio)
        {
            var perihelion_dist = (orb_radius - orb_radius * eccentricity);
            var temp = perihelion_dist * Math.Sqrt(stell_luminosity_ratio);
            return GlobalConstants.B * Math.Pow(temp, -0.75);
        }

        private void AccreteDust(ref double seed_mass, ref double new_dust, ref double new_gas, double a, double e, double crit_mass, double body_inner_bound, double body_outer_bound)
        {
            var new_mass = seed_mass;
            double temp_mass;

            do
            {
                temp_mass = new_mass;
                new_mass = CollectDust(new_mass, ref new_dust, ref new_gas, a, e, crit_mass, ref _dustHead);
            }
            while (!(new_mass - temp_mass < 0.0001 * temp_mass));

            seed_mass = seed_mass + new_mass;
            UpdateDustLanes(_rInner, _rOuter, seed_mass, crit_mass, body_inner_bound, body_outer_bound);
        }

        private bool DoMoons(PlanetSeed planet, double mass, double critMass, double dustMass, double gasMass)
        {
            var finished = false;
            var existingMass = 0.0;

            if (planet.FirstMoon != null)
            {
                for (var m = planet.FirstMoon; m != null; m = m.NextPlanet)
                {
                    existingMass += m.Mass;
                }
            }

            if (mass < critMass)
            {
                if (mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES < 2.5 && mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES > .0001 && existingMass < planet.Mass * .05)
                {
                    var moon = new PlanetSeed(0, 0, mass, dustMass, gasMass);

                    if (moon.DustMass + moon.GasMass > planet.DustMass + planet.GasMass)
                    {
                        var tempDust = planet.DustMass;
                        var tempGas = planet.GasMass;
                        var tempMass = planet.Mass;

                        planet.DustMass = moon.DustMass;
                        planet.GasMass = moon.GasMass;
                        planet.Mass = moon.Mass;

                        moon.DustMass = tempDust;
                        moon.GasMass = tempGas;
                        moon.Mass = tempMass;
                    }

                    if (planet.FirstMoon == null)
                    {
                        planet.FirstMoon = moon;
                    }
                    else
                    {
                        moon.NextPlanet = planet.FirstMoon;
                        planet.FirstMoon = moon;
                    }

                    finished = true;

                    //Trace.TraceInformation("Moon captured... {0:0.00} AU ({1:0.00}EM) <- {2:0.00} EM", 
                    //    the_planet.a, 
                    //    the_planet.mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
                    //    mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES);
                }
                //else
                //{
                //    Trace.TraceInformation("Moon escapes... {0:0.00} AU ({1:0.00} EM){2} {3:0.00}L {4}", 
                //        the_planet.a,
                //        the_planet.mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
                //        existing_mass < (the_planet.mass * .05) ? "" : " (big moons)",
                //        mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
                //        (mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) >= 2.5 ? ", too big" : (mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) <= .0001 ? ", too small" : "");
                //}
            }

            return finished;
        }

        private double GetNewEccentricity(PlanetSeed the_planet, double e, double a, double mass, double newA)
        {
            var newE = the_planet.Mass * Math.Sqrt(the_planet.SemiMajorAxisAU) * Math.Sqrt(1.0 - Math.Pow(the_planet.Eccentricity, 2.0));
            newE = newE + (mass * Math.Sqrt(a) * Math.Sqrt(Math.Sqrt(1.0 - Math.Pow(e, 2.0))));
            newE = newE / ((the_planet.Mass + mass) * Math.Sqrt(newA));
            newE = 1.0 - Math.Pow(newE, 2.0);
            if (newE < 0.0 || newE >= 1.0)
            {
                newE = 0.0;
            }
            return Math.Sqrt(newE);
        }

        private void CoalescePlanetesimals(double a, double e, double mass, double critMass, double dustMass, double gasMass, double stellLuminosityRatio, double bodyInnerBound, double bodyOuterBound, bool doMoons)
        {
            // First we try to find an existing planet with an over-lapping orbit.
            PlanetSeed thePlanet = null;
            PlanetSeed nextPlanet = null;
            PlanetSeed prevPlanet = null;
            var finished = false;
            for (thePlanet = _planetHead; thePlanet != null; thePlanet = thePlanet.NextPlanet)
            {
                var diff = thePlanet.SemiMajorAxisAU - a;
                double dist1;
                double dist2;

                if (diff > 0.0)
                {
                    dist1 = (a * (1.0 + e) * (1.0 + _reducedMass)) - a;
                    /* x aphelion	 */
                    _reducedMass = Math.Pow((thePlanet.Mass / (1.0 + thePlanet.Mass)), (1.0 / 4.0));
                    dist2 = thePlanet.SemiMajorAxisAU
                        - (thePlanet.SemiMajorAxisAU * (1.0 - thePlanet.Eccentricity) * (1.0 - _reducedMass));
                }
                else
                {
                    dist1 = a - (a * (1.0 - e) * (1.0 - _reducedMass));
                    /* x perihelion */
                    _reducedMass = Math.Pow((thePlanet.Mass / (1.0 + thePlanet.Mass)), (1.0 / 4.0));
                    dist2 = (thePlanet.SemiMajorAxisAU * (1.0 + thePlanet.Eccentricity) * (1.0 + _reducedMass))
                        - thePlanet.SemiMajorAxisAU;
                }

                // Did the planetesimal collide with this planet?
                if (Math.Abs(diff) <= Math.Abs(dist1) || Math.Abs(diff) <= Math.Abs(dist2))
                {
                    double new_dust = 0;
                    double new_gas = 0;
                    var new_a = (thePlanet.Mass + mass) / ((thePlanet.Mass / thePlanet.SemiMajorAxisAU) + (mass / a));
                    
                    e = GetNewEccentricity(thePlanet, e, a, mass, new_a);

                    if (doMoons)
                    {
                        finished = DoMoons(thePlanet, mass, critMass, dustMass, gasMass);
                    }

                    if (!finished)
                    {
                        //Trace.TraceInformation("Collision between two planetesimals!\n {0:0.00} AU ({1:0.00}EM) + {2:0.00} AU ({3:0.00}EM = {4:0.00}EMd + {5:0.00}EMg [{6:0.00}EM]). {7:0.00} AU ({8:0.00})",
                        //    the_planet.a, the_planet.mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
                        //    a, mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
                        //    dust_mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
                        //    gas_mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
                        //    crit_mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
                        //    new_a, e);

                        var newMass = thePlanet.Mass + mass;
                        AccreteDust(ref newMass, ref new_dust, ref new_gas,
                                     new_a, e, stellLuminosityRatio,
                                     bodyInnerBound, bodyOuterBound);

                        thePlanet.SemiMajorAxisAU = new_a;
                        thePlanet.Eccentricity = e;
                        thePlanet.Mass = newMass;
                        thePlanet.DustMass += dustMass + new_dust;
                        thePlanet.GasMass += gasMass + new_gas;
                        if (thePlanet.Mass >= critMass)
                        {
                            thePlanet.IsGasGiant = true;
                        }

                        while (thePlanet.NextPlanet != null && thePlanet.NextPlanet.SemiMajorAxisAU < new_a)
                        {
                            nextPlanet = thePlanet.NextPlanet;

                            if (thePlanet == _planetHead)
                            {
                                _planetHead = nextPlanet;
                            }
                            else
                            {
                                prevPlanet.NextPlanet = nextPlanet;
                            }

                            thePlanet.NextPlanet = nextPlanet.NextPlanet;
                            nextPlanet.NextPlanet = thePlanet;
                            prevPlanet = nextPlanet;
                        }
                    }

                    finished = true;
                    break;
                }
                else
                {
                    prevPlanet = thePlanet;
                }
            }

            // Planetesimals didn't collide. Make it a planet.
            if (!(finished))
            {
                thePlanet = new PlanetSeed(a, e, mass, dustMass, gasMass);

                if (mass >= critMass)
                {
                    thePlanet.IsGasGiant = true;
                }
                else
                {
                    thePlanet.IsGasGiant = false;
                }

                if (_planetHead == null)
                {
                    _planetHead = thePlanet;
                    thePlanet.NextPlanet = null;
                }
                else if (a < _planetHead.SemiMajorAxisAU)
                {
                    thePlanet.NextPlanet = _planetHead;
                    _planetHead = thePlanet;
                }
                else if (_planetHead.NextPlanet == null)
                {
                    _planetHead.NextPlanet = thePlanet;
                    thePlanet.NextPlanet = null;
                }
                else
                {
                    nextPlanet = _planetHead;
                    while (nextPlanet != null && nextPlanet.SemiMajorAxisAU < a)
                    {
                        prevPlanet = nextPlanet;
                        nextPlanet = nextPlanet.NextPlanet;
                    }
                    thePlanet.NextPlanet = nextPlanet;
                    prevPlanet.NextPlanet = thePlanet;
                }
            }
        }

    }
}