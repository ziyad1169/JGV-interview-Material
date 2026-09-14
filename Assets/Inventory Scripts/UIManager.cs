using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("1. Money Canvas (Always Visible)")]
    public GameObject moneyCanvas;
    public TMP_Text moneyText;

    [Header("2. Interact Prompt Canvas (Zone Triggered)")]
    public GameObject interactPromptCanvas;

    [Header("3. Inventory Canvas (Toggled with Tab)")]
    public GameObject inventoryCanvas;
    public TMP_Text inventoryItemsText;

    [Header("4. Vending Machine Canvas (Toggled with E)")]
    public GameObject vendingCanvas;
    public Transform itemsContainer;
    public GameObject itemButtonPrefab;

    private bool isVendingOpen = false;
    private bool isInventoryOpen = false;
    private bool isStoreBuilt = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        FixPromptCanvas();

        if (moneyCanvas != null) moneyCanvas.SetActive(true);
        if (interactPromptCanvas != null) interactPromptCanvas.SetActive(false);
        if (inventoryCanvas != null) inventoryCanvas.SetActive(false);
        if (vendingCanvas != null) vendingCanvas.SetActive(false);

        BuildVendingStoreUI();
    }

    void FixPromptCanvas()
    {
        if (interactPromptCanvas == null) return;

        // 1. Fix scale in case it was initialized to zero
        interactPromptCanvas.transform.localScale = Vector3.one;

        // 2. Ensure Canvas Scaler uses ScaleWithScreenSize (1920x1080)
        var scaler = interactPromptCanvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }

        // 3. Ensure Canvas renders on top
        var canvas = interactPromptCanvas.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 25;
        }

        // 4. Ensure prompt child position is not pushed off-screen on lower resolutions
        for (int i = 0; i < interactPromptCanvas.transform.childCount; i++)
        {
            RectTransform childRt = interactPromptCanvas.transform.GetChild(i) as RectTransform;
            if (childRt != null)
            {
                Vector2 pos = childRt.anchoredPosition;
                if (pos.y < -350)
                {
                    pos.y = -250;
                    childRt.anchoredPosition = pos;
                }
            }
        }
    }

    void Update()
    {
        if (!isStoreBuilt && VendingMachine.Instance != null && VendingMachine.Instance.availableItems != null)
        {
            BuildVendingStoreUI();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (VendingMachineTrigger.IsPlayerInZone)
            {
                isVendingOpen = !isVendingOpen;
                ShowVendingCanvas(isVendingOpen);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInventoryOpen = !isInventoryOpen;
            if (inventoryCanvas != null) inventoryCanvas.SetActive(isInventoryOpen);
        }

        if (isInventoryOpen && PlayerInventory.LocalPlayer != null)
        {
            UpdateInventoryText(PlayerInventory.LocalPlayer.GetItemsIdString());
        }

        if (PlayerInventory.LocalPlayer != null && moneyText != null)
        {
            moneyText.text = $"{PlayerInventory.LocalPlayer.money}";
        }
    }

    public void ShowInteractPrompt(bool show)
    {
        if (interactPromptCanvas != null)
        {
            interactPromptCanvas.SetActive(show);
            if (show)
            {
                for (int i = 0; i < interactPromptCanvas.transform.childCount; i++)
                {
                    interactPromptCanvas.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    public void ShowVendingCanvas(bool show)
    {
        isVendingOpen = show;
        if (vendingCanvas != null)
        {
            vendingCanvas.SetActive(show);
        }
    }

    public void UpdateInventoryText(string itemsFormatted)
    {
        if (inventoryItemsText != null)
        {
            inventoryItemsText.text = $"Items: {itemsFormatted}";
        }
    }

    public void BuildVendingStoreUI()
    {
        if (VendingMachine.Instance == null || itemsContainer == null || itemButtonPrefab == null)
            return;

        if (VendingMachine.Instance.availableItems == null || VendingMachine.Instance.availableItems.Length == 0)
            return;

        // Clear existing buttons
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in VendingMachine.Instance.availableItems)
        {
            if (item == null) continue;

            GameObject btnObj = Instantiate(itemButtonPrefab, itemsContainer);
            int itemId = item.id;

            StoreItemButton storeBtn = btnObj.GetComponent<StoreItemButton>();
            if (storeBtn != null)
            {
                storeBtn.Setup(item, () =>
                {
                    PlayerInventory.LocalPlayer?.CmdRequestPurchase(itemId);
                });
            }
            else
            {
                var txt = btnObj.GetComponentInChildren<TMP_Text>();
                if (txt != null)
                {
                    txt.text = $"{item.itemName} (ID: {item.id}) - ${item.price}";
                }

                Button btn = btnObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        PlayerInventory.LocalPlayer?.CmdRequestPurchase(itemId);
                    });
                }
            }
        }

        isStoreBuilt = true;
    }
}
