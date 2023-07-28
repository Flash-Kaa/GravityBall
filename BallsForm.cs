using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GravityBalls
{
	public class BallsForm : Form
	{
        private Timer timer;
		private WorldModel world;

        private WorldModel CreateWorldModel()
		{
			var w = new WorldModel
			{
				WorldHeight = ClientSize.Height,
				WorldWidth = ClientSize.Width,
				BallRadius = 10
			};
			w.BallX = w.WorldHeight / 2;
			w.BallY = w.BallRadius;
			return w;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			world.WorldHeight = ClientSize.Height;
			world.WorldWidth = ClientSize.Width;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			DoubleBuffered = true;
			BackColor = Color.Black;
			world = CreateWorldModel();
			timer = new Timer { Interval = 30 };
			timer.Tick += TimerOnTick;
			timer.Start();
			world.WorldHeight = ClientSize.Height;
			world.WorldWidth = ClientSize.Width;
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			world.SimulateTimeframe(timer.Interval / 1000d);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
            DrawCounterAndGates(e, Color.Red, WorldModel.indent,WorldModel.CountRedGoal);
            DrawCounterAndGates(e, Color.Green, (int)(ClientSize.Width * (1 - WorldModel.partOccupByGate)) - WorldModel.indent, WorldModel.CountGreenGoal);
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.FillEllipse(Brushes.GreenYellow, (float)(world.BallX - world.BallRadius), 
				(float)(world.BallY - world.BallRadius), 
				2 * (float)world.BallRadius, 2 * (float)world.BallRadius);
        }

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Text = string.Format("Cursor ({0}, {1})", e.X, e.Y);
		}

		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BallsForm));
            this.SuspendLayout();
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "BallsForm";
            this.ResumeLayout(false);

		}
        private void DrawCounterAndGates(PaintEventArgs e, Color colorRect, int startX, int goal)
        {
			Goalsa(e, colorRect, startX, goal);
            Rectangle rectangle = new Rectangle(startX, WorldModel.indent, 
				(int)(ClientSize.Width * WorldModel.partOccupByGate), ClientSize.Height - 5* WorldModel.indent);
            e.Graphics.DrawRectangle(new Pen(colorRect), rectangle);
        }

		private void Goalsa(PaintEventArgs e, Color colorRect, int startX, int goal)
		{ 
			Rectangle rectangle = new Rectangle(startX, WorldModel.indent, (int)(ClientSize.Width * WorldModel.partOccupByGate), goal* (Height / WorldModel.countToWinGoals));
            e.Graphics.FillRectangle(new SolidBrush(colorRect), rectangle);
        }
    }
}