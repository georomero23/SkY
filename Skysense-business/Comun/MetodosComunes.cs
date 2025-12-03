using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_business.Comun
{
    public static class MetodosComunes
    {
        static Random aleatoriedad = new Random();

        public static string GeneraCodigoAleatorio(byte longitud)
        {
            const string caracteresValidos = "ABCDEFGHJKMNPQRTUVWXYZabcdefghjkmnpqrstuvwxyz0123456789";

            string codigo = "";


            do
            {
                codigo += caracteresValidos[aleatoriedad.Next(caracteresValidos.Length)];
            } while (codigo.Length < longitud);

            return codigo;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static bool FechasIguales(DateTime f1, DateTime f2)
        {
            return f1.Year == f2.Year && f2.Month == f1.Month && f1.Day == f2.Day;
        }

        public static long DateTimeToUnixTimeStampMS(DateTime f)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(f, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
        }
    }
}
