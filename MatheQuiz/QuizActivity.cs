using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;

namespace MatheQuiz
{
    //This Activity contains the logic for the quiz.

    [Activity(Label = "QuizActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    //"ConigurationChanges = ..." prevents the QuizActivity from restarting after the orientation of the device was changend.
    public class QuizActivity : Activity
    {
        TextView ViewPoints;
        TextView ShowQuestion;
        TextView myTimer;
        TextView QuestionNumber;
        Timer theTimer = new Timer();
        private Button[] buttonAnswer;
        MediaPlayer ButtonClick;
        MediaPlayer AnswerWrong;
        MediaPlayer AnswerRight;
        string Correct;
        int ButtonIndex;
        int AnswerIndex;
        int count = 180; //Sets the duration of the timer
        int points;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle); //Hides the title of this Activity.

            // Set our view from the "main" layout resource.
            SetContentView(Resource.Layout.Main);

            ViewPoints = FindViewById<TextView>(Resource.Id.view_points); //Displays the gained Points.
            ShowQuestion = FindViewById<TextView>(Resource.Id.question_text); //Displays the Question.
            QuestionNumber = FindViewById<TextView>(Resource.Id.questionNumber); //Displays the number of the Question.
            buttonAnswer = new Button[4]; //Array of Buttons, displaying the possible answers.
            buttonAnswer[0] = FindViewById<Button>(Resource.Id.answer1);
            buttonAnswer[1] = FindViewById<Button>(Resource.Id.answer2);
            buttonAnswer[2] = FindViewById<Button>(Resource.Id.answer3);
            buttonAnswer[3] = FindViewById<Button>(Resource.Id.answer4);
            myTimer = FindViewById<TextView>(Resource.Id.timer); ////Displays the timer.

            buttonAnswer[0].Click += showButtonIndex0;
            buttonAnswer[0].Click += AddPointChangeColor;

            buttonAnswer[1].Click += showButtonIndex1;
            buttonAnswer[1].Click += AddPointChangeColor;

            buttonAnswer[2].Click += showButtonIndex2;
            buttonAnswer[2].Click += AddPointChangeColor;

            buttonAnswer[3].Click += showButtonIndex3;
            buttonAnswer[3].Click += AddPointChangeColor;

            ButtonClick = MediaPlayer.Create(this, Resource.Raw.button_click1);
            AnswerWrong = MediaPlayer.Create(this, Resource.Raw.AnswerWrong);
            AnswerRight = MediaPlayer.Create(this, Resource.Raw.AnswerRight);

            LoadViews(); //Loads the views

            //Initiates the timer
            theTimer.Interval = 1000;
            theTimer.Elapsed += TimerElapsed;
            theTimer.Start();
        }


        /*This method creates objects of the class QuestitionsAndAnswers and loads their content
        into the associated views. The possible answers are distributed to the buttons randomly.*/
        private void LoadViews()
        {
            QuestionsAndAnswers[] qas = new QuestionsAndAnswers[10];

            qas[0] = new QuestionsAndAnswers("Was ergibt 2+4?",
                new string[] { "4", "7", "6", "24" }, "6");

            qas[1] = new QuestionsAndAnswers("Was ergibt 7-9?",
                new string[] { "2", "16", "-2", "63" }, "-2");

            qas[2] = new QuestionsAndAnswers("Was ergibt 2*8?",
                new string[] { "11", "16", "-2", "63" }, "16");

            qas[3] = new QuestionsAndAnswers("Was ergibt 3 + 120/24?",
                new string[] { "27", "8", "13", "123" }, "8");

            qas[4] = new QuestionsAndAnswers("Was ergibt 11/8 + 10/16?",
                new string[] { "16", "1/2", "2", "21/8" }, "2");

            qas[5] = new QuestionsAndAnswers("Was ist die 3. Wurzel aus 27?",
            new string[] { "4", "9", "6", "3" }, "3");

            qas[6] = new QuestionsAndAnswers("Was ergibt 24 : 1/5?",
            new string[] { "120", "240", "5/24", "24/5" }, "120");

            qas[7] = new QuestionsAndAnswers("Was sind die Nullstellen von f(x)=(x-2)*(x+3)?",
            new string[] { "x1=2, x2=-3", "x1=-2, x2=3", "x1=2, x2=3", "x1=-2, x2=-3" }, "x1=2, x2=-3");

            qas[8] = new QuestionsAndAnswers("Wie lässt sich log_a(x) noch ausdrücken?",
            new string[] { "a*e^x", "ln(x)/ln(a)", "e^(a*ln(x))", "sqrt(a)*e^x" }, "ln(x)/ln(a)");

            qas[9] = new QuestionsAndAnswers("Was ist die erste Ableitung von f(x)=e^(-x^2+2)?",
            new string[] { "-2*x^2*e^(-2x+2)", "e^(-2x+2)", "-2*x*e^(-x^2+2)", "-e^(x^2+2)" }, "-2*x*e^(-x^2+2)");

            //Random distribution of answers via List<T>- and Random-Class.

            var rndNumbers = new int[4];
            Random random = new Random();

            List<int> rndList = new List<int>(4);
            for (int i = 0; i < 4; i++) rndList.Add(i);

            for (int i = 0; i < 4; i++)
            {
                int index = random.Next(0, rndList.Count);
                rndNumbers[i] = rndList[index];
                rndList.RemoveAt(index);
            }
            
            ShowQuestion.Text = qas[AnswerIndex].getQuestionText;
            buttonAnswer[rndNumbers[0]].Text = qas[AnswerIndex].getAnswer1Text;
            buttonAnswer[rndNumbers[1]].Text = qas[AnswerIndex].getAnswer2Text;
            buttonAnswer[rndNumbers[2]].Text = qas[AnswerIndex].getAnswer3Text;
            buttonAnswer[rndNumbers[3]].Text = qas[AnswerIndex].getAnswer4Text;
            Correct = qas[AnswerIndex].getCorrectAnswer;
        }

        //Defines the properties of the timer.
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //Instructs the timer to count down from the start value and displays the timer on UI.
            if (count > 0)
            {
                count--;
                RunOnUiThread(() => { myTimer.Text = "Verbleibende Zeit: " + count + "s"; });
            }

            //Stops the game when time runs out and jumps to EndScoreActivity.
            else
            {
                    Intent nextActivity = new Intent(this, typeof(EndScoreActivity));
                    nextActivity.PutExtra("score", "Zeit abgelaufen!\n" + points.ToString());
                    StartActivity(nextActivity);
                    points = 0;
                    theTimer.Stop();
            }
        }


