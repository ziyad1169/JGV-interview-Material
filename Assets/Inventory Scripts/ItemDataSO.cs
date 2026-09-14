using UnityEngine;
[CreateAssetMenu(fileName = "NewItemData", menuName = "Vending/Item Data")]
public class ItemDataSO : ScriptableObject
{
    public int id;
    public string itemName;
    public int price;
}
