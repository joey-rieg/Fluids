using UnityEngine;

public class SPHBoundary3D : SPHBoundary<Vector3>
{
  protected override void SetBoundsFromTransform(Transform source)
  {
    Center = source.position;
    Size = source.localScale;
  }

  protected override void DrawWireCube(Vector3 center, Vector3 size)
  {
    Gizmos.DrawWireCube(center, size);
  }
}
