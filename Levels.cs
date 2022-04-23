namespace Levels;

using GameComponents;
using System.Text.Json;
using System.Text.Json.Serialization;

class PlainCertificate {
    public string token { get; set; }
    public string owner { get; set; }
    public string level { get; set; }
}

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


static class CertificateManager {
    static string xorcrypt (string text)
    {
        string key = "102392892831209";
        var result = new System.Text.StringBuilder();
        for (int c = 0; c < text.Length; c++)
            result.Append((char)((uint)text[c] ^ (uint)key[c % key.Length]));
        return result.ToString();
    }

    static public string generateCertificateToken(byte[] id,  string levelbody) {
        System.Security.Cryptography.HashAlgorithm algo = new System.Security.Cryptography.SHA256Managed();
        string body = levelbody;
        byte[] bytes = new byte[body.Length];

        for (int i = 0; i < body.Length; i++)
        {
            bytes[i] = ((byte)body[i]);
        }
        byte[] levelCode = algo.ComputeHash(bytes); 

        byte[] bytes1 = new byte[id.Length + levelCode.Length];

        for (int i = 0; i < id.Length; i++)
        {
            bytes1[i] = id[i];
        }
        for (int i = 0; i < levelCode.Length; i++)
        {
            bytes1[id.Length + i] = levelCode[i];
        }
        byte[] token = algo.ComputeHash(bytes1);
        for (int i = 0; i < token.Length; i++)
        {
            token[i] = (byte)((int)(token[i]) % 128);
        }
        return System.Text.Encoding.Default.GetString(token);
    }

    static public string generateCertificate(string card_name, string level_name) {
        string card_path = @"gamedata/profile/" + card_name + ".card";
        byte[] id = System.IO.File.ReadAllBytes(card_path);

        string level_path = @"gamedata/levels/" + level_name;
        string body = System.IO.File.ReadAllText(level_path);

        string token = CertificateManager.generateCertificateToken(id, body);

        PlainCertificate plain = new();
        plain.token = token;
        plain.owner = CertificateManager.xorcrypt(card_name);
        plain.level = CertificateManager.xorcrypt(level_name);

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(plain, options);
        return jsonString;
    }

    static public bool validateCertificate(string path) {
        if (!File.Exists(path)) {
            return false;
        } else {
            
            string json = System.IO.File.ReadAllText(path);
            PlainCertificate pc = JsonSerializer.Deserialize<PlainCertificate>(json);
            string test = pc.token;
            string card_path = @"gamedata/profile/" + CertificateManager.xorcrypt(pc.owner) + ".card";
            string level_path = @"gamedata/levels/" + CertificateManager.xorcrypt(pc.level);
            byte[] id = System.IO.File.ReadAllBytes(card_path);
            string body = System.IO.File.ReadAllText(level_path);
            string actualCertificate = CertificateManager.generateCertificateToken(id, body);
            
            return test.SequenceEqual(actualCertificate);
        }
    }
}

public class Level {
    public Scheme scheme; 
    public string levelName;
    public int turns;
    public string? levelPath;
    byte[]? levelCode = null;

    public void init(){
        int difficulty = 0;
        foreach (BaseGate item in this.scheme.getGates())
        {
            if (item.is_hidden) difficulty = difficulty + 1;
        }
        this.turns = difficulty * 2 + 2;
    }
    public bool fromJson(string json, bool metaOnly) {
        try
        {
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
                /*System.Security.Cryptography.HashAlgorithm algo = new System.Security.Cryptography.SHA256Managed();
                string body = json;
                byte[] bytes = new byte[body.Length];

                for (int i = 0; i < body.Length; i++)
                {
                    bytes[i] = ((byte)body[i]);
                }
                this.levelCode = algo.ComputeHash(bytes); */
            }
            return true;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
    public string certificatePath(string login) {
        string levelFileName = levelPath;
        int j = levelFileName.LastIndexOf('/');
        levelFileName = levelFileName.Substring(j + 1, levelFileName.Length - j - 1);

        string path = @"gamedata/certificates/" + login + "/" + levelFileName + ".certificate";
        return path;
    }
    public void createCertificateFile(string login) {
        string path = this.certificatePath(login);
        System.IO.Directory.CreateDirectory(@"gamedata/certificates/" + login + "/");
        string levelFileName = levelPath;
        int j = levelFileName.LastIndexOf('/');
        levelFileName = levelFileName.Substring(j + 1, levelFileName.Length - j - 1);

        string certificate = CertificateManager.generateCertificate(login, levelFileName);
        System.IO.File.WriteAllText(path, certificate);
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
            foreach (List<Pipe> slot in component.recievers!) {
            foreach (Pipe con in slot)
            {
                PlainPipe pp = new();
                pp.from_slot = con.source_slot;
                pp.to_slot = con.dest_slot;
                pp.destination = IDReference[con.dest]; 
                pc.output.Add(pp);
            }
            }
            pl.body[IDReference[component]] = pc;
        }
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(pl, options);
        return jsonString;
    }

}