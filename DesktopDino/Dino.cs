using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace DesktopDino
{
    class Dino : PictureBox
    {

        /// <summary>
        /// A list of all the names of the images
        /// </summary>
        public Bitmap[] Frames;

        public Bitmap[] FlippedFrames;

        private int frame = 0;

        private bool moving = false;

        /// <summary>
        /// This is the time between frames in milliseconds
        /// </summary>
        public float TimeBetweenFrames = 300;

        public Timer Timer;
        private Timer dinoWait;

        private ResourceManager resources;

        private Form1 form;

        private IKeyboardMouseEvents globalHook;

        private Point newLocation;

        private PointF movementVector;

        private float speed = 10;

        private bool newLocToTheRight = true;
        private bool newLocIsUp = false;

        private int maxY;
        private int maxX;

        public Dino() { }

        public Dino(Timer timer, Timer dinoTick, Timer dinoWait, Form1 form)
        {
            globalHook = Hook.GlobalEvents();
            globalHook.MouseClick += MouseClick;
            Location = new Point(128, 160);
            SizeMode = PictureBoxSizeMode.AutoSize;
            resources = new ResourceManager("DinoFrames", GetType().Assembly);
            Timer = timer;
            Timer.Tick += FrameTick;
            Frames = new Bitmap[] { Properties.Resources.DinoDesktop_1, Properties.Resources.DinoDesktop_3, Properties.Resources.DinoDesktop_4 };
            FlippedFrames = new Bitmap[] { Properties.Resources.DinoDesktopFlipped_1, Properties.Resources.DinoDesktopFlipped_3, Properties.Resources.DinoDesktopFlipped_4 };
            Timer.Start();
            dinoTick.Tick += Tick;
            newLocation = Location;
            dinoTick.Start();
            this.form = form;
            dinoWait.Tick += DinoWait;
            maxX = form.Size.Width - Properties.Resources.DinoDesktop_1.Width - 10;
            maxY = form.Size.Height - Properties.Resources.DinoDesktop_1.Height - 10;
            dinoWait.Start();
            this.dinoWait = dinoWait;
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            newLocation = e.Location;
            if (newLocation.X > Location.X) //positive
            {
                newLocToTheRight = true;
            }
            else
            {
                newLocToTheRight = false;
            }
            if (newLocation.Y > Location.Y)
            {
                newLocIsUp = false;
            }
            else
            {
                newLocIsUp = true;
            }
            if (newLocation.X > form.Size.Width - Properties.Resources.DinoDesktop_1.Width - 10)
            {
                newLocation = new Point(maxX, newLocation.Y);
            }
            if (newLocation.Y > form.Size.Height - Properties.Resources.DinoDesktop_1.Height - 10)
            {
                newLocation = new Point(newLocation.X, maxY);
            }
            movementVector = Normalize(new Point(newLocation.X - Location.X, newLocation.Y - Location.Y));
        }

        private void FrameTick(object sender, EventArgs e)
        {
            if (moving)
            {
                if (frame == 0) frame = 1;
                Image = newLocToTheRight ? Frames[frame] : FlippedFrames[frame];
                frame = frame == 1 ? 2 : 1;
            }
            else
            {
                frame = 0;
                Image = newLocToTheRight ? Properties.Resources.DinoDesktop_1 : Properties.Resources.DinoDesktopFlipped_1;
            }
        }

        private void Tick(object sender, EventArgs e)
        {
            if (newLocation == Location)
            {
                moving = false;
            }
            if ((newLocToTheRight && Location.X >= newLocation.X) || (newLocIsUp && Location.Y <= newLocation.Y))
            {
                moving = false;
            }
            else if ((!newLocToTheRight && Location.X <= newLocation.X) || (!newLocIsUp && Location.Y >= newLocation.Y))
            {
                moving = false;
            }
            else 
            {
                moving = true;
                Location = new Point((int)(movementVector.X * speed) + Location.X, (int)(movementVector.Y * speed) + Location.Y);
            }
        }

        private PointF Normalize(Point point)
        {
            float distance = (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
            return new PointF((point.X / distance), (point.Y / distance));
        }

        private void DinoWait(object sender, EventArgs e)
        {
            if (moving) return;
            Random random = new Random();
            newLocation = new Point(random.Next(0, maxX), random.Next(0, maxY));
            if (newLocation.X > Location.X) //positive
            {
                newLocToTheRight = true;
            }
            else
            {
                newLocToTheRight = false;
            }
            if (newLocation.Y > Location.Y)
            {
                newLocIsUp = false;
            }
            else
            {
                newLocIsUp = true;
            }
            if (newLocation.X > form.Size.Width - Properties.Resources.DinoDesktop_1.Width - 10)
            {
                newLocation = new Point(maxX, newLocation.Y);
            }
            if (newLocation.Y > form.Size.Height - Properties.Resources.DinoDesktop_1.Height - 10)
            {
                newLocation = new Point(newLocation.X, maxY);
            }
            movementVector = Normalize(new Point(newLocation.X - Location.X, newLocation.Y - Location.Y));
        }

    }
}
