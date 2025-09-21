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

    private Item item;
    public int Amount { get; private set; }


    void Awake()
    {
        // На всякий случай — проверим, что не нацепили два SlotUI на один объект
        var dups = GetComponents<SlotUI>();
        if (dups.Length > 1)
            Debug.LogError($"[SlotUI] На объекте '{name}' обнаружено {dups.Length} компонентов SlotUI!");

        if (amountText == null)
            amountText = GetComponentInChildren<TMP_Text>(true);

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        // ВАЖНО: ничего не чистим здесь.
    }

    // Если хочешь — можно оставить. Он безопасен, потому что мы больше не трогаем sprite при пустом слоте.
    void OnEnable() => ForceRefresh();

    // ---- API ----
    public void SetItem(Item newItem, int amount)
    {
        item = newItem;
        Amount = amount;
        ForceRefresh();
    }

    public void AddAmount(int amount)
    {
        Amount += amount;
        ForceRefresh();
    }

    public bool HasItem(Item checkItem) => item == checkItem;
    public bool IsEmpty() => item == null;

    // ---- Visual refresh ----
    public void ForceRefresh()
    {
        if (icon != null)
        {
            if (item != null)
            {
                if (!icon.gameObject.activeSelf) icon.gameObject.SetActive(true);
                icon.enabled = true;
                icon.sprite = item.icon;   // <- здесь выставляем спрайт
                icon.color = Color.white;
            }
            else
            {
                // Когда слота нет — просто прячем иконку.
                // НИЧЕГО не делаем со sprite, чтобы никто не "стер" картинку.
                icon.enabled = false;
            }
        }

        if (amountText != null)
        {
            if (Amount > 1)
            {
                amountText.text = Amount.ToString();
                amountText.gameObject.SetActive(true);
            }
            else
            {
                amountText.text = "";
                amountText.gameObject.SetActive(false);
            }
        }
    }

    // Tooltip
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
