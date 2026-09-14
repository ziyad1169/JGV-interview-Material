using System;

[Serializable]
public struct InventoryItem
{
    public int id;
    public string uniqueIdentifier;

    public InventoryItem(int id)
    {
        this.id = id;
        this.uniqueIdentifier = Guid.NewGuid().ToString();
    }
}
