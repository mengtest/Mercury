using System;

namespace Mercury
{
    [Serializable]
    public class EntryLocation
    {
        public readonly string entryType;
        public readonly AssetLocation id;
        private string _toString;

        public EntryLocation(string entryType, AssetLocation id)
        {
            this.entryType = entryType;
            this.id = id;
            _toString = $"[{entryType}:{id}]";
        }

        public override string ToString() { return _toString; }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public override bool Equals(object obj) { return obj is EntryLocation entry && entryType == entry.entryType && id.Equals(entry.id); }
    }
}