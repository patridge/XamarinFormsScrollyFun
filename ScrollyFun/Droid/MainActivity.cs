using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ScrollyFun.Droid {
    [Activity(Label = "ScrollyFun.Droid", Icon = "@drawable/icon", Theme = "@style/SomeStyles", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity {
        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
        }
    }
}
