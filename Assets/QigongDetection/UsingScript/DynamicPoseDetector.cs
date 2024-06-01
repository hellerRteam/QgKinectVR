using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.rfilkov.kinect;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEditor.PackageManager;

namespace com.rfilkov.components
{
    /// <summary>
    /// Dynamic pose detector checks whether the user's pose matches predefined animated model's pose.
    /// </summary>
    public class DynamicPoseDetector : MonoBehaviour
    {
        [SerializeField]Data data = new Data();
        //[SerializeField]Data data=new Data();
        Animator animator;
        int NextActionIdx = -1;
        int CurActionIdx = -1;
        int hasStarted;
        int AccumulateCntTmp = 0;
        float AccumulateAccuracyTmp = 0;
        public GameObject RetrainDialog;
        public float AngleTorlerance = 30;
        int RetrainAction = 0;
        int RetrainActionTotal = 0;
        int MaxNumRetry = 2;
        int SceneIdx = 0; // 0: QgSceneTrain; 1: QgSceneRetrain

        List<int> realTimeLargeErrCnt;
        List<float> realTimeLargeErrAccumulator;
        public TextMeshProUGUI realTimetext;

        public float realTimeInstructionBegin=1.5f;
        public float realTimeInstructionEnd = 1.2f;

        protected static readonly Dictionary<KinectInterop.JointType, string> jointMap2Chinese = new Dictionary<KinectInterop.JointType, string>
        {
            {KinectInterop.JointType.Pelvis, "骨盆"},
            {KinectInterop.JointType.SpineNaval, "腰"},
            {KinectInterop.JointType.SpineChest, "胸"},
            {KinectInterop.JointType.Neck, "颈"},
            {KinectInterop.JointType.Head, "头"},

            {KinectInterop.JointType.ClavicleLeft, "左侧锁骨"},
            {KinectInterop.JointType.ShoulderLeft, "左肩"},
            {KinectInterop.JointType.ElbowLeft, "左肘"},
            {KinectInterop.JointType.WristLeft, "左手腕"},

            {KinectInterop.JointType.ClavicleRight, "右侧锁骨"},
            {KinectInterop.JointType.ShoulderRight, "右肩"},
            {KinectInterop.JointType.ElbowRight, "右肘"},
            {KinectInterop.JointType.WristRight, "右手腕"},

            {KinectInterop.JointType.HipLeft, "左髋"},
            {KinectInterop.JointType.KneeLeft, "左膝"},
            {KinectInterop.JointType.AnkleLeft, "左脚踝"},
            {KinectInterop.JointType.FootLeft, "左脚"},

            {KinectInterop.JointType.HipRight, "右髋"},
            {KinectInterop.JointType.KneeRight, "右膝"},
            {KinectInterop.JointType.AnkleRight, "右脚踝"},
            {KinectInterop.JointType.FootRight, "右脚"},
        };
        public List<GameObject> PoseName;
        [Tooltip("User avatar model, who needs to reach the target pose.")]
        public PoseModelHelper avatarModel;

        [Tooltip("Model in pose that need to be reached by the user.")]
        public PoseModelHelper poseModel;

        [Tooltip("List of joints to compare.")]
        public List<KinectInterop.JointType> poseJoints = new List<KinectInterop.JointType>();

        [Tooltip("Allowed delay in pose match, in seconds. 0 means no delay allowed.")]
        [Range(0f, 10f)]
        public float delayAllowed = 2f;

        [Tooltip("Time between pose-match checks, in seconds. 0 means check each frame.")]
        [Range(0f, 1f)]
        public float timeBetweenChecks = 0.1f;

        [Tooltip("Threshold, above which we consider the pose is matched.")]
        [Range(0.5f, 1f)]
        public float matchThreshold = 0.7f;

        [Tooltip("GUI-Text to display information messages.")]
        public UnityEngine.UI.Text infoText;
        public TextMeshProUGUI textMeshPro;

        // whether the pose is matched or not
        private bool bPoseMatched = false;
        // match percent (between 0 and 1)
        private float fMatchPercent = 0f;
        // pose-time with best matching
        private float fMatchPoseTime = 0f;

