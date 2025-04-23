using UnityEngine;

public static class BoundsHelper
{
    public static bool IsWithinBounds(Vector2Int tilePosition, Vector2Int lowerBounds, Vector2Int upperBounds) => 
    (
        tilePosition.x >= lowerBounds.x &&
        tilePosition.y >= lowerBounds.y && 
        tilePosition.x <= upperBounds.x && 
        tilePosition.y <= upperBounds.y
    );

    public static bool IsWithinBounds(Vector2Int tilePosition, int lowerBoundsX, int lowerBoundsY, int upperBoundsX, int upperBoundsY) => 
    (
        tilePosition.x >= lowerBoundsX && 
        tilePosition.y >= lowerBoundsY && 
        tilePosition.x <= upperBoundsX && 
        tilePosition.y <= upperBoundsY
    );

    public static bool IsWithinBounds(int x, int y, Vector2Int lowerBounds, Vector2Int upperBounds) => 
    (
        x >= lowerBounds.x && 
        y >= lowerBounds.y && 
        x <= upperBounds.x && 
        y <= upperBounds.y
    );

    public static bool IsWithinBounds(int x, int y, int lowerBoundsX, int lowerBoundsY, int upperBoundsX, int upperBoundsY) => 
    (
        x >= lowerBoundsX && 
        y >= lowerBoundsY && 
        x <= upperBoundsX && 
        y <= upperBoundsY
    );
}

