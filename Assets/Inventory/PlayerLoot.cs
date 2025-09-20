using UnityEngine;

public class PlayerLoot : MonoBehaviour
{
    private LootItem currentLoot;

    void Update()
    {
        // Если игрок в зоне и жмёт X
        if (currentLoot != null && Input.GetKeyDown(KeyCode.X))
        {
            InventoryUI.Instance.AddItem(currentLoot.item, currentLoot.amount);
            Destroy(currentLoot.gameObject); // убираем предмет с земли
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