        private void Start()
        {
            //DontDestroyOnLoad(this);
            animator = avatarModel.GetComponent<Animator>();
            NextActionIdx = -1;
            hasStarted = 0;
            RetrainAction = 0;
            RetrainActionTotal = 0;
            animator.SetInteger("MotionStartCnt", NextActionIdx);
            animator.SetInteger("Retrain", RetrainAction);
            AccumulateCntTmp = 0;
            AccumulateAccuracyTmp = 0;
            SceneIdx = SceneManager.GetActiveScene().name == "QgSceneTrain" ? 0 : 1;
            realTimeLargeErrCnt= new List<int>();
            realTimeLargeErrAccumulator= new List<float>();
            for (int idx = 0; idx < poseJoints.Capacity; idx++) {
                realTimeLargeErrCnt.Add(0);
                realTimeLargeErrAccumulator.Add(0f);
            }
            GameObject gameobject = GameObject.FindWithTag("Data");
            data = gameobject.GetComponent<Data>();
            /*try
            {
                data = GameObject.Find("Data").GetComponent<Data>();
            }
            catch (Exception ex) {
                Debug.Log(ex.Message);
            }*/


        }
        // data for each saved pose
        public class PoseModelData
        {
            public float fTime;
            public float[] avBoneDirs;
        }

        // list of saved pose data
        private List<PoseModelData> alSavedPoses = new List<PoseModelData>();

        // current avatar pose
        private PoseModelData poseAvatar = new PoseModelData();

        // last time the model pose was saved 
        private float lastPoseSavedTime = 0f;


        /// <summary>
        /// Determines whether the target pose is matched or not.
        /// </summary>
        /// <returns><c>true</c> if the target pose is matched; otherwise, <c>false</c>.</returns>
        public bool IsPoseMatched()
        {
            return bPoseMatched;
        }


        /// <summary>
        /// Gets the pose match percent.
        /// </summary>
        /// <returns>The match percent (value between 0 and 1).</returns>
        public float GetMatchPercent()
        {
            return fMatchPercent;
        }


        /// <summary>
        /// Gets the time of the best matching pose.
        /// </summary>
        /// <returns>Time of the best matching pose.</returns>
        public float GetMatchPoseTime()
        {
            return fMatchPoseTime;
        }


        /// <summary>
        /// Gets the last check time.
        /// </summary>
        /// <returns>The last check time.</returns>
        public float GetPoseCheckTime()
        {
            return lastPoseSavedTime;
        }


