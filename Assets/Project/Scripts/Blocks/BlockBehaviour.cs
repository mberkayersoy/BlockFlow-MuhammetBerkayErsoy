using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class BlockBehaviour : MonoBehaviour
{
    private List<BaseBlockFeatureBehaviour> _featureBehaviours = new();
    [SerializeField] private GameObject _model;
    public GameObject Model => _model;

    public BlockData BlockData { get; private set; }
    private Rigidbody _rigidBody;

    // Movement blocking flags
    public bool BlockedPosX;
    public bool BlockedNegX;
    public bool BlockedPosZ;
    public bool BlockedNegZ;

    // Stores contact normals per this object's collider and the other collider
    private readonly Dictionary<Collider, Dictionary<Collider, Vector3>> _contactNormalsByCollider = new();

    void Awake()
    {
        if (!TryGetComponent(out _rigidBody))
            _rigidBody = gameObject.AddComponent<Rigidbody>();

        _rigidBody.useGravity = false;
        _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void InitializeBlockData(BlockData blockData, List<FeaturePrefabMapping> prefabList)
    {
        BlockData = blockData;

        foreach (var feature in blockData.BlockFeatures)
        {
            if (feature == null) continue;

            var mapping = prefabList.FirstOrDefault(m => m.FeatureTypeName == feature.GetType().Name);
            if (mapping?.Prefab == null)
            {
                Debug.LogWarning($"Feature {feature.GetType().Name} prefab not found!");
                continue;
            }

            var instance = Instantiate(mapping.Prefab, transform);
            instance.SetData(feature);
            _featureBehaviours.Add(instance);
            instance.Apply(this);
        }
    }

    public void AddFeature(BaseBlockFeatureData data, BaseBlockFeatureBehaviour prefab)
    {
        var featureInstance = Instantiate(prefab, transform);
        BlockData.AddFeature(data);
        featureInstance.SetData(data);
        _featureBehaviours.Add(featureInstance);
        featureInstance.Apply(this);
    }

    // Common method to update contact normals for both OnCollisionEnter and OnCollisionStay
    private void UpdateContactNormals(Collision collision)
    {
        var otherBlock = collision.collider.GetComponentInParent<BlockBehaviour>();
        if (otherBlock == this) return;

        // Temp dictionary to accumulate normals per collider pair
        var tempNormals = new Dictionary<(Collider thisCol, Collider otherCol), Vector3>();

        for (int i = 0; i < collision.contactCount; i++)
        {
            var contact = collision.GetContact(i);
            var thisCol = contact.thisCollider;
            var otherCol = contact.otherCollider;
            if (thisCol == null || otherCol == null) continue;

            var key = (thisCol, otherCol);
            if (!tempNormals.ContainsKey(key))
                tempNormals[key] = Vector3.zero;

            tempNormals[key] += contact.normal;
        }

        // Normalize and save averaged normals
        foreach (var kvp in tempNormals)
        {
            var thisCol = kvp.Key.thisCol;
            var otherCol = kvp.Key.otherCol;
            var avgNormal = kvp.Value.normalized;

            if (!_contactNormalsByCollider.TryGetValue(thisCol, out var dict))
            {
                dict = new Dictionary<Collider, Vector3>();
                _contactNormalsByCollider[thisCol] = dict;
            }

            dict[otherCol] = avgNormal;
        }

        UpdateBlockStates();
    }
    void OnDisable()
    {
        _contactNormalsByCollider.Clear();
        BlockedPosX = BlockedNegX = BlockedPosZ = BlockedNegZ = false;
    }

    public T GetFeature<T>() where T : BaseBlockFeatureBehaviour
    {
        return _featureBehaviours.OfType<T>().FirstOrDefault();
    }

    public bool HasFeature<T>() where T : BaseBlockFeatureBehaviour
    {
        return GetFeature<T>() != null;
    }
    void OnCollisionEnter(Collision collision) => UpdateContactNormals(collision);

    void OnCollisionStay(Collision collision) => UpdateContactNormals(collision);

    void OnCollisionExit(Collision collision)
    {
        var otherCollider = collision.collider;
        if (otherCollider == null) return;

        var keysToRemove = new List<Collider>();

        foreach (var (thisCol, otherDict) in _contactNormalsByCollider)
        {
            if (otherDict.Remove(otherCollider) && otherDict.Count == 0)
                keysToRemove.Add(thisCol);
        }

        foreach (var key in keysToRemove)
            _contactNormalsByCollider.Remove(key);

        UpdateBlockStates();
    }

    // Updates the movement block flags based on current contact normals
    private void UpdateBlockStates()
    {
        BlockedPosX = BlockedNegX = BlockedPosZ = BlockedNegZ = false;

        foreach (var otherDict in _contactNormalsByCollider.Values)
        {
            foreach (var normal in otherDict.Values)
            {
                if (normal.x > 0.5f) BlockedNegX = true;
                if (normal.x < -0.5f) BlockedPosX = true;
                if (normal.z > 0.5f) BlockedNegZ = true;
                if (normal.z < -0.5f) BlockedPosZ = true;
            }
        }
    }
}
