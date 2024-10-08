using UnityEngine;

public abstract class SPHRenderer<T> : MonoBehaviour
{
  [SerializeField]
  [Tooltip("Mesh to render as particle")]
  private Mesh _mesh;

  [SerializeField, Range(0.01f, 1f)]
  [Tooltip("Defines the scale of the rendered particle in cm")]
  protected float _particleScale;

  [SerializeField]
  [Tooltip("Material used to render the particles")]
  private Material _material;

  [SerializeField]
  [Tooltip("Maximum velocity value for gradient evaluation")]
  private float _maxVelocity = 5;

  [SerializeField]
  [Tooltip("Gradient used to color particles")]
  private Gradient _gradient;

  [SerializeField]
  [Tooltip("Defines how granular the gradient is sampled")]
  private int _gradientResolution = 64;

  // Buffers and render params
  private GraphicsBuffer _commandBuffer;
  private GraphicsBuffer.IndirectDrawIndexedArgs[] _commandData;
  private RenderParams _renderParams;

  // Material properties
  private int _localTransformID;
  private int _scaleID;
  private int _maxVelocityID;
  private int _gradientTextureID;

  // Gradient texture
  private Texture2D _gradientTexture;


  public void Init(SPHSystem<T> simulation)
  {
    // Init Buffers
    _commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
    _commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];

    // Fill command buffer
    _renderParams = new RenderParams(_material);
    _renderParams.worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000);
    _commandData[0].indexCountPerInstance = _mesh.GetIndexCount(0);
    _commandData[0].instanceCount = (uint)simulation.ParticleCount;
    _commandBuffer.SetData(_commandData);

    // Set shader property IDs
    _localTransformID = Shader.PropertyToID("LocalTransform");
    _scaleID = Shader.PropertyToID("Scale");
    _maxVelocityID = Shader.PropertyToID("MaxVelocity");
    _gradientTextureID = Shader.PropertyToID("GradientTexture");

    // Set buffer IDs
    int positionBufferID = Shader.PropertyToID("Positions");
    int velocityBufferID = Shader.PropertyToID("Velocities");

    _material.SetBuffer(positionBufferID, simulation.Positions);
    _material.SetBuffer(velocityBufferID, simulation.Velocities);

    GenerateTextureFromGradient();
    _material.SetTexture(_gradientTextureID, _gradientTexture);
  }

  void OnGUI()
  {
    if (!Application.isPlaying)
      return;

    GenerateTextureFromGradient();
    _material.SetTexture(_gradientTextureID, _gradientTexture);
  }

  void LateUpdate()
  {
    _material.SetMatrix(_localTransformID, transform.localToWorldMatrix);
    _material.SetFloat(_scaleID, _particleScale);
    _material.SetFloat(_maxVelocityID, _maxVelocity);

    Graphics.RenderMeshIndirect(_renderParams, _mesh, _commandBuffer, 1);
  }

  void OnDestroy()
  {
    DestroyTexture();
    _commandBuffer?.Release();
    _commandBuffer = null;
    _commandData = null;
  }

  // TODO: extract to helper class
  void GenerateTextureFromGradient()
  {
    // Destroy old texture
    DestroyTexture();

    _gradientTexture = new Texture2D(_gradientResolution, 1, TextureFormat.RGBAHalf, false);
    Color[] pixelData = new Color[_gradientResolution];
    for (int i = 0; i < _gradientResolution; i++)
    {
      pixelData[i] = _gradient.Evaluate((float)i / (_gradientResolution - 1));
    }

    _gradientTexture.SetPixels(pixelData);
    _gradientTexture.Apply();
  }

  void DestroyTexture()
  {
    if (_gradientTexture != null)
    {
      Destroy(_gradientTexture);
      _gradientTexture = null;
    }
  }
}
