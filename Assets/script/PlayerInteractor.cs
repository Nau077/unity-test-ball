using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // новый инпут
#endif

public class PlayerInteractor : MonoBehaviour
{
    public GridManager grid;
    public Transform player;

    [Header("Aim")]
    public float aheadDistance = 0.6f;   // насколько впереди целимся
    public float maxSnapDistance = 1.2f; // допуск до центра клетки

    private enum ToolMode { None, Hoe, Water, Build, Clear }
    [SerializeField] private ToolMode tool = ToolMode.None;

    void Update()
    {
        if (!grid || !player) return;

        // направление вперёд в плоскости XZ
        var fwd = player.forward; fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward;

        // точка прицеливания перед персонажем
        var probe = player.position + fwd.normalized * aheadDistance;

        // вычисляем клетку под прицелом
        var cell = grid.WorldToCell(probe);
        var center = grid.CellToWorldCenter(cell);
        bool ok = grid.InBounds(cell) && (Vector3.Distance(center, probe) <= maxSnapDistance);

        // ------------------ выбор инструмента ------------------
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k != null)
        {
            if (k.digit1Key.wasPressedThisFrame) tool = ToolMode.Hoe;
            if (k.digit2Key.wasPressedThisFrame) tool = ToolMode.Water;
            if (k.digit3Key.wasPressedThisFrame) tool = ToolMode.Build;
            if (k.rKey.wasPressedThisFrame) tool = ToolMode.Clear;

            if (k.digit0Key.wasPressedThisFrame || k.escapeKey.wasPressedThisFrame)
                tool = ToolMode.None;
        }
#else
        if (Input.GetKeyDown(KeyCode.Alpha1)) tool = ToolMode.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) tool = ToolMode.Water;
        if (Input.GetKeyDown(KeyCode.Alpha3)) tool = ToolMode.Build;
        if (Input.GetKeyDown(KeyCode.R))      tool = ToolMode.Clear;
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Escape))
            tool = ToolMode.None;
#endif

        // подсветку показываем ТОЛЬКО когда выбран инструмент
        bool aiming = tool != ToolMode.None;
        grid.ShowHighlight(cell, aiming && ok);
        if (!(aiming && ok)) return;

        // ------------------ подтверждение действия ------------------
        bool apply = false;
#if ENABLE_INPUT_SYSTEM
        var m = Mouse.current;
        apply = (k != null && (k.eKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame))
                || (m != null && m.leftButton.wasPressedThisFrame);
#else
        apply = Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0);
#endif
        if (!apply) return;

        // выполняем выбранный инструмент
        switch (tool)
        {
            case ToolMode.Hoe: TillCell(cell); break;
            case ToolMode.Water: WaterCell(cell); break;
            case ToolMode.Build: BuildCell(cell); break;
            case ToolMode.Clear: grid.ClearState(cell); break;
        }
    }

    // --- действия над клеткой ---
    private void TillCell(Vector2Int cell)
    {
        var s = grid.GetState(cell);
        if (s == CellState.Empty || s == CellState.Path)
            grid.SetState(cell, CellState.Soil);
    }

    private void WaterCell(Vector2Int cell)
    {
        var s = grid.GetState(cell);
        if (s == CellState.Soil || s == CellState.Watered)
            grid.SetState(cell, CellState.Watered);
    }

    private void BuildCell(Vector2Int cell)
    {
        var s = grid.GetState(cell);
        if (s != CellState.Building)
            grid.SetState(cell, CellState.Building);
    }
}
