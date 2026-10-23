using UnityEngine;

public abstract class CardDataSO : ScriptableObject
{
    [Header("card ui")]
    public string cardName;
    [TextArea] public string description;
    public Sprite cardArt;

    public abstract int GoldCost { get; }
}