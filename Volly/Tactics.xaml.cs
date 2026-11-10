
using Microsoft.Maui.Platform;

namespace Volly;

public partial class Tactics : ContentPage
{
    private readonly Dictionary<Border, Point> originalPositions = new();
    private readonly Dictionary<Border, Point> dragStartPositions = new();
    private readonly Dictionary<Border, Point> soloDragStartPositions = new();

    private string selectedScenario = "Serve Receive";

    private sealed class DemoPlayer
    {
        public int Number { get; init; }
        public string Name { get; init; } = "";
        public string Position { get; init; } = "";
        public int Attack { get; init; }
        public int Reception { get; init; }
        public int Blocking { get; init; }
        public int Speed { get; init; }
        public int Defense { get; init; }
    }

    private readonly List<DemoPlayer> demoPlayers = new()
    {
        new() { Number = 1, Name = "Player 1", Position = "Setter", Attack = 76, Reception = 84, Blocking = 70, Speed = 90, Defense = 85 },
        new() { Number = 4, Name = "Player 4", Position = "Outside Hitter", Attack = 91, Reception = 82, Blocking = 78, Speed = 86, Defense = 80 },
        new() { Number = 7, Name = "Player 7", Position = "Middle Blocker", Attack = 84, Reception = 68, Blocking = 93, Speed = 78, Defense = 76 },
        new() { Number = 9, Name = "Player 9", Position = "Opposite", Attack = 94, Reception = 70, Blocking = 85, Speed = 82, Defense = 74 },
        new() { Number = 11, Name = "Player 11", Position = "Middle Blocker", Attack = 82, Reception = 72, Blocking = 89, Speed = 80, Defense = 78 },
        new() { Number = 14, Name = "Player 14", Position = "Libero", Attack = 45, Reception = 97, Blocking = 40, Speed = 94, Defense = 98 }
    };

    public Tactics()
    {
        InitializeComponent();
        SaveOriginalPositions();
        SaveSoloOriginalPositions();
    }

    // ---------------- BOARD ----------------

    private void SaveOriginalPositions()
    {
        foreach (View view in PlayerLayer.Children)
        {
            if (view is Border player)
            {
                Rect bounds = AbsoluteLayout.GetLayoutBounds(player);
                originalPositions[player] = new Point(bounds.X, bounds.Y);
            }
        }
    }

    private void SaveSoloOriginalPositions()
    {
        foreach (View view in SoloPlayerLayer.Children)
        {
            if (view is Border player)
            {
                Rect bounds = AbsoluteLayout.GetLayoutBounds(player);
                soloDragStartPositions[player] = new Point(bounds.X, bounds.Y);
            }
        }
    }

    private async void OnPlayerTapped(object sender, TappedEventArgs e)
    {
        if (sender is not Border player)
            return;

        string currentNumber = (player.Content as Label)?.Text ?? "";

        string result = await DisplayPromptAsync(
            "Edit player",
            "Enter a new player number:",
            "Save",
            "Cancel",
            "Maximum 2 digits",
            keyboard: Keyboard.Numeric,
            initialValue: currentNumber);

        if (string.IsNullOrWhiteSpace(result))
            return;

        if (int.TryParse(result, out int number) && number >= 0 && number <= 99)
        {
            if (player.Content is Label label)
                label.Text = number.ToString();
        }
        else
        {
            await DisplayAlert("Invalid number",
                "Enter a number between 0 and 99.", "OK");
        }
    }

