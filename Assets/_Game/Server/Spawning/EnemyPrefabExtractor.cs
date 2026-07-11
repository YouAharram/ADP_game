using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPrefabExtractor
{
    private List<GameObject> prefabs;

    public List<GameObject> Prefabs { get => prefabs; set => prefabs = value; }

    public abstract GameObject ExtractEnemyPrefab();
}