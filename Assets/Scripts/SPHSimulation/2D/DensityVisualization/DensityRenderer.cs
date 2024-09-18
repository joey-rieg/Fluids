using UnityEngine;

[RequireComponent (typeof(Camera))]
public class DensityRenderer : MonoBehaviour
{
  [SerializeField]
  private ComputeShader _shader;

  [SerializeField]
  private Material _densityDisplayMat;

  [SerializeField]
  private SPHSystem2D _simulation;

  [SerializeField]
  private SPHSystemProperties _properties;

  [SerializeField]
  private bool _isRendering;

  [SerializeField]
  private bool _renderGradient = false;

  private int _kernelIdx;
  private int _positionsID;
  private int _kernelRadiusID;
  private uint _threadGroupSizeX, _threadGroupSizeY;

  private Camera _camera;
  private RenderTexture _texture;
  private GameObject _densityDisplay;
  bool _isInitialized = false;


  void Awake()
  {
    _camera = GetComponent<Camera>();
    if (!_camera.orthographic)
      Debug.LogWarning("Camera is not in orthographic mode! Density display is wrongly calculated");

  }

  void OnDestroy()
  {
    _texture?.Release();
    _texture = null;
    GameObject.Destroy(_densityDisplay);
  }

  void Update()
  {
    if (!_isRendering)
      return;

    if (!_isInitialized)
      Init();

    UpdateProperties();

    int numGroupsX = (int)((_camera.pixelWidth + _threadGroupSizeX - 1) / _threadGroupSizeX);
    int numGroupsY = (int)((_camera.pixelHeight + _threadGroupSizeY - 1) / _threadGroupSizeY);

    _shader.Dispatch(_kernelIdx, numGroupsX, numGroupsY, 1);    
  }

  private void Init()
  {
    InitDensityDisplay();
    CreateRenderTexture();

    SetShaderIDs();

    _shader.SetBuffer(_kernelIdx, _positionsID, _simulation.Positions);
    _shader.SetTexture(_kernelIdx, "RenderTexture", _texture);

    _shader.GetKernelThreadGroupSizes(_kernelIdx, out _threadGroupSizeX, out _threadGroupSizeY, out _);
  }

  private void InitDensityDisplay()
  {
    // Create quad and assign material
    _densityDisplay = GameObject.CreatePrimitive(PrimitiveType.Quad);
    _densityDisplay.transform.parent = _camera.transform;
    _densityDisplay.GetComponent<Renderer>().material = _densityDisplayMat;
    _densityDisplay.transform.localPosition = new Vector3(0, 0, 5);

    // Place and scale quad such that whole FOV is covered
    float _displayScaleX = 2 * _camera.orthographicSize * _camera.aspect;
    float _displayScaleY = 2 * _camera.orthographicSize;
    _densityDisplay.transform.localScale = new Vector3(_displayScaleX, _displayScaleY, 1);

    _isInitialized = true;
  }

  private void CreateRenderTexture()
  {
    _texture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0);
    _texture.filterMode = FilterMode.Bilinear;
    _texture.enableRandomWrite = true;
    _texture.Create();
    _densityDisplayMat.SetTexture("_MainTexture", _texture);
  }

  private void SetShaderIDs()
  {
    _kernelIdx = _shader.FindKernel("CSMain");
    _positionsID = Shader.PropertyToID("Positions");
    _kernelRadiusID = Shader.PropertyToID("KernelRadius");
  }

  private void UpdateProperties()
  {
    _shader.SetFloat(_kernelRadiusID, _properties.KernelRadius);
    _shader.SetInt("ParticleCount", _simulation.ParticleCount);
    _shader.SetVector("Resolution", new Vector4(GetComponent<Camera>().pixelWidth, _camera.pixelHeight, 0 ,0));
    _shader.SetFloat("OrthographicSize", _camera.orthographicSize);
    _shader.SetFloat("AspectRatio", _camera.aspect);
    _shader.SetFloat("TargetDensity", _properties.TargetDensity);
    _shader.SetBool("RenderGradient", _renderGradient);
  }
}
