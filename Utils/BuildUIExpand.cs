using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;

namespace IG.Module.UI{
    /// <summary>
    /// Build user interface expand.
    /// </summary>
    public static class BuildUIExpand{
        public static T Find<T>(this Transform value, string name) where T : Component{
            Transform transform = value.Find(name);
            if (transform == null){
                return null;
            }

            return transform.GetComponent<T>();
        }

        public static T Find<T>(this MonoBehaviour value, string name) where T : Component{
            Transform transform = value.transform.Find(name);
            if (transform == null){
                return null;
            }

            return transform.GetComponent<T>();
        }

        public static Transform Find(this Component value, string name){ return value.transform.Find(name); }

        public static bool BuildText(this int value, string textName, Transform transform, string format = ""){
            Transform textTransform = transform.Find(textName);
            if (textTransform == null){
                return false;
            }

            Text text = textTransform.GetComponent<Text>();
            if (text == null){
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null){
                    if (string.IsNullOrEmpty(format)){
                        textMesh.text = value.ToString();
                    }
                    else{
                        textMesh.text = string.Format(format, value);
                    }

                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this int value, string textName, Behaviour behaviour, string format = ""){
            Transform textTransform = behaviour.transform.Find(textName);
            if (textTransform == null){
                return false;
            }

            Text text = textTransform.GetComponent<Text>();
            if (text == null){
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null){
                    if (string.IsNullOrEmpty(format)){
                        textMesh.text = value.ToString();
                    }
                    else{
                        textMesh.text = string.Format(format, value);
                    }

                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this DateTime value, string textName, Behaviour behaviour, string format = ""){
            Transform textTransform = behaviour.transform.Find(textName);
            if (textTransform == null){
                return false;
            }

            Text text = textTransform.GetComponent<Text>();
            if (text == null){
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null){
                    if (string.IsNullOrEmpty(format)){
                        textMesh.text = value.ToString();
                    }
                    else{
                        textMesh.text = value.ToString(format);
                    }

                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = value.ToString(format);
            }

            return true;
        }

        public static bool BuildText(this string value, string textName, Transform transform, string format = ""){
            Transform textTransform = transform.Find(textName);
            if (textTransform == null){
                return false;
            }

            Text text = textTransform.GetComponent<Text>();
            if (text == null){
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null){
                    if (string.IsNullOrEmpty(format)){
                        textMesh.text = value;
                    }
                    else{
                        textMesh.text = string.Format(format, value);
                    }

                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value;
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this DateTime value, string textName, Transform transform, string format = ""){
            Transform textTransform = transform.Find(textName);
            if (textTransform == null){
                return false;
            }

            Text text = textTransform.GetComponent<Text>();
            if (text == null){
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null){
                    if (string.IsNullOrEmpty(format)){
                        textMesh.text = value.ToString();
                    }
                    else{
                        textMesh.text = value.ToString(format);
                    }

                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = value.ToString(format);
            }

            return true;
        }

        public static bool BuildText(this string value, string textName, Behaviour behaviour, string format = ""){
            Transform textTransform = behaviour.transform.Find(textName);
            if (textTransform == null){
                return false;
            }

            Text text = textTransform.GetComponent<Text>();
            if (text == null){
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null){
                    if (string.IsNullOrEmpty(format)){
                        textMesh.text = value;
                    }
                    else{
                        textMesh.text = string.Format(format, value);
                    }

                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value;
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this float value, string textName, Transform transform, string format){
            Transform textTransform = transform.Find(textName);
            if (textTransform == null){
                return false;
            }

            Text text = textTransform.GetComponent<Text>();
            if (text == null){
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null){
                    if (string.IsNullOrEmpty(format)){
                        textMesh.text = value.ToString();
                    }
                    else{
                        textMesh.text = string.Format(format, value);
                    }

                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this float value, string textName, Behaviour behaviour, string format = ""){
            Transform textTransform = behaviour.transform.Find(textName);
            if (textTransform == null){
                return false;
            }

            Text text = textTransform.GetComponent<Text>();
            if (text == null){
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null){
                    if (string.IsNullOrEmpty(format)){
                        textMesh.text = value.ToString();
                    }
                    else{
                        textMesh.text = string.Format(format, value);
                    }

                    return true;
                }

                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this DateTime value, GameText gameText, string format = ""){
            Text text = gameText;
            if (text == null){
                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = value.ToString(format);
            }

            return true;
        }

        public static bool BuildText(this DateTime value, TextMeshProUGUI gameText, string format = ""){
            TextMeshProUGUI text = gameText;
            if (text == null){
                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = value.ToString(format);
            }

            return true;
        }

        public static bool BuildText(this float value, GameText gameText, string format = ""){
            Text text = gameText;
            if (text == null){
                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this float value, TextMeshProUGUI gameText, string format = ""){
            TextMeshProUGUI text = gameText;
            if (text == null){
                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this int value, GameText gameText, string format = ""){
            Text text = gameText;
            if (text == null){
                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this int value, TextMeshProUGUI gameText, string format = ""){
            TextMeshProUGUI text = gameText;
            if (text == null){
                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this string value, GameText gameText, string format = ""){
            Text text = gameText;
            if (text == null){
                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value.ToString();
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool BuildText(this string value, TextMeshProUGUI gameText, string format = ""){
            TextMeshProUGUI text = gameText;
            if (text == null){
                return false;
            }

            if (string.IsNullOrEmpty(format)){
                text.text = value;
            }
            else{
                text.text = string.Format(format, value);
            }

            return true;
        }

        public static bool Build<T>(this List<T> value, GameScrollView gameScrollView) where T : IGameScrollItemData{
            gameScrollView.SetData<T>(value);
            return true;
        }

        //public static bool Build<T> (this List<T> value, GamePageScrollView gameScrollView)where T:IGameScrollItemData
        //{
        //	gameScrollView.SetDatas<T> (value);
        //	return true;
        //}

        public static string IsNullOrEmptyDefault(this string value, string defaultValue){
            if (string.IsNullOrEmpty(value)){
                return defaultValue;
            }

            return value;
        }
    }
}