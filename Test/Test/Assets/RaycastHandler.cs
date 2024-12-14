using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RaycastHandler : MonoBehaviour
{
    [Header("Настройки рейкаста")]
    public Camera PlayerCamera;                 
    public LayerMask InteractableLayer;        
    public LayerMask UILayer;                  

    [Header("Панель взаимодействия")]
    public GameObject InteractionPanelPrefab;  
    private GameObject currentPanel;            
    private HousePart currentPart;              

    void Update()
    {
        HandleRaycast();
    }

    void HandleRaycast()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if (IsPointerOverUI()) return;

            Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, InteractableLayer))
            {
                currentPart = hit.collider.GetComponent<HousePartComponents>()?.HousePart;
                if (currentPart != null)
                {
                    SpawnInteractionPanel();
                }
            }
        }
    }

    void SpawnInteractionPanel()
    {
        if (currentPanel != null) Destroy(currentPanel);

        Vector3 spawnPosition = PlayerCamera.transform.position + PlayerCamera.transform.forward * 2.0f;
        spawnPosition.y -= 0.5f;
        currentPanel = Instantiate(InteractionPanelPrefab, spawnPosition, Quaternion.identity);

        currentPanel.transform.LookAt(PlayerCamera.transform);
        currentPanel.transform.rotation = Quaternion.Euler(0, currentPanel.transform.rotation.eulerAngles.y + 180, 0);

        SetLayerRecursively(currentPanel, LayerMask.NameToLayer("UI"));

        GenerateDropdownAndButtons();
    }

    void GenerateDropdownAndButtons()
    {
        Dropdown dropdown = GameObject.FindWithTag("PanelDropdown").GetComponent<Dropdown>();
        Button confirmButton = GameObject.FindWithTag("ConfrimButton").GetComponent<Button>();
        Button resetButton = GameObject.FindWithTag("ResetButton").GetComponent<Button>();

        if (dropdown != null)
        {
            dropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (var material in currentPart.AvailableMaterials)
            {
                options.Add(material.name);
            }
            dropdown.AddOptions(options);

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                int selectedIndex = dropdown.value;
                currentPart.SetMaterial(currentPart.AvailableMaterials[selectedIndex]);
                Destroy(currentPanel);
            });

            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(() =>
            {
                currentPart.SetMaterial(currentPart.DefaultMaterial);
                Destroy(currentPanel);
            });
        }
    }

    bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}