using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelInfo : SceneItemInfo
{
    public float MoveSpeed { get; set; }
    public int GroundY { get; set; }
    public float Dir { get; set; }
    public bool IsFloat { get; set; }
    public int Item { get; set; }
}