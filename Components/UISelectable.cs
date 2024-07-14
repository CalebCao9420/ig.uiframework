using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = System.Object;

/// <summary>
/// UGUI扩展
/// </summary>
/// 
namespace IG.Module.UI{
    public class UIDragSelectable : MonoBehaviour, IDragHandler{
        public Action<PointerEventData> OnTouchMove;
        public UIDragSelectable(){ }

        public void OnDrag(PointerEventData eventData){
            if (null != OnTouchMove){
                OnTouchMove(eventData);
            }
        }
    }

    public class UISelectable : MonoBehaviour,
                                IPointerClickHandler,
                                IPointerDownHandler,
                                IPointerUpHandler,
                                IPointerEnterHandler,
                                IPointerExitHandler /*,IDragHandler*/{
        public Action<PointerEventData> OnClick;
        public Action<PointerEventData> OnUp;
        public Action<PointerEventData> OnDown;
        public Action<PointerEventData> OnEnter;
        public Action<PointerEventData> OnExit;
        public UISelectable(){ }

        public void OnPointerClick(PointerEventData eventData){
            if (null != OnClick){
                OnClick(eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData){
            if (null != OnDown){
                OnDown(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData){
            if (null != OnUp){
                OnUp(eventData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData){
            if (null != OnEnter){
                OnEnter(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData){
            if (null != OnExit){
                OnExit(eventData);
            }
        }
    }

#region GameObject UISelectalbe relative

    public static class GameObjectUIExtensions{
        public static void AddClick(this GameObject go, Action click){
            UISelectable selectable = go.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = go.AddComponent<UISelectable>();
            }

            if (null != selectable){
                selectable.OnClick = (PointerEventData data) => {
                    click();
                };
            }
        }

        public static void AddClick(this GameObject go, Action<PointerEventData, GameObject> click){
            UISelectable selectable = go.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = go.AddComponent<UISelectable>();
            }

            if (null != selectable){
                selectable.OnClick = (PointerEventData data) => {
                    click(data, go);
                };
            }
        }

        public static void RemoveClick(this GameObject go){
            UISelectable selectable = go.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = go.AddComponent<UISelectable>();
            }

            if (null != selectable){
                selectable.OnClick = null;
                GameObject.Destroy(selectable);
            }
        }

        public static void AddTouchDown(this GameObject go, Action<PointerEventData> OnTouchDown){
            UISelectable selectable = go.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = go.AddComponent<UISelectable>();
            }

            selectable.OnDown = OnTouchDown;
        }

        public static void AddTouchUp(this GameObject go, Action<PointerEventData> OnTouchUp){
            UISelectable selectable = go.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = go.AddComponent<UISelectable>();
            }

            selectable.OnUp = OnTouchUp;
        }
    }

#endregion

#region Image UISelectalbe relative

    /// <summary>
    /// Image扩展
    /// </summary>
    public static class ImageExtensions{
        public static UISelectable AddClick(this Image img, Action<Object> click){
            UISelectable selectable = img.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = img.gameObject.AddComponent<UISelectable>();
            }

            if (null != selectable){
                selectable.OnClick = (PointerEventData data) => {
                    if (null != click){
                        click(null);
                    }
                };
            }

            img.raycastTarget = true;
            return selectable;
        }

        public static UISelectable AddClick(this Image img, Action<Object> click, Object value = null){
            if (click == null){
                return null;
            }

            UISelectable selectable = img.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = img.gameObject.AddComponent<UISelectable>();
            }

            if (null != selectable){
                Action<PointerEventData> action = (PointerEventData data) => {
                    click(value);
                };
                if (selectable.OnClick == null){
                    selectable.OnClick = action;
                }
                else{
                    selectable.OnClick += action;
                }
            }

            img.raycastTarget = true;
            return selectable;
        }

        public static UISelectable AddClick(this Image img, Action click){
            if (null == click){
                return null;
            }

            UISelectable selectable = img.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = img.gameObject.AddComponent<UISelectable>();
            }

            if (null != selectable){
                Action<PointerEventData> action = (PointerEventData data) => {
                    click();
                };
                if (selectable.OnClick == null){
                    selectable.OnClick = action;
                }
                else{
                    selectable.OnClick += action;
                }
            }

            img.raycastTarget = true;
            return selectable;
        }

        public static void AddTouchDown(this Image img, Action<PointerEventData> OnTouchDown){
            UISelectable selectable = img.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = img.gameObject.AddComponent<UISelectable>();
            }

            selectable.OnDown = OnTouchDown;
        }

        public static void AddTouchUp(this Image img, Action<PointerEventData> OnTouchUp){
            UISelectable selectable = img.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = img.gameObject.AddComponent<UISelectable>();
            }

            selectable.OnUp = OnTouchUp;
        }

        public static void AddTouchEnter(this Image img, Action<PointerEventData> OnTouchEnter){
            UISelectable selectable = img.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = img.gameObject.AddComponent<UISelectable>();
            }

            selectable.OnEnter = OnTouchEnter;
        }

        public static void AddTouchMove(this Image img, Action<PointerEventData> OnTouchMove){
            UIDragSelectable selectable = img.gameObject.GetComponent<UIDragSelectable>();
            if (null == selectable){
                selectable = img.gameObject.AddComponent<UIDragSelectable>();
            }

            selectable.OnTouchMove = OnTouchMove;
        }

        /// <summary>
        /// 移除点击响应
        /// </summary>
        /// <param name="img"></param>
        public static void RemoveClick(this Image img, bool removeSelect = true){
            UISelectable selectable = img.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = img.gameObject.AddComponent<UISelectable>();
            }

            if (null != selectable){
                selectable.OnClick = null;
                if (removeSelect){
                    GameObject.Destroy(selectable);
                }
            }
        }
    }

#endregion

#region Text UISelectalbe relative

    public static class TextExtensions{
        /// <summary>
        /// 添加点击响应
        /// </summary>
        /// <param name="img"></param>
        /// <param name="click"></param>
        public static void AddClick(this Text txt, Action<PointerEventData> click){
            UISelectable selectable = txt.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = txt.gameObject.AddComponent<UISelectable>();
            }

            if (null != selectable){
                selectable.OnClick = click;
            }
        }

        /// <summary>
        /// 移除点击响应
        /// </summary>
        /// <param name="img"></param>
        public static void RemoveClick(this Text txt){
            UISelectable selectable = txt.gameObject.GetComponent<UISelectable>();
            if (null == selectable){
                selectable = txt.gameObject.AddComponent<UISelectable>();
            }

            if (null != selectable){
                selectable.OnClick = null;
                GameObject.Destroy(selectable);
            }
        }
    }

#endregion

    //TODO:
    // public static class UIToggleExtentions{
    //     public static void IsGroupChildren(this UIToggle toggle, bool value){ toggle.IsClickToggle = !value; }
    // }
}