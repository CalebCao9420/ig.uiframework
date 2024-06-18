/*
 * Copyright (C) 2012 GREE, Inc.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System.Collections;
using System.IO;
using IG.Runtime.Log;
using UnityEngine;

namespace IG.Module.UI{
    public class WebView : MonoBehaviour{
        string        Url;
        WebViewObject webViewObject;
        bool          isLoaded = false;
        string        jsData   = "";

        public void OpenUrl(string url, int left, int top, int right, int bottom){
            this.Url      = url + "?" + Time.time;
            this.jsData   = "";
            this.isLoaded = false;
            StartCoroutine(Load(left, top, right, bottom));
        }

        IEnumerator Load(int left, int top, int right, int bottom){
            webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
            RectTransform rectTransform = webViewObject.gameObject.AddComponent<RectTransform>();
            webViewObject.transform.SetParent(this.transform);
            rectTransform.anchorMin          = Vector2.zero;
            rectTransform.anchorMax          = Vector2.one;
            rectTransform.anchoredPosition3D = new Vector3(100, 100, 0);
            rectTransform.anchoredPosition   = new Vector2(100, 100);
            webViewObject.Init(
                               cb : (msg) => {
                                   this.Log(string.Format("CallFromJS[{0}]", msg));
                               },
                               err : (msg) => {
                                   this.Log(string.Format("CallOnError[{0}]", msg));
                               },
                               ld : (msg) => {
                                   this.isLoaded = true;
                                   this.Log(string.Format("CallOnLoaded[{0}]", msg));
#if !UNITY_ANDROID
                                   // NOTE: depending on the situation, you might prefer
                                   // the 'iframe' approach.
                                   // cf. https://github.com/gree/unity-webview/issues/189
#if true
                                   webViewObject.EvaluateJS(
                                                            @"
                  window.Unity = {
                    call: function(msg) {
                      window.location = 'unity:' + msg;
                    }
                  }
                "
                                                           );
#else
				webViewObject.EvaluateJS(@"
				window.Unity = {
				call: function(msg) {
				var iframe = document.createElement('IFRAME');
				iframe.setAttribute('src', 'unity:' + msg);
				document.documentElement.appendChild(iframe);
				iframe.parentNode.removeChild(iframe);
				iframe = null;
				}
				}
				");
#endif
#endif
                                   webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
                                   if (!string.IsNullOrEmpty(jsData)){
                                       PEvaluateJS(jsData);
                                       jsData = "";
                                   }
                               },
                               //ua: "custom user agent string",
                               enableWKWebView : true
                              );
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		webViewObject.bitmapRefreshCycle = 1;
#endif
            webViewObject.SetMargins(left, top, right, bottom);
            webViewObject.SetVisibility(true);
#if !UNITY_WEBPLAYER
            if (Url.StartsWith("http")){
                webViewObject.LoadURL(Url.Replace(" ", "%20"));
            }
            else{
                var exts = new string[]{
                                           ".jpg", ".js", ".html" // should be last
                                       };
                foreach (var ext in exts){
                    var    url    = Url.Replace(".html", ext);
                    var    src    = Path.Combine(Application.streamingAssetsPath, url);
                    var    dst    = Path.Combine(Application.persistentDataPath,  url);
                    byte[] result = null;
                    if (src.Contains("://")){
                        // for Android
                        var www = new WWW(src);
                        yield return www;
                        result = www.bytes;
                    }
                    else{
                        result = File.ReadAllBytes(src);
                    }

                    File.WriteAllBytes(dst, result);
                    if (ext == ".html"){
                        webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                        break;
                    }
                }
            }
#else
		if (Url.StartsWith("http")) {
		webViewObject.LoadURL(Url.Replace(" ", "%20"));
		} else {
		webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
		}
		webViewObject.EvaluateJS(
		"parent.$(function() {" +
		"   window.Unity = {" +
		"       call:function(msg) {" +
		"           parent.unityWebView.sendMessage('WebViewObject', msg)" +
		"       }" +
		"   };" +
		"});");
#endif
            yield break;
        }

        public void EvaluateJS(string js){
            if (this.isLoaded){
                PEvaluateJS(js);
            }
            else{
                this.jsData = js;
            }
        }

        private void PEvaluateJS(string js){
            this.Log("js:" + js);
            webViewObject.EvaluateJS(js);
        }

        public void SetVisibility(bool value){ webViewObject?.SetVisibility(value); }
    }
}