        void Update()
        {
            animator = avatarModel.GetComponent<Animator>();
            AnimatorStateInfo stateinfo = animator.GetCurrentAnimatorStateInfo(0);

            /* if (stateinfo.IsName("Base Layer.Idle"))
                {
                    Debug.Log("Idle");
                    textMeshPro.text = "Idle";

                    if (NextActionIdx == 10)
                    {
                        textMeshPro.text = "Finished";
                        float averageAccuracy = data.getAverageAccuracy();
                        if (SceneIdx == 0)
                        {
                            data.TargetAccuracy = averageAccuracy;
                        }
                        textMeshPro.text += (averageAccuracy);

                        Invoke("gotoNextScene", 3);
                    }
                    else if (NextActionIdx == -1 && hasStarted == 0)
                    {
                        hasStarted = 1;
                        Invoke("GoToNextSection", 2);
                    }
                    else if (AccumulateCntTmp > 0)
                    {
                        if (NextActionIdx >= 0)
                        {
                            float CurrentAccuracy = AccumulateAccuracyTmp / AccumulateCntTmp;
                            Debug.Log("Motion" + (NextActionIdx) + " accuracy:" + CurrentAccuracy);
                            textMeshPro.text = "Motion" + (NextActionIdx) + " accuracy:" + CurrentAccuracy;
                            if (NextActionIdx - 1 >= 0 && NextActionIdx - 1 < data.getTotalAction())
                            {
                                data.AccumulateAccuracy(NextActionIdx - 1, CurrentAccuracy);
                            }

                            // retrain
                            if (SceneIdx == 1 && NextActionIdx > 0 && CurrentAccuracy < data.TargetAccuracy /*|| NextActionIdx == 3)
                            {
                                NextActionIdx--;
                                Debug.Log("准确度过低，重新训练动作" + (NextActionIdx));
                                textMeshPro.text = "准确度过低，重新训练动作" + (NextActionIdx);
                                RetrainDialog.SetActive(true);
                                RetrainAction = 1;
                                animator.SetInteger("Retrain", RetrainAction);

                            }
                            else if (SceneIdx == 1 && CurrentAccuracy >= data.TargetAccuracy)
                            {
                                RetrainDialog.SetActive(false);
                                RetrainAction = 0;
                                animator.SetInteger("Retrain", RetrainAction);
                            }

                            GoToNextSection();


                        }

                    }
                }*/

            // 判断是否正在播放动画.
            for (int idx = 0; idx < 10; idx++)
            {
                if (stateinfo.IsName("Base Layer.QgAnim " + idx))
                {
                    Debug.Log("QgAnim " + idx);
                    textMeshPro.text = "QgAnim " + idx;
                    CurActionIdx = idx;
                    if (CurActionIdx - 1 >= 0 && CurActionIdx - 1 < data.getTotalAction())
                    {
                        data.AccumulateTrainingTime(CurActionIdx - 1, Time.deltaTime);
                        Debug.Log("Null");
                        AccumulateCntTmp++;
                        AccumulateAccuracyTmp += CalculatePoseDifference();
                        Debug.Log("ActionIdx" + NextActionIdx + "\tAccumulateCnt:" + AccumulateCntTmp + "\tAccumulateAccuracy:" + AccumulateAccuracyTmp);
                        break;
                    }
                }
            }
            if (NextActionIdx == 10)
            {
                textMeshPro.text = "Finished";
                float averageAccuracy = data.getAverageAccuracy();
                if (SceneIdx == 0)
                {
                    data.TargetAccuracy = averageAccuracy;
                }
                Debug.Log("averageAccuracy:" + averageAccuracy);
                textMeshPro.text += (averageAccuracy);

                Invoke("gotoNextScene", 3);
            }
            else if (NextActionIdx == -1 && hasStarted == 0)
            {
                hasStarted = 1;
                Invoke("GoToNextSection", 2);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5 && RetrainAction > 0) { // 避免循环进入动画，卡在开头
                RetrainAction = 0;
                animator.SetInteger("Retrain", 0);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && NextActionIdx == CurActionIdx) // 判断动画播放结束normalizedTime的值为0~1，0为开始，1为结束。
            {
                float CurrentAccuracy = AccumulateAccuracyTmp / AccumulateCntTmp;
                Debug.Log("Motion" + (NextActionIdx) + " accuracy:" + CurrentAccuracy);
                textMeshPro.text = "Motion" + (NextActionIdx) + " accuracy:" + CurrentAccuracy;
                if (NextActionIdx - 1 >= 0 && NextActionIdx - 1 < data.getTotalAction())
                {
                    data.AccumulateAccuracy(NextActionIdx - 1, CurrentAccuracy);
                }

                 //retrain
                if (SceneIdx == 1 && NextActionIdx > 0 && NextActionIdx<9 && RetrainActionTotal < MaxNumRetry+1 && CurrentAccuracy < data.TargetAccuracy /*|| NextActionIdx == 3*/)
                {
                    Debug.Log("准确度过低，重新训练动作" + (NextActionIdx));
                    textMeshPro.text = "准确度过低，重新训练动作" + (NextActionIdx);
                    RetrainDialog.SetActive(true);
                    if(RetrainAction == 0 && RetrainActionTotal < MaxNumRetry + 1)
                    {
                        RetrainAction++;
                        RetrainActionTotal++;
                    }
                    animator.SetInteger("Retrain", RetrainAction);
                    NextActionIdx--;
                }else if (SceneIdx == 1 && RetrainActionTotal >= MaxNumRetry + 1)
                {
                    Debug.Log("RetrainActionTotal:"+ RetrainActionTotal);
                    RetrainAction = 0;
                    RetrainActionTotal = 0;
                    animator.SetInteger("Retrain", 0);
                    RetrainDialog.SetActive(false);
                }

                GoToNextSection();
            }


            // InvokeRepeating("play",3,3);



        }
        void gotoNextScene() {
            if (SceneIdx==0)
            {
                // DontDestroyOnLoad(this);
                SceneManager.LoadScene("QgSceneRetrain");
            }
            else if (SceneIdx==1)
            {
                // DontDestroyOnLoad(this);
                SceneManager.LoadScene("Finish");
            }
        }
        float CalculatePoseDifference() {
            KinectManager kinectManager = KinectManager.Instance;

            AvatarController avatarCtrl = avatarModel ? avatarModel.gameObject.GetComponent<AvatarController>() : null;

            // get mirrored state
            bool isMirrored = avatarCtrl ? avatarCtrl.mirroredMovement : true;  // true by default

            // current time
            float fCurrentTime = Time.realtimeSinceStartup;

            // save model pose, if needed
            if ((fCurrentTime - lastPoseSavedTime) >= timeBetweenChecks)
            {
                lastPoseSavedTime = fCurrentTime;

                // remove old poses and save current one
                RemoveOldSavedPoses(fCurrentTime);
                AddCurrentPoseToSaved(fCurrentTime, isMirrored, avatarModel, poseModel);
            }
            print(avatarModel);
            //print(kinectManager.IsUserTracked(avatarCtrl.playerId));

            //if (true || kinectManager != null && kinectManager.IsInitialized() &&
            //   avatarModel != null && avatarCtrl && kinectManager.IsUserTracked(avatarCtrl.playerId))
            if (true || kinectManager != null && kinectManager.IsInitialized() &&
               avatarModel != null && avatarCtrl)
            {
                Debug.Log("ok");
                // get current avatar pose
                GetKinectPose(fCurrentTime, isMirrored, poseModel);

                // get the difference
                bool bDebugPose = infoText != null;
                string sDebugPose = GetPoseDifference(isMirrored, bDebugPose);
                Debug.Log("index:"+SceneIdx);
                // realtime instruction
                if (SceneIdx == 1 && realTimeLargeErrCnt[0] > 100) {
                    string current_instruction = "";
                    for (int i = 0; i < poseJoints.Capacity; i++) {
                        //Debug.LogFormat("{0} {1}", poseJoints[i], realTimeLargeErrAccumulator[i] / realTimeLargeErrCnt[i]);
                        if (realTimeLargeErrAccumulator[i] / realTimeLargeErrCnt[i] > AngleTorlerance * realTimeInstructionBegin)
                        {
                            // Debug.Log("请注意" + jointMap2Chinese[poseJoints[i]]);
                            current_instruction += "请注意" + jointMap2Chinese[poseJoints[i]]+"动作\n";
                        }
                        else if (realTimeLargeErrAccumulator[i] / realTimeLargeErrCnt[i] < AngleTorlerance * realTimeInstructionEnd) { 
                            // todo
                        }
                    }
                    if (realTimetext.text != current_instruction)
                    {
                        realTimetext.text = current_instruction;
                    }
                }
                
                
                if (infoText != null)
                {
                    string sPoseMessage = string.Format("Motion Matching Degree: {0:F0}%", fMatchPercent * 100f,
                                                        (bPoseMatched ? "- Matched" : ""));
                    if (bDebugPose)
                    {
                        sPoseMessage += "\n\n" + sDebugPose;
                    }

                    infoText.text = sPoseMessage;
                }
            }
            else
            {
                // no user found
                fMatchPercent = 0f;
                fMatchPoseTime = 0f;
                bPoseMatched = false;

                if (infoText != null)
                {
                    infoText.text = "跟着教练的动作";
                }
            }
            return fMatchPercent;
        }
        void GoToNextSection()
        {
            if (SceneIdx == 0)
            {
                GameObject reminder = GameObject.Find("Canvas_Train/正式开始提醒");
                if (reminder) {
                    reminder.SetActive(false);
                }
            }
            for (int i = 0; i < PoseName.Capacity; i++)
            {
                PoseName[i].SetActive(false);
            }

            // proceed to next movement
            ResetCurrentAccuracy();
            if (NextActionIdx >= 0 && NextActionIdx < PoseName.Capacity)
            {
                // GameObject currentSectionPoster = GameObject.Find(SectionPosterName[NextActionIdx]);
                PoseName[NextActionIdx].SetActive(true);
            }
            NextActionIdx++;
            animator.SetInteger("MotionStartCnt", NextActionIdx);

        }
        void ResetCurrentAccuracy()
        {
            AccumulateCntTmp = 0;
            AccumulateAccuracyTmp = 0;
            for (int i = 0; i < poseJoints.Capacity; i++)
            {
                realTimeLargeErrCnt[i] = 0;
                realTimeLargeErrAccumulator[i] = 0;
            }
        }

