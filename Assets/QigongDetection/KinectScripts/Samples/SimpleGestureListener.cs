using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.rfilkov.kinect;


namespace com.rfilkov.components
{
    /// <summary>
    /// Simple gesture listener that only displays the status and progress of the given gestures.
    /// </summary>
    public class SimpleGestureListener : MonoBehaviour, GestureListenerInterface
    {
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("List of the gestures to detect.")]
        public List<GestureType> detectGestures = new List<GestureType>();

        [Tooltip("UI-Text to display the gesture-listener output.")]
        public UnityEngine.UI.Text gestureInfo;

        // private bool to track if progress message has been displayed
        private bool progressDisplayed;
        private float progressGestureTime;


        // invoked when a new user is detected
        public void UserDetected(ulong userId, int userIndex)
        {
            if (userIndex == playerIndex)
            {
                // as an example - detect these user specific gestures
                KinectGestureManager gestureManager = KinectManager.Instance.gestureManager;

                foreach(GestureType gesture in detectGestures)
                {
                    gestureManager.DetectGesture(userId, gesture);
                }
            }

            if (gestureInfo != null)
            {
                //gestureInfo.text = "Please do the gestures and look for the gesture detection state.";
            }
        }


        // invoked when the user is lost
        public void UserLost(ulong userId, int userIndex)
        {
            if (userIndex != playerIndex)
                return;

            if (gestureInfo != null)
            {
                gestureInfo.text = string.Empty;
            }
        }


        // invoked to report gesture progress. important for the continuous gestures, because they never complete.
        public void GestureInProgress(ulong userId, int userIndex, GestureType gesture,
                                      float progress, KinectInterop.JointType joint, Vector3 screenPos)
        {
            if (userIndex != playerIndex)
                return;
            switch (gesture)
            {
                //    case GestureType.Qg_PrePose:
                //        if (progress > 0.5f && gestureInfo != null)
                //        {
                //            string sGestureText = string.Format("正在进行预备式"+gesture);
                //            gestureInfo.text = sGestureText;

                //            progressDisplayed = true;
                //            progressGestureTime = Time.realtimeSinceStartup;
                //        }
                //        break;

                //    case GestureType.QgMove1_1:
                //        if (progress > 0.5f && gestureInfo != null)
                //        {
                //            string sGestureText = string.Format("正在进行两手托天理三焦" + gesture);
                //            gestureInfo.text = sGestureText;

                //            progressDisplayed = true;
                //            progressGestureTime = Time.realtimeSinceStartup;
                //        }
                //        break;

                //    case GestureType.QgMove2_1:
                //        if (progress > 0.5f && gestureInfo != null)
                //        {
                //            string sGestureText = string.Format("正在进行左右开弓似射雕搭腕" + gesture);
                //            gestureInfo.text = sGestureText;

                //            progressDisplayed = true;
                //            progressGestureTime = Time.realtimeSinceStartup;
                //        }
                //        break;

                //    case GestureType.QgMove2_2L:
                //        if (progress > 0.5f && gestureInfo != null)
                //        {
                //            string sGestureText = string.Format($"正在进行左右开弓似射雕-搭腕" + gesture);
                //            gestureInfo.text = sGestureText;

                //            progressDisplayed = true;
                //            progressGestureTime = Time.realtimeSinceStartup;
                //        }
                //        break;

                //switch (gesture)
                //    {
                //        case GestureType.Qg_PrePose:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% - 预备式", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove1_1:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% - 两手托天理三焦", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove2_1:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -左右开弓似射雕，搭腕", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove2_2L:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -左右开弓似射雕，左开弓", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove2_2R:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -左右开弓似射雕，右开弓", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove3_2L:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -调理脾胃需单举，举左手", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;

                //        case GestureType.QgMove3_2R:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -调理脾胃需单举，举右手", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove4_2:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -五劳七伤往后瞧", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;

                //        case GestureType.QgMove6_1:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -两手攀足固肾腰", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove7_1:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -攒拳怒目增气力，抱拳", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove7_2L:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -攒拳怒目增气力，出左拳", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove7_2R:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}% -攒拳怒目增气力，出右拳", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove8_1:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - {1:F0}%-背后七颠百病消，踮脚", gesture, screenPos.z * 100f);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;


                //        case GestureType.QgMove5_2:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - 左倾角度：{1:F0} 度-摇头摆尾去心火，左倾/左旋", gesture, screenPos.z);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove5_3:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - 右倾角度：{1:F0} 度-摇头摆尾去心火，右倾/右旋", gesture, screenPos.z);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                //        case GestureType.QgMove5_4:
                //            if (progress > 0.5f && gestureInfo != null)
                //            {
                //                string sGestureText = string.Format("{0} - 前倾角度：{1:F0} 度-摇头摆尾去心火，前倾", gesture, screenPos.z);
                //                gestureInfo.text = sGestureText;

                //                progressDisplayed = true;
                //                progressGestureTime = Time.realtimeSinceStartup;
                //            }
                //            break;
                case GestureType.Wheel:
                case GestureType.LeanRight:
                case GestureType.LeanLeft:
                case GestureType.LeanForward:
                case GestureType.LeanBack:
                    if (progress > 0.5f && gestureInfo != null)
                    {
                        string sGestureText = string.Format("{0} - {1:F0} degrees", gesture, screenPos.z);
                        gestureInfo.text = sGestureText;

                        progressDisplayed = true;
                        progressGestureTime = Time.realtimeSinceStartup;
                    }
                    break;

                case GestureType.Run:
                    if (progress > 0.5f && gestureInfo != null)
                    {
                        string sGestureText = string.Format("{0} - progress: {1:F0}%", gesture, progress * 100);
                        gestureInfo.text = sGestureText;

                        progressDisplayed = true;
                        progressGestureTime = Time.realtimeSinceStartup;
                    }
                    break;
            }
        }


        // invoked when a (discrete) gesture is complete.
        public bool GestureCompleted(ulong userId, int userIndex, GestureType gesture,
                                      KinectInterop.JointType joint, Vector3 screenPos)
        {
            if (userIndex != playerIndex)
                return false;

            if (progressDisplayed)
                return true;

            if ( gestureInfo != null)
            {
                string sGestureText = "You are doing \n" + gesture;
                //Debug.Log(sGestureText);
                gestureInfo.text = sGestureText;
            }


            return true;
        }


        // invoked when a gesture gets cancelled by the user
        public bool GestureCancelled(ulong userId, int userIndex, GestureType gesture,
                                      KinectInterop.JointType joint)
        {
            if (userIndex != playerIndex)
                return false;

            if (progressDisplayed)
            {
                progressDisplayed = false;

                if (gestureInfo != null)
                {
                    gestureInfo.text = String.Empty;
                }
            }

            return true;
        }


        public void Update()
        {
            // checks for timed out progress message
            if (progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
            {
                progressDisplayed = false;

                if (gestureInfo != null)
                {
                    gestureInfo.text = String.Empty;
                }

                Debug.Log("Forced progress to end.");
            }
        }

    }
}
