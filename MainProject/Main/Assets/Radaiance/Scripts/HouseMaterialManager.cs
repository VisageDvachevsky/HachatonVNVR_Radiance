using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HouseMaterialManager : MonoBehaviour
{
    public List<HousePart> HouseParts = new List<HousePart>();

    public void SetMaterial(GameObject houseElement, string displayName)
    {
        var renderer = houseElement.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError($"Объект {houseElement.name} не имеет компонента Renderer.");
            return;
        }

        string[] nameParts = houseElement.name.Split('_');
        if (nameParts.Length < 2)
        {
            Debug.LogError($"Название объекта {houseElement.name} не соответствует формату.");
            return;
        }

        string houseName = nameParts[0];
        string partType = nameParts[1];

        var housePart = HouseParts.Find(part => part.HouseName == houseName && part.PartType == partType);
        if (housePart == null)
        {
            Debug.LogError($"Часть дома с именем {houseName}_{partType} не найдена в структуре.");
            return;
        }

        var materialInfo = housePart.Materials.FirstOrDefault(mat => mat.DisplayName == displayName);

        if (materialInfo == null)
        {
            Debug.LogError($"Материал с отображаемым именем {displayName} не найден для {houseName}_{partType}");
            return;
        }

        renderer.material = materialInfo.Material;
    }

    public List<HousePart> GetAllHouseParts()
    {
        return HouseParts;
    }

    public List<string> GetMaterialDisplayNames(string houseName, string partType)
    {
        var housePart = HouseParts.Find(part => part.HouseName == houseName && part.PartType == partType);
        if (housePart == null)
        {
            Debug.LogError($"Часть дома с именем {houseName}_{partType} не найдена.");
            return null;
        }

        return housePart.Materials.ConvertAll(matInfo => matInfo.DisplayName);
    }
}