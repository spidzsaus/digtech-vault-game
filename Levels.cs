namespace Levels;

using GameComponents;
using System.Text.Json;
using System.Text.Json.Serialization;

class PlainPipe {
    public int from_slot  { get; set; }
    public int to_slot { get; set; }
    public int destination { get; set; }
}

class PlainBaseComponent {
    public string type { get; set; }
    public bool visible { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public List<PlainPipe> output { get; set; }
}

class PlainLevel {
    public string name { get; set; }
    public Dictionary<int, PlainBaseComponent> body { get; set; }

}

public class Level {
    public Scheme scheme; 
    public string levelName;

    public string toJson() {
        Dictionary<BaseGate, int> IDReference = new();
        int maxID = 0;
        foreach (BaseGate component in this.scheme.getGates())
        {
            IDReference[component] = maxID;
            maxID = maxID + 1;
        }

        PlainLevel pl = new();
        pl.name = this.levelName;

        pl.body = new();
        foreach (BaseGate component in this.scheme.getGates())
        {
            PlainBaseComponent pc = new();
            pc.type = Alliases.componentNames[component.GetType()];
            pc.visible = !component.is_hidden;
            pc.X = component.gamefieldX;
            pc.Y = component.gamefieldY;
            pc.output = new();
            foreach (Pipe con in component.recievers)
            {
                PlainPipe pp = new();
                pp.from_slot = con.source_slot;
                pp.to_slot = con.dest_slot;
                pp.destination = IDReference[con.dest]; 
                pc.output.Add(pp);
            }
            pl.body[IDReference[component]] = pc;
        }
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(pl, options);
        return jsonString;
    }

}