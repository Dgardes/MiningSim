using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Scriptable Objects/BlockData")]
public class BlockData : ScriptableObject
{
    [Header("Identity")]
    public string code;
    public string localizationKey;  

    [Header("Base settings")]
    public int durability;

    public GameObject particles;    
    public Sprite blockSprite;
    public AudioClip breakAudio;
    
    [Header("Drop and Luck")]
    public ItemData dropItem;

    public int minBaseDrop = 1;
    public int maxBaseDrop = 1;

    public int maxBonusDrop = 1;

    public float luckSensitivity = 0.5f;
}
