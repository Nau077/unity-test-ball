using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLoot : MonoBehaviour
{
    public float lootRange = 2f;
    private InventoryUI inventory;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Enable();
        controls.Player.Loot.performed += ctx => TryLoot();
    }

    void Start()
    {
        inventory = FindObjectOfType<InventoryUI>();
    }

    void TryLoot()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, lootRange);
        foreach (var hit in hits)
        {
            LootItem loot = hit.GetComponent<LootItem>();
            if (loot != null)
            {
                inventory.AddItem(loot.item, loot.amount);
                Destroy(hit.gameObject); // убираем с земли
                break;
            }
        }
    }
}
