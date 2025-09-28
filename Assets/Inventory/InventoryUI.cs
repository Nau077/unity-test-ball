using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform slotsParent;
    public int slotCount = 25;

    private List<SlotData> slots = new List<SlotData>();
    private const int MaxStack = 99;

    private bool initialized = false;

    private void Awake()
    {
        Instance = this;
        EnsureInitialized();
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
            this.item = null;
            this.amount = 0;
            slotUI.Clear(); // 🔥 сразу чистим UI
        }

        public void SetItem(Item newItem, int newAmount)
        {
            item = newItem;
            amount = newAmount;
            slotUI.ForceRefresh(item, amount); // только UI-отрисовка
        }

        public void AddAmount(int add)
        {
            amount += add;
            if (amount > InventoryUI.MaxStack)
                amount = InventoryUI.MaxStack;

            slotUI.ForceRefresh(item, amount);
        }

        public bool HasItem(Item checkItem)
        {
            return item != null && item.itemID == checkItem.itemID;
        }

        public bool IsEmpty()
        {
            Debug.Log($"[IsEmpty] slotUI={slotUI.name}, item={(item == null ? "NULL" : item.itemName)} amount={amount}");
            return item == null || amount <= 0;
        }
    }
    

    // -------------------------------
    // Ленивая инициализация
    // -------------------------------
    private void EnsureInitialized()
    {
        if (initialized) return;


        foreach (Transform child in slotsParent)
        {
            var slotUI = child.GetComponent<SlotUI>();
            if (slotUI != null)
                slots.Add(new SlotData(slotUI));
        }

        int toCreate = slotCount - slots.Count;
        for (int i = 0; i < toCreate; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotsParent);
            var slotUI = slotGO.GetComponent<SlotUI>();
            slots.Add(new SlotData(slotUI));
        }

        initialized = true;
    }

    // -------------------------------
    // Методы работы с инвентарём
    // -------------------------------
    public void AddItem(Item newItem, int amount)
    {
        EnsureInitialized(); // 🔥 гарантируем, что слоты готовы
        // Debug.Log($"[AddItem] Start. slots.Count={slots.Count}, initialized={initialized}");
        Debug.Log($"[AddItem] Добавляем {newItem.itemName} x{amount}, slots.Count={slots.Count}");

        int remaining = amount;

        // сначала стакуем
        foreach (var slot in slots)
        {
            Debug.Log($"[AddItem] Проверяем слот {slot.slotUI.name}, item={(slot.item == null ? "NULL" : slot.item.itemName)}, amount={slot.amount}");
            if (slot.HasItem(newItem) && slot.amount < MaxStack)
            {
                int canAdd = Mathf.Min(MaxStack - slot.amount, remaining);
                slot.AddAmount(canAdd);
                remaining -= canAdd;
                // Debug.Log($"[AddItem] Стакуем {newItem.itemName}, остаток {remaining}");

                if (remaining <= 0)
                {
                    DebugInventory();
                    return;
                }
            }
        }

        // если некуда стакать — кладём в пустые
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                int putAmount = Mathf.Min(MaxStack, remaining);
                slot.SetItem(newItem, putAmount);
                remaining -= putAmount;
                Debug.Log($"[AddItem] Новый слот: {newItem.itemName} x{putAmount}");

                if (remaining <= 0)
                {
                    DebugInventory();
                    return;
                }
            }
        }

        if (remaining > 0)
            Debug.LogWarning($"[AddItem] Не хватило места для {newItem.itemName}, остаток {remaining}");

        DebugInventory();
    }

    private void DebugInventory()
    {
        Debug.Log("=== Текущее содержимое инвентаря ===");
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty())
                Debug.Log($"Слот {i + 1}: {slot.item.itemName} x{slot.amount}");
        }
    }

}
