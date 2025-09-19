using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
// InventoryUI.cs
// Создает сетку слотов в UI и управляет их заполнением.
// Нужно для отображения всего инвентаря и добавления предметов в слоты.
{
    public GameObject slotPrefab;   // Префаб слота
    public Transform slotsParent;   // Сетка слотов
    public int slotCount = 25;      // Кол-во слотов

    private List<SlotUI> slots = new List<SlotUI>();

    void Start()
    {
        // создаём сетку
        for (int i = 0; i < 25; i++)   // ✅ будет ровно 25 слотов
        {
            var slot = Instantiate(slotPrefab, slotsParent);
            slots.Add(slot.GetComponent<SlotUI>());
        }
        // тест: положим семена в первый слот
        Item seed = Resources.Load<Item>("SeedItem");
        if (seed != null && slots.Count > 0)
        {
            slots[0].AddItem(seed);
        }
    }
}
