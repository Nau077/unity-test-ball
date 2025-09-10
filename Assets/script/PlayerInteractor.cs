using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // ����� Input System
#endif

/// ���������� � ��������:
/// 1 = ������ (��������), 2 = ����� (������), R = ��������, F = ���������.
/// 0 ��� Esc � ����� ���������� (��������� ������).
public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public GridManager grid;   // �������� ���� ������ Grid
    public Transform player;  // ��������� ���������, ������� ������� ��������������

    [Header("Aiming")]
    public float aheadDistance = 0.6f; // ��������� ������� �������
    public float maxSnapDistance = 1.2f; // ������ �� ������ ������

    private enum ToolMode { None, Hoe, Water, Clear }
    [SerializeField] private ToolMode tool = ToolMode.None;

    void Update()
    {
        if (!grid || !player) return;

        // --- �����/������ ����������� ---
        HandleToolSelection();

        // --- ��������� ������ ����� ���������� ---
        var fwd = player.forward;
        fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward;

        var probe = player.position + fwd.normalized * aheadDistance;
        var cell = grid.WorldToCell(probe);
        var center = grid.CellToWorldCenter(cell);
        // ����� ������ �������� �� maxSnapDistance?
        // ����� ��������� �� ��������� ��� �������� ������� ����� 
        // �������� � �� �������� ������ �� ���������
        // ����, ���� probe ���� �����������.��� �������� UX - ������.
        bool ok = grid.InBounds(cell) && (Vector3.Distance(center, probe) <= maxSnapDistance);

        // --- ��������� ������ ��� ��������� ����������� ---
        bool aiming = tool != ToolMode.None;
        grid.ShowHighlight(cell, aiming && ok);
        if (!(aiming && ok)) return;

        // --- ��������: ������ F ---
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

    // === �����/������ ����������� ===
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
        // ��������� ������� ��� �� ������ ��������� ����������
        tool = (tool == t) ? ToolMode.None : t;
    }

    // === ������������� �������� (F) ===
    private bool ActionPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.F);
#endif
    }

    // === �������� ��� ������� ===
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
