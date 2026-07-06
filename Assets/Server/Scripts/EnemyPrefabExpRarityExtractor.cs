using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPrefabExpRarityExtractor : EnemyPrefabExtractor
{
    public override GameObject ExtractEnemyPrefab()
    {
        List<GameObject> prefabsWithExtractedRarity = new List<GameObject>();
        
        do
        {
            float extractedRarity = RollRarityValue();

            prefabsWithExtractedRarity = Prefabs.Where(
                prefab => prefab.GetComponent<EnemyPrefabBaseStats>().RarityIndex == extractedRarity)
                .ToList();
            
        } while (prefabsWithExtractedRarity.Count == 0);

        int indexMobExtracted = UnityEngine.Random.Range(0, prefabsWithExtractedRarity.Count);

        return prefabsWithExtractedRarity[indexMobExtracted];
    }

    private int RollRarityValue()
    {
        int maxRarity = Prefabs.Max(prefab => prefab.GetComponent<EnemyPrefabBaseStats>().RarityIndex);
        int extractedRarity = 0;

        float lambda = MathF.Log(2.0f);
        
        do
        {
            float u = UnityEngine.Random.value;
            
            if (u >= 1.0f)
                u = 0.999999f; 
            else if (u <= 0.0f)
                u = 0.000001f;

            float x = -(float)(Math.Log(1.0f - u) / lambda);
            extractedRarity = Mathf.CeilToInt(x);

        } while (extractedRarity > maxRarity);

        return extractedRarity;
    }



}