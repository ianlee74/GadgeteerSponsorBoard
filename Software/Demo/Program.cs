using System;
using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.IngenuityMicro;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace Kit_Demo
{
	public partial class Program : Gadgeteer.Program
	{
		public void ProgramStarted()
		{
            ledLevel.CylonSleep = 400;
            ledLevel.CylonLedCount = 16;
            ledLevel.StartCylon();
			Bouncer demo = new Bouncer(display_N18, tunes, joystick, led_Strip, button, ledLevel);
			demo.Start();
		}
	}

	public class Bouncer
	{
        
		private const int STARTING_LIVES = 7;
		private const int SPLASH_LENGTH = 3500;
		private const int FPS = 30;
		private const int MS_PER_FRAME = 1000 / Bouncer.FPS;
		public const int SCREEN_WIDTH = 128;
		public const int SCREEN_HEIGHT = 160;

		private DisplayN18 Display;
		private Tunes Sound;
		private Joystick Input;
		private LEDStrip LED;
        private LedLevel _ledLevel;
//		private LightSensor Light;
        private Button lives_button;
		private int Lives;
		private Ball Ball;
		private Paddle Paddle;
		private Timer Timer;
		private DateTime StartTime;

//        public Bouncer(Display_N18 display, Tunes tunes, Joystick joystick, LightSensor light, LED_Strip led, Button button)
        public Bouncer(DisplayN18 display, Tunes tunes, Joystick joystick,  LEDStrip led, Button button, LedLevel ledLevel)
        {
			this.Display = display;
			this.Sound = tunes;
			this.Input = joystick;
			this.LED = led;
            this._ledLevel = ledLevel;
			//this.Light = light;
            this.lives_button = button;
			this.Ball = new Ball(this.Display, this.Sound, this._ledLevel);
			this.Paddle = new Paddle(this.Display, this.Input);
		}

		public void Start()
		{
			this.DisplaySplash();
			this.Lives = Bouncer.STARTING_LIVES;
            this.lives_button.ButtonPressed += new Button.ButtonEventHandler(lives_button_ButtonPressed);
			this.StartTime = DateTime.Now;
			this.Timer = new Timer(this.Tick, null, Bouncer.MS_PER_FRAME, Bouncer.MS_PER_FRAME);
		}
        private void lives_button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            this.Lives += 1;
            if (this.Lives > Bouncer.STARTING_LIVES)
                this.Lives = Bouncer.STARTING_LIVES;
        }
		private void Tick(Object state)
		{
			this.Paddle.Tick();

			if (this.Ball.Tick(this.Paddle) && --this.Lives == 0)
			{
				this.Timer.Change(Timeout.Infinite, Timeout.Infinite);
				this.DisplayLose();
				return;
			}

			this.DisplayLives();
		}

		private void DisplayLives()
		{
			for (int i = 0; i < 7; i++)
				this.LED.TurnLEDOff(i);
			for (int i = 0; i < this.Lives; i++)
				this.LED.TurnLEDOn(i);
		}

		private void DisplaySplash()
		{
			this.Display.Clear();

			uint totalWidth = Bouncer.SCREEN_WIDTH;
			uint totalHeight = Bouncer.SCREEN_HEIGHT;
			uint width = totalWidth / 8;
			uint height = totalHeight / 5;
			byte[] data = new byte[width * height * 2];
			for (uint i = 0; i < width * height * 2; i++)
				data[i] = 0xFF;

			for (uint x = 0; x < totalWidth; x += width)
				for (uint y = 0; y < totalHeight; y += height)
					this.Display.DrawRaw(data, width, height, x, y);
/*
			string text = "It is bright here!";
			double reading = this.Light.ReadLightSensorPercentage();
			if (reading < 66 && reading > 33)
				text = "It is average";
			else if (reading <= 33)
				text = "It is dark here!";
*/
            string text = "Hello World!";
			Bitmap textBmp = new Bitmap(75, 12);
			textBmp.DrawRectangle(Color.White, 1, 0, 0, 75, 12, 0, 0, Color.White, 0, 0, Color.White, 75, 12, 0xFF);
			textBmp.DrawText(text, Resources.GetFont(Resources.FontResources.small), Color.Black, 0, 0);
			this.Display.Draw(textBmp, (uint)((Bouncer.SCREEN_WIDTH / 2) - (textBmp.Width / 2)), 0);
			textBmp.Dispose();

			var bitmap = Resources.GetBitmap(Resources.BitmapResources.Splash);
			this.Display.Draw(bitmap, (uint)((Bouncer.SCREEN_WIDTH / 2) - (bitmap.Width / 2)), (uint)((Bouncer.SCREEN_HEIGHT / 2) - (bitmap.Height / 2)));
			bitmap.Dispose();
			Thread.Sleep(Bouncer.SPLASH_LENGTH);
			this.Display.Clear();
		}

		private void DisplayLose()
		{
			this.LED.TurnLEDOff(0);
			this.Display.Clear();

			Bitmap textBmp = new Bitmap(60, 12);
			textBmp.DrawText("Lived: " + (DateTime.Now - this.StartTime).Seconds.ToString() + "s", Resources.GetFont(Resources.FontResources.small), Color.White, 0, 0);
			this.Display.Draw(textBmp, (uint)((Bouncer.SCREEN_WIDTH / 2) - (textBmp.Width / 2)), (uint)((Bouncer.SCREEN_HEIGHT / 2) - (textBmp.Height / 2)));
			textBmp.Clear();
			textBmp.DrawText("You Lose!", Resources.GetFont(Resources.FontResources.small), Color.White, 0, 0);
			this.Display.Draw(textBmp, (uint)((Bouncer.SCREEN_WIDTH / 2) - (textBmp.Width / 2) + 1), (uint)((Bouncer.SCREEN_HEIGHT / 2) - (textBmp.Height / 2) + 14));
			textBmp.Dispose();
		}
	}

	public class Paddle
	{
		public const int WIDTH = 35;
		public const int HEIGHT = 5;

		private Joystick Input;
		private DisplayN18 Display;

		private Bitmap Image;
		private Bitmap Clear;


		public int X;

        public Paddle(DisplayN18 display, Joystick joystick)
        {
            this.Input = joystick;
            this.Display = display;
            this.X = Bouncer.SCREEN_WIDTH / 2 - Paddle.WIDTH / 2;

            this.Image = new Bitmap(Paddle.WIDTH, Paddle.HEIGHT);
            this.Image.DrawRectangle(Gadgeteer.Color.Blue, 1, 0, 0, Paddle.WIDTH, Paddle.HEIGHT, 0, 0, Gadgeteer.Color.Blue, 0, 0, Gadgeteer.Color.Blue, Paddle.WIDTH, Paddle.HEIGHT, 0xFF);

            this.Clear = new Bitmap(Paddle.WIDTH, Paddle.HEIGHT);
        }

		public void Tick()
		{
			this.Display.Draw(this.Clear, (uint)this.X, Bouncer.SCREEN_HEIGHT - Paddle.HEIGHT);

		
				this.X = (int)(this.Input.GetJoystickPosition().X * Bouncer.SCREEN_WIDTH - Paddle.WIDTH / 2);
	
			if (this.X < 0)
				this.X = 0;
			if (this.X + Paddle.WIDTH > Bouncer.SCREEN_WIDTH)
				this.X = Bouncer.SCREEN_WIDTH - Paddle.WIDTH;

			this.Display.Draw(this.Image, (uint)this.X, Bouncer.SCREEN_HEIGHT - Paddle.HEIGHT);
		}


	}

	public class Ball
	{
		private const int HIT_DURATION = 75;
		private const int MAX_SPEED = 4;
		private const int MIN_SPEED = 2;
		private const int RADIUS = 9;
		private const int SIZE = Ball.RADIUS * 2 + 1;

		private static Random Rng = new Random((int)DateTime.Now.Ticks);

		private Bitmap Image;
		private Bitmap Clear;

		private DisplayN18 Display;
		private Tunes Tunes;
        private LedLevel _ledLevel;

		private int XSpeed;
		private int YSpeed;
		public int X;
		public int Y;

		public Ball(DisplayN18 display, Tunes tunes, LedLevel ledLevel)
		{
			this.X = Bouncer.SCREEN_WIDTH / 2;
			this.Y = Bouncer.SCREEN_HEIGHT / 2;
			this.XSpeed = this.GenerateSpeed();
			this.YSpeed = this.GenerateSpeed();

			this.Display = display;
			this.Tunes = tunes;
            _ledLevel = ledLevel;

            this.Image = new Bitmap(Ball.SIZE, Ball.SIZE);
            this.Image.DrawImage(0, 0, Resources.GetBitmap(Resources.BitmapResources.Ball), 0, 0, Ball.SIZE, Ball.SIZE);

            this.Clear = new Bitmap(Ball.SIZE, Ball.SIZE);
        }

		public bool Tick(Paddle paddle)
		{
			this.Display.Draw(this.Clear, (uint)(this.X - Ball.RADIUS), (uint)(this.Y - Ball.RADIUS));
			bool lostLife = this.Advance(paddle);
			this.Display.Draw(this.Image, (uint)(this.X - Ball.RADIUS), (uint)(this.Y - Ball.RADIUS));
			return lostLife;
		}

		private bool Advance(Paddle paddle)
		{
			this.X += this.XSpeed;
			this.Y += this.YSpeed;

			if (this.Y + Ball.RADIUS + 2 >= Bouncer.SCREEN_HEIGHT - Paddle.HEIGHT)
			{
				if (this.X >= paddle.X && this.X <= paddle.X + Paddle.WIDTH)
				{
					this.YSpeed *= -1;

					this.Tunes.AddNote(new Tunes.MusicNote(Tunes.Tone.B4, Ball.HIT_DURATION));
					this.Tunes.Play();
				}
				else
				{
                    _ledLevel.StopCylon();
                    _ledLevel.SetLevel(LedLevel.LED.LED12);

                    this.X = Bouncer.SCREEN_WIDTH / 2;
					this.Y = Bouncer.SCREEN_HEIGHT / 2;
					this.XSpeed = this.GenerateSpeed();
					this.YSpeed = this.GenerateSpeed();

                    _ledLevel.Clear();
                    _ledLevel.StartCylon();

					return true;
				}
			}

			if (this.X + this.XSpeed - Ball.RADIUS < 0 || this.X + Ball.RADIUS >= Bouncer.SCREEN_WIDTH)
				this.XSpeed *= -1;

			if (this.Y + this.YSpeed - Ball.RADIUS < 0 || this.Y + Ball.RADIUS >= Bouncer.SCREEN_HEIGHT)
				this.YSpeed *= -1;

			return false;
		}

		private int GenerateSpeed()
		{
			return (Ball.Rng.Next(Ball.MAX_SPEED - Ball.MIN_SPEED) + Ball.MIN_SPEED) * (Ball.Rng.Next(10) % 2 == 0 ? 1 : -1);
		}
	}
}
