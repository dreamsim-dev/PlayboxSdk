using System;
using System.Collections.Generic;
using CI.Utils.Extentions;
#if UNITY_EDITOR
using InspectorButton;
#endif
using Playbox.SdkConfigurations;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playbox
{
    public class MainInitialization : PlayboxBehaviour
    {
        [SerializeField] private bool useInAppValidation = true;
        [SerializeField] private bool isDebugSplash = false;
        
        private List<PlayboxBehaviour> behaviours = new();
        
        private const string objectName = "[Global] MainInitialization";
        
        private static Dictionary<string,bool> initStatus = new();

        public static Dictionary<string, bool> InitStatus
        {
            get => initStatus ??= new Dictionary<string, bool>();
            set => initStatus = value;
        }

        public static Action PostInitialization = delegate { };
        public static Action PreInitialization = delegate { };

        private void Awake()
        {
            Initialization();
        }
        
        public override void Initialization()
        {
            PreInitialization?.Invoke();
            
            if(Application.isPlaying)
                DontDestroyOnLoad(gameObject);
            
            GlobalPlayboxConfig.Load();
            
            if(isDebugSplash) behaviours.Add(AddToGameObject<PlayboxSplashUGUILogger>(gameObject));
            behaviours.Add(AddToGameObject<FirebaseInitialization>(gameObject));
            behaviours.Add(AddToGameObject<AppsFlyerInitialization>(gameObject));
            behaviours.Add(AddToGameObject<DevToDevInitialization>(gameObject));
            behaviours.Add(AddToGameObject<FacebookSdkInitialization>(gameObject));
            behaviours.Add(AddToGameObject<AppLovinInitialization>(gameObject));
            behaviours.Add(AddToGameObject<InAppVerification>(gameObject, useInAppValidation) ?? null);
            
            if(isDebugSplash) InitStatus[nameof(PlayboxSplashUGUILogger)] = false;
            InitStatus[nameof(FirebaseInitialization)] = false;
            InitStatus[nameof(AppsFlyerInitialization)] = false;
            InitStatus[nameof(DevToDevInitialization)] = false;
            InitStatus[nameof(FacebookSdkInitialization)] = false;
            InitStatus[nameof(AppLovinInitialization)] = false;
            
            foreach (var item in behaviours)
            {
                if(item != null)
                    item.GetInitStatus(() =>
                    {
                        item.playboxName.PlayboxInfo("INITIALIZED");
                        InitStatus[item.playboxName] = true;
                        
                    });
            }
            
            foreach (var item in behaviours)
            {
                if(item != null)
                    item.Initialization();
            }
            
            PostInitialization?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (var item in behaviours)
            { 
                if(item != null)
                    item.Close();   
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var mainInit = FindObjectOfType<MainInitialization>();

            if (mainInit != this)
            {
                if(mainInit != null)
                    Destroy(mainInit.gameObject);
            }

        }
        
#if UNITY_EDITOR
        [MenuItem("PlayBox/Initialization/Create")]
        public static void CreateAnalyticsObject()
        { 
            var findable = GameObject.Find(objectName);

            if (findable == null)
            {
                var go = new GameObject(objectName); 
                
                go.AddComponent<MainInitialization>();
            }
            else
            {
                if (findable.TryGetComponent(out MainInitialization main))
                {
                    DestroyImmediate(main);
                }
                else
                {
                    findable.AddComponent<MainInitialization>();   
                }
            }
            
        }
        
        [MenuItem("PlayBox/Initialization/Remove")]
        public static void RemoveAnalyticsObject()
        {
            var go = GameObject.Find(objectName);

            if (go != null)
            {
                DestroyImmediate(go);
            }

        }
#endif
    }
}