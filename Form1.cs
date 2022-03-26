namespace digtech_vault_game;
using LogiWidgets;
using GameComponents;
using System;
using Scenes;

public partial class Form1 : Form
{

    public SceneManager sceneManager;
    public Form1()
    {
        InitializeComponent();
        this.sceneManager = new();
        this.sceneManager.Width = 800;
        this.sceneManager.Height = 800;
        this.Controls.Add(this.sceneManager);
    }

}
