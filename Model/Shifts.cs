

namespace FlightTraining.Model
{
    public class Shifts
    {
        public Shifts() { Dx = Dy = Dz = 0; }

        public Shifts(double dx, double dy, double dz)
        {
            Dx = dx;
            Dy = dy;
            Dz = dz;
        }

        public double Dx { get; set; }

        public double Dy { get; set; }

        public double Dz { get; set; }
    }
}
