﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Collection of generally useful functions.
/// </summary>
namespace AVR.Core.Utils {
    /// <summary>
    /// Geometry-centric utility functions.
    /// </summary>
    public static class Geom
    {
        /// <summary>
        /// Creates a new, empty transform in the scene with a given parent, name and local coordinates
        /// </summary>
        /// <param name="parent"> Parent of the spawned transform. Pass null if there ought to be no parent. </param>
        /// <param name="name"> Name of the newly created transform </param>
        /// <param name="localCoords"> Local coordiantes of the newly spawned transform. </param>
        /// <returns></returns>
        public static Transform addEmptyTransform(Transform parent, string name="EmptyTransform", Vector3 localCoords=default(Vector3)) {
            Transform T = new GameObject(name).transform;
            T.SetParent(parent);
            T.localPosition=localCoords;
            return T;
        }

        /// <summary>
        /// Clamps the rotation to a quaternion to the given euler-angle bounds. Eg ClampQuaternionRotation(q, new Vector3(30, 0, 120)) will clamp q to a min/max rotation of +/- 30 deg around the x Axis etc.
        /// </summary>
        /// <param name="q">Quaternion to be clamped</param>
        /// <param name="bounds">Euler-angle bounds. (Will count as both positive and negative.</param>
        /// <returns></returns>
        public static Quaternion ClampQuaternionRotation(Quaternion q, Vector3 bounds)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
            angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

            float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
            angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

            return q;
        }

