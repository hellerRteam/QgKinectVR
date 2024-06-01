using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.rfilkov.kinect;

namespace com.rfilkov.components
{
    public class QigongGestureListener : MonoBehaviour
    {
        // Start is called before the first frame update
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("List of the gestures to detect.")]
        public List<GestureType> detectGestures = new List<GestureType>();

        [Tooltip("UI-Text to display the gesture-listener output.")]
        public UnityEngine.UI.Text gestureInfo;

        private bool PrePose = false;
        // private bool to track if progress message has been displayed
        private bool progressDisplayed;
        private float progressGestureTime;
        private static QigongGestureListener instance = null;

        public static QigongGestureListener Instance
        {
            get
            {
                return instance;
            }
        }

        public void UserDetected(ulong userId, int userIndex)
        {
            // the gestures are allowed for the selected user only
            KinectGestureManager gestureManager = KinectManager.Instance.gestureManager;
            if (!gestureManager || (userIndex != playerIndex))
                return;

            // set the gestures to detect
            foreach (GestureType gesture in detectGestures)
            {
                gestureManager.DetectGesture(userId, gesture);
            }

            if (gestureInfo != null)
            {
                gestureInfo.text = "跟着老师的动作开始八段锦训练吧。";
            }
        }
        public void UserLost(ulong userId, int userIndex)
        {
            // the gestures are allowed for the primary user only
            if (userIndex != playerIndex)
                return;

            if (gestureInfo != null)
            {
                gestureInfo.text = string.Empty;
            }
        }

        public bool GestureCompleted(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint, Vector3 screenPos)
        {
            // the gestures are allowed for the primary user only
            if (userIndex != playerIndex)
                return false;

            if (gesture == GestureType.Qg_PrePose && gestureInfo != null)
            {
                PrePose = true;            
            }

            return true;
        }

        void Start()
        {

        }
        void Awake()
        {
            instance = this;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }

}
