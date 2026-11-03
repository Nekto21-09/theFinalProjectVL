
namespace Volly;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Draw volleyball
        VolleyballView.Drawable = new VolleyballDrawable();
    }

    private async void OnGetStartedTapped(
        object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("SignupPage");
    }

    private async void OnLoginTapped(
        object sender, TappedEventArgs e)
    {
        await DisplayAlert(
            "Login",
            "Login page will open here.",
            "OK");
    }
}


// ==========================================
// VOLLEYBALL
// ==========================================

public class VolleyballDrawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float scale = Math.Min(
            dirtyRect.Width,
            dirtyRect.Height) / 120f;

        canvas.SaveState();

        canvas.Translate(
            (dirtyRect.Width - 120 * scale) / 2,
            (dirtyRect.Height - 120 * scale) / 2);

        canvas.Scale(scale, scale);

        PathF circleClip = new PathF();

        circleClip.MoveTo(60, 2);

        circleClip.CurveTo(92, 2, 118, 28, 118, 60);
        circleClip.CurveTo(118, 92, 92, 118, 60, 118);
        circleClip.CurveTo(28, 118, 2, 92, 2, 60);
        circleClip.CurveTo(2, 28, 28, 2, 60, 2);

        circleClip.Close();

        canvas.ClipPath(circleClip);

        canvas.FillColor = Color.FromArgb("#F5FAFF");
        canvas.FillRectangle(0, 0, 120, 120);

        canvas.FillColor = Color.FromArgb("#E3EDF5");
        canvas.FillRectangle(105, 0, 15, 120);

        canvas.StrokeColor = Color.FromArgb("#80B5E7");
        canvas.StrokeSize = 7;

        PathF panel1 = new PathF();

        panel1.MoveTo(42, -5);
        panel1.CurveTo(43, 15, 48, 36, 62, 38);
        panel1.CurveTo(76, 38, 79, 15, 80, -5);

        canvas.DrawPath(panel1);

        PathF panel2 = new PathF();

        panel2.MoveTo(-5, 85);
        panel2.CurveTo(17, 82, 35, 58, 37, 34);
        panel2.CurveTo(38, 19, 31, 3, 22, -5);

        canvas.DrawPath(panel2);

        PathF panel3 = new PathF();

        panel3.MoveTo(84, -5);
        panel3.CurveTo(72, 17, 69, 43, 80, 65);
        panel3.CurveTo(89, 83, 108, 91, 125, 91);

        canvas.DrawPath(panel3);

        PathF panel4 = new PathF();

        panel4.MoveTo(20, 125);
        panel4.CurveTo(35, 98, 54, 79, 76, 77);
        panel4.CurveTo(97, 75, 115, 88, 125, 105);

        canvas.DrawPath(panel4);

        canvas.RestoreState();
    }
}
