using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTraining.Model
{
    public static class Convertation
    {
        /// <summary>
        /// Конвертирует координаты x, y, z из системы координат данной нам схемы в систему координат программы 
        /// с переносом направления осей и ковертацией метров в пиксели.
        /// </summary>
        /// <returns></returns>
        public static Tuple<int, int, int> ConvertFromSchemeToProgram(int x, int y, int z)
        {
            return Tuple.Create(ConvertMetersToPixels(z + ProgramOptions.XShiftForConvertation),
                ConvertMetersToPixels(ProgramOptions.TopFieldBorder - x), y);
        }

        public static int ConvertMetersToPixels(double meters)
        {
            return (int)(meters / ProgramOptions.MetersInCell * ProgramOptions.PixelsInCell);
        }

        public static int ConvertPixelsToMeters(double pixels)
        {
            return (int)(pixels / ProgramOptions.PixelsInCell * ProgramOptions.MetersInCell);
        }
    }
}
