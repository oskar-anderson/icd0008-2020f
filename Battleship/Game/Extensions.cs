using System;
using System.Collections.Generic;
using RogueSharp;

namespace Game
{
    public static class RectangleExtensions
    {
        public static List<Point> ToPoints(this Rectangle rect)
        {
            List<Point> rectAsPoints = new List<Point>();
            for (int x = rect.X; x < rect.Right; x++)
            {
                for (int y = rect.Y; y < rect.Bottom; y++)
                {
                    rectAsPoints.Add(new Point(x, y));
                }
            }

            return rectAsPoints;
        }

        public static List<Point> ToHitboxPoints(this Rectangle rect, int rule)
        {
            List<Point> rectAsPoints;
            switch (rule)
            {
                case 0:
                    rectAsPoints = ToPoints(rect);
                    break;
                case 1:
                    Rectangle boundingRect = new Rectangle(
                        rect.Left - 1,rect.Top - 1, rect.Width + 2, rect.Height + 2);
                    rectAsPoints = ToPoints(boundingRect);
                    rectAsPoints.Remove(new Point(rect.Left - 1, rect.Top - 1));
                    rectAsPoints.Remove(new Point(rect.Right, rect.Top - 1));
                    rectAsPoints.Remove(new Point(rect.Right, rect.Bottom));
                    rectAsPoints.Remove(new Point(rect.Left - 1, rect.Bottom));
                    break;
                case 2:
                    rectAsPoints = ToPoints(new Rectangle(
                        rect.Left - 1, rect.Top - 1, rect.Width + 2, rect.Height + 2));
                    break;
                default:
                    throw new Exception("Unexpected!");
            }

            return rectAsPoints;
        }
    }
    
    public static class ArrayExtensions
    {
        public static T Get<T>(this T[,] board, Point p)
        {
            return board[p.Y, p.X];
        }

        public static void Set<T>(this T[,] board, Point p, T value)
        {
            board[p.Y, p.X] = value;
        }
        
        // https://stackoverflow.com/questions/3010219/jagged-arrays-multidimensional-arrays-conversion-in-asp-net
        public static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
        {
            int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
            int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            int numberOfRows = rowsLastIndex + 1;

            int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex + 1;

            T[][] jaggedArray = new T[numberOfRows][];
            for (int i = rowsFirstIndex; i <= rowsLastIndex; i++)
            {
                jaggedArray[i] = new T[numberOfColumns];

                for (int j = columnsFirstIndex; j <= columnsLastIndex; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }
            return jaggedArray;
        }
        
        public static int[,] ConvertTo2DArray(this int[][] jaggedArray, int numOfColumns, int numOfRows)
        {
            int[,] temp2DArray = new int[numOfColumns, numOfRows];
            
            for (int c = 0; c < numOfColumns; c++)
            {
                for (int r = 0; r < numOfRows; r++)
                {
                    temp2DArray[c, r] = jaggedArray[c][r];
                }
            }

            return temp2DArray;
        } 
    }
}