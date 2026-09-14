using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class StoreItemButton : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text idText;
    public TMP_Text priceText;

    private Button itemButton;

    public void Setup(ItemDataSO item, System.Action onBuyClicked)
    {
        if (nameText != null) nameText.text = item.itemName;
        if (idText != null) idText.text = $"ID: {item.id}";
        if (priceText != null) priceText.text = $"${item.price}";

        if (itemButton == null) itemButton = GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => onBuyClicked?.Invoke());
        }
    }
}
