using UnityEngine;

public static class CodeExtensions
{
    public static Vector2[] GetColliderEdges(this BoxCollider2D boxCollider)
    {
        if (boxCollider == null) return new Vector2[0];

        Vector2 center = boxCollider.offset;
        Vector2 extents = boxCollider.size / 2f;

        Vector2 topLeft = new Vector2(center.x - extents.x, center.y + extents.y);
        Vector2 topRight = new Vector2(center.x + extents.x, center.y + extents.y);
        Vector2 bottomLeft = new Vector2(center.x - extents.x, center.y - extents.y);
        Vector2 bottomRight = new Vector2(center.x + extents.x, center.y - extents.y);

        return new Vector2[]
        {
            boxCollider.transform.TransformPoint(topLeft),
            boxCollider.transform.TransformPoint(topRight),
            boxCollider.transform.TransformPoint(bottomLeft),
            boxCollider.transform.TransformPoint(bottomRight)
        };
    }
}
