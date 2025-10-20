using UnityEngine;

//图层交错效果
public class InterlacingOfLayers : MonoBehaviour
{
    public float moveRate;//移动幅度                
    public bool lockY;//Y轴是否移动,需要Y轴移动不用勾,不需要就勾                      

    // Start is called before the first frame update
    private void Start()
    {
        m_StartPointX = transform.position.x;
        m_StartPointX = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float cameraPosX = CameraMgr.instance.cameraRoot.transform.position.x;
        float cameraPosY = CameraMgr.instance.cameraRoot.transform.position.y;
        //如果Y轴不移动
        if (lockY)
        {
            //当前挂在的对象的坐标 = 新的二维向量坐标(开始坐标 + 摄像机的x坐标 * 移动幅度 , y轴不变)
            transform.position = new Vector2(m_StartPointX + cameraPosX * moveRate, transform.position.y);
        }
        //移动X,Y轴
        else
        {
            transform.position = new Vector2(m_StartPointY + cameraPosX * moveRate, m_StartPointY + cameraPosY * moveRate);
        }
    }

    private float m_StartPointX;//开始的X点,Y点
    private float m_StartPointY; 
}