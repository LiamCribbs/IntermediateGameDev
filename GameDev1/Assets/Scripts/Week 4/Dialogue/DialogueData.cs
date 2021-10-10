using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;

[System.Serializable]
public class DialogueData
{
    [Tooltip("ID can be used to find this instance\n\nShould be unique")]
    public string id;

    [Space(10)]
    [TextArea(6, 20)] public string text;
    [Tooltip("Effect that plays when the text writes")]
    public Effect effect;
    [Tooltip("How long the effect lasts\n\nSet to 0 for infinite")]
    public float effectDuration;

    [Space(10)]
    [Tooltip("Should we check completion condition as soon as the text starts writing or only after it's finished writing")]
    public bool alwaysCheckForCompletion;
    [Tooltip("Predicates for the text to clear")]
    public Predicates completeCondition;

    [Space(10)]
    [Tooltip("Fired just before the text starts writing")]
    public UnityEvent<DialogueEmitter> onStart;
    [Tooltip("Fired when the text finishes writing")]
    public UnityEvent<DialogueEmitter> onFinishedWriting;

    [Space(10)]
    [Tooltip("Fires when the complete condition is met")]
    public DialoguePredicateEvent onCompleteEvent;
    [Space(10)]
    [Tooltip("Fires when the text has finished clearing")]
    public DialoguePredicateEvent onClearedEvent;

    public enum Effect
    {
        None, ShakeSoft, ShakeHard, FloatSoft, FloatHard
    }
}

public enum Operator
{
    AND, OR, XOR
}

[System.Serializable]
public class Predicates
{
    [System.Serializable]
    public class Predicate
    {
        public UltEvent<DialogueEmitter> predicate;
        public Operator Operator;
    }

    public Predicate[] predicates;

    public static bool Operate(bool a, bool b, Operator op)
    {
        return op switch
        {
            Operator.AND => a && b,
            Operator.OR => a || b,
            Operator.XOR => a ^ b
        };
    }

    public bool Invoke(DialogueEmitter emitter)
    {
        if (predicates != null && predicates.Length > 0)
        {
            var predicate = predicates[0].predicate;
            predicate.Invoke(emitter);
            bool state = (bool)predicate.returnedValues[0];

            for (int i = 1; i < predicates.Length; i++)
            {
                predicate = predicates[i].predicate;
                predicate.Invoke(emitter);
                state = Operate(state, (bool)predicate.returnedValues[i], predicates[i - 1].Operator);
            }

            if (!state)
            {
                return false;
            }
        }

        return true;
    }
}

[System.Serializable]
public class DialoguePredicateEvent
{
    public UnityEvent<DialogueEmitter> action;
    [Space(-20)]
    public Predicates predicates;

    public void Invoke(DialogueEmitter emitter)
    {
        if (predicates.Invoke(emitter))
        {
            action?.Invoke(emitter);
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