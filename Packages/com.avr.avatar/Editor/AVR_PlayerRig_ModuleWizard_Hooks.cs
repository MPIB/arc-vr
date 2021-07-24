using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.UEditor.Core
{
    public class AVR_PlayerRigWizard_Hook_AvatarModule : AVR_WizardHook_SimpleToggle<AVR_PlayerRig_ModuleWizard, AVR.Avatar.AVR_PoseProvider>
    {
        protected override string moduleName => "Avatar Module";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/avatarModule";

        public override void on_submit(GameObject targetObject) {
            base.on_submit(targetObject);

            AVR.Avatar.AVR_PoseProvider prov = targetObject.GetComponent<AVR.Avatar.AVR_PoseProvider>();
        }
    }

    public class AVR_PlayerRigWizard_Hook_SimpleAvatarModule : AVR_WizardHook_SimpleToggle<AVR_PlayerRig_ModuleWizard, AVR.Avatar.AVR_SimpleAvatar>
    {
        protected override string moduleName => "SimpleAvatar Module";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/simpleAvatarModule";
    }
}

