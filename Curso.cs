using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
//using System.Net.HttpUtility;

namespace TesteHTML
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        private static string conexaoString = Configuracao.ObterStringConexao();
        private static string caminhoHTML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pagina2_Curso.html");

        public Curso() { }

        public Curso(int id, string nome)
        {
            this.Id = id;
            this.Nome = nome;
        }

        public static void Cadastro(string dados)
        {
            var parametros = System.Web.HttpUtility.ParseQueryString(dados);
            int id = int.Parse(parametros["id"]);
            string nome = parametros["nome"];

            Curso curso = new Curso(id, nome);
            curso.ExibirCadastro();
            curso.SalvarBanco();
        }

        public static string PaginaCursos()
        {
            if (!File.Exists(caminhoHTML))
            {
                return "<h2>Erro: Página não encontrada!</h2>";
            }

            string html = File.ReadAllText(caminhoHTML, Encoding.UTF8);
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

        public static List<Curso> ListarCursos()
        {
            List<Curso> cursos = new List<Curso>();

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(Configuracao.ObterStringConexao()))
                {
                    conexao.Open();
                    string query = "SELECT id, nome FROM cursos";

                    using (MySqlCommand comando = new MySqlCommand(query, conexao))
                    using (MySqlDataReader leitor = comando.ExecuteReader())
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
                Console.WriteLine("Erro ao buscar cursos: " + ex.Message);
            }

            return cursos;
        }

        public void ExibirCadastro()
        {
            Console.WriteLine($"ID = {Id} - Nome = {Nome}");
        }

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
                Console.WriteLine("Curso cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar no banco: {ex.Message}");
            }
        }
    }
}
