using System;

namespace bootshellwebapi.Models
{
    public class CustomModel
    {
        public class rpCalificacionMotoristas
        {
            #region variables
            private DateTime fechainfo;
            private int variableid;
            private int motoristaid;
            private string nombre;
            private string descripcion;
            private int cantidadinfracciones;
            private int resta;
            private int mobileid;
            private int vecesinfraccion;
            private double valorinfraccion;
            private int sancioncalificacion;
            private DateTime dateinfo;//Para fecha de km diario motorista
            private double km;
            private double kmevaluacion;
            private int colorid;
            private string colorname;
            private int valor;

            public virtual DateTime FechaInfo
            {
                get { return fechainfo; }
                set { fechainfo = value; }
            }
            public virtual int VariableID
            {
                get { return variableid; }
                set { variableid = value; }
            }
            public virtual int MotoristaID
            {
                get { return motoristaid; }
                set { motoristaid = value; }
            }
            public virtual string NombreConductor
            {
                get { return nombre; }
                set { nombre = value; }
            }
            public virtual string Variable
            {
                get { return descripcion; }
                set { descripcion = value; }
            }
            public virtual int CantInfracReal
            {
                get { return cantidadinfracciones; }
                set { cantidadinfracciones = value; }
            }
            public virtual int Resta
            {
                get { return resta; }
                set { resta = value; }
            }
            public virtual int Mobileid
            {
                get { return mobileid; }
                set { mobileid = value; }
            }
            public virtual int VecesInfraccionRegla
            {
                get { return vecesinfraccion; }
                set { vecesinfraccion = value; }
            }
            public virtual double ValorInfraccionRegla
            {
                get { return valorinfraccion; }
                set { valorinfraccion = value; }
            }
            public virtual int SancionCalificacionRegla
            {
                get { return sancioncalificacion; }
                set { sancioncalificacion = value; }
            }
            public virtual DateTime DateInfo
            {
                get { return dateinfo; }
                set { dateinfo = value; }
            }
            public virtual double Kilometraje
            {
                get { return km; }
                set { km = value; }
            }
            public virtual double KmEvaluacion
            {
                get { return kmevaluacion; }
                set { kmevaluacion = value; }
            }
            public virtual int Color
            {
                get { return colorid; }
                set { colorid = value; }
            }
            public virtual int ValorColor
            {
                get { return valor; }
                set { valor = value; }
            }
            public virtual string NombreColor
            {
                get { return colorname; }
                set { colorname = value; }
            }
            #endregion
        }
        public class ReporteCalificacionConductoresDetalleDia
        {
            public int Conductorid { get; set; }
            public string NombreConductor { get; set; }
            public string FechaInfo { get; set; }
            public int CalifIniciodia { get; set; }
            public int MaxVelCant { get; set; }
            public int MaxVelResta { get; set; }
            public int FrenadoBruscoCant { get; set; }
            public int FrenadoBruscoResta { get; set; }
            public int RPMCant { get; set; }
            public int RPMResta { get; set; }
            public int TiempoExcesoCant { get; set; }
            public int TiempoExcesoResta { get; set; }
            public int TiempoRalentiCant { get; set; }
            public int TiempoRalentiResta { get; set; }
            public int AceleracionBruscaCant { get; set; }
            public int AceleracionBruscaResta { get; set; }
            public int Califfindia { get; set; }
            public int MaxVelCallestierraCant { get; set; }
            public int MaxVelCallestierraResta { get; set; }
        }
        public class ReporteCalificacionConductoresResumen
        {
            public int Conductorid { get; set; }
            public string NombreConductor { get; set; }
            public string FechaInfo { get; set; }
            public int Califfindia { get; set; }
            public double? EvalPromedioSeg { get; set; }
            public double Km { get; set; }
        }

    }
}
