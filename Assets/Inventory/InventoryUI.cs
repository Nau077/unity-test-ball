using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject slotPrefab;   // Префаб слота
    public Transform slotsParent;   // Сетка слотов
    public int slotCount = 25;      // Количество слотов

    private List<SlotData> slots = new List<SlotData>();

    private const int MaxStack = 99;

    // -------------------------------
    // Внутренний класс: хранит данные слота
    // -------------------------------
    private class SlotData
    {
        public Item item;
        public int amount;
        public Image icon;
        public Text amountText;

        public SlotData(Image icon, Text amountText)
        {
            this.icon = icon;
            this.amountText = amountText;
            Clear();
        }

        public void SetItem(Item newItem, int newAmount)
        {
            item = newItem;
            amount = newAmount;
            icon.sprite = item.icon;
            icon.color = Color.white;
            UpdateText();
        }

        public void AddAmount(int add)
        {
            amount += add;
            if (amount > InventoryUI.MaxStack)
                amount = InventoryUI.MaxStack; // ограничение стака
            UpdateText();
        }

        public void Clear()
        {
            item = null;
            amount = 0;
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
            if (amountText != null) amountText.text = "";
        }

        public bool HasItem(Item checkItem) => item == checkItem;
        public bool IsEmpty() => item == null;

        private void UpdateText()
        {
            if (amountText != null)
                amountText.text = amount > 1 ? amount.ToString() : "";
        }
    }

    // -------------------------------
    // Unity lifecycle
    // -------------------------------
    void Start()
    {
        // создаём сетку
        for (int i = 0; i < slotCount; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotsParent);
            Image icon = slotGO.transform.Find("ItemIcon").GetComponent<Image>();
            Text amountText = slotGO.transform.Find("AmountText").GetComponent<Text>();

            slots.Add(new SlotData(icon, amountText));
        }

        // Тестовые предметы
        Item seed = Resources.Load<Item>("SeedItem");
        Item metall = Resources.Load<Item>("MetallItem");

        AddItem(seed, 5);   // семена
        AddItem(metall, 12); // металл
        AddItem(metall, 200); // тест на авторазделение по стакам
    }

    // -------------------------------
    // Методы работы с инвентарём
    // -------------------------------

    // Добавить предмет с учётом стака
    public void AddItem(Item newItem, int amount)
    {
        int remaining = amount;

        // 1. Проверяем существующие стаки
        foreach (var slot in slots)
        {
            if (slot.HasItem(newItem) && slot.amount < MaxStack)
            {
                int canAdd = Mathf.Min(MaxStack - slot.amount, remaining);
                slot.AddAmount(canAdd);
                remaining -= canAdd;
                if (remaining <= 0) return;
            }
        }

        // 2. Если ещё остались предметы — ищем пустые слоты
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                int putAmount = Mathf.Min(MaxStack, remaining);
                slot.SetItem(newItem, putAmount);
                remaining -= putAmount;
                if (remaining <= 0) return;
            }
        }

        // 3. Если предметов больше, чем свободных слотов — лишние теряются (можно доработать под дроп)
        if (remaining > 0)
        {
            Debug.LogWarning($"Не хватило места в инвентаре для {newItem.name}, остаток: {remaining}");
        }
    }
}
