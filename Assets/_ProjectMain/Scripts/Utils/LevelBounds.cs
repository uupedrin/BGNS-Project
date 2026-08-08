using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelBounds : MonoSingleton<LevelBounds>
{
    private BoxCollider2D levelBoundsCollider;
    private LevelBoundsInfo? boundsInfo = null;

    protected override void AwakeBehaviour()
    {
        levelBoundsCollider = GetComponent<BoxCollider2D>();
    }

    public LevelBoundsInfo GetLevelBounds() => boundsInfo ??= _GetLevelBounds();

    private LevelBoundsInfo _GetLevelBounds()
    {
        LevelBoundsInfo info = new LevelBoundsInfo()
        {
            minX = float.MaxValue,
            maxX = float.MinValue,
            minY = float.MaxValue,
            maxY = float.MinValue
        };

        foreach (Vector2 edge in levelBoundsCollider.GetColliderEdges())
        {
            if (edge.x < info.minX) info.minX = edge.x;
            if (edge.x > info.maxX) info.maxX = edge.x;
            if (edge.y < info.minY) info.minY = edge.y;
            if (edge.y > info.maxY) info.maxY = edge.y;
        }

        return info;
    }
}

public struct LevelBoundsInfo
{
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
}