        // removes saved poses older than delayAllowed from the list
        private void RemoveOldSavedPoses(float fCurrentTime)
        {
            for (int i = alSavedPoses.Count - 1; i >= 0; i--)
            {
                if ((fCurrentTime - alSavedPoses[i].fTime) >= delayAllowed)
                {
                    alSavedPoses.RemoveAt(i);
                }
            }
        }


        // adds current pose of poseModel to the saved poses list
        private void AddCurrentPoseToSaved(float fCurrentTime, bool isMirrored, PoseModelHelper avatar, PoseModelHelper kinectModel)
        {
            KinectManager kinectManager = KinectManager.Instance;
            if (kinectManager == null || avatar == null || poseJoints == null)
                return;

            PoseModelData pose = new PoseModelData();
            pose.fTime = fCurrentTime;
            pose.avBoneDirs = new float[poseJoints.Count];

            // save model rotation
            Quaternion poseSavedRotation = avatar.GetBoneTransform(0).rotation;
            avatar.GetBoneTransform(0).rotation = kinectModel.GetBoneTransform(0).rotation;

            int numJoints = kinectManager.GetJointCount();
            for (int i = 0; i < poseJoints.Count; i++)
            {
                KinectInterop.JointType joint = poseJoints[i];
                KinectInterop.JointType nextJoint = kinectManager.GetNextJoint(joint);
                KinectInterop.JointType nextnextJoint = kinectManager.GetNextJoint(nextJoint);
                if (nextJoint != joint && nextJoint >= 0 && (int)nextJoint < numJoints)
                {
                    Transform poseTransform1 = avatar.GetBoneTransform(avatar.GetBoneIndexByJoint(joint, isMirrored));
                    Transform poseTransform2 = avatar.GetBoneTransform(avatar.GetBoneIndexByJoint(nextJoint, isMirrored));
                    Transform poseTransform3 = avatar.GetBoneTransform(avatar.GetBoneIndexByJoint(nextnextJoint, isMirrored));
                    if (poseTransform1 != null && poseTransform2 != null && poseTransform3 != null)
                    {
                        pose.avBoneDirs[i] = Angle(poseTransform2.position, poseTransform1.position, poseTransform3.position);
                    }
                }
            }

            // add pose to the list
            alSavedPoses.Add(pose);

            // restore model rotation
            avatar.GetBoneTransform(0).rotation = poseSavedRotation;
        }


