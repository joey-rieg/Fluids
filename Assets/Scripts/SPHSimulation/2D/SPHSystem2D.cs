using UnityEngine;
using UnityEngine.InputSystem;

public class SPHSystem2D : SPHSystem<Vector2>
{
  [Header("Input interaction settings")]
  [SerializeField]
  [Tooltip("Determines how far particles are influenced from the interaction point")]
  private float _interactionRadius = 2;

  [SerializeField]
  [Tooltip("Determines how much particles are influenced")]
  private float _interactionStrength = 10f;

  [SerializeField]
  [Tooltip("Position of the obstacle")]
  private Vector2 _obstaclePosition;

  [SerializeField]
  [Tooltip("Extents of the obstacle")]
  private Vector2 _obstacleExtents;

  void OnDrawGizmos()
  {
    if (Application.isPlaying)
    {
      bool isPull = Mouse.current.leftButton.isPressed;
      bool isPush = Mouse.current.rightButton.isPressed;
      if (isPull || isPush)
      {
        Vector2 position = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);

        Gizmos.color = isPush ? Color.red : Color.green;
        Gizmos.DrawWireSphere(position, _interactionRadius);
      }
    }

    if (_obstacleExtents != Vector2.zero)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(_obstaclePosition, _obstacleExtents);
    }
  }

  protected override void InitBuffers()
  {
    Positions = new ComputeBuffer(ParticleCount, 2 * sizeof(float), ComputeBufferType.Structured);
    Velocities = new ComputeBuffer(ParticleCount, 2 * sizeof(float), ComputeBufferType.Structured);
    Densities = new ComputeBuffer(ParticleCount, 2 * sizeof(float), ComputeBufferType.Structured);
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

  protected override void SetComputeExternalForces()
  {
    Vector2 interactionCenter = Vector2.zero;
    float interactionStrength = 0;
    bool isPull = Mouse.current.leftButton.isPressed;
    bool isPush = Mouse.current.rightButton.isPressed;

    if (isPush || isPull)
    {
      Vector2 mousePos = Mouse.current.position.value;
      interactionStrength = isPush ? -_interactionStrength : _interactionStrength;
      interactionCenter = Camera.main.ScreenToWorldPoint(mousePos);
    }

    _compute.SetVector("InteractionCenter", interactionCenter);
    _compute.SetFloat("InteractionStrength", interactionStrength);
    _compute.SetFloat("InteractionRadius", _interactionRadius);
  }
}
