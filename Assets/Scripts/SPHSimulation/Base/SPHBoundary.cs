using UnityEngine;

public abstract class SPHBoundary<T> : MonoBehaviour
{
  [field: SerializeField]
  public T Center { get; protected set; }

  [field:SerializeField]
  public T Size { get; protected set; }

  [field: SerializeField]
  public Color WireColor { get; private set; }

  void OnDrawGizmos()
  {
    SetBoundsFromTransform(transform);
    Gizmos.color = WireColor;
    DrawWireCube(Center, Size);
  }

  protected abstract void SetBoundsFromTransform(Transform source);

  protected abstract void DrawWireCube(T center, T size);
}
