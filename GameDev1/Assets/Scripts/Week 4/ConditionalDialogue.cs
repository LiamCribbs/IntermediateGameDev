using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConditionalDialogue : MonoBehaviour
{
    [System.Serializable]
    public class Conditional
    {
        public Predicates predicate;

        public UnityEvent<DialogueEmitter> ifTrue;
        public UnityEvent<DialogueEmitter> ifFalse;

        public void Invoke(DialogueEmitter emitter)
        {
            (predicate.Invoke(emitter) ? ifTrue : ifFalse).Invoke(emitter);
        }
    }

    public Conditional[] conditionals;

    public bool HasDoongesGreaterOrEqualTo(int numDoonges)
    {
        return GameManager.doonges >= numDoonges;
    }

    public void Invoke0(DialogueEmitter emitter)
    {
        Conditional conditional = conditionals[0];
        (conditional.predicate.Invoke(emitter) ? conditional.ifTrue : conditional.ifFalse).Invoke(emitter);
    }

    public void Invoke1(DialogueEmitter emitter)
    {
        Conditional conditional = conditionals[1];
        (conditional.predicate.Invoke(emitter) ? conditional.ifTrue : conditional.ifFalse).Invoke(emitter);
    }

    public void Invoke2(DialogueEmitter emitter)
    {
        Conditional conditional = conditionals[2];
        (conditional.predicate.Invoke(emitter) ? conditional.ifTrue : conditional.ifFalse).Invoke(emitter);
    }

    public void Invoke3(DialogueEmitter emitter)
    {
        Conditional conditional = conditionals[3];
        (conditional.predicate.Invoke(emitter) ? conditional.ifTrue : conditional.ifFalse).Invoke(emitter);
    }

    public void Invoke4(DialogueEmitter emitter)
    {
        Conditional conditional = conditionals[4];
        (conditional.predicate.Invoke(emitter) ? conditional.ifTrue : conditional.ifFalse).Invoke(emitter);
    }
}