using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "td/merge database")]
public class MergeDatabaseSO : ScriptableObject
{
    [Serializable]
    public struct MergeRecipe
    {
        public TowerCardSO cardA;
        public TowerCardSO cardB;
        public TowerCardSO resultCard;
    }

    [Header("recipes")]
    public List<MergeRecipe> recipes;

    public TowerCardSO GetMergeResult(TowerCardSO first, TowerCardSO second)
    {
        if (first == null || second == null || recipes == null) return null;

        for (int i = 0; i < recipes.Count; i++)
        {
            var r = recipes[i];
            // check both directions so order of dragging doesn't matter
            bool matchForward = r.cardA == first && r.cardB == second;
            bool matchReverse = r.cardA == second && r.cardB == first;

            if (matchForward || matchReverse)
            {
                return r.resultCard;
            }
        }

        return null;
    }
}