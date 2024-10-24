using System.Collections.Generic;
using UnityEngine;

public class RadixSort : SortingAlgorithm
{
  public List<int>[] Buckets { get; private set; }
  public int MaxValue { get; private set; }
  public int NumDigits { get; private set; }
  public int CurrentBitPosition { get; private set; }

  protected override bool IsSorted { get; set; }
  protected override bool IsSorting { get; set; }

  protected override int[] Sort()
  {
    int[] numbers = _values.Numbers;

    // Hardcoded for now for performance reasons in base 2 scenarios
    // Division for other bases is costly, but will be implemented to allow other bases.
    // Idea: use bit shift if radix == 2, otherwise division
    int radix = 2;

    Buckets = new List<int>[radix];
    for (int i = 0; i < radix; i++)
    {
      Buckets[i] = new List<int>();
    }

    MaxValue = GetMax();
    NumDigits = Mathf.FloorToInt(Mathf.Log(MaxValue, radix)) + 1;

    for (int i = 0; i < NumDigits; i++)
    {
      for (int j = 0; j < _numValues; j++)
      {
        int num = numbers[j];

        // i == bit shift (ith digit is wanted)
        int bit = (num >> i) & 1;
        Buckets[bit].Add(num);
      }

      int arrayIdx = 0;
      for (int k = 0; k < radix; k++)
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
