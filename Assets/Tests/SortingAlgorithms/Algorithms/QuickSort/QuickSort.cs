using UnityEngine;

namespace SortingAlgorithms
{
  [CreateAssetMenu(fileName = "QuickSort", menuName = "SortingAlgorithms/Quicksort")]
  public class QuickSort : SortingAlgorithm
  {
    public override void Sort(int[] numbers)
    {
      // define pivot element as last element
      // partition such that left side is smaller than pivot
      // and right side is bigger
      // recurse until sorted
      Sort(numbers, 0, numbers.Length - 1);
    }

    private void Sort(int[] array, int start, int end)
    {
      if (start >= end)
        return;

      int pivot = Partition(array, start, end);
      Sort(array, start, pivot - 1);
      Sort(array, pivot + 1, end);
    }

    private int Partition(int[] array, int start, int end)
    {
      Debug.Log($"Partition with {start} and {end}");
      int pivot = array[end];
      
      int i = start - 1;
      int j = start;

      while (j < end)
      {
        if (array[j] < pivot)
        {
          ++i;
          int temp = array[i];
          array[i] = array[j];
          array[j] = temp;
        }

        ++j;
      }

      int t = array[++i];
      array[i] = array[end];
      array[end] = t;

      return i;
    }
  }
}
