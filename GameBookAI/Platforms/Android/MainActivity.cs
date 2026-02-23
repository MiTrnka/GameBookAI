using Android.App;
using Android.Content.PM;
using Android.OS;

namespace GameBookAI
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            // Předání null místo savedInstanceState zabrání MAUI v pokusu o obnovu
            // zastaralého stavu Shellu, který po nasazení nové verze může způsobit
            // zamrznutí na načítací obrazovce.
            base.OnCreate(null);
        }
    }
}
