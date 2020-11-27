using System;

public class FloatingMenuController
{
    public FloatingMenuModel Model { get; private set; }
    public FloatingMenuView View { get; private set; }

    public FloatingMenuController(FloatingMenuModel model, FloatingMenuView view)
    {
        this.Model = model;
        this.View = view;
    }
}
