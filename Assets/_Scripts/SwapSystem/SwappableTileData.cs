namespace SwapSystem
{
    // TODO: posible use double dictionary
    [System.Serializable]
    public struct SwappableTileData
    {
        public PathGeneration.TileType Type;
        public SwapVariant Variant;

        public SwappableTileData(PathGeneration.TileType type, SwapVariant variant) => (Type, Variant) = (type, variant);

        public readonly override bool Equals(object obj)
        {
            if (obj is SwappableTileData other)
            {
                return Type == other.Type && Variant == other.Variant;
            }
            return false;
        }

        public readonly override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + Variant.GetHashCode();
                return hash;
            }
        }
    }
}