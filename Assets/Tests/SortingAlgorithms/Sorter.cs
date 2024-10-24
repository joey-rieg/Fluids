using UnityEngine;
using UnityEngine.InputSystem;

namespace SortingAlgorithms
{
  public class Sorter : MonoBehaviour
  {
    [SerializeField]
    private SortingAlgorithm _algorithm;

    [SerializeField]
    private Numbers _numbers;

    void Update()
    {
      if (Keyboard.current.spaceKey.wasPressedThisFrame)
      {
        Debug.Log($"Starting {_algorithm.name}...");
        float start = Time.realtimeSinceStartup;

        _algorithm.Sort(_numbers.Values);

        float end = Time.realtimeSinceStartup;
        Debug.Log($"{_algorithm.name} done in {end - start} seconds for {_numbers.Values.Length} numbers.");
      }
    }    
  }
}
