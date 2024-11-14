using UnityEngine;

public class InstanceItemContainer : MonoBehaviour
{
    public ItemInstance item;



    private void Awake()
    {
    }

    private void Start()
    {
        InitItem();


        UpdateItem();
    }

    public void InitItem()
    {
        Debug.Log("INITIALIZING " + gameObject.name);
        item.name = item.itemType.itemName;
    }

    public void UpdateItem()
    {

    }

    public ItemInstance TakeItem()
    {
        Destroy(gameObject);
        return item;
    }
}
