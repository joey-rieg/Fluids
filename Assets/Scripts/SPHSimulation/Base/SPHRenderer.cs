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

  private GraphicsBuffer _commandBuffer;
  private GraphicsBuffer.IndirectDrawIndexedArgs[] _commandData;
  private RenderParams _renderParams;

  private int _localTransformID;
  private int _scaleID;
  private SPHBoundary<T> _boundary;

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

    _localTransformID = Shader.PropertyToID("LocalTransform");
    _scaleID = Shader.PropertyToID("Scale");
    int positionBufferID = Shader.PropertyToID("Positions");
    _material.SetBuffer(positionBufferID, simulation.Positions);

    _boundary = simulation.Boundary;
  }

  void LateUpdate()
  {
    _material.SetMatrix(_localTransformID, transform.localToWorldMatrix);
    _material.SetFloat(_scaleID, _particleScale);

    Graphics.RenderMeshIndirect(_renderParams, _mesh, _commandBuffer, 1);
  }

  void OnDestroy()
  {
    _commandBuffer?.Release();
    _commandBuffer = null;
    _commandData = null;
  }
}
