using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace TesteHTML
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServidorHTTP.Iniciar_Servidor();











            //string diretorioAtual = @"D:\Documents\ESTUDOS_LACERDA\Projetos\Dever de casa - Agezandro\TesteHTML";

            //Console.WriteLine("Diretório atual: " + diretorioAtual);

            //// Cria o caminho completo para o arquivo HTML (Page.html), unindo o diretório do projeto com o nome do arquivo
            //string pagina = Path.Combine(diretorioAtual, "Pagina_inicial.html");
            //Console.WriteLine($"Caminho da página: {pagina}");

            //if (File.Exists(pagina))
            //{
            //    // Se o arquivo existe, cria um novo processo e executa o arquivo HTML com o programa padrão do sistema (geralmente o navegador)
            //    Process.Start(new ProcessStartInfo
            //    {
            //        FileName = pagina, 
            //        UseShellExecute = true // Indica que o sistema deve usar o shell do sistema operacional para abrir o arquivo
            //    });
            //}   
            //else
            //{
            //    Console.WriteLine("Arquivo HTML não encontrado: " + pagina);
            //}
        }
    }
}
