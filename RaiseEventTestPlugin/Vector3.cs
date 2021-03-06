﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    public class Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }

        public Vector3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        //Operator Overloads

        static public Vector3 operator -(Vector3 pointA, Vector3 pointB)
        {
            return new Vector3(pointA.x - pointB.x, pointA.y - pointB.y, pointA.z - pointB.z);
        }

        static public Vector3 operator +(Vector3 pointA, Vector3 pointB)
        {
            return new Vector3(pointA.x + pointB.x, pointA.y + pointB.y, pointA.z + pointB.z);
        }
    }
}
