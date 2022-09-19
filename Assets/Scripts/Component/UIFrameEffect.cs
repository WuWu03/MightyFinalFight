using System.Collections;
using System.Collections.Generic;
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
        m_FramteTimer = 0;
        m_ImgSprite.sprite = sprites[0];
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_IsStop || sprites == null || sprites.Length < 1)
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

        m_FramteTimer += Time.deltaTime;

        if(m_FramteTimer >= m_PreFrameTime)
        {
            m_FramteTimer = 0;
            m_FrameIndex++;
            m_ImgSprite.sprite = sprites[m_FrameIndex];
        }
    }

    public void StartFrame()
    {
        m_IsStop = false;
    }

    public void StopFrame()
    {
        m_IsStop = true;
    }

    private bool m_IsStop = false;
    private float m_PreFrameTime = 0;
    private float m_FramteTimer = 0;
    private int m_FrameIndex = 0;
    private Image m_ImgSprite = null;
}
