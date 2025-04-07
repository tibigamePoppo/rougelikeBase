using System;
using UnityEngine;

[Serializable]
public class Parameter {
  public Story _Parameter;
}

[Serializable]
public class Story {
  public int storyId;
  public string image;
  public string characterName;
  public string text;
}

[Serializable]
public class CharacterImage {
  public Sprite Sample1;
  public Sprite kuma;
  public Sprite nashi;
  public Sprite shika;
}

[Serializable]
public class Card {
  public int contentId;
  public string name;
  public int popularity;
  public int monetary;
  public int cuteness;
  public int year;
  public string text;
  public string image;
}

[Serializable]
public class MasterData {
  public Parameter[] Parameter;
  public Story[] Story;
  public CharacterImage CharacterImage;
  public Card[] Card;
}

