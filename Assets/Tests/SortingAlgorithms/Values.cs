using UnityEngine;

[CreateAssetMenu(fileName = "Values", menuName = "Tests/SortingAlgorithm/Values")]
public class Values : ScriptableObject
{
  public int[] Numbers;

  public void Init(int numOfValues, int min = 0, int max = 100, bool random = false)
  {
    Numbers = new int[numOfValues];
    for (int i = 0; i < numOfValues; i++)
    {
      if (random)
      {
        Numbers[i] = Random.Range(min, max + 1); // + 1 because exclusive on ints
      }
      else
      {
        {
          Numbers[i] = i;
        }
      }
    }
  }
}
