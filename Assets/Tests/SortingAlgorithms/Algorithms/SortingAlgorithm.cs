using UnityEngine;

namespace SortingAlgorithms
{
  public abstract class SortingAlgorithm : ScriptableObject
  {
    public abstract void Sort(int[] numbers);
  }
}