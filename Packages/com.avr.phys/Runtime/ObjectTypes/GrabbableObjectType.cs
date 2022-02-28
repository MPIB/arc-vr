using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Phys {

    /// <summary>
    /// Defines a set of sounds and modifiers to be played during physics events for GrabbableObjectType's
    /// </summary>
    [System.Serializable]
    public struct GrabbableObjectSoundData
    {
        public AudioClip pickupSound;
        public AudioClip releaseSound;
        public AudioClip collideSound;

        //Value from 0 - 1 that the AudioSource will use as a volume multiplier
        [Range(0, 1)] public float volumeMultiplier;
    }

    /// <summary>
    /// Defines how an object behaves whilst grabbed.
    /// </summary>
    [CreateAssetMenu(fileName = "Data", menuName = "arc-vr/phys/GrabbableObjectType", order = 1)]
    public class GrabbableObjectType : ScriptableObject
    {
        public enum FollowType { FREE, STATIC, CONSTRAINED, HEAVY };
        public FollowType followType;

        public bool handToObject = false;

        public bool allowTwoHanded = false;
        public bool changeObjectTypeOnTwoHanded;
        public GrabbableObjectType typeOnTwoHanded;

        [Range(0.0f, 1.0f)]
        public float Lightness = 1.0f;
        [Range(0.0f, 1.0f)]
        public float Angular_Lightness = 1.0f;

        [Range(0.05f, 2.0f)]
        public float Break_grab_distance = 0.5f;

        // This multiplier will essentially make the target objects rigidbody weight = weight * 1.0/multiplier
        public float Heavy_force_multiplier = 1.0f;

        public GrabbableObjectSoundData soundData;

        public static GrabbableObjectType defaultObjectType(){
            GrabbableObjectType o = new GrabbableObjectType();
            o.followType = FollowType.FREE;
            o.handToObject = false;
            o.allowTwoHanded = false;
            o.changeObjectTypeOnTwoHanded = false;
            o.Lightness = 1.0f;
            o.Angular_Lightness = 1.0f;
            o.Break_grab_distance = 0.5f;
            o.soundData.pickupSound = null;
            o.soundData.releaseSound = null;
            o.soundData.collideSound = null;
            o.soundData.volumeMultiplier = 1.0f;
            
            return o;
        }

        //When the user resets the scriptable object in the inspector, give them a default object.
        private void Reset()
        {
            followType = FollowType.FREE;
            handToObject = false;
            allowTwoHanded = false;
            changeObjectTypeOnTwoHanded = false;
            Lightness = 1.0f;
            Angular_Lightness = 1.0f;
            Break_grab_distance = 0.5f;
            soundData.pickupSound = null;
            soundData.releaseSound = null;
            soundData.collideSound = null;
            soundData.volumeMultiplier = 1.0f;
        }
    }
}