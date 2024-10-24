using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SortingAlgorithm : MonoBehaviour
{
  [SerializeField]
  protected int _numValues = 100;

  [SerializeField]
  protected int _minValue = 0;

  [SerializeField]
  protected int _maxValue = 100;

  [SerializeField]
  protected bool _randomizeValues = false;

  [SerializeField]
  protected Values _values;

  protected abstract bool IsSorted { get; set; }

  protected abstract bool IsSorting { get; set; }

  void Awake()
  {
    _values.Init(_numValues, _minValue, _maxValue, _randomizeValues);
  }

  void Update()
  {
    if (Keyboard.current.spaceKey.wasPressedThisFrame && !IsSorting)
    {
      Debug.Log("Start sorting...");
      float startTime = Time.realtimeSinceStartup;
      Sort();
      float endTime = Time.realtimeSinceStartup;
      Debug.Log($"Sorting finished in {(endTime - startTime).ToString($"F{5}")}");
    }
  }

  protected abstract int[] Sort();
}
