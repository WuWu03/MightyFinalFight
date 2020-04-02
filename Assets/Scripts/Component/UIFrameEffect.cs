using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIFrameEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] Sprites;
    public bool IsLoop = true;
    public int FrameRate = 24;
    private float m_PreFrameTime = 0;
    private float m_FramteTimer = 0;
    private int m_FrameIndex = 0;
    private Image m_ImgSprite = null;

    private void Awake()
    {
        if(Sprites == null || Sprites.Length < 1)
        {
            return;
        }

        m_ImgSprite = GetComponent<Image>();
        m_PreFrameTime = (float)1 / FrameRate;
        m_ImgSprite.sprite = Sprites[0];
    }

    private void OnEnable()
    {
        m_FrameIndex = 0;
        m_FramteTimer = 0;
        m_ImgSprite.sprite = Sprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Sprites == null || Sprites.Length < 1)
        {
            return;
        }

        if (m_FrameIndex >= Sprites.Length - 1)
        {
            if (!IsLoop) return;
            m_FrameIndex = -1;



        }

        m_FramteTimer += Time.deltaTime;

        if(m_FramteTimer >= m_PreFrameTime)
        {
            m_FramteTimer = 0;
            m_FrameIndex++;
            m_ImgSprite.sprite = Sprites[m_FrameIndex];
        }
    }
}
