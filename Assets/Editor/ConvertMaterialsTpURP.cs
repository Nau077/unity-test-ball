using UnityEngine;
using UnityEditor;

public class ConvertMaterialsToURP : EditorWindow
{
    [MenuItem("Tools/Convert Materials To URP")]
    static void Convert()
    {
        // Загружаем все материалы в проекте
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");

        int converted = 0;

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat != null && mat.shader != null && mat.shader.name == "Standard")
            {
                Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
                if (urpLit != null)
                {
                    mat.shader = urpLit;
                    EditorUtility.SetDirty(mat);
                    converted++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Конвертировано {converted} материалов в URP/Lit.");
    }
}
