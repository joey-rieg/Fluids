using UnityEngine;

public class SPHSpawner2DRectangle : SPHSpawner<Vector2>
{
  [SerializeField]
  private int _particlesPerDimensionY = 10;

  [SerializeField, Range(1, 100)]
  private int _sizeY;

  public override SpawnData<Vector2> GetSpawnData()
  {
    int particleCount = _particlesPerDimension * _particlesPerDimensionY;
    Vector2[] positions = new Vector2[particleCount];
    Vector2[] velocities = new Vector2[particleCount];

    Vector3 transformOffset = transform.position;
    int i = 0;
    for (int y = 0; y < _particlesPerDimensionY; y++)
    {
      for (int x = 0; x < _particlesPerDimension; x++)
      {
        float tx = x / (float) (_particlesPerDimension - 1);
        float ty = y / (float)(_particlesPerDimensionY - 1);

        if (_randomize)
        {
          float halfX = 0.5f * _size;
          float halfY = 0.5f * _sizeY;
          positions[i] = new Vector2(Random.Range(-halfX, halfX), Random.Range(-halfY, halfY))
            + new Vector2(transformOffset.x, transformOffset.y);
        }
        else
        {
          positions[i] = new Vector2((tx - 0.5f) * _size + transformOffset.x, (ty - 0.5f) * _sizeY + transformOffset.y);
        }

        velocities[i] = Vector2.zero;

        i++;
      }
    }

    return new SpawnData<Vector2> { Positions = positions, Velocities = velocities };
  }

  protected override void DrawBounds(float size)
  {
    DrawBoundsRectangular(size, _sizeY);
  }

  private void DrawBoundsRectangular(float sizeX, float sizeY)
  {
    Gizmos.DrawWireCube(Vector2.zero, new Vector2(sizeX, sizeY));
  }
}
