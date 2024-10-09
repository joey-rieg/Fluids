using UnityEngine;

public struct SpawnData<T>
{
  public T[] Positions;
  public T[] Velocities;
}

public abstract class SPHSpawner<T> : MonoBehaviour
{
  [SerializeField, Range(1, 100)]
  [Tooltip("Defines how many particles in each dimension are spawned")]
  protected int _particlesPerDimension;

  [SerializeField, Range(1, 100)]
  [Tooltip("Defines the margin between particles and the overall size")]
  protected float _size;

  [SerializeField]
  [Tooltip("Jitter added on top of spawn positions")]
  protected float _jitter = 0.001f;

  [SerializeField]
  [Tooltip("Toggles bounds visualization on/off")]
  protected bool _showBounds = true;

  [SerializeField]
  [Tooltip("Defines color of bounds visualization")]
  protected Color _boundsColor = Color.green;

  [SerializeField]
  [Tooltip("Randomizes spawn positions inside bounds.")]
  protected bool _randomize = false;

  public abstract SpawnData<T> GetSpawnData();
  protected abstract void DrawBounds(float size);

  void OnDrawGizmos()
  {
    if (_showBounds && !Application.isPlaying)
    {
      Matrix4x4 matrix = Gizmos.matrix;
      Gizmos.matrix = transform.localToWorldMatrix;
      Gizmos.color = _boundsColor;
      DrawBounds(_size);
      Gizmos.matrix = matrix;
    }
  }
}