        // gets the current avatar pose
        private void GetKinectPose(float fCurrentTime, bool isMirrored, PoseModelHelper kinectModel)
        {
            KinectManager kinectManager = KinectManager.Instance;
            if (kinectManager == null || kinectModel == null || poseJoints == null)
                return;

            poseAvatar.fTime = fCurrentTime;
            if (poseAvatar.avBoneDirs == null)
            {
                poseAvatar.avBoneDirs = new float[poseJoints.Count];
            }

            int numJoints = kinectManager.GetJointCount();
            for (int i = 0; i < poseJoints.Count; i++)
            {
                KinectInterop.JointType joint = poseJoints[i];
                KinectInterop.JointType nextJoint = kinectManager.GetNextJoint(joint);
                KinectInterop.JointType nextnextJoint = kinectManager.GetNextJoint(nextJoint);
                if (nextJoint != joint && nextJoint >= 0 && (int)nextJoint < numJoints)
                {
                    Transform avatarTransform1 = kinectModel.GetBoneTransform(kinectModel.GetBoneIndexByJoint(joint, isMirrored));
                    Transform avatarTransform2 = kinectModel.GetBoneTransform(kinectModel.GetBoneIndexByJoint(nextJoint, isMirrored));
                    Transform avatarTransform3 = kinectModel.GetBoneTransform(kinectModel.GetBoneIndexByJoint(nextnextJoint, isMirrored));
                    //if (joint == KinectInterop.JointType.ElbowLeft)
                    //{
                    //    Debug.Log(avatarTransform1.position+"\t"+ avatarTransform1.localPosition);
                    //}
                    if (avatarTransform1 != null && avatarTransform2 != null && avatarTransform3 != null)
                    {
                        poseAvatar.avBoneDirs[i] = Angle(avatarTransform2.position, avatarTransform1.position, avatarTransform3.position);
                    }
                }
            }
        }


