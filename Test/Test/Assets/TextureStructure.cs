using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HousePart
{
    [Tooltip("Уникальный ID объекта")]
    public string PartID;

    [Tooltip("GameObject на сцене")]
    public GameObject TargetObject;

    [Tooltip("Материал по умолчанию")]
    public Material DefaultMaterial;

    [Tooltip("Список доступных материалов")]
    public List<Material> AvailableMaterials;

    [HideInInspector]
    public Material CurrentMaterial;

    public void ParseCurrentMaterial()
    {
        if (TargetObject != null && TargetObject.GetComponent<Renderer>() != null)
        {
            CurrentMaterial = TargetObject.GetComponent<Renderer>().sharedMaterial;
        }
        else
        {
            Debug.LogWarning($"Renderer не найден у объекта {PartID}");
        }
    }

    public void SetMaterial(Material newMaterial)
    {
        if (AvailableMaterials.Contains(newMaterial) && TargetObject != null)
        {
            TargetObject.GetComponent<Renderer>().material = newMaterial;
            CurrentMaterial = newMaterial;
        }
        else
        {
            Debug.LogWarning($"Материал недоступен для объекта {PartID}.");
        }
    }
}