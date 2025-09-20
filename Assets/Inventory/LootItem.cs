using UnityEngine;

public class LootItem : MonoBehaviour
{
    public Item item;   // что за предмет (ScriptableObject)
    public int amount = 1;

    private void OnDrawGizmos()
    {
        if (item != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}
