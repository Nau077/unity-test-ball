using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateMaterialPair
{
    // Пара “состояние клетки → материал для отрисовки”.
    // [Serializable] позволяет видеть/редактировать список таких пар в Inspector.
    public CellState State;   // например, Soil, Watered и т.п.
    public Material Material; // материал, который надо показывать для этого состояния
}

/// <summary>
/// Управляет сеткой клеток (tilemap-логика в 3D) и их визуальной “накладкой”
/// поверх базового пола (травы) через маленькие quad-префабы.
/// </summary>
public class GridManager : MonoBehaviour
{
    // ---- ПАРАМЕТРЫ СЕТКИ ----
    [Header("Grid")]
    public int Width = 50;         // количество клеток по X (вправо)
    public int Height = 50;        // количество клеток по Y (вперёд, по оси Z)
    public float CellSize = 1f;    // размер одной клетки в мировых единицах
    public Vector3 Origin;         // мировая позиция “левого-нижнего” угла сетки

    // ---- ВИЗУАЛИЗАЦИЯ ----
    [Header("Visuals")]
    public GameObject overlayPrefab;   // префаб квадрата (PF_CellOverlay), который кладём на клетку
    public GameObject highlightPrefab; // префаб подсветки выбранной клетки (полупрозрачный квадрат)
    public Transform overlaysParent;   // родитель для всех оверлеев (для порядка в иерархии)
    public List<StateMaterialPair> stateMaterials = new(); // задать через Inspector соответствия “State → Material”

    // ---- ВНУТРЕННЯЯ СТРУКТУРА ОДНОЙ КЛЕТКИ ----
    private class GridCell
    {
        public CellState State = CellState.Empty; // текущее состояние клетки
        public GameObject OverlayGO;              // инстанс оверлея (quad), который лежит на этой клетке
    }

    // 2D-массив клеток (логика), объект подсветки и кэш словаря “состояние → материал”
    private GridCell[,] _cells;
    private GameObject _highlightGO;
    private readonly Dictionary<CellState, Material> _matCache = new();

    // Awake вызывается сразу после создания компонента, до Start.
    // Здесь инициализируем данные и готовим визуальные объекты.
    void Awake()
    {
        // 1) Собираем быстрый кэш материалов, чтобы не искать их каждый раз в списке.
        foreach (var p in stateMaterials)
            if (p != null && p.Material != null)
                _matCache[p.State] = p.Material;

        // 2) Создаём сетку данных (Width x Height) и заполняем её пустыми клетками.
        _cells = new GridCell[Width, Height];
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                _cells[x, y] = new GridCell();

        // 3) Создаём (один раз) объект подсветки, если указан префаб.
        if (highlightPrefab != null)
        {
            _highlightGO = Instantiate(highlightPrefab, transform);     // делаем дочерним Grid
            _highlightGO.SetActive(false);                               // по умолчанию спрятан
            _highlightGO.transform.localScale = new Vector3(CellSize, 1f, CellSize); // растягиваем под размер клетки
        }

        // 4) Если не указан родитель для оверлеев — создаём пустышку “Overlays”.
        if (overlaysParent == null)
        {
            var go = new GameObject("Overlays");
            go.transform.SetParent(transform);
            overlaysParent = go.transform;
        }
    }

    // Быстрая проверка: находится ли индекс клетки внутри границ сетки.
    public bool InBounds(Vector2Int c) => c.x >= 0 && c.y >= 0 && c.x < Width && c.y < Height;

    // Перевод мировой позиции (Vector3) → координаты клетки (Vector2Int).
    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        // Сдвигаем мировую позицию так, чтобы (Origin) был нулём.
        var local = worldPos - Origin;

