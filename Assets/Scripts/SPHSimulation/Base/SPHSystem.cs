using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SPHSystem<T> : MonoBehaviour
{
  [SerializeField]
  protected ComputeShader _compute;

  [SerializeField]
  protected SPHSpawner<T> _spawner;

  [SerializeField]
  protected SPHRenderer<T> _renderer;

  [field:SerializeField]
  public SPHBoundary<T> Boundary { get; set; }

  [SerializeField]
  private SPHSystemProperties _properties;

  [SerializeField]
  private bool _stepWiseSimulation = true;

  public ComputeBuffer Positions { get; protected set; }

  public ComputeBuffer Velocities { get; protected set; }

  public ComputeBuffer Densities { get; protected set; }

  public int ParticleCount { get; private set; }

  protected int _simulationKernelIdx;
  protected int _densityKernelIdx;

  private bool _isPaused = false;
  private bool _pauseNextFrame = false;

  void Start()
  {
    // Compute shader IDs
    _simulationKernelIdx = _compute.FindKernel("Simulate");
    _densityKernelIdx = _compute.FindKernel("CalculateDensity");

    int positionBufferID = Shader.PropertyToID("Positions");
    int velocityBufferID = Shader.PropertyToID("Velocities");
    int densityBufferID = Shader.PropertyToID("Densities");
    int particleCountID = Shader.PropertyToID("ParticleCount");

    // Time initalization
    float deltaTime = 1 / 60f;
    Time.fixedDeltaTime = deltaTime;

    // Spawn Data
    var spawnData = _spawner.GetSpawnData();
    ParticleCount = spawnData.Positions.Length;

    // Buffer initialization and fill
    InitBuffers();
    FillBuffers(ref spawnData);

    ComputeHelper.SetBuffer(_compute, Positions, positionBufferID, _simulationKernelIdx, _densityKernelIdx);
    ComputeHelper.SetBuffer(_compute, Velocities, velocityBufferID, _simulationKernelIdx);
    ComputeHelper.SetBuffer(_compute, Densities, densityBufferID, _simulationKernelIdx, _densityKernelIdx);

    _compute.SetInt(particleCountID, ParticleCount);

    // Renderer initialization
    _renderer.Init(this);
  }

  void OnDestroy()
  {
    ComputeHelper.Release(Positions, Velocities, Densities);
  }

  void Update()
  {
    if (!_stepWiseSimulation && Time.frameCount > 10)
    {
      RunSimulationStep(Time.deltaTime);      
    }

    if (_pauseNextFrame)
    {
      _isPaused = true;
      _pauseNextFrame = false; 
    }

    HandleInput();
  }

  void LateUpdate()
  {
    if (_stepWiseSimulation)
    {
      RunSimulationStep(Time.fixedDeltaTime);
    }
  }

  protected abstract void InitBuffers();

  protected abstract void FillBuffers(ref SpawnData<T> data);

  protected abstract void SetComputeBoundary();

  void RunSimulationStep(float timeStep)
  {
    float simulationStep = (timeStep / _properties.IterationsPerFrame) * _properties.TimeScale;
    SetComputeProperties(simulationStep);

    if (_isPaused)
      return;

    for (int i = 0; i < _properties.IterationsPerFrame; i++)
    {
      ComputeHelper.Dispatch(_compute, ParticleCount, 1, 1, _densityKernelIdx);
      ComputeHelper.Dispatch(_compute, ParticleCount, 1, 1, _simulationKernelIdx);
    }
  }

  private void SetComputeProperties(float timeStep)
  {
    SetComputeBoundary();

    _compute.SetFloat("TimeStep", timeStep);
    _compute.SetFloat("KernelRadius", _properties.KernelRadius);
    _compute.SetVector("Gravity", _properties.Gravity);
    _compute.SetFloat("BoundaryDampening", _properties.BoundaryDampening);
  }

  private void HandleInput()
  {
    if (Keyboard.current.spaceKey.wasPressedThisFrame)
    {
      _isPaused = !_isPaused;
    }

    if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
    {
      _isPaused = false;
      _pauseNextFrame = true;
    }
  }
}
