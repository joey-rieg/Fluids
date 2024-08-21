using UnityEngine;
using UnityEngine.InputSystem;

public class DensityTest : MonoBehaviour
{
  public ComputeShader _compute;

  public Shader _particleShader;

  public Mesh _particleMesh;

  public Material _kernelMat;

  public float _particleSpread = 1.0f;

  public int _particleCountPerDimension = 20;

  public float _particleScale = 1.0f;

  private int _particleCount;
  private ComputeBuffer _positions;
  private Vector2 _kernelCenter;
  private float _kernelRadius;

  private Vector2 _kernelRadiusOrigin;
  private bool _kernelSpawned = false;
  private GameObject _kernelVis;

  private int _kernelID;
  private int _positionsID;
  private int _colorsID;
  private int _kernelRadiusID, _kernelCenterID;
  private int _particleSpreadID;

  private RenderParams _renderParams;
  private GraphicsBuffer _commandBuffer;
  private GraphicsBuffer.IndirectDrawIndexedArgs[] _commandData;
  private Material _particleMat;

  void Start()
  {
    _kernelVis = GameObject.CreatePrimitive(PrimitiveType.Quad);
    _kernelVis.GetComponent<Renderer>().material = _kernelMat;
    _kernelVis.SetActive(false);
    _particleCount = _particleCountPerDimension * _particleCountPerDimension;
    _positions = new ComputeBuffer(_particleCount, sizeof(float) * 2);

    _kernelID = _compute.FindKernel("CSMain");
    _positionsID = Shader.PropertyToID("Positions");
    _colorsID = Shader.PropertyToID("Colors");
    _kernelRadiusID = Shader.PropertyToID("KernelRadius");
    _kernelCenterID = Shader.PropertyToID("KernelCenter");
    _particleSpreadID = Shader.PropertyToID("ParticleSpread");

    _compute.SetBuffer(_kernelID, _positionsID, _positions);

    _commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
    _commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];


    _particleMat = new Material(_particleShader);
    _renderParams = new RenderParams(_particleMat);
    _renderParams.worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000);
    _commandData[0].indexCountPerInstance = _particleMesh.GetIndexCount(0);
    _commandData[0].instanceCount = (uint)_particleCount;
    _commandBuffer.SetData(_commandData);

    _particleMat.SetBuffer(_positionsID, _positions);
  }

  void OnDestroy()
  {
    _positions.Release();
    _positions = null;
    _commandBuffer.Release();
    _commandBuffer = null;
    _commandData = null;
    Destroy(_particleMat);
  }

  void Update()
  {
    HandleInput();

    UpdateComputeSettings();

    uint threadGroupSizeX;
    _compute.GetKernelThreadGroupSizes(_kernelID, out threadGroupSizeX, out _, out _);

    int threadsX = (int)((_particleCount + (threadGroupSizeX -1)) / threadGroupSizeX);

    _compute.Dispatch(_kernelID, threadsX, 1, 1);
  }

  void LateUpdate()
  {
    Graphics.RenderMeshIndirect(_renderParams, _particleMesh, _commandBuffer);
  }

  void UpdateComputeSettings()
  {
    _compute.SetFloat(_kernelRadiusID, _kernelRadius);
    _compute.SetVector(_kernelCenterID, _kernelCenter);
    _compute.SetFloat(_particleSpreadID, _particleSpread);
    _compute.SetInt("ParticleCount", _particleCount);
    _compute.SetInt("ParticlePerDimension", _particleCountPerDimension);
  }

  void HandleInput()
  {
    if (Mouse.current.leftButton.wasPressedThisFrame && !_kernelSpawned)
    {
      Vector3 mousePos = Mouse.current.position.ReadValue();
      mousePos.z = -Camera.main.transform.position.z;
      _kernelCenter = Camera.main.ScreenToWorldPoint(mousePos);
      _kernelVis.SetActive(true);
      _kernelVis.transform.position = _kernelCenter;
      _kernelSpawned = true;
    }

    if (_kernelSpawned && Mouse.current.leftButton.wasPressedThisFrame)
    {
      Vector3 mousePos = Mouse.current.position.ReadValue();
      mousePos.z = -Camera.main.transform.position.z;
      _kernelRadiusOrigin = Camera.main.ScreenToWorldPoint(mousePos);
    }

    if (_kernelSpawned && Mouse.current.leftButton.isPressed)
    {
      Vector3 mousePos = Mouse.current.position.ReadValue();
      mousePos.z = -Camera.main.transform.position.z;
      Vector2 dragPoint = Camera.main.ScreenToWorldPoint(mousePos);
      _kernelRadius = (dragPoint - _kernelRadiusOrigin).magnitude;
      _kernelVis.transform.localScale = new Vector3(_kernelRadius, _kernelRadius, 1);
    }
  }
}
