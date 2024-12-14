using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [Header("Список частей дома")]
    public List<HousePart> HouseParts;

    void Start()
    {
        // Парсим текущие материалы при старте сцены
        foreach (var part in HouseParts)
        {
            part.ParseCurrentMaterial();
        }
    }

    // Метод для смены материала по ID части
    public void ChangeMaterial(string partID, Material newMaterial)
    {
        HousePart part = HouseParts.Find(p => p.PartID == partID);
        if (part != null)
        {
            part.SetMaterial(newMaterial);
        }
        else
        {
            Debug.LogWarning($"Объект с ID {partID} не найден.");
        }
    }
}