using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CSOrbits.Classes
{
    class MovingObject
    {
        double mass;
        ObjPosition position;
        ObjPosition nextPosition;
        ObjMovement movement;
        double radius;
        Color colour;

#region Constructors

        public void init()
        {
            movement = new ObjMovement();
            position = new ObjPosition();
            nextPosition = new ObjPosition();
            mass = 0;
            position.xPos = 0;
            position.yPos = 0;
            movement.xVelocity = 0;
            movement.yVelocity = 0;
            radius = 0;
            colour = Color.FromArgb(255, 255, 255);
        }

        public MovingObject()
        {
            // Constructor
            init();
        }

        public MovingObject(MovingObject copy)
        {
            // Constructor
            init();
            mass = copy.mass;
            position.xPos = copy.position.xPos;
            position.yPos = copy.position.yPos;
            movement = copy.movement;
            radius = copy.radius;
            colour = copy.colour;
        }

        public MovingObject(double inMass, double inxPos, double inyPos, double inxVelocity, double inyVelocity, double inRadius)
        {
            init();
            position.xPos = inxPos;
            position.yPos = inyPos;
            movement.xVelocity = inxVelocity;
            movement.yVelocity = inyVelocity;
            radius = inRadius;
            colour = Color.FromArgb(255, 255, 255);
        }

        public MovingObject(double inMass, double inxPos, double inyPos, double inxVelocity, double inyVelocity, double inRadius, Color inColour)
        {
            init();
            mass = inMass;
            position.xPos = inxPos;
            position.yPos = inyPos;
            movement.xVelocity = inxVelocity;
            movement.yVelocity = inyVelocity;
            radius = inRadius;
            colour = inColour;
        }

        public MovingObject(double inMass, ObjPosition inPosition, ObjMovement inMovement, double inRadius)
        {
            init();
            mass = inMass;
	        position.xPos = inPosition.xPos;
	        position.yPos = inPosition.yPos;
	        movement.xVelocity = inMovement.xVelocity;
	        movement.yVelocity = inMovement.yVelocity;
	        radius = inRadius;
        }

        public MovingObject(double inMass, ObjPosition inPosition, ObjMovement inMovement, double inRadius, Color inColour)
        {
            init();
            mass = inMass;
	        position.xPos = inPosition.xPos;
	        position.yPos = inPosition.yPos;
	        movement.xVelocity = inMovement.xVelocity;
	        movement.yVelocity = inMovement.yVelocity;
	        radius = inRadius;
	        colour = inColour;
        }

#endregion Constructors
#region Setters

        public void setMass(double newMass)
        {
            mass = newMass;
        }

        public void setNextPosition(ObjPosition newPosition)
        {
            nextPosition = newPosition;
        }

        public void setNextPosition(double newXPos, double newYPos)
        {
            nextPosition.xPos = newXPos;
            nextPosition.yPos = newYPos;
        }

        public void setMovement(ObjMovement newMovement)
        {
            movement = newMovement;
        }

        public void setMovement(double newXVel, double newYVel)
        {
            movement.xVelocity = newXVel;
            movement.yVelocity = newYVel;
        }

        public void setColour(Color newColour)
        {
            colour = newColour;
        }

        public void setColour(int r, int g, int b)
        {
            if (r >= 0 && r < 256 && g >= 0 && g < 256 && b >= 0 && b < 256)
                colour = Color.FromArgb(r, g, b);
        }

        public void setRadius(double newRadius)
        {
            radius = newRadius;
        }

#endregion Setters
#region Getters

        public ObjPosition getPosition()
        {
            return position;
        }

        public ObjMovement getMovement()
        {
            return movement;
        }

        public double getVelocity()
        {
            return movement.speed();
        }

        public double getDirection()
        {
            return movement.direction();
        }

        public double getMass()
        {
            return mass;
        }

        public Color getColour()
        {
            return colour;
        }

        public double getRadius()
        {
            return radius;
        }

#endregion Getters
#region Functions

        public void updatePosition()
        {
            position.assign(nextPosition);
        }

        public void changeMovement(double gravitationalForce, double direction)
        {
            // We have the magnitude of the gravitational force and the direction
            // Now we merely have to work out what influence it has on the movement of this object

            // The force will be countered by the object's mass

            double xMag = (gravitationalForce / mass) * Math.Cos(direction);
            double yMag = (gravitationalForce / mass) * Math.Sin(direction);

            // These will change the current movement values
            movement.xVelocity += xMag;
            movement.yVelocity += yMag;

            // Then add these to the position
            nextPosition.xPos = position.xPos + movement.xVelocity;
            nextPosition.yPos = position.yPos + movement.yVelocity;
        }

        public void changeMovement(ObjMovement objMovement)
        {
            movement.xVelocity += objMovement.xVelocity;
            movement.yVelocity += objMovement.yVelocity;

            nextPosition.xPos = position.xPos + movement.xVelocity;
            nextPosition.yPos = position.yPos + movement.yVelocity;
        }

        public static ObjMovement calcMovement(double magnitude, double direction)
        {
            ObjMovement calcMovement = new ObjMovement();
            calcMovement.xVelocity = magnitude * Math.Cos(direction);
            calcMovement.yVelocity = magnitude * Math.Sin(direction);
            return calcMovement;
        }

#endregion Functions
    }
}

