using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInfo : BaseSceneObjectInfo
{
    public bool IsSmoon { get; set; }
    public Vector2 AddTargetForce { get; set; }
    public string NormalAnim { get; set; }
    public string HitAnim { get; set; }
    public float NormalAnimSpeed { get; set; }
    public float HitAnimSpeed { get; set; }
    public Vector2 Dir { get; set; }
    public Vector2 Pos { get; set; }
    public Vector2 Velocity { get; set; }
    public float HitRange { get; set; }
    public float Drag { get; set; }
    public Vector2 TriggerOffest { get; set; }
    public Vector2 TriggerSize { get; set; }
    public bool IsPenatrate { get; set; }//是否穿透
    public int SkillExp { get; set; }
}
