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

  public int ParticleCount { get; private set; }

  protected int _kernelIdx;
  protected uint _threadGroupSizeX;
  protected int _particleCount;

  private bool _isPaused = false;
  private bool _pauseNextFrame = false;

  void Start()
  {
    // Compute shader IDs
    _kernelIdx = _compute.FindKernel("CSMain");
    _compute.GetKernelThreadGroupSizes(_kernelIdx, out _threadGroupSizeX, out _, out _);
    int positionBufferID = Shader.PropertyToID("Positions");
    int velocityBufferID = Shader.PropertyToID("Velocities");
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

    _compute.SetBuffer(_kernelIdx, positionBufferID, Positions);
    _compute.SetBuffer(_kernelIdx, velocityBufferID, Velocities);

    _compute.SetInt(particleCountID, ParticleCount);

    // Renderer initialization
    _renderer.Init(this);
  }

  void OnDestroy()
  {
    Positions?.Release();
    Positions = null;
    Velocities?.Release();
    Velocities = null;
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
      int threadsX = (int)((ParticleCount + (_threadGroupSizeX - 1)) / _threadGroupSizeX);
      _compute.Dispatch(_kernelIdx, threadsX, 1, 1);
    }
  }

  private void SetComputeProperties(float timeStep)
  {
    SetComputeBoundary();

    _compute.SetFloat("TimeStep", timeStep);
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