        //This methods set the index of the button clicked.
        private void showButtonIndex0(object sender, EventArgs e)
        {
            ButtonIndex = 0;
        }

        private void showButtonIndex1(object sender, EventArgs e)
        {
            ButtonIndex = 1;
        }

        private void showButtonIndex2(object sender, EventArgs e)
        {
            ButtonIndex = 2;
        }

        private void showButtonIndex3(object sender, EventArgs e)
        {
            ButtonIndex = 3;
        }
        
        /*This method changes the color of the selected button first to blue and then checks if 
         the answer is correct. If so, the color is changed to green and a point is added to
         the score, otherwise color is changed to red and no point is added. (After
         a button was clicked, all buttons are disabled until the next question is loaded.)
         Then the method jumps to the next question and loads the views with new content. If 
         all questions are answered, the method stops the game and jumps to EndScoreActivity.*/

        async private void AddPointChangeColor(object sender, EventArgs e)
        {
            ButtonClick.Start();
            if (buttonAnswer[ButtonIndex].Text == Correct)
            {
                buttonAnswer[ButtonIndex].SetBackgroundColor(Android.Graphics.Color.Blue);
                buttonAnswer[0].Enabled = false;
                buttonAnswer[1].Enabled = false;
                buttonAnswer[2].Enabled = false;
                buttonAnswer[3].Enabled = false;
                await Task.Delay(500);
                AnswerRight.Start();
                buttonAnswer[ButtonIndex].SetBackgroundColor(Android.Graphics.Color.Green);
                ViewPoints.Text = (++points).ToString();
                await Task.Delay(500);
            }
            else
            {
                buttonAnswer[ButtonIndex].SetBackgroundColor(Android.Graphics.Color.Blue);
                buttonAnswer[0].Enabled = false;
                buttonAnswer[1].Enabled = false;
                buttonAnswer[2].Enabled = false;
                buttonAnswer[3].Enabled = false;
                await Task.Delay(500);
                AnswerWrong.Start();
                buttonAnswer[ButtonIndex].SetBackgroundColor(Android.Graphics.Color.Red);
                await Task.Delay(500);
            }


            await Task.Delay(1000);
            ShowQuestion.SetBackgroundColor((Android.Graphics.Color.Azure));
            await Task.Delay(600);
            ShowQuestion.SetBackgroundColor((Android.Graphics.Color.PaleVioletRed));

            AnswerIndex++;

            if (AnswerIndex == 10)
            {
                Intent nextActivity = new Intent(this, typeof(EndScoreActivity));
                nextActivity.PutExtra("score", "Alle Fragen beantwortet!\n" + points.ToString());
                nextActivity.PutExtra("scoreInt", points);
                StartActivity(nextActivity);
                points = 0;
                theTimer.Stop();
            }

            else
            {
                LoadViews();

                buttonAnswer[0].Enabled = true;
                buttonAnswer[1].Enabled = true;
                buttonAnswer[2].Enabled = true;
                buttonAnswer[3].Enabled = true;

                buttonAnswer[ButtonIndex].SetBackgroundColor(Android.Graphics.Color.AntiqueWhite);
            }
            if (AnswerIndex < 10) QuestionNumber.Text = "Frage " + (AnswerIndex + 1) + " von 10";
            else QuestionNumber.Text = "Frage 10 von 10";
        }
    }

    /*Class of Questions and Answers. Contains attributes, constructors and get-methods 
     for creating questions and answers and to pass the data to other variables.*/
    public class QuestionsAndAnswers
    {
        private string questionText;
        private string[] Answers;
        private string CorrectAnswer;

        public QuestionsAndAnswers(string qText, string[] aText, string cAnswer)
        {
            questionText = qText;
            Answers = aText;
            CorrectAnswer = cAnswer;
        }

        public string getQuestionText
        {
            get { return questionText; }
        }

        public string getAnswer1Text
        {
            get { return Answers[0]; }
        }

        public string getAnswer2Text
        {
            get { return Answers[1]; }
        }

        public string getAnswer3Text
        {
            get { return Answers[2]; }
        }
        public string getAnswer4Text
        {
            get { return Answers[3]; }
        }

        public string getCorrectAnswer
        {
            get { return CorrectAnswer; }
        }
    }


}

