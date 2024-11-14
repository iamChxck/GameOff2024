using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemCategory;
    public string itemDescription;

    public GameObject modelPrefab;
    public Sprite icon;
}
