using System;
using System.IO;
using Newtonsoft.Json.Linq;  // Corrigido para o namespace correto

namespace TesteHTML
{
    public static class Configuracao
    {
        public static string ObterStringConexao()
        {
            string caminhoConfig = "config.json";

            if (!File.Exists(caminhoConfig))
            {
                throw new FileNotFoundException("Arquivo config.json não encontrado!");
            }

            string json = File.ReadAllText(caminhoConfig);
            JObject obj = JObject.Parse(json);
            return obj["ConexaoMySQL"]?.ToString() ?? throw new Exception("Chave 'ConexaoMySQL' não encontrada no config.json");
        }
    }
}
