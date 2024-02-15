using System;
using System.Net;
using Newtonsoft.Json.Linq;

public class Program
{
    public static double GetExchangeRate(string fromCurrency, string toCurrency)
    {
        string url = $"https://api.exchangeratesapi.io/latest?base={fromCurrency}&symbols={toCurrency}";

        using (WebClient web = new WebClient())
        {
            try
            {
                string json = web.DownloadString(url);
                JObject obj = JObject.Parse(json);

                if (obj.TryGetValue("rates", out JToken ratesToken))
                {
                    if (ratesToken is JObject rates)
                    {
                        if (rates.TryGetValue(toCurrency, out JToken rateToken))
                        {
                            return (double)rateToken;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Se houver algum erro ao obter a taxa de câmbio, retornar -1
                // para indicar que a taxa de câmbio não foi encontrada
                return -1;
            }

            // Se a moeda de destino não estiver presente nas taxas de câmbio,
            // retornar -1 para indicar que a taxa de câmbio não foi encontrada
            return -1;
        }
    }

    public static double ConvertCurrency(double amount, double rate)
    {
        return amount * rate;
    }

    public static void Main(string[] args)
    {
        bool continuar = true;
        while (continuar)
        {
            Console.WriteLine("Conversor de Moeda");
            Console.Write("Digite o valor a ser convertido (ou digite '0' para sair): ");
            double amount;
            if (!double.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine("Valor inválido. Tente novamente.");
                continue;
            }

            if (amount == 0)
            {
                continuar = false;
                break;
            }

            Console.Write("Digite a moeda de origem (ex: BRL): ");
            string fromCurrency = Console.ReadLine().ToUpper();

            Console.Write("Digite a moeda de destino (ex: EUR): ");
            string toCurrency = Console.ReadLine().ToUpper();

            double exchangeRate = GetExchangeRate(fromCurrency, toCurrency);

            // Verificar se a taxa de câmbio é válida
            if (exchangeRate == -1)
            {
                Console.WriteLine("Taxa de câmbio não encontrada para a moeda especificada.");
                continue; // Volta para o início do loop para permitir que o usuário insira novos valores
            }

            double convertedAmount = ConvertCurrency(amount, exchangeRate);

            Console.WriteLine($"{amount} {fromCurrency} equivale a {convertedAmount} {toCurrency}");

            Console.Write("Deseja converter outra quantia? (Sim/Não): ");
            string resposta = Console.ReadLine().ToUpper();
            continuar = (resposta == "SIM");
        }

        Console.WriteLine("Obrigado por usar o conversor de moedas!");
    }
}



