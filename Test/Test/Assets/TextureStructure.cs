using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HousePart
{
    [Tooltip("���������� ID �������")]
    public string PartID;

    [Tooltip("GameObject �� �����")]
    public GameObject TargetObject;

    [Tooltip("�������� �� ���������")]
    public Material DefaultMaterial;

    [Tooltip("������ ��������� ����������")]
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
            Debug.LogWarning($"Renderer �� ������ � ������� {PartID}");
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
            Debug.LogWarning($"�������� ���������� ��� ������� {PartID}.");
        }
    }
}