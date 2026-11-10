
namespace Volly;

public partial class SignupPage : ContentPage
{
    private string selectedRole = "Player";
    private bool isPasswordVisible = false;

    public SignupPage()
    {
        InitializeComponent();

        UpdateRoleSelection();
    }

    // BACK BUTTON

    private async void OnBackTapped(
        object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // SELECT PLAYER

    private void OnPlayerSelected(
        object sender, TappedEventArgs e)
    {
        selectedRole = "Player";

        InvitationCodeSection.IsVisible = true;

        UpdateRoleSelection();
    }

    // SELECT COACH

    private void OnCoachSelected(
        object sender, TappedEventArgs e)
    {
        selectedRole = "Coach";

        InvitationCodeSection.IsVisible = false;

        UpdateRoleSelection();
    }

    // UPDATE ROLE HIGHLIGHT

    private void UpdateRoleSelection()
    {
        bool isPlayer = selectedRole == "Player";

        PlayerRoleBorder.Background =
            new SolidColorBrush(
                isPlayer
                ? Color.FromArgb("#1B2942")
                : Colors.Transparent);

        CoachRoleBorder.Background =
            new SolidColorBrush(
                !isPlayer
                ? Color.FromArgb("#1B2942")
                : Colors.Transparent);

        PlayerRoleLabel.TextColor = Colors.White;
        CoachRoleLabel.TextColor = Colors.White;
    }

    // SHOW / HIDE PASSWORD

    private void OnPasswordToggleClicked(
        object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;

        PasswordEntry.IsPassword = !isPasswordVisible;

        PasswordToggleButton.Text =
            isPasswordVisible ? "◎" : "◉";
    }

    // CREATE ACCOUNT

    private async void OnCreateAccountClicked(
        object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
        {
            await DisplayAlert(
                "Error",
                "Please enter your full name.",
                "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert(
                "Error",
                "Please enter your email or username.",
                "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert(
                "Error",
                "Please enter your password.",
                "OK");
            return;
        }

        if (!TermsCheckBox.IsChecked)
        {
            await DisplayAlert(
                "Error",
                "Please agree to the Terms and Conditions.",
                "OK");
            return;
        }

        await Shell.Current.GoToAsync("//PlayerHP");

    }

    // LOGIN


    private async void OnLoginTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("LoginPage");
    }

}
