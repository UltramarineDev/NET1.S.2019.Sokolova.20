using System;

namespace IOStreams
{
    public sealed class PlanetInfo : IEquatable<PlanetInfo>
    {
        public string Name { get; set; }
        public double MeanRadius { get; set; }

        public override string ToString() => $"{Name} {MeanRadius}";

        public bool Equals(PlanetInfo other)
            => other != null && Name.Equals(other.Name) && MeanRadius.Equals(other.MeanRadius);

    }
}