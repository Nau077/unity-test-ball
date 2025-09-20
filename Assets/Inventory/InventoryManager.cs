using UnityEngine;
using UnityEngine.InputSystem; // подключаем новый Input System

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel; // сюда перетащим InventoryPanel из сцены

    private PlayerControls controls;

    private void Awake()
    {
        // создаём экземпляр PlayerControls
        controls = new PlayerControls();

        // подписываемся на событие нажатия кнопки
        controls.UI.ToggleInventory.performed += ctx => ToggleInventory();
    }

    private void OnEnable()
    {
        controls.UI.Enable(); // включаем карту действий UI
    }

    private void OnDisable()
    {
        controls.UI.Disable(); // выключаем карту действий UI
    }

    /// <summary>
    /// Открыть/закрыть инвентарь
    /// </summary>
    private void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("⚠ InventoryManager: inventoryPanel не назначен!");
        }
    }
}
