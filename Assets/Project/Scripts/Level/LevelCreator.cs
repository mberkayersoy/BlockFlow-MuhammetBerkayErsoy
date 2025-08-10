using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] private BlockBehaviour _blockPrefab;

    private LevelData _levelData;
    private GameObject _levelParent;
    private GameObject _blockParent;

    public void CreateLevel(LevelData level)
    {
        _levelData = level;

    }
}
