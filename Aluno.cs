using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Text; 
using System.Threading.Tasks;  
using MySql.Data.MySqlClient;  
using System.Web; 
using System.IO;  

namespace TesteHTML  
{
    public class Aluno  
    {
        public int Id { get; set; }  
        public string Nome { get; set; }  

        private static string conexaoString = Configuracao.ObterStringConexao();  // String de conexão com o banco de dados
        private static string caminhoHTML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pagina2_Aluno.html"); // Arquivo HTML para exibir lista de alunos

        public Aluno() { }  

        public Aluno(int id, string nome)  
        {
            this.Id = id;
            this.Nome = nome;
        }

        // Cadastrar aluno no banco de dados
        public static void Cadastro(string dados)
        {
            var parametros = System.Web.HttpUtility.ParseQueryString(dados);  
            int id = int.Parse(parametros["id"]);  
            string nome = parametros["nome"];  

            Aluno aluno = new Aluno(id, nome); 
            aluno.SalvarBanco(); 
        }

        // Gerar página HTML com a lista de alunos
        public static string PaginaAlunos()
        {
            if (!File.Exists(caminhoHTML))  
            {
                return "<h2>Erro: Página não encontrada!</h2>"; 
            }

            string html = File.ReadAllText(caminhoHTML, Encoding.UTF8);  
            StringBuilder listaHtml = new StringBuilder();  

            List<Aluno> alunos = Aluno.ListarAlunos();  

            if (alunos.Count == 0)  
            {
                listaHtml.AppendLine("<li>Nenhum aluno cadastrado.</li>"); 
            }
            else 
            {
                foreach (Aluno aluno in alunos)  
                {
                    listaHtml.AppendLine($"<li>ID: {aluno.Id} - Nome: {aluno.Nome}</li>");  
                }
            }

            return html.Replace("<!--Alunos-->", $"<ul>{listaHtml}</ul>");  
        }

        // Listar todos os alunos cadastrados no banco de dados
        public static List<Aluno> ListarAlunos()
        {
            List<Aluno> alunos = new List<Aluno>();  

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(Configuracao.ObterStringConexao()))
                {
                    conexao.Open(); 
                    string query = "SELECT id, nome FROM alunos";  

                    using (MySqlCommand comando = new MySqlCommand(query, conexao))  
                    using (MySqlDataReader leitor = comando.ExecuteReader())  
                    {
                        while (leitor.Read())  
                        {
                            var aluno = new Aluno(leitor.GetInt32("id"), leitor.GetString("nome")); 
                            alunos.Add(aluno);  
                        }
                    }
                }
            }
            catch (Exception ex)  
            {
            }

            return alunos;  // Retorna a lista de alunos
        }

        // Salvar dados de um aluno no banco de dados
        public void SalvarBanco()
        {
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(Configuracao.ObterStringConexao()))  
                {
                    conexao.Open();  
                    string query = "INSERT INTO alunos (id, nome) VALUES (@id, @nome)";  

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