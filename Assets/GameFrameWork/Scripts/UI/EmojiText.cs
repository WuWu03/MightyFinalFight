using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.EventSystems;
using System;
using GameFrameWork.Utility;

namespace GameFrameWork.UI
{
    public class EmojiText : Text, IPointerClickHandler
    {
        public Color HrefColor = Color.blue;
        /// <summary>
        /// 超链接信息类
        /// </summary>
        class HrefInfo
        {
            public int endIndex;
            public int newEndIndex;
            public int startIndex;
            public int newStartIndex;
            public string name;
            public readonly List<Rect> boxes = new List<Rect>();
        }

        struct EmojiInfo
        {
            public float x;
            public float y;
            public float size;
            public int len;
        }

        //超连接点击委托
        public delegate void VoidOnHrefClick(string hrefName);
        public VoidOnHrefClick onHrefClick;

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            m_OutputText = GetOutputText(text);
        }

        /// <summary>
        /// 重写顶点填充
        /// </summary>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            if (EmojiIndex == null)
            {
                EmojiIndex = new Dictionary<string, EmojiInfo>();

                //load emoji data, and you can overwrite this segment code base on your project.
                TextAsset emojiContent = UnityEngine.Resources.Load<TextAsset>("emoji");
                string[] lines = emojiContent.text.Split('\n');
                for (int i = 1; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                    {
                        string[] strs = lines[i].Split('\t');
                        EmojiInfo info;
                        info.x = float.Parse(strs[3]);
                        info.y = float.Parse(strs[4]);
                        info.size = float.Parse(strs[5]);
                        info.len = 0;
                        EmojiIndex.Add(strs[1], info);
                    }
                }
            }

            //key是标签在字符串中的索引

            Dictionary<int, EmojiInfo> emojiDic = new Dictionary<int, EmojiInfo>();
            if (supportRichText)
            {
#if UNITY_2019_1_OR_NEWER
                MatchCollection matches = m_EmojiRegex.Matches(ReplaceRichText(m_OutputText));//把表情标签全部匹配出来
#else
                MatchCollection matches = m_EmojiRegex.Matches(m_OutputText);//把表情标签全部匹配出来
#endif
                for (int i = 0; i < matches.Count; i++)
                {
                    EmojiInfo info;
                    if (EmojiIndex.TryGetValue(matches[i].Value, out info))
                    {
                        info.len = matches[i].Length;
                        emojiDic.Add(matches[i].Index, info);
                    }
                }
            }

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;
            
            var orignText = m_Text;
            m_Text = m_OutputText;

            Vector2 extents = rectTransform.rect.size;
            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.Populate(m_Text, settings);//重置网格
            m_Text = orignText;
            Rect inputRect = rectTransform.rect;

            // get the text alignment anchor point for the text in local space
            Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
            refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);

            // Determine fraction of pixel to offset text mesh.
            Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line...
#if UNITY_2019_1_OR_NEWER
            int vertCount = verts.Count;// verts.Count - 4;最后四个顶点不渲染，导致少一个字符
#else
            int vertCount = verts.Count - 4;
