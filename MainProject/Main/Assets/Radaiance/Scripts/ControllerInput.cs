using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    public InputActionProperty buttonBAction;
    private bool buttonBPressed = false;

    public GameObject spawnPrefab;
    public float spawnDistance = 2.0f;
    private Transform playerTransform;

    private GameObject currentSpawnedObject;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player с тегом 'Player' не найден!");
        }
    }

    void Update()
    {
        float buttonValue = buttonBAction.action.ReadValue<float>();

        if (buttonValue > 0.5f && !buttonBPressed)
        {
            SpawnOrReplaceObject();
            buttonBPressed = true;
        }
        else if (buttonValue <= 0.5f)
        {
            buttonBPressed = false;
        }
    }

    void SpawnOrReplaceObject()
    {
        GameObject tabletParentObject = GameObject.FindGameObjectWithTag("TabletParent");
        if (tabletParentObject != null)
        {
            Destroy(tabletParentObject);
            Debug.Log("Объект с тегом 'TabletParent' удалён.");
        }

        if (currentSpawnedObject != null)
        {
            Destroy(currentSpawnedObject);
            Debug.Log("Старый объект удалён.");
        }

        Vector3 spawnPosition = playerTransform.position - playerTransform.forward * spawnDistance;
        currentSpawnedObject = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Новый объект заспавнен за спиной игрока.");

        StartMovementForSpawnedObject();
    }

    void StartMovementForSpawnedObject()
    {
        if (currentSpawnedObject != null)
        {
            RobotMoveAndAnim movementComponent = currentSpawnedObject.GetComponent<RobotMoveAndAnim>();

            if (movementComponent != null)
            {
                movementComponent.StartMovement();
                Debug.Log("Движение заспавненного объекта запущено.");
            }
            else
            {
                Debug.LogError("У заспавненного объекта отсутствует компонент RobotMoveAndAnim.");
            }
        }
    }
}