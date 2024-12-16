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
            Debug.LogError($"������ {houseElement.name} �� ����� ���������� Renderer.");
            return;
        }

        string[] nameParts = houseElement.name.Split('_');
        if (nameParts.Length < 2)
        {
            Debug.LogError($"�������� ������� {houseElement.name} �� ������������� �������.");
            return;
        }

        string houseName = nameParts[0];
        string partType = nameParts[1];

        var housePart = HouseParts.Find(part => part.HouseName == houseName && part.PartType == partType);
        if (housePart == null)
        {
            Debug.LogError($"����� ���� � ������ {houseName}_{partType} �� ������� � ���������.");
            return;
        }

        var materialInfo = housePart.Materials.FirstOrDefault(mat => mat.DisplayName == displayName);

        if (materialInfo == null)
        {
            Debug.LogError($"�������� � ������������ ������ {displayName} �� ������ ��� {houseName}_{partType}");
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
            Debug.LogError($"����� ���� � ������ {houseName}_{partType} �� �������.");
            return null;
        }

        return housePart.Materials.ConvertAll(matInfo => matInfo.DisplayName);
    }
}