using UnityEngine;

public class SPHSpawner2D : SPHSpawner<Vector2>
{
  public override SpawnData<Vector2> GetSpawnData()
  {
    int particleCount= _particlesPerDimension * _particlesPerDimension;
    Vector2[] positions = new Vector2[particleCount];
    Vector2[] velocities = new Vector2[particleCount];

    Vector3 transformOffset = transform.position;
    Debug.Log("Offset is: " + transformOffset);

    int i = 0;
    for (int y = 0; y < _particlesPerDimension; y++)
    {
      for (int x = 0; x < _particlesPerDimension; x++)
      {
        float tx = x / (float)(_particlesPerDimension - 1);
        float ty = y / (float)(_particlesPerDimension - 1);

        positions[i] = new Vector2((tx - 0.5f) * _size + transformOffset.x, (ty - 0.5f) * _size + transformOffset.y);
        if (i < 3)
          Debug.Log("Position for " + i + " is " + positions[i]);
        velocities[i] = Vector2.zero;

        i++;
      }
    }

    return new SpawnData<Vector2> { Positions = positions, Velocities = velocities };
  }

  protected override void DrawBounds(float size)
  {
    Gizmos.DrawWireCube(Vector2.zero, Vector2.one * size);
  }
}