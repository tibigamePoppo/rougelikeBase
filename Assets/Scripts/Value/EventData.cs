using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ResourceValue/EventData")]
public class EventData : ScriptableObject
{
    public string eventName;
    [TextArea]
    public string text;
    public Sprite sprite;
    public EventLimit limit;
    public List<EventEffectArg> eventEffectArgs;
}
