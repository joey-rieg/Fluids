using UnityEngine;

public static class ComputeHelper
{
  public static void Dispatch(ComputeShader compute, int numIterationsX, int numIterationsY = 1, int numIterationsZ = 1, int kernelIndex = 0)
  {
    Vector3Int threadGroupSizes = GetThreadGroupSizes(compute, kernelIndex);
    int numGroupsX = Mathf.CeilToInt(numIterationsX / (float)threadGroupSizes.x);
    int numGroupsY = Mathf.CeilToInt(numIterationsY / (float)threadGroupSizes.y);
    int numGroupsZ = Mathf.CeilToInt(numIterationsZ / (float)threadGroupSizes.y);
    compute.Dispatch(kernelIndex, numGroupsX, numGroupsY, numGroupsZ);
  }

  public static void SetBuffer(ComputeShader compute, ComputeBuffer buffer, int bufferID, params int[] kernels)
  {
    for (int i = 0; i < kernels.Length; ++i)
    {
      compute.SetBuffer(kernels[i], bufferID, buffer);
    }
  }

  public static void SetBuffer(ComputeShader compute, ComputeBuffer buffer, string bufferName, params int[] kernels)
  {
    int bufferID = Shader.PropertyToID(bufferName);

    SetBuffer(compute, buffer, bufferID, kernels);
  }

  public static Vector3Int GetThreadGroupSizes(ComputeShader compute, int kernelIndex)
  {
    uint x, y, z;
    compute.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);

    return new Vector3Int((int)x, (int)y, (int)z);
  }
}
