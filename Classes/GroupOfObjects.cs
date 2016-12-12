using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSOrbits.Classes
{
    class GroupOfObjects
    {
        private List<MovingObject> vecOfObjects;
        private double scaleFactor;

#region Constructors

        public GroupOfObjects()
        {
            // Constructor
            scaleFactor = 100.0; // Default value
            vecOfObjects = new List<MovingObject>();
        }

        public GroupOfObjects(GroupOfObjects newGOP)
        {
            // Copy constructor

            // Copy the contents of the List
            foreach(MovingObject gop in newGOP.vecOfObjects)
            {
                vecOfObjects.Add(gop);
            }
        }



        #endregion Constructors

        public void addObject(MovingObject newObject)
        {
            vecOfObjects.Add(newObject);
        }

        public int numberOfObjects()
        {
            return vecOfObjects.Count;
        }

        public MovingObject getObject(int index)
        {
            if (index < 0 || index > vecOfObjects.Count)
            {
                return null;
            }

            return vecOfObjects[index];
        }

        public void setScaleFactor(double newScaleFactor)
        {
            if (newScaleFactor > 0.0)
                scaleFactor = newScaleFactor;
        }

        public void tick()
        {
            // The progression of the objects in the list

            // Start off with calcs for 0, 1 & 2
            ObjMovement tempMov;
            ObjPosition tempPos;
            double mass1, mass2, direction1, direction2, distance, gravitationalForce;

            switch (vecOfObjects.Count)
            {
                case 0:
                    // Nothing to do
                    break;
                case 1:
                    // If it is moving, update the position to reflect the movement
                    tempPos = vecOfObjects[0].getPosition();
                    tempMov = vecOfObjects[0].getMovement();
                    tempPos.xPos += tempMov.xVelocity;
                    tempPos.yPos += tempMov.yVelocity;
                    vecOfObjects[0].setNextPosition(tempPos);
                    vecOfObjects[0].updatePosition();
                    break;
                case 2:
                    // Calculate the movement of the 2 objects.
                    // Each acts on the other proportional to the mass and inversely propertional to the distance squared

                    // Now to do the math
                    ObjPosition objPos1 = vecOfObjects[0].getPosition();
                    ObjPosition objPos2 = vecOfObjects[1].getPosition();
                    mass1 = vecOfObjects[0].getMass();
                    mass2 = vecOfObjects[1].getMass();

                    distance = Math.Sqrt(((objPos1.xPos - objPos2.xPos) * (objPos1.xPos - objPos2.xPos)) + ((objPos1.yPos - objPos2.yPos) * (objPos1.yPos - objPos2.yPos)));

                    gravitationalForce = (mass1 * mass2) / (distance * distance);
                    gravitationalForce /= scaleFactor;

                    // Get the direction of the force
                    direction1 = Math.Atan2((objPos1.yPos - objPos2.yPos), (objPos1.xPos - objPos2.xPos));
                    direction2 = Math.Atan2((objPos2.yPos - objPos1.yPos), (objPos2.xPos - objPos1.xPos));


                    // Test - check for collision
                    if (collisionDetected(vecOfObjects[0], vecOfObjects[1]))
                    {
                        rebound(vecOfObjects[0], vecOfObjects[1]);

                    }
                    else
                    {

                        // So the objects velocity will change by the amount described in the direction described
                        vecOfObjects[0].changeMovement(gravitationalForce, direction2);
                        vecOfObjects[1].changeMovement(gravitationalForce, direction1);
                    }

                    vecOfObjects[0].updatePosition();
                    vecOfObjects[1].updatePosition();

                    break;
                default:
                    // Now to calculate for more than 2 objects
                    // Each object is acted upon by both of the others

                    for (int ourObject = 0; ourObject < vecOfObjects.Count; ++ourObject)
                    {
                        // Calculate the force between our item and each other item and then add up the forces
                        ObjMovement ourMovement = new ObjMovement();
                        ourMovement.xVelocity = 0;
                        ourMovement.yVelocity = 0;

                        ObjPosition ourPosition = vecOfObjects[ourObject].getPosition();
                        double ourMass = vecOfObjects[ourObject].getMass();
                        if (ourMass == 0) ourMass = Double.MinValue;

                        for (int otherObjects = 0; otherObjects < vecOfObjects.Count; ++otherObjects)
                        {
                            // Calc the value for every object that isn't us
                            if (otherObjects != ourObject)
                            {
                                // Calculate if any objects have collided
                                // Make sure we don't do it twice
                                if (otherObjects > ourObject)
                                {
                                    if (collisionDetected(vecOfObjects[ourObject], vecOfObjects[otherObjects]))
                                    {
                                        rebound(vecOfObjects[ourObject], vecOfObjects[otherObjects]);
                                    }
                                }

                                ObjPosition otherPosition = vecOfObjects[otherObjects].getPosition();
                                // Don't process 2 objects in exactly the same location as this gives infinity and everything breaks
                                if (ourPosition.xPos == otherPosition.xPos &&
                                    ourPosition.yPos == otherPosition.yPos)
                                    continue;

                                double otherMass = vecOfObjects[otherObjects].getMass();
                                int infinityfound = 1;

                                distance = Math.Sqrt(((ourPosition.xPos - otherPosition.xPos) * (ourPosition.xPos - otherPosition.xPos)) + ((ourPosition.yPos - otherPosition.yPos) * (ourPosition.yPos - otherPosition.yPos)));

                                gravitationalForce = (ourMass * otherMass) / (distance * distance);
                                gravitationalForce /= scaleFactor;
                                if (Double.IsInfinity(gravitationalForce))
                                    infinityfound = 0;

                                double otherDirection = Math.Atan2((otherPosition.yPos - ourPosition.yPos), (otherPosition.xPos - ourPosition.xPos));

                                ourMovement += MovingObject.calcMovement((gravitationalForce / ourMass), otherDirection);
                            }
                        }

                        // Total movement force has been calculated. Now update
                        vecOfObjects[ourObject].changeMovement(ourMovement);

                        int nanan = 0;
                        if (Double.IsNaN(vecOfObjects[ourObject].getPosition().xPos) || Double.IsInfinity(vecOfObjects[ourObject].getPosition().xPos))
                            nanan = 0;
                    }

                    // Update position and also ask - Have any objects gone too far away?
                    for (int ourObject = vecOfObjects.Count - 1; ourObject >= 0; --ourObject)
                    {
                        vecOfObjects[ourObject].updatePosition();
                        if (vecOfObjects[ourObject].getPosition().xPos > 100000 || vecOfObjects[ourObject].getPosition().xPos < -100000 ||
                            vecOfObjects[ourObject].getPosition().yPos > 100000 || vecOfObjects[ourObject].getPosition().yPos < -100000)
                        {
                            vecOfObjects.RemoveAt(ourObject);
                        }
                    }
                    break;
            }
        }

        public bool collisionDetected(MovingObject obj1, MovingObject obj2)
        {
            // Changes needed to detect collisions along the path of movement of the two objects to
            // avoid the situation of them passing through each other due to their speed, or an unusual seeming
            // rebound as the collision is detected after obj1 has partly passed through obj2

            // Create a rectangle bounding where the object starts from and where it is going to, taking into account its radius
            // Do this for both and then see if the rectangles overlap. If not exit, otherwise try to work out if they do really
            // collide and where they are when this happens.

            // Can't use CRect. Need a similar struct but with doubles as it is rounding up all to identical values
            ObjRect rect1 = new ObjRect();
            ObjRect rect2 = new ObjRect();
            // If the object has a positive velocity it is moving right, so add the velocity amount to get where it will be to make the
            // right edge of the rectangle, else if is moving left make the left edge where it will be by adding the velocity.
            rect1.left = obj1.getPosition().xPos - obj1.getRadius();
            rect1.right = obj1.getPosition().xPos + obj1.getRadius();
            rect1.top = obj1.getPosition().yPos - obj1.getRadius();
            rect1.bottom = obj1.getPosition().yPos + obj1.getRadius();
            rect2.left = obj2.getPosition().xPos - obj2.getRadius();
            rect2.right = obj2.getPosition().xPos + obj2.getRadius();
            rect2.top = obj2.getPosition().yPos - obj2.getRadius();
            rect2.bottom = obj2.getPosition().yPos + obj2.getRadius();
            if (obj1.getMovement().xVelocity > 0)
            {
                rect1.right += obj1.getMovement().xVelocity;
            }
            else
            {
                rect1.left += obj1.getMovement().xVelocity;
            }
            if (obj1.getMovement().yVelocity > 0)
            {
                rect1.bottom += obj1.getMovement().yVelocity;
            }
            else
            {
                rect1.top += obj1.getMovement().yVelocity;
            }

            if (obj2.getMovement().xVelocity > 0)
            {
                rect2.right += obj2.getMovement().xVelocity;
            }
            else
            {
                rect2.left += obj2.getMovement().xVelocity;
            }
            if (obj2.getMovement().yVelocity > 0)
            {
                rect2.bottom += obj2.getMovement().yVelocity;
            }
            else
            {
                rect2.top += obj2.getMovement().yVelocity;
            }

            // Do the rectangles overlap?
            if (rect1.left > rect2.right || rect1.right < rect2.left ||
                rect1.top > rect2.bottom || rect1.bottom < rect2.top)
            {
                return false;
            }

            // The rectangles overlap, but this doesn't necessarily mean they will collide or are colliding just now

            // Are they currently colliding?
            double obj1XPos = obj1.getPosition().xPos;
            double obj1YPos = obj1.getPosition().yPos;
            double obj2XPos = obj2.getPosition().xPos;
            double obj2YPos = obj2.getPosition().yPos;
            double xDistance = obj1XPos - obj2XPos;
            double yDistance = obj1YPos - obj2YPos;
            double distanceNowSQR = (xDistance * xDistance) + (yDistance * yDistance);
            double combinedRadii = obj1.getRadius() + obj2.getRadius();
            combinedRadii -= (obj1.getRadius() > obj2.getRadius() ? (obj2.getRadius() / 10) : (obj1.getRadius() / 10));
            double combinedRadiiSQR = combinedRadii * combinedRadii;
            if (combinedRadiiSQR > distanceNowSQR) return true;

            double xDistanceThen = xDistance + obj1.getMovement().xVelocity - obj2.getMovement().xVelocity;
            double yDistanceThen = yDistance + obj1.getMovement().yVelocity - obj2.getMovement().yVelocity;
            double distanceThenSQR = (xDistanceThen * xDistanceThen) + (yDistanceThen * yDistanceThen);

            // So the objects don't collide at the start or end of the movement. So we need to find out if they collide at any point
            // inbetween
            // So we try to find the closest point of the 2 objects

            // The maths of detecting closest points between 2 moving objects is surprisingly complex. It is more that it has a lot of steps
            // using trig functions. It turns out multisampling will be quicker in most cases - i.e. binary search

            bool closeEnough = false;
            double newDistance;
            int MAXSTEP = 10;
            int CurrentStep = 0;
            double startPointDistance = distanceNowSQR;
            double endPointDistance = distanceThenSQR;
            double x1StartPoint = obj1XPos;
            double x1EndPoint = obj1XPos + obj1.getMovement().xVelocity;
            double x2StartPoint = obj2XPos;
            double x2EndPoint = obj2XPos + obj2.getMovement().xVelocity;
            double y1StartPoint = obj1YPos;
            double y1EndPoint = obj1YPos + obj1.getMovement().yVelocity;
            double y2StartPoint = obj2YPos;
            double y2EndPoint = obj2YPos + obj2.getMovement().yVelocity;
            double y1MidPoint = 0;
            double x1MidPoint = 0;
            double y2MidPoint = 0;
            double x2MidPoint = 0;
            while (!closeEnough && (CurrentStep < MAXSTEP))
            {
                // Take the distances at the start and end points and then take the distance half-way inbetween
                // If this value is closer to the start point in value make it the next end point, else vice versa
                // This will close in on the closest point.
                // Bail out if either a distance of zero or less is hit (a collision point) or if we have made more
                // steps that the defined maxiumum

                // Find the new mid-points
                x1MidPoint = x1StartPoint + ((x1EndPoint - x1StartPoint) / 2);
                y1MidPoint = y1StartPoint + ((y1EndPoint - y1StartPoint) / 2);
                x2MidPoint = x2StartPoint + ((x2EndPoint - x2StartPoint) / 2);
                y2MidPoint = y2StartPoint + ((y2EndPoint - y2StartPoint) / 2);

                // Found out how far apart they are
                newDistance = ((x2MidPoint - x1MidPoint) * (x2MidPoint - x1MidPoint)) +
                              ((y2MidPoint - y1MidPoint) * (y2MidPoint - y1MidPoint));

                if (newDistance <= combinedRadiiSQR)
                {
                    // We have a collision
                    closeEnough = true;
                }
                else
                {
                    CurrentStep++;
                    // Find out if the mid point is closer to collision than the end point or start point
                    if (Math.Abs(newDistance - startPointDistance) < Math.Abs(newDistance - endPointDistance))
                    {
                        endPointDistance = newDistance;
                        x1EndPoint = x1MidPoint;
                        x2EndPoint = x2MidPoint;
                        y1EndPoint = y1MidPoint;
                        y2EndPoint = y2MidPoint;
                    }
                    else
                    {
                        startPointDistance = newDistance;
                        x1StartPoint = x1MidPoint;
                        x2StartPoint = x2MidPoint;
                        y1StartPoint = y1MidPoint;
                        y2StartPoint = y2MidPoint;
                    }
                }
            }

            // If a collision is detected, update the objects to their collision position in order to calculate the correct rebound.
            if (closeEnough)
            {
                obj1.setNextPosition(x1MidPoint, y1MidPoint);
                obj2.setNextPosition(x2MidPoint, y2MidPoint);
            }

            return closeEnough;
        }

        public void rebound(MovingObject obj1, MovingObject obj2)
        {
            // What to do if 2 objects have collided?
            // We need to also assume collisions at an angle
            // obj1's vector is added to obj2's vector proportionate to their relative masses.
            // Momentum is conserved in a collision. The x & y momentum components are also conserved
            // Momentum is mass * velocity


            // The code is from stackexchange, with the variables changed to something better than 'a', 'b', 'c', etc
            // The code for inelastic collisions (where energy is lost) are commented out
            double obj1xPos = obj1.getPosition().xPos;
            double obj1yPos = obj1.getPosition().yPos;
            double obj2xPos = obj2.getPosition().xPos;
            double obj2yPos = obj2.getPosition().yPos;
            double xDist = obj2xPos - obj1xPos;
            double yDist = obj2yPos - obj1yPos;
            double obj1xVel = obj1.getMovement().xVelocity;
            double obj1yVel = obj1.getMovement().yVelocity;
            double obj2xVel = obj2.getMovement().xVelocity;
            double obj2yVel = obj2.getMovement().yVelocity;
            double xVelocity = obj2xVel - obj1xVel;
            double yVelocity = obj2yVel - obj1yVel;
            double dotProduct = (-xDist) * xVelocity + (-yDist) * yVelocity;
            //Neat vector maths, used for checking if the objects move towards one another.
            if (dotProduct > 0)
            {
                double combinedMass;
                double absYDist;
                double sign;
                double collisionWeight;
                double yDivx;
                //vx_cm, vy_cm;

                double mass1 = obj1.getMass();
                double mass2 = obj2.getMass();

                combinedMass = mass2 / mass1;

                // Code for inelastic collisions commented out for now
                //vx_cm = (mass1 * obj1xVel + mass2 * obj2xVel) / (mass1 + mass2);
                //vy_cm = (mass1 * obj1yVel + mass2 * obj2yVel) / (mass1 + mass2);

                // to avoid a zero divide; 
                absYDist = Double.MinValue * Math.Abs(yDist);
                if (Math.Abs(xDist) < absYDist)
                {
                    if (xDist < 0) { sign = -1; } else { sign = 1; }
                    xDist = absYDist * sign;
                }

                //     ***  update velocities ***
                yDivx = yDist / xDist;
                collisionWeight = -2 * (xVelocity + yDivx * yVelocity) / ((1 + yDivx * yDivx) * (1 + combinedMass));
                obj2xVel = obj2xVel + collisionWeight;
                obj2yVel = obj2yVel + yDivx * collisionWeight;
                obj1xVel = obj1xVel - combinedMass * collisionWeight;
                obj1yVel = obj1yVel - yDivx * combinedMass * collisionWeight;

                //     ***  velocity correction for inelastic collisions ***
                //obj1xVel = (obj1xVel - vx_cm)*R + vx_cm;
                //obj1yVel = (obj1yVel - vy_cm)*R + vy_cm;
                //obj2xVel = (obj2xVel - vx_cm)*R + vx_cm;
                //obj2yVel = (obj2yVel - vy_cm)*R + vy_cm;


            }

            // Update velocities and positions
            obj1.setMovement(obj1xVel, obj1yVel);
            obj2.setMovement(obj2xVel, obj2yVel);

            obj1.setNextPosition(obj1.getPosition().xPos + obj1xVel, obj1.getPosition().yPos + obj1yVel);
            obj2.setNextPosition(obj2.getPosition().xPos + obj2xVel, obj2.getPosition().yPos + obj2yVel);

        }


        public int objectOverlaps(MovingObject checkObject) // return -1 for false, else return the number of the overlapping object
        {
            // It is assumed the object being passed as a parameter isn't part of the held list
            for (int i = 0; i < vecOfObjects.Count; ++i)
            {
                // If the distance between the objects is less than their radius, then they are overlapping
                double xDist = vecOfObjects[i].getPosition().xPos - checkObject.getPosition().xPos;
                double yDist = vecOfObjects[i].getPosition().yPos - checkObject.getPosition().yPos;
                double combinedRadius = vecOfObjects[i].getRadius() + checkObject.getRadius();
                if ((xDist * xDist + yDist * yDist) < (combinedRadius * combinedRadius))
                    return i;
            }

            return -1;
        }

    }
}
