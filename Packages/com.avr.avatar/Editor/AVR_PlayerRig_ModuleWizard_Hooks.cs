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

            //prov.lookAtTarget = (AVR.Core.Utils.Misc.GlobalFind("LookAt", typeof(GameObject)) as GameObject).transform;
            //prov.eyeTransform = Camera.main.transform;
            //prov.leftHandTarget = (AVR.Core.Utils.Misc.GlobalFind("LeftHandController", typeof(GameObject)) as GameObject).transform;
            //prov.rightHandTarget = (AVR.Core.Utils.Misc.GlobalFind("RightHandController", typeof(GameObject)) as GameObject).transform;
        }
    }
}

