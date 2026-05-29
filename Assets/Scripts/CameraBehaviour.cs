using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 pos = player.transform.position;

            Camera.main.transform.position = new Vector3(pos.x + 0.07f, pos.y + 1.675f, pos.z + 0.103f);
        }
    }
}
