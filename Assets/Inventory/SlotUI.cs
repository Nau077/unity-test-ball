using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refs")]
    public Image icon;              // ItemIcon (Image)
    public TMP_Text amountText;     // AmountText (TMP)

    [Header("Tooltip")]
    public GameObject tooltipPanel; // TooltipPanel
    public TMP_Text tooltipText;    // TooltipText (TMP)

    // ---- API ----
    public void ForceRefresh(Item item, int amount)
    {
        if (icon != null)
        {
            if (item != null)
            {
                if (!icon.gameObject.activeSelf) icon.gameObject.SetActive(true);
                icon.enabled = true;
                icon.sprite = item.icon;
                icon.color = Color.white;
            }
            else
            {
                icon.enabled = false;
            }
        }

        if (amountText != null)
        {
            if (amount > 1)
            {
                amountText.text = amount.ToString();
                amountText.gameObject.SetActive(true);
            }
            else
            {
                amountText.text = "";
                amountText.gameObject.SetActive(false);
            }
        }

        // Тултип
        if (tooltipPanel != null)
        {
            if (item != null)
                tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
            else
                tooltipPanel.SetActive(false);
        }
    }

    public void Clear()
    {
        ForceRefresh(null, 0);
    }

    // Tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null && icon != null && icon.enabled)
            tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}
