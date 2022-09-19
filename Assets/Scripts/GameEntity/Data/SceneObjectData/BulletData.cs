using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletData : BaseSceneObjectData
{
    public int bulletIndex { get; set; }
    public string normalAnim { get; set; }
    public string hitAnim { get; set; }
    public float normalAnimSpeed { get; set; }
    public float hitAnimSpeed { get; set; }
    public Vector2 dir { get; set; }
    public Vector2 pos { get; set; }
    public Vector2 velocity { get; set; }
    public float hitRange { get; set; }
    public float drag { get; set; }
    public bool isPenatrate { get; set; }//是否穿透

    public override void Clear()
    {
        base.Clear();
        bulletIndex = 0;
        normalAnim = string.Empty;
        hitAnim = string.Empty;
        normalAnimSpeed = 0;
        hitAnimSpeed = 0;
        dir = Vector2.zero;
        pos = Vector2.zero;
        velocity = Vector2.zero;
        hitRange = 0;
        drag = 0;
        isPenatrate = false;
    }
}
