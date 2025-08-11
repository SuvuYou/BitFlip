using UnityEngine;

namespace SwapSystem
{   
    public class SwappableRuleTile : ISwappable
    {
        public void Swap(SwapVariant variant) => _currentVariant = variant;
        public bool IsCurrentVariantEqualTo(SwapVariant variant) => _currentVariant == variant;

        private SwappableTileDataSO _swappableTileDataSO;

        private PathGeneration.TileType _currentTileType;
        private SwapVariant _currentVariant;

        public Color SavedColor { get; private set; } = Color.white;

        public bool IsCollidable => _swappableTileDataSO.CollidablesDictionary[_currentTileType];

        public readonly int X, Y;

        public SwappableRuleTile(int x, int y, SwappableTileDataSO swappableTileDataSO) 
        {
            X = x; Y = y;

            _swappableTileDataSO = swappableTileDataSO;

            (this as ISwappable).Register(staticPositionY: Y);
        } 

        public void SetColor(Color color) => SavedColor = color;

        public void SetTileType(PathGeneration.TileType type) => _currentTileType = type;

        public RuleTile GetActiveVariant() => _swappableTileDataSO.VariantsDictionary[new SwappableTileData(_currentTileType, _currentVariant)];
    }
}