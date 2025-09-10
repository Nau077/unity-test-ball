using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // Ќовый Input System
#endif

/// »нтеракции с клетками:
/// 1 = мотыга (вспахать), 2 = лейка (полить), R = очистить, F = выполнить.
/// 0 или Esc Ч сн€ть инструмент (подсветка гаснет).
public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public GridManager grid;   // перетащи сюда объект Grid
    public Transform player;  // трансформ персонажа, который реально поворачиваетс€

    [Header("Aiming")]
    public float aheadDistance = 0.6f; // насколько впереди целимс€
    public float maxSnapDistance = 1.2f; // допуск до центра клетки

    private enum ToolMode { None, Hoe, Water, Clear }
    [SerializeField] private ToolMode tool = ToolMode.None;

    void Update()
    {
        if (!grid || !player) return;

        // --- выбор/сн€тие инструмента ---
        HandleToolSelection();

        // --- вычисл€ем клетку перед персонажем ---
        var fwd = player.forward;
        fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward;

        var probe = player.position + fwd.normalized * aheadDistance;
        var cell = grid.WorldToCell(probe);
        var center = grid.CellToWorldCenter(cell);
        // «ачем вообще проверка на maxSnapDistance?
        // „тобы подсветка не УскакалаФ при переходе границы между 
        // клетками и не выбирала клетки за пределами
        // пол€, если probe чуть промахнулс€.Ёто при€тный UX - фильтр.
        bool ok = grid.InBounds(cell) && (Vector3.Distance(center, probe) <= maxSnapDistance);

        // --- подсветка только при выбранном инструменте ---
        bool aiming = tool != ToolMode.None;
        grid.ShowHighlight(cell, aiming && ok);
        if (!(aiming && ok)) return;

        // --- действие: только F ---
        if (ActionPressedThisFrame())
        {
            switch (tool)
            {
                case ToolMode.Hoe: TillCell(cell); break;
                case ToolMode.Water: WaterCell(cell); break;
                case ToolMode.Clear: grid.ClearState(cell); break;
            }
        }
    }

    // === выбор/сн€тие инструмента ===
    private void HandleToolSelection()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k == null) return;

        if (k.digit1Key.wasPressedThisFrame) ToggleTool(ToolMode.Hoe);
        if (k.digit2Key.wasPressedThisFrame) ToggleTool(ToolMode.Water);
        if (k.rKey.wasPressedThisFrame) ToggleTool(ToolMode.Clear);

        if (k.digit0Key.wasPressedThisFrame || k.escapeKey.wasPressedThisFrame)
            tool = ToolMode.None;
#else
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleTool(ToolMode.Hoe);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleTool(ToolMode.Water);
        if (Input.GetKeyDown(KeyCode.R))      ToggleTool(ToolMode.Clear);
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Escape))
            tool = ToolMode.None;
#endif
    }

    private void ToggleTool(ToolMode t)
    {
        // повторное нажатие той же кнопки выключает инструмент
        tool = (tool == t) ? ToolMode.None : t;
    }

    // === подтверждение действи€ (F) ===
    private bool ActionPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.F);
#endif
    }

    // === действи€ над клеткой ===
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
}
