using UnityEngine;

// LootItem.cs
// Отвечает за предмет, который лежит на земле.
// Хранит ссылку на Item (например, металл) и количество.
public class LootItem : MonoBehaviour
{
    public Item item;        // какой предмет лежит
    public int amount = 1;   // сколько штук

    void OnDrawGizmos()
    {
        // Чтобы в редакторе видеть над кубиком подпись
        if (item != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"{item.itemName} x{amount}");
        }
    }
}
