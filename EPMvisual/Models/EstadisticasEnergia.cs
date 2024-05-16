using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EPMvisual.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public double ConsumoAgua { get; set; }
        public double ConsumoEnergia { get; set; }
        public double PromedioAgua { get; set; }
        public double MetaAhorro { get; set; }
    }

    public class Estadisticas
    {
        private static string connectionString = "data source=DESKTOP-US30RFT;initial catalog=EPM;integrated security=True;trustservercertificate=True;";

        public static double CalcularPromedioConsumoEnergia()
        {
            List<Cliente> clientes = ObtenerClientes();
            double sumatoriaConsumoEnergia = 0;
            foreach (Cliente cliente in clientes)
            {
                sumatoriaConsumoEnergia += cliente.ConsumoEnergia;
            }
            double promedioConsumoGeneral = sumatoriaConsumoEnergia / clientes.Count;
            return promedioConsumoGeneral;
        }

        public static double CalcularTotalDescuentos()
        {
            List<Cliente> clientes = ObtenerClientes();
            double valorKilovatio = ObtenerValorKilovatio();
            double totalDescuentosIncentivos = 0;

            foreach (Cliente cliente in clientes)
            {
                if (cliente.ConsumoEnergia < cliente.MetaAhorro)
                {
                    double valorIncentivo = (cliente.MetaAhorro - cliente.ConsumoEnergia) * valorKilovatio;
                    totalDescuentosIncentivos += valorIncentivo;
                }
            }
            return totalDescuentosIncentivos;
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

        private static double ObtenerValorKilovatio()
        {
            double valorKilovatio = 0;
            string query = "SELECT valorKilovatio FROM tarifas"; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    valorKilovatio = Convert.ToDouble(reader["valorKilovatio"]);
                }
            }
            return valorKilovatio;
        }

        public static Cliente ObtenerClienteMayorDiferencia()
        {
            Cliente clienteMayorDiferencia = null;
            double mayorDifencia = 0;

            string query = @"
        SELECT *
        FROM consumoEnergia
        WHERE consumoEnergia - metaAhorroEnergia > @MayorDiferencia";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MayorDiferencia", mayorDifencia);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clienteMayorDiferencia = new Cliente
                    {
                        ClienteId = Convert.ToInt32(reader["cedulaCliente"]),
                        
                    };
                }
            }

            return clienteMayorDiferencia;
        }

        public static Tuple<int, int> ObtenerEstratoMayorMenorConsumoEnergia()
        {
            int estratoMayorConsumo = 0;
            int estratoMenorConsumo = 0;

            string queryMayorConsumo = @"
        SELECT TOP 1 estratoCliente
        FROM clientes
        GROUP BY estratoCliente
        ORDER BY SUM(consumoEnergia) DESC";

            string queryMenorConsumo = @"
        SELECT TOP 1 estratoCliente
        FROM clientes
        GROUP BY estratoCliente
        ORDER BY SUM(consumoEnergia) ASC";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand commandMayorConsumo = new SqlCommand(queryMayorConsumo, connection);
                SqlCommand commandMenorConsumo = new SqlCommand(queryMenorConsumo, connection);
                connection.Open();
                SqlDataReader readerMayorConsumo = commandMayorConsumo.ExecuteReader();
                SqlDataReader readerMenorConsumo = commandMenorConsumo.ExecuteReader();

                if (readerMayorConsumo.Read())
                {
                    estratoMayorConsumo = Convert.ToInt32(readerMayorConsumo["estratoCliente"]);
                }

                if (readerMenorConsumo.Read())
                {
                    estratoMenorConsumo = Convert.ToInt32(readerMenorConsumo["estratoCliente"]);
                }
            }

            return Tuple.Create(estratoMayorConsumo, estratoMenorConsumo);
        }

        public static double CalcularValorTotalPagar()
        {
            double totalValorEnergia = 0;
            double totalValorAgua = 0;

           
            double valorKilovatio = 0;
            double valorM3Agua = 0;

            string queryTarifa = @"
        SELECT valorKilovatio, valorM3Agua
        FROM tarifa";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryTarifa, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    valorKilovatio = Convert.ToDouble(reader["valorKilovatio"]);
                    valorM3Agua = Convert.ToDouble(reader["valorM3Agua"]);
                }
            }

            

            string queryClientes = @"
        SELECT consumoEnergia, consumoAgua, metaAhorroEnergia, promedioConsumoAgua
        FROM clientes";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryClientes, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    double consumoEnergia = Convert.ToDouble(reader["ConsumoEnergia"]);
                    double consumoAgua = Convert.ToDouble(reader["ConsumoAgua"]);
                    double metaAhorro = Convert.ToDouble(reader["MetaAhorro"]);
                    double promedioAgua = Convert.ToDouble(reader["PromedioAgua"]);

                    double valorParcialEnergia = consumoEnergia * valorKilovatio;
                    double valorIncentivoEnergia = (metaAhorro - consumoEnergia) * valorKilovatio;
                    double valorPagarEnergia = valorParcialEnergia - valorIncentivoEnergia;

                    double valorPagarAgua;
                    if (consumoAgua > promedioAgua)
                    {
                        double excesoAgua = consumoAgua - promedioAgua;
                        double castigoExceso = excesoAgua * (2 * valorM3Agua);
                        double costoPromedio = promedioAgua * valorM3Agua;
                        valorPagarAgua = costoPromedio + castigoExceso;
                    }
                    else
                    {
                        valorPagarAgua = consumoAgua * valorM3Agua;
                    }

                    totalValorEnergia += valorPagarEnergia;
                    totalValorAgua += valorPagarAgua;
                }
            }

            return totalValorEnergia + totalValorAgua;
        }




    }




}
