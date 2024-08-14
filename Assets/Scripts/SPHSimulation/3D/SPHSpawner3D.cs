using UnityEngine;

public class SPHSpawner3D : SPHSpawner<Vector3>
{
  public override SpawnData<Vector3> GetSpawnData()
  {
    int particleCount = _particlesPerDimension * _particlesPerDimension * _particlesPerDimension;
    Vector3[] positions = new Vector3[particleCount];
    Vector3[] velocities = new Vector3[particleCount];


    Vector3 transformOffset = transform.position;
    Debug.Log("Offset is: " + transformOffset);


    int i = 0;
    for (int z = 0; z < _particlesPerDimension; z++)
    {
      for (int y = 0; y < _particlesPerDimension; y++)
      {
        for (int x = 0; x < _particlesPerDimension; x++)
        {
          float tx = x / (float)(_particlesPerDimension - 1);
          float ty = y / (float)(_particlesPerDimension - 1);
          float tz = z / (float)(_particlesPerDimension - 1);

          positions[i] = new Vector3((tx - 0.5f) * _size + transformOffset.x, (ty - 0.5f) * _size + transformOffset.y, (tz - 0.5f) * _size + transformOffset.z);
          velocities[i] = Vector3.zero;

          i++;
        }
      }
    }

    return new SpawnData<Vector3> { Positions = positions, Velocities = velocities };
  }

  protected override void DrawBounds(float size)
  {
    Gizmos.DrawWireCube(Vector3.zero, Vector3.one * size);
  }
}
