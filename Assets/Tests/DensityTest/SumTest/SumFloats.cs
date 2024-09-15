using UnityEngine;

public class SumFloats : MonoBehaviour
{
  public ComputeShader _shader;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    float[] values = new float[2050];
    float verificationSum = 0;
    for (int i = 0; i < values.Length; i++)
    {
      values[i] = i;
      verificationSum += values[i];
    }

    var kernelIdx = _shader.FindKernel("CSMain");
    uint threadGroupSizeX;
    _shader.GetKernelThreadGroupSizes(kernelIdx, out threadGroupSizeX, out _, out _);
    int threadsX = Mathf.CeilToInt(values.Length / (float)threadGroupSizeX);

    var inputBuffer = new ComputeBuffer(values.Length, sizeof(float));
    inputBuffer.SetData(values);
    float[] outputData = new float[threadsX];
    var outputBuffer = new ComputeBuffer(outputData.Length, sizeof(float));

    _shader.SetBuffer(kernelIdx, "Values", inputBuffer);
    _shader.SetBuffer(kernelIdx, "Output", outputBuffer);
    _shader.SetFloat("BufferLength", values.Length);

    _shader.Dispatch(kernelIdx, threadsX, 1, 1);

    outputBuffer.GetData(outputData);

    float sum = 0;
    foreach (var value in outputData)
      sum += value;

    Debug.Log("Sum is " + sum + " Actual sum is: " + verificationSum);

    inputBuffer.Release();
    inputBuffer = null;
    outputBuffer.Release();
    outputBuffer = null;        
  }
}
