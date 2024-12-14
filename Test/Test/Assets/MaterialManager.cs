using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [Header("������ ������ ����")]
    public List<HousePart> HouseParts;

    void Start()
    {
        // ������ ������� ��������� ��� ������ �����
        foreach (var part in HouseParts)
        {
            part.ParseCurrentMaterial();
        }
    }

    // ����� ��� ����� ��������� �� ID �����
    public void ChangeMaterial(string partID, Material newMaterial)
    {
        HousePart part = HouseParts.Find(p => p.PartID == partID);
        if (part != null)
        {
            part.SetMaterial(newMaterial);
        }
        else
        {
            Debug.LogWarning($"������ � ID {partID} �� ������.");
        }
    }
}