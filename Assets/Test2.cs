using System.Security.Policy;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public AudioSource m_AudioSource;
    public AudioClip clip;
    public void Awake()
    {
        // IntPtr.
       
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            //m_AudioSource.PlayOneShot(clip, 1.0f);// Random.Range(0.01f, 1.0f));
            m_AudioSource.PlayOneShot(clip, 1.0f);// Random.Range(0.01f, 1.0f));
            Debug.Log(m_AudioSource.isPlaying);
        }
    }
}