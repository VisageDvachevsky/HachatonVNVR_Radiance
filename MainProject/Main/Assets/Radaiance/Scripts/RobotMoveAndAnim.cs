using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class RobotMoveAndAnim : MonoBehaviour
{
    [Header("Movement Settings")]
    public float distanceThreshold = 0.5f;
    public float curveAmount = 2f;
    public int pathPoints = 10;

    [Header("References")]
    private Transform player;
    private Animator anim;
    private NavMeshAgent agent;
    private CharacterController playerController;

    private Vector3[] path;
    private int currentPathIndex;
    private bool isMoving = false;
    private bool isWaitingForAnim = false;
    private bool isReturning = false;

    [SerializeField] private Shader fadeOutShader;

    private void Awake()
    {
        InitializeComponents();
        agent.enabled = false;
    }

    private void Update()
    {
        if (isWaitingForAnim) CheckAnimationCompletion();
        if (isMoving) HandleMovement();
    }

    private void InitializeComponents()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerController = player?.GetComponent<CharacterController>();

        if (player == null || anim == null || agent == null || playerController == null)
        {
            Debug.LogError("Missing required components or Player not found!");
            enabled = false;
        }
    }

    private void CheckAnimationCompletion()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("closed_Roll_Loop") && stateInfo.normalizedTime >= 1f)
        {
            StartMoving();
            isWaitingForAnim = false;
        }
    }

    private void HandleMovement()
    {
        if (currentPathIndex < path.Length && Vector3.Distance(transform.position, path[currentPathIndex]) < distanceThreshold)
        {
            currentPathIndex++;

            if (currentPathIndex >= path.Length)
            {
                StopMovement();
                if (isReturning) Destroy(gameObject);
                else SpawnPanel();
            }
            else
            {
                agent.SetDestination(path[currentPathIndex]);
            }
        }
    }

    public void StartMovement()
    {
        anim.SetBool("Roll_Anim", true);
        isWaitingForAnim = true;
    }

    private void StartMoving()
    {
        agent.enabled = true;
        isMoving = true;
       


        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.position + playerController.transform.forward * 1.5f;

        path = GenerateCurvedPath(startPosition, targetPosition);
        currentPathIndex = 0;

        if (path.Length > 0)
        {
            agent.SetDestination(path[currentPathIndex]);
        }
    }

    private void StopMovement()
    {
        isMoving = false;
        agent.enabled = false;
        anim.SetBool("Roll_Anim", false);
    }

    private Vector3[] GenerateCurvedPath(Vector3 startPosition, Vector3 targetPosition)
    {
        List<Vector3> curvedPath = new List<Vector3>();

        Vector3 direction = (targetPosition - startPosition).normalized;
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x);

        for (int i = 0; i < pathPoints; i++)
        {
            float t = i / (float)(pathPoints - 1);

            float curveFactor = Mathf.Clamp01(1 - Mathf.Abs(t - 0.5f) * 2) * curveAmount; 
            Vector3 pointOnCurve = Vector3.Lerp(startPosition, targetPosition, t);
            pointOnCurve += perpendicular * curveFactor;
            pointOnCurve.y = startPosition.y;

            if (NavMesh.SamplePosition(pointOnCurve, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                curvedPath.Add(hit.position);
            }
        }

        return curvedPath.ToArray();
    }


    public void SpawnPanel()
    {
        PanelSpawner panelSpawner = GameObject.FindGameObjectWithTag("Manager")?.GetComponent<PanelSpawner>();

        if (panelSpawner != null)
        {
            panelSpawner.SpawnPanel();
        }
        else
        {
            Debug.LogError("PanelSpawner not found or not assigned!");
        }
    }

    public void ReturnPath()
    {
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Path is not initialized. Cannot return.");
            return;
        }

        System.Array.Reverse(path);
        currentPathIndex = 0;

        anim.SetBool("Roll_Anim", true);
        isWaitingForAnim = true;

        StartCoroutine(StartReturnAfterAnim());
    }

    private IEnumerator FadeAndDestroy(GameObject targetObject, float fadeDuration)
    {
        if (fadeOutShader == null)
        {
            Debug.LogError("FadeOutShader not assigned.");
            yield break;
        }

        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("No renderers found on the target object or its children.");
            yield break;
        }

        Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
        foreach (var renderer in renderers)
        {
            originalMaterials[renderer] = renderer.materials;

            Material[] fadeMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                fadeMaterials[i] = new Material(fadeOutShader)
                {
                    mainTexture = renderer.materials[i].mainTexture
                };
            }
            renderer.materials = fadeMaterials;
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float fade = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (material.HasProperty("_Fade")) material.SetFloat("_Fade", fade);
                }
            }
            yield return null;
        }

        Destroy(targetObject);
    }

    private IEnumerator StartReturnAfterAnim()
    {
        yield return FadeAndDestroy(gameObject, 2f);

        while (isWaitingForAnim) yield return null;

        agent.enabled = true;
        isMoving = true;
        agent.SetDestination(path[currentPathIndex]);
        isReturning = true;
    }

    private void OnDrawGizmos()
    {
        if (path == null || path.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
        }
    }

}