#endif
            toFill.Clear();
            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            else
            {
                float repairDistance = 0;
                float repairDistanceHalf = 0;
                float repairY = 0;
                if (vertCount > 0)
                {
                    repairY = verts[3].position.y;
                }
                for (int i = 0; i < vertCount; ++i)
                {
                    EmojiInfo info;
                    int index = i / 4;//每个字符4个顶点
                    if (emojiDic.TryGetValue(index, out info))//这个顶点位置是否为表情开始的index
                    {
                        //compute the distance of '[' and get the distance of emoji 
                        //计算表情标签2个顶点之间的距离， * 3 得出宽度（表情有3位）
                        float charDis = 2 * (verts[i + 1].position.x - verts[i].position.x) * 3;
                        m_TempVerts[3] = verts[i];//1
                        m_TempVerts[2] = verts[i + 1];//2
                        m_TempVerts[1] = verts[i + 2];//3
                        m_TempVerts[0] = verts[i + 3];//4

                        //the real distance of an emoji
                        m_TempVerts[2].position += new Vector3(charDis, 0, 0);
                        m_TempVerts[1].position += new Vector3(charDis, 0, 0);

                        float fixWidth = m_TempVerts[2].position.x - m_TempVerts[3].position.x;
                        float fixHeight = (m_TempVerts[2].position.y - m_TempVerts[1].position.y);
                        //make emoji has equal width and height
                        float fixValue = (fixWidth - fixHeight);//把宽度变得跟高度一样
                        m_TempVerts[2].position -= new Vector3(fixValue, 0, 0);
                        m_TempVerts[1].position -= new Vector3(fixValue, 0, 0);

                        float curRepairDis = 0;
                        if (verts[i].position.y < repairY)// to judge current char in the same line or not
                        {
                            repairDistance = repairDistanceHalf;
                            repairDistanceHalf = 0;
                            repairY = verts[i + 3].position.y;
                        }
                        curRepairDis = repairDistance;
                        int dot = 0;//repair next line distance
                        for (int j = info.len - 1; j > 0; j--)
                        {
                            int infoIndex = i + j * 4 + 3;
                            if (verts.Count > infoIndex && verts[infoIndex].position.y >= verts[i + 3].position.y)
                            {
                                repairDistance += verts[i + j * 4 + 1].position.x - m_TempVerts[2].position.x;
                                break;
                            }
                            else
                            {
                                dot = i + 4 * j;
                            }
                        }
                        if (dot > 0)
                        {
                            int nextChar = i + info.len * 4;
                            if (nextChar < verts.Count)
                            {
                                repairDistanceHalf = verts[nextChar].position.x - verts[dot].position.x;
                            }
                        }

                        for (int j = 0; j < 4; j++)//repair its distance
                        {
                            m_TempVerts[j].position -= new Vector3(curRepairDis, 0, 0);
                        }

                        m_TempVerts[0].position *= unitsPerPixel;
                        m_TempVerts[1].position *= unitsPerPixel;
                        m_TempVerts[2].position *= unitsPerPixel;
                        m_TempVerts[3].position *= unitsPerPixel;

                        float pixelOffset = emojiDic[index].size / 32 / 2;
                        m_TempVerts[0].uv1 = new Vector2(emojiDic[index].x + pixelOffset, emojiDic[index].y + pixelOffset);
                        m_TempVerts[1].uv1 = new Vector2(emojiDic[index].x - pixelOffset + emojiDic[index].size, emojiDic[index].y + pixelOffset);
                        m_TempVerts[2].uv1 = new Vector2(emojiDic[index].x - pixelOffset + emojiDic[index].size, emojiDic[index].y - pixelOffset + emojiDic[index].size);
                        m_TempVerts[3].uv1 = new Vector2(emojiDic[index].x + pixelOffset, emojiDic[index].y - pixelOffset + emojiDic[index].size);

                        toFill.AddUIVertexQuad(m_TempVerts);

                        i += 4 * info.len - 1;
                    }
                    else
                    {
                        int tempVertsIndex = i & 3;
                        if (tempVertsIndex == 0 && verts[i].position.y < repairY)
                        {
                            repairY = verts[i + 3].position.y;
                            repairDistance = repairDistanceHalf;
                            repairDistanceHalf = 0;
                        }
                        m_TempVerts[tempVertsIndex] = verts[i];
                        m_TempVerts[tempVertsIndex].position -= new Vector3(repairDistance, 0, 0);
                        m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                        if (tempVertsIndex == 3)
                            toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
            }

            if (m_HrefInfos.Count > 0)
            {           
                for (int i = 0; i < m_HrefInfos.Count; i++)// 处理超链接包围框  
                {
                    m_HrefInfos[i].boxes.Clear();
#if UNITY_2019_1_OR_NEWER
                    int startIndex = m_HrefInfos[i].newStartIndex;
                    int endIndex = m_HrefInfos[i].newEndIndex;
#else
                    int startIndex = m_HrefInfos[i].startIndex;
                    int endIndex = m_HrefInfos[i].endIndex;
#endif
                    if (startIndex >= toFill.currentVertCount)
                        continue;

                    toFill.PopulateUIVertex(ref vert, startIndex);// 将超链接里面的文本顶点索引坐标加入到包围框  

                    var pos = vert.position;
                    var bounds = new Bounds(pos, Vector3.zero);
                    for (int j = startIndex + 1; j < endIndex; j++)
                    {
                        if (j >= toFill.currentVertCount)
                        {
                            break;
                        }
                        toFill.PopulateUIVertex(ref vert, j);
                        pos = vert.position;
                        if (pos.x < bounds.min.x)
                        { 
                            m_HrefInfos[i].boxes.Add(new Rect(bounds.min, bounds.size)); // 换行重新添加包围框  
                            bounds = new Bounds(pos, Vector3.zero);
                        }
                        else
                        {
                            bounds.Encapsulate(pos); // 扩展包围框  
                        }
                    }         
                    m_HrefInfos[i].boxes.Add(new Rect(bounds.min, bounds.size));//添加包围盒
                }
            }

            m_DisableFontTextureRebuiltCallback = false;
        }


        /// <summary>
        /// 获取超链接解析后的最后输出文本
        /// </summary>
        protected virtual string GetOutputText(string outputText)
        {
            s_TextBuilder.Length = 0;
            m_HrefInfos.Clear();

            if (string.IsNullOrEmpty(outputText))
                return "";

            s_TextBuilder.Remove(0, s_TextBuilder.Length);

            int textIndex = 0;
            int newIndex = 0;
            int removeEmojiCount = 0;

            foreach (Match match in m_HrefRegex.Matches(outputText))
            {
                var hrefInfo = new HrefInfo();
                string part = outputText.Substring(textIndex, match.Index - textIndex);
                int removeEmojiCountNew = 0;
                MatchCollection collection = m_EmojiRegex.Matches(part);

                foreach (Match emojiMatch in collection)
                {
                    removeEmojiCount += 8;
                    removeEmojiCountNew += 8;
                }

                s_TextBuilder.Append(part);
                s_TextBuilder.AppendFormat("<color={0}>", Utility.Util.ToRGBHex(HrefColor));
                int startIndex = s_TextBuilder.Length * 4 - removeEmojiCount;
                s_TextBuilder.Append(match.Groups[2].Value);
                int endIndex = s_TextBuilder.Length * 4 - removeEmojiCount;
                s_TextBuilder.Append("</color>");

                hrefInfo.startIndex = startIndex;// 超链接里的文本起始顶点索引
                hrefInfo.endIndex = endIndex;

#if UNITY_2019_1_OR_NEWER
                newIndex = newIndex + ReplaceRichText(part).Length * 4 - removeEmojiCountNew;//移除超连接前面的表情的顶点
                int newStartIndex = newIndex;
                newIndex = newIndex + match.Groups[2].Value.Length * 4;
                hrefInfo.newStartIndex = newStartIndex;
                hrefInfo.newEndIndex = newIndex;
#endif
                hrefInfo.name = match.Groups[1].Value;
                m_HrefInfos.Add(hrefInfo);
                textIndex = match.Index + match.Length;
            }

            s_TextBuilder.Append(outputText.Substring(textIndex, outputText.Length - textIndex));
            return s_TextBuilder.ToString();
        }

        /// <summary>
        /// 换掉富文本
        /// </summary>
        private string ReplaceRichText(string str)
        {
            str = Regex.Replace(str, @"<color=(.+?)>", "");
            str = str.Replace("</color>", "");
            str = Regex.Replace(str, @"<a href=(.+?)>", "");
            str = str.Replace("</a>", "");
            str = str.Replace("<b>", "");
            str = str.Replace("</b>", "");
            str = str.Replace("<i>", "");
            str = str.Replace("</i>", "");
            str = str.Replace("\n", "");
            str = str.Replace("\t", "");
            str = str.Replace("\r", "");
            str = str.Replace(" ", "");

            return str;
        }

        /// <summary>
        /// 点击事件检测是否点击到超链接文本
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var hrefInfo in m_HrefInfos)
            {
                var boxes = hrefInfo.boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        if (onHrefClick != null)
                        {
                            onHrefClick(hrefInfo.name);
                        }
                        Log.Debugger.Log("点击了:" + hrefInfo.name);
                        return;
                    }
                }
            }
        }

        private string m_OutputText;//解析之后的文本
        private const bool EMOJI_LARGE = true;
        private static Dictionary<string, EmojiInfo> EmojiIndex = null;

        private readonly UIVertex[] m_TempVerts = new UIVertex[4];

        private static readonly Regex m_HrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline); // 超链接正则
        private static readonly Regex m_EmojiRegex = new Regex("\\[[a-z0-9A-Z]+\\]", RegexOptions.Singleline); // 表情正则

        private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();// 超链接信息列表
        private static readonly StringBuilder s_TextBuilder = new StringBuilder();// 文本构造器

        private UIVertex vert = new UIVertex();
    }
}