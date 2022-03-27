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

class ConnectionQueueSlot {
    public BaseGate executor { get; set; }
    public PlainPipe connection { get; set; }
}

public class Level {
    public Scheme scheme; 
    public string levelName;
    public int turns;
    public string? levelPath;

    public void init(){
        int difficulty = 0;
        foreach (BaseGate item in this.scheme.getGates())
        {
            if (item.is_hidden) difficulty = difficulty + 1;
        }
        this.turns = difficulty * 2 + 3;
    }
    public void fromJson(string json, bool metaOnly) {
        PlainLevel pl = JsonSerializer.Deserialize<PlainLevel>(json);
        this.levelName = pl.name;

        this.scheme = new();
        if (!metaOnly) {
            Dictionary<int, BaseGate> IDReference = new();
            List<ConnectionQueueSlot> connectionQueue = new();

            foreach (KeyValuePair<int, PlainBaseComponent> entry in pl.body)
            {   
                PlainBaseComponent pb = entry.Value;
                Type GateType = Alliases.nameMeanings[pb.type];
                dynamic gate = Activator.CreateInstance(GateType);
                gate.is_hidden = !pb.visible;
                gate.gamefieldX = pb.X;
                gate.gamefieldY = pb.Y;
                IDReference[entry.Key] = gate;
                foreach (PlainPipe pipe in pb.output)
                {
                    ConnectionQueueSlot cq = new();
                    cq.connection = pipe;
                    cq.executor = gate;
                    connectionQueue.Add(cq);
                }
                this.scheme.addGate(gate);
            }

            foreach (ConnectionQueueSlot item in connectionQueue)
            {
                item.executor.connect(IDReference[item.connection.destination], item.connection.from_slot, item.connection.to_slot);
            }

            this.scheme.compile();
        }
    }

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