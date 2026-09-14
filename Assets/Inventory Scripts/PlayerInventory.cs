using System;
using System.Text;
using UnityEngine;
using Mirror;

public class PlayerInventory : NetworkBehaviour
{
    public static PlayerInventory HostInventory { get; private set; }
    public static PlayerInventory LocalPlayer { get; private set; }

    [SyncVar(hook = nameof(OnMoneyChanged))]
    public int money = 100;

    public readonly SyncList<InventoryItem> items = new SyncList<InventoryItem>();

    public event Action<int> OnMoneyUpdated;
    public event Action<string> OnItemsUpdated;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (connectionToClient != null && (connectionToClient == NetworkServer.localConnection || connectionToClient.connectionId == 0))
        {
            HostInventory = this;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        LocalPlayer = this;

        if (isServer)
        {
            HostInventory = this;
        }

        items.Callback += OnItemsCallback;

        OnMoneyUpdated?.Invoke(money);
        OnItemsUpdated?.Invoke(GetItemsIdString());
    }

    void OnDestroy()
    {
        if (isLocalPlayer) items.Callback -= OnItemsCallback;
    }

    void OnMoneyChanged(int oldVal, int newVal)
    {
        if (isLocalPlayer) OnMoneyUpdated?.Invoke(newVal);
    }

    void OnItemsCallback(SyncList<InventoryItem>.Operation op, int index, InventoryItem oldItem, InventoryItem newItem)
    {
        if (isLocalPlayer) OnItemsUpdated?.Invoke(GetItemsIdString());
    }

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public string GetItemsIdString()
    {
        if (items.Count == 0) return "Empty";

        var sortedIds = new System.Collections.Generic.List<int>();
        for (int i = 0; i < items.Count; i++)
        {
            sortedIds.Add(items[i].id);
        }
        sortedIds.Sort();

        return string.Join(" - ", sortedIds);
    }

    [Command]
    public void CmdRequestPurchase(int itemId)
    {
        VendingMachine.Instance?.ServerProcessPurchase(this, itemId);
    }

    [Server]
    public bool ServerTryDeductMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }

    [Server]
    public void ServerAddItem(InventoryItem item)
    {
        items.Add(item);
    }
}
