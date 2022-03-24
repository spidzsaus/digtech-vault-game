namespace digtech_vault_game;
using LogiWidgets;
using GameComponents;
using System;

public partial class Form1 : Form
{

    LevelViewer levelViewer;
    public Form1()
    {
        InitializeComponent();
    }

    public void openLevel(Level level){
        levelViewer = new(level);
        this.Controls.Add(levelViewer);
    }

    protected override void OnPaint(PaintEventArgs e) {
        base.OnPaint(e);
        foreach (LogiComponent component in this.logiComponents)
        {
            component.draw(e, 10, 10, 100f);
        }

    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        
    }
}
