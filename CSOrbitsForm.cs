using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSOrbits.Classes;

namespace CSOrbits
{
    public partial class CSOrbitsForm : Form
    {
        private GroupOfObjects groupOfObjects = new GroupOfObjects();
        private bool timerRunning;
        private bool inTickEvent;
        private System.Timers.Timer timer;

        public CSOrbitsForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();

            timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            timer.Interval = 10;

            // Add test objects to the system
            MovingObject test1 = new MovingObject();
            MovingObject test2 = new MovingObject();
            MovingObject test3 = new MovingObject();
            MovingObject test4 = new MovingObject();

            test1.setMass(100);
            test1.setRadius(20);
            test1.setNextPosition(100, 130);
            test1.setMovement(0, 0);
            test1.setColour(Color.FromArgb(255, 255, 0));

            test2.setMass(1);
            test2.setRadius(5);
            test2.setNextPosition(100, 80);
            test2.setMovement(0.15, 0.0);
            test2.setColour(Color.FromArgb(0, 255, 0));

            test3.setMass(0.2);
            test3.setRadius(2);
            test3.setNextPosition(100, 40);
            test3.setMovement(0.1, 0);
            test3.setColour(Color.FromArgb(0, 0, 255));

            test4.setMass(1);
            test4.setRadius(5);
            test4.setNextPosition(100, 180);
            test4.setMovement(-0.15, 0);
            test4.setColour(Color.FromArgb(255, 0, 0));

            test1.updatePosition();
            test2.updatePosition();
            test3.updatePosition();
            test4.updatePosition();

            groupOfObjects.addObject(test1);
            groupOfObjects.addObject(test2);
            groupOfObjects.addObject(test3);
            groupOfObjects.addObject(test4);


            // Add 200 objects. Check to make sure the one being added doesn't overlap another one
            
            MovingObject insertObject;
            int addedObjects = 0;
            Random rand = new Random();

            while (addedObjects < 200)
            {
                double tempMass = rand.Next(1,100);
                insertObject = new MovingObject();
                insertObject.setMass(tempMass / 100);
                insertObject.setRadius(tempMass / 10);
                if (insertObject.getRadius() < 1) insertObject.setRadius(1);
                insertObject.setNextPosition(rand.Next(300,600), rand.Next(1,300));
                //		insertObject.SetMovement(((double)(rand() % 100) / 100.0) - 0.5, ((double)(rand() % 100) / 100.0) - 0.5);
                insertObject.setColour(Color.FromArgb(rand.Next(0,255), rand.Next(0, 255), rand.Next(0, 255)));
                insertObject.updatePosition();
                if (groupOfObjects.objectOverlaps(insertObject) < 0)
                {
                    groupOfObjects.addObject(insertObject);
                    addedObjects++;
                }
            }

            insertObject = new MovingObject();
            insertObject.setMass(1);
            insertObject.setRadius(2);
            insertObject.setNextPosition(10000, 150);
            insertObject.setMovement(-10, 0);
            insertObject.setColour(Color.FromArgb(0, 0, 0));
            insertObject.updatePosition();
            groupOfObjects.addObject(insertObject);


            // Set the timer flags to false (as it isn't running yet)
            timerRunning = false;
            inTickEvent = false;
        }

        private void CSOrbitsForm_Paint(object sender, PaintEventArgs e)
        {

            BufferedGraphicsContext currentContext;
            BufferedGraphics myBuffer;
            currentContext = BufferedGraphicsManager.Current;
            currentContext.MaximumBuffer = new Size(this.ClientSize.Width + 1, this.ClientSize.Height + 1);
            myBuffer = currentContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
            myBuffer.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));

            // Access each moving object and draw it
            MovingObject tempObj;
            for (int i = 0; i < groupOfObjects.numberOfObjects(); i++)
            {
                tempObj = groupOfObjects.getObject(i);

                // Check to see if the object has hit the edge and get it to bounce back
                int xSign = 1;
                int ySign = 1;
                if (((tempObj.getPosition().xPos - tempObj.getRadius()) <= 0 && tempObj.getMovement().xVelocity < 0) ||
                    ((tempObj.getPosition().xPos + tempObj.getRadius()) >= this.ClientSize.Width && tempObj.getMovement().xVelocity > 0))
                {
                    xSign = -1;
                }
                if (((tempObj.getPosition().yPos - tempObj.getRadius()) <= 0 && tempObj.getMovement().yVelocity < 0) ||
                    ((tempObj.getPosition().yPos + tempObj.getRadius()) >= this.ClientSize.Height && tempObj.getMovement().yVelocity > 0))
                {
                    ySign = -1;
                }
                if (xSign != 1 || ySign != 1)
                    tempObj.setMovement(tempObj.getMovement().xVelocity * xSign, tempObj.getMovement().yVelocity * ySign);

                int radius = (int)tempObj.getRadius();
                Rectangle myRect = new Rectangle((int)tempObj.getPosition().xPos - radius, (int)tempObj.getPosition().yPos - radius, 
                    (int)tempObj.getRadius() + radius, (int)tempObj.getRadius() + radius);
                Brush myBrush = new SolidBrush(tempObj.getColour());
                myBuffer.Graphics.FillEllipse(myBrush, myRect);

                myBuffer.Render(e.Graphics);
/*
                System.Drawing.Graphics formGraphics = this.CreateGraphics();
                string drawString = "XPos = " + groupOfObjects.getObject(0).getPosition().xPos;
                System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 10);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                float x = 10.0F;
                float y = 370.0F;
                System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                formGraphics.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);

                y = 400.0F;
                drawString = "YPos = " + groupOfObjects.getObject(0).getPosition().yPos;
                formGraphics.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
                drawFont.Dispose();
                drawBrush.Dispose();
                formGraphics.Dispose();
*/
            }
        }

        override protected void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing, to get rid of flickering
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            timer.Stop();
            Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (timerRunning)
            {
                timerRunning = false;
                timer.Stop();
                buttonStart.Text= "Start";
            }
            else
            {
                timerRunning = true;
                timer.Start();
                buttonStart.Text = "Stop";
            }

        }

        private void OnTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            //timer.Stop();

            if (!inTickEvent)
            {
                inTickEvent = true;
                groupOfObjects.tick();
                Invalidate();
                inTickEvent = false;
            }

            //timer.Start();
        }
    }
}
