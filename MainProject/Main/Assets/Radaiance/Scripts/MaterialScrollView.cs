using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialScrollView : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;       
    [SerializeField] private Transform selectedHousePart;

    private List<HousePart> houseParts; 

    private void Start()
    {

        // Получаем данные из менеджера
        GameObject managerObject = GameObject.FindGameObjectWithTag("Manager");
        if (managerObject == null)
        {
            Debug.LogError("Объект с тегом Manager не найден!");
            return;
        }

        houseParts = managerObject.GetComponent<HouseMaterialManager>()?.GetAllHouseParts();
        if (houseParts == null || houseParts.Count == 0)
        {
            Debug.LogError("Данные о частях дома не найдены!");
            return;
        }
    }

}
