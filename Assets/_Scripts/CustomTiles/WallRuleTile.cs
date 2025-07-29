using UnityEngine;
using UnityEngine.Tilemaps;

namespace CustomTiles
{
    [CreateAssetMenu(menuName = "CustomTiles/WallRuleTile")]
    public class WallRuleTile : RuleTile
    {
        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            return neighbor switch
            {
                1 => tile is WallRuleTile || tile is DoorRuleTile,
                2 => tile is not WallRuleTile && tile is not DoorRuleTile,
                _ => true,
            };
        }
    }
}

