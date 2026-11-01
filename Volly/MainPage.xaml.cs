namespace Volly;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnSignupClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new SignupPage());
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new LoginPage());
;    }
}

