using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IG.Module.UI{
    public class DefaultIcon : GameScrollItem, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler{
        [SerializeField]
        private List<EDefFrameInfo> _frames;

        [SerializeField]
        private List<EDefIconInfo> _icons;

        public  GameImage       Frame;
        public  GameImage       Icon;
        private UIDisable       _disable;
        public  UIDisable       Disable{ get{ return this._disable ?? (this._disable = this.GetComponent<UIDisable>()); } }
        public  DefaultIconData Data   { get; private set; }

        protected override void SetData(bool isRepeatRefresh){
            this.Data         = (this._Obj as DefaultIconData);
            this.Frame.sprite = _frames.Find(f => f.m_type == this.Data.m_FrameType).m_Sprite;
            this.Icon.sprite  = _icons.Find(f => f.m_type  == this.Data.m_IconType).m_Sprite;
        }

        public override float GetWidth()                                { return Icon.mainTexture.width; }
        public override float GetHeight()                               { return Icon.mainTexture.height; }
        public          void  OnPointerClick(PointerEventData eventData){ }
        public          void  OnPointerDown(PointerEventData  eventData){ }
        public          void  OnPointerUp(PointerEventData    eventData){ }
    }

    [Serializable]
    public class DefaultIconData : IGameScrollItemData{
        public EDefFrameType m_FrameType      { get; private set; }
        public EDefIconType  m_IconType       { get; private set; }
        public string        IconResourcesPath{ get; private set; }
        public Action        onCallBack;
        public bool          IsClick = true;

        public DefaultIconData(EDefFrameType TFrame, EDefIconType TIcon, Action onCallBack){
            this.m_FrameType = TFrame;
            this.m_IconType  = TIcon;
            this.onCallBack  = onCallBack;
        }

        public DefaultIconData(EDefFrameType TFrame, EDefIconType TIcon, string path, Action onCallBack){
            this.m_FrameType       = TFrame;
            this.m_IconType        = TIcon;
            this.onCallBack        = onCallBack;
            this.IconResourcesPath = path;
        }

        public DefaultIconData(string path, EDefFrameType TFrame, Action onCallBack){
            this.m_IconType        = EDefIconType.Free;
            this.m_FrameType       = TFrame;
            this.onCallBack        = onCallBack;
            this.IconResourcesPath = path;
        }
    }

    [Serializable]
    public class EDefFrameInfo{
        public EDefFrameType m_type;
        public Sprite        m_Sprite;
    }

    [Serializable]
    public class EDefIconInfo{
        public EDefIconType m_type;
        public Sprite       m_Sprite;
    }

    public enum EDefFrameType{
        Type1 = 1,
        Type2 = 2,
    }

    public enum EDefIconType{
        Free     = 0,
        Plus     = 1,
        Remove   = 2,
        NoImage  = 3,
        SetOK    = 4,
        CanEquip = 5,
    }
}