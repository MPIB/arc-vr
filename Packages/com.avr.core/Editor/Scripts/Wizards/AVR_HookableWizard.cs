﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AVR.UEditor.Core {
    /// Short documentation on HookableWizard:
    /// A Hookable Wizard is a simple wizard to change porperties on a certain object or component. A typical example: Adding Modules to a VR Controller (such as movement provider etc.)
    /// The wizard itself is merely an empty editorwindow with a title and a "Update" button. The actual contents/options are inserted via "WizardHooks".
    /// Take the example given above. The wizard to add controller modules by itself is empty. But each package adds individual hooks by declaring them. (arc-vr-motion would add the option to add a MovementProvider module).
    /// By keeping the wizards and hooks separate from one another, it allows us to add wizard-options on a per-package basis.
    /// 
    /// Creating a new wizard is as simple as this:
    //# public class MyWizard : AVR_HookableWizard<MyWizard> {}
    /// Keep in mind that the class needs to *self-referencing* to work. (T needs to be the class of the wizard itself)
    /// 
    /// Now, to display thw wizard one must only call the following:
    //# MyWizard.ShowWindow(referenceObject, "My Wizard");
    /// 
    /// As for the Hooks. Declaring a new hook (option) for your wizard consits only of declaring the class. An example for a simple hook:
    //# public class MyHook : AVR_WizardHook<MyWizard> { }
    /// 
    /// Just by exisitng, this hook now automatically appears in "MyWizard". Note that *how* this hook functions is not defined. Thus, it will simply appear as a label stating "Unnamed Module".
    /// If you are looking for a simple hook, namely: a toggle field that will add or remove a given prefab with a certain component as a child to our target object, there is a preexisitng class:
    //# public class MyHook : AVR_WizardHook_SimpleToggle<MyWizard, MyComponent> {
    //#     protected override string moduleName => "My Module";
    //#     protected override string prefabPathSettingsToken => "/some/settings/token";
    //# }
    /// This hook will add a toggle-field with the label "My Module" to the MyWizard. If the toggle is turned on and the targetObject has no MyComponent on itself or its children, it will
    /// instantiate a prefab found under the path designated by the prefabPathSettingsToken (in AVR_Settings).
    /// If the toggle is off, all gameobjects with a "MyComponent" will be destroyed.
    /// 
    /// Implementing your own custom hooks is easy. There are only three methods to implement:
    /// - void on_create_wizard(GameObject targetObject) will be called when a wizard that contains this hook is created with the given target gameobject
    /// - void embed_GUI() contains the GUI of the given hook. If the hook should consist of a simple boolean toggle, you would write something along the lines of EditorGUILayout.Toggle(...) in here.
    /// - void on_submit(GameObject targetObject) is called when the "Update" button on the wizard is called. This is where you ought to save your changes, instantiate prefabs or anything else.

    public abstract class AVR_HookableWizard<T> : EditorWindow where T : AVR_HookableWizard<T>
    {
        private static List<AVR_WizardHook<T>> registered_hooks = new List<AVR_WizardHook<T>>();

        private static GameObject targetObject;

        public static void ShowWindow(GameObject _targetObject, string windowName = "Wizard")
        {
            registered_hooks.Clear();
            targetObject = _targetObject;

            // Get all classes that inherit from ModuleWizard_Hook
            List<System.Type> hook_classes = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(AVR_WizardHook<T>))).ToList();

            foreach (System.Type hook in hook_classes) {
                // Don't register abstract hooks.
                if(hook.IsAbstract) continue;

                AVR_WizardHook<T> o = (AVR_WizardHook<T>)System.Activator.CreateInstance(hook);

                registered_hooks.Add(o);
                o.on_create_wizard(targetObject);
            }

            registered_hooks.Sort((a, b) => a.category.CompareTo(b.category));

            EditorWindow.GetWindow(typeof(T), true, windowName);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            foreach(AVR_WizardHook<T> o in registered_hooks) {
                EditorGUILayout.BeginVertical();

                o.embed_GUI();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            if(GUILayout.Button("Update")) {
                foreach (AVR_WizardHook<T> o in registered_hooks) {
                    o.on_submit(targetObject);
                }
                this.Close();
            }

        }
    }



    public abstract class AVR_WizardHook<Wiz> where Wiz : AVR_HookableWizard<Wiz>
    {
        // Flags can be used for WizardHooks to communicate with one another. Example: dependecies on InputManager
        protected static Dictionary<string, bool> flags = new Dictionary<string, bool>();

        public virtual int category
        {
            get { return 9; }
        }

        public virtual void on_create_wizard(GameObject targetObject)
        {

        }

        public virtual void embed_GUI()
        {
            EditorGUILayout.LabelField("Unnamed Module");
        }

        public virtual void on_submit(GameObject targetObject)
        {
            PrefabUtility.UnpackPrefabInstance(targetObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        }

        protected virtual void safeDestroyImmediate(GameObject toDestroy, GameObject wizardTarget) {
            try {
                GameObject.DestroyImmediate(toDestroy);
            }
            catch(System.InvalidOperationException) {
                PrefabUtility.UnpackPrefabInstance(wizardTarget, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                GameObject.DestroyImmediate(toDestroy);
            }
        }
    }

    public abstract class AVR_WizardHook_SimpleToggle<Wiz, Mod> : AVR_WizardHook<Wiz> where Wiz : AVR_HookableWizard<Wiz> where Mod : MonoBehaviour
    {
        protected abstract string moduleName { get; }

        protected abstract string prefabPathSettingsToken { get; }

        protected virtual string[] dependencies => new string[] {};

        protected Mod[] _module;

        public bool module;

        public override void on_create_wizard(GameObject targetObject)
        {
            _module = targetObject.GetComponentsInChildren<Mod>();
            module = _module.Length > 0;
            flags[moduleName] = module;
        }

        public override void embed_GUI()
        {
            module = EditorGUILayout.BeginToggleGroup(moduleName, module);
            // Nothing to see here
            EditorGUILayout.EndToggleGroup();

            // See if there are any missing dependencies
            string[] missing_d = dependencies.Where((s) => !flags[s]).ToArray();
            // If yes, display error
            if(module && missing_d.Length > 0) {
                EditorGUILayout.HelpBox(moduleName + " needs " + string.Join(", ", missing_d), MessageType.Error);
            }

            // Set flag for this module
            flags[moduleName] = module;
        }

        public override void on_submit(GameObject targetObject)
        {
            if (module && _module.Length < 1)
            {
                AVR_EditorUtility.InstantiatePrefabAsChild(targetObject.transform, prefabPathSettingsToken);
            }
            else if (!module && _module.Length > 0)
            {
                foreach (Mod c in _module) safeDestroyImmediate(c.gameObject, targetObject);
            }
        }
    }

    public abstract class AVR_WizardHook_SimpleFilteredToggle<Wiz, Mod> : AVR_WizardHook_SimpleToggle<Wiz, Mod> where Wiz : AVR_HookableWizard<Wiz> where Mod : MonoBehaviour
    {
        protected abstract System.Func<Mod, bool> filter { get; }

        public override void on_create_wizard(GameObject targetObject)
        {
            _module = targetObject.GetComponentsInChildren<Mod>().Where(filter).ToArray();
            module = _module.Length > 0;
            flags[moduleName] = module;
        }
    }

    public abstract class AVR_WizardHook_DropdownChoiceToggle<Wiz, Mod> : AVR_WizardHook<Wiz> where Wiz : AVR_HookableWizard<Wiz> where Mod : MonoBehaviour
    {
        protected abstract string moduleName { get; }

        protected abstract DDChoice[] options { get; }

        protected virtual string[] dependencies => new string[] { };

        protected struct DDChoice {
            public DDChoice(string choiceName, string prefabPathSettingsToken, System.Func<Mod, bool> filter) {
                this.choiceName = choiceName;
                this.prefabPathSettingsToken = prefabPathSettingsToken;
                this.filter = filter;
            }

            public string choiceName;
            public string prefabPathSettingsToken;
            public System.Func<Mod, bool> filter;
        }

        protected Mod[] _module;

        protected DDChoice _selected;
        protected DDChoice _prevselected;

        private string [] moduleTypeList;

        public bool module;

        public override void on_create_wizard(GameObject targetObject)
        {
            _module = targetObject.GetComponentsInChildren<Mod>();
            moduleTypeList = options.Select(t => t.choiceName).ToArray();

            module = false;

            foreach(var entry in options) {
                if(_module.Any(entry.filter)) {
                    _prevselected = _selected = entry;
                    module = true;
                    break;
                }
            }

            flags[moduleName] = module;
        }

        public override void embed_GUI()
        {
            module = EditorGUILayout.BeginToggleGroup(moduleName, module);

            int index = Mathf.Max(0, System.Array.FindIndex(moduleTypeList, w => w == _selected.choiceName));
            index = EditorGUILayout.Popup(index, moduleTypeList);
            _selected = options[index];

            EditorGUILayout.EndToggleGroup();

            // See if there are any missing dependencies
            string[] missing_d = dependencies.Where((s) => !flags[s]).ToArray();
            // If yes, display error
            if (module && missing_d.Length > 0)
            {
                EditorGUILayout.HelpBox(moduleName + " needs " + string.Join(", ", missing_d), MessageType.Error);
            }

            // Set flag for this module
            flags[moduleName] = module;
        }

        public override void on_submit(GameObject targetObject)
        {
            if (module && _module.Length < 1)
            {
                AVR_EditorUtility.InstantiatePrefabAsChild(targetObject.transform, _selected.prefabPathSettingsToken);
            }
            else if (module && _module.Length > 0)
            {
                Mod first = _module[0];
                if (!_selected.filter.Invoke(first)) {
                    safeDestroyImmediate(first.gameObject, targetObject);
                    AVR_EditorUtility.InstantiatePrefabAsChild(targetObject.transform, _selected.prefabPathSettingsToken);
                }
            }
            else if (!module && _module.Length > 0)
            {
                foreach (Mod c in _module) safeDestroyImmediate(c.gameObject, targetObject);
            }
        }
    }
}
