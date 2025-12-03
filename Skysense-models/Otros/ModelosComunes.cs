using Skysense_models.APIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_models.Otros
{

    public  class OpcionesCatalogo
    {
        public int iIdOpcion { get; set; }
        public string sOpcion { get; set; } = "";

    }

    public class OpcionesSelect
    {
        public string value { get; set; } = "";
        public string label { get; set; } = "";

    }

    public class MethodResponse<T>
    {
        public MethodResponse(bool Exito, string Mensaje){
            bExito = Exito;
            sMensaje = Mensaje;
            iCodigoError = bExito ? 0 : 1;
        }

        public MethodResponse(bool Exito, string Mensaje, int CodigoError)
        {
            bExito = Exito;
            sMensaje = Mensaje;
            iCodigoError = CodigoError;
        }
        public MethodResponse(bool Exito, string Mensaje, int CodigoError, T Data)
        {
            bExito = Exito;
            sMensaje = Mensaje;
            iCodigoError = CodigoError;
            tData = Data;
        }



        public bool bExito { get; set; }
        public string sMensaje { get; set; }
        public int iCodigoError { get; set; }
        public T tData { get; set; }
    }
}
