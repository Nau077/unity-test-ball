using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.UI.ToggleInventory.performed += ctx => ToggleInventory();
    }

    private void OnEnable()
    {
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Disable();
    }

    private void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool active = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(active);

            if (active)
            {
                // 🔥 Форсим обновление слотов
                InventoryUI.Instance?.RefreshUI();

                controls.Player.Disable(); // замораживаем управление
            }
            else
            {
                controls.Player.Enable();
            }
        }
        else
        {
            Debug.LogWarning("⚠ InventoryManager: inventoryPanel не назначен!");
        }
    }
}
