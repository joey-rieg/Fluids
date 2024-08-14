using System.Linq;
using UnityEngine;

public abstract class SPHSystem<T> : MonoBehaviour
{
  [SerializeField]
  protected ComputeShader _compute;

  [SerializeField]
  protected SPHSpawner<T> _spawner;

  [SerializeField]
  protected SPHRenderer<T> _renderer;

  public ComputeBuffer Positions { get; protected set; }
  public ComputeBuffer Velocities { get; protected set; }

  public int ParticleCount { get; private set; }

  protected int _kernelIdx;
  protected uint _threadGroupSizeX;
  protected int _particleCount;

  void Start()
  {
    // Compute shader IDs
    _kernelIdx = _compute.FindKernel("CSMain");
    _compute.GetKernelThreadGroupSizes(_kernelIdx, out _threadGroupSizeX, out _, out _);
    int positionBufferID = Shader.PropertyToID("Positions");
    int velocityBufferID = Shader.PropertyToID("Velocities");
    int particleCountID = Shader.PropertyToID("ParticleCount");

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
    InitRenderer();
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
    int threadsX = (int)((ParticleCount + (_threadGroupSizeX - 1)) / _threadGroupSizeX);
    _compute.Dispatch(_kernelIdx, threadsX, 1, 1);

  }
  protected abstract void InitBuffers();

  protected abstract void InitRenderer();

  protected abstract void FillBuffers(ref SpawnData<T> data);
}
