
namespace Volly;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("SignupPage", typeof(SignupPage));
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("PlayerHP", typeof(PlayerHP));
        Routing.RegisterRoute("Tactics", typeof(Tactics));
    }
}
