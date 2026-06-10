using UnityEngine;


public class Test2 : MonoBehaviour
{
    public float vol = 0f;

    private void Awake()
    {
        this.GetComponent<Rigidbody2D>().linearVelocityY = vol;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log(this.GetComponent<Rigidbody2D>().linearVelocityY);
            transform.position = Vector3.zero;
            this.GetComponent<Rigidbody2D>().linearVelocityY = vol;
        }
    }
}