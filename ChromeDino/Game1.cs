using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ChromeDino
{
    enum DrawAction { Idle, Running, Crouch, Dead };
    enum RunningFrames { FirstFrame, LastFrame };
    enum CrouchingFrames { FirstFrame, LastFrame };
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Form MainForm;
        double DinoFrameDrawtime = 0;
        double FloorFrameDrawtime = 0;
        int score;
        DrawAction drawAction;

        Texture2D JumpTexture;
        Rectangle JumpRect;
        Rectangle JumpRectDest;

        Texture2D RunningTexture;
        Rectangle RunningRect;
        Rectangle RunningRectDest;
        RunningFrames runningFrames;

        Texture2D CrouchTexture;
        Rectangle CrouchRect;
        Rectangle CrouchRectDest;
        CrouchingFrames crouchingFrames;
        int CFrameMargin;

        Texture2D CactusTexture;
        Rectangle Cactuse1Rect;
        Rectangle Cactuse1DestRect;
        Rectangle Cactuse2Rect;
        Rectangle Cactuse2RectDest;
        Rectangle Cactuse3Rect;
        Rectangle Cactuse3RectDest;

        Texture2D FloorTexture;
        Rectangle FloorRect;
        Rectangle FloorRectDest;
        Rectangle FloorRect1;
        Rectangle FloorRect1Dest;
        int FloorMargin;

        Texture2D[] NumbersTexturesArray = new Texture2D[10];
        Texture2D HITexture;
        Rectangle NumbersRect;
        Rectangle NumbersRectDest;
        Rectangle HIRect;
        Rectangle HIRectDest;

        Texture2D CloudTexture;
        Rectangle Cloud1Rect;
        Rectangle Cloud1RectDest;
        Rectangle Cloud2Rect;
        Rectangle Cloud2RectDest;
        Rectangle Cloud3Rect;
        Rectangle Cloud3RectDest;

        Texture2D DeadDinoTexture;
        Rectangle DeadDinoRect;
        Rectangle DeadDinoRectDest;

        Texture2D CurrentDinoTexture;
        Rectangle CurrentDinoRectDest;
        Rectangle IntersecedRect;

        bool Jumping = false;
        bool Jumped = false;
        int jumpHeight = 120;
        double AirTime = 0;
        int AirTimeTotal = 45;

        Color DinoColor;
        Color FloorColor;
        Color CactusColor;
        Color BackColor;
        Color TextColor;
        bool LightColorMode = true;
        double DayNightCycleTime;
        int MainFormHeightByTwo;

        string messege = "";
        int CollisionCount = 0;
        bool DebugTextures = false;
        Random random;

        Texture2D DinoDebugTexture;
        Rectangle DinoDebugRect;
        Rectangle DinoDebugRectDest;

        Texture2D CactusDebugTexture;
        Rectangle CactusDebugRect;
        Rectangle CactusDebugRectDest;

        Texture2D RestartTexture;
        Rectangle RestartRect;
        Rectangle RestartRectDest;

        bool Pausegame = false;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "T Rex Runner";
            IsMouseVisible = false;
            MainForm = (Form)Control.FromHandle(Window.Handle);

            score = 0;
            drawAction = DrawAction.Crouch;
            runningFrames = RunningFrames.FirstFrame;
            crouchingFrames = CrouchingFrames.FirstFrame;
        }
        protected override void Initialize()
        {
            base.Initialize();
            {
                MainFormHeightByTwo = MainForm.Height / 2;
                MainForm.StartPosition = FormStartPosition.Manual;
                MainForm.Location = new System.Drawing.Point(0, (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) - (MainFormHeightByTwo));
                MainForm.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                CFrameMargin = RunningTexture.Height - CrouchTexture.Height;
                FloorMargin = (RunningTexture.Height - FloorTexture.Height) - 5;
                CactusColor = FloorColor = DinoColor = Color.White;
                BackColor = Color.White;
                TextColor = Color.Black;

                random = new Random();
                InitAllRecangles();

            }
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>(@"Fonts/fonts");
            JumpTexture = Content.Load<Texture2D>(@"Dino/Idle");
            RunningTexture = Content.Load<Texture2D>(@"Dino/Running");
            CrouchTexture = Content.Load<Texture2D>(@"Dino/Crouching");
            DeadDinoTexture = Content.Load<Texture2D>(@"Dino/DeadDino");

            for (int i = 0; i < 10; i++)
                NumbersTexturesArray[i] = Content.Load<Texture2D>(@"Numbers/Number-" + i);

            CactusTexture = Content.Load<Texture2D>(@"Cactuses/Small Cactuses/Cactus-0");

            FloorTexture = Content.Load<Texture2D>(@"Others/Floor1");
            HITexture = Content.Load<Texture2D>(@"Others/HI");
            CloudTexture = Content.Load<Texture2D>(@"Others/Cloud");
            RestartTexture = Content.Load<Texture2D>(@"Others/Restart");

            DinoDebugTexture = new Texture2D(GraphicsDevice, RunningTexture.Width, RunningTexture.Height);
            Color[] DinoColor = new Color[RunningTexture.Width * RunningTexture.Height];
            for (int i = 0; i < RunningTexture.Width * RunningTexture.Height; i++)
                DinoColor[i] = Color.Yellow;
            DinoDebugTexture.SetData(DinoColor);

            CactusDebugTexture = new Texture2D(GraphicsDevice, CactusTexture.Width, CactusTexture.Height);
            Color[] CactuseColor = new Color[CactusTexture.Width * CactusTexture.Height];
            for (int i = 0; i < CactusTexture.Width * CactusTexture.Height; i++)
                CactuseColor[i] = Color.Green;
            CactusDebugTexture.SetData(CactuseColor);
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Escape))
                Exit();
            // CycleDayAndNight(gameTime);
            InitCurrentDino();
            if (!Pausegame)
            {
                CheckDinoCactusCollision(Cactuse1DestRect);

                //userInput
                if (!Jumping)
                {
                    CheckUserInput(keyState);
                }
                //JumpLogic
                if (Jumping)
                {
                    Jump(gameTime);
                }

                //Floor and Cactus Logic
                FloorFrameDrawtime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (FloorFrameDrawtime >= 15)
                {
                    //Floor
                    RollTheFloor();

                    //Cactuse
                    RollTheCactuses();

                    //Cloud
                    RollTheCloud();

                    FloorFrameDrawtime = 0;
                }

                //Dino Logic
                DinoFrameDrawtime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (DinoFrameDrawtime >= 85)
                {
                    score++;
                    if (drawAction == DrawAction.Running)
                    {
                        ChangeRunningFrames();
                    }
                    if (drawAction == DrawAction.Crouch)
                    {
                        ChangeChrouchFrames();
                    }
                    DinoFrameDrawtime = 0;
                }
                DinoDebugRectDest = CurrentDinoRectDest;
                CactusDebugRectDest = Cactuse1DestRect;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    InitAllRecangles();
                    Pausegame = false;
                }
            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackColor);
            spriteBatch.Begin();

            if (DebugTextures)
            {
                spriteBatch.Draw(DinoDebugTexture, DinoDebugRectDest, DinoDebugRect, Color.White);
                spriteBatch.Draw(CactusDebugTexture, CactusDebugRectDest, CactusDebugRect, Color.White);
            }

            //Drawing floor
            spriteBatch.Draw(FloorTexture, FloorRectDest, FloorRect, FloorColor);
            spriteBatch.Draw(FloorTexture, FloorRect1Dest, FloorRect1, FloorColor);

            //Drawing Cloud
            spriteBatch.Draw(CloudTexture, Cloud1RectDest, Cloud1Rect, Color.White);

            //Drawing Cactus
            spriteBatch.Draw(CactusTexture, Cactuse1DestRect, Cactuse1Rect, CactusColor);

            //Drawing Score
            // spriteBatch.Draw(HITexture, HIRectDest, HIRect, Color.White);
            DrawScore();

            //Drawing Messesge
            //spriteBatch.DrawString(font, messege.ToString() + "Paused: " + Pausegame, Vector2.One, TextColor);

            if (!Pausegame)
            {
                //Drawing Dino Animations
                if (drawAction == DrawAction.Running)
                {
                    spriteBatch.Draw(RunningTexture, RunningRectDest, RunningRect, DinoColor);
                }
                else if (drawAction == DrawAction.Crouch)
                {
                    spriteBatch.Draw(CrouchTexture, CrouchRectDest, CrouchRect, DinoColor);
                }
                else if (drawAction == DrawAction.Idle)
                {
                    spriteBatch.Draw(JumpTexture, JumpRectDest, JumpRect, DinoColor);
                }
                else if (drawAction == DrawAction.Dead)
                {
                    spriteBatch.Draw(DeadDinoTexture, DeadDinoRectDest, DeadDinoRect, Color.White);
                }

            }

            if (Pausegame)
            {
                
                spriteBatch.Draw(DeadDinoTexture, DeadDinoRectDest, DeadDinoRect, Color.White);
                spriteBatch.Draw(RestartTexture, RestartRectDest, RestartRect, Color.White);
                spriteBatch.DrawString(font, "Press SPACE to Restart the game.", new Vector2(RestartRectDest.X - 145, RestartRectDest.Y + RestartTexture.Height + 10), Color.Gray);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void Jump(GameTime gameTime)
        {
            if (!Jumped)
            {
                if (((MainFormHeightByTwo) - JumpRectDest.Y) >= jumpHeight)
                {
                    Jumped = true;
                }
                else
                {
                    JumpRectDest.Y -= 10;
                    //RectIdleDest.X += 2;
                }
            }
            else
            {
                if (AirTime >= AirTimeTotal)
                {
                    if (JumpRectDest.Y == (MainFormHeightByTwo))
                    {
                        Jumping = false;
                        Jumped = false;
                        AirTime = 0;
                    }
                    else
                    {
                        JumpRectDest.Y += 5;
                        //RectIdleDest.X--;
                    }
                }
                else
                {
                    AirTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
        }

        private void CheckUserInput(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Enter))
                score += 1000;
            else if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
                drawAction = DrawAction.Crouch;
            else if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
            {
                Jumping = true;
                drawAction = DrawAction.Idle;
            }
            else if (keyState.IsKeyDown(Keys.D))
            {
                drawAction = DrawAction.Dead;
            }
            else
                drawAction = DrawAction.Running;
        }

        private void RollTheFloor()
        {
            if (!(FloorRectDest.X + FloorRectDest.Width < 0))
            {
                FloorRectDest.X -= 10;
            }
            else
            {
                FloorRectDest.X = FloorRect1Dest.X + (FloorRect1Dest.Width - 13);
            }
            if (!(FloorRect1Dest.X + FloorRect1Dest.Width < 0))
            {
                FloorRect1Dest.X -= 10;
            }
            else
            {
                FloorRect1Dest.X = FloorRectDest.X + (FloorRectDest.Width - 11);
            }
            if (FloorRectDest.X <= MainForm.Width + 5)
            {
                FloorRect = new Rectangle(0, 0, FloorTexture.Width, FloorTexture.Height);
            }
            else
            {
                FloorRect = new Rectangle(FloorRect.Width, 0, FloorTexture.Width, FloorTexture.Height);
            }
            if (FloorRect1Dest.X <= MainForm.Width + 5)
            {
                FloorRect1 = new Rectangle(0, 0, FloorTexture.Width, FloorTexture.Height);
            }
            else
            {
                FloorRect1 = new Rectangle(FloorRect.Width, 0, FloorTexture.Width, FloorTexture.Height);
            }
        }

        private void RollTheCactuses()
        {
            if (!(Cactuse1DestRect.X + Cactuse1Rect.Width <= -5))
            {
                Cactuse1DestRect.X -= 10;
            }
            else
            {
                Cactuse1DestRect.X = MainForm.Width + 10;
            }
        }

        private void RollTheCloud()
        {
            if (!(Cloud1RectDest.X + CloudTexture.Width < -10))
                Cloud1RectDest.X--;
            else
                Cloud1RectDest = new Rectangle(random.Next(MainForm.Width + CloudTexture.Width + 10, MainForm.Width + CloudTexture.Width + 50), random.Next(CloudTexture.Height + 10, FloorRectDest.Y - (CloudTexture.Height + 10)), Cloud1Rect.Width, Cloud1Rect.Height);
        }

        private void ChangeRunningFrames()
        {

            if (runningFrames == RunningFrames.FirstFrame)
            {
                runningFrames = RunningFrames.LastFrame;
                RunningRect = new Rectangle(0, 0, RunningTexture.Width / 2, RunningTexture.Height);
            }
            else
            {
                runningFrames = RunningFrames.FirstFrame;
                RunningRect = new Rectangle(RunningTexture.Width / 2, 0, RunningTexture.Width / 2, RunningTexture.Height);
            }

        }

        private void ChangeChrouchFrames()
        {
            if (crouchingFrames == CrouchingFrames.FirstFrame)
            {
                crouchingFrames = CrouchingFrames.LastFrame;
                CrouchRect = new Rectangle(0, 0, CrouchTexture.Width / 2, CrouchTexture.Height);
            }
            else
            {
                crouchingFrames = CrouchingFrames.FirstFrame;
                CrouchRect = new Rectangle(CrouchTexture.Width / 2, 0, CrouchTexture.Width / 2, CrouchTexture.Height);
            }

        }

        private void CycleDayAndNight(GameTime gameTime)
        {
            DayNightCycleTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (DayNightCycleTime >= 5)
            {
                if (LightColorMode)
                {
                    BackColor = Color.Black;
                    LightColorMode = false;
                    //CactusColor = Color.LightSeaGreen;
                    //FloorColor = Color.LightGoldenrodYellow;
                    //DinoColor = Color.Red;
                    TextColor = Color.White;
                }
                else
                {
                    BackColor = CactusColor = FloorColor = DinoColor = Color.White;
                    TextColor = Color.Black;
                    LightColorMode = true;
                }
                DayNightCycleTime = 0;
            }
        }

        private void CheckDinoCactusCollision(Rectangle CactusDestRect)
        {
            if (CurrentDinoRectDest.Intersects(CactusDestRect))
            {
                IntersecedRect = Rectangle.Intersect(CurrentDinoRectDest, CactusDestRect);
                bool IntersectedRegionDino;
                if (!IsRegionTransparent(CurrentDinoTexture, new Rectangle(0, 0, IntersecedRect.Width, IntersecedRect.Height)))
                {
                    IntersectedRegionDino = false;
                }
                else
                {
                    IntersectedRegionDino = true;
                }
                //bool IntersectedRegionCactus = IsRegionTransparent(CactusTexture, CactusDestRect);
                if (!IntersectedRegionDino)
                {
                    if(drawAction == DrawAction.Idle)
                    {
                        DeadDinoRectDest.X = JumpRectDest.X;
                        DeadDinoRectDest.Y = JumpRectDest.Y;
                    }
                    Pausegame = true;
                    drawAction = DrawAction.Dead;
                    GameOver();
                }
            }
        }

        private bool IsRegionTransparent(Texture2D texture, Rectangle r)
        {
            int size = r.Width * r.Height;
            Color[] buffer = new Color[size];
            texture.GetData(0, r, buffer, 0, size);
            return buffer.All(c => c == Color.Transparent);
        }

        private void InitCurrentDino()
        {
            if (drawAction == DrawAction.Crouch)
            {
                CurrentDinoRectDest = CrouchRectDest;
                CurrentDinoTexture = CrouchTexture;
            }
            else if (drawAction == DrawAction.Running)
            {
                CurrentDinoRectDest = RunningRectDest;
                CurrentDinoTexture = RunningTexture;
            }
            else if (drawAction == DrawAction.Idle)
            {
                CurrentDinoRectDest = JumpRectDest;
                CurrentDinoTexture = JumpTexture;
            }
        }

        private int[] GetDigits(int number)
        {
            string str = Convert.ToString(number);
            char[] charArr = str.ToCharArray();
            int[] FinalArr = new int[charArr.Length];
            for (int i = 0; i < charArr.Length; i++)
            {
                FinalArr[i] = int.Parse(charArr[i].ToString());
            }
            return FinalArr;
        }

        private Texture2D IntTotexture2d(int num)
        {
            return NumbersTexturesArray[num];
        }

        private void DrawScore()
        {
            int[] digits = GetDigits(score);
            for (int i = 0; i < digits.Length; i++)
            {
                if (digits.Length == 1)
                {
                    spriteBatch.Draw(IntTotexture2d(digits[i]), NumbersRectDest, NumbersRect, Color.White);
                }
                else if (digits.Length == 2)
                {
                    if (i == 0)
                        NumbersRectDest.X = NumbersRectDest.X - NumbersTexturesArray[0].Width;
                    spriteBatch.Draw(IntTotexture2d(digits[i]), NumbersRectDest, NumbersRect, Color.White);
                }
                else if (digits.Length == 3)
                {
                    if (i == 0)
                        NumbersRectDest.X = NumbersRectDest.X - (NumbersTexturesArray[0].Width * 2);
                    if (i == 1)
                        NumbersRectDest.X = NumbersRectDest.X - NumbersTexturesArray[0].Width;
                    spriteBatch.Draw(IntTotexture2d(digits[i]), NumbersRectDest, NumbersRect, Color.White);
                }
                else if (digits.Length >= 4)
                {
                    if (i == 0)
                        NumbersRectDest.X = NumbersRectDest.X - (NumbersTexturesArray[0].Width * 3);
                    if (i == 1)
                        NumbersRectDest.X = NumbersRectDest.X - (NumbersTexturesArray[0].Width * 2);
                    if (i == 2)
                        NumbersRectDest.X = NumbersRectDest.X - NumbersTexturesArray[0].Width;
                    spriteBatch.Draw(IntTotexture2d(digits[i]), NumbersRectDest, NumbersRect, Color.White);
                }
                NumbersRectDest.X = HIRectDest.X - NumbersTexturesArray[0].Width;
            }
        }

        private void GameOver()
        {
            Pausegame = true;
            score = 0;

        }

        private void InitAllRecangles()
        {

            JumpRect = new Rectangle(0, 0, JumpTexture.Width / 2, JumpTexture.Height);
            JumpRectDest = new Rectangle(0, MainFormHeightByTwo, JumpRect.Width, JumpRect.Height);

            RunningRect = new Rectangle(0, 0, RunningTexture.Width / 2, RunningTexture.Height);
            RunningRectDest = new Rectangle(0, MainFormHeightByTwo, RunningRect.Width, RunningRect.Height);

            CrouchRect = new Rectangle(0, 0, CrouchTexture.Width / 2, CrouchTexture.Height);
            CrouchRectDest = new Rectangle(0, MainFormHeightByTwo + CFrameMargin, CrouchRect.Width, CrouchRect.Height);

            FloorRect = new Rectangle(0, 0, FloorTexture.Width, FloorTexture.Height);
            FloorRectDest = new Rectangle(0, MainFormHeightByTwo + FloorMargin, FloorRect.Width, FloorRect.Height);

            FloorRect1 = new Rectangle(FloorRect.Width, 0, FloorTexture.Width, FloorTexture.Height);
            FloorRect1Dest = new Rectangle(FloorRectDest.Width, MainFormHeightByTwo + FloorMargin, FloorRect1.Width, FloorRect1.Height);

            Cactuse1Rect = new Rectangle(0, 0, CactusTexture.Width, CactusTexture.Height);
            Cactuse1DestRect = new Rectangle(1000, RunningRectDest.Y + 15, Cactuse1Rect.Width, Cactuse1Rect.Height);

            Cactuse2Rect = new Rectangle(0, 0, CactusTexture.Width, CactusTexture.Height);
            Cactuse2RectDest = new Rectangle(200, RunningRectDest.Y + 15, Cactuse2Rect.Width, Cactuse2Rect.Height);

            Cactuse3Rect = new Rectangle(0, 0, CactusTexture.Width, CactusTexture.Height);
            Cactuse3RectDest = new Rectangle(200, RunningRectDest.Y + 15, Cactuse3Rect.Width, Cactuse3Rect.Height);

            HIRect = new Rectangle(0, 0, HITexture.Width, HITexture.Height);
            HIRectDest = new Rectangle(MainForm.Width - (HITexture.Width + 18), 0, HIRect.Width, HIRect.Height);

            NumbersRect = new Rectangle(0, 0, NumbersTexturesArray[0].Width, NumbersTexturesArray[0].Height);
            NumbersRectDest = new Rectangle(HIRectDest.X - NumbersTexturesArray[0].Width, 0, NumbersRect.Width, NumbersRect.Height);

            Cloud1Rect = new Rectangle(0, 0, CloudTexture.Width, CloudTexture.Height);
            Cloud1RectDest = new Rectangle(random.Next(MainForm.Width + CloudTexture.Width + 10, MainForm.Width + CloudTexture.Width + 50), random.Next(CloudTexture.Height + 10, FloorRectDest.Y - 10), Cloud1Rect.Width, Cloud1Rect.Height);

            DeadDinoRect = new Rectangle(0, 0, DeadDinoTexture.Width, DeadDinoTexture.Height);
            DeadDinoRectDest = new Rectangle(RunningRectDest.X, RunningRectDest.Y, DeadDinoRect.Width, DeadDinoRect.Height);

            DinoDebugRect = new Rectangle(0, 0, RunningRect.Width, RunningRect.Height);
            DinoDebugRectDest = new Rectangle(RunningRectDest.X, RunningRectDest.Y, DinoDebugRect.Width, DinoDebugRect.Height);

            CactusDebugRect = new Rectangle(0, 0, Cactuse1Rect.Width, Cactuse1Rect.Height);
            CactusDebugRectDest = new Rectangle(Cactuse1DestRect.X, Cactuse1DestRect.Y, CactusDebugRect.Width, CactusDebugRect.Height);

            RestartRect = new Rectangle(0, 0, RestartTexture.Width, RestartTexture.Height);
            RestartRectDest = new Rectangle(MainForm.Width / 2 - RestartTexture.Width / 2, MainFormHeightByTwo - RestartTexture.Height / 2, RestartRect.Width, RestartRect.Height);
        }
    }
}