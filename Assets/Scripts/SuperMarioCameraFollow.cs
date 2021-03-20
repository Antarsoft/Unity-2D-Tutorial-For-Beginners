#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SuperMarioCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(1, 10)]
    public float smoothFactor;
    [HideInInspector]
    public Vector3 minValues, maxValue;
    [Range(0,1)]
    public float moveCameraTriggerValue;


    //Editors Fields
    [HideInInspector]
    public bool setupComplete = false;
    public enum SetupState { None, Step1, Step2 }
    [HideInInspector]
    public SetupState ss = SetupState.None;

    void Start() { Follow(); }

    private void FixedUpdate()
    {
        var playerViewortPos = Camera.main.WorldToViewportPoint(target.transform.position);
        if (playerViewortPos.x>= moveCameraTriggerValue)
        {
            Follow();
        }        
    }

    void Follow()
    {
        Debug.Log("X");
        Vector3 targetPosition = target.position + offset;
        //Verify if the targetPosition is out of bound or not
        //Limit it to the min and max values
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValues.x, maxValue.x),
            Mathf.Clamp(targetPosition.y, minValues.y, maxValue.y),
            Mathf.Clamp(targetPosition.z, minValues.z, maxValue.z));

        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothFactor * Time.fixedDeltaTime);
        transform.position = smoothPosition;

        minValues.x = smoothPosition.x;
    }

    public void ResetValues()
    {
        setupComplete = false;
        minValues = Vector3.zero;
        maxValue = Vector3.zero;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SuperMarioCameraFollow))]
public class SuperMarioCameraFollowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Assign the MonoBehaviour target script
        var script = (SuperMarioCameraFollow)target;
        //Check if Values are setup or not

        //Blank Space
        GUILayout.Space(20);

        GUIStyle defaultStyle = new GUIStyle();
        defaultStyle.fontSize = 12;
        defaultStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 15;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("-=- Camera Boundaries Settings -=-", titleStyle);
        //If they are setup display the Min and Max values along with preview button
        //Also have a reset button for the values
        if (script.setupComplete)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Minimum Values:", defaultStyle);
            GUILayout.Label("Maximum Values:", defaultStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"X = {script.minValues.x}", defaultStyle);
            GUILayout.Label($"X = {script.maxValue.x}", defaultStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Y = {script.minValues.y}", defaultStyle);
            GUILayout.Label($"Y = {script.maxValue.y}", defaultStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("View Minumum"))
            {
                //Snap the camera view to the minimum values
                Camera.main.transform.position = script.minValues;
            }
            if (GUILayout.Button("View Maximum"))
            {
                //Snap the camera view to the maximum values
                Camera.main.transform.position = script.maxValue;
            }
            GUILayout.EndHorizontal();

            //Reset the view to the target
            if (GUILayout.Button("Focus On Target"))
            {
                Vector3 targetPos = script.target.position + script.offset;
                targetPos.z = script.minValues.z;
                Camera.main.transform.position = targetPos;
            }

            if (GUILayout.Button("Reset Camera Values"))
            {
                //Reset the setupcomplete boolean
                //reset the min max vec3 values
                script.ResetValues();
            }
        }
        //If they are not setup display a start setup button
        else
        {
            //Step 0 : Show the start wizard button
            if (script.ss == SuperMarioCameraFollow.SetupState.None)
            {
                if (GUILayout.Button("Start Setting Camera Values"))
                {
                    //Changes the state to step1
                    script.ss = SuperMarioCameraFollow.SetupState.Step1;
                }
            }
            //Step 1 : Setup the bottom left boundary (min values)        
            else if (script.ss == SuperMarioCameraFollow.SetupState.Step1)
            {
                //Instruction on what to do
                GUILayout.Label($"1- Select your main Camera", defaultStyle);
                GUILayout.Label($"2- Move it to the bottom left bound limit of your level", defaultStyle);
                GUILayout.Label($"3- Click the 'Set Minimum Values' Button", defaultStyle);
                //Button to set the min values
                if (GUILayout.Button("Set Minimum Values"))
                {
                    //Set the minimun values of the camera limit
                    script.minValues = Camera.main.transform.position;
                    //Change to step 2
                    script.ss = SuperMarioCameraFollow.SetupState.Step2;
                }
            }
            //Step 2 : Setup the top right boundary (max values)
            else if (script.ss == SuperMarioCameraFollow.SetupState.Step2)
            {
                //Instruction on what to do
                GUILayout.Label($"1- Select your main Camera", defaultStyle);
                GUILayout.Label($"2- Move it to the top right bound limit of your level", defaultStyle);
                GUILayout.Label($"3- Click the 'Set Maximum Values' Button", defaultStyle);
                //Button to set the max values
                if (GUILayout.Button("Set Maximum Values"))
                {
                    //Set the minimun values of the camera limit
                    script.maxValue = Camera.main.transform.position;
                    //Set the state to None
                    script.ss = SuperMarioCameraFollow.SetupState.None;
                    //Enable the SetupComplete boolean
                    script.setupComplete = true;
                    //Reset view to Player
                    Vector3 targetPos = script.target.position + script.offset;
                    targetPos.z = script.minValues.z;
                    Camera.main.transform.position = targetPos;
                }
            }
        }
    }
}
#endif