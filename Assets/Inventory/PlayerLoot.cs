using UnityEngine;

public class PlayerLoot : MonoBehaviour
{
    private LootItem currentLoot;

    void Update()
    {

        if (InventoryUI.Instance == null)
        {
            Debug.LogError("❌ InventoryUI всё ещё null, не нашли!");
            return;
        }

        if (currentLoot != null && Input.GetKeyDown(KeyCode.X))
        {
            if (InventoryUI.Instance == null)
            {
                Debug.LogError("❌ InventoryUI.Instance == null! В сцене нет активного объекта с InventoryUI");
                return;
            }

            if (currentLoot.item == null)
            {
                Debug.LogError("❌ currentLoot.item == null! У LootItem не назначен Item");
                return;
            }

            Debug.Log($"▶ Добавляем {currentLoot.amount} x {currentLoot.item.itemName} в инвентарь");
            InventoryUI.Instance.AddItem(currentLoot.item, currentLoot.amount);

            Destroy(currentLoot.gameObject);
            currentLoot = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        LootItem loot = other.GetComponent<LootItem>();
        if (loot != null)
        {
            currentLoot = loot;
            Debug.Log($"Подойди и нажми X, чтобы поднять {loot.item.itemName}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LootItem loot = other.GetComponent<LootItem>();
        if (loot != null && loot == currentLoot)
        {
            currentLoot = null;
            Debug.Log("Вышел из зоны подбора");
        }
    }
}
