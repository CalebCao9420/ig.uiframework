using System;
using System.Collections;
using IG.AssetBundle;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameRawImage : RawImage{
        private          string _url;
        private          bool   _isNetAsset{ get{ return !string.IsNullOrEmpty(_url); } }
        private          bool   _isDone    = false;
        private readonly int    _maxCount  = 3;
        private          int    _loadCount = 0;

        public void AsyncLoadImage(string path, Action<RawImage> action = null, bool isForceLocal = false){
            AssetsSystem.LoadAsync(
                                   (o, oArg) => {
                                       Texture tex = o as Texture;
                                       this.texture = tex;
                                       action?.Invoke(this);
                                   },
                                   path
                                  );
        }

        public GameRawImage LoadImage(string path, bool isForceLocal = false){
            var tex = AssetsSystem.Load<Texture>(path);
            this.texture = tex;
            return this;
        }

        public void SetUrl(string url){
            if (string.IsNullOrEmpty(url)){
                return;
            }

            if (url.StartsWith("http", StringComparison.Ordinal)){
                _url = url;
                WWWLoad();
            }
            else{
                _url = string.Empty;
                AssetsSystem.LoadAsync(
                                       (o, oArg) => {
                                           Texture tex = o as Texture;
                                           this.texture = tex;
                                       },
                                       url
                                      );
            }

            this.name = url;
        }

        private void WWWLoad(){
            this.StopAllCoroutines();
            this.StartCoroutine(IEWWWLoad());
        }

        private IEnumerator IEWWWLoad(){
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
            yield return www.SendWebRequest();
            if (this != null && www.isDone && www.result != UnityWebRequest.Result.ConnectionError){
                this.texture = (www.downloadHandler as DownloadHandlerTexture).texture;
                _isDone       = true;
                www.Dispose();
                www = null;
            }
            else{
                www.Dispose();
                www = null;
                _loadCount++;
                if (_loadCount < _maxCount){
                    WWWLoad();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData){
            if (_isNetAsset && !_isDone && _loadCount >= _maxCount){
                _loadCount = 0;
                WWWLoad();
            }
        }

        protected override void OnDestroy(){ Resources.UnloadAsset(this.texture); }
    }
}