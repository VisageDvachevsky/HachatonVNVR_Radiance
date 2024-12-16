using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public GameObject[] clouds;
    public float speed = 2.0f;
    public float resetX = -50.0f; 
    public float startX = 50.0f; 
    public float minY = 10.0f; 
    public float maxY = 20.0f; 

    void Update()
    {
        foreach (GameObject cloud in clouds)
        {
            cloud.transform.Translate(Vector3.left * speed * Time.deltaTime);

            if (cloud.transform.position.x < resetX)
            {
                cloud.transform.position = new Vector3(
                    startX,
                    Random.Range(minY, maxY), 
                    cloud.transform.position.z
                );
            }
        }
    }
}
