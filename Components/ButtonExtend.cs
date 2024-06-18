using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace IG.Module.UI{
    public static class ButtonExtend{
        public static void SetInteractable(this Button button, bool val, Color normalColor, Color disabledColor){
            button.interactable = val;
            TMP_Text[] texts = button.GetComponentsInChildren<TMP_Text>();
            if (val){
                for (int i = 0; i < texts.Length; i++){
                    texts[i].color = normalColor;
                }
            }
            else{
                for (int i = 0; i < texts.Length; i++){
                    texts[i].color = disabledColor;
                }
            }

            GameText[] gameTexts = button.GetComponentsInChildren<GameText>();
            if (val){
                for (int i = 0; i < gameTexts.Length; i++){
                    gameTexts[i].color = normalColor;
                }
            }
            else{
                for (int i = 0; i < gameTexts.Length; i++){
                    gameTexts[i].color = disabledColor;
                }
            }
        }

        public static void SetInteractableOfDefault(this Button button, bool val){
            ColorBlock colors = button.colors;
            colors.disabledColor = Color.gray;
            button.colors        = colors;
            button.SetInteractable(val);
        }

        public static void SetInteractable(this  Button        button,    bool        val,  Color       normalColor){ button.SetInteractable(val, normalColor, Color.gray); }
        public static void AddClickListener(this Transform     transform, string      name, UnityAction call)       { transform.Find<Button>(name).AddClickListener(call); }
        public static void AddClickListener(this MonoBehaviour behaviour, string      name, UnityAction call)       { behaviour.Find<Button>(name).AddClickListener(call); }
        public static void AddClickListener(this Button        button,    UnityAction call){ button.onClick.AddListener(call); }

        public static void AddValueChanged(this MonoBehaviour behaviour, string name, UnityAction<string> call){
            TMP_InputField tmpInputField = behaviour.Find<TMP_InputField>(name);
            if (tmpInputField.AddValueChanged(call)){
                return;
            }

            InputField inputField = behaviour.Find<InputField>(name);
            inputField.AddValueChanged(call);
        }

        public static bool AddValueChanged(this TMP_InputField inputField, UnityAction<string> call){
            if (inputField != null){
                inputField.onValueChanged.AddListener(call);
                return true;
            }

            return false;
        }

        public static bool AddValueChanged(this InputField inputField, UnityAction<string> call){
            if (inputField != null){
                inputField.onValueChanged.AddListener(call);
                return true;
            }

            return false;
        }
    }
}