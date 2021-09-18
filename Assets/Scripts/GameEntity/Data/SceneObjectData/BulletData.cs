using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletData : BaseSceneObjectData
{
    public int BulletIndex { get; set; }
    public string NormalAnim { get; set; }
    public string HitAnim { get; set; }
    public float NormalAnimSpeed { get; set; }
    public float HitAnimSpeed { get; set; }
    public Vector2 Dir { get; set; }
    public Vector2 Pos { get; set; }
    public Vector2 Velocity { get; set; }
    public float HitRange { get; set; }
    public float Drag { get; set; }
    public bool IsPenatrate { get; set; }//是否穿透

    public override void Clear()
    {
        base.Clear();
        BulletIndex = 0;
        NormalAnim = string.Empty;
        HitAnim = string.Empty;
        NormalAnimSpeed = 0;
        HitAnimSpeed = 0;
        Dir = Vector2.zero;
        Pos = Vector2.zero;
        Velocity = Vector2.zero;
        HitRange = 0;
        Drag = 0;
        IsPenatrate = false;
    }
}
