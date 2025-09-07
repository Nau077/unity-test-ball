// Assets/script/CellState.cs  (путь может быть любым в Assets)
public enum CellState
{
    Empty = 0,   // пусто (только базовая трава на полу)
    Soil = 1,    // вспаханная земля
    Watered = 2, // политая земля
    Path = 3,    // дорожка (опционально)
    Building = 4 // занята постройкой/зоной
}
