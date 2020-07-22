using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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
        Rectangle JumpRectPos;

        Texture2D RunningTexture;
        Rectangle RunningRect;
        Rectangle RunningRectPos;
        RunningFrames runningFrames;

        Texture2D CrouchTexture;
        Rectangle CrouchRect;
        Rectangle CrouchRectPos;
        CrouchingFrames crouchingFrames;
        int CFrameMargin;

        List<Texture2D> SmallCactusTextureList = new List<Texture2D>();
        List<Texture2D> BigCactusTextureList = new List<Texture2D>();
        Texture2D Cactus1Texture;
        Rectangle Cactuse1Rect;
        Rectangle Cactuse1PosRect;
        Texture2D Cactus2Texture;
        Rectangle Cactuse2Rect;
        Rectangle Cactuse2PosRect;
        Texture2D Cactus3Texture;
        Rectangle Cactuse3Rect;
        Rectangle Cactuse3PosRect;
        bool BigCactus;
        int[] SmallCactusIndexes;
        int[] BigCactusIndexes;

        Texture2D FloorTexture;
        Rectangle FloorRect;
        Rectangle FloorRectPos;
        Rectangle FloorRect1;
        Rectangle FloorRect1Pos;
        int FloorMargin;

        Texture2D[] NumbersTexturesArray = new Texture2D[10];
        Rectangle NumbersRect;
        Rectangle NumbersRectPos;
        Texture2D HITexture;
        Rectangle HIRect;
        Rectangle HIRectPos;

        Texture2D CloudTexture;
        Rectangle Cloud1Rect;
        Rectangle Cloud1RectPos;
        Rectangle Cloud2Rect;
        Rectangle Cloud2RectPos;
        Rectangle Cloud3Rect;
        Rectangle Cloud3RectPos;

        Texture2D DeadDinoTexture;
        Rectangle DeadDinoRect;
        Rectangle DeadDinoRectPos;

        Texture2D CurrentDinoTexture;
        Rectangle CurrentDinoRectPos;
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
        Rectangle DinoDebugRectPos;

        Texture2D CactusDebugTexture;
        Rectangle CactusDebugRect;
        Rectangle CactusDebugRectPos;

        Texture2D RestartTexture;
        Rectangle RestartRect;
        Rectangle RestartRectPos;

        int ScreenWidth;

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
                ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                CFrameMargin = RunningTexture.Height - CrouchTexture.Height;
                FloorMargin = (RunningTexture.Height - FloorTexture.Height) - 5;

                CactusColor = Color.Red;
                FloorColor = Color.White;
                DinoColor = Color.Green;
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

            for (int i = 0; i < 4; i++)
                SmallCactusTextureList.Add(Content.Load<Texture2D>(@"Cactuses/Small Cactuses/Cactus-" + i));


            for (int i = 0; i < 4; i++)
                BigCactusTextureList.Add(Content.Load<Texture2D>(@"Cactuses/Big Cactuses/BigCactus-" + i));
            InitCactus(1);
            InitCactus(2);
            InitCactus(3);

            HITexture = Content.Load<Texture2D>(@"Others /HI");
            CloudTexture = Content.Load<Texture2D>(@"Others/Cloud");
            FloorTexture = Content.Load<Texture2D>(@"Others/Floor1");
            RestartTexture = Content.Load<Texture2D>(@"Others/Restart");

            DinoDebugTexture = new Texture2D(GraphicsDevice, RunningTexture.Width, RunningTexture.Height);
            Color[] DinoColor = new Color[RunningTexture.Width * RunningTexture.Height];
            for (int i = 0; i < RunningTexture.Width * RunningTexture.Height; i++)
                DinoColor[i] = Color.Yellow;
            DinoDebugTexture.SetData(DinoColor);

            //CactusDebugTexture = new Texture2D(GraphicsDevice, Cactus1Texture.Width, Cactus1Texture.Height);
            //Color[] CactuseColor = new Color[Cactus1Texture.Width * Cactus1Texture.Height];
            //for (int i = 0; i < Cactus1Texture.Width * Cactus1Texture.Height; i++)
            //    CactuseColor[i] = Color.Green;
            //CactusDebugTexture.SetData(CactuseColor);
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
                CheckDinoCactusCollision(Cactuse1PosRect);
                CheckDinoCactusCollision(Cactuse2PosRect);
                CheckDinoCactusCollision(Cactuse3PosRect);

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
                DinoDebugRectPos = CurrentDinoRectPos;
                CactusDebugRectPos = Cactuse1PosRect;
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
                spriteBatch.Draw(DinoDebugTexture, DinoDebugRectPos, DinoDebugRect, Color.White);
                spriteBatch.Draw(CactusDebugTexture, CactusDebugRectPos, CactusDebugRect, Color.White);
            }

            //Drawing floor
            spriteBatch.Draw(FloorTexture, FloorRectPos, FloorRect, FloorColor);
            spriteBatch.Draw(FloorTexture, FloorRect1Pos, FloorRect1, FloorColor);

            //Drawing Cloud
            spriteBatch.Draw(CloudTexture, Cloud1RectPos, Cloud1Rect, Color.White);

            //Drawing Cactus
            spriteBatch.Draw(Cactus1Texture, Cactuse1PosRect, Cactuse1Rect, CactusColor);
            spriteBatch.Draw(Cactus2Texture, Cactuse2PosRect, Cactuse2Rect, CactusColor);
            spriteBatch.Draw(Cactus3Texture, Cactuse3PosRect, Cactuse3Rect, CactusColor);

            //Drawing Score
            DrawScore();

            //Drawing Messesge
           // spriteBatch.DrawString(font, "1: " + Cactuse1PosRect.X + ", 2: " + Cactuse2PosRect.X + ", 3: " + Cactuse3PosRect.X, new Vector2(0, 1), TextColor);

            if (!Pausegame)
            {
                //Drawing Dino Animations
                if (drawAction == DrawAction.Running)
                {
                    spriteBatch.Draw(RunningTexture, RunningRectPos, RunningRect, DinoColor);
                }
                else if (drawAction == DrawAction.Crouch)
                {
                    spriteBatch.Draw(CrouchTexture, CrouchRectPos, CrouchRect, DinoColor);
                }
                else if (drawAction == DrawAction.Idle)
                {
                    spriteBatch.Draw(JumpTexture, JumpRectPos, JumpRect, DinoColor);
                }
                else if (drawAction == DrawAction.Dead)
                {
                    spriteBatch.Draw(DeadDinoTexture, DeadDinoRectPos, DeadDinoRect, DinoColor);
                }
            }
            if (Pausegame)
            {

                spriteBatch.Draw(DeadDinoTexture, DeadDinoRectPos, DeadDinoRect, DinoColor);
                spriteBatch.Draw(RestartTexture, RestartRectPos, RestartRect, Color.White);
                spriteBatch.DrawString(font, "Press SPACE to Restart the game.", new Vector2(RestartRectPos.X - 145, RestartRectPos.Y + RestartTexture.Height + 10), Color.Gray);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void Jump(GameTime gameTime)
        {
            if (!Jumped)
            {
                if (((MainFormHeightByTwo) - JumpRectPos.Y) >= jumpHeight)
                {
                    Jumped = true;
                }
                else
                {
                    JumpRectPos.Y -= 10;
                    //RectIdlePos.X += 2;
                }
            }
            else
            {
                if (AirTime >= AirTimeTotal)
                {
                    if (JumpRectPos.Y == (MainFormHeightByTwo))
                    {
                        Jumping = false;
                        Jumped = false;
                        AirTime = 0;
                    }
                    else
                    {
                        JumpRectPos.Y += 5;
                        //RectIdlePos.X--;
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
            if (!(FloorRectPos.X + FloorRectPos.Width < 0))
            {
                FloorRectPos.X -= 10;
            }
            else
            {
                FloorRectPos.X = FloorRect1Pos.X + (FloorRect1Pos.Width - 13);
            }
            if (!(FloorRect1Pos.X + FloorRect1Pos.Width < 0))
            {
                FloorRect1Pos.X -= 10;
            }
            else
            {
                FloorRect1Pos.X = FloorRectPos.X + (FloorRectPos.Width - 11);
            }
            if (FloorRectPos.X <= MainForm.Width + 5)
            {
                FloorRect = new Rectangle(0, 0, FloorTexture.Width, FloorTexture.Height);
            }
            else
            {
                FloorRect = new Rectangle(FloorRect.Width, 0, FloorTexture.Width, FloorTexture.Height);
            }
            if (FloorRect1Pos.X <= MainForm.Width + 5)
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
            if (!(Cactuse1PosRect.X + Cactuse1Rect.Width <= -100))
            {
                Cactuse1PosRect.X -= 10;
            }
            else
            {
                InitCactus(1);
            }
            if (!(Cactuse2PosRect.X + Cactuse2Rect.Width <= -100))
            {
                Cactuse2PosRect.X -= 10;
            }
            else
            {
                InitCactus(2);
            }
            if (!(Cactuse3PosRect.X + Cactuse3Rect.Width <= -100))
            {
                Cactuse3PosRect.X -= 10;
            }
            else
            {
                InitCactus(3);
            }
        }

        private void RollTheCloud()
        {
            if (!(Cloud1RectPos.X + CloudTexture.Width < -10))
                Cloud1RectPos.X--;
            else
                Cloud1RectPos = new Rectangle(random.Next(MainForm.Width + CloudTexture.Width + 10, MainForm.Width + CloudTexture.Width + 50), random.Next(CloudTexture.Height + 10, FloorRectPos.Y - (CloudTexture.Height + 10)), Cloud1Rect.Width, Cloud1Rect.Height);
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

        private void CheckDinoCactusCollision(Rectangle CactusPosRect)
        {
            if (CurrentDinoRectPos.Intersects(CactusPosRect))
            {
                IntersecedRect = Rectangle.Intersect(CurrentDinoRectPos, CactusPosRect);
                bool IntersectedRegionDino;
                if (!IsRegionTransparent(CurrentDinoTexture, new Rectangle(0, 0, IntersecedRect.Width, IntersecedRect.Height)))
                {
                    IntersectedRegionDino = false;
                }
                else
                {
                    IntersectedRegionDino = true;
                }
                //bool IntersectedRegionCactus = IsRegionTransparent(CactusTexture, CactusPosRect);
                if (!IntersectedRegionDino)
                {
                    if (drawAction == DrawAction.Idle)
                    {
                        DeadDinoRectPos.X = JumpRectPos.X;
                        DeadDinoRectPos.Y = JumpRectPos.Y;
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
                CurrentDinoRectPos = CrouchRectPos;
                CurrentDinoTexture = CrouchTexture;
            }
            else if (drawAction == DrawAction.Running)
            {
                CurrentDinoRectPos = RunningRectPos;
                CurrentDinoTexture = RunningTexture;
            }
            else if (drawAction == DrawAction.Idle)
            {
                CurrentDinoRectPos = JumpRectPos;
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

        private Texture2D IntToTexture2d(int num)
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
                    spriteBatch.Draw(IntToTexture2d(digits[i]), NumbersRectPos, NumbersRect, Color.White);
                }
                else if (digits.Length == 2)
                {
                    if (i == 0)
                        NumbersRectPos.X = NumbersRectPos.X - NumbersTexturesArray[0].Width;
                    spriteBatch.Draw(IntToTexture2d(digits[i]), NumbersRectPos, NumbersRect, Color.White);
                }
                else if (digits.Length == 3)
                {
                    if (i == 0)
                        NumbersRectPos.X = NumbersRectPos.X - (NumbersTexturesArray[0].Width * 2);
                    if (i == 1)
                        NumbersRectPos.X = NumbersRectPos.X - NumbersTexturesArray[0].Width;
                    spriteBatch.Draw(IntToTexture2d(digits[i]), NumbersRectPos, NumbersRect, Color.White);
                }
                else if (digits.Length >= 4)
                {
                    if (i == 0)
                        NumbersRectPos.X = NumbersRectPos.X - (NumbersTexturesArray[0].Width * 3);
                    if (i == 1)
                        NumbersRectPos.X = NumbersRectPos.X - (NumbersTexturesArray[0].Width * 2);
                    if (i == 2)
                        NumbersRectPos.X = NumbersRectPos.X - NumbersTexturesArray[0].Width;
                    spriteBatch.Draw(IntToTexture2d(digits[i]), NumbersRectPos, NumbersRect, Color.White);
                }
                NumbersRectPos.X = HIRectPos.X - NumbersTexturesArray[0].Width;
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
            JumpRectPos = new Rectangle(0, MainFormHeightByTwo, JumpRect.Width, JumpRect.Height);

            RunningRect = new Rectangle(0, 0, RunningTexture.Width / 2, RunningTexture.Height);
            RunningRectPos = new Rectangle(0, MainFormHeightByTwo, RunningRect.Width, RunningRect.Height);

            CrouchRect = new Rectangle(0, 0, CrouchTexture.Width / 2, CrouchTexture.Height);
            CrouchRectPos = new Rectangle(0, MainFormHeightByTwo + CFrameMargin, CrouchRect.Width, CrouchRect.Height);

            FloorRect = new Rectangle(0, 0, FloorTexture.Width, FloorTexture.Height);
            FloorRectPos = new Rectangle(0, MainFormHeightByTwo + FloorMargin, FloorRect.Width, FloorRect.Height);

            FloorRect1 = new Rectangle(FloorRect.Width, 0, FloorTexture.Width, FloorTexture.Height);
            FloorRect1Pos = new Rectangle(FloorRectPos.Width, MainFormHeightByTwo + FloorMargin, FloorRect1.Width, FloorRect1.Height);

            HIRect = new Rectangle(0, 0, HITexture.Width, HITexture.Height);
            HIRectPos = new Rectangle(MainForm.Width - (HITexture.Width + 18), 0, HIRect.Width, HIRect.Height);

            NumbersRect = new Rectangle(0, 0, NumbersTexturesArray[0].Width, NumbersTexturesArray[0].Height);
            NumbersRectPos = new Rectangle(HIRectPos.X - NumbersTexturesArray[0].Width, 0, NumbersRect.Width, NumbersRect.Height);

            Cloud1Rect = new Rectangle(0, 0, CloudTexture.Width, CloudTexture.Height);
            Cloud1RectPos = new Rectangle(random.Next(MainForm.Width + CloudTexture.Width + 10, MainForm.Width + CloudTexture.Width + 50), random.Next(CloudTexture.Height + 10, FloorRectPos.Y - 10), Cloud1Rect.Width, Cloud1Rect.Height);

            DeadDinoRect = new Rectangle(0, 0, DeadDinoTexture.Width, DeadDinoTexture.Height);
            DeadDinoRectPos = new Rectangle(RunningRectPos.X, RunningRectPos.Y, DeadDinoRect.Width, DeadDinoRect.Height);

            DinoDebugRect = new Rectangle(0, 0, RunningRect.Width, RunningRect.Height);
            DinoDebugRectPos = new Rectangle(RunningRectPos.X, RunningRectPos.Y, DinoDebugRect.Width, DinoDebugRect.Height);

            Cactuse1Rect = new Rectangle(0, 0, Cactus1Texture.Width, Cactus1Texture.Height);
            Cactuse1PosRect = new Rectangle(ScreenWidth + Cactus1Texture.Width + 10, RunningRectPos.Y + 15, Cactuse1Rect.Width, Cactuse1Rect.Height);

            int Cac2X = random.Next(Cactuse1PosRect.X + 400, Cactuse1PosRect.X + 500);
            Cactuse2Rect = new Rectangle(0, 0, Cactus2Texture.Width, Cactus2Texture.Height);
            Cactuse2PosRect = new Rectangle(Cac2X, RunningRectPos.Y + 15, Cactuse2Rect.Width, Cactuse2Rect.Height);

            int Cac3X = random.Next(Cactuse2PosRect.X + 400, Cactuse2PosRect.X + 500);
            Cactuse3Rect = new Rectangle(0, 0, Cactus3Texture.Width, Cactus3Texture.Height);
            Cactuse3PosRect = new Rectangle(Cac3X, RunningRectPos.Y + 15, Cactuse3Rect.Width, Cactuse3Rect.Height);

            CactusDebugRect = new Rectangle(0, 0, Cactuse1Rect.Width, Cactuse1Rect.Height);
            CactusDebugRectPos = new Rectangle(Cactuse1PosRect.X, Cactuse1PosRect.Y, CactusDebugRect.Width, CactusDebugRect.Height);

            RestartRect = new Rectangle(0, 0, RestartTexture.Width, RestartTexture.Height);
            RestartRectPos = new Rectangle(MainForm.Width / 2 - RestartTexture.Width / 2, MainFormHeightByTwo - RestartTexture.Height / 2, RestartRect.Width, RestartRect.Height);
        }

        private void InitCactus(int cactus)
        {
            if (cactus == 1)
            {
                int Cac1Y = 0;
                if (IsBigCactus())
                {
                    Cactus1Texture = BigCactusTextureList[random.Next(0, BigCactusTextureList.Count)];
                    Cac1Y = RunningRectPos.Y + 15;
                }
                else
                {
                    Cactus1Texture = SmallCactusTextureList[random.Next(0, SmallCactusTextureList.Count)];
                    Cac1Y = RunningRectPos.Y + 15;
                }
                int Cac1X = GetRandomX(Cactus1Texture.Width);
                CheckIfTwoCactusesAreClose(1);
                Cactuse1Rect = new Rectangle(0, 0, Cactus1Texture.Width, Cactus1Texture.Height);
                Cactuse1PosRect = new Rectangle(Cac1X, Cac1Y, Cactuse1Rect.Width, Cactuse1Rect.Height);
            }
            else if (cactus == 2)
            {
                int Cac2Y = 0;
                if (IsBigCactus())
                {
                    Cactus2Texture = BigCactusTextureList[random.Next(0, BigCactusTextureList.Count)];
                    Cac2Y = RunningRectPos.Y + 15;
                }
                else
                {
                    Cactus2Texture = SmallCactusTextureList[random.Next(0, SmallCactusTextureList.Count)];
                    Cac2Y = RunningRectPos.Y + 15;
                }

                int Cac2X = GetRandomX(Cactus2Texture.Width);
                CheckIfTwoCactusesAreClose(2);
                Cactuse2Rect = new Rectangle(0, 0, Cactus2Texture.Width, Cactus2Texture.Height);
                Cactuse2PosRect = new Rectangle(Cac2X, Cac2Y, Cactuse2Rect.Width, Cactuse2Rect.Height);
            }
            else if (cactus == 3)
            {
                int Cac3Y = 0;
                if (IsBigCactus())
                {
                    Cactus3Texture = BigCactusTextureList[random.Next(0, BigCactusTextureList.Count)];
                    Cac3Y = RunningRectPos.Y + 15;
                }
                else
                {
                    Cactus3Texture = SmallCactusTextureList[random.Next(0, SmallCactusTextureList.Count)];
                    Cac3Y = RunningRectPos.Y + 15;
                }

                int Cac3X = GetRandomX(Cactus3Texture.Width);
                CheckIfTwoCactusesAreClose(3);
                Cactuse3Rect = new Rectangle(0, 0, Cactus3Texture.Width, Cactus3Texture.Height);
                Cactuse3PosRect = new Rectangle(Cac3X, Cac3Y, Cactuse3Rect.Width, Cactuse3Rect.Height);
            }
        }

        private bool IsBigCactus()
        {
            try
            {
                random.Next();
            }
            catch (NullReferenceException e)
            {
                random = new Random();
            }
            if (random.Next(0, 2) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int GetRandomX(int textureWidth)
        {
            return random.Next(ScreenWidth + textureWidth + 10, ScreenWidth + textureWidth + 300);
        }

        private void CheckIfTwoCactusesAreClose(int cactusNo)
        {
            if (cactusNo == 1)
            {
                if (Cactuse1PosRect.X - Cactuse3PosRect.X <= 300)
                {
                    Cactuse1PosRect.X = Cactuse1PosRect.X + random.Next(200, 400);
                }
            }
            if (cactusNo == 2)
            {
                if (Cactuse2PosRect.X - Cactuse1PosRect.X <= 300)
                {
                    Cactuse2PosRect.X = Cactuse2PosRect.X + random.Next(200, 400);
                }
            }
            if (cactusNo == 3)
            {
                if (Cactuse3PosRect.X - Cactuse2PosRect.X <= 300)
                {
                    Cactuse3PosRect.X = Cactuse3PosRect.X + random.Next(200, 400);
                }
            }
        }
    }
}