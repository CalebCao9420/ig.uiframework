using System.Collections.Generic;
using System;
using IG.Runtime.Log;
using UnityEngine;

namespace IG.Module.UI.Res{
    /// <summary>
    /// Game resource interface
    /// </summary>
    public class GamePrefabPaths{
        static readonly Dictionary<string, PrefabInfo> s_viewPathDic = new Dictionary<string, PrefabInfo>();

        /// <summary>
        /// Adds the view path.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="isForceLocal">If set to <c>true</c> is force local.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected static void AddViewPath<T>(string path, bool isForceLocal = false) where T : MonoBehaviour{
            PrefabInfo prefabInfo = new PrefabInfo(path, isForceLocal);
            AddViewPath(typeof(T).ToString(), prefabInfo);
        }

        /// <summary>
        /// Adds the view path.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="isForceLocal">If set to <c>true</c> is force local.</param>
        /// <param name="isAsync">If set to <c>true</c> is async.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected static void AddViewPath<T>(string path, bool isForceLocal, bool isAsync) where T : MonoBehaviour{
            PrefabInfo prefabInfo = new PrefabInfo(path, isForceLocal, isAsync);
            AddViewPath(typeof(T).ToString(), prefabInfo);
        }

        private static void AddViewPath(string key, PrefabInfo prefabInfo){
            if (!s_viewPathDic.ContainsKey(key)){
                s_viewPathDic.Add(key, prefabInfo);
            }
            else{
                LogAnalyzer.Log("The " + key + " already exists in the PrefabPaths", LogType.Warning);
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="type">Type.</param>
        public static PrefabInfo GetPath(Type type){
            string key = type.ToString();
            return GetPath(key);
        }

        public static PrefabInfo GetPath(string key){
            if (s_viewPathDic.ContainsKey(key)){
                return s_viewPathDic[key];
            }

            throw new Exception(key + " Prefab No registration, please register first in the ViewPaths classã€‚");
        }

        /// <summary>
        /// Obtain the corresponding prefab path according to the view type.
        /// </summary>
        /// <returns>prefab path.</returns>
        /// <typeparam name="T">MonoBehaviour type.</typeparam>
        public static PrefabInfo GetPath<T>() where T : Component{ return GetPath(typeof(T)); }
    }
}

/// <summary>
/// Prefab Basic Information
/// </summary>
public class PrefabInfo{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrefabInfo"/> class.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="isForceLocal">If set to <c>true</c> is force local.</param>
    public PrefabInfo(string path, bool isForceLocal){
        this.path         = path;
        this.isForceLocal = isForceLocal;
        this.isAsync      = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrefabInfo"/> class.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="isForceLocal">If set to <c>true</c> is force local.</param>
    /// <param name="isAsync">If set to <c>true</c> is async.</param>
    public PrefabInfo(string path, bool isForceLocal, bool isAsync){
        this.path         = path;
        this.isForceLocal = isForceLocal;
        this.isAsync      = isAsync;
    }

    /// <summary>
    /// prefab path
    /// </summary>
    public string path = string.Empty;

    /// <summary>
    /// Whether to force local resources to load
    /// </summary>
    public bool isForceLocal = false;

    /// <summary>
    /// The is async.
    /// </summary>
    public bool isAsync = true;

    public string LocalPath{ get{ return path.Replace(".prefab", ""); } }
}