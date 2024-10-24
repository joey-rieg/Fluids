using UnityEngine;

namespace SortingAlgorithms
{
  [CreateAssetMenu(fileName = "Numbers", menuName = "SortingAlgorithms/Data/Numbers")]
  public class Numbers : ScriptableObject
  {
    public int NumValues = 100;
    public int MinValue = 0;
    public int MaxValue = 100;
    public bool Randomize = false;

    [Header("Values can also be manually set.")]
    public int[] Values;

    public void Init(int numOfValues, int min = 0, int max = 100, bool random = false)
    {
      Values = new int[numOfValues];
      for (int i = 0; i < numOfValues; i++)
      {
        if (random)
        {
          Values[i] = Random.Range(min, max + 1); // + 1 because exclusive on ints
        }
        else
        {
          {
            Values[i] = i;
          }
        }
      }
    }
  }
}
