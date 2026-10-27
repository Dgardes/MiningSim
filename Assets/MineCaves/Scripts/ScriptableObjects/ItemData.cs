using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string code;
    public string itemName;
    public int value;
    public Sprite itemSprite;
}
