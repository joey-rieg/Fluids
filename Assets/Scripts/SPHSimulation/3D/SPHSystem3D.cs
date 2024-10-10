using UnityEngine;

public class SPHSystem3D : SPHSystem<Vector3>
{
  protected override void InitBuffers()
  {
    Positions = new ComputeBuffer(ParticleCount, 3 * sizeof(float), ComputeBufferType.Structured);
    Velocities = new ComputeBuffer(ParticleCount, 3 * sizeof(float), ComputeBufferType.Structured);
  }

  protected override void FillBuffers(ref SpawnData<Vector3> data)
  {
    Positions.SetData(data.Positions);
    Velocities.SetData(data.Velocities);
  }

  protected override void SetComputeBoundary()
  {
    _compute.SetVector("BoundaryCenter", Boundary.Center);
    _compute.SetVector("BoundarySize", Boundary.Size);
  }

  protected override void SetComputeExternalForces()
  {
  }
}
