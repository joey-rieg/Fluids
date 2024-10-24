using System.Collections.Generic;
using UnityEngine;

namespace SortingAlgorithms
{
  public class SortingBars : MonoBehaviour
  {
    [SerializeField]
    private Numbers _numbers;

    [SerializeField]
    private GameObject _barPrefab;

    private int _currentNumValues;

    private List<GameObject> _bars = new();

    void Start()
    {
      CreateBars();
    }

    void OnDestroy()
    {
      foreach (var bar in _bars)
      {
        Destroy(bar);
      }
    }

    void Update()
    {
      if (_currentNumValues != _numbers.Values.Length)
      {
        CreateBars();
      }

      UpdateBars();
    }

    void CreateBars()
    {
      _currentNumValues = _numbers.Values.Length;
      int diff = _currentNumValues - _bars.Count;

      if (diff > 0)
      {
        for (int i = _bars.Count; i < _currentNumValues; i++)
        {
          _bars.Add(Instantiate(_barPrefab, transform));

        }
      }
      else if (diff < 0)
      {
        for (int i = _bars.Count - 1; i >= _currentNumValues; i--)
        {
          Destroy(_bars[i]);
        }
        // _currentNumValues already updated and is equal to the index to remove now
        _bars.RemoveRange(_currentNumValues, Mathf.Abs(diff));
      }
    }

    void UpdateBars()
    {
      for (int i = 0; i < _currentNumValues; i++)
      {
        _bars[i].transform.localScale = new Vector3(1, _numbers.Values[i], 1);
        _bars[i].transform.position = new Vector3(i - ((float)_currentNumValues / 2 - 0.5f), 0, 0);
      }
    }
  }
}