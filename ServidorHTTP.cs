using Org.BouncyCastle.Ocsp;  
using System;  
using System.IO; 
using System.Net;  
using System.Text;  
using ZstdSharp.Unsafe;  
using System.Diagnostics;  

namespace TesteHTML  
{
    public class ServidorHTTP 
    {
        private static string conexaoString = Configuracao.ObterStringConexao();  // String de conexão do banco de dados 
        private static string basePath = AppDomain.CurrentDomain.BaseDirectory;  // Diretório base 

        public static void Iniciar_Servidor()  // Iniciar o servidor HTTP
        {
            HttpListener servidor = new HttpListener();  // Instância do servidor HTTP
            servidor.Prefixes.Add("http://localhost:5000/");  // Prefixo do servidor, onde será feito as requisições
            servidor.Start();

            
            Process.Start(new ProcessStartInfo
            {
                FileName = "http://localhost:5000/Pagina_inicial.html",  
                UseShellExecute = true  
            });

            while (true)  
            {
                HttpListenerContext contexto = servidor.GetContext();  
                HttpListenerRequest request = contexto.Request;  
                HttpListenerResponse response = contexto.Response;  

                string caminho = request.Url.AbsolutePath.TrimStart('/');  
                string caminhoPagina = Path.Combine(basePath, caminho);  // Diretório base + página solicitada
                string caminhoConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");  

                try  
                {
                    if (request.HttpMethod == "POST" && caminho == "cadastrar") 
                    {
                        
                        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string dados = reader.ReadToEnd(); 
                            Aluno.Cadastro(dados);  
                        }

                       
                        string responseString = "<html><body><h2>Cadastro realizado com sucesso!</h2><button onclick=\"window.history.back();\">Voltar</button></body></html>";
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);  
                        response.ContentLength64 = buffer.Length;  
                        response.OutputStream.Write(buffer, 0, buffer.Length);  
                        response.OutputStream.Close();  
                        continue;  // Continua para aguardar nova requisição
                    }

                    if (caminho == "Pagina2_Aluno.html")
                    {
                        string respostaTexto = Aluno.PaginaAlunos(); 

                        byte[] buffer = Encoding.UTF8.GetBytes(respostaTexto);  
                        response.ContentLength64 = buffer.Length;  
                        response.OutputStream.Write(buffer, 0, buffer.Length); 
                        response.OutputStream.Close();  
                        continue;  
                    }

                    else if (request.HttpMethod == "POST" && caminho == "cadastrar_curso")  
                    {
                        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))  
                        {
                            string dados = reader.ReadToEnd();  
                            Curso.Cadastro(dados);  
                        }

                        
                        string responseString = "<html><body><h2>Curso cadastrado com sucesso!</h2><button onclick=\"window.history.back();\">Voltar</button></body></html>";
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);  
                        response.ContentLength64 = buffer.Length;  
                        response.OutputStream.Write(buffer, 0, buffer.Length); 
                        response.OutputStream.Close();  
                    }

                    else if (caminho == "Pagina2_Curso.html") 
                    {
                        string respostaTexto = Curso.PaginaCursos();  

                        byte[] buffer = Encoding.UTF8.GetBytes(respostaTexto);  
                        response.ContentLength64 = buffer.Length;  
                        response.OutputStream.Write(buffer, 0, buffer.Length);  
                        response.OutputStream.Close(); 
                        continue;  
                    }

                    
                    if (File.Exists(caminhoPagina))
                    {
                        string responseString = File.ReadAllText(caminhoPagina);  
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);  
                        response.ContentLength64 = buffer.Length;  
                        response.OutputStream.Write(buffer, 0, buffer.Length);  
                        response.OutputStream.Close();  
                        continue; 
                    }

                  
                    string caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, caminho);

                    if (File.Exists(caminhoArquivo))
                    {
                        byte[] buffer = File.ReadAllBytes(caminhoArquivo);  
                        response.ContentType = "text/html";  
                        response.ContentLength64 = buffer.Length;  
                        response.OutputStream.Write(buffer, 0, buffer.Length);  
                        response.OutputStream.Close();  
                        continue;  
                    }

                   
                    string errorMessage = "<html><body><h2>Página não encontrada</h2><button onclick=\"window.history.back();\">Voltar</button></body></html>";
                    byte[] errorBuffer = Encoding.UTF8.GetBytes(errorMessage);  
                    response.ContentLength64 = errorBuffer.Length;  
                    response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);  
                }
                catch (Exception ex)  
                {
                    
                    string errorMessage = $"<html><body><h2>Erro ao processar requisição: {ex.Message}</h2></body></html>";
                    byte[] errorBuffer = Encoding.UTF8.GetBytes(errorMessage);  
                    response.StatusCode = 500;  
                    response.ContentType = "text/html";  

                    try
                    {
                        response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);  
                    }
                    catch (Exception e) { }  
                    finally
                    {
                        response.OutputStream.Close();  
                    }
                }
                finally
                {
                    response.Close();  
                }
            }
        }
    }
}