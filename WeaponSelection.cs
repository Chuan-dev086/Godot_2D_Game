using Godot;

public partial class WeaponSelection : Control
{
    private string _selectedWeapon = "";

    // Reference to the GameSettings Autoload
    private GameSettings _gameSettings;

    // UI references
    private Label _selectedWeaponLabel;
    private Button _confirmButton;

    public override void _Ready()
    {
        // Get GameSettings Autoload
        _gameSettings = GetNode<GameSettings>("/root/GameSettings");

        // Get UI nodes
        _selectedWeaponLabel = GetNode<Label>("Background/MainContainer/SelectedWeaponLabel");

        _confirmButton = GetNode<Button>("Background/MainContainer/ConfirmButton");

        // Disable button until a weapon is selected
        _confirmButton.Disabled = true;

        // Connect weapon card events
        ConnectWeaponCard("PistolCard", "Pistol");
        ConnectWeaponCard("SMGCard", "SMG");
        ConnectWeaponCard("ShotgunCard", "Shotgun");
        ConnectWeaponCard("SniperCard", "Sniper");

        // Connect confirm button
        _confirmButton.Pressed += OnConfirmPressed;
    }

    private void ConnectWeaponCard(string cardName, string weaponName)
    {
        PanelContainer card = GetNode<PanelContainer>(
            $"Background/MainContainer/WeaponCards/{cardName}"
        );

        card.MouseFilter = Control.MouseFilterEnum.Stop;

        SetChildrenMouseFilter(card);

        card.GuiInput += (InputEvent inputEvent) =>
        {
            if (
                inputEvent is InputEventMouseButton mouseEvent
                && mouseEvent.Pressed
                && mouseEvent.ButtonIndex == MouseButton.Left
            )
            {
                SelectWeapon(weaponName);
            }
        };
    }

    private void SetChildrenMouseFilter(Node parent)
    {
        foreach (Node child in parent.GetChildren())
        {
            if (child is Control control)
            {
                control.MouseFilter = Control.MouseFilterEnum.Ignore;
            }

            SetChildrenMouseFilter(child);
        }
    }

    private void SelectWeapon(string weaponName)
    {
        _selectedWeapon = weaponName;

        _selectedWeaponLabel.Text = $"Selected Weapon: {_selectedWeapon}";

        _confirmButton.Disabled = false;

        GD.Print($"Selected Weapon: {_selectedWeapon}");
    }

    private void OnConfirmPressed()
    {
        if (string.IsNullOrEmpty(_selectedWeapon))
        {
            return;
        }

        // Save selected weapon
        _gameSettings.InitialWeapon = _selectedWeapon;

        GD.Print($"Starting game with: {_selectedWeapon}");

        // Switch to Main scene
        GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
    }
}
