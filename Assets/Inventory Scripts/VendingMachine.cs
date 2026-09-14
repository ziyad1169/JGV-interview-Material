using System;
using UnityEngine;
using Mirror;

public class VendingMachine : MonoBehaviour
{
    public static VendingMachine Instance { get; private set; }


    public ItemDataSO[] availableItems;

    void Awake()
    {
        Instance = this;
    }

    [Server]
    public void ServerProcessPurchase(PlayerInventory buyer, int itemId)
    {
        if (buyer == null || availableItems == null) return;


        ItemDataSO itemToBuy = Array.Find(availableItems, item => item != null && item.id == itemId);
        if (itemToBuy == null) return;


        if (!buyer.ServerTryDeductMoney(itemToBuy.price)) return;


        InventoryItem newItem = new InventoryItem(itemToBuy.id);


        PlayerInventory recipient = (itemToBuy.id % 2 != 0)
            ? buyer
            : (PlayerInventory.HostInventory ?? (buyer.isServer ? buyer : null));

        recipient?.ServerAddItem(newItem);
    }
}
