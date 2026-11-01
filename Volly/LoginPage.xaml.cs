namespace Volly;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        HttpClient client = new HttpClient();

        try
        {
            HttpResponseMessage response = await client.GetAsync(
                $"http://localhost:5264/login?username={username}&password={password}");

            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.GoToAsync("//PlayerHP");
            }
            else
            {
                await DisplayAlert("Error", result, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}