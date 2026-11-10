
namespace Volly;

public partial class PlayerHP : ContentPage
{
    // Goals assigned by the coach.
    // Later these values will come from the database.
    private const double WaterGoal = 3.0;
    private const double SleepGoal = 8.0;

    // Player's reported results.
    private double waterConsumed = 0;
    private double sleepHours = 0;
    private bool exercisesCompleted = false;
    private bool exercisesReported = false;

    private string attendance = "";

    // Demo value until previous week's reports are available.
    private double? previousWeekProgress = null;

    public PlayerHP()
    {
        InitializeComponent();

        UpdateGreeting();
        UpdateWater();
        UpdateSleep();
        UpdateWeeklyProgress();
    }

    private void UpdateGreeting()
    {
        int hour = DateTime.Now.Hour;

        if (hour >= 5 && hour < 12)
            GreetingLabel.Text = "Good morning,";
        else if (hour >= 12 && hour < 17)
            GreetingLabel.Text = "Good afternoon,";
        else if (hour >= 17 && hour < 22)
            GreetingLabel.Text = "Good evening,";
        else
            GreetingLabel.Text = "Good night,";
    }

    // ---------------- ATTENDANCE ----------------

    private void SetAttendance(string value)
    {
        attendance = value;

        GoingButton.BackgroundColor =
            value == "Going" ? Color.FromArgb("#253D65") : Color.FromArgb("#0D172A");

        MaybeButton.BackgroundColor =
            value == "Maybe" ? Color.FromArgb("#253D65") : Color.FromArgb("#0D172A");

        CantAttendButton.BackgroundColor =
            value == "Can't attend" ? Color.FromArgb("#253D65") : Color.FromArgb("#0D172A");

        GoingButton.TextColor = value == "Going" ? Colors.White : Color.FromArgb("#A8B5CC");
        MaybeButton.TextColor = value == "Maybe" ? Colors.White : Color.FromArgb("#A8B5CC");
        CantAttendButton.TextColor = value == "Can't attend" ? Colors.White : Color.FromArgb("#A8B5CC");
    }

    private void OnGoingClicked(object sender, EventArgs e)
    {
        SetAttendance("Going");
    }

    private void OnMaybeClicked(object sender, EventArgs e)
    {
        SetAttendance("Maybe");
    }

    private void OnCantAttendClicked(object sender, EventArgs e)
    {
        SetAttendance("Can't attend");
    }

    // ---------------- WATER ----------------

    private async void OnWaterTapped(object sender, TappedEventArgs e)
    {
        string result = await DisplayPromptAsync(
            "Water intake",
            $"Your coach's goal is {WaterGoal:0.#} liters.\nHow many liters did you drink today?",
            "Save",
            "Cancel",
            "Example: 2.5",
            keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(result))
            return;

        if (double.TryParse(result.Replace(',', '.'),
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double amount)
            && amount >= 0)
        {
            waterConsumed = amount;
            UpdateWater();
            UpdateWeeklyProgress();
        }
        else
        {
            await DisplayAlert("Invalid value",
                "Please enter a valid number of liters.",
                "OK");
        }
    }

    private void UpdateWater()
    {
        WaterProgressLabel.Text =
            $"{waterConsumed:0.##} L / {WaterGoal:0.#} L";
    }

    // ---------------- SLEEP ----------------

    private async void OnSleepTapped(object sender, TappedEventArgs e)
    {
        string result = await DisplayPromptAsync(
            "Sleep report",
            $"Your coach's goal is {SleepGoal:0.#} hours.\nHow many hours did you sleep?",
            "Save",
            "Cancel",
            "Example: 6.5",
            keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(result))
            return;

        if (double.TryParse(result.Replace(',', '.'),
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double amount)
            && amount >= 0)
        {
            sleepHours = amount;
            UpdateSleep();
            UpdateWeeklyProgress();
        }
        else
        {
            await DisplayAlert("Invalid value",
                "Please enter a valid number of hours.",
                "OK");
        }
    }

    private void UpdateSleep()
    {
        SleepProgressLabel.Text =
            $"Reported: {sleepHours:0.##} / {SleepGoal:0.#} hours";
    }

    // ---------------- EXERCISES ----------------

    private async void OnExerciseTapped(object sender, TappedEventArgs e)
    {
        string answer = await DisplayActionSheet(
            "Explosive jump series",
            "Cancel",
            null,
            "Completed",
            "Not completed");

        if (answer == "Completed")
        {
            exercisesCompleted = true;
            exercisesReported = true;
        }
        else if (answer == "Not completed")
        {
            exercisesCompleted = false;
            exercisesReported = true;
        }
        else
        {
            return;
        }

        UpdateWeeklyProgress();

        await DisplayAlert(
            "Report saved",
            exercisesCompleted
                ? "Your coach's exercise was marked as completed."
                : "Your report was saved as not completed.",
            "OK");
    }

    // ---------------- WEEKLY PROGRESS ----------------

    private void UpdateWeeklyProgress()
    {
        // Only calculate the goals that the player has reported.
        double total = 0;
        int reportedGoals = 0;

        // Water
        total += Math.Min(waterConsumed / WaterGoal, 1.0) * 100;
        reportedGoals++;

        // Sleep
        total += Math.Min(sleepHours / SleepGoal, 1.0) * 100;
        reportedGoals++;

        // Exercises
        if (exercisesReported)
        {
            total += exercisesCompleted ? 100 : 0;
            reportedGoals++;
        }

        double progress = reportedGoals == 0
            ? 0
            : total / reportedGoals;

        WeeklyPercentageLabel.Text = $"{progress:0}%";

        // Temporary visual bar. It will be replaced with a real
        // weekly history calculation when daily reports are stored.
        double screenWidth =
            DeviceDisplay.MainDisplayInfo.Width /
            DeviceDisplay.MainDisplayInfo.Density;

        WeeklyProgressBar.WidthRequest =
            Math.Max(0, (screenWidth - 90) * progress / 100);

        if (previousWeekProgress.HasValue)
        {
            double difference = progress - previousWeekProgress.Value;
            string sign = difference > 0 ? "+" : "";

            WeeklyComparisonLabel.Text =
                $"{sign}{difference:0}% compared to last week";
        }
        else
        {
            WeeklyComparisonLabel.Text = "Previous week: no data yet";
        }
    }

    // ---------------- OTHER BUTTONS ----------------

    private async void OnViewTeamTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("My Team",
            "The team page will be created next.",
            "OK");
    }

    private async void OnViewPlanTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Today's plan",
            "The full training plan will be available here.",
            "OK");
    }


    private async void OnTacticsTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Tactics");
    }


    private async void OnViewTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("View",
            "The View page will be created soon.",
            "OK");
    }

    private async void OnBookTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Book",
            "The Book page will be created soon.",
            "OK");
    }

    private async void OnYouTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("You",
            "The profile page will be created soon.",
            "OK");
    }
}
