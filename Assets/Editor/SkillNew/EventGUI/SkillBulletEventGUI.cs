using GameFrameWork.Editor;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillBulletEventGUI : SkillEventGUI
    {
        public SkillBulletEventGUI(EditorWindow window) : base(window)
        {
            m_CurrBulletEventInfo = new BulletEventInfo();
            m_BulletTypeNames = EditorUtil.GetAssemblyTypeNames("SkillBaseBullet", false, "SkillBaseBullet");
            m_CurrBulletTypeIndex = 0;
        }

        protected override void OnUpdateSkillEvent()
        {
            if (m_CurrEvent.bulletEventInfo == null)
            {
                m_CurrEvent.bulletEventInfo = new BulletEventInfo();
            }

            m_CurrBulletEventInfo.bulletName = m_CurrEvent.bulletEventInfo.bulletName;
            m_CurrBulletEventInfo.assetPath = m_CurrEvent.bulletEventInfo.assetPath;
            m_CurrBulletEventInfo.bulletClass = m_CurrEvent.bulletEventInfo.bulletClass;
            m_CurrBulletEventInfo.normalAnim = m_CurrEvent.bulletEventInfo.normalAnim;
            m_CurrBulletEventInfo.hitAnim = m_CurrEvent.bulletEventInfo.hitAnim;
            m_CurrBulletEventInfo.normalAnimSpeed = m_CurrEvent.bulletEventInfo.normalAnimSpeed;
            m_CurrBulletEventInfo.hitAnimSpeed = m_CurrEvent.bulletEventInfo.hitAnimSpeed;
            m_CurrBulletEventInfo.hitRange = m_CurrEvent.bulletEventInfo.hitRange;
            m_CurrBulletEventInfo.bulletCount = m_CurrEvent.bulletEventInfo.bulletCount;
            m_CurrBulletEventInfo.pos = m_CurrEvent.bulletEventInfo.pos;
            m_CurrBulletEventInfo.velocity = m_CurrEvent.bulletEventInfo.velocity;
            m_CurrBulletEventInfo.drag = m_CurrEvent.bulletEventInfo.drag;
            m_CurrBulletEventInfo.moveSpeed = m_CurrEvent.bulletEventInfo.moveSpeed;
            m_CurrBulletEventInfo.isPhysicsMove = m_CurrEvent.bulletEventInfo.isPhysicsMove;
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            DrawField(() => { return m_CurrBulletEventInfo.bulletName != m_CurrEvent.bulletEventInfo.bulletName; },
                () => { m_CurrBulletEventInfo.bulletName = EditorGUILayout.TextField("子弹名称", m_CurrBulletEventInfo.bulletName); },
                () => { m_CurrEvent.bulletEventInfo.bulletName = m_CurrBulletEventInfo.bulletName; });

            DrawField(() => { return m_CurrBulletEventInfo.assetPath != m_CurrEvent.bulletEventInfo.assetPath; },
                () => { m_CurrBulletEventInfo.assetPath = EditorGUILayout.TextField("资源路径", m_CurrBulletEventInfo.assetPath); },
                () => { m_CurrEvent.bulletEventInfo.assetPath = m_CurrBulletEventInfo.assetPath; });

            DrawField(() => { return m_CurrBulletEventInfo.bulletClass != m_CurrEvent.bulletEventInfo.bulletClass; },
                () =>
                {
                    int selectIndex = EditorGUILayout.Popup("子弹脚本", m_CurrBulletTypeIndex, m_BulletTypeNames);

                    if (selectIndex != m_CurrBulletTypeIndex)
                    {
                        m_CurrBulletTypeIndex = selectIndex;

                        if (m_BulletTypeNames != null && m_BulletTypeNames.Length > 0 && m_CurrBulletTypeIndex > -1 && m_CurrBulletTypeIndex < m_BulletTypeNames.Length)
                        {
                            m_CurrBulletEventInfo.assetPath = m_BulletTypeNames[m_CurrBulletTypeIndex];
                        }
                    }
                },
                () => { m_CurrEvent.bulletEventInfo.assetPath = m_CurrBulletEventInfo.assetPath; });

            DrawField(() => { return m_CurrBulletEventInfo.normalAnim != m_CurrEvent.bulletEventInfo.normalAnim; },
                () => { m_CurrBulletEventInfo.normalAnim = EditorGUILayout.TextField("初始动画", m_CurrBulletEventInfo.normalAnim); },
                () => { m_CurrEvent.bulletEventInfo.normalAnim = m_CurrBulletEventInfo.normalAnim; });

            DrawField(() => { return m_CurrBulletEventInfo.normalAnimSpeed != m_CurrEvent.bulletEventInfo.normalAnimSpeed; },
                () => { m_CurrBulletEventInfo.normalAnimSpeed = EditorGUILayout.Slider("初始动画速度", m_CurrBulletEventInfo.normalAnimSpeed, 0f, 10f); },
                () => { m_CurrEvent.bulletEventInfo.normalAnimSpeed = m_CurrBulletEventInfo.normalAnimSpeed; });

            DrawField(() => { return m_CurrBulletEventInfo.hitAnim != m_CurrEvent.bulletEventInfo.hitAnim; },
                () => { m_CurrBulletEventInfo.hitAnim = EditorGUILayout.TextField("击中动画", m_CurrBulletEventInfo.hitAnim); },
                () => { m_CurrEvent.bulletEventInfo.hitAnim = m_CurrBulletEventInfo.hitAnim; });

            DrawField(() => { return m_CurrBulletEventInfo.hitAnimSpeed != m_CurrEvent.bulletEventInfo.hitAnimSpeed; },
                () => { m_CurrBulletEventInfo.hitAnimSpeed = EditorGUILayout.Slider("击中动画速度", m_CurrBulletEventInfo.hitAnimSpeed, 0f, 10f); },
                () => { m_CurrEvent.bulletEventInfo.hitAnimSpeed = m_CurrBulletEventInfo.hitAnimSpeed; });

            DrawField(() => { return m_CurrBulletEventInfo.hitRange != m_CurrEvent.bulletEventInfo.hitRange; },
                () => { m_CurrBulletEventInfo.hitRange = Mathf.Max(0, EditorGUILayout.FloatField("打击范围", m_CurrBulletEventInfo.hitRange)); },
                () => { m_CurrEvent.bulletEventInfo.hitRange = m_CurrBulletEventInfo.hitRange; });

            DrawField(() => { return m_CurrBulletEventInfo.bulletCount != m_CurrEvent.bulletEventInfo.bulletCount; },
                () => { m_CurrBulletEventInfo.bulletCount = EditorGUILayout.IntSlider("子弹数量", m_CurrBulletEventInfo.bulletCount, -1, 999); },
                () => { m_CurrEvent.bulletEventInfo.bulletCount = m_CurrBulletEventInfo.bulletCount; });

            DrawField(() => { return m_CurrBulletEventInfo.pos != m_CurrEvent.bulletEventInfo.pos; },
                () => { m_CurrBulletEventInfo.pos = EditorGUILayout.Vector2Field("初始位置", m_CurrBulletEventInfo.pos); },
                () => { m_CurrEvent.bulletEventInfo.pos = m_CurrBulletEventInfo.pos; }, 40);

            DrawField(() => { return m_CurrBulletEventInfo.isPhysicsMove != m_CurrEvent.bulletEventInfo.isPhysicsMove; },
                () => { m_CurrBulletEventInfo.isPhysicsMove = EditorGUILayout.Toggle("物理运动", m_CurrBulletEventInfo.isPhysicsMove); },
                () => { m_CurrEvent.bulletEventInfo.isPhysicsMove = m_CurrBulletEventInfo.isPhysicsMove; });

            if (m_CurrEvent.bulletEventInfo.isPhysicsMove)
            {
                m_CurrEvent.bulletEventInfo.moveSpeed = 0f;

                DrawField(() => { return m_CurrBulletEventInfo.velocity != m_CurrEvent.bulletEventInfo.velocity; },
                    () => { m_CurrBulletEventInfo.velocity = EditorGUILayout.Vector2Field("物理速度", m_CurrBulletEventInfo.velocity); },
                    () => { m_CurrEvent.bulletEventInfo.velocity = m_CurrBulletEventInfo.velocity; }, 40);
                DrawField(() => { return m_CurrBulletEventInfo.drag != m_CurrEvent.bulletEventInfo.drag; },
                    () => { m_CurrBulletEventInfo.drag = Mathf.Max(0f, EditorGUILayout.FloatField("运动阻力", m_CurrBulletEventInfo.drag)); },
                    () => { m_CurrEvent.bulletEventInfo.drag = m_CurrBulletEventInfo.drag; });
            }
            else
            {
                m_CurrEvent.bulletEventInfo.velocity = Vector2.zero;
                m_CurrEvent.bulletEventInfo.drag = 0f;

                DrawField(() => { return m_CurrBulletEventInfo.moveSpeed != m_CurrEvent.bulletEventInfo.moveSpeed; },
                    () => { m_CurrBulletEventInfo.moveSpeed = Mathf.Max(0f, EditorGUILayout.FloatField("线性速度", m_CurrBulletEventInfo.moveSpeed)); },
                    () => { m_CurrEvent.bulletEventInfo.moveSpeed = m_CurrBulletEventInfo.moveSpeed; });
            }
        }

        protected override void OnResetEvent()
        {
            base.OnResetEvent();
            m_CurrEvent.bulletEventInfo = null;
        }

        private string[] m_BulletTypeNames = null;
        private int m_CurrBulletTypeIndex = 0;
        private BulletEventInfo m_CurrBulletEventInfo = null;
    }
}