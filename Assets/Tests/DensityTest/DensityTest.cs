using TMPro;
using UnityEngine;

public class DensityTest : MonoBehaviour
{
  [Header("Shaders")]
  public ComputeShader ComputeSpawn;
  public ComputeShader ComputeDensity;
  public Shader ParticleShader;

  [Header("Kernel visualization")]
  public Mesh ParticleMesh;
  public Material KernelMat;

  [Header("Particle spawn and visualization properties")]
  public float ParticleSpread = 1.0f;
  public int ParticleCountPerDimension = 20;
  public float ParticleScale = 1.0f;

  [Header("Density simulation properties")]
  public Vector2 KernelCenter;
  public float KernelRadius;

  [Header("Text fields")]
  public TextMeshProUGUI ComputeDensityGUI;
  public TextMeshProUGUI CPUDensityGUI;

  // Helper fields
  private int _particleCount;
  private GameObject _kernelVis;

  // Compute buffer and data
  private ComputeBuffer _positions, _densityOutput;
  float[] _densityOutputData;

  // Shader IDs
  private int _kernelID;
  private int _positionsID;
  private int _kernelRadiusID, _kernelCenterID;
  private int _particlePerDimensionID, _particleCountID;
  private int _particleSpreadID, _particleScaleID;
  private int _threadsX;

  // Instanced rendering fields
  private RenderParams _renderParams;
  private GraphicsBuffer _commandBuffer;
  private GraphicsBuffer.IndirectDrawIndexedArgs[] _commandData;
  private Material _particleMat;

  void Start()
  {
    _particleCount = ParticleCountPerDimension * ParticleCountPerDimension;

    SetShaderIDs();

    InitComputeShaders();

    InitParticleRendering();
  }

  void OnDestroy()
  {
    ReleaseBuffers();
    Destroy(_particleMat);
  }

  void Update()
  {
    UpdateComputeSettings();

    ComputeSpawn.Dispatch(_kernelID, _threadsX, 1, 1);
    ComputeDensity.Dispatch(_kernelID, _threadsX, 1, 1);

    Vector2[] positions = new Vector2[_particleCount];
    _positions.GetData(positions);

    _densityOutput.GetData(_densityOutputData);

    ValidateDensity(ref positions, ref _densityOutputData);

    _kernelVis.transform.position = KernelCenter;
    _kernelVis.transform.localScale = new Vector3(2 * KernelRadius, 2 * KernelRadius, 1);
  }

  void LateUpdate()
  {
    Graphics.RenderMeshIndirect(_renderParams, ParticleMesh, _commandBuffer);
  }

  void UpdateComputeSettings()
  {
    _particleMat.SetFloat(_particleScaleID, ParticleScale);
    _particleMat.SetVector(_kernelCenterID, KernelCenter);
    _particleMat.SetFloat(_kernelRadiusID, KernelRadius);

    ComputeDensity.SetFloat(_kernelRadiusID, KernelRadius);
    ComputeDensity.SetVector(_kernelCenterID, KernelCenter);
    ComputeDensity.SetInt(_particleCountID, _particleCount);

    ComputeSpawn.SetFloat(_particleSpreadID, ParticleSpread);
    ComputeSpawn.SetInt(_particleCountID, _particleCount);
    ComputeSpawn.SetInt(_particlePerDimensionID, ParticleCountPerDimension);
  }

  void ValidateDensity(ref Vector2[] positions, ref float[] densityOutput)
  {
    float computeSum = 0;
    foreach (var val in densityOutput)
      computeSum += val;

    float realSum = 0;

    foreach (var pos in positions)
    {
      realSum += LinearDensity(pos, KernelCenter, KernelRadius);
    }

    float diff = Mathf.Abs(computeSum - realSum);

    ComputeDensityGUI.text = $"Compute: {computeSum:F2}     Radius:    {KernelRadius:F2}";
    CPUDensityGUI.text =     $"CPU    : {realSum:F2}     Difference: {diff:F2}";
  }

  float Density(Vector2 samplePoint, Vector2 center, float radius)
  {
    float distance = (samplePoint - center).magnitude;

    float volume = Mathf.PI * Mathf.Pow(radius, 8) / 4;

    float value = Mathf.Max(0, radius * radius - distance * distance);

    return value * value * value / volume;
  }
  float LinearDensity(Vector2 samplePoint, Vector2 center, float radius)
  {
    float distance = (samplePoint - center).magnitude;

    float volume = (Mathf.PI * radius * radius) / 3.0f;

    float influence = Mathf.Max(0, 1.0f - distance / radius);

    return influence / volume;
  }

    void SetShaderIDs()
  {
    _kernelID = ComputeSpawn.FindKernel("CSMain");
    _positionsID = Shader.PropertyToID("Positions");
    _kernelRadiusID = Shader.PropertyToID("KernelRadius");
    _kernelCenterID = Shader.PropertyToID("KernelCenter");
    _particleSpreadID = Shader.PropertyToID("ParticleSpread");
    _particleScaleID = Shader.PropertyToID("Scale");
    _particleCountID = Shader.PropertyToID("ParticleCount");
    _particlePerDimensionID = Shader.PropertyToID("ParticlePerDimension");
  }

  void InitComputeShaders()
  {
    uint threadGroupSizeX;
    ComputeSpawn.GetKernelThreadGroupSizes(_kernelID, out threadGroupSizeX, out _, out _);

    _threadsX = (int)((_particleCount + (threadGroupSizeX - 1)) / threadGroupSizeX);

    _positions = new ComputeBuffer(_particleCount, sizeof(float) * 2);
    _densityOutputData = new float[_threadsX];
    _densityOutput = new ComputeBuffer(_threadsX, sizeof(float));

    ComputeSpawn.SetBuffer(_kernelID, _positionsID, _positions);
    ComputeDensity.SetBuffer(_kernelID, _positionsID, _positions);
    ComputeDensity.SetBuffer(_kernelID, "DensityOutput", _densityOutput);
  }

  void InitParticleRendering()
  {
    _kernelVis = GameObject.CreatePrimitive(PrimitiveType.Quad);
    _kernelVis.GetComponent<Renderer>().material = KernelMat;

    _commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
    _commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];

    _particleMat = new Material(ParticleShader);
    _renderParams = new RenderParams(_particleMat);
    _renderParams.worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000);
    _commandData[0].indexCountPerInstance = ParticleMesh.GetIndexCount(0);
    _commandData[0].instanceCount = (uint)_particleCount;
    _commandBuffer.SetData(_commandData);

    _particleMat.SetBuffer(_positionsID, _positions);
  }

  void ReleaseBuffers()
  {
    _positions.Release();
    _positions = null;
    _densityOutput.Release();
    _densityOutput = null;
    _commandBuffer.Release();
    _commandBuffer = null;
    _commandData = null;
  }
}
