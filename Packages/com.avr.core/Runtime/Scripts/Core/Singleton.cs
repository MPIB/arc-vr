using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// Basic class for Singleton classes. Sets a single, static instance of the class to the "Instance" property.
    /// </summary>
    /// <typeparam name="T"> Type of the Singleton. Should correspond to the class deriving from this. Example: class Example : Singleton&lt;Example&gt; </typeparam>
    public abstract class Singleton<T> : AVR_Behaviour where T : AVR_Behaviour
    {
        public static T Instance
        {
            get
            {
                if (_Instance == null) _Instance = FindObjectOfType<T>();
                return _Instance;
            }
            private set
            {
                _Instance = value;
            }
        }

        private static T _Instance;

        void Awake()
        {
            SetInstance();
        }

        private void SetInstance()
        {
            if (_Instance != null && _Instance != this)
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