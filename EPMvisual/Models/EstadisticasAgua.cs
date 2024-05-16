using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class Cliente
{
    public int ClienteId { get; set; }
    public double ConsumoAgua { get; set; }
    public double ConsumoEnergia { get; set; }
    public double PromedioAgua { get; set; }
    public double MetaAhorro { get; set; }
}


namespace EPMvisual.Models
{
    public class EstadisticasAgua
    {
        private static string connectionString = "data source=DESKTOP-US30RFT;initial catalog=EPM;integrated security=True;trustservercertificate=True;";

        public static double CalcularExcesoAgua()
        {
            List<Cliente> clientes = ObtenerClientes();
            double contConsumoagua = 0;

            foreach (Cliente cliente in clientes)
            {
                contConsumoagua += cliente.ConsumoAgua;
            }
            double promedioConsumoagua = contConsumoagua / clientes.Count;
            double aguaMayorpromedio = 0;

            foreach (Cliente cliente in clientes)
            {
                if (cliente.ConsumoAgua > promedioConsumoagua)
                {
                    aguaMayorpromedio += cliente.ConsumoAgua - promedioConsumoagua;
                }
            }
            return aguaMayorpromedio;
        }

        public static int ContabilizarClientesMayorPromedioAgua()
        {
            List<Cliente> clientes = ObtenerClientes();
            int contCLientesMayorPromedio = 0;

            foreach (Cliente cliente in clientes)
            {
                if (cliente.ConsumoAgua > cliente.PromedioAgua)
                {
                    contCLientesMayorPromedio += 1;
                }
            }
            return contCLientesMayorPromedio;
        }

        private static List<Cliente> ObtenerClientes()
        {
            List<Cliente> clientes = new List<Cliente>();
            string query = "SELECT cedulaCliente, consumoAgua, consumoEnergia, promedioConsumoAgua, metaAhorroEnergia FROM clientes";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Cliente cliente = new Cliente
                    {
                        ClienteId = Convert.ToInt32(reader["cedulaCliente"]),
                        ConsumoAgua = Convert.ToDouble(reader["consumoAgua"]),
                        ConsumoEnergia = Convert.ToDouble(reader["consumoEnergia"]),
                        PromedioAgua = Convert.ToDouble(reader["promedioConsumoAgua"]),
                        MetaAhorro = Convert.ToDouble(reader["metaAhorroEnergia"])
                    };
                    clientes.Add(cliente);
                }
            }
            return clientes;
        }

        public static List<double> CalcularPorcentajesConsumoAguaPorEstrato()
        {
            List<double> porcentajes = new List<double>();

            string query = @"
        SELECT estratoCliente, 
               SUM(consumoAgua) AS consumoTotal, 
               SUM(CASE WHEN consumoAgua > promedioConsumoAgua THEN consumoAgua - promedioConsumoAgua ELSE 0 END) AS 
        FROM consumoAgua
        GROUP BY estratoCliente";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    double consumoTotal = Convert.ToDouble(reader["consumoTotal"]);
                    double excesoConsumo = Convert.ToDouble(reader["excesoConsumo"]);
                    double porcentaje = (excesoConsumo / consumoTotal) * 100;
                    porcentajes.Add(porcentaje);
                }
            }

            return porcentajes;
        }

        public static int ObtenerEstratoMayorAhorroAgua()
        {
            int estratoMayorAhorro = 0;

            string query = @"
        SELECT TOP 1 estratoCliente
        FROM clientes
        WHERE promedioConsumoAgua > consumoAgua
        GROUP BY estratoCliente
        ORDER BY COUNT(*) DESC";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    estratoMayorAhorro = Convert.ToInt32(reader["estratoCliente"]);
                }
            }

            return estratoMayorAhorro;
        }

        public static Cliente ObtenerMayorConsumoDeAguaPorPeriodoDeConsumo(int periodo)
        {
            Cliente clienteMayorConsumoPeriodo = null;

            string query = @"
        SELECT *
        FROM clientes
        WHERE periodoConsumoCliente = @Periodo
        ORDER BY ConsumoAgua DESC
        OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Periodo", periodo);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    clienteMayorConsumoPeriodo = new Cliente
                    {
                        ClienteId = Convert.ToInt32(reader["cedulaCliente"]),
                        
                    };
                }
            }

            return clienteMayorConsumoPeriodo;
        }

    }
}