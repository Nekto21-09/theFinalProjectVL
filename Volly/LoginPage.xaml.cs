
namespace Volly;

public partial class LoginPage : ContentPage
{
    private bool isPasswordVisible = false;

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OnPasswordToggleClicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;

        PasswordEntry.IsPassword = !isPasswordVisible;
        PasswordToggleButton.Text = isPasswordVisible ? "◎" : "◉";
    }

    private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert(
            "Forgot password?",
            "Password recovery will be available soon.",
            "OK");
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Error", "Please enter your email or username.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Error", "Please enter your password.", "OK");
            return;
        }

        await DisplayAlert("Login", "Login functionality will be connected later.", "OK");
    }

    private async void OnCreateAccountTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("SignupPage");
    }
}
