﻿using Org.BouncyCastle.Ocsp;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace TesteHTML
{
    public class ServidorHTTP
    {
        private static string conexaoString = "server=localhost; database=banco_projeto_age; user=root; password=Glss;";
        private static string basePath = @"D:\Documents\ESTUDOS_LACERDA\Projetos\Dever de casa - Agezandro\TesteHTML";

        public static void Iniciar_Servidor()
        {
            HttpListener servidor = new HttpListener();
            servidor.Prefixes.Add("http://localhost:5000/");
            servidor.Start();
            Console.WriteLine("Servidor rodando em http://localhost:5000");

            while (true)
            {
                HttpListenerContext contexto = servidor.GetContext();
                HttpListenerRequest request = contexto.Request;
                HttpListenerResponse response = contexto.Response;

                string caminho = request.Url.AbsolutePath.TrimStart('/');
                string caminhoPagina = Path.Combine(basePath, caminho);

                Console.WriteLine($"🔎 Requisição recebida: {request.HttpMethod} {caminho}");

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
                        continue;
                    }

                    if (caminho == "Pagina2_Aluno.html")
                    {
                        Console.WriteLine("✅ Página de alunos requisitada! Gerando lista...");
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
                        Console.WriteLine("✅ Página de cursos requisitada! Gerando lista...");
                        string respostaTexto = Curso.PaginaCursos();

                        byte[] buffer = Encoding.UTF8.GetBytes(respostaTexto);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        response.OutputStream.Close();
                        continue;
                    }                   

                    if (File.Exists(caminhoPagina))
                    {
                        Console.WriteLine($"📄 Servindo arquivo: {caminhoPagina}");
                        string responseString = File.ReadAllText(caminhoPagina);
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        response.OutputStream.Close();
                        continue;
                    }

                    Console.WriteLine("❌ Página não encontrada.");
                    string errorMessage = "<html><body><h2>Página não encontrada</h2><button onclick=\"window.history.back();\">Voltar</button></body></html>";
                    byte[] errorBuffer = Encoding.UTF8.GetBytes(errorMessage);
                    response.ContentLength64 = errorBuffer.Length;
                    response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);
                }

                catch (Exception ex)
                {
                    // Exibe mensagem de erro no console
                    Console.WriteLine($"Erro: {ex.Message}");

                    // Prepara a mensagem de erro para resposta HTTP
                    string errorMessage = $"<html><body><h2>Erro ao processar requisição: {ex.Message}</h2></body></html>";
                    byte[] errorBuffer = Encoding.UTF8.GetBytes(errorMessage);

                    response.StatusCode = 500; // Define erro interno no servidor
                    response.ContentType = "text/html";

                    try
                    {
                        response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Erro ao enviar resposta: {e.Message}");
                    }
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
