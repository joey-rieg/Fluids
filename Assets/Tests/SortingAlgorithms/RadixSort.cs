using System.Collections.Generic;
using UnityEngine;

public class RadixSort : SortingAlgorithm
{
  [Header("Algorithm Properties")]
  [field:SerializeField]
  public int Base { get; private set; }

  public List<int>[] Buckets { get; private set; }
  public int MaxValue { get; private set; }
  public int NumDigits { get; private set; }
  public int CurrentBitPosition { get; private set; }

  protected override bool IsSorted { get; set; }
  protected override bool IsSorting { get; set; }

  protected override int[] Sort()
  {
    int[] numbers = _values.Numbers;

    Buckets = new List<int>[Base];
    for (int i = 0; i < Base; i++)
    {
      Buckets[i] = new List<int>();
    }

    MaxValue = GetMax();
    NumDigits = Mathf.FloorToInt(Mathf.Log(MaxValue, Base)) + 1;
    CurrentBitPosition = 0;

    // Loop over number of digits
    for (int i = 0; i < NumDigits; i++)
    {
      for (int j = 0; j < _numValues; j++ )
      {
        int binary = numbers[j] & (int)Mathf.Pow(Base, CurrentBitPosition);

        int bit = binary >> CurrentBitPosition;
        Buckets[bit].Add(numbers[j]);
      }
      
      CurrentBitPosition++;

      int arrayIdx = 0;
      for (int k = 0; k < Base; k++)
      {
        for (int l = 0; l < Buckets[k].Count; l++)
        {
          numbers[arrayIdx++] = Buckets[k][l];
        }
        Buckets[k].Clear();
      }
    }

    return numbers;
  }

  protected override int[] SortNextStep()
  {
    if (!IsSorting)
      IsSorting = true;

    // TODO: implement
    return _values.Numbers;
  }

  private int GetMax()
  {
    int max = 0;
    for (int i = 0; i < _numValues; i++)
    {
      if (max < _values.Numbers[i])
        max = _values.Numbers[i];
    }

    return max;
  }
}
