using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform slotsParent;
    public int slotCount = 25;

    private List<SlotData> slots = new List<SlotData>();
    private const int MaxStack = 99;

    private void Awake()
    {
        Instance = this;
    }

    // -------------------------------
    // Внутренний класс: хранит данные слота
    // -------------------------------
    private class SlotData
    {
        public Item item;
        public int amount;
        public SlotUI slotUI;

        public SlotData(SlotUI slotUI)
        {
            this.slotUI = slotUI;
            Clear();
        }

        public void SetItem(Item newItem, int newAmount)
        {
            item = newItem;
            amount = newAmount;
            slotUI.SetItem(newItem, newAmount);
        }

        public void AddAmount(int add)
        {
            amount += add;
            if (amount > InventoryUI.MaxStack)
                amount = InventoryUI.MaxStack;
            slotUI.AddAmount(add);
        }

        public void Clear()
        {
            item = null;
            amount = 0;
            slotUI.ClearSlot();
        }

        public bool HasItem(Item checkItem) => item == checkItem;
        public bool IsEmpty() => item == null;
    }

    // -------------------------------
    // Unity lifecycle
    // -------------------------------
    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotsParent);
            var slotUI = slotGO.GetComponent<SlotUI>();
            slots.Add(new SlotData(slotUI));
        }

        // Тестовые предметы
        Item seed = Resources.Load<Item>("SeedItem");
        Item metal = Resources.Load<Item>("MetallItem");

        AddItem(seed, 5);
        AddItem(metal, 12);
        AddItem(metal, 200);
    }

    // -------------------------------
    // Методы работы с инвентарём
    // -------------------------------
    public void AddItem(Item newItem, int amount)
    {
        int remaining = amount;

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

        if (remaining > 0)
            Debug.LogWarning($"Не хватило места для {newItem.name}, остаток: {remaining}");
    }
}
