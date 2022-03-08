namespace digtech_vault_game;
using LogiWidgets;
using GameComponents;

public partial class Form1 : Form
{

    List<LogiComponent> logiComponents = new List<LogiComponent>();

    public Form1()
    {
        InitializeComponent();
    }

    public LogiComponent addGate(BaseGate gate) {
        LogiComponent component = new LogiComponent(gate);
        this.logiComponents.Add(component);
        return component;
    }

    protected override void OnPaint(PaintEventArgs e) {
        base.OnPaint(e);
        foreach (LogiComponent component in this.logiComponents)
        {
            component.draw(e, 10, 10, 100f);
        }

    }
}
