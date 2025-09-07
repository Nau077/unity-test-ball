using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BoundsLimiter : MonoBehaviour
{
    [Tooltip("Ссылка на объект поля: перетащи сюда Plane (или любой объект с Renderer)")]
    public Renderer areaRenderer;
    public float extraPadding = 0.02f; // небольшой зазор от края

    private CharacterController cc;

    void Awake() => cc = GetComponent<CharacterController>();

    /// Вернёт позицию, «подрезанную» по границам поля.
    public Vector3 ClampPosition(Vector3 worldPos)
    {
        if (areaRenderer == null) return worldPos;

        Bounds b = areaRenderer.bounds; // мировые границы рендера
        float pad = (cc != null ? cc.radius : 0f) + extraPadding;

        worldPos.x = Mathf.Clamp(worldPos.x, b.min.x + pad, b.max.x - pad);
        worldPos.z = Mathf.Clamp(worldPos.z, b.min.z + pad, b.max.z - pad);
        return worldPos;
    }

    // Опционально: автоматически держать персонажа внутри каждый кадр
    void LateUpdate()
    {
        if (areaRenderer == null) return;
        transform.position = ClampPosition(transform.position);
    }
}
