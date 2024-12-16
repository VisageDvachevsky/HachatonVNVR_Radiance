using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialInfo
{
    public string DisplayName; 
    public Material Material;

    public MaterialInfo(string displayName, Material material)
    {
        DisplayName = displayName;
        Material = material;
    }
}

[System.Serializable]
public class HousePart
{
    public string HouseName;   
    public string PartType;       
    public string Index;        
    public List<MaterialInfo> Materials = new List<MaterialInfo>(); 

    public HousePart(string houseName, string partType, string index)
    {
        HouseName = houseName;
        PartType = partType;
        Index = index;
    }
}