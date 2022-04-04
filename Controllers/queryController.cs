using Microsoft.AspNetCore.Mvc;
using bootshellwebapi.Interfaces;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using bootshellwebapi.Models;
namespace bootshellwebapi.Controllers
{
    [Route("rp/[controller]")]
    [ApiController]
    public class queryController : ControllerBase
    {
        private readonly _ISqlCnConfigMain _ISqlCnConfigMain;
        private readonly IConsultaDinamicaSQL _Imobile;
        private readonly ICompanykey _ICompanykey;
        private readonly IOperaciones _Operaciones;
        public queryController(
            _ISqlCnConfigMain _ISqlCnConfigMain_,
            IConsultaDinamicaSQL _Imobile_,
            ICompanykey _ICompanykey_,
            IOperaciones _Operaciones_
        ) {
            _Imobile = _Imobile_;
            _ISqlCnConfigMain = _ISqlCnConfigMain_;
            _ICompanykey = _ICompanykey_;
            _Operaciones = _Operaciones_;
        }

       

        [HttpGet]
        [Route("{origen}/{operacionid}/{companyidkey}/q/{parametros?}")]// o es el parámetro usado en origen de ta tabla OrigenConsulta para determinar que conexion usará,  q es reemplazado por el nombre de la funcion consultasSQL la q de los parametros
        public async Task<JsonResult> consultasSQL(string origen, string operacionid, string companyidkey, string parametros = "") //consultasSQL es reemplazado por la letra q de los parametros puede ser cualquier otra letra es solo para identificar que acá se meterá
        {
             try
             {

          //  var dOp = (await _Operaciones.ObtenerSPFromTblOperacion(_ISqlCnConfigMain, origen, operacionid));



                var dataOperaciones = (await _Operaciones.ObtenerSPFromTblOperacion(_ISqlCnConfigMain, origen, operacionid)).FirstOrDefault();
            


                string spname = dataOperaciones.spname;
                string cadenaconexion = dataOperaciones.cadenaconexion;

                int companyid = (await _ICompanykey.ObtenerCompanyId(_ISqlCnConfigMain, origen, companyidkey)).FirstOrDefault().companyid;

           
                var p = new Dictionary<string, object>();
                string strspparams = "";

            parametros+= ";companyid," + companyid.ToString();

            var lista = parametros.Split(";")
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Split(","))
                .Select(x => Tuple.Create(x[0], x[1]))
                .ToList();



            for(int i = 0, len = lista.Count(); i < len; i++)
            {
                p.Add("@" + lista[i].Item1 , lista[i].Item2.Replace("..", ":"));
                strspparams += "@" + lista[i].Item1 + ",";
            }
            strspparams = strspparams.Remove(strspparams.Length - 1);

           
            var data = await _Imobile.consultaDinamicaSQL(cadenaconexion, spname, strspparams, p);
                return await Task.Run(() => {
                    return new JsonResult(data);
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(BadRequest(ex.Message));
            }

        }


        [HttpGet]
        [Route("{origen}/{operacionidA}/{operacionidB}/{companyidkey}/cal/{Motoristaid}/{tipo}/{parametros?}")]
        public async Task<IActionResult> rpCalificacionMotoristas(string origen, string operacionidA, string operacionidB, string companyidkey, int? Motoristaid = 0, int? tipo = 0, string parametros = "")
        {
            IList<CustomModel.rpCalificacionMotoristas> ilCalificacionMotorista;
            IList<CustomModel.rpCalificacionMotoristas> ilKmMotorista;

            List<CustomModel.ReporteCalificacionConductoresDetalleDia> lRepCalifCondDetalle = new List<CustomModel.ReporteCalificacionConductoresDetalleDia>();
            List<CustomModel.ReporteCalificacionConductoresResumen> lRepCalifCondResumen = new List<CustomModel.ReporteCalificacionConductoresResumen>();

            int companyid = (await _ICompanykey.ObtenerCompanyId(_ISqlCnConfigMain, origen, companyidkey)).FirstOrDefault().companyid;

            var dataOperaciones_A = (await _Operaciones.ObtenerSPFromTblOperacion(_ISqlCnConfigMain, origen, operacionidA)).FirstOrDefault();
            var dataOperaciones_B = (await _Operaciones.ObtenerSPFromTblOperacion(_ISqlCnConfigMain, origen, operacionidB)).FirstOrDefault();

            var p = new Dictionary<string, object>();
            string strspparams = "";

            parametros += ";companyid," + companyid.ToString();

            var lista = parametros.Split(";")
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Split(","))
                .Select(x => Tuple.Create(x[0], x[1]))
                .ToList();

            for (int i = 0, len = lista.Count(); i < len; i++)
            {
                p.Add("@" + lista[i].Item1, lista[i].Item2.Replace("..", ":"));
                strspparams += "@" + lista[i].Item1 + ",";
            }
            strspparams = strspparams.Remove(strspparams.Length - 1);

            bool bExiste = false;
          
            int iTotalResta = 0;
            double iKmEvaluacion = 0;

            string spname_A = dataOperaciones_A.spname,
                   cadenaconexion_A = dataOperaciones_A.cadenaconexion,
                   spname_B = dataOperaciones_B.spname,
                   cadenaconexion_B = dataOperaciones_B.cadenaconexion;

            var ListaOrdenada = (dynamic)null;

            ilCalificacionMotorista = (IList<CustomModel.rpCalificacionMotoristas>)(await _Imobile.consultaDinamicaSQL_calmotorista(cadenaconexion_A, spname_A, strspparams, p)).ToList();

            ilKmMotorista = (IList<CustomModel.rpCalificacionMotoristas>)(await _Imobile.consultaDinamicaSQL_calmotorista(cadenaconexion_B, spname_B, strspparams, p)).ToList();

            if (ilKmMotorista.Count > 0)
            {


                iKmEvaluacion = ilKmMotorista[0].KmEvaluacion;//Para obtener los segmentos de evaluación

                for (int i = 0; i < ilCalificacionMotorista.Count; i++)
                {
                    if (Motoristaid == 0)
                    {
                        if (lRepCalifCondDetalle.Count > 0)
                        {
                            for (int j = 0; j < lRepCalifCondDetalle.Count; j++)
                            {
                                if (lRepCalifCondDetalle[j].Conductorid == ilCalificacionMotorista[i].MotoristaID && Convert.ToDateTime(lRepCalifCondDetalle[j].FechaInfo) == ilCalificacionMotorista[i].FechaInfo)
                                {
                                    bExiste = true;
                                    if (ilCalificacionMotorista[i].VariableID == 1)
                                    {
                                        lRepCalifCondDetalle[j].MaxVelCant += ilCalificacionMotorista[i].CantInfracReal;
                                        lRepCalifCondDetalle[j].MaxVelResta += ilCalificacionMotorista[i].Resta;
                                    }
                                    else
                                    {
                                        if (ilCalificacionMotorista[i].VariableID == 2)
                                        {
                                            lRepCalifCondDetalle[j].FrenadoBruscoCant += ilCalificacionMotorista[i].CantInfracReal;
                                            lRepCalifCondDetalle[j].FrenadoBruscoResta += ilCalificacionMotorista[i].Resta;
                                        }
                                        else
                                        {
                                            if (ilCalificacionMotorista[i].VariableID == 3)
                                            {
                                                lRepCalifCondDetalle[j].RPMCant += ilCalificacionMotorista[i].CantInfracReal;
                                                lRepCalifCondDetalle[j].RPMResta += ilCalificacionMotorista[i].Resta;
                                            }
                                            else
                                            {
                                                if (ilCalificacionMotorista[i].VariableID == 4)
                                                {
                                                    lRepCalifCondDetalle[j].TiempoExcesoCant += ilCalificacionMotorista[i].CantInfracReal;
                                                    lRepCalifCondDetalle[j].TiempoExcesoResta += ilCalificacionMotorista[i].Resta;
                                                }
                                                else
                                                {
                                                    if (ilCalificacionMotorista[i].VariableID == 5)
                                                    {
                                                        lRepCalifCondDetalle[j].TiempoRalentiCant += ilCalificacionMotorista[i].CantInfracReal;
                                                        lRepCalifCondDetalle[j].TiempoRalentiResta += ilCalificacionMotorista[i].Resta;
                                                    }
                                                    else
                                                    {
                                                        if (ilCalificacionMotorista[i].VariableID == 6)
                                                        {
                                                            lRepCalifCondDetalle[j].AceleracionBruscaCant += ilCalificacionMotorista[i].CantInfracReal;
                                                            lRepCalifCondDetalle[j].AceleracionBruscaResta += ilCalificacionMotorista[i].Resta;
                                                        }
                                                        else
                                                        {
                                                            if (ilCalificacionMotorista[i].VariableID == 7)
                                                            {
                                                                lRepCalifCondDetalle[j].MaxVelCallestierraCant += ilCalificacionMotorista[i].CantInfracReal;
                                                                lRepCalifCondDetalle[j].MaxVelCallestierraResta += ilCalificacionMotorista[i].Resta;
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (bExiste == false)
                            {
                                CustomModel.ReporteCalificacionConductoresDetalleDia datos = new CustomModel.ReporteCalificacionConductoresDetalleDia();
                                datos.Conductorid = ilCalificacionMotorista[i].MotoristaID;
                                datos.NombreConductor = ilCalificacionMotorista[i].NombreConductor;
                                datos.FechaInfo = Convert.ToString(ilCalificacionMotorista[i].FechaInfo);
                                datos.CalifIniciodia = 100;
                                if (ilCalificacionMotorista[i].VariableID == 1)
                                {
                                    datos.MaxVelCant += ilCalificacionMotorista[i].CantInfracReal;
                                    datos.MaxVelResta += ilCalificacionMotorista[i].Resta;
                                }
                                else
                                {
                                    if (ilCalificacionMotorista[i].VariableID == 2)
                                    {
                                        datos.FrenadoBruscoCant += ilCalificacionMotorista[i].CantInfracReal;
                                        datos.FrenadoBruscoResta += ilCalificacionMotorista[i].Resta;
                                    }
                                    else
                                    {
                                        if (ilCalificacionMotorista[i].VariableID == 3)
                                        {
                                            datos.RPMCant += ilCalificacionMotorista[i].CantInfracReal;
                                            datos.RPMResta += ilCalificacionMotorista[i].Resta;
                                        }
                                        else
                                        {
                                            if (ilCalificacionMotorista[i].VariableID == 4)
                                            {
                                                datos.TiempoExcesoCant += ilCalificacionMotorista[i].CantInfracReal;
                                                datos.TiempoExcesoResta += ilCalificacionMotorista[i].Resta;
                                            }
                                            else
                                            {
                                                if (ilCalificacionMotorista[i].VariableID == 5)
                                                {
                                                    datos.TiempoRalentiCant += ilCalificacionMotorista[i].CantInfracReal;
                                                    datos.TiempoRalentiResta += ilCalificacionMotorista[i].Resta;
                                                }
                                                else
                                                {
                                                    if (ilCalificacionMotorista[i].VariableID == 6)
                                                    {
                                                        datos.AceleracionBruscaCant += ilCalificacionMotorista[i].CantInfracReal;
                                                        datos.AceleracionBruscaResta += ilCalificacionMotorista[i].Resta;
                                                    }
                                                    else
                                                    {
                                                        if (ilCalificacionMotorista[i].VariableID == 7)
                                                        {
                                                            datos.MaxVelCallestierraCant += ilCalificacionMotorista[i].CantInfracReal;
                                                            datos.MaxVelCallestierraResta += ilCalificacionMotorista[i].Resta;
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                lRepCalifCondDetalle.Add(datos);
                            }
                            else
                            {
                                bExiste = false;
                            }
                        }
                        else
                        {
                            CustomModel.ReporteCalificacionConductoresDetalleDia datos = new CustomModel.ReporteCalificacionConductoresDetalleDia();
                            datos.Conductorid = ilCalificacionMotorista[i].MotoristaID;
                            datos.NombreConductor = ilCalificacionMotorista[i].NombreConductor;
                            datos.FechaInfo = Convert.ToString(ilCalificacionMotorista[i].FechaInfo);
                            datos.CalifIniciodia = 100;
                            if (ilCalificacionMotorista[i].VariableID == 1)
                            {
                                datos.MaxVelCant += ilCalificacionMotorista[i].CantInfracReal;
                                datos.MaxVelResta += ilCalificacionMotorista[i].Resta;
                            }
                            else
                            {
                                if (ilCalificacionMotorista[i].VariableID == 2)
                                {
                                    datos.FrenadoBruscoCant += ilCalificacionMotorista[i].CantInfracReal;
                                    datos.FrenadoBruscoResta += ilCalificacionMotorista[i].Resta;
                                }
                                else
                                {
                                    if (ilCalificacionMotorista[i].VariableID == 3)
                                    {
                                        datos.RPMCant += ilCalificacionMotorista[i].CantInfracReal;
                                        datos.RPMResta += ilCalificacionMotorista[i].Resta;
                                    }
                                    else
                                    {
                                        if (ilCalificacionMotorista[i].VariableID == 4)
                                        {
                                            datos.TiempoExcesoCant += ilCalificacionMotorista[i].CantInfracReal;
                                            datos.TiempoExcesoResta += ilCalificacionMotorista[i].Resta;
                                        }
                                        else
                                        {
                                            if (ilCalificacionMotorista[i].VariableID == 5)
                                            {
                                                datos.TiempoRalentiCant += ilCalificacionMotorista[i].CantInfracReal;
                                                datos.TiempoRalentiResta += ilCalificacionMotorista[i].Resta;
                                            }
                                            else
                                            {
                                                if (ilCalificacionMotorista[i].VariableID == 6)
                                                {
                                                    datos.AceleracionBruscaCant += ilCalificacionMotorista[i].CantInfracReal;
                                                    datos.AceleracionBruscaResta += ilCalificacionMotorista[i].Resta;
                                                }
                                                else
                                                {
                                                    if (ilCalificacionMotorista[i].VariableID == 7)
                                                    {
                                                        datos.MaxVelCallestierraCant += ilCalificacionMotorista[i].CantInfracReal;
                                                        datos.MaxVelCallestierraResta += ilCalificacionMotorista[i].Resta;
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            lRepCalifCondDetalle.Add(datos);
                        }
                    }
                    else
                    {
                        if (ilCalificacionMotorista[i].MotoristaID == Motoristaid)
                        {
                            if (lRepCalifCondDetalle.Count > 0)
                            {
                                for (int j = 0; j < lRepCalifCondDetalle.Count; j++)
                                {
                                    if (lRepCalifCondDetalle[j].Conductorid == ilCalificacionMotorista[i].MotoristaID && Convert.ToDateTime(lRepCalifCondDetalle[j].FechaInfo) == ilCalificacionMotorista[i].FechaInfo)
                                    {
                                        bExiste = true;
                                        if (ilCalificacionMotorista[i].VariableID == 1)
                                        {
                                            lRepCalifCondDetalle[j].MaxVelCant += ilCalificacionMotorista[i].CantInfracReal;
                                            lRepCalifCondDetalle[j].MaxVelResta += ilCalificacionMotorista[i].Resta;
                                        }
                                        else
                                        {
                                            if (ilCalificacionMotorista[i].VariableID == 2)
                                            {
                                                lRepCalifCondDetalle[j].FrenadoBruscoCant += ilCalificacionMotorista[i].CantInfracReal;
                                                lRepCalifCondDetalle[j].FrenadoBruscoResta += ilCalificacionMotorista[i].Resta;
                                            }
                                            else
                                            {
                                                if (ilCalificacionMotorista[i].VariableID == 3)
                                                {
                                                    lRepCalifCondDetalle[j].RPMCant += ilCalificacionMotorista[i].CantInfracReal;
                                                    lRepCalifCondDetalle[j].RPMResta += ilCalificacionMotorista[i].Resta;
                                                }
                                                else
                                                {
                                                    if (ilCalificacionMotorista[i].VariableID == 4)
                                                    {
                                                        lRepCalifCondDetalle[j].TiempoExcesoCant += ilCalificacionMotorista[i].CantInfracReal;
                                                        lRepCalifCondDetalle[j].TiempoExcesoResta += ilCalificacionMotorista[i].Resta;
                                                    }
                                                    else
                                                    {
                                                        if (ilCalificacionMotorista[i].VariableID == 5)
                                                        {
                                                            lRepCalifCondDetalle[j].TiempoRalentiCant += ilCalificacionMotorista[i].CantInfracReal;
                                                            lRepCalifCondDetalle[j].TiempoRalentiResta += ilCalificacionMotorista[i].Resta;
                                                        }
                                                        else
                                                        {
                                                            if (ilCalificacionMotorista[i].VariableID == 6)
                                                            {
                                                                lRepCalifCondDetalle[j].AceleracionBruscaCant += ilCalificacionMotorista[i].CantInfracReal;
                                                                lRepCalifCondDetalle[j].AceleracionBruscaResta += ilCalificacionMotorista[i].Resta;
                                                            }
                                                            else
                                                            {
                                                                if (ilCalificacionMotorista[i].VariableID == 7)
                                                                {
                                                                    lRepCalifCondDetalle[j].MaxVelCallestierraCant += ilCalificacionMotorista[i].CantInfracReal;
                                                                    lRepCalifCondDetalle[j].MaxVelCallestierraResta += ilCalificacionMotorista[i].Resta;
                                                                }

                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (bExiste == false)
                                {
                                    CustomModel.ReporteCalificacionConductoresDetalleDia datos = new CustomModel.ReporteCalificacionConductoresDetalleDia();
                                    datos.Conductorid = ilCalificacionMotorista[i].MotoristaID;
                                    datos.NombreConductor = ilCalificacionMotorista[i].NombreConductor;
                                    datos.FechaInfo = Convert.ToString(ilCalificacionMotorista[i].FechaInfo);
                                    datos.CalifIniciodia = 100;
                                    if (ilCalificacionMotorista[i].VariableID == 1)
                                    {
                                        datos.MaxVelCant += ilCalificacionMotorista[i].CantInfracReal;
                                        datos.MaxVelResta += ilCalificacionMotorista[i].Resta;
                                    }
                                    else
                                    {
                                        if (ilCalificacionMotorista[i].VariableID == 2)
                                        {
                                            datos.FrenadoBruscoCant += ilCalificacionMotorista[i].CantInfracReal;
                                            datos.FrenadoBruscoResta += ilCalificacionMotorista[i].Resta;
                                        }
                                        else
                                        {
                                            if (ilCalificacionMotorista[i].VariableID == 3)
                                            {
                                                datos.RPMCant += ilCalificacionMotorista[i].CantInfracReal;
                                                datos.RPMResta += ilCalificacionMotorista[i].Resta;
                                            }
                                            else
                                            {
                                                if (ilCalificacionMotorista[i].VariableID == 4)
                                                {
                                                    datos.TiempoExcesoCant += ilCalificacionMotorista[i].CantInfracReal;
                                                    datos.TiempoExcesoResta += ilCalificacionMotorista[i].Resta;
                                                }
                                                else
                                                {
                                                    if (ilCalificacionMotorista[i].VariableID == 5)
                                                    {
                                                        datos.TiempoRalentiCant += ilCalificacionMotorista[i].CantInfracReal;
                                                        datos.TiempoRalentiResta += ilCalificacionMotorista[i].Resta;
                                                    }
                                                    else
                                                    {
                                                        if (ilCalificacionMotorista[i].VariableID == 6)
                                                        {
                                                            datos.AceleracionBruscaCant += ilCalificacionMotorista[i].CantInfracReal;
                                                            datos.AceleracionBruscaResta += ilCalificacionMotorista[i].Resta;
                                                        }
                                                        else
                                                        {
                                                            if (ilCalificacionMotorista[i].VariableID == 7)
                                                            {
                                                                datos.MaxVelCallestierraCant += ilCalificacionMotorista[i].CantInfracReal;
                                                                datos.MaxVelCallestierraResta += ilCalificacionMotorista[i].Resta;
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    lRepCalifCondDetalle.Add(datos);
                                }
                                else
                                {
                                    bExiste = false;
                                }
                            }
                            else
                            {
                                CustomModel.ReporteCalificacionConductoresDetalleDia datos = new CustomModel.ReporteCalificacionConductoresDetalleDia();
                                datos.Conductorid = ilCalificacionMotorista[i].MotoristaID;
                                datos.NombreConductor = ilCalificacionMotorista[i].NombreConductor;
                                datos.FechaInfo = Convert.ToString(ilCalificacionMotorista[i].FechaInfo);
                                datos.CalifIniciodia = 100;
                                if (ilCalificacionMotorista[i].VariableID == 1)
                                {
                                    datos.MaxVelCant += ilCalificacionMotorista[i].CantInfracReal;
                                    datos.MaxVelResta += ilCalificacionMotorista[i].Resta;
                                }
                                else
                                {
                                    if (ilCalificacionMotorista[i].VariableID == 2)
                                    {
                                        datos.FrenadoBruscoCant += ilCalificacionMotorista[i].CantInfracReal;
                                        datos.FrenadoBruscoResta += ilCalificacionMotorista[i].Resta;
                                    }
                                    else
                                    {
                                        if (ilCalificacionMotorista[i].VariableID == 3)
                                        {
                                            datos.RPMCant += ilCalificacionMotorista[i].CantInfracReal;
                                            datos.RPMResta += ilCalificacionMotorista[i].Resta;
                                        }
                                        else
                                        {
                                            if (ilCalificacionMotorista[i].VariableID == 4)
                                            {
                                                datos.TiempoExcesoCant += ilCalificacionMotorista[i].CantInfracReal;
                                                datos.TiempoExcesoResta += ilCalificacionMotorista[i].Resta;
                                            }
                                            else
                                            {
                                                if (ilCalificacionMotorista[i].VariableID == 5)
                                                {
                                                    datos.TiempoRalentiCant += ilCalificacionMotorista[i].CantInfracReal;
                                                    datos.TiempoRalentiResta += ilCalificacionMotorista[i].Resta;
                                                }
                                                else
                                                {
                                                    if (ilCalificacionMotorista[i].VariableID == 6)
                                                    {
                                                        datos.AceleracionBruscaCant += ilCalificacionMotorista[i].CantInfracReal;
                                                        datos.AceleracionBruscaResta += ilCalificacionMotorista[i].Resta;
                                                    }
                                                    else
                                                    {
                                                        if (ilCalificacionMotorista[i].VariableID == 7)
                                                        {
                                                            datos.MaxVelCallestierraCant += ilCalificacionMotorista[i].CantInfracReal;
                                                            datos.MaxVelCallestierraResta += ilCalificacionMotorista[i].Resta;
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                lRepCalifCondDetalle.Add(datos);
                            }
                        }
                    }
                }

                for (int f = 0; f < lRepCalifCondDetalle.Count; f++)
                {
                    iTotalResta = lRepCalifCondDetalle[f].MaxVelResta + lRepCalifCondDetalle[f].FrenadoBruscoResta + lRepCalifCondDetalle[f].RPMResta + lRepCalifCondDetalle[f].TiempoExcesoResta + lRepCalifCondDetalle[f].TiempoRalentiResta + lRepCalifCondDetalle[f].AceleracionBruscaResta;
                    lRepCalifCondDetalle[f].Califfindia = iTotalResta;
                }

                //Creando el resumen del mes
                for (int l = 0; l < lRepCalifCondDetalle.Count; l++)
                {
                    CustomModel.ReporteCalificacionConductoresResumen datosResumen = new CustomModel.ReporteCalificacionConductoresResumen();
                    datosResumen.Conductorid = lRepCalifCondDetalle[l].Conductorid;
                    datosResumen.NombreConductor = lRepCalifCondDetalle[l].NombreConductor;
                    datosResumen.FechaInfo = Convert.ToDateTime(lRepCalifCondDetalle[l].FechaInfo).ToShortDateString();
                    datosResumen.Califfindia = lRepCalifCondDetalle[l].Califfindia;
                    lRepCalifCondResumen.Add(datosResumen);
                }
                //Agregando el km del día por motorista
                for (int k = 0; k < ilKmMotorista.Count; k++)
                {
                    for (int i = 0; i < lRepCalifCondResumen.Count; i++)
                    {
                        if (lRepCalifCondResumen[i].Conductorid == ilKmMotorista[k].MotoristaID)
                        {
                            if (lRepCalifCondResumen[i].FechaInfo == ilKmMotorista[k].DateInfo.ToShortDateString())
                            {
                                lRepCalifCondResumen[i].Km += ilKmMotorista[k].Kilometraje;
                            }
                        }
                    }
                }

                //Para obtener el promedio por cada segmento y el final.
                double kmrecorrido = 0;
                int iTotalPuntosaRestar = 0;
                int iConductor = 0;
                double dbEvalucionPorSegmento;
                ListaOrdenada = lRepCalifCondResumen.OrderBy(c => c.Conductorid).ToList();
                for (int i = 0; i < ListaOrdenada.Count; i++)
                {
                    if (i == 0)
                    {
                        //Tomando el primer registro:
                        iConductor = ListaOrdenada[i].Conductorid;
                        kmrecorrido = ListaOrdenada[i].Km;
                        iTotalPuntosaRestar = ListaOrdenada[i].Califfindia;
                        if (kmrecorrido >= iKmEvaluacion)
                        {
                            //dbEvalucionPorSegmento = 100 - (iTotalPuntosaRestar / kmrecorrido) * 100;
                            dbEvalucionPorSegmento = 100 - iTotalPuntosaRestar;
                            //if (dbEvalucionPorSegmento < 100)
                            if (dbEvalucionPorSegmento < 0)
                            {
                                dbEvalucionPorSegmento = 0;
                            }
                            ListaOrdenada[i].EvalPromedioSeg = dbEvalucionPorSegmento;
                            kmrecorrido = 0;
                            iTotalPuntosaRestar = 0;
                        }
                        else
                        {
                            ListaOrdenada[i].EvalPromedioSeg = null;
                        }
                    }
                    else
                    {
                        if (iConductor == ListaOrdenada[i].Conductorid)
                        {
                            kmrecorrido += ListaOrdenada[i].Km;
                            iTotalPuntosaRestar += ListaOrdenada[i].Califfindia;
                            if (kmrecorrido >= iKmEvaluacion)
                            {
                                //dbEvalucionPorSegmento = 100 - (iTotalPuntosaRestar / kmrecorrido) * 100;
                                dbEvalucionPorSegmento = 100 - iTotalPuntosaRestar;
                                if (dbEvalucionPorSegmento < 0)
                                {
                                    dbEvalucionPorSegmento = 0;
                                }
                                ListaOrdenada[i].EvalPromedioSeg = dbEvalucionPorSegmento;
                                kmrecorrido = 0;
                                iTotalPuntosaRestar = 0;
                            }
                            else
                            {
                                ListaOrdenada[i].EvalPromedioSeg = null;
                            }
                        }
                        else
                        {
                            if (kmrecorrido > 0)
                            {
                                //dbEvalucionPorSegmento = 100 - (iTotalPuntosaRestar / kmrecorrido) * 100;
                                dbEvalucionPorSegmento = 100 - iTotalPuntosaRestar;
                                if (dbEvalucionPorSegmento < 0)
                                {
                                    dbEvalucionPorSegmento = 0;
                                }
                                ListaOrdenada[i - 1].EvalPromedioSeg = dbEvalucionPorSegmento;
                                kmrecorrido = 0;
                                iTotalPuntosaRestar = 0;
                            }
                            kmrecorrido = ListaOrdenada[i].Km;
                            iTotalPuntosaRestar = ListaOrdenada[i].Califfindia;
                            if (kmrecorrido >= iKmEvaluacion)
                            {
                                // dbEvalucionPorSegmento = 100 - (iTotalPuntosaRestar / kmrecorrido) * 100;
                                dbEvalucionPorSegmento = 100 - iTotalPuntosaRestar;
                                if (dbEvalucionPorSegmento < 0)
                                {
                                    dbEvalucionPorSegmento = 0;
                                }
                                ListaOrdenada[i].EvalPromedioSeg = dbEvalucionPorSegmento;
                                kmrecorrido = 0;
                                iTotalPuntosaRestar = 0;
                            }
                            else
                            {
                                ListaOrdenada[i].EvalPromedioSeg = null;
                            }
                        }
                    }
                    iConductor = ListaOrdenada[i].Conductorid;
                }
                if (kmrecorrido > 0)
                {
                    //dbEvalucionPorSegmento = 100 - (iTotalPuntosaRestar / kmrecorrido) * 100;
                    dbEvalucionPorSegmento = 100 - iTotalPuntosaRestar;
                    if (dbEvalucionPorSegmento < 0)
                    {
                        dbEvalucionPorSegmento = 100;
                    }
                    ListaOrdenada[ListaOrdenada.Count - 1].EvalPromedioSeg = dbEvalucionPorSegmento;
                }

                //Session["lRepCalifCondResumen"] = ListaOrdenada;
            }

            if (tipo == 0)
            {
                return await Task.Run(() => {
                    return new JsonResult(lRepCalifCondDetalle);
                });
            }
            else
            {
                return await Task.Run(() => {
                    return new JsonResult(ListaOrdenada);
                });
                
            }

            

        }

    }
   
}

