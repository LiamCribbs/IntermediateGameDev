using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class DialogueData
{
    public string id;

    [Space(10)]
    public string text;
    public Effect effect;
    public float effectDuration;

    [Space(10)]
    public UnityEvent<DialogueEmitter, DialogueData> completePredicate;

    [Space(10)]
    public UnityEvent<DialogueEmitter> onStart;
    public UnityEvent<DialogueEmitter> onFinishedWriting;

    [Space(10)]
    public DialoguePredicateEvent onCompleteEvent;
    public DialoguePredicateEvent onClearedEvent;

    public void MarkCompleted()
    {

    }

    public enum Effect
    {
        None, ShakeSoft, ShakeHard, FloatSoft, FloatHard
    }
}

public enum Operator
{
    NONE, AND, OR, XOR
}

[System.Serializable]
public class DialoguePredicateEvent
{
    public UnityEvent<DialogueEmitter> action;

    public Predicate[] predicates;

    [System.Serializable]
    public class Predicate
    {
        public UnityEvent predicate;
        public Operator Operator;
    }

    public void Invoke()
    {
        if (predicates.Length > 0)
        {
            //bool state = predicates[0].predicate.Invoke();
            for (int i = 0; i < predicates.Length; i++)
            {

            }
        }
    }
}

//#if UNITY_EDITOR
//[CustomPropertyDrawer(typeof(DialogueCompleteEvent.Predicate))]
//public class DialogueCompleteEventPredicateDrawer : PropertyDrawer
//{
//    // Draw the property inside the given rect
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // Using BeginProperty / EndProperty on the parent property means that
//        // prefab override logic works on the entire property.

//        EditorGUI.BeginProperty(position, label, property);

        
//        // Draw label
//        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//        // Don't make child fields be indented
//        var indent = EditorGUI.indentLevel;
//        EditorGUI.indentLevel = 0;
        
//        // Calculate rects
//        var amountRect = new Rect(position.x, position.y, position.width * 0.7f, position.height);
//        var unitRect = new Rect(position.x + position.width * 0.8f, position.y, position.width * 0.2f, position.height);

//        // Draw fields - passs GUIContent.none to each so they are drawn without labels
//        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("predicate"), GUIContent.none);
//        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("Operator"), GUIContent.none);

//        // Set indent back to what it was
//        EditorGUI.indentLevel = indent;

//        EditorGUI.EndProperty();
//    }
//}
//#endif