using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

public class TextureParserEditor : MonoBehaviour
{
    [MenuItem("Tools/Parse Materials")]
    public static void ParseMaterials()
    {
        // Папка, где находятся материалы
        string materialsFolder = "Assets/Radaiance/Materials/testMaterials";

        // Найти все материалы в указанной папке
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { materialsFolder });
        var materials = new List<Material>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            materials.Add(material);
        }

        // Создать структуру и заполнить её
        var houseMaterialManager = FindObjectOfType<HouseMaterialManager>();
        if (houseMaterialManager == null)
        {
            Debug.LogError("HouseMaterialManager не найден на сцене.");
            return;
        }

        houseMaterialManager.HouseParts.Clear();

        // Получить все объекты на текущей сцене
        var sceneObjects = FindObjectsOfType<GameObject>();
        HashSet<string> houseNamesOnScene = new HashSet<string>();

        foreach (var obj in sceneObjects)
        {
            string[] nameParts = obj.name.Split('_');
            if (nameParts.Length >= 2)
            {
                houseNamesOnScene.Add(nameParts[0]);
            }
        }

        foreach (var material in materials)
        {
            string[] nameParts = material.name.Split('_');
            if (nameParts.Length < 3)
            {
                Debug.LogError($"Материал {material.name} не соответствует формату.");
                continue;
            }

            string houseName = nameParts[0];
            string partWithIndex = nameParts[1];
            string partType = new string(partWithIndex.TakeWhile(char.IsLetter).ToArray());
            string index = new string(partWithIndex.SkipWhile(char.IsLetter).ToArray());

            // Проверить, принадлежит ли дом текущей сцене
            if (!houseNamesOnScene.Contains(houseName))
            {
                Debug.LogWarning($"Материал {material.name} пропущен, так как дом {houseName} не найден на текущей сцене.");
                continue;
            }

            var housePart = houseMaterialManager.HouseParts.Find(part => part.HouseName == houseName && part.PartType == partType && part.Index == index);
            if (housePart == null)
            {
                housePart = new HousePart(houseName, partType, index);
                houseMaterialManager.HouseParts.Add(housePart);
            }

            // Добавляем материал без генерации отображаемого имени
            housePart.Materials.Add(new MaterialInfo(null, material));
        }

        Debug.Log("Материалы успешно спарсены и добавлены в структуру.");
    }
}
#endif
