using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Phys;

[ExecuteInEditMode]
public class AVR_GrabNode_PoseHelper : MonoBehaviour
{
    AVR_DebugHand hand;
    AVR_GrabNode node;
    public Transform grab_point;

    // Start is called before the first frame update
    void Start()
    {
        hand = GetComponent<AVR_DebugHand>();
        node = GetComponentInParent<AVR_GrabNode>();

        transform.position = transform.parent.position - (grab_point.position - transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        hand = GetComponent<AVR_DebugHand>();
        node = GetComponentInParent<AVR_GrabNode>();

        //transform.rotation = node.transform.trans
        transform.localRotation = Quaternion.identity * Quaternion.Euler(node.yaw_test * node.allowed_yaw, node.pitch_test * node.allowed_pitch, node.roll_test * node.allowed_roll);
        //transform.RotateAround(transform.position, transform.up, node.pitch_test*node.allowed_pitch);

        transform.position = node.transform.position - (grab_point.position - transform.position);

        hand.index_f = node.index_pose;
        hand.middle_f = node.middle_pose;
        hand.ring_f = node.ring_pose;
        hand.pinky_f = node.pinky_pose;
        hand.thumb_f = node.thumb_pose;
        //hand.fingers[0].setStateImmediate(node.index_pose);
        //hand.fingers[1].setStateImmediate(node.middle_pose);
        //hand.fingers[2].setStateImmediate(node.ring_pose);
        //hand.fingers[3].setStateImmediate(node.pinky_pose);
        //hand.fingers[4].setStateImmediate(node.thumb_pose);
    }
}
