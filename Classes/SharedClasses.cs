﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Shared Classes
 * 
 * These are simple wrapper classes around small sets of properties that get passed around
 * 
 * ObjPosition - the x & y co-ords of an object
 * ObjMovement - the x & y velocity of an object.
 *               Includes calculations of total speed and direction.
 *               Also overrides the + operator so 2 movements can be directly added
 * ObjRect     - the bounding edges of an object. Used instead of CRect or similar as all
 *               the existing classes use Ints and I needed doubles.
 * 
 */


namespace CSOrbits.Classes
{
    struct ObjPosition
    {
        public double xPos;
        public double yPos;

        public ObjPosition(ObjPosition copyPosition)
        {
            xPos = copyPosition.xPos;
            yPos = copyPosition.yPos;
        }

        public void assign(ObjPosition newPosition)
        {
            xPos = newPosition.xPos;
            yPos = newPosition.yPos;
        }
    }

    class ObjMovement
    {
        public double xVelocity;
        public double yVelocity;

        public ObjMovement()
        {
            xVelocity = 0;
            yVelocity = 0;
        }

        public double speed()
        {
            return (Math.Sqrt((xVelocity * xVelocity) + (yVelocity * yVelocity)));
        }

        public double direction()
        {
            return (Math.Atan2(yVelocity, xVelocity));
        }

        public void assign(ObjMovement newMovement)
        {
            xVelocity = newMovement.xVelocity;
            yVelocity = newMovement.yVelocity;
        }

        public static ObjMovement operator +(ObjMovement objMov1, ObjMovement objMov2)
        {
            ObjMovement temp = new ObjMovement();
            temp.xVelocity = objMov1.xVelocity + objMov2.xVelocity;
            temp.yVelocity = objMov1.yVelocity + objMov2.yVelocity;
            return temp;
        }
    }

    struct ObjRect
    {
        public double left;
        public double right;
        public double top;
        public double bottom;
    }
}
