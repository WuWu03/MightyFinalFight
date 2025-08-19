using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class EmptyImage : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
