using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
/*[CustomEditor(typeof(MInput))]
public class MInputInspector : Editor
{
    

    private SerializedProperty _keys;
    private MInput _mInput;

    private void OnEnable()
    {
        _mInput = target as MInput;
        // do this only once here
        _keys = serializedObject.FindProperty("Keys");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();

        // Ofcourse you also want to change the list size here
        _keys.arraySize = EditorGUILayout.IntField("No of Keys", _keys.arraySize);

        for (int i = 0; i < _keys.arraySize; i++)
        {
            var dialogue = _keys.GetArrayElementAtIndex(i);
            
            EditorGUILayout.PropertyField(dialogue, new GUIContent("" + i), true);
        }

        // Note: You also forgot to add this
        serializedObject.ApplyModifiedProperties();
    }
}*/
#endif


public enum MKeyName
{
    Jump,
    Top,
    Left,
    Down,
    Right,
}

public class MInput : MonoBehaviour
{
    [System.Serializable]
    public class KeyInfo
    {
        public string Name;
        public KeyCode KeyCode;
        [ReadOnly] public int KeyTime;

        public static implicit operator int(KeyInfo key) => key.KeyTime;

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(KeyInfo))]
        public class Drawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUIUtility.singleLineHeight + MarginY/** 2*/;// (EditorGUIUtility.wideMode ? 1 : 2);
            }

            private const float MarginY = 6;
            private const float MarginX = 9;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // Find the SerializedProperties by name
                var name = property.FindPropertyRelative(nameof(Name));
                var keyCode = property.FindPropertyRelative(nameof(KeyCode));
                var keyTime = property.FindPropertyRelative(nameof(KeyTime));

                //EditorGUILayout.PropertyField(name);
                //EditorGUILayout.PropertyField(keyCode);
                //EditorGUILayout.PropertyField(keyTime);

                float labelWidth = EditorGUIUtility.labelWidth;
                /*float lineHeight = EditorGUIUtility.singleLineHeight;
                var labelRect = new Rect(position.x, position.y, labelWidth, position.height / 2);*/

                float demarginedWidth = position.width - 2 * MarginX;

                var rect1 = new Rect(
                    position.x, 
                    position.y + MarginY / 2 /*+ lineHeight*/, 
                    demarginedWidth * 0.3f, 
                    position.height - MarginY /*/ 2*/);
                
                var rect2 = new Rect(
                    rect1.x + MarginX + rect1.width, 
                    rect1.y, 
                    demarginedWidth * 0.43f, 
                    rect1.height);

                var rect3 = new Rect(
                    rect2.x + MarginX + rect2.width, 
                    rect2.y, 
                    demarginedWidth * 0.27f, 
                    rect2.height);

                /*EditorGUIUtility.labelWidth = 12.0f;
                EditorGUI.LabelField(labelRect, label);*/

                EditorGUIUtility.labelWidth = 36;
                EditorGUI.PropertyField(rect1, name);
                EditorGUIUtility.labelWidth = 60;
                EditorGUI.PropertyField(rect2, keyCode);
                EditorGUIUtility.labelWidth = 60;
                EditorGUI.PropertyField(rect3, keyTime);
                EditorGUIUtility.labelWidth = labelWidth;

                //const int SingleCharWidth = 8;
                //static void SetLableWidth(string nameOfProp)
                //{
                //    EditorGUIUtility.labelWidth = nameOfProp.Length * SingleCharWidth;
                //}
            }
        }
#endif
    }

    public KeyInfo this[string keyName] => Keys.Find(key => key.Name == keyName);
    public KeyInfo this[MKeyName keyName] => this[keyName.ToString()];


    [SerializeField] private List<KeyInfo> Keys = new();


    [SerializeField] private sbyte WalkKeyStatus;

    public void Start()
    {

    }
    public void Update()
    {
        
    }
    public void FixedUpdate()
    {
        for(int i = 0; i < Keys.Count; i++)
        {
            if (Input.GetKey(Keys[i].KeyCode))
            {
                if (Keys[i].KeyTime <= 0) Keys[i].KeyTime = 0;
                Keys[i].KeyTime++;
            }
            else
            {
                if (Keys[i].KeyTime >= 0) Keys[i].KeyTime = 0;
                Keys[i].KeyTime--;
            }
        }
    }

    public bool KeyPressed(string keyName) => this[keyName].KeyTime >= 1;
    public bool KeyReleased(string keyName) => this[keyName].KeyTime <= -1;
    public bool KeyPressedThisFrame(string keyName) => this[keyName].KeyTime == 1;
    public bool KeyReleasedThisFrame(string keyName) => this[keyName].KeyTime == -1;

    
    public bool KeyPressed(MKeyName key) => this[key.ToString()].KeyTime >= 1;
    public bool KeyReleased(MKeyName key) => this[key.ToString()].KeyTime <= -1;
    public bool KeyPressedThisFrame(MKeyName key) => this[key.ToString()].KeyTime == 1;
    public bool KeyReleasedThisFrame(MKeyName key) => this[key.ToString()].KeyTime == -1;
    
}

