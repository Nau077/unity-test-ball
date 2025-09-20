using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
// Item.cs
// Описывает данные предмета как ScriptableObject (название, иконка).
// Нужно для того, чтобы легко создавать и хранить разные предметы (мечи, семена, броня).
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool stackable = true;

    [TextArea(2, 5)]
    public string description;   // новое поле
}
