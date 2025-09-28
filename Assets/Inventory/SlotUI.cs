using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Refs")]
    public Image icon;              // ItemIcon (Image)
    public TMP_Text amountText;     // AmountText (TMP)

    [Header("Tooltip")]
    public GameObject tooltipPanel; // TooltipPanel
    public TMP_Text tooltipText;    // TooltipText (TMP)

    // Для InventoryUI
    public Item CurrentItem { get; private set; }
    public int CurrentAmount { get; private set; }

    // ---- Отрисовка ----
    public void ForceRefresh(Item item, int amount)
    {
        CurrentItem = item;
        CurrentAmount = amount;

        if (icon != null)
        {
            // иконка всегда активна, чтобы слот ловил Raycast/Drop
            if (!icon.gameObject.activeSelf) icon.gameObject.SetActive(true);

            if (item != null)
            {
                icon.sprite = item.icon;
                icon.color = Color.white;      // видимая
            }
            else
            {
                icon.sprite = null;            // не держим старый спрайт
                var c = icon.color;            // делаем прозрачной вместо enabled=false
                c.a = 0f;
                icon.color = c;
            }
        }

        if (amountText != null)
        {
            if (item != null && amount > 1)
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

        // Тултип: обновляем текст и ОБЯЗАТЕЛЬНО прячем (чтоб не «залипал»)
        if (tooltipPanel != null)
        {
            if (item != null && tooltipText != null)
                tooltipText.text = $"<b>{item.itemName}</b>\n{item.description}";
            else if (tooltipText != null)
                tooltipText.text = "";

            tooltipPanel.SetActive(false);
        }
    }

    public void Clear() => ForceRefresh(null, 0);

    // ---- Tooltip ----
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null && CurrentItem != null)
            tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    // ---- Drag & Drop ----
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CurrentItem == null) return;
        if (tooltipPanel != null) tooltipPanel.SetActive(false); // не залипаем
        InventoryUI.Instance.StartDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryUI.Instance.UpdateDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryUI.Instance.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryUI.Instance.DropItem(this);
    }
}
