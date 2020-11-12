using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Threading.Tasks;
using System.Drawing;
using Android.Graphics;
using Android.Content;
using Android.Views;
using Android.Media;

//This Activity contains the logic for the start screen.
namespace MatheQuiz
{
    [Activity(Label = "MatheQuiz", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        Button BtnStart;
        MediaPlayer BackgroundMusic;
        MediaPlayer ButtonClick;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.StartScreen);

            BtnStart = FindViewById<Button>(Resource.Id.btnStart);

            MediaPlayer BackgroundMusic = new MediaPlayer();
            BackgroundMusic = MediaPlayer.Create(this, Resource.Raw.BackgroundStart);
            BackgroundMusic.Start();

            ButtonClick = MediaPlayer.Create(this, Resource.Raw.button_click1);


            //Start the game by clicking on the button.
            BtnStart.Click += delegate
            {
                ButtonClick.Start();
                StartActivity(typeof(QuizActivity));
                BackgroundMusic.Pause();
            };
        }
    }
}