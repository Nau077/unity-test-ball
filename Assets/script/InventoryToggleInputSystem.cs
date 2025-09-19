using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggleInputSystem : MonoBehaviour
{
    [SerializeField] private GameObject inventoryRoot;
    private InputAction toggleAction, escAction;

    void OnEnable()
    {
        toggleAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/i");
        escAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
        toggleAction.performed += _ => Toggle();
        escAction.performed += _ => Hide();
        toggleAction.Enable(); escAction.Enable();
    }
    void OnDisable()
    {
        toggleAction?.Disable(); escAction?.Disable();
        toggleAction?.Dispose(); escAction?.Dispose();
    }
    void Toggle() { if (inventoryRoot) inventoryRoot.SetActive(!inventoryRoot.activeSelf); }
    public void Hide() { if (inventoryRoot) inventoryRoot.SetActive(false); }
}
