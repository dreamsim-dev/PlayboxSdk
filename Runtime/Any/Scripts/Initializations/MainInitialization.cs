using System.Collections.Generic;
using CI.Utils.Extentions;
using EditorCools;
using Playbox.SdkConfigurations;

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
        
        private List<PlayboxBehaviour> behaviours = new();
        
        private const string objectName = "[Global] MainInitialization";

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            GlobalPlayboxConfig.Load();
            
            behaviours.Add(AddToGameObject<FirebaseInitialization>(gameObject));
            behaviours.Add(AddToGameObject<AppsFlyerInitialization>(gameObject));
            behaviours.Add(AddToGameObject<DevToDevInitialization>(gameObject));
            behaviours.Add(AddToGameObject<FacebookSdkInitialization>(gameObject));
            behaviours.Add(AddToGameObject<AppLovinInitialization>(gameObject));
            behaviours.Add(AddToGameObject<InAppVerification>(gameObject, useInAppValidation) ?? null);
            //behaviours.Add(AddToGameObject<InaAP>(gameObject));
            
            foreach (var item in behaviours)
            {
                if(item != null)
                    item.Initialization();
            }
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