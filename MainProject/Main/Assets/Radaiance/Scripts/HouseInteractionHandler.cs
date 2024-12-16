using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.EventSystems;

public class HouseInteractionHandler : MonoBehaviour
{
    public GameObject UIPanelPrefab;
    private GameObject spawnedPanel;
    private HouseMaterialManager materialManager;
    [SerializeField] private GameObject selectedHousePart;
    private Transform playerTransform;

    [SerializeField] private InputActionProperty selectAction;

    [SerializeField] private Dropdown dropdown;
    [SerializeField] private Image previewImage;
    [SerializeField] private Button confirmButton;

    private LayerMask raycastMask;

    private Dictionary<string, Material> displayNameToMaterialMap;


    void Start()
    {
        GameObject managerObj = GameObject.FindWithTag("Manager");
        if (managerObj != null)
        {
            materialManager = managerObj.GetComponent<HouseMaterialManager>();
        }
        else
        {
            Debug.LogError("Manager с тегом Manager не найден!");
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("Объект с тегом Player не найден!");
        }

        selectAction.action.started += OnSelectAction;

        int uiLayer = LayerMask.NameToLayer("UI");
        raycastMask = ~LayerMask.GetMask("UI");

        selectAction.action.started += OnSelectAction;
    }

    private void OnDestroy()
    {
        selectAction.action.started -= OnSelectAction;
    }

    private void OnSelectAction(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Взаимодействие с UI, пропускаем обработку.");
            return;
        }

        XRRayInteractor rayInteractor = GetComponent<XRRayInteractor>();
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("UI"))
            {
                OnRaycastHit(hitObject);
            }

            if (hitObject.CompareTag("ColorPicker"))
            {
                HandleColorPickerInteraction(hitObject, hit);
            }
        }
    }

    private void HandleColorPickerInteraction(GameObject hitObject, RaycastHit hit)
    {
        ColorPicker colorPicker = hitObject.GetComponent<ColorPicker>();
        if (colorPicker == null) return;

        RectTransform rectTransform = hitObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("Объект с тегом ColorPicker не имеет RectTransform.");
            return;
        }

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            hit.point,
            Camera.main,
            out localPoint))
        {
            colorPicker.SetPointerPosition(localPoint);
            Debug.Log($"Колорпикер обновлен по точке: {localPoint}");
        }
    }


    public void OnRaycastHit(GameObject hitObject)
    {
        if (hitObject != null && materialManager != null)
        {
            string[] nameParts = hitObject.name.Split('_');
            if (nameParts.Length < 2) return;

            string houseName = nameParts[0];
            string partWithIndex = nameParts[1];
            string partType = new string(partWithIndex.TakeWhile(char.IsLetter).ToArray());
            string index = new string(partWithIndex.SkipWhile(char.IsLetter).ToArray());

            HousePart housePart = materialManager.HouseParts.Find(part =>
                part.HouseName == houseName && part.PartType == partType && part.Index == index);

            if (housePart != null)
            {
                selectedHousePart = hitObject;

                if (spawnedPanel != null)
                {
                    Destroy(spawnedPanel);
                }

                SpawnUIPanel(housePart);
            }
        }
    }

    private void SpawnUIPanel(HousePart housePart)
    {
        if (playerTransform == null || selectedHousePart == null || UIPanelPrefab == null)
        {
            Debug.LogError("Не все необходимые объекты заданы (Player, UIPanelPrefab или выбранная часть дома).");
            return;
        }

        if (spawnedPanel != null)
        {
            Destroy(spawnedPanel);
        }

        spawnedPanel = Instantiate(UIPanelPrefab);

        dropdown = spawnedPanel.GetComponentInChildren<Dropdown>();
        previewImage = spawnedPanel.GetComponentsInChildren<Image>()
                                .FirstOrDefault(img => img.gameObject.name == "PreviewImage");
        confirmButton = spawnedPanel.transform.Find("Buttons/Text Button").GetComponent<Button>();

        if (confirmButton != null)
        {
            Debug.Log("Кнопка найдена!");
        }
        else
        {
            Debug.LogWarning("Кнопка не найдена!");
        }


        if (dropdown != null)
        {
            PopulateDropdown(housePart);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(() => ApplySelectedMaterial(housePart));
        }

        Vector3 housePartPosition = selectedHousePart.transform.position;
        Vector3 playerPosition = playerTransform.position;

        Vector3 spawnPosition = Vector3.Lerp(playerPosition, housePartPosition, 0.5f);
        spawnPosition.y += 0.5f;

        spawnedPanel.transform.position = spawnPosition;

        Vector3 directionToPlayer = playerTransform.position - spawnedPanel.transform.position;
        directionToPlayer.y = 0;

        if (directionToPlayer.sqrMagnitude < 0.001f)
            directionToPlayer = Vector3.forward;

        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        spawnedPanel.transform.rotation = lookRotation;
    }

    private void PopulateDropdown(HousePart housePart)
    {
        if (dropdown == null) return;

        dropdown.ClearOptions();
        displayNameToMaterialMap = new Dictionary<string, Material>();

        List<string> displayNames = new List<string> { "Выберите текстуру" };
        foreach (var matInfo in housePart.Materials)
        {
            if (matInfo == null || matInfo.Material == null) continue;

            string displayName = matInfo.DisplayName ?? matInfo.Material.name;
            displayNameToMaterialMap[displayName] = matInfo.Material;
            displayNames.Add(displayName);
        }

        dropdown.AddOptions(displayNames);

        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener(index =>
        {
            Debug.Log($"Выбрана опция: {dropdown.options[index].text}");
        });

        dropdown.value = 0;
    }

    private void ApplySelectedMaterial(HousePart housePart)
    {
        if (dropdown == null || selectedHousePart == null) return;

        string selectedDisplayName = dropdown.options[dropdown.value].text;
        print(selectedDisplayName);

        if (selectedDisplayName == "Выберите текстуру")
        {
            ColorPicker colorPicker = FindObjectOfType<ColorPicker>();
            if (colorPicker == null)
            {
                Debug.LogWarning("Компонент ColorPicker не найден.");
                return;
            }

            Color selectedColor = colorPicker.color;

            Material material = selectedHousePart.GetComponent<Renderer>()?.material;
            if (material != null)
            {
                material.color = selectedColor;
                Debug.Log($"Применён цвет {selectedColor} к {selectedHousePart.name}");
            }
            else
            {
                Debug.LogWarning("Материал у выбранной части дома отсутствует.");
            }
        }
        else
        {
            if (!displayNameToMaterialMap.TryGetValue(selectedDisplayName, out Material selectedMaterial))
            {
                Debug.LogWarning($"Материал с отображаемым именем {selectedDisplayName} не найден.");
                return;
            }

            materialManager.SetMaterial(selectedHousePart, selectedDisplayName);
            Debug.Log($"Материал с отображаемым именем {selectedDisplayName} применён к {selectedHousePart.name}");
        }
    }



}
