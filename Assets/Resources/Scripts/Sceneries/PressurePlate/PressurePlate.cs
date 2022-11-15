using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//#if UNITY_EDITOR
//[CustomEditor(typeof(CircuitInput), true)]
//public class PressurePlateInspector : Editor
//{
//    private PressurePlate _pressurePlate;
//    private SerializedProperty _outputs;

//    private void OnEnable()
//    {
//        _pressurePlate = target as PressurePlate;
//        // do this only once here
//        _outputs = serializedObject.FindProperty("Outputs");
//    }

//    public override void OnInspectorGUI()
//    {
//        EditorGUILayout.PropertyField(_outputs);

//        base.OnInspectorGUI();
//    }
//}
//#endif


public class PressurePlate : CircuitInput
{
    public Animator Animator;

    protected override void ActivateAction()
    {
        Animator.SetBool("Activate", true);
    }
    protected override void DeActivateAction()
    {
        Animator.SetBool("Activate", false);
    }


    private void Start()
    {
        Animator.SetBool("Activate", IsActivated);
    }
}
