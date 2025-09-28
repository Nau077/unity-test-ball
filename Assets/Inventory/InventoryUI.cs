using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform slotsParent;
    public int slotCount = 25;

    [Header("Drag&Drop")]
    public Image dragIconPrefab;   // перетаскиваемая иконка
    private Image dragIconInstance;
    private SlotUI draggedSlot;

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
            item = null;
            amount = 0;
            slotUI.Clear();
        }

        public void SetItem(Item newItem, int newAmount)
        {
            item = newItem;
            amount = newAmount;
            slotUI.ForceRefresh(item, amount);
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
            return item == null || amount <= 0;
        }
    }

    // -------------------------------
    // Ленивая инициализация
    // -------------------------------
    private void EnsureInitialized()
    {
        if (initialized) return;

        int index = 0;
        foreach (Transform child in slotsParent)
        {
            var slotUI = child.GetComponent<SlotUI>();
            if (slotUI != null)
            {
                slots.Add(new SlotData(slotUI));
                index++;
            }
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
        EnsureInitialized();

        int remaining = amount;

        // сначала стакуем
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

        // затем кладём в пустые
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
            Debug.LogWarning($"[AddItem] Не хватило места для {newItem.itemName}, остаток {remaining}");
    }

    // -------------------------------
    // Drag&Drop
    // -------------------------------
    public void StartDrag(SlotUI slot)
    {
        int index = slots.FindIndex(s => s.slotUI == slot);
        if (index < 0 || slots[index].IsEmpty()) return;

        draggedSlot = slot;
        dragIconInstance = Instantiate(dragIconPrefab, transform.parent);
        dragIconInstance.sprite = slot.icon.sprite;
        dragIconInstance.transform.SetAsLastSibling();
        dragIconInstance.raycastTarget = false;
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        if (dragIconInstance != null)
            dragIconInstance.transform.position = eventData.position;
    }

    public void EndDrag()
    {
        if (dragIconInstance != null)
            Destroy(dragIconInstance.gameObject);

        dragIconInstance = null;
        draggedSlot = null;
    }

    public void DropItem(SlotUI targetSlot)
    {
        if (draggedSlot == null || targetSlot == null) return;

        int fromIndex = slots.FindIndex(s => s.slotUI == draggedSlot);
        int toIndex = slots.FindIndex(s => s.slotUI == targetSlot);

        if (fromIndex < 0 || toIndex < 0) return;

        var from = slots[fromIndex];
        var to = slots[toIndex];

        if (from == to) { EndDrag(); return; }

        // -----------------------------
        // 1. если таргет пустой → просто перемещаем
        // -----------------------------
        if (to.item == null || to.amount <= 0)
        {
            to.SetItem(from.item, from.amount);
            from.SetItem(null, 0);
            EndDrag();
            return;
        }

        // -----------------------------
        // 2. если такой же предмет → стакуем
        // -----------------------------
        if (to.item != null && from.item != null && to.item.itemID == from.item.itemID)
        {
            int canAdd = Mathf.Min(MaxStack - to.amount, from.amount);
            to.AddAmount(canAdd);
            from.amount -= canAdd;

            if (from.amount <= 0)
                from.SetItem(null, 0);
            else
                from.slotUI.ForceRefresh(from.item, from.amount);

            EndDrag();
            return;
        }

        // -----------------------------
        // 3. иначе свап
        // -----------------------------
        var tmpItem = to.item;
        var tmpAmount = to.amount;

        to.SetItem(from.item, from.amount);
        from.SetItem(tmpItem, tmpAmount);

        EndDrag();
    }

}
