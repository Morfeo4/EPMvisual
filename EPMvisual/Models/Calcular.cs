using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
public class Tarifa
{
    [Key]
    public int TarifaId { get; set; }
    public double ValorKilovatio { get; set; }
    public double ValorM3Agua { get; set; }

    // profe este es el método para calcular el valor a pagar por el consumo de energía
    public double CalcularPagoEnergia(double consumoEnergia)
    {
        return consumoEnergia * ValorKilovatio;
    }

    // profe este es el método para calcular el valor a pagar por el consumo de agua
    public double CalcularPagoAgua(double consumoAgua, double promedioAgua)
    {
        if (consumoAgua > promedioAgua)
        {
            double excesoAgua = consumoAgua - promedioAgua;
            double castigoExceso = excesoAgua * (2 * ValorM3Agua);
            double costoPromedio = promedioAgua * ValorM3Agua;
            return costoPromedio + castigoExceso;
        }
        else
        {
            return consumoAgua * ValorM3Agua;
        }
    }
}

public class Consumo
{
    [Key]
    public int ConsumoId { get; set; }
    public double ConsumoAgua { get; set; }
    public double ConsumoEnergia { get; set; }
    public double PromedioAgua { get; set; }
    public double MetaAhorro { get; set; }

    // profe este es el método para calcular el valor total a pagar por el consumo de agua y energía
    public double CalcularPagoTotal(Tarifa tarifa)
    {
        double valorPagarEnergia = ConsumoEnergia * tarifa.ValorKilovatio;
        double valorIncentivo = (MetaAhorro - ConsumoEnergia) * tarifa.ValorKilovatio;
        double valorPagarAgua;
        if (ConsumoAgua > PromedioAgua)
        {
            double excesoAgua = ConsumoAgua - PromedioAgua;
            double castigoExceso = excesoAgua * (2 * tarifa.ValorM3Agua);
            double costoPromedio = PromedioAgua * tarifa.ValorM3Agua;
            valorPagarAgua = costoPromedio + castigoExceso;
        }
        else
        {
            valorPagarAgua = ConsumoAgua * tarifa.ValorM3Agua;
        }
        double valorServicios = valorPagarEnergia + valorPagarAgua - valorIncentivo;

        return valorServicios;
    }
}

public class CalculadoraServicios
{
    public List<double> CalcularPagoAguaEnergia(string cedulaCliente)
    {
        List<double> valoresServicios = new List<double>();

        string connectionString = "data source=DESKTOP-US30RFT;initial catalog=EPM;integrated security=True;trustservercertificate=True;";

        // profe estas son las variables para almacenar los valores de tarifas
        double valorKilovatio = 0;
        double valorM3Agua = 0;

        string queryTarifas = "SELECT valorKilovatio, valorM3Agua FROM tarifas";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(queryTarifas, connection);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                valorKilovatio = Convert.ToDouble(reader["valorKilovatio"]);
                valorM3Agua = Convert.ToDouble(reader["valorM3Agua"]);
            }

            reader.Close();
        }

        
        string queryConsumoEnergia = "SELECT consumoEnergia, metaAhorroEnergia FROM consumoEnergia WHERE cedulaCliente = @CedulaCliente";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(queryConsumoEnergia, connection);
            command.Parameters.AddWithValue("@CedulaCliente", cedulaCliente);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                double consumoEnergia = Convert.ToDouble(reader["consumoEnergia"]);
                double metaAhorro = Convert.ToDouble(reader["metaAhorroEnergia"]);

                
                string queryConsumoAgua = "SELECT consumoAgua, promedioConsumoAgua FROM consumoAgua WHERE cedulaCliente = @CedulaCliente";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(queryConsumoAgua, conn);
                    cmd.Parameters.AddWithValue("@CedulaCliente", cedulaCliente);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        double consumoAgua = Convert.ToDouble(rdr["consumoAgua"]);
                        double promedioAgua = Convert.ToDouble(rdr["promedioConsumoAgua"]);

                     
                        double valorPagarEnergia = consumoEnergia * valorKilovatio;
                        double valorIncentivo = (metaAhorro - consumoEnergia) * valorKilovatio;
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
                        double valorServicios = valorPagarEnergia + valorPagarAgua - valorIncentivo;

                        valoresServicios.Add(valorServicios);
                    }

                    rdr.Close();
                }
            }

            reader.Close();
        }

        return valoresServicios;
    }
}