namespace Volly;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("SignupPage", typeof(SignupPage));
    }
}