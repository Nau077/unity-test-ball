using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refs")]
    public Image icon;                 // ItemIcon
    public TMP_Text amountText;        // AmountText

    [Header("Tooltip")]
    public GameObject tooltipPanel;    // TooltipPanel
    public TMP_Text tooltipText;       // TooltipText внутри

    private Item item;
    public int Amount { get; private set; }

    void Awake()
    {
        if (amountText == null)
            amountText = GetComponentInChildren<TMP_Text>(true);

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    public void SetItem(Item newItem, int amount)
    {
        item = newItem;
        Amount = amount;
        icon.enabled = true;
        icon.sprite = item.icon;
        icon.color = Color.white;

        amountText.gameObject.SetActive(true);
        UpdateText();
    }

    public void AddAmount(int amount)
    {
        Amount += amount;
        amountText.gameObject.SetActive(true);
        UpdateText();
    }

    public void ClearSlot()
    {
        item = null;
        Amount = 0;
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0);
        if (amountText != null) amountText.text = "";

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    public bool HasItem(Item checkItem) => item == checkItem;
    public bool IsEmpty() => item == null;

    private void UpdateText()
    {
        if (amountText == null) return;

        if (Amount > 1)
        {
            amountText.gameObject.SetActive(true);
            amountText.text = Amount.ToString();
        }
        else
        {
            // полностью выключаем объект
            amountText.gameObject.SetActive(false);
        }
    }

    // ===========================
    // Tooltip
    // ===========================
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && tooltipPanel != null)
        {
            tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}