        // gets the difference between the avatar pose and the list of saved poses
        private string GetPoseDifference(bool isMirrored, bool bDebugPose)
        {
            // by-default values
            bPoseMatched = false;
            fMatchPercent = 0f;
            fMatchPoseTime = 0f;

            KinectManager kinectManager = KinectManager.Instance;
            if (poseJoints == null || poseAvatar.avBoneDirs == null)
                return string.Empty;

            StringBuilder sbDebugPose = bDebugPose ? new StringBuilder() : null;
            string sDebugPose = string.Empty;

            // check the difference with saved poses, starting from the last one
            for (int p = alSavedPoses.Count - 1; p >= 0; p--)
            {
                float fAngleDiff = 0f;

                PoseModelData poseModel = alSavedPoses[p];
                StringBuilder angList = new StringBuilder();
                for (int i = 0; i < poseJoints.Count; i++)
                {
                    float vPoseBone = poseModel.avBoneDirs[i];
                    float vAvatarBone = poseAvatar.avBoneDirs[i];

                    float fDiff = Mathf.Abs(vPoseBone - vAvatarBone);// Vector3.Angle(vPoseBone, vAvatarBone);
                    //if (fDiff > 90f)
                    //    fDiff = 90f;

                    fAngleDiff += fDiff;
                    realTimeLargeErrAccumulator[i] += fDiff;
                    realTimeLargeErrCnt[i] += 1;
                    //if(bDebugPose)
                    //{
                    //    sbDebugPose.AppendFormat("{0} - diff: {1:F0}", poseJoints[i], fDiff).AppendLine();
                    //}
                    angList.AppendFormat(fDiff + "  ");
                }
                // Debug.Log(angList);
                float fPoseMatch = Mathf.Min(1,Mathf.Max(1f - fAngleDiff / AngleTorlerance/poseJoints.Count,0));
                // Debug.Log(fPoseMatch);
                if (fPoseMatch > fMatchPercent)
                {
                    fMatchPercent = fPoseMatch;
                    fMatchPoseTime = poseModel.fTime;
                    bPoseMatched = (fMatchPercent >= matchThreshold);

                    if(bDebugPose)
                    {
                        sDebugPose = sbDebugPose.ToString();
                    }
                }

                if (bDebugPose)
                {
                    sbDebugPose.Clear();
                }
            }

            return sDebugPose;
        }
        public float Angle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            Vector3 a = new Vector3(
            point1.x - point2.x,
            point1.y - point2.y,
            point1.z - point2.z);
            Vector3 b = new Vector3(
            point1.x - point3.x,
            point1.y - point3.y,
            point1.z - point3.z);
            float dot = a.x * b.x + a.y * b.y + a.z * b.z;
            float lengthA = Mathf.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
            float lengthB = Mathf.Sqrt(b.x * b.x + b.y * b.y + b.z * b.z);
            if (lengthA == 0.0f || lengthB == 0.0f)
            {
                return 0.0f;
            }
            float theta = Mathf.Acos(dot / lengthA * lengthB);
            theta *= 180.0f / Mathf.PI; // Radians to degrees
            return theta;
        }
    }
}