    private void OnPlayerPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is not Border player)
            return;

        if (e.StatusType == GestureStatus.Started)
        {
            Rect bounds = AbsoluteLayout.GetLayoutBounds(player);
            dragStartPositions[player] = new Point(bounds.X, bounds.Y);
        }
        else if (e.StatusType == GestureStatus.Running)
        {
            if (!dragStartPositions.TryGetValue(player, out Point start))
                return;

            if (PlayerLayer.Width <= 0 || PlayerLayer.Height <= 0)
                return;

            double newX = Math.Clamp(
                start.X + e.TotalX / Math.Max(1, PlayerLayer.Width - 40), 0, 1);

            double newY = Math.Clamp(
                start.Y + e.TotalY / Math.Max(1, PlayerLayer.Height - 40), 0, 1);

            AbsoluteLayout.SetLayoutBounds(player, new Rect(newX, newY, 40, 40));
        }
        else if (e.StatusType == GestureStatus.Completed ||
                 e.StatusType == GestureStatus.Canceled)
        {
            dragStartPositions.Remove(player);
        }
    }

    private async void OnFormationTapped(object sender, TappedEventArgs e)
    {
        string choice = await DisplayActionSheet(
            "Choose formation", "Cancel", null,
            "5-1 Formation", "6-0 Formation", "4-2 Formation");

        if (!string.IsNullOrEmpty(choice) && choice != "Cancel")
            FormationLabel.Text = choice;
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        foreach (var item in originalPositions)
        {
            AbsoluteLayout.SetLayoutBounds(
                item.Key,
                new Rect(item.Value.X, item.Value.Y, 40, 40));
        }
    }

    private async void OnSaveTacticClicked(object sender, EventArgs e)
    {
        var numbers = PlayerLayer.Children
            .OfType<Border>()
            .Select(p => (p.Content as Label)?.Text ?? "?");

        await DisplayAlert(
            "Tactic saved",
            $"Your current formation: {FormationLabel.Text}\n\nPlayers: {string.Join(", ", numbers)}\n\nThis is a temporary save.",
            "OK");
    }

    // ---------------- TABS ----------------

    private void ShowBoard()
    {
        BoardContent.IsVisible = true;
        SoloSimContent.IsVisible = false;
    }

    private void ShowSoloSim()
    {
        BoardContent.IsVisible = false;
        SoloSimContent.IsVisible = true;
        SimulationResults.IsVisible = false;
    }

    private void OnBoardTabClicked(object sender, EventArgs e) => ShowBoard();

    private void OnSoloSimTabClicked(object sender, EventArgs e) => ShowSoloSim();

    private async void OnVsAiTabClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Vs AI",
            "We will build this section after Solo Simulation.", "OK");
    }

    private async void OnPerformanceTabClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Performance",
            "We will build the statistics section later.", "OK");
    }

    private async void OnMyTacticsTabClicked(object sender, EventArgs e)
    {
        await DisplayAlert("My Tactics",
            "Saved tactics will be displayed here.", "OK");
    }

    // ---------------- SOLO SIM COURT ----------------

    private void OnSoloPlayerPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is not Border player)
            return;

        if (e.StatusType == GestureStatus.Started)
        {
            Rect bounds = AbsoluteLayout.GetLayoutBounds(player);
            soloDragStartPositions[player] = new Point(bounds.X, bounds.Y);
        }
        else if (e.StatusType == GestureStatus.Running)
        {
            if (!soloDragStartPositions.TryGetValue(player, out Point start))
                return;

            if (SoloPlayerLayer.Width <= 0 || SoloPlayerLayer.Height <= 0)
                return;

            double newX = Math.Clamp(
                start.X + e.TotalX / Math.Max(1, SoloPlayerLayer.Width - 40), 0, 1);

            double newY = Math.Clamp(
                start.Y + e.TotalY / Math.Max(1, SoloPlayerLayer.Height - 40), 0, 1);

            AbsoluteLayout.SetLayoutBounds(
                player, new Rect(newX, newY, 40, 40));
        }
        else if (e.StatusType == GestureStatus.Completed ||
                 e.StatusType == GestureStatus.Canceled)
        {
            soloDragStartPositions.Remove(player);
        }
    }

    private void OnSoloResetClicked(object sender, EventArgs e)
    {
        foreach (var item in soloDragStartPositions)
        {
            AbsoluteLayout.SetLayoutBounds(
                item.Key,
                new Rect(item.Value.X, item.Value.Y, 40, 40));
        }
    }

    // ---------------- SCENARIOS ----------------

    private void SelectScenario(string scenario)
    {
        selectedScenario = scenario;

        ScenarioDescriptionLabel.Text = scenario switch
        {
            "Serve Receive" => "Serve Receive · Medium difficulty",
            "Defensive Coverage" => "Defensive Coverage · Hard difficulty",
            "Transition Attack" => "Transition Attack · Medium difficulty",
            _ => scenario
        };

        ServeReceiveButton.BackgroundColor =
            scenario == "Serve Receive" ? Color.FromArgb("#253D65") : Color.FromArgb("#111B30");

        DefensiveCoverageButton.BackgroundColor =
            scenario == "Defensive Coverage" ? Color.FromArgb("#253D65") : Color.FromArgb("#111B30");

        TransitionAttackButton.BackgroundColor =
            scenario == "Transition Attack" ? Color.FromArgb("#253D65") : Color.FromArgb("#111B30");

        ServeReceiveButton.TextColor =
            scenario == "Serve Receive" ? Colors.White : Color.FromArgb("#A8B5CC");

        DefensiveCoverageButton.TextColor =
            scenario == "Defensive Coverage" ? Colors.White : Color.FromArgb("#A8B5CC");

        TransitionAttackButton.TextColor =
            scenario == "Transition Attack" ? Colors.White : Color.FromArgb("#A8B5CC");

        SimulationResults.IsVisible = false;
    }

    private void OnServeReceiveClicked(object sender, EventArgs e) =>
        SelectScenario("Serve Receive");

    private void OnDefensiveCoverageClicked(object sender, EventArgs e) =>
        SelectScenario("Defensive Coverage");

    private void OnTransitionAttackClicked(object sender, EventArgs e) =>
        SelectScenario("Transition Attack");

    // ---------------- PLAYER PROFILES ----------------

    private async void OnSoloPlayerClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        int number = button.Text.Contains("14") ? 14 :
                     button.Text.Contains("11") ? 11 :
                     button.Text.Contains("9") ? 9 :
                     button.Text.Contains("7") ? 7 :
                     button.Text.Contains("4") ? 4 : 1;

        DemoPlayer? player = demoPlayers.FirstOrDefault(p => p.Number == number);

        if (player == null)
            return;

        string profile =
            $"{player.Name} · #{player.Number}\n{player.Position}\n\n" +
            $"Attack: {player.Attack}/100\n" +
            $"Reception: {player.Reception}/100\n" +
            $"Blocking: {player.Blocking}/100\n" +
            $"Speed: {player.Speed}/100\n" +
            $"Defense: {player.Defense}/100";

        await DisplayAlert("Player attributes", profile, "Close");
    }

    // ---------------- SIMULATION ----------------

    private async void OnRunSimulationClicked(object sender, EventArgs e)
    {
        try
        {
            if (demoPlayers.Count == 0)
            {
                await DisplayAlert("No players",
                    "There are no players available for this simulation.", "OK");
                return;
            }

            double positioning = demoPlayers.Average(
                p => (p.Speed + p.Defense) / 2.0);

            double attack = demoPlayers.Average(p => p.Attack);

            double scenarioScore = selectedScenario switch
            {
                "Serve Receive" => demoPlayers.Average(
                    p => p.Reception * 0.65 + p.Defense * 0.35),

                "Defensive Coverage" => demoPlayers.Average(
                    p => p.Defense * 0.55 + p.Speed * 0.45),

                "Transition Attack" => demoPlayers.Average(
                    p => p.Attack * 0.65 + p.Speed * 0.35),

                _ => 0
            };

            int overall = (int)Math.Round(
                positioning * 0.30 + attack * 0.25 + scenarioScore * 0.45);

            overall = Math.Clamp(overall, 0, 100);

            int positioningValue = (int)Math.Round(positioning);
            int attackValue = (int)Math.Round(attack);

            string weakArea = selectedScenario switch
            {
                "Serve Receive" => "Reception",
                "Defensive Coverage" => "Defense",
                "Transition Attack" => "Attack",
                _ => "—"
            };

            int weakValue = weakArea switch
            {
                "Reception" => (int)Math.Round(demoPlayers.Average(p => p.Reception)),
                "Defense" => (int)Math.Round(demoPlayers.Average(p => p.Defense)),
                "Attack" => attackValue,
                _ => 0
            };

            SimulationScoreLabel.Text = $"{overall}%";

            SimulationRatingLabel.Text = overall >= 85
                ? "Strong result"
                : overall >= 70
                    ? "Good potential"
                    : "Needs improvement";

            PositioningScoreLabel.Text = $"{positioningValue}%";
            AttackScoreLabel.Text = $"{attackValue}%";
            WeakAreaLabel.Text = $"{weakArea} · {weakValue}%";

            SimulationAdviceLabel.Text = overall >= 85
                ? "Your lineup has a strong balance for this scenario. Try adjusting positions to explore other options."
                : $"Focus on improving {weakArea.ToLower()} and review the players' individual attributes.";

            SimulationResults.IsVisible = true;

            // Allow the results to appear, then scroll down to them.
            await Task.Yield();
            await MainScrollView.ScrollToAsync(
                SimulationResults, ScrollToPosition.End, true);
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Simulation error",
                ex.Message,
                "OK");
        }
    }

    // ---------------- BOTTOM NAVIGATION ----------------

    private async void OnHomeTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//PlayerHP");
    }

    private async void OnViewTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("View", "This page will be created later.", "OK");
    }

    private async void OnBookTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Book", "This page will be created later.", "OK");
    }

    private async void OnYouTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("You", "This page will be created later.", "OK");
    }
}