        // Делим на размер клетки и берём пол (Floor) — получаем номер клетки.
        int x = Mathf.FloorToInt(local.x / CellSize);
        int y = Mathf.FloorToInt(local.z / CellSize); // ВАЖНО: используем Z как “вперёд”
        return new Vector2Int(x, y);
    }

    // Обратное: из индекса клетки получить мировой центр этой клетки.
    public Vector3 CellToWorldCenter(Vector2Int cell)
    {
        // +0.5f — чтобы попасть в центр квадрата, а не в его левый-нижний угол.
        return Origin + new Vector3((cell.x + 0.5f) * CellSize, 0f, (cell.y + 0.5f) * CellSize);
    }

    // Узнать текущее состояние клетки (безопасно, с проверкой границ).
    public CellState GetState(Vector2Int cell)
    {
        if (!InBounds(cell)) return CellState.Empty;         // вне сетки считаем “пусто”
        return _cells[cell.x, cell.y].State;
    }

    // Установить состояние клетки и обновить её визуальный оверлей.
    public void SetState(Vector2Int cell, CellState newState)
    {
        if (!InBounds(cell)) return;
        var gc = _cells[cell.x, cell.y];
        if (gc.State == newState) return;                    // ничего не делаем, если не меняется

        gc.State = newState;
        UpdateOverlay(cell, gc);                             // применяем визуальные изменения
    }

    // Сброс клетки в “пусто”.
    public void ClearState(Vector2Int cell)
    {
        SetState(cell, CellState.Empty);
    }

    // Создаёт/обновляет оверлей (квадрат) поверх клетки и задаёт материал согласно состоянию.
    private void UpdateOverlay(Vector2Int cell, GridCell gc)
    {
        // Если клетка пустая — оверлей не нужен: просто скрываем, если он был.
        if (gc.State == CellState.Empty)
        {
            if (gc.OverlayGO != null)
            {
                gc.OverlayGO.SetActive(false);
            }
            return;
        }

        // Берём материал для текущего состояния из кэша.
        if (!_matCache.TryGetValue(gc.State, out var mat)) return; // если нет — тихо выходим

        // Если оверлея ещё нет — создаём из префаба.
        if (gc.OverlayGO == null)
        {
            if (overlayPrefab == null) return; // защита: префаб не задан — ничего не делаем
            gc.OverlayGO = Instantiate(overlayPrefab, overlaysParent);
            gc.OverlayGO.transform.localScale = new Vector3(CellSize, 1f, CellSize); // под размер клетки
        }

        // Выставляем позицию в центр клетки. Слегка приподнимаем (+0.01), чтобы избежать Z-fighting
        // (мерцание, когда два полигона лежат в одной плоскости).
        gc.OverlayGO.transform.position = CellToWorldCenter(cell) + Vector3.up * 0.01f;

        // Меняем материал у Renderer (берём из детей на случай, если меш не на корне префаба).
        var rend = gc.OverlayGO.GetComponentInChildren<Renderer>();
        if (rend != null) rend.sharedMaterial = mat; // sharedMaterial — без создания уникальной копии материала

        gc.OverlayGO.SetActive(true);
    }

    // Показ/скрытие подсветки на заданной клетке.
    public void ShowHighlight(Vector2Int cell, bool show)
    {
        if (_highlightGO == null) return;
        if (!show || !InBounds(cell))
        {
            _highlightGO.SetActive(false);
            return;
        }

        // Позиционируем подсветку в центре клетки, чуть выше пола.
        _highlightGO.transform.position = CellToWorldCenter(cell) + Vector3.up * 0.02f;
        _highlightGO.SetActive(true);
    }

#if UNITY_EDITOR
void OnDrawGizmosSelected()
{
    // рамка
    Gizmos.color = Color.green;
    var size = new Vector3(Width * CellSize, 0.01f, Height * CellSize);
    Vector3 center = Origin + new Vector3(size.x, 0, size.z) * 0.5f;

    Gizmos.DrawWireCube(center, size); // контур — видно всегда

    // заливка (чуть заметнее)
    Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
    Gizmos.DrawCube(center, size);
}
#endif

//#if UNITY_EDITOR
//    void OnValidate()
//    {
//        // В редакторе, когда меняешь Width/Height/CellSize,
//        // автоматически центрирует Origin под плоскость в (0,0,0).
//        if (!Application.isPlaying)
//            Origin = new Vector3(-Width * CellSize / 2f, 0f, -Height * CellSize / 2f);
//    }
//#endif
}
