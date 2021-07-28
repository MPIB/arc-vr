using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// Combination of AVR_Component with a singleton.
    /// </summary>
    /// <typeparam name="T">Type of the Singleton. Should correspond to the class deriving from this. Example: class Example : AVR_SingletonComponent&lt;Example&gt;</typeparam>
    public class AVR_SingletonComponent<T> : AVR_Component where T : MonoBehaviour
    {
        public static T Instance {
            get{
                if(_Instance==null) _Instance = FindObjectOfType<T>();
                return _Instance;
            }
            private set{
                _Instance = value;
            }
        }

        private static T _Instance;

        protected override void Awake()
        {
            base.Awake();

            SetInstance();
        }

        private void SetInstance() {
            if (Instance != null)
            {
                AVR_DevConsole.cwarn(this.name + " is marked as Singleton but another instance was found in the scene. Continuing to use old one.", this);
                return;
            }

            Instance = gameObject.GetComponent<T>();

            if (Instance == null)
            {
                AVR_DevConsole.cerror(this.name + " is marked as a Singleton of type " + typeof(T).ToString() + " but no component of this type was found!", this);
            }

            DontDestroyOnLoad(this);
        }

        public virtual void OnApplicationQuit()
        {
            Instance = null;
        }
    }
}