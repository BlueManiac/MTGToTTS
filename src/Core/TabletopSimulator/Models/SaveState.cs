namespace Core.TabletopSimulator;

public class SaveState
{
    public List<ObjectState> ObjectStates { get; set; }
}

public class ObjectState
{
    public string Name { get; set; }
    public TransformState Transform { get; set; }
    public string Nickname { get; set; }
    public string Description { get; set; }
    public int? CardID { get; set; }
    public List<int>? DeckIDs { get; set; }
    public Dictionary<int, CustomDeckState>? CustomDeck { get; set; }
    public List<ObjectState>? ContainedObjects { get; set; }
}

#pragma warning disable IDE1006 // Naming Styles

public class TransformState
{
    public int? posX { get; set; }
    public int? posY { get; set; }
    public int? posZ { get; set; }

    public int rotX { get; set; }
    public int rotY { get; set; }
    public int rotZ { get; set; }

    public float scaleX { get; set; }
    public float scaleY { get; set; }
    public float scaleZ { get; set; }
}

public class CustomDeckState
{
    public string FaceURL { get; set; }
    public string BackURL { get; set; }
    public int NumHeight { get; set; }
    public int NumWidth { get; set; }
    public bool BackIsHidden { get; set; }
}