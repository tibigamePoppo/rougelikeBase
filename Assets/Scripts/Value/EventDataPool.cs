using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourcePool/EventPool")]
public class EventDataPool : ScriptableObject
{
    public List<EventData> events;
}
