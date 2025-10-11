using UnityEngine;
using UnityEngine.UI;
public class UIFrameEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] sprites;
    public bool isLoop = true;
    public int frameRate = 24;
    
    private void Awake()
    {
        if(sprites == null || sprites.Length < 1)
        {
            return;
        }

        m_ImgSprite = GetComponent<Image>();
        m_PreFrameTime = (float)1 / frameRate;
        m_ImgSprite.sprite = sprites[0];
    }

    private void OnEnable()
    {
        m_FrameIndex = 0;
        m_FrameTimer = 0;
        m_ImgSprite.sprite = sprites[0];
        StartFrame();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_IsPlaying || sprites == null || sprites.Length < 1)
        {
            return;
        }

        if (m_FrameIndex >= sprites.Length - 1)
        {
            if (!isLoop)
            {
                return;
            }

            m_FrameIndex = -1;
        }

        m_FrameTimer += Time.deltaTime;

        if (m_FrameTimer < m_PreFrameTime)
        {
            return;
        }
        
        m_FrameTimer = 0;
        m_FrameIndex++;
        m_ImgSprite.sprite = sprites[m_FrameIndex];
    }

    public void StartFrame()
    {
        m_IsPlaying = true;
    }

    public void StopFrame()
    {
        m_IsPlaying = false;
    }

    private bool m_IsPlaying = false;
    private float m_PreFrameTime = 0;
    private float m_FrameTimer = 0;
    private int m_FrameIndex = 0;
    private Image m_ImgSprite = null;
}
