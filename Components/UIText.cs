using System.Collections.Generic;
using IG.Module.Language;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    [AddComponentMenu("UI工具/UI组件/多语言Text", 10)]
    [ExecuteAlways]
    public class UIText : Text{
        [TextArea(3, 10)]
        [SerializeField]
        public string CurLanguageKey;

        [SerializeField]
        public SystemLanguage CurLanguage = SystemLanguage.English;

        public override string text{
            get{
                if (null == m_Text){
                    m_Text = LanguageManagerWrap.GetLangText(CurLanguageKey);
                }

                return m_Text;
            }
            set{
                CurLanguageKey = value;
                CurLanguage    = LanguageManagerWrap.CurrenLang;
                var text = LanguageManagerWrap.GetLangText(CurLanguageKey);
                base.text = text;
            }
        }

        public Material MaskMaterial{ get{ return m_MaskMaterial; } }

        protected override void Awake(){
            CurLanguage = LanguageManagerWrap.CurrenLang;
            m_Text      = LanguageManagerWrap.GetLangText(CurLanguageKey);
            base.Awake();
        }

    #region 新版描边实现

        /*
            旧版描边是用outlineEx实现的，目前有如下问题：
            1，每个非黑色描边需要动态创建材质；
            2，需要对mask进行单独的支持，不管是mask还是rect mask 2d;
            3，不同颜色无法合批；
            4，右边的字母会有一点覆盖左边字母；

            新版解决了以上问题
            如遇其他问题请及时与richard沟通

            2022.5.20
            新增对渐变色的支持，可选上下左右颜色
            新增对阴影的支持，主要是旧的阴影绘制顶点太多，新版采用一层绘制

        */

    #region 公有属性

        public float outlineWidth{
            get{ return m_OutlineWidth; }
            set{
                if (m_OutlineWidth == value) return;
                m_OutlineWidth = value;
                SetVerticesDirty();
            }
        }

        public bool enableOutline{
            get{ return m_EnableOutline; }
            set{
                if (m_EnableOutline == value) return;
                m_EnableOutline = value;
                SetVerticesDirty();
            }
        }

        public Color outlineCol{
            get{ return m_OutlineCol; }
            set{
                if (m_OutlineCol == value) return;
                m_OutlineCol = value;
                SetVerticesDirty();
            }
        }

        public bool enableGradient{
            get{ return m_EnableGradient; }
            set{
                if (m_EnableGradient == value) return;
                m_EnableGradient = value;
                SetVerticesDirty();
            }
        }

        public bool supportMultipleLines{
            get{ return m_SupportMultipleLines; }
            set{
                if (m_SupportMultipleLines == value) return;
                m_SupportMultipleLines = value;
                SetVerticesDirty();
            }
        }

        public Color gradientColLeft{
            get{ return m_GradientColLeft; }
            set{
                if (m_GradientColLeft == value) return;
                m_GradientColLeft = value;
                SetVerticesDirty();
            }
        }

        public Color gradientColRight{
            get{ return m_GradientColRight; }
            set{
                if (m_GradientColRight == value) return;
                m_GradientColRight = value;
                SetVerticesDirty();
            }
        }

        public Color gradientColTop{
            get{ return m_GradientColTop; }
            set{
                if (m_GradientColTop == value) return;
                m_GradientColTop = value;
                SetVerticesDirty();
            }
        }

        public Color gradientColBottom{
            get{ return m_GradientColBottom; }
            set{
                if (m_GradientColBottom == value) return;
                m_GradientColBottom = value;
                SetVerticesDirty();
            }
        }

        public float gradientRatio{
            get{ return m_GradientRatio; }
            set{
                if (m_GradientRatio == value) return;
                m_GradientRatio = value;
                SetVerticesDirty();
            }
        }

        public bool enableShadow{
            get{ return m_EnableShadow; }
            set{
                if (m_EnableShadow == value) return;
                m_EnableShadow = value;
                SetVerticesDirty();
            }
        }

        public float shadowScale{
            get{ return m_ShadowScale; }
            set{
                if (m_ShadowScale == value) return;
                m_ShadowScale = value;
                SetVerticesDirty();
            }
        }

        public Vector2 shadowOffset{
            get{ return m_ShadowOffset; }
            set{
                if (m_ShadowOffset == value) return;
                m_ShadowOffset = value;
                SetVerticesDirty();
            }
        }

        public Color shadowCol{
            get{ return m_ShadowCol; }
            set{
                if (m_ShadowCol == value) return;
                m_ShadowCol = value;
                SetVerticesDirty();
            }
        }

    #endregion

        [Range(0, 10)]
        [SerializeField]
        private float m_OutlineWidth = 1;

        [SerializeField]
        private bool m_EnableOutline = false;

        [SerializeField]
        private Color m_OutlineCol = Color.black;

        [SerializeField]
        private bool m_EnableGradient = false;

        [SerializeField]
        private Color m_GradientColLeft = Color.blue;

        [SerializeField]
        private Color m_GradientColRight = Color.red;

        [SerializeField]
        private Color m_GradientColTop = Color.white;

        [SerializeField]
        private Color m_GradientColBottom = Color.black;

        [Range(0, 1)]
        [SerializeField]
        private float m_GradientRatio = 0.5f; // 1-垂直， 0-水平

        [SerializeField]
        private bool m_SupportMultipleLines = false; //支持多行

        [SerializeField]
        private bool m_EnableShadow = false;

        [SerializeField]
        [Range(1, 1.2f)]
        private float m_ShadowScale = 1f;

        const float ShadowScaleFixed = 0.1f;

        [SerializeField]
        private Vector2 m_ShadowOffset = new Vector2(0, 3);

        // 比例 主要是跟项目里旧的shadow组件匹配 数值
        const float ShadowOffsetFixed = 1.5f;

        [SerializeField]
        private Color m_ShadowCol = Color.black;

        readonly UIVertex[] m_TempVerts = new UIVertex[4];

        protected override void OnPopulateMesh(VertexHelper toFill){
            if (font == null) return;
            m_DisableFontTextureRebuiltCallback = true;
            Vector2 extents  = rectTransform.rect.size;
            var     settings = GetGenerationSettings(extents);
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
            IList<UIVertex> verts         = cachedTextGenerator.verts;
            float           unitsPerPixel = 1 / pixelsPerUnit;
            int             vertCount     = verts.Count;
            if (vertCount <= 0){
                toFill.Clear();
                return;
            }

            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();
            if (m_EnableShadow) SetFontVertex(verts, m_ShadowOffset.normalized * (Length(m_ShadowOffset) + ShadowOffsetFixed), unitsPerPixel, toFill, m_ShadowCol, roundingOffset, m_ShadowScale + ShadowScaleFixed);
            SetOutline(verts, unitsPerPixel, toFill, roundingOffset);
            var   mdp   = CalcRect(verts);
            float ratio = 1f;
            for (int i = 0; i < vertCount; ++i){
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                var v = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (m_EnableGradient) m_TempVerts[tempVertsIndex].color = CalcOneDir(tempVertsIndex, verts, mdp) * ratio;
                if (roundingOffset != Vector2.zero){
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                }

                if (tempVertsIndex == 3){
                    toFill.AddUIVertexQuad(m_TempVerts);
                }
            }

            m_DisableFontTextureRebuiltCallback = false;
        }

        float Length(Vector3 v3){ return Mathf.Sqrt(v3.x * v3.x + v3.y * v3.y + v3.z * v3.z); }
        float Length(Vector2 v2){ return Mathf.Sqrt(v2.x * v2.x + v2.y * v2.y); }

        private Color CalcOneDir(int tempVertsIndex, IList<UIVertex> verts, MaxDirPos mdp){
            var maxLen  = mdp.maxLen;
            var maxHor  = maxLen.x - maxLen.y;
            var x_ratio = (m_TempVerts[tempVertsIndex].position.x - maxLen.y) / maxHor;
            var x_color = m_GradientColRight * x_ratio + m_GradientColLeft * (1 - x_ratio);
            var maxVor  = maxLen.z                     - maxLen.w;
            var y_ratio = (m_TempVerts[tempVertsIndex].position.y - maxLen.w) / maxVor;

            //支持多行，目前还有如下问题：不能合理避开行间距的影响
            if (m_SupportMultipleLines){
                var lines_ratio = 1.0f / cachedTextGenerator.lineCount + 0.001f;
                y_ratio = (y_ratio % lines_ratio) / lines_ratio;
            }

            var y_color = m_GradientColTop * y_ratio + m_GradientColBottom * (1 - y_ratio);
            return (x_color * m_GradientRatio + y_color * (1 - m_GradientRatio));
        }

        Vector3 up        = Vector3.zero;
        Vector3 down      = Vector3.zero;
        Vector3 left      = Vector3.zero;
        Vector3 right     = Vector3.zero;
        Vector3 upleft    = Vector3.zero;
        Vector3 downleft  = Vector3.zero;
        Vector3 downright = Vector3.zero;
        Vector3 upright   = Vector3.zero;

        protected void SetOutline(IList<UIVertex> verts, float unitsPerPixel, VertexHelper toFill, Vector2 roundingOffset){
            if (!m_EnableOutline) return;
            float width = font.fontSize / 17f * m_OutlineWidth;
            up.y        = width;
            down.y      = -width;
            left.x      = -width;
            right.x     = width;
            upleft.x    = -width;
            upleft.y    = width;
            downright.x = width;
            downright.y = -width;
            downleft.x  = -width;
            downleft.y  = -width;
            upright.x   = width;
            upright.y   = width;
            SetFontVertex(verts, up,        unitsPerPixel, toFill, m_OutlineCol, roundingOffset);
            SetFontVertex(verts, down,      unitsPerPixel, toFill, m_OutlineCol, roundingOffset);
            SetFontVertex(verts, left,      unitsPerPixel, toFill, m_OutlineCol, roundingOffset);
            SetFontVertex(verts, right,     unitsPerPixel, toFill, m_OutlineCol, roundingOffset);
            SetFontVertex(verts, upleft,    unitsPerPixel, toFill, m_OutlineCol, roundingOffset);
            SetFontVertex(verts, downright, unitsPerPixel, toFill, m_OutlineCol, roundingOffset);
            SetFontVertex(verts, upright,   unitsPerPixel, toFill, m_OutlineCol, roundingOffset);
            SetFontVertex(verts, downleft,  unitsPerPixel, toFill, m_OutlineCol, roundingOffset);
        }

        float Smoothstep(float t1, float t2, float x){
            x = Mathf.Clamp((x - t1) / (t2 - t1), 0.0f, 1.0f);
            return x * x * (3 - 2 * x);
        }

        protected void SetFontVertex(IList<UIVertex> verts, Vector3 offset, float unitsPerPixel, VertexHelper toFill, Color col, Vector2 roundingOffset, float ratio = 1){
            for (int i = 0; i < verts.Count; ++i){
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex]          =  verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (roundingOffset != Vector2.zero){
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                }

                m_TempVerts[tempVertsIndex].color = col;
                if (tempVertsIndex == 3){
                    Vector3 center = CalcCenter(m_TempVerts[0].position, m_TempVerts[2].position);
                    m_TempVerts[0].position = ScaleWithCenter(ratio, center, m_TempVerts[0].position, offset);
                    m_TempVerts[1].position = ScaleWithCenter(ratio, center, m_TempVerts[1].position, offset);
                    m_TempVerts[2].position = ScaleWithCenter(ratio, center, m_TempVerts[2].position, offset);
                    m_TempVerts[3].position = ScaleWithCenter(ratio, center, m_TempVerts[3].position, offset);
                    toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
        }

        protected Vector3 CalcCenter(Vector3 p1, Vector3 p2){
            float maxX = Mathf.Max(p1.x, p2.x);
            float minX = Mathf.Min(p1.x, p2.x);
            float maxY = Mathf.Max(p1.y, p2.y);
            float minY = Mathf.Min(p1.y, p2.y);
            float maxZ = Mathf.Max(p1.z, p2.z);
            float minZ = Mathf.Min(p1.z, p2.z);
            return new Vector3((maxX - minX) / 2 + minX, (maxY - minY) / 2 + minY, (maxZ - minZ) / 2 + minZ);
        }

        public struct MaxDirPos{
            //public int4 xyzw;
            public Vector4 maxLen;
        }

        protected MaxDirPos CalcRect(IList<UIVertex> vertexs){
            //int4 xyzw = new int4(0, 0, 0, 0);
            Vector4 maxLen = new Vector4(0, 0, 0, 0);
            if (m_EnableGradient){
                for (int i = 0; i < vertexs.Count; i++){
                    var vp = vertexs[i].position;
                    if (vp.x > maxLen.x){
                        maxLen.x = vp.x;
                        //xyzw.x = i;
                    }

                    if (vp.x < maxLen.y){
                        maxLen.y = vp.x;
                        //xyzw.y = i;
                    }

                    if (vp.y > maxLen.z){
                        maxLen.z = vp.y;
                        //xyzw.z = i;
                    }

                    if (vp.y < maxLen.w){
                        maxLen.w = vp.y;
                        //xyzw.w = i;
                    }
                }
            }

            MaxDirPos result;
            //result.xyzw = xyzw;
            result.maxLen = maxLen;
            return result;
        }

        protected Vector3 ScaleWithCenter(float ratio, Vector3 center, Vector3 src, Vector3 offset){
            src -= center;
            Matrix4x4 sm = new Matrix4x4(
                                         new Vector4(ratio, 0,     0,     0),
                                         new Vector4(0,     ratio, 0,     0),
                                         new Vector4(0,     0,     ratio, 0),
                                         new Vector4(0,     0,     0,     1)
                                        );
            src += (center / ratio + offset);
            return sm.MultiplyPoint(src);
        }

    #endregion
    }
}