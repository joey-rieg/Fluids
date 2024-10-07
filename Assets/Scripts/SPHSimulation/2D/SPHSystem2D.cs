using UnityEngine;

public class SPHSystem2D : SPHSystem<Vector2>
{
  protected override void InitBuffers()
  {
    Positions = new ComputeBuffer(ParticleCount, 2 * sizeof(float), ComputeBufferType.Structured);
    Velocities = new ComputeBuffer(ParticleCount, 2 * sizeof (float), ComputeBufferType.Structured);
    Densities = new ComputeBuffer(ParticleCount, sizeof(float), ComputeBufferType.Structured);
    _predictedPositions = new ComputeBuffer(ParticleCount, 2 * sizeof(float), ComputeBufferType.Structured);
  }

  protected override void FillBuffers(ref SpawnData<Vector2> data)
  {
    Positions.SetData(data.Positions);
    Velocities.SetData(data.Velocities);
  }

  protected override void SetComputeBoundary()
  {
    _compute.SetVector("BoundaryCenter", Boundary.Center);
    _compute.SetVector("BoundarySize", Boundary.Size);
  }
}
