using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;

namespace MatheQuiz
{
    [Activity(Label = "EndScoreActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class EndScoreActivity : Activity
    {
        TextView FinalScore;
        Button GoBack;
        MediaPlayer BackgroundMusic;
        MediaPlayer ButtonClick;
        string score;
        int scoreInt;


        /*This activity contains the logic for the final Screen. It displays the final score and 
         allows to restart the game by clicking a button.*/

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);

            SetContentView(Resource.Layout.EndScore);
            FinalScore = FindViewById<TextView>(Resource.Id.finalScore);
            GoBack = FindViewById<Button>(Resource.Id.goBack);

            MediaPlayer BackgroundMusic = new MediaPlayer();
            BackgroundMusic = MediaPlayer.Create(this, Resource.Raw.BackgroundEnd);
            BackgroundMusic.Start();

            ButtonClick = MediaPlayer.Create(this, Resource.Raw.button_click1);

            score = Intent.GetStringExtra("score");
            scoreInt = Intent.GetIntExtra("scoreInt", 0);

            FinalScore.Text = score + " von 10 Punkten";


            //Jumps to MainActivity.
            GoBack.Click += delegate
            {
                ButtonClick.Start();
                StartActivity(typeof(MainActivity));
                BackgroundMusic.Stop();
            };
        }
    }
}