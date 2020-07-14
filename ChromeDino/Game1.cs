using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ChromeDino
{
    enum DrawAction { Idle, Running, Crouch };
    enum IdleFrames { FirstFrame, LastFrame };
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

        Texture2D IdleTexture;
        Rectangle IdleRect;
        Rectangle IdleRectDest;
        //IdleFrames idleFrame;

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

        string messege = "";
        int CollisionCount = 0;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "T Rex Runner";
            IsMouseVisible = false;
            MainForm = (Form)Form.FromHandle(Window.Handle);
            //Window.AllowUserResizing = true;
            score = 0;
            drawAction = DrawAction.Crouch;
            //idleFrame = IdleFrames.FirstFrame;
            runningFrames = RunningFrames.FirstFrame;
            crouchingFrames = CrouchingFrames.FirstFrame;
        }
        protected override void Initialize()
        {
            base.Initialize();
            {
                CFrameMargin = RunningTexture.Height - CrouchTexture.Height;
                FloorMargin = (RunningTexture.Height - FloorTexture.Height) - 5;
                CactusColor = FloorColor = DinoColor = Color.White;
                BackColor = Color.White;
                TextColor = Color.Black;

                IdleRect = new Rectangle(0, 0, IdleTexture.Width / 2, IdleTexture.Height);
                IdleRectDest = new Rectangle(0, MainForm.Height / 2, IdleRect.Width, IdleRect.Height);

                RunningRect = new Rectangle(0, 0, RunningTexture.Width / 2, RunningTexture.Height);
                RunningRectDest = new Rectangle(0, MainForm.Height / 2, RunningRect.Width, RunningRect.Height);

                CrouchRect = new Rectangle(0, 0, CrouchTexture.Width / 2, CrouchTexture.Height);
                CrouchRectDest = new Rectangle(0, MainForm.Height / 2 + CFrameMargin, CrouchRect.Width, CrouchRect.Height);

                FloorRect = new Rectangle(0, 0, FloorTexture.Width, FloorTexture.Height);
                FloorRectDest = new Rectangle(0, MainForm.Height / 2 + FloorMargin, FloorRect.Width, FloorRect.Height);

                FloorRect1 = new Rectangle(FloorRect.Width, 0, FloorTexture.Width, FloorTexture.Height);
                FloorRect1Dest = new Rectangle(FloorRectDest.Width, MainForm.Height / 2 + FloorMargin, FloorRect1.Width, FloorRect1.Height);

                Cactuse1Rect = new Rectangle(0, 0, CactusTexture.Width, CactusTexture.Height);
                Cactuse1DestRect = new Rectangle(1000, RunningRectDest.Y + 15, Cactuse1Rect.Width, Cactuse1Rect.Height);

                Cactuse2Rect = new Rectangle(0, 0, CactusTexture.Width, CactusTexture.Height);
                Cactuse2RectDest = new Rectangle(200, RunningRectDest.Y + 15, Cactuse2Rect.Width, Cactuse2Rect.Height);

                Cactuse3Rect = new Rectangle(0, 0, CactusTexture.Width, CactusTexture.Height);
                Cactuse3RectDest = new Rectangle(200, RunningRectDest.Y + 15, Cactuse3Rect.Width, Cactuse3Rect.Height);

                HIRect = new Rectangle(0, 0, HITexture.Width, HITexture.Height);
                HIRectDest = new Rectangle(MainForm.Width - (HITexture.Width + 18), 0, HIRect.Width, HIRect.Height);

                NumbersRect = new Rectangle(0, 0, NumbersTexturesArray[0].Width, NumbersTexturesArray[0].Height);
                NumbersRectDest = new Rectangle(10, 0, NumbersRect.Width, NumbersRect.Height);
            }
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("fonts");
            IdleTexture = Content.Load<Texture2D>("Idle");
            RunningTexture = Content.Load<Texture2D>("Running");
            CrouchTexture = Content.Load<Texture2D>("Crouching");
            FloorTexture = Content.Load<Texture2D>("Floor1");
            CactusTexture = Content.Load<Texture2D>("Cactus-0");
            for (int i = 0; i < 10; i++)
                NumbersTexturesArray[i] = Content.Load<Texture2D>($"Number-{i}");
            HITexture = Content.Load<Texture2D>("HI");

        }
        protected override void Update(GameTime gameTime)
        {

            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            // CycleDayAndNight(gameTime);
            InitCurrentDino();

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

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackColor);
            spriteBatch.Begin();
            //Drawing floor
            spriteBatch.Draw(FloorTexture, FloorRectDest, FloorRect, FloorColor);
            spriteBatch.Draw(FloorTexture, FloorRect1Dest, FloorRect1, FloorColor);

            //Drawing Cactus
            spriteBatch.Draw(CactusTexture, Cactuse1DestRect, Cactuse1Rect, CactusColor);

            //Drawing Score
            spriteBatch.Draw(HITexture, HIRectDest, HIRect, Color.White);
            messege = "";
            int[] digits = GetDigits(score);
            for (int i = 0; i < digits.Length; i++)
            {
                spriteBatch.Draw(IntTotexture2d(digits[i]), )
            }
            //Drawing Messesge
            spriteBatch.DrawString(font, messege.ToString()+"Score: "+score, Vector2.One, TextColor);

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
                spriteBatch.Draw(IdleTexture, IdleRectDest, IdleRect, DinoColor);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void Jump(GameTime gameTime)
        {
            if (!Jumped)
            {
                if (((MainForm.Height / 2) - IdleRectDest.Y) >= jumpHeight)
                {
                    Jumped = true;
                }
                else
                {
                    IdleRectDest.Y -= 10;
                    //RectIdleDest.X += 2;
                }
            }
            else
            {
                if (AirTime >= AirTimeTotal)
                {
                    if (IdleRectDest.Y == (MainForm.Height / 2))
                    {
                        Jumping = false;
                        Jumped = false;
                        AirTime = 0;
                    }
                    else
                    {
                        IdleRectDest.Y += 5;
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
            if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
                drawAction = DrawAction.Crouch;
            else if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
            {
                Jumping = true;
                drawAction = DrawAction.Idle;
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
                if (!IsRegionTransparent(CurrentDinoTexture, new Rectangle(0, 0, CurrentDinoRectDest.Width, CurrentDinoRectDest.Height))) //&& !(IsRegionTransparent(CactusTexture, new Rectangle(0,0, CactusDestRect.Width, CactusDestRect.Height))))
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
                    messege = $"Collision: {CollisionCount}";
                    CollisionCount++;
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
            if (drawAction == DrawAction.Running)
            {
                CurrentDinoRectDest = RunningRectDest;
                CurrentDinoTexture = RunningTexture;
            }
            if (drawAction == DrawAction.Idle)
            {
                CurrentDinoRectDest = IdleRectDest;
                CurrentDinoTexture = IdleTexture;
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
    }
}