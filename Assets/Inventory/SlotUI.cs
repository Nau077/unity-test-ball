using UnityEngine;
using UnityEngine.UI;
using TMPro; // ВАЖНО!

public class SlotUI : MonoBehaviour
{
    [Header("Refs")]
    public Image icon;                 // ItemIcon (Image)
    public TMP_Text amountText;        // можно не назначать — найдём сами

    private Item item;
    public int Amount { get; private set; }

    void Awake()
    {
        // если в инспекторе не назначено — попробуем найти в детях
        if (amountText == null)
            amountText = GetComponentInChildren<TMP_Text>(true);
    }

    public void SetItem(Item newItem, int amount)
    {
        item = newItem;
        Amount = amount;
        icon.sprite = item.icon;
        icon.color = Color.white;
        UpdateText();
    }

    public void AddAmount(int amount)
    {
        Amount += amount;
        UpdateText();
    }

    public void ClearSlot()
    {
        item = null;
        Amount = 0;
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0);
        if (amountText != null) amountText.text = "";
    }

    public bool HasItem(Item checkItem) => item == checkItem;
    public bool IsEmpty() => item == null;

    private void UpdateText()
    {
        if (amountText == null) return;              // нет текста — просто выходим
        amountText.text = Amount > 1 ? $"{Amount}" : "";
    }
}
