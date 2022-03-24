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
        levelViewer.openLevel(level);
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        
    }
}
