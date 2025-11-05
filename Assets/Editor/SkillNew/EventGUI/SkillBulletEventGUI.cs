using GameFrameWork.Editor;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillBulletEventGUI : SkillEventGUI
    {
        private readonly string[] m_BulletTypeNames;
        private int m_CurrBulletTypeIndex;
        private BulletEventInfo m_CurrBulletEventInfo;
        
        public SkillBulletEventGUI(EditorWindow window) : base(window)
        {
            m_CurrBulletEventInfo = new BulletEventInfo();
            m_BulletTypeNames = EditorUtil.GetAssemblyTypeNames("SkillBaseBullet", false, "SkillBaseBullet");
            m_CurrBulletTypeIndex = 0;
        }

        protected override void OnUpdateSkillEvent()
        {
            currEvent.bulletEventInfo ??= new();
            m_CurrBulletEventInfo ??= new();
            m_CurrBulletEventInfo.bulletName = currEvent.bulletEventInfo.bulletName;
            m_CurrBulletEventInfo.assetPath = currEvent.bulletEventInfo.assetPath;
            m_CurrBulletEventInfo.bulletClass = currEvent.bulletEventInfo.bulletClass;
            m_CurrBulletEventInfo.normalAnim = currEvent.bulletEventInfo.normalAnim;
            m_CurrBulletEventInfo.hitAnim = currEvent.bulletEventInfo.hitAnim;
            m_CurrBulletEventInfo.normalAnimSpeed = currEvent.bulletEventInfo.normalAnimSpeed;
            m_CurrBulletEventInfo.hitAnimSpeed = currEvent.bulletEventInfo.hitAnimSpeed;
            m_CurrBulletEventInfo.hitRange = currEvent.bulletEventInfo.hitRange;
            m_CurrBulletEventInfo.bulletCount = currEvent.bulletEventInfo.bulletCount;
            m_CurrBulletEventInfo.pos = currEvent.bulletEventInfo.pos;
            m_CurrBulletEventInfo.velocity = currEvent.bulletEventInfo.velocity;
            m_CurrBulletEventInfo.drag = currEvent.bulletEventInfo.drag;
            m_CurrBulletEventInfo.moveSpeed = currEvent.bulletEventInfo.moveSpeed;
            m_CurrBulletEventInfo.isPhysicsMove = currEvent.bulletEventInfo.isPhysicsMove;
        }

        protected override void OnGUI()
        {
            DrawField(() => { return m_CurrBulletEventInfo.bulletName != currEvent.bulletEventInfo.bulletName; },
                () => { m_CurrBulletEventInfo.bulletName = EditorGUILayout.TextField("子弹名称", m_CurrBulletEventInfo.bulletName); },
                () => { currEvent.bulletEventInfo.bulletName = m_CurrBulletEventInfo.bulletName; });

            DrawField(() => { return m_CurrBulletEventInfo.assetPath != currEvent.bulletEventInfo.assetPath; },
                () => { m_CurrBulletEventInfo.assetPath = EditorGUILayout.TextField("资源路径", m_CurrBulletEventInfo.assetPath); },
                () => { currEvent.bulletEventInfo.assetPath = m_CurrBulletEventInfo.assetPath; });

            DrawField(() => { return m_CurrBulletEventInfo.bulletClass != currEvent.bulletEventInfo.bulletClass; },
                () =>
                {
                    int selectIndex = EditorGUILayout.Popup("子弹脚本", m_CurrBulletTypeIndex, m_BulletTypeNames);

                    if (selectIndex != m_CurrBulletTypeIndex)
                    {
                        m_CurrBulletTypeIndex = selectIndex;

                        if (m_BulletTypeNames is { Length: > 0 } && m_CurrBulletTypeIndex > -1 && m_CurrBulletTypeIndex < m_BulletTypeNames.Length)
                        {
                            m_CurrBulletEventInfo.assetPath = m_BulletTypeNames[m_CurrBulletTypeIndex];
                        }
                    }
                },
                () => { currEvent.bulletEventInfo.assetPath = m_CurrBulletEventInfo.assetPath; });

            DrawField(() => { return m_CurrBulletEventInfo.normalAnim != currEvent.bulletEventInfo.normalAnim; },
                () => { m_CurrBulletEventInfo.normalAnim = EditorGUILayout.TextField("初始动画", m_CurrBulletEventInfo.normalAnim); },
                () => { currEvent.bulletEventInfo.normalAnim = m_CurrBulletEventInfo.normalAnim; });

            DrawField(() => { return !Mathf.Approximately(m_CurrBulletEventInfo.normalAnimSpeed, currEvent.bulletEventInfo.normalAnimSpeed); },
                () => { m_CurrBulletEventInfo.normalAnimSpeed = EditorGUILayout.Slider("初始动画速度", m_CurrBulletEventInfo.normalAnimSpeed, 0f, 10f); },
                () => { currEvent.bulletEventInfo.normalAnimSpeed = m_CurrBulletEventInfo.normalAnimSpeed; });

            DrawField(() => { return m_CurrBulletEventInfo.hitAnim != currEvent.bulletEventInfo.hitAnim; },
                () => { m_CurrBulletEventInfo.hitAnim = EditorGUILayout.TextField("击中动画", m_CurrBulletEventInfo.hitAnim); },
                () => { currEvent.bulletEventInfo.hitAnim = m_CurrBulletEventInfo.hitAnim; });

            DrawField(() => { return !Mathf.Approximately(m_CurrBulletEventInfo.hitAnimSpeed, currEvent.bulletEventInfo.hitAnimSpeed); },
                () => { m_CurrBulletEventInfo.hitAnimSpeed = EditorGUILayout.Slider("击中动画速度", m_CurrBulletEventInfo.hitAnimSpeed, 0f, 10f); },
                () => { currEvent.bulletEventInfo.hitAnimSpeed = m_CurrBulletEventInfo.hitAnimSpeed; });

            DrawField(() => { return !Mathf.Approximately(m_CurrBulletEventInfo.hitRange, currEvent.bulletEventInfo.hitRange); },
                () => { m_CurrBulletEventInfo.hitRange = Mathf.Max(0, EditorGUILayout.FloatField("打击范围", m_CurrBulletEventInfo.hitRange)); },
                () => { currEvent.bulletEventInfo.hitRange = m_CurrBulletEventInfo.hitRange; });

            DrawField(() => { return m_CurrBulletEventInfo.bulletCount != currEvent.bulletEventInfo.bulletCount; },
                () => { m_CurrBulletEventInfo.bulletCount = EditorGUILayout.IntSlider("子弹数量", m_CurrBulletEventInfo.bulletCount, -1, 999); },
                () => { currEvent.bulletEventInfo.bulletCount = m_CurrBulletEventInfo.bulletCount; });

            DrawField(() => { return m_CurrBulletEventInfo.pos != currEvent.bulletEventInfo.pos; },
                () => { m_CurrBulletEventInfo.pos = EditorGUILayout.Vector2Field("初始位置", m_CurrBulletEventInfo.pos); },
                () => { currEvent.bulletEventInfo.pos = m_CurrBulletEventInfo.pos; }, 40);

            DrawField(() => { return m_CurrBulletEventInfo.isPhysicsMove != currEvent.bulletEventInfo.isPhysicsMove; },
                () => { m_CurrBulletEventInfo.isPhysicsMove = EditorGUILayout.Toggle("物理运动", m_CurrBulletEventInfo.isPhysicsMove); },
                () => { currEvent.bulletEventInfo.isPhysicsMove = m_CurrBulletEventInfo.isPhysicsMove; });

            if (currEvent.bulletEventInfo.isPhysicsMove)
            {
                currEvent.bulletEventInfo.moveSpeed = 0f;

                DrawField(() => { return m_CurrBulletEventInfo.velocity != currEvent.bulletEventInfo.velocity; },
                    () => { m_CurrBulletEventInfo.velocity = EditorGUILayout.Vector2Field("物理速度", m_CurrBulletEventInfo.velocity); },
                    () => { currEvent.bulletEventInfo.velocity = m_CurrBulletEventInfo.velocity; }, 40);
                DrawField(() => { return !Mathf.Approximately(m_CurrBulletEventInfo.drag, currEvent.bulletEventInfo.drag); },
                    () => { m_CurrBulletEventInfo.drag = Mathf.Max(0f, EditorGUILayout.FloatField("运动阻力", m_CurrBulletEventInfo.drag)); },
                    () => { currEvent.bulletEventInfo.drag = m_CurrBulletEventInfo.drag; });
            }
            else
            {
                currEvent.bulletEventInfo.velocity = Vector2.zero;
                currEvent.bulletEventInfo.drag = 0f;

                DrawField(() => { return !Mathf.Approximately(m_CurrBulletEventInfo.moveSpeed, currEvent.bulletEventInfo.moveSpeed); },
                    () => { m_CurrBulletEventInfo.moveSpeed = Mathf.Max(0f, EditorGUILayout.FloatField("线性速度", m_CurrBulletEventInfo.moveSpeed)); },
                    () => { currEvent.bulletEventInfo.moveSpeed = m_CurrBulletEventInfo.moveSpeed; });
            }
        }

        protected override void OnResetEvent()
        {
            m_CurrBulletEventInfo = null;
        }
    }
}