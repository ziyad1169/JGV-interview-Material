using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VendingMachineTrigger : MonoBehaviour
{
    public static bool IsPlayerInZone { get; private set; }

    [Header("Proximity Check (Guarantees Detection)")]
    public float interactionRadius = 3f;

    private int activeColliders = 0;

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
        if (PlayerInventory.LocalPlayer != null)
        {
            float dist = Vector3.Distance(transform.position, PlayerInventory.LocalPlayer.transform.position);
            bool isClose = dist <= interactionRadius;

            if (isClose && !IsPlayerInZone)
            {
                SetZoneState(true);
            }
            else if (!isClose && IsPlayerInZone && activeColliders <= 0)
            {
                SetZoneState(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerInventory>();
        if (player == null || !player.isLocalPlayer) return;

        activeColliders++;
        SetZoneState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponentInParent<PlayerInventory>();
        if (player != null && player.isLocalPlayer)
        {
            activeColliders = Mathf.Max(0, activeColliders - 1);
            if (activeColliders <= 0)
            {
                if (PlayerInventory.LocalPlayer == null || Vector3.Distance(transform.position, PlayerInventory.LocalPlayer.transform.position) > interactionRadius)
                {
                    SetZoneState(false);
                }
            }
        }
    }

    private void SetZoneState(bool inside)
    {
        IsPlayerInZone = inside;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowInteractPrompt(inside);

            if (!inside)
            {
                UIManager.Instance.ShowVendingCanvas(false);
            }
        }
    }
}
