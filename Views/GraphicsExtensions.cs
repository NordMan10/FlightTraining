using FlightTraining.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FlightTraining.Views
{
    public static class GraphicsExtensions
    {
        private readonly static Pen layoutPen = new Pen(Color.Gray, 1);
        public readonly static SolidBrush PlanePointsBrush = new SolidBrush(Color.FromArgb(0x0, 0x92, 0x3F));
        public readonly static SolidBrush UmvPointsBrush = new SolidBrush(Color.FromArgb(0x0, 0x7C, 0xC3));
        private readonly static Pen aircraftPointBorderPen = new Pen(Color.Black, 4);
        private readonly static Pen restrZonePointBorderPen = new Pen(Color.Black, 2);
        private readonly static Pen restrZoneBorderPen = new Pen(Color.Black, 2);
        private readonly static SolidBrush restrZonePointBrush = new SolidBrush(Color.FromArgb(0xE7, 0x78, 0x17));
        private readonly static SolidBrush restrZoneBrush = new SolidBrush(Color.FromArgb(0xEA, 0xCE, 0x0));
        public readonly static Pen PlaneTrajectoryPen = new Pen(Color.FromArgb(0x0, 0x92, 0x3F), 2);
        public readonly static Pen UmvTrajectoryPen = new Pen(Color.FromArgb(0x0, 0x7C, 0xC3), 2);
        public readonly static Pen OrdinaryPen = new Pen(Color.Black, 2);
        public readonly static SolidBrush OrdinaryBrush = new SolidBrush(Color.White);

        private static void DrawTrianglePoint(this Graphics g, int x, int y, SolidBrush brush)
        {
            var coords = new Point[]
            {
                new Point(x - 7, y + 4),
                new Point(x, y - 8),
                new Point(x + 7, y + 4),
            };

            g.DrawPolygon(aircraftPointBorderPen, coords);
            g.FillPolygon(brush, coords);
        }

        public static void DrawRestrZone(this Graphics g, List<IThreeDPoint> points)
        {
            var leftTopPoint = points.Where(point => point.Id == 0).ToList()[0];
            var rightBottomPoint = points.Where(point => point.Id == 3).ToList()[0];

            g.DrawRectangle(restrZoneBorderPen, new Rectangle(leftTopPoint.X, leftTopPoint.Y, rightBottomPoint.X - leftTopPoint.X, rightBottomPoint.Y - leftTopPoint.Y));
            g.FillRectangle(restrZoneBrush, new Rectangle(leftTopPoint.X, leftTopPoint.Y, rightBottomPoint.X - leftTopPoint.X, rightBottomPoint.Y - leftTopPoint.Y));

            for (var i = 0; i < points.Count; i++)
            {
                var rect = new Rectangle(points[i].X - ProgramOptions.RectPointDiameter / 2, points[i].Y - ProgramOptions.RectPointDiameter / 2, 
                    ProgramOptions.RectPointDiameter, ProgramOptions.RectPointDiameter);
                g.DrawEllipse(restrZonePointBorderPen, rect);
                g.FillEllipse(restrZonePointBrush, rect);
            }
        }

        public static void DrawLayout(this Graphics g, List<Tuple<Point, Point>> xPoints, List<Tuple<Point, Point>> yPoints)
        {
            for (var i = 0; i < xPoints.Count; i++)
            {
                g.DrawLine(layoutPen, xPoints[i].Item1, xPoints[i].Item2);
            }

            for (var i = 0; i < yPoints.Count; i++)
            {
                g.DrawLine(layoutPen, yPoints[i].Item1, yPoints[i].Item2);
            }
        }

        public static void DrawAircraftPoints(this Graphics g, List<IThreeDPoint> points, SolidBrush brush)
        {
            for (var i = 0; i < points.Count; i++)
            {
                DrawTrianglePoint(g, points[i].X, points[i].Y, brush);
            }
        }

        public static void DrawPlaneTrajectories(this Graphics g, List<Tuple<Point, Point>> pointsPairs, Pen pen)
        {
            for (var i = 0; i < pointsPairs.Count; i++)
                g.DrawLine(pen, pointsPairs[i].Item1, pointsPairs[i].Item2);
        }

        public static void DrawAircrafts(this Graphics g, Dictionary<int, IAircraft> aircrafts)
        {
            foreach (var aircraft in aircrafts.Values)
            {
                g.DrawImage(aircraft.Image, (float)(aircraft.X - aircraft.ImageSize / 2),
                    (float)(aircraft.Y - aircraft.ImageSize / 2), aircraft.ImageSize,
                    aircraft.ImageSize);
            }
        }

        public static void DrawRoundPoint(this Graphics g, Rectangle rect, Pen pen, SolidBrush brush)
        {
            g.DrawEllipse(pen, rect);
            g.FillEllipse(brush, rect);
        }
    }
}
