using UnityEngine;
using System.Collections;

public class PanelSpawner : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject panelPrefab;
    public Material panelMaterial;

    [Header("Hologram Settings")]
    public float blendDuration = 6f;
    public float minLineFrequency = 2f;
    public float maxLineFrequency = 100f;
    public float rotationSpeed = 15f;

    private GameObject currentPanel;
    private Renderer panelRenderer;
    private Material hologramMaterial;
    private Rigidbody panelRigidbody;

    private ParticleSystem hologramPS;

    private bool isGrabbed = false;

    public void SpawnPanel()
    {
        Transform placeholder = GameObject.FindGameObjectWithTag("TabletPlaceholder")?.transform;

        if (placeholder == null)
        {
            Debug.LogError("TabletPlaceholder not found! Ensure the tag is correctly set.");
            return;
        }

        currentPanel = Instantiate(panelPrefab, transform);
        Vector3 panelSize = currentPanel.GetComponentInChildren<Renderer>().bounds.size;
        currentPanel.transform.position = placeholder.position + new Vector3(0, panelSize.y / 2, 0);

        panelRenderer = currentPanel.GetComponentInChildren<MeshRenderer>();

        if (panelRenderer == null)
        {
            Debug.LogError("Panel does not have a MeshRenderer component!");
            return;
        }

        hologramMaterial = panelRenderer.material;
        panelRigidbody = currentPanel.GetComponentInChildren<Rigidbody>();

        if (panelRigidbody != null)
        {
            panelRigidbody.isKinematic = true;
        }

        hologramPS = GameObject.FindGameObjectWithTag("RobotPS")?.GetComponent<ParticleSystem>();

        if (hologramPS == null)
        {
            Debug.LogError("Hologram Particle System (RobotPS) not found!");
        }
        else
        {
            var mainModule = hologramPS.main;
            mainModule.duration = blendDuration;

            hologramPS.Play();
        }

        StartCoroutine(TransitionToPanelMaterial());
        StartCoroutine(RotateWhileNotGrabbed());
    }

    public void GrabPanel()
    {
        isGrabbed = true;

        if (panelRigidbody != null)
        {
            panelRigidbody.isKinematic = false;
        }

        if (hologramPS != null)
        {
            hologramPS.Stop();
        }

        RobotMoveAndAnim robot = GameObject.FindGameObjectWithTag("Robot")?.GetComponent<RobotMoveAndAnim>();

        if (robot != null)
        {
            robot.ReturnPath();
        }
        else
        {
            Debug.LogWarning("RobotMoveAndAnim not found or not assigned!");
        }
    }

    public void UngrabPanel()
    {
        isGrabbed = false;

        if (panelRigidbody != null)
        {
            panelRigidbody.isKinematic = true;
        }

        if (currentPanel != null)
        {
            currentPanel.transform.rotation = Quaternion.identity;
        }

        StartCoroutine(RotateWhileNotGrabbed());
    }

    private IEnumerator TransitionToPanelMaterial()
    {
        float elapsedTime = 0f;

        while (elapsedTime < blendDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / blendDuration;

            if (hologramMaterial != null)
            {
                hologramMaterial.SetFloat("_LineFrequency", Mathf.Lerp(maxLineFrequency, minLineFrequency, progress));
                hologramMaterial.SetFloat("_BlendProgress", progress);
            }

            yield return null;
        }

        if (panelRenderer != null && panelMaterial != null)
        {
            panelRenderer.material = panelMaterial;
        }

        if (hologramPS != null)
        {
            hologramPS.Stop();
        }
    }

    private IEnumerator RotateWhileNotGrabbed()
    {
        while (!isGrabbed && currentPanel != null)
        {
            currentPanel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
}
