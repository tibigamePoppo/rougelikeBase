namespace Scenes.EventScene
{
    public class EventBase
    {
        public string buttonText = "tempText";
        public EventEffectArg effectArg;
        public EventBase(string buttonText, EventEffectArg effectArg)
        {
            this.buttonText = buttonText;
            this.effectArg = effectArg;
        }
    }
}