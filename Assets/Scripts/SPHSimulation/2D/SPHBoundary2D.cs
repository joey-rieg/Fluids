using UnityEngine;

public class SPHBoundary2D : SPHBoundary<Vector2>
{
  protected override void SetBoundsFromTransform(Transform source)
  {
    Center = source.position;
    Size = source.localScale;
  }

  protected override void DrawWireCube(Vector2 center, Vector2 size)
  {
    Gizmos.DrawWireCube(center, size);
  }
}
