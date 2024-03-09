using GameFrameWork.UI;
using GameFrameWork.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/TestMask")]
public class TestMask : MaskableGraphic, ICanvasRaycastFilter
{
    public Vector3 _targetBoundsMin;
    public Vector3 _targetBoundsMax;
    public RectTransform _target;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (this._targetBoundsMin == Vector3.zero && this._targetBoundsMax == Vector3.zero)
        {
            base.OnPopulateMesh(vh);
            return;
        }

        vh.Clear();

        Vector2 pivot = this.rectTransform.pivot;
        Rect rect = this.rectTransform.rect;
        float outerLeftBottomX = -pivot.x * rect.width;
        float outerLeftBottomY = -pivot.y * rect.height;
        float outerRightTopX = (1 - pivot.x) * rect.width;
        float outerRightTopY = (1 - pivot.y) * rect.height;

        // 准备顶点数据
        UIVertex vert = UIVertex.simpleVert;
        // 填充顶点颜色
        vert.color = this.color;

        // 计算遮罩区域顶点位置
        // 0 outer LeftTop
        vert.position = new Vector3(outerLeftBottomX, outerRightTopY);
        vh.AddVert(vert);
        // 1 outer RightTop
        vert.position = new Vector3(outerRightTopX, outerRightTopY);
        vh.AddVert(vert);
        // 2 outer RightBottom
        vert.position = new Vector3(outerRightTopX, outerLeftBottomY);
        vh.AddVert(vert);
        // 3 outer LeftBottom
        vert.position = new Vector3(outerLeftBottomX, outerLeftBottomY);
        vh.AddVert(vert);

        // 计算镂空区域顶点位置
        // 4 outer LeftTop
        vert.position = new Vector3(_targetBoundsMin.x, _targetBoundsMax.y);
        vh.AddVert(vert);
        // 5 inner RightTop
        vert.position = new Vector3(_targetBoundsMax.x, _targetBoundsMax.y);
        vh.AddVert(vert);
        // 6 inner RightBottom
        vert.position = new Vector3(_targetBoundsMax.x, _targetBoundsMin.y);
        vh.AddVert(vert);
        // 7 inner LeftBottom
        vert.position = new Vector3(_targetBoundsMin.x, _targetBoundsMin.y);
        vh.AddVert(vert);

        // 向缓冲区中添加三角形
        vh.AddTriangle(4, 0, 1);
        vh.AddTriangle(4, 1, 5);
        vh.AddTriangle(5, 1, 2);
        vh.AddTriangle(5, 2, 6);
        vh.AddTriangle(6, 2, 3);
        vh.AddTriangle(6, 3, 7);
        vh.AddTriangle(7, 3, 0);
        vh.AddTriangle(7, 0, 4);
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        // 透传镂空区域事件
        return this._target != null && !RectTransformUtility.RectangleContainsScreenPoint(this._target, sp, eventCamera);
    }
}