        /// <summary>
        /// Clamps the rotation to a quaternion to the given euler-angle bounds. Eg ClampQuaternionRotation(q, new Vector3(30, 0, 120)) will clamp q to a min/max rotation of +/- 30 deg around the x Axis etc.
        /// </summary>
        /// <param name="q">Quaternion to be clamped</param>
        /// <param name="minbounds">Minimum euler-angle bounds</param>
        /// <param name="maxbounds">Maximum euler-angle bounds</param>
        /// <returns></returns>
        public static Quaternion ClampQuaternionRotation(Quaternion q, Vector3 minbounds, Vector3 maxbounds)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, -minbounds.x, maxbounds.x);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
            angleY = Mathf.Clamp(angleY, -minbounds.y, maxbounds.y);
            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

            float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
            angleZ = Mathf.Clamp(angleZ, -minbounds.z, maxbounds.z);
            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

            return q;
        }
    }

    /// <summary>
    /// Physics and raycast-centric utility functions.
    /// </summary>
    public static class Phys
    {
        /// <summary>
        /// Perform linecasts along a spline of positions.
        /// </summary>
        /// <param name="positions"> List of positions. Linecasts will be performed for each pair of subsequent points </param>
        /// <param name="hit"> RaycastHit of the first hit encountered. </param>
        /// <returns> True if something is hit, otherwise false. </returns>
        public static bool PathCast(Vector3[] positions, out RaycastHit hit)
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                if (Physics.Linecast(positions[i], positions[i + 1], out hit))
                {
                    return true;
                }
            }
            hit = new RaycastHit();
            return false;
        }

        /// <summary>
        /// Perform linecasts along a spline of positions.
        /// </summary>
        /// <param name="positions"> List of positions. Linecasts will be performed for each pair of subsequent points </param>
        /// <param name="hit"> RaycastHit of the first hit encountered. </param>
        /// <param name="mask"> Mask passed to the Linecast function. </param>
        /// <returns> True if something is hit, otherwise false. </returns>
        public static bool PathCast(Vector3[] positions, out RaycastHit hit, LayerMask mask)
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                if (Physics.Linecast(positions[i], positions[i + 1], out hit, mask))
                {
                    return true;
                }
            }
            hit = new RaycastHit();
            return false;
        }

        /// <summary>
        /// Perform linecasts along a spline of positions given in local space.
        /// </summary>
        /// <param name="positions"> List of positions in LOCAL space. Linecasts will be performed for each pair of subsequent points </param>
        /// <param name="hit"> RaycastHit of the first hit encountered. </param>
        /// <param name="localToWorld"> Transformation matrix to convert given positions from local to world space. </param>
        /// <returns> True if something is hit, otherwise false. </returns>
        public static bool PathCast(Vector3[] positions, out RaycastHit hit, Matrix4x4 localToWorld)
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                if (Physics.Linecast(localToWorld * positions[i], localToWorld * positions[i + 1], out hit))
                {
                    return true;
                }
            }
            hit = new RaycastHit();
            return false;
        }

        public static bool LineCast(Vector3 start, Vector3 end, out RaycastHit hit)
        {
            return Physics.Linecast(start, end, out hit);
        }

        public static bool LineCast(Vector3 start, Vector3 end, out RaycastHit hit, LayerMask mask)
        {
            return Physics.Linecast(start, end, out hit, mask);
        }

        public static bool LineCast(Vector3 start, Vector3 end, out RaycastHit hit, Matrix4x4 localToWorld)
        {
            return Physics.Linecast(localToWorld * start, localToWorld * end, out hit);
        }
    }

    /// <summary>
    /// Misc. utility functions.
    /// </summary>
    public static class Misc {
        /// <summary>
        /// Returns location within the hierarchy of a scene object.
        /// </summary>
        /// <param name="self"> Transform component of object in question </param>
        /// <returns> Hierarchy location formated as a directory-string </returns>
        public static string GetHierarchyPath(Transform self)
        {
            string path = self.gameObject.name;
            Transform p = self.parent;
            while (p != null)
            {
                path = p.name + "/" + path;
                p = p.parent;
            }
            return path;
        }

        /// <summary>
        /// Finds an Object in the Scene by name and type including inactive ones
        /// </summary>
        /// <param name="name"> Name of object in question </param>
        /// <param name="type"> Type of object in question </param>
        /// <returns> First object found that has the given name and type. Null if none is found. </returns>
        public static Object GlobalFind(string name, System.Type type)
        {
            Object[] objs = Resources.FindObjectsOfTypeAll(type);

            foreach (Object obj in objs)
            {
                if (obj.name == name)
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Get custom System.Attribute from a given type
        /// </summary>
        /// <param name="type"> Type to get Attribute from </param>
        /// <typeparam name="TAttribute">Attribute type to retrive</typeparam>
        /// <returns>First attribute of given type attatched to given type.</returns>
        public static TAttribute GetAttribute<TAttribute>(this System.Type type) where TAttribute : System.Attribute
        {
            return type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
        }

        /// <summary>
        /// Creates a Texture2D object from a given image path
        /// </summary>
        /// <param name="path"> Path (relative to assembly) where the image is located. Example: "Packages/com.avr.core/Package_Resources/avr_logo_vr.png" </param>
        /// <returns></returns>
        public static Texture2D Image2Texture(string path) {
            var rawData = System.IO.File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(1, 1); // Size should not matter here, as LoadImage resets it.
            tex.LoadImage(rawData);
            return tex;
        }

        /// <summary>
        /// Creates an empty gameobject with specifed position and rotation. Returns its transform component.
        /// </summary>
        /// <param name="name">Name of the gameobject</param>
        /// <param name="parent">Parent of the gameobject (null for no parent)</param>
        /// <param name="worldPos">World-space position to instantiate the object at</param>
        /// <param name="worldRot">World-space rotation of the gameobject</param>
        public static Transform CreateEmptyGameObject(string name, Transform parent, Vector3 worldPos, Quaternion worldRot) {
            GameObject o = new GameObject(name);
            o.transform.SetParent(parent);
            o.transform.SetPositionAndRotation(worldPos, worldRot);
            return o.transform;
        }

        /// <summary>
        /// Creates an empty gameobject with a localposition and rotation of zero. Returns its transform component.
        /// </summary>
        /// <param name="name">Name of the gameobject</param>
        /// <param name="parent">Parent of the gameobject (null for no parent)</param>
        public static Transform CreateEmptyGameObject(string name = "EmptyTransform", Transform parent = null)
        {
            GameObject o = new GameObject(name);
            o.transform.SetParent(parent);
            o.transform.localPosition = Vector3.zero;
            o.transform.localRotation = Quaternion.identity;
            return o.transform;
        }
    }
}
