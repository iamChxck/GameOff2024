using UnityEngine;
using UnityEngine.InputSystem;

public class InstanceItemContainer : Interactables
{
    public ItemInstance item;

    [SerializeField]
    private PlayerInventory inventory;


    private void Awake()
    {
    }

    private void Start()
    {

        InitItem();
        UpdateItem();
    }

    public override void Interact()
    {
        TakeItem();
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
        inventory = FindObjectOfType<PlayerInventory>();
        Destroy(gameObject);
        inventory.inventory.AddItem(item);
        return item;
    }


}
