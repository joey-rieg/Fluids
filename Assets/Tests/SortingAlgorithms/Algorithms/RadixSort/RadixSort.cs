using System.Collections.Generic;
using UnityEngine;

namespace SortingAlgorithms
{
  [CreateAssetMenu(fileName = "RadixSort", menuName = "SortingAlgorithms/RadixSort")]
  public class RadixSort : SortingAlgorithm
  {
    public override void Sort(int[] numbers)
    {
      if (numbers.Length <= 1)
        return;

      // Hardcoded for now for performance reasons in base 2 scenarios
      // Division for other bases is costly, but will be implemented to allow other bases.
      // Idea: use bit shift if radix == 2, otherwise division
      int radix = 2;

      int numValues = numbers.Length;

      var Buckets = new List<int>[radix];
      for (int i = 0; i < radix; i++)
      {
        Buckets[i] = new List<int>();
      }

      var MaxValue = GetMax(numbers);
      var NumDigits = Mathf.FloorToInt(Mathf.Log(MaxValue, radix)) + 1;

      for (int i = 0; i < NumDigits; i++)
      {
        for (int j = 0; j < numValues; j++)
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
    }

    private int GetMax(int[] numbers)
    {
      int max = 0;
      int numValues = numbers.Length;
      for (int i = 0; i < numValues; i++)
      {
        if (max < numbers[i])
          max = numbers[i];
      }

      return max;
    }
  }
}
