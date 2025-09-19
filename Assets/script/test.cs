using UnityEngine;

public class InputDebugTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            Debug.Log("Inventory button pressed!");
        }
    }
}
