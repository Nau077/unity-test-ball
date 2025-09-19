using UnityEngine;
using UnityEngine.UI;
// SlotUI.cs
// Управляет одним слотом в инвентаре (показывает иконку предмета, очищает слот).
// Нужно для отображения предмета внутри отдельной ячейки инвентаря.
public class SlotUI : MonoBehaviour
{
    public Image icon;   // ссылка на картинку ItemIcon
    private Item item;   // что лежит в этом слоте

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.color = Color.white; // делаем иконку видимой
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0); // делаем иконку прозрачной
    }
}
