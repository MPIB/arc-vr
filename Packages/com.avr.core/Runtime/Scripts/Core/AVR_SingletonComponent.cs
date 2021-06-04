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
        public static T Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance != null)
            {
                AVR_DevConsole.cwarn(this.name + " is marked as Singleton but another instance was found in the scene. Destroying object: " + Instance.name, this);
                GameObject.Destroy(Instance);
            }

            Instance = gameObject.GetComponent<T>();

            if (Instance == null)
            {
                AVR_DevConsole.cerror(this.name + " is marked as a Singleton of type " + typeof(T).ToString() + " but no component of this type was found!", this);
            }

            DontDestroyOnLoad(this);

            base.Awake();
        }

        public virtual void OnApplicationQuit()
        {
            Instance = null;
        }
    }
}