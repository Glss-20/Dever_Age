using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Text;  
using MySql.Data.MySqlClient;  
using System.Web;  

namespace TesteHTML  
{
    public class Curso  
    {
        public int Id { get; set; }  
        public string Nome { get; set; }  

        private static string conexaoString = Configuracao.ObterStringConexao(); // String de conexão com o banco de dados
        private static string caminhoHTML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pagina2_Curso.html"); // Arquivo HTML para a lista de cursos

        public Curso() { }  

        public Curso(int id, string nome) 
        {
            this.Id = id;
            this.Nome = nome;
        }

        // Cadastrar curso no banco de dados
        public static void Cadastro(string dados)
        {
            var parametros = System.Web.HttpUtility.ParseQueryString(dados); 
            int id = int.Parse(parametros["id"]);  
            string nome = parametros["nome"];  

            Curso curso = new Curso(id, nome);  
            curso.SalvarBanco();  
        }

        // Gerar a página HTML com a lista de cursos
        public static string PaginaCursos()
        {
            if (!File.Exists(caminhoHTML))  
            {
                return "<h2>Erro: Página não encontrada!</h2>";  
            }

            string html = File.ReadAllText(caminhoHTML, Encoding.UTF8); // Lê o conteúdo do HTML
            StringBuilder listaHtml = new StringBuilder();  

            List<Curso> cursos = Curso.ListarCursos(); 

            if (cursos.Count == 0) 
            {
                listaHtml.AppendLine("<li>Nenhum curso cadastrado.</li>");  
            }
            else 
            {
                foreach (Curso curso in cursos)  
                {
                    listaHtml.AppendLine($"<li>ID: {curso.Id} - Nome: {curso.Nome}</li>"); 
                }
            }

            return html.Replace("<!--Cursos-->", $"<ul>{listaHtml}</ul>"); 
        }

        // Listar todos os cursos cadastrados no banco de dados
        public static List<Curso> ListarCursos()
        {
            List<Curso> cursos = new List<Curso>();  

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(Configuracao.ObterStringConexao())) // Nova conexão com o banco de dados
                {
                    conexao.Open();  
                    string query = "SELECT id, nome FROM cursos";  

                    using (MySqlCommand comando = new MySqlCommand(query, conexao)) // Comando SQL
                    using (MySqlDataReader leitor = comando.ExecuteReader())  // Executa a consulta e obtém os dados
                    {
                        while (leitor.Read())  
                        {
                            var curso = new Curso(leitor.GetInt32("id"), leitor.GetString("nome")); 
                            cursos.Add(curso); 
                        }
                    }
                }
            }
            catch (Exception ex)  
            {
                
            }

            return cursos; // Retorna a lista de cursos
        }

        // Salvar dados do curso no banco de dados
        public void SalvarBanco()
        {
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(Configuracao.ObterStringConexao())) 
                {
                    conexao.Open(); 
                    string query = "INSERT INTO cursos (id, nome) VALUES (@id, @nome)";  

                    using (MySqlCommand comando = new MySqlCommand(query, conexao))  
                    {
                        comando.Parameters.AddWithValue("@id", Id);  
                        comando.Parameters.AddWithValue("@nome", Nome);  
                        comando.ExecuteNonQuery();  
                    }
                }
            }
            catch (Exception ex)  
            {
               
            }
        }
    }
}