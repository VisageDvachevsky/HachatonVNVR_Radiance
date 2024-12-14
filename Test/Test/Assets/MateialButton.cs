using UnityEngine;

public class MaterialButton : MonoBehaviour
{
    public MaterialManager Manager;  
    public string TargetPartID;      
    public Material MaterialToApply; 

    public void OnButtonClick()
    {
        Manager.ChangeMaterial(TargetPartID, MaterialToApply);
    }
}