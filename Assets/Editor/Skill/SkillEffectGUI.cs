using GameFrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillEffectGUI : SkillGUI
{
    public SkillEffectGUI(EditorWindow window) : base(window)
    {
        m_ListSkillEffect = new List<SkillConfigData.SkillEffect>();
    }

    protected override void OnUpdateData()
    {
        base.OnUpdateData();
        CloneEffects();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
        EditorGUILayout.Space(10f);
        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
        for (int i = 0; i < SkillHelper.CurrConfigData.SkillEffects.Length; i++)
        {
            SkillConfigData.SkillEffect skillEffect = SkillHelper.CurrConfigData.SkillEffects[i];
            SkillConfigData.SkillEffect tempEffect = m_ListSkillEffect[i];

            GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label((i + 1).ToString() + ".");
                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    Util.DeleteElement(SkillHelper.CurrConfigData.SkillEffects, i);
                    m_ListSkillEffect.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();

                skillEffect.EffectorType = (SkillConfigData.SkillEffectorType)EditorGUILayout.EnumPopup("EffectorType", skillEffect.EffectorType);
                skillEffect.SelectorType = (SkillConfigData.SkillSelectorType)EditorGUILayout.EnumPopup("SelectorType", skillEffect.SelectorType);

                if (skillEffect.EffectorType != SkillConfigData.SkillEffectorType.BulletHitEffect)
                {
                    NormalEffectGUI(skillEffect, tempEffect);
                }
                else
                {
                    GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
                    {
                        BulletEffectGUI(skillEffect, tempEffect);
                    });
                }
         
                EditorGUILayout.EndVertical();
            });
        }

        EditorGUILayout.EndScrollView();


        GUILayout.FlexibleSpace();

        if (GUILayout.Button("增加技能效果器"))
        {
            m_ListSkillEffect.Add(new SkillConfigData.SkillEffect());
            Util.AddElement(SkillHelper.CurrConfigData.SkillEffects, new SkillConfigData.SkillEffect());
            return;
        }
    }

    private void BulletEffectGUI(SkillConfigData.SkillEffect skillEffect, SkillConfigData.SkillEffect tempEffect)
    {
        for (int i = 0; i < skillEffect.Bullets.Length; i++)
        {
            SkillConfigData.Bullet skillBullet = skillEffect.Bullets[i];
            SkillConfigData.Bullet tempBullet = tempEffect.Bullets[i];

            EditorGUILayout.BeginHorizontal();
            tempBullet.Name = EditorGUILayout.TextField("Name", tempBullet.Name);
            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                skillBullet.Name = tempBullet.Name;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void NormalEffectGUI(SkillConfigData.SkillEffect skillEffect, SkillConfigData.SkillEffect tempEffect)
    {
        skillEffect.ForceType = (SkillConfigData.SkillAddForceType)EditorGUILayout.EnumPopup("ForceType", skillEffect.ForceType);

        EditorGUILayout.BeginHorizontal();
        tempEffect.SelectorOffest = EditorGUILayout.Vector2Field("SelectorOffest", tempEffect.SelectorOffest);
        if (GUILayout.Button("更改", GUILayout.Width(100), GUILayout.Height(40)))
        {
            skillEffect.SelectorOffest = tempEffect.SelectorOffest;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.AddTargetForce = EditorGUILayout.Vector2Field("AddTargetForce", tempEffect.AddTargetForce);
        if (GUILayout.Button("更改", GUILayout.Width(100), GUILayout.Height(40)))
        {
            skillEffect.AddTargetForce = tempEffect.AddTargetForce;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.AddSelfForce = EditorGUILayout.Vector2Field("AddSelfForce", tempEffect.AddSelfForce);
        if (GUILayout.Button("更改", GUILayout.Width(100), GUILayout.Height(40)))
        {
            skillEffect.AddSelfForce = tempEffect.AddSelfForce;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.MoveTarget = EditorGUILayout.Vector2Field("MoveTarget", tempEffect.MoveTarget);
        if (GUILayout.Button("更改", GUILayout.Width(100), GUILayout.Height(40)))
        {
            skillEffect.MoveTarget = tempEffect.MoveTarget;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.ScaleTarget = EditorGUILayout.Vector2Field("ScaleTarget", tempEffect.ScaleTarget);
        if (GUILayout.Button("更改", GUILayout.Width(100), GUILayout.Height(40)))
        {
            skillEffect.ScaleTarget = tempEffect.ScaleTarget;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.AddSelfVelocity = EditorGUILayout.Vector2Field("AddSelfVelocity", tempEffect.AddSelfVelocity);
        if (GUILayout.Button("更改", GUILayout.Width(100), GUILayout.Height(40)))
        {
            skillEffect.AddSelfVelocity = tempEffect.AddSelfVelocity;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.SelectorAngle = EditorGUILayout.FloatField("SelectorAngle", tempEffect.SelectorAngle);
        if (GUILayout.Button("更改", GUILayout.Width(100)))
        {
            skillEffect.SelectorAngle = tempEffect.SelectorAngle;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.SelectorRadius = EditorGUILayout.FloatField("SelectorRadius", tempEffect.SelectorRadius);
        if (GUILayout.Button("更改", GUILayout.Width(100)))
        {
            skillEffect.SelectorRadius = tempEffect.SelectorRadius;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.AddSelfDrag = EditorGUILayout.FloatField("AddSelfDrag", tempEffect.AddSelfDrag);
        if (GUILayout.Button("更改", GUILayout.Width(100)))
        {
            skillEffect.AddSelfDrag = tempEffect.AddSelfDrag;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.MoveDistance = EditorGUILayout.FloatField("MoveDistance", tempEffect.MoveDistance);
        if (GUILayout.Button("更改", GUILayout.Width(100)))
        {
            skillEffect.MoveDistance = tempEffect.MoveDistance;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.Gravity = EditorGUILayout.FloatField("Gravity", tempEffect.Gravity);
        if (GUILayout.Button("更改", GUILayout.Width(100)))
        {
            skillEffect.Gravity = tempEffect.Gravity;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.DamageMulity = EditorGUILayout.FloatField("DamageMulity", tempEffect.DamageMulity);
        if (GUILayout.Button("更改", GUILayout.Width(100)))
        {
            skillEffect.DamageMulity = tempEffect.DamageMulity;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tempEffect.Args = EditorGUILayout.TextField("Args", tempEffect.Args);
        if (GUILayout.Button("更改", GUILayout.Width(100)))
        {
            skillEffect.Args = tempEffect.Args;
            ShowNotification("更改成功");
        }
        EditorGUILayout.EndHorizontal();

        skillEffect.IsSmoon = EditorGUILayout.Toggle("IsSmoon", skillEffect.IsSmoon);
        skillEffect.IsShakeCamera = EditorGUILayout.Toggle("IsShakeCamera", skillEffect.IsShakeCamera);
        skillEffect.IsOnGroundHurt = EditorGUILayout.Toggle("IsOnGroundHurt", skillEffect.IsOnGroundHurt);
        skillEffect.IsOnGroundEffect = EditorGUILayout.Toggle("IsOnGroundEffect", skillEffect.IsOnGroundEffect);
        skillEffect.CanBeDefense = EditorGUILayout.Toggle("CanBeDefense", skillEffect.CanBeDefense);
        skillEffect.HitOne = EditorGUILayout.Toggle("HitOne", skillEffect.HitOne);
    }


    private void CloneEffects()
    {
        m_ListSkillEffect.Clear();
        for (int i = 0; i < SkillHelper.CurrConfigData.SkillEffects.Length; i++)
        {
            m_ListSkillEffect.Add(Clone(SkillHelper.CurrConfigData.SkillEffects[i]));
        }
    }

    private SkillConfigData.SkillEffect Clone(SkillConfigData.SkillEffect source)
    {
        SkillConfigData.SkillEffect newSkillEffect = new SkillConfigData.SkillEffect();
        newSkillEffect.EffectorType = source.EffectorType;
        newSkillEffect.SelectorType = source.SelectorType;
        newSkillEffect.ForceType = source.ForceType;
        newSkillEffect.Bullets = new SkillConfigData.Bullet[source.Bullets.Length];

        for (int j = 0; j < source.Bullets.Length; j++)
        {
            newSkillEffect.Bullets[j] = new SkillConfigData.Bullet();
            newSkillEffect.Bullets[j].Name = source.Bullets[j].Name;
            newSkillEffect.Bullets[j].AssetName = source.Bullets[j].AssetName;
            newSkillEffect.Bullets[j].NormalAnim = source.Bullets[j].NormalAnim;
            newSkillEffect.Bullets[j].HitAnim = source.Bullets[j].HitAnim;
            newSkillEffect.Bullets[j].NormalAnimSpeed = source.Bullets[j].NormalAnimSpeed;
            newSkillEffect.Bullets[j].HitAnimSpeed = source.Bullets[j].HitAnimSpeed;
            newSkillEffect.Bullets[j].Dir = source.Bullets[j].Dir;
            newSkillEffect.Bullets[j].Pos = source.Bullets[j].Pos;
            newSkillEffect.Bullets[j].Velocity = source.Bullets[j].Velocity;
            newSkillEffect.Bullets[j].HitRange = source.Bullets[j].HitRange;
            newSkillEffect.Bullets[j].Drag = source.Bullets[j].Drag;
            newSkillEffect.Bullets[j].IsPenatrate = source.Bullets[j].IsPenatrate;
        }

        newSkillEffect.SelectorOffest = source.SelectorOffest;
        newSkillEffect.AddTargetForce = source.AddTargetForce;
        newSkillEffect.AddSelfVelocity = source.AddSelfVelocity;
        newSkillEffect.SelectorAngle = source.SelectorAngle;
        newSkillEffect.SelectorRadius = source.SelectorRadius;
        newSkillEffect.AddSelfDrag = source.AddSelfDrag;
        newSkillEffect.MoveDistance = source.MoveDistance;
        newSkillEffect.Gravity = source.Gravity;
        newSkillEffect.DamageMulity = source.DamageMulity;
        newSkillEffect.IsSmoon = source.IsSmoon;
        newSkillEffect.IsShakeCamera = source.IsShakeCamera;
        newSkillEffect.IsOnGroundHurt = source.IsOnGroundHurt;
        newSkillEffect.IsOnGroundEffect = source.IsOnGroundEffect;
        newSkillEffect.CanBeDefense = source.CanBeDefense;
        newSkillEffect.HitOne = source.HitOne;
        newSkillEffect.Args = source.Args;

        return newSkillEffect;
    }

    private Vector2 m_ScrollPos = Vector2.zero;
    private List<SkillConfigData.SkillEffect> m_ListSkillEffect = null;
}