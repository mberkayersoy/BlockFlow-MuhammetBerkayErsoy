using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    private List<BaseBlockFeature> _features;
    [SerializeField] private GameObject _model;

    public GameObject Model { get => _model; private set => _model = value; }

    private void Awake()
    {
        _features = GetComponentsInChildren<BaseBlockFeature>().ToList();
    }

    public void AddFeature<T>(T feature) where T : BaseBlockFeature
    {
        _features.Add(feature);
        Instantiate(feature, transform);
        // To do: add object pool pattern to reuse features.
    }

    public T GetFeature<T>() where T : BaseBlockFeature
    {
        return _features.OfType<T>().FirstOrDefault();
    }

    public bool HasFeature<T>() where T : BaseBlockFeature
    {
        return GetFeature<T>() != null;
    }
}
