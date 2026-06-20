#if __ANDROID__
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Mario
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.Orientation |
                              ConfigChanges.KeyboardHidden |
                              ConfigChanges.Keyboard |
                              ConfigChanges.ScreenSize |
                              ConfigChanges.ScreenLayout |
                              ConfigChanges.SmallestScreenSize |
                              ConfigChanges.UiMode
    )]
    public class MainActivity : Microsoft.Xna.Framework.AndroidGameActivity
    {
        private MarioGame _game;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _game = new MarioGame();
            SetContentView((Android.Views.View)_game.Services.GetService(typeof(Android.Views.View)));
            _game.Run();
        }
    }
}
#endif